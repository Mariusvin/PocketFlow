using System.Collections;
using Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

namespace Utility
{
    public class SceneLoader : MonoSingleton<SceneLoader>
    {
        private const float LOAD_THRESHOLD = 0.9f;

        private Scenes previousScene;
        
        public static string GetCurrentSceneName => SceneManager.GetActiveScene().name;

        public static int GetCurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;
        

        protected override void ValidAwake()
        {
            base.ValidAwake();
            previousScene = Scenes.LandingScreen;
            DontDestroyOnLoad(gameObject);
        }

        public static void LoadSceneByIndex(int _sceneIndex)
        {
            Instance.TrackTimeSpentInPreviousScene();
            Instance.SetPreviousScene(_sceneIndex);
            Instance.LoadScene(_sceneIndex);
        }

        public static void ReloadCurrentScene()
        {
            Scene _currentScene = SceneManager.GetActiveScene();
            AnalyticsTracker.TrackPageViewEvent(_currentScene.name);
            Instance.LoadScene(_currentScene.buildIndex);
        }
        
        private void LoadScene(int _sceneIndex)
        {
            // TODO make it safe
            StartCoroutine(LoadSceneAsync(_sceneIndex));
        }

        private IEnumerator LoadSceneAsync(int _sceneIndex)
        {
            AsyncOperation _op = SceneManager.LoadSceneAsync(_sceneIndex);
            Debug.Assert(_op != null, nameof(_op) + " != null");
            _op.allowSceneActivation = false;
            while (!_op.isDone)
            {
                if (_op.progress >= LOAD_THRESHOLD)
                {
                    _op.allowSceneActivation = true;
                }
                
                yield return null;
            }
        }

        private void SetPreviousScene(int _sceneIndex)
        {
            previousScene = (Scenes)_sceneIndex;
        }

        private void TrackTimeSpentInPreviousScene()
        {
            AnalyticsTracker.TrackPageViewEvent(previousScene.ToString());
        }
    }
}
