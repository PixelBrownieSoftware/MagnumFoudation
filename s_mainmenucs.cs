using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MagnumFoudation
{
    public class s_mainmenu : MonoBehaviour
    {
        public static dat_save save;
        public static bool isload = false;
        public GUISkin menuskins;

        string currentKeyName = "none";
        KeyValuePair<string, KeyCode> keyToChange = new KeyValuePair<string, KeyCode>("none", KeyCode.None);

        public enum MENU_MODE
        {
            MENU,
            OPTIONS
        };
        MENU_MODE menu;

        void Start()
        {
            s_globals.arrowKeyConfig.Add("left", KeyCode.A);
            s_globals.arrowKeyConfig.Add("right", KeyCode.D);
            s_globals.arrowKeyConfig.Add("down", KeyCode.S);
            s_globals.arrowKeyConfig.Add("up", KeyCode.W);
            s_globals.arrowKeyConfig.Add("jump", KeyCode.Space);
            s_globals.arrowKeyConfig.Add("dash", KeyCode.LeftShift);
            s_globals.arrowKeyConfig.Add("select", KeyCode.E);
        }

        private void OnGUI()
        {
            Event e = Event.current;
            switch (menu)
            {
                case MENU_MODE.MENU:

                    if (GUI.Button(new Rect(0,0,90,40),"Start game", menuskins.GetStyle("Box")))
                    {
                        isload = false;
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Game");
                    }
                    if (File.Exists("save.MF"))
                    {
                        if (GUI.Button(new Rect(0, 50, 90, 40),"Load game", menuskins.GetStyle("Box")))
                        {
                            isload = true;
                            FileStream fs = new FileStream("save.MF", FileMode.Open);
                            BinaryFormatter bin = new BinaryFormatter();

                            save = (dat_save)bin.Deserialize(fs);

                            fs.Close();
                            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Game");
                        }
                    }
                    if (GUI.Button(new Rect(0, 100, 90, 40), "Options", menuskins.GetStyle("Box")))
                    {
                        menu = MENU_MODE.OPTIONS;
                    }

                    break;

                case MENU_MODE.OPTIONS:
                    GUILayout.Label("Click on the buttons to change controls. Current Key to change: " + currentKeyName, menuskins.GetStyle("Box"));
                    foreach (KeyValuePair<string, KeyCode> key in s_globals.arrowKeyConfig)
                    {
                        if (GUILayout.Button(key + ": " + key.Value.ToString(), menuskins.GetStyle("Box")))
                        {
                            currentKeyName = key.Key;
                        }
                    }
                    if (currentKeyName != "none")
                    {
                        
                        if (e.isKey || e.shift)
                        {
                            if (e.shift)
                                s_globals.arrowKeyConfig[currentKeyName] = KeyCode.LeftShift;
                            else
                                s_globals.arrowKeyConfig[currentKeyName] = e.keyCode;

                            currentKeyName = "none";
                        }
                    }
                    if (GUILayout.Button("Back", menuskins.GetStyle("Box")))
                    {
                        currentKeyName = "none";
                        menu = MENU_MODE.MENU;
                    }
                    break;
            }

        }
    }
}
