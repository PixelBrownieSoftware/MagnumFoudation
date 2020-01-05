using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;

namespace MagnumFoudation
{
    using UnityEngine.UI;
    public class s_nodegraph : MonoBehaviour
    {
        public s_node[] nodeGraph;
        public int tileSize = 20;
        public Vector2Int size;

        public List<s_node> path = new List<s_node>();
        
        public float CheckYFall(s_node node, int FallingNodeType)
        {
            s_node nextnode = node;
            s_node currentNode = node;
            //Vector2Int v = PosToVec(node.realPosition);
            Vector2 newPos = new Vector2(0,0);
            int n = 0;
            while (nextnode != null)
            {
                /*
                if (v.x + ((v.y - n) 
                    //* size.x
                    ) > nodeGraph.Length ||
                    v.x + ((v.y - n)
                    //* size.x
                    ) < 0)
                    break;
                nextnode = nodeGraph[v.x + ((v.y - n) 
                    //* size.x
                    )];
                */
                nextnode = PosToNode(node.realPosition - new Vector2(0, tileSize * n));
                if (nextnode != null)
                    currentNode = nextnode;
                if (nextnode == null || nextnode.COLTYPE != FallingNodeType)
                {
                    newPos = currentNode.realPosition;
                    print("falling to " + n);
                    return newPos.y;
                }
                n++;
            }
            print("falling to " + n);
            return newPos.y;
        }
        
        public void CreateNodeArray(s_map.s_tileobj[] ti)
        {
            s_node[] arr = new s_node[size.x * size.y];
            print(size);
            //o_collidableobject[] col = GameObject.Find("Tiles").GetComponentsInChildren<o_collidableobject>();

            int tilesize = 20;
            int x = 0, y = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                x++;
                if (x == size.x)
                {
                    y++;
                    x = 0;
                }
                arr[i] = new s_node();
                arr[i].realPosition = new Vector2(x * tileSize, y * tileSize);
                arr[i].COLTYPE = -1;
            }
            foreach (s_map.s_tileobj c in ti)
            {
                Vector2Int v = PosToVec(c.pos_x, c.pos_y);
                int num = v.x + (v.y * size.x);

                if (arr.Length < num)
                    continue;
                if (0 > num)
                    continue;
                //if(c.enumthing != -1)
               // print(c.enumthing + " postion " + c.pos_x + ", " + c.pos_y);

                arr[num].type = c.TYPENAME;
                arr[num].realPosition = new Vector2(c.pos_x, c.pos_y);
                arr[num].COLTYPE = c.enumthing;
                arr[num].characterExclusive = c.exceptionChar;

                if (arr[num].type == "teleport_object")
                {
                    arr[num].COLTYPE = -1;
                    arr[num].walkable = true;
                }
                arr[num].walkable = true;

                //arr[x + (y * xsi)].walkable = true;
                if (new Vector2(c.pos_x, c.pos_y) == new Vector2(x * tilesize, y * tilesize))
                {
                }
            }
            nodeGraph = arr;
        }

        struct s_quadNode
        {
            public s_quadNode(Vector2 topR, Vector2 downR, Vector2 topL, Vector2 downL)
            {
                points = new Vector2[4]
                {
                topL,
                topR,
                downL,
                downR
                };
            }
            Vector2[] points;
        }

        /*
        public void NodeToPolygon()
        {
            List<s_quadNode> quads = new List<s_quadNode>();

            Vector2[] points =
                new Vector2[4] {
                new Vector2(tileSize ,0),
                new Vector2(tileSize, tileSize),
                new Vector2(0,tileSize),
                new Vector2(0,0),
                };

            bool[,] nodebool = new bool[(xsi + 1), (ysi + 1)];

            s_node node = null;
            for (int x = 0; x < xsi; x++)
            {
                for (int y = ysi; y > 0; y--)
                {
                    print(nodebool[x, y]);
                    if (nodebool[x, y] == true)
                        continue;

                    node = PosToNode(new Vector2(x * tileSize, y * tileSize));
                    if (node != null)
                    {
                        if (node.COLTYPE != COLLISION_T.NONE)
                        {
                            int stoppper = 0;
                            int maxX = 0;

                            Vector2Int counter = new Vector2Int(0, 0);
                            s_node nextnode = PosToNode(new Vector2((x + counter.x) * tileSize, (y - counter.y) * tileSize));
                            while (nextnode != null || stoppper < 20)
                            {
                                nextnode = PosToNode(new Vector2((x + counter.x) * tileSize, (y - counter.y) * tileSize));
                                s_node nnnode = PosToNode(new Vector2((x + counter.x + 1) * tileSize, (y - counter.y) * tileSize));
                                if (counter.x > xsi || nnnode != null)
                                {
                                    if (nnnode.COLTYPE == COLLISION_T.NONE)
                                    {
                                        if (counter.x < maxX)
                                        {
                                            maxX = counter.x;
                                        }
                                        counter.x = 0;
                                        counter.y += 1;
                                        s_node ynod = PosToNode(new Vector2(x * tileSize, (y - counter.y) * tileSize));
                                        if (ynod != null)
                                        {
                                            if (ynod.COLTYPE == COLLISION_T.NONE)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                counter.x += 1;
                                maxX += 1;
                                stoppper++;
                            }

                            points = new Vector2[4]
                            {
                            new Vector2(maxX * tileSize ,0),
                            new Vector2(maxX* tileSize,counter.y* tileSize),
                            new Vector2(0,counter.y* tileSize),
                            new Vector2(0,0),
                            };
                            GameObject polyObj = Instantiate(new GameObject(), transform.position + new Vector3(x * tileSize, y * tileSize) + new Vector3(200, 200), Quaternion.identity);
                            PolygonCollider2D poly = polyObj.AddComponent<PolygonCollider2D>();
                            poly.pathCount = 1;
                            poly.SetPath(0, points);

                            for (int x2 = x; x2 > x + maxX; x2++)
                            {
                                for (int y2 = y + counter.y; y2 < y; y2--)
                                {
                                    nodebool[x2, y2] = true;
                                }
                            }

                        }
                    }
                }
            }

        }
        */
        /*
        public void CreateNodeGraph()
        {
            s_levelloader ed = GameObject.Find("General").GetComponent<s_levelloader>();
            GameObject mapn = ed.SceneLevelObject;
            o_generic[] colInMap = null;
            colInMap = mapn.transform.Find("Tiles").GetComponentsInChildren<o_generic>();

            nodeGraph = new s_node[xsi, ysi];
            for (int x = 0; x < xsi; x++)
            {
                for (int y = 0; y < ysi; y++)
                {
                    nodeGraph[x, y] = new s_node();
                    nodeGraph[x, y].realPosition = new Vector2(tileSize * x, tileSize * y);
                    nodeGraph[x, y].walkable = true;
                    foreach (o_generic h in colInMap)
                    {
                        o_generic c = h.GetComponent<o_generic>();
                        if (c)
                        {
                            if (c.transform.position != (Vector3)new Vector2(tileSize * x, tileSize * y))
                                continue;
                            nodeGraph[x, y].COLTYPE = c.collision_type;
                            if (c.collision_type == COLLISION_T.WALL)
                            {
                            }
                        }
                        else
                        {
                            nodeGraph[x, y].walkable = true;
                        }
                    }
                }
            }
        }
        */

        public Vector2Int PosToVec(int x_p, int y_p)
        {
            int x = (int)(x_p / tileSize);
            int y = (int)(y_p / tileSize);
            //print("x: " + x + " y: " + y);

            return new Vector2Int(x, y);
        }
        public Vector2Int PosToVec(Vector3 vec)
        {
            int x = (int)(vec.x / tileSize);
            int y = (int)(vec.y / tileSize);
            //print("x: " + x + " y: " + y);

            return new Vector2Int(x, y);
        }
        public s_node PosToNode(Vector2 vec)
        {
            if (nodeGraph == null)
                return null;
            int x = (int)(vec.x / tileSize);
            int y = (int)(vec.y / tileSize);
            int num = x + (y * size.x);
            if (num < 0 || num > nodeGraph.Length - 1)
            {
                //print("Sorry! x: " + x + " y: " + y);
                return null;
            }
            return nodeGraph[num];
        }
        
        float HerusticVal(Vector2 a, Vector2 b)
        {
            float distx = Mathf.Abs(a.x - b.x);
            float disty = Mathf.Abs(a.y - b.y);


            return Vector2.Distance(a, b) / tileSize;
            //D * (distx + dist)

        }
        public List<s_node> RetracePath(s_node goal, s_node start)
        {
            int i = 0;
            List<s_node> route = new List<s_node>();
            s_node current = goal;
            while (current != start)
            {
                route.Add(current);
                current = current.parent;
                i++;
                if (i == int.MaxValue)
                    return route;
            }
            route.Reverse();
            return route;
        }
        
    }

    [System.Serializable]
    public struct ev_integer
    {
        public int integer;
        public string integer_name;
    }

    [System.Serializable]
    public struct s_pooler_data
    {
        public bool single;
        public GameObject gameobject;
    }

    [System.Serializable]
    public struct dat_globalflags
    {
        public dat_globalflags(Dictionary<string, int> Flags)
        {
            this.Flags = Flags;
        }
        public Dictionary<string, int> Flags;
    }

    [System.Serializable]
    public struct dat_save
    {
        public dat_save(dat_globalflags gbflg, int health, string currentmap, List<s_map> maps)
        {
            hp = health;
            this.gbflg = gbflg;
            this.currentmap = currentmap;
            this.maps = maps;
        }
        public int hp;
        public string currentmap;
        public List<s_map> maps;
        public dat_globalflags gbflg;
    }

    public class s_globals : MonoBehaviour
    {
        public Dictionary<string, s_map> maps = new Dictionary<string, s_map>();
        public string currentlevelname;

        s_levelloader ed;
        bool CALL_STATIC_SCRIPT = false;
        bool COLLISION_SHOW = false;
        bool DEBUG_MODE_ON = false;
        enum EDITOR_MODE
        {
            FLAG,
            MAP_TRANS,
            NOCLIP,
            ENABLE_COLLISIONS,
            CALL_MAP_SCRIPT
        }
        EDITOR_MODE EDIT;

        s_map curmap;
        Vector2 scroll = new Vector2(0, 0);
        public o_character player;
        public static bool isPause = false;

        public bool saveOnQuit;
        public bool isMainGame;

        public bool isFixedSaveAreaInput = false;
        static bool isFixedSaveArea;

        public string fixedSaveAreaNameInput;
        static string fixedSaveAreaName;

        public bool EnableDebug;

        public static int Money;
        public List<ev_integer> GlobalFlagCache = new List<ev_integer>();
        public static Dictionary<string, int> GlobalFlags = new Dictionary<string, int>();

        public static Dictionary<string, KeyCode> arrowKeyConfig = new Dictionary<string, KeyCode>();

        public static void SetGlobalFlag(string flagname, int flag)
        {
            if (!GlobalFlags.ContainsKey(flagname))
            {
                print("Flag " + flagname + " does not exist.");
                return;
            }
            GlobalFlags[flagname] = flag;
            print("Flag " + flagname + "  set to " + flag);
        }

        public static int GetGlobalFlag(string flagname)
        {
            if (!GlobalFlags.ContainsKey(flagname))
            {
                //print("Flag " + flagname + " does not exist.");
                return int.MinValue;
            }
            return GlobalFlags[flagname];
        }

        public static KeyCode GetKey(string keyName)
        {
            if (arrowKeyConfig.ContainsKey(keyName))
            {
                return arrowKeyConfig[keyName];
            }
            return KeyCode.None;
        }

        public virtual void AddKeys()
        {
            s_globals.arrowKeyConfig.Add("left", KeyCode.A);
            s_globals.arrowKeyConfig.Add("right", KeyCode.D);
            s_globals.arrowKeyConfig.Add("down", KeyCode.S);
            s_globals.arrowKeyConfig.Add("up", KeyCode.W);
            s_globals.arrowKeyConfig.Add("jump", KeyCode.Space);
            s_globals.arrowKeyConfig.Add("dash", KeyCode.LeftShift);
            s_globals.arrowKeyConfig.Add("select", KeyCode.E);
        }

        public void Start()
        {
            if (SceneManager.GetActiveScene().name == "Main Game")
            {
                isFixedSaveArea = isFixedSaveAreaInput;
                fixedSaveAreaName = fixedSaveAreaNameInput;

                if (s_globals.arrowKeyConfig.Count == 0)
                    AddKeys();

                player = GameObject.Find("Player").GetComponent<o_character>();
                ed = GetComponent<s_levelloader>();
                foreach (ev_integer e in GlobalFlagCache)
                {
                    GlobalFlags.Add(e.integer_name, e.integer);
                }
                if (!EnableDebug)
                {
                    o_trigger[] trigs = s_levelloader.LevEd.GetTriggerObjects();

                    foreach (o_trigger t in trigs)
                    {
                        t.rendererObj.color = Color.clear;
                    }
                    player.IS_KINEMATIC = true;
                    ed.colmp.color = Color.clear;
                    COLLISION_SHOW = false;
                }
            }
        }

        public void LoadFlag(dat_globalflags flag)
        {
            GlobalFlags = flag.Flags;
        }

        public static void SaveData()
        {
            FileStream fs = new FileStream("save.MF", FileMode.Create);
            BinaryFormatter bin = new BinaryFormatter();
            s_levelloader lev = GameObject.Find("General").GetComponent<s_levelloader>();

            dat_save sav = new dat_save(new dat_globalflags(GlobalFlags), 1, "", lev.maps);

            if (isFixedSaveArea)
                sav.currentmap = fixedSaveAreaName;

            bin.Serialize(fs, sav);
            fs.Close();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPause)
                {
                    isPause = true;
                    Time.timeScale = 0;
                    foreach (o_character c in s_levelloader.LevEd.allcharacters)
                    {
                        c.enabled = false;
                    }
                }
            }
        }

        public void OnGUI()
        {
            if (isPause && isMainGame)
            {
                if (saveOnQuit)
                {
                    if (GUI.Button(new Rect(250, 180, 120, 40), "Save and quit"))
                    {
                        SaveData();
                        SceneManager.LoadScene(0);
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(250, 180, 120, 40), "Quit game"))
                    {
                        SceneManager.LoadScene(0);
                    }
                }
                if (GUI.Button(new Rect(250, 120, 120, 40), "Resume"))
                {
                    isPause = false;
                    Time.timeScale = 1;
                    foreach (o_character c in s_levelloader.LevEd.allcharacters)
                    {
                        c.enabled = true;
                    }
                }
            }

            if (EnableDebug)
            {
                if (DEBUG_MODE_ON)
                { 
                    if (GUI.Button(new Rect(0, 0, 120, 40), "Debug Mode Off"))
                    {
                        o_trigger[] trigs = s_levelloader.LevEd.GetTriggerObjects();

                        foreach (o_trigger t in trigs)
                        {
                            t.rendererObj.color = Color.clear;
                        }
                        DEBUG_MODE_ON = false;
                    }
                    if (GUI.Button(new Rect(0, 60, 120, 40), EDIT.ToString()))
                    {
                        EDIT++;
                        EDIT = (EDITOR_MODE)(int)Mathf.Clamp((float)EDIT, 0, 5);
                        if ((int)EDIT == 5)
                        {
                            EDIT = EDITOR_MODE.FLAG;
                        }
                    }
                    int ind = 2;
                    int x = 0, y = 0;
                    switch (EDIT)
                    {
                        case EDITOR_MODE.FLAG:

                            foreach (KeyValuePair<string, int> flag in GlobalFlags)
                            {
                                GUI.Label(new Rect(0, 50 * ind, 90, 40), flag.Key + " Value: " + flag.Value);
                                if (GUI.Button(new Rect(100, 50 * ind, 90, 40), "+"))
                                {
                                    SetGlobalFlag(flag.Key, flag.Value + 1);
                                }
                                if (GUI.Button(new Rect(200, 50 * ind, 90, 40), "-"))
                                {
                                    SetGlobalFlag(flag.Key, flag.Value - 1);
                                }
                                ind++;
                            }
                            break;

                        case EDITOR_MODE.MAP_TRANS:
                            
                            List<s_map> maps = s_levelloader.LevEd.maps;
                            for (int i = 0; i < maps.Count; i++)
                            {
                                s_map ma = maps[i];

                                if (curmap == ma)
                                {
                                    if (GUI.Button(new Rect(150, 50 * 1, 160, 50), "Back"))
                                    {
                                        curmap = null;
                                    }
                                    foreach (s_map.s_tileobj ti in ma.tilesdata)
                                    {
                                        scroll = GUI.BeginScrollView(new Rect(0, 0, 200, 400), scroll, new Rect(0, 0, 200, 300));
                                        if (ti.TYPENAME == "teleport_object")
                                        {
                                            if (y == 10)
                                            {
                                                y = 0;
                                                x++;
                                            }
                                            if (GUI.Button(new Rect(160 * x, 50 * (y + 2), 160, 50), ti.name))
                                            {
                                                s_levelloader.LevEd.TriggerSpawn(ma.name, ti.name);
                                            }
                                            ind++;
                                            y++;
                                        }
                                        GUI.EndScrollView();
                                    }
                                }
                                else
                                {
                                    if (curmap == null)
                                    {

                                        if (y == 10)
                                        {
                                            y = 0;
                                            x++;
                                        }
                                        if (GUI.Button(new Rect(160 * x, 50 * (y + 3), 160, 50), ma.name))
                                        {
                                            curmap = ma;
                                        }
                                        y++;
                                    }
                                }

                            }

                            break;

                        case EDITOR_MODE.NOCLIP:
                            if (player != null)
                            {
                                if (player.IS_KINEMATIC)
                                {
                                    if (GUI.Button(new Rect(0, 100, 160, 50), "Disable"))
                                    {
                                        player.IS_KINEMATIC = false;
                                    }
                                }
                                else
                                {
                                    if (GUI.Button(new Rect(0, 100, 160, 50), "Enable"))
                                    {
                                        player.IS_KINEMATIC = true;
                                    }
                                }
                            }
                            break;

                        case EDITOR_MODE.ENABLE_COLLISIONS:

                            if (COLLISION_SHOW)
                            {
                                if (GUI.Button(new Rect(0, 100, 160, 50), "Hide collisions"))
                                {
                                    ed.colmp.color = Color.clear;
                                    COLLISION_SHOW = false;
                                }
                            }
                            else
                            {
                                if (GUI.Button(new Rect(0, 100, 160, 50), "Show collisions"))
                                {
                                    ed.colmp.color = new Color(1, 1, 1, 0.5f);
                                    COLLISION_SHOW = true;
                                }
                            }
                            break;

                        case EDITOR_MODE.CALL_MAP_SCRIPT:
                            if (!CALL_STATIC_SCRIPT)
                            {
                                if (GUI.Button(new Rect(0, 100, 120, 40), "Current script: Map Script"))
                                {
                                    CALL_STATIC_SCRIPT = true;
                                }
                            }
                            else {

                                if (GUI.Button(new Rect(0, 100, 120, 40), "Current script: Static Script"))
                                {
                                    CALL_STATIC_SCRIPT = false;
                                }
                            }

                            List<ev_details> currentScript = null;

                            if (CALL_STATIC_SCRIPT)
                                currentScript = s_triggerhandler.trig.StaticEvents;
                            else
                                currentScript = s_levelloader.LevEd.mapDat.Map_Script;

                            scroll = GUI.BeginScrollView(new Rect(0, 150, 200, 400), scroll, new Rect(0, 150, 200, 300));

                            //To fix the bug when the first event never appears
                            ev_details detBeg = currentScript[0];
                            if (detBeg.eventType == -1)
                            {
                                if (y == 10)
                                {
                                    y = 0;
                                    x++;
                                }
                                if (GUI.Button(new Rect(160 * x, 50 * (y + 2), 160, 50), detBeg.string0))
                                {
                                    s_levelloader.LevEd.GetComponent<s_triggerhandler>().JumpToEvent(0, CALL_STATIC_SCRIPT);
                                }
                                ind++;
                                y++;
                            }

                            for (int i = 0; i < currentScript.Count; i++)
                            {
                                ev_details det = currentScript[i];
                                if (det.eventType == -1)
                                {
                                    if (y == 10)
                                    {
                                        y = 0;
                                        x++;
                                    }
                                    if (GUI.Button(new Rect(160 * x, 50 * (y + 2), 160, 50), det.string0))
                                    {
                                        s_levelloader.LevEd.GetComponent<s_triggerhandler>().JumpToEvent(i, CALL_STATIC_SCRIPT);
                                    }
                                    ind++;
                                    y++;
                                }
                            }
                            GUI.EndScrollView();
                            break;
                    }

                }
                else
                {
                    if (GUI.Button(new Rect(0, 0, 120, 40), "Debug Mode"))
                    {
                        DEBUG_MODE_ON = true;
                        o_trigger[] trigs = s_levelloader.LevEd.GetTriggerObjects();

                        foreach (o_trigger t in trigs)
                        {
                            t.rendererObj.color = Color.white;
                        }
                    }
                }
            }
            else
            {
                player.IS_KINEMATIC = true;
                ed.colmp.color = Color.clear;
                COLLISION_SHOW = false;
            }
        }
    }
    public class s_intro : MonoBehaviour
    {
        public void SwitchScene(int i)
        {
            SceneManager.LoadScene(i);
        }

        public void PlayJingleSound()
        {
            GetComponent<AudioSource>().Play();
        }
    }

    [System.Serializable]
    public struct s_dialogue_choice
    {
        public s_dialogue_choice(string option, int flagTojump)
        {
            this.option = option;
            this.flagTojump = flagTojump;
        }

        public string option;
        public int flagTojump;
    }
    public class s_triggerhandler : MonoBehaviour
    {
        public List<s_dialogue_choice> dialogueChoices = new List<s_dialogue_choice>();
        public float travSpeed = 22.5f;
        public static s_triggerhandler trig;
        public bool doingEvents = false;
        Image fade;
        public Text Dialogue;
        const float shutterdepth = 1.55f;
        public List<ev_details> StaticEvents;
        public o_character player;
        public List<ev_details> Events;
        public o_trigger current_trigger;
        bool doStatic = false;
        public o_character[] characters;
        s_levelloader leveled;
        public s_object selobj;
        public int pointer;
        int textNum = 0;
        public Image textBox;
        public ev_details current_ev { get; set; }

        bool activated_shutters = false;
        
        public void Awake()
        {
            if (trig == null)
                trig = this;
            leveled = GetComponent<s_levelloader>();
            print(leveled);
            if(GameObject.Find("GUIFADE"))
                fade = GameObject.Find("GUIFADE").GetComponent<Image>();
        }

        public void SetTrigObject()
        {
            if (trig == null)
                trig = this;
        }
        
        /// <summary>
        /// Reserve event number -1 for label
        /// </summary>
        /// <param name="label"></param>
        public void JumpToEvent(int label, bool staicev)
        {
            pointer = label;
            doStatic = staicev;
            StartCoroutine(EventPlayMast());
        }

        public void GetMapEvents()
        {
            Events = leveled.mapDat.Map_Script;
        }
        public void GetMapEvents(List<ev_label> map_script_labels, List<ev_details> Map_Script)
        {
            Events = Map_Script;
        }
        
        public IEnumerator EventPlayMast()
        {
            doingEvents = true;
            int evLeng = 0;

            if (doStatic)
                evLeng = StaticEvents.Count;
            else
                evLeng = Events.Count;

            while (pointer != -1 || pointer < evLeng)
            {
                if (!doStatic)
                    current_ev = Events[pointer];
                else
                    current_ev = StaticEvents[pointer];

                yield return StartCoroutine(EventPlay());

                // print("Pointer at: " + pointer);
                if (pointer == -1)
                    break;
                pointer++;
            }
            current_trigger = null;
            doingEvents = false;
            //isskipping = false;
            Time.timeScale = 1;
            activated_shutters = false;
        }

        /// <summary>
        /// Plays all the general events, if you want to add more events then override this function
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator EventPlay()
        {
            int labelNum = 0;
            switch (current_ev.eventType)
            {
                #region DISPLAY NOTIFICATION TEXT
                case 23:

                    s_gui.DisplayNotificationText(current_ev.string0, current_ev.float0);
                    break;
#endregion

                #region ANIMATE CHARACTER
                case 4:

                    Animator character = selobj.GetComponent<Animator>();
                    character.Play(current_ev.int0);
                    character.speed = current_ev.float0;
                    break;
#endregion

                #region MOVE CHARACTER
                case 0:

                    float timer = 1.02f;
                    o_character charaMove = null;

                    if (current_ev.string0 != "o_player")
                        charaMove = GameObject.Find(current_ev.string0).GetComponent<o_character>();
                    else
                        charaMove = player;

                    Vector2 newpos = charaMove.transform.position;
                    s_map.s_tileobj to = s_levelloader.LevEd.mapDat.tilesdata.Find(
                        x => x.TYPENAME == "teleport_object" && 
                        x.name == current_ev.string1);

                    newpos = new Vector2(to.pos_x, to.pos_y);

                    if (current_ev.boolean)
                    {
                        charaMove.transform.position = new Vector3(newpos.x, newpos.y, 0);
                        break;
                    }

                    /*
                    first_move_event = false;
                    while (timer > 0)
                    {
                        newpos += (current_ev.direcion.normalized * current_ev.float0 * current_ev.float1) * 0.007f;
                        timer -= 0.007f;
                    }
                    */

                    float dist = Vector2.Distance(charaMove.transform.position, newpos);
                    Vector2 dir = (newpos - new Vector2(charaMove.transform.position.x, charaMove.transform.position.y)).normalized;
                    print(newpos);


                    while (Vector2.Distance(charaMove.transform.position, newpos)
                        > dist * 0.01f)
                    {
                        charaMove.transform.position += (Vector3)(dir * current_ev.float0 * current_ev.float1) * 0.007f;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    break;
                #endregion

                #region DIALOUGE
                case 1:
                    textBox.gameObject.SetActive(true);
                    while (Dialogue.text.Length < current_ev.string0.Length)
                    {
                        Dialogue.text += current_ev.string0[textNum];
                        if (Input.GetKeyDown(s_globals.GetKey("select"))
                            //&& !automatic
                            )
                        {
                            /*
                            if (!isSkipped)
                                isSkipped = true;
                            */
                            Dialogue.text = current_ev.string0;
                            continue;
                        }
                        textNum++;
                        yield return new WaitForSeconds(Time.deltaTime * 2);
                    }
                    while (!Input.GetKeyDown(s_globals.GetKey("select")))
                    {
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    Dialogue.text = "";
                    textNum = 0;
                    if (pointer + 1 < Events.Count)
                    {
                        if(Events[pointer + 1].eventType != 1)
                            textBox.gameObject.SetActive(false);
                    }
                    break;
                #endregion

                #region RUN CHARACTER SCRIPT
                case 3:

                    if (current_ev.string0 == "o_player")
                    {
                        o_character ch = player.GetComponent<o_character>();
                        if (ch.rbody2d != null)
                            ch.rbody2d.velocity = Vector2.zero;
                        ch.control = current_ev.boolean;
                    }
                    else
                    {
                        GameObject obj = GameObject.Find(current_ev.string0);
                        if (obj != null)
                        {
                            o_character ch = obj.GetComponent<o_character>();
                            if (ch != null)
                            {
                                if (ch.rbody2d != null)
                                    ch.rbody2d.velocity = Vector2.zero;
                                ch.control = current_ev.boolean;
                            }
                        }
                    }
                    break;
                #endregion

                #region BREAK
                case 10:
                    /*
                    if (selobj.GetComponent<o_character>() != null)
                        selobj.GetComponent<o_character>().control = true;
                    dialogueChoices.Clear();
                    */
                    pointer = -1;
                    break;
                #endregion

                #region CAMERA MOVEMENT
                case 9:

                    GameObject ca = GameObject.Find("Main Camera");
                    ca.GetComponent<s_camera>().focus = false;
                    ca.GetComponent<s_camera>().ResetSpeedProg();
                    ca.GetComponent<s_camera>().lerping = true;

                    float spe = current_ev.float0 ; //SPEED
                    float s = 0;

                    s_object obje = null;

                    if (GameObject.Find(current_ev.string0) != null)
                        obje = GameObject.Find(current_ev.string0).GetComponent<s_object>();

                    Vector2 pos = new Vector2(0, 0);
                    if (obje != null)
                        pos = obje.transform.position;

                    if (obje != null)
                        ca.GetComponent<s_camera>().targetPos = obje.transform.position;
                    else
                        ca.GetComponent<s_camera>().targetPos = new Vector2(current_ev.pos.x, current_ev.pos.y);

                    if (current_ev.boolean)
                    {
                        float dista = Vector2.Distance(ca.transform.position, new Vector3(pos.x, pos.y));

                        while (Vector2.Distance(ca.transform.position, new Vector3(pos.x, pos.y))
                            > dista * 0.05f)
                        {
                           // s += spe * Time.deltaTime * travSpeed;
                           // ca.transform.position = Vector3.Lerp(ca.transform.position, new Vector3(pos.x, pos.y, -15), s);
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        if (current_ev.boolean1)
                        {
                            ca.GetComponent<s_camera>().focus = true;
                           // ca.GetComponent<s_camera>(). = obje.GetComponent<o_character>();

                        }
                    }
                    else
                    {

                        float dista = Vector2.Distance(ca.transform.position, new Vector3(current_ev.pos.x, current_ev.pos.y));
                        while (Vector2.Distance(ca.transform.position, new Vector3(current_ev.pos.x, current_ev.pos.y))
                            > dista * 0.05f)
                        {
                           // s += spe * Time.deltaTime * travSpeed;
                           // ca.transform.position = Vector2.Lerp(ca.transform.position, new Vector3(current_ev.pos.x, current_ev.pos.y, -15), s);
                            //ca.transform.position = new Vector3(ca.transform.position.x, ca.transform.position.y, -15);
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                    }
                    ca.GetComponent<s_camera>().lerping = false;
                    break;
                #endregion

                #region CHECK FLAG
                case 8:
                    int integr = s_globals.GetGlobalFlag(current_ev.string0);

                    labelNum = FindLabel(current_ev.string1);

                    if (labelNum == int.MinValue)
                        labelNum = current_ev.int1 - 1;

                    switch (current_ev.logic)
                    {
                        case 1:
                            if (integr == current_ev.int0)  //Check if it is equal to the value
                            {
                                pointer = labelNum;   //Label to jump to
                            }
                            else
                            {
                                if (current_ev.boolean)     //Does this have an "else if"?
                                {
                                    pointer = current_ev.int2 - 1;      //Other Label to jump to
                                }
                            }
                            break;

                        case 8:
                            if (integr != current_ev.int0)  //Check if it is not equal to the value
                            {
                                pointer = labelNum;   //Label to jump to
                            }
                            else
                            {
                                if (current_ev.boolean)     //Does this have an "else if"?
                                {
                                    pointer = current_ev.int2 - 1;      //Other Label to jump to
                                }
                            }
                            break;

                        case 0:
                            if (integr > current_ev.int0)  //Check if it is greater
                            {
                                pointer = labelNum;   //Label to jump to
                            }
                            else
                            {
                                if (current_ev.boolean)     //Does this have an "else if"?
                                {
                                    pointer = current_ev.int2 - 1;      //Other Label to jump to
                                }
                            }
                            break;


                        case 2:
                            if (integr < current_ev.int0)  //Check if it is less
                            {
                                pointer = labelNum;   //Label to jump to
                            }
                            else
                            {
                                if (current_ev.boolean)     //Does this have an "else if"?
                                {
                                    pointer = current_ev.int2 - 1;      //Other Label to jump to
                                }
                            }
                            break;

                            /*
                        case LOGIC_TYPE.CHECK_UTILITY_RETURN_NUM:

                            //This checks utilities after the INITIALIZE function
                            if (GetComponent<s_utility>().eventState == current_ev.int0)
                            {
                                pointer = current_ev.int1 - 1;   //Label to jump to
                            }
                            else
                            {
                                pointer = current_ev.int2 - 1;
                            }
                            break;
                            */
                    }
                    if (integr == current_ev.int0)
                    {

                    }
                    break;
                #endregion

                #region SHUTTERS
                case 21:
                    Image sh1 = GameObject.Find("Shutter1").GetComponent<Image>();
                    Image sh2 = GameObject.Find("Shutter2").GetComponent<Image>();
                    if (activated_shutters)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            sh1.rectTransform.position += new Vector3(0, shutterdepth);
                            sh2.rectTransform.position += new Vector3(0, -shutterdepth);
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        activated_shutters = false;
                    }
                    else
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            sh1.rectTransform.position += new Vector3(0, -shutterdepth);
                            sh2.rectTransform.position += new Vector3(0, shutterdepth);
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        activated_shutters = true;
                    }
                    break;
                #endregion

                #region FADE
                case 13:
                    Color col = new Color(current_ev.colour.r, current_ev.colour.g, current_ev.colour.b, current_ev.colour.a);
                    float t = 0;

                    while (fade.color != col)
                    {
                        t += Time.deltaTime;
                        fade.color = Color.Lerp(fade.color, col, t);
                        yield return new WaitForSeconds(Time.deltaTime * 2);
                    }
                    break;
                #endregion

                #region WAIT
                case 18:

                    yield return new WaitForSeconds(current_ev.float0);
                    break;
                #endregion

                #region SET FLAG
                case 7:
                    s_globals.SetGlobalFlag(current_ev.string0, current_ev.int0);
                    break;
                #endregion

                #region CHANGE MAP
                case 24:

                    //dialogueChoices.Clear();
                    if (player != null)
                    {
                        player.direction = new Vector2(0, 0);
                        player.rbody2d.velocity = Vector2.zero;
                        player.control = false;
                        player.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;
                    }

                    float t2 = 0;
                    while (fade.color != Color.black)
                    {
                        t2 += Time.deltaTime;
                        fade.color = Color.Lerp(Color.clear, Color.black, t2);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    s_levelloader.LevEd.TriggerSpawn(current_ev.string0, current_ev.string1, player);

                    t2 = 0;
                    while (fade.color != Color.clear)
                    {
                        t2 += Time.deltaTime;
                        fade.color = Color.Lerp(Color.black, Color.clear, t2);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }

                    if (player != null)
                        player.control = true;

                    pointer = -1;
                    doingEvents = false;
                    break;
                #endregion

                #region CHANGE SCENE
                case 20:
                    UnityEngine.SceneManagement.SceneManager.LoadScene(current_ev.int0);
                    break;
                #endregion

                #region SET UTILITY FLAG
                case 12:
                    GameObject utGO = GameObject.Find(current_ev.string0);
                    if (utGO)
                    {
                        s_utility utility = utGO.GetComponent<s_utility>();

                        if (utility != null)
                            utility.eventState = current_ev.int0;
                        else
                            break;
                    }
                    break;
                #endregion

                #region CHECK UTILITY FLAG
                //Make this like the conditional statements where it checks if the utility is in a certain state
                case 17:
                    labelNum = FindLabel(current_ev.string1);
                    GameObject utGO2 = GameObject.Find(current_ev.string0);
                    if (utGO2)
                    {
                        s_utility ut = utGO2.GetComponent<s_utility>();
                        if (ut != null)
                        {
                            if (ut.eventState == current_ev.int0)
                            {
                                pointer = labelNum;
                            }
                        }
                    }
                    break;
                #endregion

                #region ADD CHOICE
                case 28:

                    s_dialogue_choice dialo = new s_dialogue_choice(current_ev.string0, FindLabel(current_ev.string1));

                    if (dialogueChoices != null)
                        dialogueChoices.Add(dialo);
                    else
                    {
                        dialogueChoices = new List<s_dialogue_choice>();
                        dialogueChoices.Add(dialo);
                    }
                    break;
                #endregion

                #region CLEAR CHOICES
                case 29:
                    dialogueChoices.Clear();
                    break;
                #endregion

                #region PRESENT CHOICES
                case 30:

                    int choice = 0, finalchoice = -1;
                    print(choice);

                    while (finalchoice == -1)
                    {
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                            choice--;

                        if (Input.GetKeyDown(KeyCode.DownArrow))
                            choice++;

                        choice = Mathf.Clamp(choice, 0, dialogueChoices.Count - 1);

                        if (Input.GetKeyDown(KeyCode.Z))
                        {
                            print("Chosen");
                            finalchoice = choice;
                        }
                        Dialogue.text = "Arrow keys to scroll, Z to select" + "\n";
                        Dialogue.text += current_ev.string0 + "\n";
                        for (int i = 0; i < dialogueChoices.Count - 1; i++)
                        {
                            if (choice == i)
                                Dialogue.text += "-> ";

                            Dialogue.text += dialogueChoices[i].option + "\n";
                        }
                        print(choice);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    Dialogue.text = "";
                    pointer = dialogueChoices[finalchoice].flagTojump - 1;
                    break;
                #endregion

                #region VACANT FUNCTION
                case 16:
                    break;
                    #endregion

                    /*
                    UTILITY INITIALIZE
                    GameObject utGO1 = GameObject.Find(current_ev.string0);
                    if (utGO1)
                    {
                        s_utility ut = utGO1.GetComponent<s_utility>();
                        if (ut != null)
                        {
                            ut.istriggered = true;
                            ut.EventStart();
                            print("Okay!");
                        }
                        else
                            break;
                    }
                    */
                    /*
                case EVENT_TYPES.SET_OBJ_COLLISION:
                    s_object ob2 = GameObject.Find(current_ev.string0).GetComponent<s_object>();
                    ob2.collision.size = new Vector2(current_ev.int0, current_ev.int1);
                    ob2.collision.offset = new Vector2(current_ev.float0, current_ev.float1);
                    break;

                case EVENT_TYPES.CREATE_OBJECT:

                    s_object ob = leveled.SpawnObject<s_object>(current_ev.string0, new Vector3(current_ev.float0, current_ev.float1), Quaternion.identity);
                    if (current_ev.string1 != "")
                        ob.name = current_ev.string1;
                    break;

                case EVENT_TYPES.DELETE_OBJECT:
                    s_object o = GameObject.Find(current_ev.string0).GetComponent<s_object>();
                    o.DespawnObject();
                    break;

                case EVENT_TYPES.DISPLAY_CHARACTER_HEALTH:

                    bool check = current_ev.stringList.Length < 2;
                    if (current_ev.boolean)
                    {
                        if (check)
                        {
                            if (current_ev.boolean)
                                s_gui.AddCharacter(GameObject.Find(current_ev.stringList[0]).GetComponent<PDII_character>(), false);
                            else
                                s_gui.AddCharacter(GameObject.Find(current_ev.stringList[0]).GetComponent<PDII_character>(), true);
                        }
                        else
                        {
                            List<PDII_character> cha = new List<PDII_character>();
                            foreach (string st in current_ev.stringList)
                            {
                                cha.Add(GameObject.Find(st).GetComponent<PDII_character>());
                            }
                            s_gui.AddCharacter(cha);
                        }

                    }
                    break;
                    */
                    /*
                    case EVENT_TYPES.ADD_CHOICE_OPTION:
                        if (dialogueChoices != null)
                            dialogueChoices.Add(new s_dialogue_choice(current_ev.string0, current_ev.int0));
                        else
                        {
                            dialogueChoices = new List<s_dialogue_choice>();
                            dialogueChoices.Add(new s_dialogue_choice(current_ev.string0, current_ev.int0));
                        }
                        break;

                    case EVENT_TYPES.CHOICE:
                        int choice = 0, finalchoice = -1;
                        print(choice);

                        while (finalchoice == -1)
                        {
                            if (Input.GetKeyDown(KeyCode.UpArrow))
                                choice--;

                            if (Input.GetKeyDown(KeyCode.DownArrow))
                                choice++;

                            choice = Mathf.Clamp(choice, 0, dialogueChoices.Count - 1);

                            if (Input.GetKeyDown(KeyCode.Z))
                            {
                                print("Chosen");
                                finalchoice = choice;
                            }
                            Dialogue.text = "";
                            Dialogue.text += current_ev.string0 + "\n";
                            for (int i = 0; i < dialogueChoices.Count - 1; i++)
                            {
                                if (choice == i)
                                    Dialogue.text += "-> ";

                                Dialogue.text += dialogueChoices[i].option + "\n";
                            }
                            print(choice);
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        Dialogue.text = "";
                        pointer = dialogueChoices[finalchoice].flagTojump - 1;
                        break;
                        */
            }
            yield return new WaitForSeconds(Time.deltaTime);
            //yield return new WaitForSeconds(0.5f);
        }

        public int FindLabel(string labelName)
        {
            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].string0 == labelName)
                {
                    return i;
                }
            }
            return int.MinValue;
        }
    }

    public class s_levelloader : MonoBehaviour
    {
        public o_character player;
        public List<s_map> maps = new List<s_map>();
        public List<TextAsset> jsonMaps = new List<TextAsset>();
        Dictionary<string, Queue<s_object>> objectPoolList = new Dictionary<string, Queue<s_object>>();
        public List<s_pooler_data> objPoolDatabase = new List<s_pooler_data>();
        public List<Sprite> TileSprites = new List<Sprite>();
        public static s_levelloader LevEd;
        public s_map mapDat;
        public List<o_character> allcharacters = new List<o_character>();

        public List<s_map.s_chara> savedcharalist = new List<s_map.s_chara>();
        public o_trigger mapscript;

        int o = 0;  //For selecting enums
        public GameObject prefab;
        public GameObject SceneLevelObject;

        public int id = 0;

        public int boxsize;
        public Vector3 graphsize;
        public Vector3 startpos;
        public LayerMask layerMask;
        dat_save sa;

        s_nodegraph nodegraph;
        public s_save_vector mapsizeToKeep = new s_save_vector(0, 0);
        
        public int current_area;
        protected bool InEditor = true;  //Dictate the loading style
        public List<Tile> tilesNew = new List<Tile>();
        public List<Tile> collisionList = new List<Tile>();
        public Tile collisionTile;
        public Tilemap collisionTiles;

        public Tilemap tm;
        public Tilemap tm2;
        public Tilemap tm3;
        public Tilemap colmp;
        public Canvas canv;

        public GameObject tilesObj;
        public GameObject triggersObj;
        public GameObject entitiesObj;
        public GameObject itemsObj;

        s_triggerhandler triggerHand;
        public float tilesize;

        public void InitializeLoader()
        {
            InEditor = false;
            if (LevEd == null)
                LevEd = this;
            triggerHand = GetComponent<s_triggerhandler>();
            triggerHand.SetTrigObject();
        }

        public o_trigger[] GetTriggerObjects()
        {
            return triggersObj.GetComponentsInChildren<o_trigger>();
        }
        public o_character[] SetCharacterObjects(Predicate<o_character> func)
        {
            return triggersObj.GetComponentsInChildren<o_character>(); 
        }

        public void InitializeGameWorld()
        {
            maps.Clear();

            SetList();

            if (s_mainmenu.isload)
            {
                /*
                if (sa.currentmap == "NULL")
                    sa = s_mainmenu.save;
                */

                foreach (KeyValuePair<string, int> s in s_mainmenu.save.gbflg.Flags)
                {
                    s_globals.SetGlobalFlag(s.Key, s.Value);
                }
                foreach (s_map c in s_mainmenu.save.maps)
                {
                    maps.Add(c);
                }
                LoadMap(maps.Find(x => x.name == s_mainmenu.save.currentmap));
            }
            else
            {
                LoadMaps();
                //print("bnaf");
                LoadMap(maps[0]);
            }
            o_character selobj = null;
            selobj = GameObject.Find("Player").GetComponent<o_character>();

            selobj.health = selobj.maxHealth;

            Screen.SetResolution(1280, 720, false);
        }

        public void LoadMaps()
        {
            foreach (TextAsset asset in jsonMaps)
            {
                s_map cu = JsonUtility.FromJson<s_map>(asset.text);
                cu.name = asset.name;
                maps.Add(cu);
            }
        }

        public void CheckCharacters()
        {
            GameObject mapn = SceneLevelObject;
            o_character[] objectsInMap = null;
            objectsInMap = mapn.transform.Find("Entities").GetComponentsInChildren<o_character>();
            foreach (o_character obj in objectsInMap)
            {
                for (int i = 0; i < mapDat.objectdata.Count; i++)
                {
                    s_map.s_chara characterdat = mapDat.objectdata[i];
                    s_map.s_chara compare = savedcharalist.Find(x => x.charname == characterdat.charname && x.mapname == characterdat.mapname);
                    if (compare != null)
                    {

                        obj.rendererObj.color = Color.white;
                        obj.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;
                        if (!compare.possesed)     //Don't spawn if this character has previously been dead
                        {
                            //obj.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_DEFEAT;
                        }
                        else
                        {
                        }
                    }
                }
            }
        }

        public void SetList()
        {
            for (int i = 0; i < objPoolDatabase.Count; i++)
            {
                Queue<s_object> objque = new Queue<s_object>();
                GameObject obj = objPoolDatabase[i].gameobject;
                s_object newobj = Instantiate(obj).GetComponent<s_object>();
                if (newobj == null)
                {
                    print("Fault at " + obj.name);
                    continue;
                }
                objque.Enqueue(newobj);
                newobj.gameObject.SetActive(false);
                objectPoolList.Add(obj.name, objque);
            }
        }

        public void DespawnObject(s_object obj)
        {
            objectPoolList[obj.ID].Enqueue(obj);
            obj.transform.parent = null;
            obj.gameObject.SetActive(false);
        }

        public s_object[] GetTileObjects()
        {
            return tilesObj.GetComponentsInChildren<s_object>();
        }

        public void AddCharacter(o_character ch)
        {
            allcharacters.Add(ch);
        }

        public T SpawnObject<T>(string obj) where T : s_object
        {
            if (objectPoolList[obj] == null)
            {
                print(obj + " could not be spawned");
            }
            T ob = null;
            if (objectPoolList[obj].Count < 1)
            {
                ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj).gameobject).GetComponent<T>();
                ob.gameObject.SetActive(true);
                ob.ID = obj;
                return ob;
            }

            ob = objectPoolList[obj].Dequeue().GetComponent<T>();
            ob.gameObject.SetActive(true);
            ob.ID = obj;
            return ob;
        }
        public T SpawnObject<T>(string obj, Transform transformp) where T : s_object
        {
            if (objectPoolList[obj] == null)
            {
                print(obj + " could not be spawned");
            }
            T ob = null;
            if (objectPoolList[obj].Count < 1)
            {
                ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj).gameobject, transformp).GetComponent<T>();
                ob.transform.position = transform.position;
                ob.gameObject.SetActive(true);
                ob.ID = obj;
                return ob;
            }

            ob = objectPoolList[obj].Dequeue().GetComponent<T>();
            ob.gameObject.SetActive(true);
            ob.ID = obj;
            return ob;
        }
        public T SpawnObject<T>(string obj, Vector3 pos, Quaternion quant) where T : s_object
        {
            if (objectPoolList[obj] == null)
            {
                print(obj + " could not be spawned");
            }
            //print(obj + " name " + objectPoolList[obj].Count);
            T ob = null;
            s_pooler_data pd = objPoolDatabase.Find(x => x.gameobject.name == obj);
            if (!pd.single)
            {
                if (objectPoolList[obj].Count < 1)
                {
                    ob = Instantiate(pd.gameobject, pos, quant).GetComponent<T>();
                    ob.gameObject.SetActive(true);
                    ob.ID = obj;
                    return ob;
                }

                ob = objectPoolList[obj].Dequeue().GetComponent<T>();
                ob.gameObject.transform.localRotation = quant;
                ob.transform.position = pos;
                ob.gameObject.SetActive(true);
                ob.ID = obj;
            }
            else
            {
                ob = objectPoolList[obj].Peek().GetComponent<T>();
                ob.gameObject.transform.localRotation = quant;
                ob.transform.position = pos;
                ob.gameObject.SetActive(true);
                ob.ID = obj;
            }
            return ob;
        }
        public T SpawnObject<T>(GameObject obj, Vector3 pos, Quaternion quant) where T : s_object
        {

            if (objectPoolList[obj.name] == null)
            {
                print(obj + " could not be spawned");
            }
            // print(obj + " name " + objectPoolList[obj.name].Count);
            T ob = null;
            if (objectPoolList[obj.name].Count < 1)
            {
                ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj.name).gameobject, pos, quant).GetComponent<T>();
                ob.gameObject.SetActive(true);
                ob.ID = obj.name;
                return ob;
            }

            ob = objectPoolList[obj.name].Dequeue().GetComponent<T>();
            ob.gameObject.transform.localRotation = quant;
            ob.transform.position = pos;
            ob.gameObject.SetActive(true);
            ob.ID = obj.name;
            return ob;
        }
        
        public void TriggerSpawn(string string0, string teleporter, o_character chara)
        {
            NewMap();
            s_map thing = maps.Find(x => x.name == string0);
            LoadMap(thing);
            CheckCharacters();

            /*
            s_object[] tilesInMap = null;

            tilesInMap = tilesObj.GetComponentsInChildren<s_object>();

            s_object ma = null;
            foreach (s_object o in tilesInMap)
            {
                o_generic col = o.GetComponent<o_generic>();
                if (col == null)
                    continue;
                if (col.name == teleporter)
                {
                    ma = o;
                    break;
                }
                if (col.GetCollisionType() == COLLISION_T.NONE)
                {
                }
            }
            if (ma != null)
                chara.transform.position = ma.transform.position;
            */
            s_map.s_tileobj to = thing.tilesdata.Find(x => x.TYPENAME == "teleport_object" && x.name == teleporter);
            chara.transform.position = new Vector3(to.pos_x, to.pos_y);
        }
        public void TriggerSpawn(string string0, string teleporter)
        {
            NewMap();
            s_map thing = maps.Find(x => x.name == string0);
            LoadMap(thing);
            CheckCharacters();
            s_map.s_tileobj to = thing.tilesdata.Find(x => x.TYPENAME == "teleport_object" && x.name == teleporter);
            player.transform.position = new Vector3(to.pos_x, to.pos_y);
        }
        public virtual void TriggerSpawn(string teleporter)
        {
            o_character selobj = null;
            selobj = GameObject.Find("Player").GetComponent<o_character>();

            GameObject mapn = SceneLevelObject;
           // s_map.s_tileobj to = thing.tilesdata.Find(x => x.TYPENAME == "teleport_object" && x.name == string0);
            //player.transform.position = new Vector3(to.pos_x, to.pos_y);
        }

        public virtual void NewMap()
        {
            mapscript = null;
            GameObject mapn = SceneLevelObject;
            o_character[] objectsInMap = null;
            o_trigger[] triggersInMap = null;
            o_itemObj[] itemsInMap = null;
            s_utility[] utilitiesInMap = null;
            s_object[] tilesInMap = null;

            tilesInMap = tilesObj.GetComponentsInChildren<s_object>();
            objectsInMap = entitiesObj.GetComponentsInChildren<o_character>();
            triggersInMap = triggersObj.GetComponentsInChildren<o_trigger>();
            utilitiesInMap = triggersObj.GetComponentsInChildren<s_utility>();
            itemsInMap = itemsObj.GetComponentsInChildren<o_itemObj>();

            tm.ClearAllTiles();
            tm2.ClearAllTiles();
            tm3.ClearAllTiles();
            colmp.ClearAllTiles();

            if (utilitiesInMap != null)
            {
                foreach (s_utility obj in utilitiesInMap)
                {
                    //print(obj.ID);
                    if (InEditor)
                        DestroyImmediate(obj.gameObject, true);
                    else
                    {
                        obj.eventState = -1;
                        DespawnObject(obj);
                    }
                }
            }

            if (triggersInMap != null)
            {
                foreach (o_trigger obj in triggersInMap)
                {
                    //print(obj.ID);
                    if (InEditor)
                        DestroyImmediate(obj.gameObject, true);
                    else
                    {
                        obj.isactive = false;
                        DespawnObject(obj);
                    }
                }
            }

            if (objectsInMap != null)
            {
                foreach (s_object obj in objectsInMap)
                {
                    //print(obj.ID);
                    if (InEditor)
                        DestroyImmediate(obj.gameObject, true);
                    else
                    {
                        /*
                        //Save this for OtherMind
                        bool dead = true;// This isn't dead despite the name
                        if (obj.GetComponent<o_character>().CHARACTER_STATE == o_character.CHARACTER_STATES.STATE_DEFEAT)
                            dead = false;

                        //If a save for this character already exists, just replace the data.
                        s_map.s_chara exist = savedcharalist.Find(x => x.charname == obj.name && x.mapname == mapn.name);

                        if (exist != null)
                        {
                            if (mapn.name == "lvl3")
                            {
                                print("BANISHED IN THE NAME OF GOD");
                                DespawnObject(obj);
                                savedcharalist.Remove(exist);
                                continue;
                            }

                            print(obj.name + " is not dead: " + dead);
                            exist.spawnthis = dead;
                            DespawnObject(obj);
                            continue;
                        }

                        savedcharalist.Add(new s_map.s_chara(obj.positioninworld, mapn.name, obj.name, obj.ID, dead, false));
                        */
                        DespawnObject(obj);
                    }
                }
            }

            foreach (s_object obj in tilesInMap)
            {
                //print(obj.ID + " " + mapDat.name);
                if (obj.name == "Teleporter")
                {
                    DespawnObject(obj);
                }
                if (obj.name == "SpawnPoint")
                {
                    continue;
                }
                else
                {
                    if (InEditor)
                        DestroyImmediate(obj.gameObject, true);
                    else
                    {
                        obj.transform.parent = null;
                        DespawnObject(obj);
                    }
                }
            }

            foreach (o_itemObj obj in itemsInMap)
            {
                if (InEditor)
                    DestroyImmediate(obj.gameObject, true);
                else
                {
                    obj.transform.parent = null;
                    DespawnObject(obj);
                }
            }

            /* 
            FileStream fs = new FileStream("Assets/Levels/" + nam + ".txt", FileMode.CreateNew, FileAccess.Write);
            current_map = new s_map("New map " + i);
            current_map.layers.Add(new s_maplayer());
            GameObject o = Instantiate(base_map, transform.position, Quaternion.identity);
            o.name = current_map.name;
            
            if (GameObject.Find(maps[current_area].name) != null)
                 GameObject.Find(maps[current_area].name).SetActive(false);
             else
             {
                // GameObject.Find("New Level").SetActive(false);
             }
             s_map map = PrefabUtility.CreatePrefab("Assets/Levels/" + "New map " + i + ".prefab", base_map.gameObject).GetComponent<s_map>();
             maps.Add(map);
             */
        }

        public void SaveMap(string dir)
        {
            string mapdat = JsonUtility.ToJson(GetMap());
            print(mapdat);

            File.WriteAllText(dir, mapdat);
        }
        public s_map GetMap()
        {
            s_map mapfil = new s_map();
            if (mapscript != null)
                mapfil.mapscript = mapscript.name;

            triggerHand = GetComponent<s_triggerhandler>();
            GameObject mapn = SceneLevelObject;
            o_character[] objectsInMap = null;
            s_object[] triggersInMap = null;
            o_itemObj[] itemsInMap = null;
            s_object[] tilesInMap = null;
            mapfil.mapsize = mapsizeToKeep;
            //print("Map size: "+ mapfil.mapsize.x + ", " + mapfil.mapsize.y);

            tilesInMap = tilesObj.GetComponentsInChildren<s_object>();
            objectsInMap = entitiesObj.GetComponentsInChildren<o_character>();
            triggersInMap = triggersObj.GetComponentsInChildren<s_object>();
            itemsInMap = itemsObj.GetComponentsInChildren<o_itemObj>();

            //print("Saving items...");
            mapfil.itemdat = GetItems(itemsInMap);

            //print("Saving triggers...");
            mapfil.triggerdata = GetTriggers(triggersInMap);
            mapfil.Map_Script = triggerHand.Events;
            
            mapfil.objectdata = GetEntities(objectsInMap);

            mapfil.graphicTiles.Clear();

            mapfil.tilesdata.AddRange(GetTiles(tilesInMap));
            GetTileDat(ref mapfil);
            print(mapfil.tilesdata.Count);

            print(mapfil.tilesdata);

            return mapfil;
        }

        public s_map JsonToObj(string directory)
        {
            string jso = File.ReadAllText(directory);
            return JsonUtility.FromJson<s_map>(jso);
        }
        
        public virtual List<s_save_item> GetItems(o_itemObj[] itemsInMap)
        {
            return new List<s_save_item>();
        }

        public virtual List<s_map.s_tileobj> GetTiles(s_object[] tiles)
        {
            return new List<s_map.s_tileobj>();
        }

        public virtual List<s_map.s_chara> GetEntities(o_character[] characters)
        {
            return new List<s_map.s_chara>();
        }

        public virtual List<s_map.s_trig> GetTriggers(s_object[] triggers)
        {
            return new List<s_map.s_trig>();
        }

        public virtual GameObject SetTrigger(s_map.s_trig trigger)
        {
            return null;
        }
        public virtual void SetItem(s_save_item item)
        {
        }

        public void LoadMap(s_map mapdat)
        {
            allcharacters.Clear();

            if (player != null)
                allcharacters.Add(player);
            else
                print("Please assign or add a player.");

            mapscript = null;
            List<Tuple<GameObject, List<s_map.s_customType>>> depndencyReq = new List<Tuple<GameObject, List<s_map.s_customType>>>();
            mapDat = mapdat;
            //current_area = nam;
            if (mapdat == null)
                return;
            nodegraph = GetComponent<s_nodegraph>();

            triggerHand = GetComponent<s_triggerhandler>();
            
            mapsizeToKeep = mapdat.mapsize;

            nodegraph.size = new Vector2Int((int)mapsizeToKeep.x, (int)mapsizeToKeep.y);

            #region SPAWN_ENTITIES
            /*
            for (int i = 0; i < mapdat.objectdata.Count; i++)
            {
                s_map.s_chara characterdat = mapdat.objectdata[i];
                Vector2 characterPos = new Vector2(characterdat.pos_x, characterdat.pos_y);

                o_character trig = null;
                if (InEditor)
                {
                    print(mapdat.objectdata[i].charType);
                    if (mapdat.objectdata[i].charType == "")
                        continue;
                    if (FindOBJ<o_character>(characterdat.charType) == null)
                    {
                        print("Couldn't find object '" + characterdat.charType + "' in the pool, please add it to the pooler.");
                        continue;
                    }
                    print(characterdat.charType);
                    trig = Instantiate<o_character>(FindOBJ<o_character>(characterdat.charType), characterPos, Quaternion.identity);
                }
                else
                {
                    print(mapdat.objectdata[i].charType);
                    if (characterdat.charType == "")
                        continue;
                    print("Spawned " + characterdat.charType);
                    trig = SpawnObject<o_character>(characterdat.charType, characterPos, Quaternion.identity);
                }
                //trig.map_origin = mapDat.name;
                trig.control = true;
                trig.SetSpawnPoint(characterPos);
                trig.name = characterdat.charname;
                trig.transform.position = new Vector3(characterdat.pos_x, characterdat.pos_y, 1);
                trig.transform.SetParent(entitiesObj.transform);

                allcharacters.Add(trig);
            }
            */
            for (int i = 0; i < mapdat.objectdata.Count; i++)
            {
                s_map.s_chara characterdat = mapdat.objectdata[i];
                SetEntity<o_character>(characterdat);
            }
            SetEntities(mapdat.objectdata);
#endregion
            
            #region SPAWN_TRIGGERS
            for (int i = 0; i <
                mapdat.triggerdata.Count;
                i++)
            {
                GameObject trigObj = null;
                s_map.s_trig t = mapdat.triggerdata[i];
                print(t.util);
                trigObj = SetTrigger(t);
                if (trigObj != null)
                {
                    if (t.requiresDependency)
                        depndencyReq.Add(new Tuple<GameObject, List<s_map.s_customType>>(trigObj, t.CustomTypes));
                    trigObj.transform.SetParent(triggersObj.transform);
                }
            }
            if (!InEditor)
            {
                if (allcharacters != null)
                    foreach (o_character c in allcharacters)
                    {
                        if (c != null)
                            c.AddFactions(allcharacters);
                    }
            }
            player.AddFactions(allcharacters);
#endregion

            if (triggerHand != null)
                triggerHand.GetMapEvents(mapdat.map_script_labels, mapdat.Map_Script);
            
            #region SPAWN_GRAPHIC_TILES
            /*
            foreach (s_map.s_block b in mapdat.graphicTiles)
            {
                if (InEditor)
                {
                    o_tile t = Instantiate(FindOBJ("TileDecor"), new Vector3(b.position.x, b.position.y), Quaternion.identity).GetComponent<o_tile>();
                    t.SpirteRend = t.GetComponent<SpriteRenderer>();
                    t.SpirteRend.sprite = TileSprites.Find(til => til.name == b.sprite);
                    t.layer = (int)b.layer;
                    t.Intialize();
                    t.name = "TileDecor";
                    t.transform.SetParent(tileIG.transform);
                }
                else
                {
                    o_tile t = SpawnObject("TileDecor", new Vector3(b.position.x, b.position.y), Quaternion.identity).GetComponent<o_tile>();
                    t.SpirteRend = t.GetComponent<SpriteRenderer>();
                    t.SpirteRend.sprite = TileSprites.Find(til => til.name == b.sprite);
                    t.layer = (int)b.layer;
                    t.Intialize();
                    t.name = "TileDecor";
                    t.transform.SetParent(tileIG.transform);
                }
            }
            */
#endregion
            
            #region SPAWN_TILES
            /*
            for (int i = 0; i < mapdat.tilesdata.Count; i++)
            {
                s_map.s_tileobj ma = mapdat.tilesdata[i];
                s_object trig = null;
                GameObject targname = null;
                //print((s_map.s_mapteleport)mapdat.tilesdata[i]);
                if (InEditor)
                {
                    switch (mapdat.tilesdata[i].TYPENAME)
                    {
                        default:
                            continue;
                            
                    }
                }
                else
                {

                    switch (mapdat.tilesdata[i].TYPENAME)
                    {

                    }
                }

                if (trig == null)
                    continue;
                if (trig.GetComponent<o_generic>())
                {
                    o_generic col = trig.GetComponent<o_generic>();

                    col.character = mapdat.tilesdata[i].exceptionChar;
                    if (mapdat.tilesdata[i].TYPENAME == "teleport_object")
                    {
                        col.name = mapdat.tilesdata[i].name;
                    }
                    col.collision_type = (COLLISION_T)mapdat.tilesdata[i].enumthing;
                    if (!mapdat.tilesdata[i].issolid)
                    {
                        col.characterCannot = mapdat.tilesdata[i].cannotPassChar;
                        col.issolid = false;
                    }
                    else
                    {
                        col.issolid = true;
                        col.characterCannot = null;
                    }

                    SpriteRenderer spr = trig.GetComponent<SpriteRenderer>();
                    BoxCollider2D bx = trig.GetComponent<BoxCollider2D>();
                    if (bx)
                        bx.size = new Vector2(ma.size.x, ma.size.y);
                }
                if (trig.GetComponent<o_maptransition>())
                {
                    o_maptransition col = trig.GetComponent<o_maptransition>();
                    if (ma.flagchecks != null)
                    {
                        col.flagcheck = ma.flagname;
                        col.flags = new o_maptransition.s_flagcheck[ma.flagchecks.Length];
                        for (int a = 0; a < ma.flagchecks.Length; a++)
                        {
                            col.flags[a] = new o_maptransition.s_flagcheck(ma.flagchecks[a], ma.mapnames[a]);
                        }
                    }
                    //print(col);

                    col.position = new Vector2(ma.teleportpos.x, ma.teleportpos.y);
                    col.sceneToTransferTo = ma.mapname;
                    col.areaInScene = ma.areaname;
                }
                trig.transform.position = new Vector3(ma.pos_x, ma.pos_y, 0);
                trig.transform.SetParent(tilesObj.transform);

                if (targname != null)
                    trig.name = targname.name;
            }
            */
#endregion

            SetTileMap(mapdat);

            #region SPAWN_ITEMS
            for (int i = 0; i < mapdat.itemdat.Count; i++)
            {
                s_save_item item = mapdat.itemdat[i];
                SetItem(item);
            }
#endregion

            foreach (o_character c in allcharacters)
            {
                if (c != null)
                    c.AddFactions(allcharacters);
            }

            #region ADD IN OBJECT DEPENDENCIES
            SetTriggerDependency(ref depndencyReq);
            #endregion

            if (nodegraph != null)
                nodegraph.CreateNodeArray(mapdat.tilesdata.ToArray());
            else
                print("Please include the nodegraph.");

            if (mapdat.OnMapScriptCall)
                triggerHand.JumpToEvent(mapdat.onMapScriptCallLabel, false);
        }

        public virtual void SetTriggerDependency(ref List<Tuple<GameObject, List<s_map.s_customType>>> triggers)
        {
        }

        /// <summary>
        /// Depricated function, use "SetEntity instead"
        /// </summary>
        /// <param name="characters"></param>
        public virtual void SetEntities(List<s_map.s_chara> characters)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                s_map.s_chara characterdat = characters[i];
                Vector2 characterPos = new Vector2(characterdat.pos_x, characterdat.pos_y);

                o_character trig = null;
                if (InEditor)
                {
                    if (characterdat.charType == "")
                        continue;
                    if (FindOBJ(characterdat.charType) == null)
                    {
                        print("Couldn't find object '" + characterdat.charType + "' in the pool, please add it to the pooler.");
                        continue;
                    }
                    trig = Instantiate<o_character>(FindOBJ<o_character>(characterdat.charType), characterPos, Quaternion.identity);
                }
                else
                {
                    //print(characters[i].charType);
                    if (characterdat.charType == "")
                        continue;
                    //TODO find character equal to the id and spawn that
                    if (characters[i].possesed)
                        continue;
                    trig = SpawnObject<o_character>(characters[i].charType, characterPos, Quaternion.identity);

                }
                trig.control = true;
                trig.SetSpawnPoint(characterPos);
                trig.name = characterdat.charname;
                trig.transform.position = new Vector3(characterdat.pos_x, characterdat.pos_y, 1);
                trig.transform.SetParent(entitiesObj.transform);

                allcharacters.Add(trig);
            }
        }

        /// <summary>
        /// This is used to spawn the entities, you can overide this function if you want to set custom parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="character"></param>
        /// <returns></returns>
        public virtual T SetEntity<T>(s_map.s_chara character) where T : o_character
        {
            s_map.s_chara characterdat = character;
            Vector2 characterPos = new Vector2(characterdat.pos_x, characterdat.pos_y);

            T trig = null;
            if (InEditor)
            {
                if (characterdat.charType == "")
                    return null;
                if (FindOBJ(characterdat.charType) == null)
                {
                    print("Couldn't find object '" + characterdat.charType + "' in the pool, please add it to the pooler.");
                    return null;
                }
                trig = Instantiate<T>(FindOBJ<T>(characterdat.charType), characterPos, Quaternion.identity);
            }
            else
            {
                //print(characters[i].charType);
                if (characterdat.charType == "")
                    return null;
                trig = SpawnObject<T>(characterdat.charType, characterPos, Quaternion.identity);
            }
            trig.control = true;
            trig.SetSpawnPoint(characterPos);
            trig.name = characterdat.charname;
            trig.transform.position = new Vector3(characterdat.pos_x, characterdat.pos_y, 0);
            trig.transform.SetParent(entitiesObj.transform);

            allcharacters.Add(trig);
            return trig;
        }

        public virtual void SetTileMap(s_map mapdat)
        {
            s_map mp = mapdat;
            List<s_map.s_block> tile = mp.graphicTiles;
            List<s_map.s_block> tileMid = mp.graphicTilesMiddle;
            List<s_map.s_block> tileTop = mp.graphicTilesTop;
            List<s_map.s_tileobj> coll = mp.tilesdata;

            foreach (s_map.s_block b in tile)
            {
                Tile til = tilesNew.Find(ti => ti.name == b.sprite);
                if (til == null)
                    continue;
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(b.rotation.x, b.rotation.y, b.rotation.z), Vector3.one);

                Vector3Int v = new Vector3Int((int)b.position.x / (int)tilesize, (int)b.position.y / (int)tilesize, 0);

                tm.SetTransformMatrix(v, matrix);
                tm.SetTile(v, til);
            }
            foreach (s_map.s_block b in tileMid)
            {
                Tile til = tilesNew.Find(ti => ti.name == b.sprite);
                if (til == null)
                    continue;
                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(b.position.x, b.position.y), Quaternion.Euler(b.rotation.x, b.rotation.y, b.rotation.z), Vector3.one);

                Vector3Int v = new Vector3Int((int)b.position.x / (int)tilesize, (int)b.position.y / (int)tilesize, 0);

                tm2.SetTile(v, til);
                tm2.SetTransformMatrix(v, matrix);
            }
            foreach (s_map.s_block b in tileTop)
            {
                Tile til = tilesNew.Find(ti => ti.name == b.sprite);
                if (til == null)
                    continue;
                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(b.position.x, b.position.y), Quaternion.Euler(b.rotation.x, b.rotation.y, b.rotation.z), Vector3.one);

                Vector3Int v = new Vector3Int((int)b.position.x / (int)tilesize, (int)b.position.y / (int)tilesize, 0);

                tm3.SetTile(v, til);
                tm3.SetTransformMatrix(v, matrix);
            }
        }
        public virtual void GetTileDat(ref s_map mapfil)
        {
            Tile[] tiles = new Tile[(int)mapsizeToKeep.x * (int)mapsizeToKeep.y];
            Tile[] colTiles = new Tile[(int)mapsizeToKeep.x * (int)mapsizeToKeep.y];
            Vector2Int vec = new Vector2Int(0, 0);

            for (int x = 0; x < mapsizeToKeep.x; x++)
            {
                for (int y = 0; y < mapsizeToKeep.y; y++)
                {
                    Tile coltil = colmp.GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (coltil != null)
                    {
                        colTiles[(int)(x + (mapsizeToKeep.x * y))] = coltil;
                        if (colTiles[(int)(x + (mapsizeToKeep.x * y))] != null)
                        {
                            string tileName = coltil.name;
                            s_map.s_tileobj tilo = new s_map.s_tileobj(new Vector2(x * tilesize, y * tilesize), null);
                            tilo.name = tileName;
                        }

                    }

                    Tile til = tm.GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (til != null)
                    {
                        s_map.s_block tilo = new s_map.s_block(til.name,
                                   new Vector2(x * tilesize, y * tilesize));
                        Vector3 m = tm.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation.eulerAngles;
                        tilo.rotation = new s_save_vector(m.x, m.y, m.z);
                        mapfil.graphicTiles.Add(tilo);
                    }

                    Tile tilmid = tm2.GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (tilmid != null)
                    {
                        s_map.s_block tilo = new s_map.s_block(tilmid.name,
                                   new Vector2(x * tilesize, y * tilesize));
                        Vector3 m = tm2.GetTransformMatrix(new Vector3Int(x,y,0)).rotation.eulerAngles;
                        tilo.rotation = new s_save_vector(m.x, m.y, m.z);
                        //print("Middle: " + new Vector3(tilo.rotation.x, tilo.rotation.y, tilo.rotation.z));
                        mapfil.graphicTiles.Add(tilo);
                    }

                    Tile tiltop = tm3.GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (tiltop != null)
                    {
                        s_map.s_block tilo = new s_map.s_block(tiltop.name,
                                   new Vector2(x * tilesize, y * tilesize));
                        Vector3 m = tm3.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation.eulerAngles;
                        tilo.rotation = new s_save_vector(m.x, m.y, m.z);
                        mapfil.graphicTiles.Add(tilo);
                    }
                }
            }
            /*
            Tile[] tiles = new Tile[(int)mapDat.mapsize.x * (int)mapDat.mapsize.y];
            Tile[] colTiles = new Tile[(int)mapDat.mapsize.x * (int)mapDat.mapsize.y];
            Vector2Int vec = new Vector2Int(0, 0);

            for (int x = 0; x < mapDat.mapsize.x; x++)
            {
                for (int y = 0; y < mapDat.mapsize.y; y++)
                {
                    Tile coltil = colmp.GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (coltil != null)
                    {
                        colTiles[(int)(x + (mapDat.mapsize.y * y))] = coltil;
                        if (colTiles[(int)(x + (mapDat.mapsize.y * y))] != null)
                        {
                            string tileName = coltil.name;
                            s_map.s_tileobj tilo = new s_map.s_tileobj(new Vector2(x * 20, y * 20), null);
                            tilo.name = tileName;

                            tilo.tilemapPos = new s_save_vector();
                            mapfil.tilesdata.Add(tilo);
                        }

                    }

                    Tile til = tm.GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (til != null)
                    {
                        mapfil.graphicTiles.Add(
                                   new s_map.s_block(til.sprite.name,
                                   new Vector2(x * 20, y * 20),
                                   0));
                    }

                    Tile tilmid = tm2.GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (tilmid != null)
                    {
                        mapfil.graphicTilesMiddle.Add(
                                   new s_map.s_block(tilmid.sprite.name,
                                   new Vector2(x * 20, y * 20),
                                   0));
                    }

                    Tile tiltop = tm3.GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (tiltop != null)
                    {
                        mapfil.graphicTilesTop.Add(
                                   new s_map.s_block(tiltop.sprite.name,
                                   new Vector2(x * 20, y * 20),
                                   0));
                    }
                }
            }
            */
        } 

        public virtual s_map.s_trig AddTrigger(o_trigger obj)
        {
            return null;
        }

        public GameObject FindOBJ(string obname) 
        {
            foreach (s_pooler_data ga in objPoolDatabase)
            {
                if (ga.gameobject.name == obname)
                {
                    return ga.gameobject;
                }
            }
            return null;
        }

        public T FindOBJ<T>(string obname) where T : s_object
        {
            foreach (s_pooler_data ga in objPoolDatabase)
            {
                if (ga.gameobject.name == obname)
                {
                    return ga.gameobject.GetComponent<T>();
                }
            }
            return null;
        }

        protected void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(new Vector3((mapsizeToKeep.x * tilesize) / 2, (mapsizeToKeep.y * tilesize) / 2), new Vector3(mapsizeToKeep.x * tilesize, mapsizeToKeep.y * tilesize));
        }
    }

    public class s_soundmanager : MonoBehaviour
    {
        public static s_soundmanager sound;
        public AudioSource src;
        public AudioSource music;

        void Start()
        {
            src = GetComponent<AudioSource>();
            sound = GetComponent<s_soundmanager>();
        }

        public void PlaySound(ref AudioClip sound, bool BGM)
        {
            if (sound == null)
                return;
            src.PlayOneShot(sound);
        }
        public void PlaySound(ref AudioClip sound, float volume, bool isBGM)
        {
            if (sound == null)
                return;
            if (!isBGM)
            {
                src.PlayOneShot(sound, volume);
            }
        }

        public void Update()
        {
        }
        
    }

}
/*
dat = levedatabase.GetComponent<s_leveldatabase>();
dat.maps.Add(mapfil);

dat = null;
*/
