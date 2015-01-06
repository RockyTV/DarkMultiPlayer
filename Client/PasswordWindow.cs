using System;
using UnityEngine;
using DarkMultiPlayerCommon;

namespace DarkMultiPlayer
{
    public class PasswordWindow
    {
        private static PasswordWindow singleton = new PasswordWindow();
        public bool display;
        public string currentPass = "";
        private bool isWindowLocked = false;
        private bool safeDisplay;
        private bool initialized;
        //GUI Layout
        private Rect windowRect;
        private Rect moveRect;
        private GUILayoutOption[] layoutOptions;
        //Styles
        private GUIStyle windowStyle;
        private GUIStyle buttonStyle;
        private GUIStyle textAreaStyle;
        private GUIStyle labelStyle;
        //const
        private const float WINDOW_HEIGHT = 100;
        private const float WINDOW_WIDTH = 300;

        public PasswordWindow()
        {
            Client.updateEvent.Add(this.Update);
            Client.drawEvent.Add(this.Draw);
        }

        public static PasswordWindow fetch
        {
            get
            {
                return singleton;
            }
        }

        private void InitGUI()
        {
            windowRect = new Rect(Screen.width / 2f - WINDOW_WIDTH / 2f, Screen.height / 2f - WINDOW_HEIGHT / 2f, WINDOW_WIDTH, WINDOW_HEIGHT);
            moveRect = new Rect(0, 0, 10000, 20);

            windowStyle = new GUIStyle(GUI.skin.window);
            buttonStyle = new GUIStyle(GUI.skin.button);
            textAreaStyle = new GUIStyle(GUI.skin.textArea);
            labelStyle = new GUIStyle(GUI.skin.label);

            layoutOptions = new GUILayoutOption[4];
            layoutOptions[0] = GUILayout.Width(WINDOW_WIDTH);
            layoutOptions[1] = GUILayout.Height(WINDOW_HEIGHT);
            layoutOptions[2] = GUILayout.ExpandWidth(true);
            layoutOptions[3] = GUILayout.ExpandHeight(true);
        }

        private void Update()
        {
            safeDisplay = display;
        }

        private void Draw()
        {
            if (!initialized)
            {
                initialized = true;
                InitGUI();
            }
            if (safeDisplay)
            {
                windowRect = DMPGuiUtil.PreventOffscreenWindow(GUILayout.Window(6714 + Client.WINDOW_OFFSET, windowRect, DrawContent, "Server Password", windowStyle, layoutOptions));
            }
        }

        private void DrawContent(int windowID)
        {
            GUILayout.BeginVertical();
            GUI.DragWindow(moveRect);
            GUILayout.BeginHorizontal();
            currentPass = GUILayout.TextArea(currentPass, textAreaStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Connect", buttonStyle))
            {
                DarkLog.Debug("[Password] " + Common.CalculateMD5Hash(currentPass) == NetworkWorker.fetch.svPassword ? "password match" : "password don't match");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}
