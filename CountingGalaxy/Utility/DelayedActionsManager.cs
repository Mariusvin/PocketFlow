using System;
using System.Collections.Generic;

namespace Utility
{
    public class DelayedActionsManager : MonoSingleton<DelayedActionsManager>
    {
        private bool isEmpty = true;
        private readonly Dictionary<Guid, DelayedActionData> delayedActions = new();
    
        private void Update()
        {
            if (isEmpty)
            {
                return;
            }
            
            ProcessDelayedActions();
            isEmpty = delayedActions.Count == 0;
        }
    
        public static Guid DelayedCall(Action _action, int _frames)
        {
            Guid _id = Guid.NewGuid();
            Instance.delayedActions[_id] = new DelayedActionData(_action, _frames);
            Instance.isEmpty = false;
            return _id;
        }
    
        public static bool TryCancelCall(Guid _id)
        {
            return Instance.delayedActions.Remove(_id);
        }
        
        private void ProcessDelayedActions()
        {
            List<Guid> _executables = new();
            // Collect actions that are ready to be executed
            foreach (KeyValuePair<Guid, DelayedActionData> _kvp in delayedActions)
            {
                Guid _id = _kvp.Key;
                DelayedActionData _data = _kvp.Value;
                _data.remainingFrames--;
                if (_data.remainingFrames <= 0)
                {
                    _executables.Add(_id);
                }
            }

            // Execute and remove
            foreach (Guid _id in _executables)
            {
                delayedActions[_id].Action?.Invoke();
                delayedActions.Remove(_id);
            }
        }
        
        private struct DelayedActionData
        {
            public readonly Action Action;
            public int remainingFrames;
        
            public DelayedActionData(Action _action, int _frames)
            {
                Action = _action;
                remainingFrames = _frames;
            }
        }
    }
}