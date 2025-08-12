using System;

namespace Utility.Cheats
{
    public class Cheat
    {
        private event Action OnButtonClick;
        private event Action<bool> OnCheatToggle;

        private readonly CheatName name;
        private CheatState currentState;

        public string Name => name + State;
        
        private string State => currentState == CheatState.None ? "" : $" [{currentState.ToString()}]";

        public Cheat(CheatName _name, Action _onButtonClick)
        {
            name = _name;
            OnButtonClick += _onButtonClick;
            currentState = CheatState.None;
        }

        public Cheat(CheatName _name, Action<bool> _onButtonClick, CheatState _initState)
        {
            name = _name;
            OnCheatToggle += _onButtonClick;
            currentState = _initState;
        }
        
        public void Activate()
        {
            if (currentState == CheatState.None)
            {
                OnButtonClick?.Invoke();
            }
            else
            {
                currentState = currentState == CheatState.On ? CheatState.Off : CheatState.On;
                OnCheatToggle?.Invoke(currentState == CheatState.On);
            }
        }
    }
}