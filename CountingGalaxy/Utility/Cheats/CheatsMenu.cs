using System;
using System.Collections.Generic;
using TutoTOONS.Utils.Debug.Console;
using UnityEngine;
using Utility.Extensions;

namespace Utility.Cheats
{
    public class CheatsMenu : MonoSingleton<CheatsMenu>
    {
        private const float BOTTOM_PADDING = 0.1f;
        private const float HEIGHT_PADDING = 0.2f;
        private const float BUTTON_TOP_BOTTOM_PADDING = 0.6f;
        private const float BUTTON_LEFT_RIGHT_PADDING = 0.17f;
        private const string GUI_STYLE_NAME = "Button";

        private Dictionary<CheatName, Cheat> cheatsDictionary = new();
        private GUIStyle buttonStyle;
        private Rect defaultRect;
        private bool isDebugConsoleOpened;
        private bool isInitialized;
        private bool isHidden = true;

        private int CheatsCount => cheatsDictionary.Count;

        protected override void ValidAwake()
        {
            base.ValidAwake();
            DontDestroyOnLoad(gameObject);
            Initialize();
            if (DebugConsole.console_open)
            {
                isDebugConsoleOpened = true;
            }
        }

        private void OnEnable()
        {
            DebugConsole.OnShow += SetDebugConsoleOpened;
            DebugConsole.OnHide += SetDebugConsoleHidden;
        }

        private void OnDisable()
        {
            DebugConsole.OnShow -= SetDebugConsoleOpened;
            DebugConsole.OnHide -= SetDebugConsoleHidden;
        }

        private void OnGUI()
        {
            if (!isInitialized)
            {
                Initialize();
            }
            
            if (!isDebugConsoleOpened)
            {
                return;
            }

            InitializeButtonGUIStyle();
            DisplayShowCheatsButton();
            if (!isHidden)
            {
                DropDownCheatsList();
            }
        }
        
        // TODO: Implement scene / global cheats
        public static void RegisterCheat(CheatName _name, Action _onCheatButtonClick)
        {
            Instance.RegisterCheatAction(_name, _onCheatButtonClick);
        }

        public static void RegisterCheatToggle(CheatName _name, Action<bool> _onCheatButtonClick, bool _isToggled)
        {
            Instance.RegisterCheatToggle(_name, _onCheatButtonClick, _isToggled ? CheatState.On : CheatState.Off);
        }
        
        private void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            
            isInitialized = true;
            cheatsDictionary = new();
            RegisterCheatToggle(CheatName.Switch, ChangeCheatsState, CheatState.Off);
        }

        private void RegisterCheatAction(CheatName _name, Action _onCheatButtonClick)
        {
            cheatsDictionary.Add(_name, new Cheat(_name, _onCheatButtonClick));
        }
        
        private void RegisterCheatToggle(CheatName _name, Action<bool> _onCheatButtonClick, CheatState _initState)
        {
            cheatsDictionary.Add(_name, new Cheat(_name, _onCheatButtonClick, _initState));
        }

        private void SetDebugConsoleOpened()
        {
            if (!DebugConsole.instance)
            {
                return;
            }

            isDebugConsoleOpened = true;
        }

        private void SetDebugConsoleHidden()
        {
            if (!DebugConsole.instance)
            {
                return;
            }

            isDebugConsoleOpened = false;
        }

        private void InitializeButtonGUIStyle()
        {
            float _bottomPadding = ScreenExtensions.Height * BOTTOM_PADDING;
            float _height = Mathf.Min((ScreenExtensions.Height - _bottomPadding) / CheatsCount, ScreenExtensions.Height * HEIGHT_PADDING);

            buttonStyle = new GUIStyle(GUI_STYLE_NAME)
            {
                fixedWidth = ScreenExtensions.Width * BUTTON_LEFT_RIGHT_PADDING,
                fixedHeight = _height * BUTTON_TOP_BOTTOM_PADDING,
                wordWrap = true,
                fontSize = 20,
                clipping = TextClipping.Clip
            };
            defaultRect = new Rect(ScreenExtensions.Width - buttonStyle.fixedWidth, ScreenExtensions.Height, buttonStyle.fixedWidth, _height);
        }

        // Open or close cheats menu button
        private void DisplayShowCheatsButton()
        {
            Cheat _changeStateCheat = cheatsDictionary[CheatName.Switch];
            Rect _rect = new(defaultRect);
            _rect.y -= _rect.height * CheatsCount + Screen.height * BOTTOM_PADDING;
            if (GUI.Button(_rect, _changeStateCheat.Name, buttonStyle))
            {
                _changeStateCheat.Activate();
            }
        }

        // Display drop down cheats list with names
        private void DropDownCheatsList()
        {
            foreach (CheatName _cheatName in Enum.GetValues(typeof(CheatName)))
            {
                if(!cheatsDictionary.ContainsKey(_cheatName) || _cheatName == CheatName.Switch)
                {
                    continue;
                }
                
                Rect _rect = new(defaultRect);
                _rect.y -= _rect.height * (CheatsCount - 1) + ScreenExtensions.Height * BOTTOM_PADDING;
                Cheat _cheat = cheatsDictionary.GetValueOrDefault(_cheatName);
                if (GUI.Button(_rect, _cheat.Name, buttonStyle))
                {
                    _cheat.Activate();
                }
            }
        }

        private void ChangeCheatsState(bool _isOpen)
        {
            isHidden = !_isOpen;
        }
    }
}