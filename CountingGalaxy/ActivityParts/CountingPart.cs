using System;
using System.Collections.Generic;
using Activities.Shared.Architecture;
using Activities.Shared.Restaurant;
using PrimeTween;
using UnityEngine;
using Utility;
using Utility.Extensions;
using Utility.Sprites;
using Random = UnityEngine.Random;

namespace Activities.CountingGalaxy.ActivityParts
{
    public class CountingPart : ActivityPartBase<CountingGalaxyPartName>
    {
        [Header("--- Counting Part ---")] [Header("Self references")] [SerializeField]
        private Transform centerObjectParent;

        [SerializeField] private SpriteRenderer centerObjectRenderer;
        [SerializeField] private SpriteRenderer centerObjectFillRenderer;
        [SerializeField] private SpriteRenderer centerObjectShine;
        [SerializeField] private SpriteRenderer eyesRenderer;
        [SerializeField] private SpriteRenderer mouthRenderer;
        [SerializeField] private SpriteShaderAtlasHelper spriteShaderAtlasHelper;

        [Header("Project references")] [SerializeField]
        private List<Sprite> numbers;

        [SerializeField] private NumberObject numberObjectPrefab;
        [SerializeField] private Sprite defaultMouthSprite;
        [SerializeField] private Sprite defaultEyesSprite;
        [SerializeField] private Sprite happyMouthSprite;
        [SerializeField] private Sprite happyEyesSprite;

        private const float DISTRIBUTION_JITTER = 0.4f;
        private const float MIN_X_RADIUS = 3f;
        private const float MAX_X_RADIUS = 6f;
        private const float MIN_Y_RADIUS = 3f;
        private const float MAX_Y_RADIUS = 4f;
        private const float SMALLEST_SCALE = 0.5f;
        private const float SCATTER_DURATION = 0.15f;
        private const float REACH_CENTER_DURATION = 0.65f;
        private const float SCATTER_RANDOM = 0.2f;
        private const float SCATTER_DELAY_FOR_OBJECT = 0.05f;
        private const float NUMBER_SCALE_DURATION = 0.3f;
        private const float NUMBER_SCALE_DELAY = 0.2f;
        private const float RANDOM_ROTATION = 20f;
        private const float PULSATE_STRENGTH = 0.1f;
        private const float PULSATE_DURATION = 0.5f;
        private const float PULSATE_FREQ = 2f;
        private const float PART_COMPLETION_DELAY = 2f;
        private static readonly int FILL_AMOUNT = Shader.PropertyToID("_FillAmount");

        private Dictionary<NumberObject, List<Vector3>> paths;
        private Dictionary<NumberObject, float> scaleFactors;
        private List<NumberObject> numberObjects;
        private List<Sprite> numberVisuals;
        private int currentNumber;
        private int visualIndex;
        private bool initDataSet;
        private Vector2 xMinMax;
        private Vector2 yMinMax;
        private Material fillMaterial;
        private Sequence happyFaceSequence;
        private Tween pulsateTween;
        private Tween fillTween;
        private Tween shineTween;
        private ShakeSettings punchAlphaSettings;

        private int NumberOfObjectsToSpawn { get; set; } = 10;

        public override void Begin(Action<ActivityPartBase<CountingGalaxyPartName>> _onPartCompleted)
        {
            print("CountingPart Begin");
            base.Begin(_onPartCompleted);
            if (!initDataSet)
            {
                Debug.LogError("Initial data not set. Call SetInitialData() before starting the part.");
                Complete();
                return;
            }

            paths = new Dictionary<NumberObject, List<Vector3>>();
            xMinMax = new Vector2(MIN_X_RADIUS, MAX_X_RADIUS);
            yMinMax = new Vector2(MIN_Y_RADIUS, MAX_Y_RADIUS);
            currentNumber = 1;
            centerObjectParent.transform.localScale = Vector3.one * SMALLEST_SCALE;
            SpawnNumberObjects();
            DistributeObjectsAroundCenter();
            GeneratePathForObjects();
            PrepareForScatter();
            ScatterOnPath(ExposeNumbers);
        }

        public void SetInitialData(int _number, Sprite _centerObject, Sprite _centerObjectShine, List<Sprite> _sprites)
        {
            fillMaterial = new Material(centerObjectFillRenderer.material);
            fillMaterial.SetFloat(FILL_AMOUNT, 0f);
            centerObjectFillRenderer.material = fillMaterial;
            centerObjectRenderer.sprite = _centerObject;
            centerObjectFillRenderer.sprite = _centerObject;
            centerObjectShine.sprite = _centerObjectShine;
            NumberOfObjectsToSpawn = _number;
            numberVisuals = _sprites;
            initDataSet = true;
            punchAlphaSettings = new ShakeSettings
            {
                strength = Vector3.one,
                duration = PULSATE_DURATION,
                frequency = PULSATE_FREQ
            };

            spriteShaderAtlasHelper.ApplyUV();
            UpdateShineAlpha(0f);
        }

        private void SpawnNumberObjects()
        {
            numberObjects = new List<NumberObject>();
            scaleFactors = new Dictionary<NumberObject, float>();
            for (int i = 0; i < NumberOfObjectsToSpawn; i++)
            {
                NumberObject _numObject = Instantiate(numberObjectPrefab, partObject.transform);
                _numObject.Setup(i + 1, GetObjectVisual(), numbers[i + 1]); // Assign a number and visual (null for now)
                _numObject.OnNumberClicked += HandleNumberObjectClicked;
                _numObject.DisableFloating();
                numberObjects.Add(_numObject);
                scaleFactors.Add(_numObject, Mathf.Lerp(SMALLEST_SCALE, 1f, (i + 1f) / NumberOfObjectsToSpawn));
            }
        }

        private void DistributeObjectsAroundCenter()
        {
            List<Transform> _objTransforms = numberObjects.ConvertAll(_object => _object.transform);
            _objTransforms.Shuffle();
            ObjectsDistributor.DistributeInSunflowerSpiral(_objTransforms, Vector3.zero, xMinMax, yMinMax,
                DISTRIBUTION_JITTER);
        }

        private void GeneratePathForObjects()
        {
            Vector3 _origin = Vector3.zero;
            foreach (NumberObject _numberObject in numberObjects)
            {
                List<Vector3> _path = new();
                Vector3 _endPoint = _numberObject.transform.position;
                BezierControlPoints _controlPoints =
                    Bezier.GetAsymmetricSCurveControlPoints(_origin, _endPoint, 0.1f, 0.2f);
                _path.Add(_origin);
                _path.Add(_controlPoints.controlPoint1);
                _path.Add(_controlPoints.controlPoint2);
                _path.Add(_endPoint);
                paths.Add(_numberObject, _path);
                for (int index = 0; index < _path.Count; index++)
                {
                    Vector3 _point = _path[index];
                    // debug path connections
                    if (index < _path.Count - 1)
                    {
                        Debug.DrawLine(_point, _path[index + 1], Color.green, 999f);
                    }
                }
            }
        }

        private void PrepareForScatter()
        {
            foreach (NumberObject _numberObject in numberObjects)
            {
                Transform _t = _numberObject.transform;
                _t.localScale = Vector3.zero;
                _t.position = Vector3.zero;
                _numberObject.IsInteractive = false;
            }
        }

        private void ScatterOnPath(Action _onComplete)
        {
            int _reached = 0;
            Sequence _seq = Sequence.Create();
            foreach (NumberObject _numberObject in numberObjects)
            {
                _seq.ChainCallback(target: this, _target => _target.ScatterObject(_numberObject, CountCompleted));
                //_seq.ChainDelay(SCATTER_DELAY_FOR_OBJECT);
            }

            // Local method
            void CountCompleted()
            {
                _reached++;
                if (_reached == numberObjects.Count)
                {
                    _onComplete?.Invoke();
                }
            }
        }

        private void ScatterObject(NumberObject _numberObject, Action _onComplete)
        {
            float _duration = SCATTER_DURATION;//+ Random.Range(-SCATTER_RANDOM, SCATTER_RANDOM);
            _numberObject.FollowPath(paths[_numberObject], _duration, HandleDestinationReached);
            _numberObject.ScaleTo(scaleFactors[_numberObject], _duration, Ease.OutBack);
            _numberObject.RotateTo(Random.Range(-RANDOM_ROTATION, RANDOM_ROTATION), _duration);

            // Local method
            void HandleDestinationReached()
            {
                _onComplete?.Invoke();
                _numberObject.CacheScale();
                _numberObject.EnableFloating();
                _numberObject.IsInteractive = true;
            }
        }

        private void ExposeNumbers()
        {
            Sequence _seq = Sequence.Create();
            foreach (NumberObject _numberObject in numberObjects)
            {
                _seq.ChainCallback(target: _numberObject, _target => _target.UpscaleNumber(NUMBER_SCALE_DURATION));
                _seq.ChainDelay(NUMBER_SCALE_DELAY);
            }
        }

        private Sprite GetObjectVisual()
        {
            if (numberVisuals.Count == 0)
            {
                Debug.LogError("No visuals available for numbers. Returning null.");
                return null;
            }

            if (numberVisuals.Count <= visualIndex)
            {
                visualIndex = 0;
            }

            return numberVisuals[visualIndex++];
        }

        private void HandleNumberObjectClicked(NumberObject _number)
        {
            if (_number.AssignedNumber != currentNumber)
            {
                return; // Clicked the wrong number
            }

            currentNumber++;
            Action _onReachActions = null;
            _onReachActions += PulsateFill;
            _onReachActions += UpdateFill;

            if (currentNumber > NumberOfObjectsToSpawn)
            {
                _onReachActions += ShineForever;
                _onReachActions += SetFaceHappy;
                _onReachActions += ShowEndingSequence;
            }
            else
            {
                _onReachActions += PulsateShine;
                _onReachActions += SetFaceHappyTemporary;
            }

            MoveObjectToCenter(_number, _onReachActions);
        }

        private void MoveObjectToCenter(NumberObject _numberObject, Action _onReach)
        {
            List<Vector3> _reversedPath = paths[_numberObject];
            _reversedPath.Reverse();
            _numberObject.IsInteractive = false;
            _numberObject.DisableFloating();
            _numberObject.ScaleTo(0f, REACH_CENTER_DURATION, Ease.InBack);
            _numberObject.RotateTo(0f, REACH_CENTER_DURATION);
            _numberObject.FollowPath(_reversedPath, REACH_CENTER_DURATION, _onReach);
        }

        // pulsate and spin multiple times
        private void ShowEndingSequence()
        {
            const int SPIN_COUNT = 3;
            const float SPIN_DURATION = 0.5f;
            const float PULSATE_STR = 1.1f;
            Vector3 _rot = new Vector3(0f, 0f, 360f);

            if (!centerObjectParent)
            {
                Debug.LogError(
                    "CenterObjectParent is null in ShowEndingSequence. Cannot play ending sequence. Completing part.");
                Complete();
                return;
            }

            Sequence endingSeq = Sequence.Create();
            for (int i = 0; i < SPIN_COUNT; i++)
            {
                endingSeq.Chain(Tween.PunchScale(centerObjectParent, Vector3.one * PULSATE_STR, SPIN_DURATION, PULSATE_FREQ));
                endingSeq.Group(Tween.LocalEulerAngles(centerObjectParent, Vector3.zero, _rot, SPIN_DURATION, Ease.InOutSine));
            }

            endingSeq.ChainCallback(target: this, _ => Complete());
        }

        private void PulsateFill()
        {
            pulsateTween.Stop();
            centerObjectParent.localScale =
                Vector3.one * Mathf.Lerp(SMALLEST_SCALE, 1f, (currentNumber - 1f) / NumberOfObjectsToSpawn);
            pulsateTween = Tween.PunchScale(centerObjectParent.transform, Vector3.one * PULSATE_STRENGTH,
                PULSATE_DURATION, PULSATE_FREQ);
        }

        private void PulsateShine()
        {
            shineTween.Stop();
            shineTween = Tween.PunchCustom(centerObjectShine.transform, Vector3.zero, punchAlphaSettings, UpdateShineAlphaPunch);
            
            // Local method
            void UpdateShineAlphaPunch(Transform _target, Vector3 _frac)
            {
                UpdateShineAlpha(_frac.x);
            }
        }

        private void ShineForever()
        {
            shineTween.Stop();
            shineTween = Tween.Custom(0f, 1f, PULSATE_DURATION, UpdateShineAlpha, Ease.InSine);
        }

        private void UpdateShineAlpha(float _target)
        {
            Color _color = centerObjectShine.color;
            _color.a = Mathf.Lerp(0f, 1f, _target);
            centerObjectShine.color = _color;
        }

        private void UpdateFill()
        {
            float _currentFill = fillMaterial.GetFloat(FILL_AMOUNT);
            fillTween.Stop();
            fillTween = Tween.Custom(0f, 1f, PULSATE_DURATION, UpdateFillFrac);

            // Local method
            void UpdateFillFrac(float _frac)
            {
                float _fillAmount = Mathf.Lerp(_currentFill,
                    Mathf.Clamp01((currentNumber - 1) / (float)NumberOfObjectsToSpawn), _frac);
                fillMaterial.SetFloat(FILL_AMOUNT, _fillAmount);
            }
        }

        private void SetFaceHappyTemporary()
        {
            happyFaceSequence.Stop();
            happyFaceSequence = Sequence.Create();
            happyFaceSequence.ChainCallback(SetFaceHappy);
            happyFaceSequence.ChainDelay(PULSATE_DURATION);
            happyFaceSequence.ChainCallback(SetFaceDefault);
        }

        private void SetFaceHappy()
        {
            eyesRenderer.sprite = happyEyesSprite;
            mouthRenderer.sprite = happyMouthSprite;
        }

        private void SetFaceDefault()
        {
            eyesRenderer.sprite = defaultEyesSprite;
            mouthRenderer.sprite = defaultMouthSprite;
        }
    }
}
