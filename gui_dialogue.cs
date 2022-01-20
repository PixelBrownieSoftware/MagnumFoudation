using System.Collections;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace MagnumFoudation
{
    public class s_gui : MonoBehaviour
    {

        public Vector2Int menuchoice;
        int[,] cells;
        public Texture2D text;
        static List<string> texttoWrite = new List<string>();

        public string gem_require_text { get; set; }
        public Text textstr;
        public o_character character;
        public static o_character[] othercharacter;
        public static o_character allycharacter;

        public static s_gui gui;
        static float timer = 0;

        int cell_x, cell_y;
        static bool stayOn;
        bool open_menu = false;

        static string TextShowNotification;
        public Text NotificationText;
        static Image NotificationBox;

        List<Vector2Int> cell_colours = new List<Vector2Int>();

        void Awake()
        {
            cell_x = 20;
            cell_y = 10;
            gui = GetComponent<s_gui>();
        }

        private void Start()
        {
            NotificationBox = GameObject.Find("NotificationBox").GetComponent<Image>();
        }

        public static void DisplayNotificationText(string n, float t)
        {
            TextShowNotification = n;

            if (t < 0)
                stayOn = true;
            else
                stayOn = false;

            timer = t;
            NotificationBox.enabled = true;
        }
        public static void HideNotificationText()
        {
            TextShowNotification = "";
            timer = 0;
            NotificationBox.enabled = false;
        }

        public static void RemoveCharacters(bool top)
        {
            if (top)
            {
                allycharacter = null;
            }
            else
            {
                othercharacter = null;
            }
        }

        public static void RemoveText(string text)
        {
            texttoWrite.Remove(texttoWrite.Find(x => x == text));
        }

        public void ChangeCOlour(Color colou)
        {
            character.rendererObj.color = colou;
        }

        public static void AddText(string str)
        {
            texttoWrite.Add(str);
        }

        public static void AddCharacter(List<o_character> cha)
        {
            othercharacter = cha.ToArray();
        }
        public static void AddCharacter(o_character cha, bool top)
        {
            if (top)
            {
                allycharacter = cha;
            }
            else
            {
                othercharacter = new o_character[1];
                othercharacter[0] = cha;
            }
        }

        private void OnGUI()
        {
            if (textstr != null)
            {
                texttoWrite.Clear();
                /*
                textstr.text = "Money : " + s_globals.Money //+ "\nCharacter Zpos:" + character.Z_offset + " Floor:" + character.Z_floor + "\n" +
               + "Press X to open menu. \n" + "Press N to activate colour.\n" + "Press E to interact with red thing";
                if (menuchoice.x + (int)Input.GetAxisRaw("Horizontal") >= 0 && menuchoice.x + (int)Input.GetAxisRaw("Horizontal") <= cell_x)


                texttoWrite.Add("Gems: " + s_globals.Money + "\n");

                foreach (string s in texttoWrite)
                {
                    textstr.text += s + "\n";
                }
                */
                textstr.text = "Gems: " + s_globals.Money + "\n";
                /*    if (menu)
                        DrawInMulti(new Vector2(cell_x, cell_y), 45);
                        */
                //DrawInARow(4, 15);
                

            }
        }

        public void CreateCells(int x, int y)
        {
            cell_x = x;
            cell_y = y;

            cells = new int[cell_x, cell_y];
        }

        public void CreateCells(int quantity, int lim, bool x_limit)
        {
            int q = Mathf.CeilToInt(quantity / lim);
            /*
            if (this.x_limit)
            {
            }*/

            cell_x = lim;
            cell_y = q;

            cells = new int[cell_x, cell_y];
        }

        public void ResetData()
        {
            cells = null;
        }

        public void DrawInARow(int count, float spacing, float y_offset)
        {
            for (int i = 0; i < count; i++)
            {
                DrawObject(new Rect(30 + spacing * i, 30 + y_offset, 30, 30), text, Color.white);
            }
        }

        /*
        public void DrawInMulti(int quantity ,float spacing)
        {
            int tot = cell_x * cell_y - quantity;
            int x_y = 0;
            int y_x = 0;
            for (int i = 0; i < tot; i++)
            {
                if (i % cell_y == 0)
                {
                    y_x = 0;
                    x_y++;
                }
                y_x++;

                if (menuchoice.x * menuchoice.y > tot)
                {
                    menuchoice.x = 0;
                    menuchoice.y = 0;
                }

                print(menuchoice);
                DrawObject(new Rect(30 + spacing * x_y,  spacing * y_x, 30, 30), text, Color.white);
                if (menuchoice.x == x_y && menuchoice.y == y_x)
                {
                    DrawObject(new Rect(30 + spacing * x_y,  spacing * y_x, 30, 30), text, Color.red);
                }


            }
        }
        */
        public int PickFromList(int amount)
        {

            if (cells == null)
            {
                cell_x = 1;
                cell_y = amount;
                cells = new int[cell_x, cell_y];
            }

            return menuchoice.y;
        }

        /*
        public void DrawInMulti(Vector2 count, float spacing)
        {
            for (int x = 0; x < count.x; x++)
            {
                for (int y = 0; y < count.y; y++)
                {
                    if (menuchoice == new Vector2Int(x, y))
                    {
                        DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, Color.red);
                        if (Input.GetKeyDown(KeyCode.N))
                        {
                            foreach (gui_button butt in buttons)
                            {
                                if (butt.position == menuchoice)
                                {
                                    butt.funct(butt.colour);
                                }
                            }


                            cell_colours.Add(new Vector2Int(x, y));
                            DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, Color.blue);
                        }
                    }
                    else
                    {
                        DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, Color.white);

                        foreach (Vector2Int cell in cell_colours)
                        {
                            if (x == cell.x && y == cell.y)
                            {
                                DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, Color.green);
                            }
                        }
                        foreach (gui_button button in buttons)
                        {
                            if (x == button.position.x && y == button.position.y)
                            {
                                DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, button.colour);
                            }
                        }
                    }


                }
            }
        }
        */

        public void DrawTextGUI(string letter)
        {
            GUI.TextArea(new Rect(90, 90, 140, 80), letter);
        }

        public void DrawText(string letter)
        {
            GetComponent<Text>().text = letter;
        }

        public void DrawObject(Rect rectan, Texture2D thing, Color colour)
        {
            GUI.color = colour;
            GUI.DrawTexture(rectan, text);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                menuchoice.y += 1;

            if (Input.GetKeyDown(KeyCode.UpArrow))
                menuchoice.y -= 1; //(int)Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.RightArrow))
                menuchoice.x += 1; //(int)Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                menuchoice.x -= 1;

            menuchoice.x = Mathf.Clamp(menuchoice.x, 0, cell_x);

            if (NotificationText.text != null)
                NotificationText.text = TextShowNotification;

            if (!stayOn)
            {
                if (timer > 0)
                {
                    NotificationBox.enabled = true;
                    timer = timer - Time.deltaTime;
                }
                else
                {
                    NotificationBox.enabled = false;
                    TextShowNotification = "";
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!open_menu)
                {
                    Time.timeScale = 0;
                    open_menu = true;
                }
                else
                {
                    Time.timeScale = 1;
                    open_menu = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                cell_colours.Clear();
            }
            texttoWrite.Clear();
        }
    }
    public class gui_dialogue : s_gui
    {
        #region VARIABLES
        public Text textthing;
        public Image muggshot;
        public GameObject anim;

        public enum TEXT_STATE
        {
            TYPING,
            INPUT
        }
        public TEXT_STATE TEXST;

        int current_text = 0;
        public bool done_event = false;
        public float speed = 0.05f;
        public bool automatic { get; set; }
        public float default_speed;

        public bool readtext { get; set; }

        public string[] disptxt;
        public string textTarg;
        public GameObject fad;

        GameObject txbx;
        public RectTransform trans;
        Text textB;

        #endregion

        public void Start()
        {
            textB = GameObject.Find("Dialogue").GetComponent<Text>();
            txbx = GameObject.Find("TextBox");
            txbx.SetActive(false);
            readtext = false;
            default_speed = speed;
            textthing = GameObject.Find("Dialogue").GetComponent<Text>();
        }

        #region COPILATION
        
        #endregion

        #region TYPE
        void TypeLine(int currentline)
        {
            textthing.text += textTarg[currentline];
        }
        
        #endregion

        public void Set(string iconName, int number)
        {
            /*
            trans.sizeDelta = new Vector2(880, trans.sizeDelta.y);
            trans.position = new Vector2(706, trans.position.y);
            if (iconName == "none" || iconName == null)
            {
                RemoveIcon();
                return;
            }
            muggshot.enabled = true;
            s_icon ic = icons.Find(x => x.name == iconName);
            for (int i = 0; i < ic.sprites.Count; i++)
            {
                if (i == number)
                    muggshot.sprite = ic.sprites[i];
            }
            */

        }

        public void RemoveIcon()
        {
            trans.sizeDelta = new Vector2(995, trans.sizeDelta.y);
            trans.position = new Vector2(618.3f, trans.position.y);
            muggshot.enabled = false;
        }

        public IEnumerator DisplayDialogue(string textasset)
        {
            done_event = false;
            txbx.SetActive(true);
            textTarg = textasset;
            bool istyping = true;
            bool isSkipped = false;
            current_text = 0;
            while (current_text != disptxt.Length)
            {
                if (istyping)
                {
                    for (int i = 0; i < disptxt[current_text].Length; i++)
                    {
                        if (Input.GetKeyDown(KeyCode.Space) && !automatic)
                        {
                            if (!isSkipped)
                                isSkipped = true;
                        }
                    }
                    isSkipped = false;
                    istyping = false;
                }

                yield return new WaitForSeconds(Time.deltaTime);

                if (automatic)
                {
                    yield return new WaitForSeconds(0.8f);
                    textthing.text = "";
                    istyping = true;
                    current_text++;
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    textthing.text = "";
                    istyping = true;
                    current_text++;
                }

            }
            RemoveIcon();
            txbx.SetActive(false);
            done_event = true;
        }
    }
}
