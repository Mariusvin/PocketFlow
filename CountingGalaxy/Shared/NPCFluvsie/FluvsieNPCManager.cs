using System;
using System.Collections.Generic;
using SpineAnimations.Fluvsie;
using UnityEngine;

namespace Activities.Shared.NPCFluvsie
{
    public class FluvsieNPCManager : MonoBehaviour
    {
        [SerializeField] private List<FluvsieNPCAnimator> characters;

        public static readonly FluvsieSkins MAIN_CHARACTER = FluvsieSkins.MidnightFluff;
        
        public void Initialize()
        {
            InitSkins();
        }
        
        public void Initialize(List<FluvsieNPCAnimator> _characters)
        {
            characters = _characters;
            InitSkins();
        }
        
        public void PlayReaction(NPCResponseType _responseType)
        {
            foreach (FluvsieNPCAnimator _npc in characters)
            {
                _npc.PlayReaction(_responseType);
            }
        }

        private void InitSkins()
        {
            List<FluvsieSkins> _enumValues = new((FluvsieSkins[])Enum.GetValues(typeof(FluvsieSkins)));
            _enumValues.Remove(MAIN_CHARACTER);
            
            foreach (FluvsieNPCAnimator _npc in characters)
            {
                _npc.Initialize();
                if (_npc.RandomizeSkin)
                {
                    FluvsieSkins _rndSkin = _enumValues[UnityEngine.Random.Range(0, _enumValues.Count)];
                    _npc.SetSkin(_rndSkin);
                    _enumValues.Remove(_rndSkin);
                }
            }
        }
    }
}
