using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;

namespace MagnumFoudation
{
    using System.Linq;
    using System.Reflection;
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
                   // print("falling to " + n);
                    return newPos.y;
                }
                n++;
            }
            //print("falling to " + n);
            return newPos.y;
        }
        
        public void CreateNodeArray(s_map.s_tileobj[] ti)
        {
            s_node[] arr = new s_node[size.x * size.y];
           // print(size);
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
    public class dat_save
    {
        public enum TYPEOFVAL { INT, STRING, FLOAT, SHORT, UINT };
        
        public dynamic FindData(string n) {
            return variables.Find(x => x.Item1 == n).Item2;
        }
        public string FindDataST(string n)
        {
            return stringVariables.Find(x => x.Item1 == n).Item2;
        }
        public short FindDataSH(string n)
        {
            return shortVariables.Find(x => x.Item1 == n).Item2;
        }
        public int FindDataI(string n)
        {
            return intVariables.Find(x => x.Item1 == n).Item2;
        }
        public float FindDataF(string n)
        {
            return floatVariables.Find(x => x.Item1 == n).Item2;
        }
        public uint FindDataUI(string n)
        {
            return uintVariables.Find(x => x.Item1 == n).Item2;
        }
        public ushort FindDataUS(string n)
        {
            return ushortVariables.Find(x => x.Item1 == n).Item2;
        }

        public void AddStrings() { }

        public dat_save()
        {
        }
        public dat_save(dat_globalflags gbflg, int health, int MAXhp, string currentmap, Vector2 location)
        {
            hp = health;
            this.gbflg = gbflg;
            this.MAXhp = MAXhp;
            this.currentmap = currentmap;
            this.location = new s_save_vector(location.x, location.y);
            variables = null;
            stringVariables = null;
            floatVariables = null;
            intVariables = null;
            shortVariables = null;
            uintVariables = null;
            ushortVariables = null;
        }
        public int hp;
        public int MAXhp;
        public string currentmap;
        public dat_globalflags gbflg;
        public s_save_vector location;

        public List<Tuple<string, dynamic>> variables;
        public List<Tuple<string, string>> stringVariables;
        public List<Tuple<string, float>> floatVariables;
        public List<Tuple<string, int>> intVariables;
        public List<Tuple<string, short>> shortVariables;
        public List<Tuple<string, uint>> uintVariables;
        public List<Tuple<string, ushort>> ushortVariables;
    }

    public class s_globals : MonoBehaviour
    {
        public Dictionary<string, s_map> maps = new Dictionary<string, s_map>();
        public string currentlevelname;

        public static s_globals globalSingle;

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
            CALL_MAP_SCRIPT,
            SAVE
        }
        EDITOR_MODE EDIT;

        s_map curmap;
        Vector2 scroll = new Vector2(0, 0);
        public o_character player;
        public static bool isPause = false;
        int map_menu_select = 0;

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
        public string[] allScenes;

        public static void SetButtonKeyPref(string buttonName, KeyCode keyCode)
        {
            PlayerPrefs.SetInt(buttonName, (int)keyCode);
        }
        public static void SetButtonKey(string buttonName, KeyCode keyCode)
        {
            if (!arrowKeyConfig.ContainsKey(buttonName))
            {
                arrowKeyConfig.Add(buttonName, keyCode);
                return;
            }
            arrowKeyConfig[buttonName] = keyCode;
        }

        public static void SetGlobalFlag(string flagname, int flag)
        {
            if (!GlobalFlags.ContainsKey(flagname))
            {
                //print("Flag " + flagname + " does not exist, Creating flag");
                GlobalFlags.Add(flagname, flag);
                return;
            }
            GlobalFlags[flagname] = flag;
            //print("Flag " + flagname + "  set to " + flag);
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

        public static KeyCode GetKeyPref(string keyName)
        {
            if (PlayerPrefs.HasKey(keyName))
            {
                return (KeyCode)PlayerPrefs.GetInt(keyName);
            }
            return KeyCode.None;
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
            SetButtonKeyPref("left", KeyCode.A);
            SetButtonKeyPref("right", KeyCode.D);
            SetButtonKeyPref("down", KeyCode.S);
            SetButtonKeyPref("up", KeyCode.W);
            SetButtonKeyPref("jump", KeyCode.Space);
            SetButtonKeyPref("dash", KeyCode.LeftShift);
            SetButtonKeyPref("select", KeyCode.E);
        }

        public void Awake()
        {
            if(globalSingle == null)
                globalSingle = this;
            //AddKeys();
            if (isMainGame)
            {
                isFixedSaveArea = isFixedSaveAreaInput;
                fixedSaveAreaName = fixedSaveAreaNameInput;

                /*
                if (arrowKeyConfig.Count == 0)
                    AddKeys();
                */
                if(GetComponent<s_levelloader>() != null)
                    ed = GetComponent<s_levelloader>();
                foreach (ev_integer e in GlobalFlagCache)
                {
                    if (!GlobalFlags.ContainsKey(e.integer_name))
                        GlobalFlags.Add(e.integer_name, e.integer);
                }
                if (!EnableDebug)
                {
                    if(s_levelloader.LevEd != null)
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
        }

        public void LoadFlag(dat_globalflags flag)
        {
            GlobalFlags = flag.Flags;
        }

        public virtual void SaveData()
        {
            FileStream fs = new FileStream("save.MF", FileMode.Create);
            BinaryFormatter bin = new BinaryFormatter();
            s_levelloader lev = GameObject.Find("General").GetComponent<s_levelloader>();

            dat_save sav = new dat_save(new dat_globalflags(GlobalFlags), (int)player.health, (int)player.maxHealth, lev.mapDat.name, player.transform.position);

            if (isFixedSaveArea)
                sav.currentmap = fixedSaveAreaName;

            bin.Serialize(fs, sav);
            fs.Close();
        }

        public static void Pause()
        {
            Time.timeScale = 0;
            if (s_levelloader.LevEd != null) {

                foreach (o_character c in s_levelloader.LevEd.allcharacters)
                {
                    c.enabled = false;
                }
            }
            else if(s_mapManager.LevEd.mapEvHolder != null) {

                foreach (o_character c in s_levelloader.LevEd.mapEvHolder.allChacacters)
                {
                    c.enabled = false;
                }
            }
        }
        public static void Resume()
        {
            Time.timeScale = 1;
            if (s_levelloader.LevEd != null)
            {

                foreach (o_character c in s_levelloader.LevEd.allcharacters)
                {
                    c.enabled = true;
                }
            }
            else if (s_mapManager.LevEd.mapEvHolder != null)
            {

                foreach (o_character c in s_mapManager.LevEd.mapEvHolder.allChacacters)
                {
                    c.enabled = true;
                }
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPause)
                {
                    isPause = true;
                    Pause();
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
                    Resume();
                }
            }
            if (isMainGame)
            {
                if (EnableDebug)
                {
                    if (DEBUG_MODE_ON)
                    {
                        if (GUI.Button(new Rect(0, 0, 120, 40), "Debug Mode Off"))
                        {
                            if (s_levelloader.LevEd != null) {

                                o_trigger[] trigs = s_levelloader.LevEd.GetTriggerObjects();

                                foreach (o_trigger t in trigs)
                                {
                                    t.rendererObj.color = Color.clear;
                                }
                            }
                            DEBUG_MODE_ON = false;
                        }
                        if (GUI.Button(new Rect(0, 60, 120, 40), EDIT.ToString()))
                        {
                            EDIT++;
                            EDIT = (EDITOR_MODE)(int)Mathf.Clamp((float)EDIT, 0, 6);
                            if ((int)EDIT == 6)
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
                                    GUI.Label(new Rect(0, 50 * ind, 90, 40), flag.Key + " Value: " + flag.Value, GUIStyle.none);
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

                                if (s_levelloader.LevEd != null) {

                                    if (s_levelloader.LevEd.LoadScene)
                                    {
                                        if (allScenes.Length > 80)
                                        {
                                            if (GUI.Button(new Rect(355, 50 * 1, 50, 50), "<"))
                                            {
                                                map_menu_select--;
                                            }
                                            if (GUI.Button(new Rect(405, 50 * 1, 50, 50), ">"))
                                            {
                                                map_menu_select++;
                                            }
                                            map_menu_select = Mathf.Clamp(map_menu_select, 0, 5);
                                        }

                                        for (int i = 0; i < 80; i++)
                                        {
                                            if (i + (80 * map_menu_select) > allScenes.Length - 1)
                                            {
                                                break;
                                            }
                                            if (GUI.Button(new Rect(160, 50 * (y + 2), 160, 50), allScenes[i]))
                                            {
                                                s_levelloader.LevEd.TriggerSpawn(allScenes[i], "");
                                            }
                                            if (y == 10)
                                            {
                                                y = 0;
                                                x++;
                                            }
                                            y++;
                                        }
                                    }
                                    else
                                    {
                                        List<s_map> maps = s_levelloader.LevEd.maps;
                                        if (maps.Count > 80)
                                        {
                                            if (GUI.Button(new Rect(355, 50 * 1, 50, 50), "<"))
                                            {
                                                map_menu_select--;
                                            }
                                            if (GUI.Button(new Rect(405, 50 * 1, 50, 50), ">"))
                                            {
                                                map_menu_select++;
                                            }
                                            map_menu_select = Mathf.Clamp(map_menu_select, 0, 5);
                                        }

                                        for (int i = 0; i < 80; i++)
                                        {
                                            if (i + (80 * map_menu_select) > maps.Count - 1)
                                            {
                                                break;
                                            }
                                            s_map ma = maps[i + (80 * map_menu_select)];
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
                                    }
                                } 
                                else {

                                    if (allScenes.Length > 9)
                                    {
                                        if (GUI.Button(new Rect(355, 50 * 1, 50, 50), "<"))
                                        {
                                            map_menu_select--;
                                        }
                                        if (GUI.Button(new Rect(405, 50 * 1, 50, 50), ">"))
                                        {
                                            map_menu_select++;
                                        }
                                        float lengA = 9;
                                        float lengB = allScenes.Length;

                                        map_menu_select = Mathf.Clamp(map_menu_select, 0, (int)(lengB / lengA));
                                    }

                                    for (int i = 0; i < 9; i++)
                                    {
                                        if (i + (9 * map_menu_select) > allScenes.Length - 1)
                                        {
                                            break;
                                        }
                                        string ma = allScenes[i + (9 * map_menu_select)];
                                        if (GUI.Button(new Rect(160, 50 * (y + 2), 160, 50), ma))
                                        {
                                            s_mapManager.LevEd.TriggerSpawn(ma, "");
                                        }
                                        y++;
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
                                else
                                {

                                    if (GUI.Button(new Rect(0, 100, 120, 40), "Current script: Static Script"))
                                    {
                                        CALL_STATIC_SCRIPT = false;
                                    }
                                }

                                List<ev_details> currentScript = null;

                                if (CALL_STATIC_SCRIPT)
                                    currentScript = s_triggerhandler.trig.StaticEvents;
                                else
                                    currentScript = s_triggerhandler.trig.Events;

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
                                        s_triggerhandler.trig.JumpToEvent(0, CALL_STATIC_SCRIPT);
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
                                            s_triggerhandler.trig.JumpToEvent(i, CALL_STATIC_SCRIPT);
                                        }
                                        ind++;
                                        y++;
                                    }
                                }
                                GUI.EndScrollView();
                                break;

                            case EDITOR_MODE.SAVE:
                                if (GUI.Button(new Rect(200, 50 * ind, 90, 40), "Save data"))
                                {
                                    SaveData();
                                }
                                break;
                        }

                    }
                    else
                    {
                        if (GUI.Button(new Rect(0, 0, 120, 40), "Debug Mode"))
                        {
                            DEBUG_MODE_ON = true;
                            if (s_levelloader.LevEd != null) {

                                o_trigger[] trigs = s_levelloader.LevEd.GetTriggerObjects();

                                foreach (o_trigger t in trigs)
                                {
                                    t.rendererObj.color = Color.white;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //player.IS_KINEMATIC = true;
                    /*
                    if (ed != null)
                        if (ed.colmp != null)
                            ed.colmp.color = Color.clear;
                    */
                    COLLISION_SHOW = false;
                }
            }
        }
    }

    [System.Serializable]
    public class s_pathNode
    {
        public List<int> neighbours = new List<int>();

        public bool isLight;
        public bool isgoal = false;

        public s_pathNode parent;
        public int list;
        public Vector2 position;
        public int id;
        public float g_cost = 0, h_cost = 0;
        public float f_cost
        {
            get
            {
                return g_cost + h_cost;
            }
        }

        public s_pathNode(int id, Vector2 position, List<int> path)
        {
            neighbours = path;
            this.id = id;
            this.position = position;
        }
        public s_pathNode()
        {
        }
    }

    public class s_pathfind : MonoBehaviour
    {
        public List<s_pathNode> nodes = new List<s_pathNode>();

        public void SetNodes(ref List<s_pathNode> nodes)
        {
            this.nodes = nodes;
        }
        public void ResetNodes()
        {
            foreach (s_pathNode no in nodes)
            {
                no.g_cost = 0;
                no.h_cost = 0;
                no.parent = null;
                no.list = 0;
                no.isgoal = false;
                no.isLight = false;
            }
        }

        void Start()
        {

        }

        void Update()
        {

        }
        private void OnDrawGizmos()
        {
            /*
            foreach (s_pathNode nod in nodes)
            {
                Vector3 nodepos = new Vector3(nod.position.x, nod.position.y);

                if (nod.isgoal)
                    Handles.Label(nodepos, "Goal");

                if (nod.parent != null)
                    Handles.Label(nodepos, "Step: " + nod.list);
                Handles.Label(nodepos, "ID: " + nod.id);

                Gizmos.DrawSphere(nodepos, 20);
                if (nod.neighbours != null)
                {
                    foreach (int idN in nod.neighbours)
                    {
                        s_pathNode neighbour = nodes.Find(x => x.id == idN);
                        Vector3 neighPos = new Vector3(neighbour.position.x, neighbour.position.y);
                        Gizmos.DrawLine(nodepos, neighPos);
                    }
                }
            }
            */
        }

        public s_pathNode FindPathNode(int id)
        {
            foreach (s_pathNode p in nodes)
            {
                if (p.id == id)
                    return p;
            }
            return null;
        }

        public Tuple<List<s_pathNode>, float> PathFind(Vector2 start, Vector2 end)
        {
            ResetNodes();
            int round = 0;
            List<s_pathNode> openList = new List<s_pathNode>();
            HashSet<s_pathNode> closedList = new HashSet<s_pathNode>();

            s_pathNode startNode = FindNodeAtPosition(start);
            s_pathNode goal = FindNodeAtPosition(end);

            if (goal == null)
                return null;

            goal.isgoal = true;

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                if (round == 200)
                    return null;

                s_pathNode current = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    s_pathNode pa = openList[i];
                    if (pa.f_cost < current.f_cost)
                    {
                        //FindPathNode(pa.id)
                        current = FindNodeAtPosition(pa.position);
                    }
                }

                if (current.id == goal.id)
                {
                    List<s_pathNode> pathList = new List<s_pathNode>();
                    s_pathNode path = goal;
                    int l = 0;
                    while (path != startNode)
                    {
                        path.list = l;
                        pathList.Add(path);
                        l++;
                        path = path.parent;
                    }
                    pathList.Reverse();
                    return new Tuple<List<s_pathNode>, float>(pathList, current.f_cost);
                }

                openList.Remove(current);
                closedList.Add(current);
                /*
                if (current.neighbours == null)
                    continue;
                */

                foreach (int n in current.neighbours)
                {
                    s_pathNode no = nodes.Find(x => x.id == n);
                    float g_cost = current.g_cost + Vector2.Distance(no.position, current.position);
                    float h_cost = current.h_cost + Vector2.Distance(no.position, goal.position);

                    if (closedList.Contains(no))
                        continue;

                    if (!openList.Contains(no) || g_cost < no.g_cost)
                    {
                        no.g_cost = g_cost;
                        no.h_cost = h_cost;
                        no.parent = current;

                        if (!openList.Contains(no))
                            openList.Add(no);
                    }
                }

                round++;
            }
            return null;
        }
        public Tuple<List<s_pathNode>, float> PathFind(Vector2 start, s_pathNode end)
        {
            ResetNodes();
            int round = 0;
            List<s_pathNode> openList = new List<s_pathNode>();
            HashSet<s_pathNode> closedList = new HashSet<s_pathNode>();

            s_pathNode startNode = FindNodeAtPosition(start);
            s_pathNode goal = end;

            if (goal == null)
                return null;

            goal.isgoal = true;

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                if (round == 200)
                    return null;

                s_pathNode current = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    s_pathNode pa = openList[i];
                    if (pa.f_cost < current.f_cost)
                    {
                        //FindPathNode(pa.id)
                        current = FindNodeAtPosition(pa.position);
                    }
                }

                if (current.id == goal.id)
                {
                    List<s_pathNode> pathList = new List<s_pathNode>();
                    s_pathNode path = goal;
                    int l = 0;
                    while (path != startNode)
                    {
                        path.list = l;
                        pathList.Add(path);
                        l++;
                        path = path.parent;
                    }
                    pathList.Reverse();
                    return new Tuple<List<s_pathNode>, float>(pathList, current.f_cost);
                }

                openList.Remove(current);
                closedList.Add(current);
                /*
                if (current.neighbours == null)
                    continue;
                */

                foreach (int n in current.neighbours)
                {
                    s_pathNode no = nodes.Find(x => x.id == n);
                    float g_cost = current.g_cost + Vector2.Distance(no.position, current.position);
                    float h_cost = current.h_cost + Vector2.Distance(no.position, goal.position);

                    if (closedList.Contains(no))
                        continue;

                    if (!openList.Contains(no) || g_cost < no.g_cost)
                    {
                        no.g_cost = g_cost;
                        no.h_cost = h_cost;
                        no.parent = current;

                        if (!openList.Contains(no))
                            openList.Add(no);
                    }
                }

                round++;
            }
            return null;
        }

        public s_pathNode FindNodeAtPosition(Vector2 pos)
        {
            s_pathNode returnNode = null;
            float smallest = float.MaxValue;
            for (int i = 0; i < nodes.Count; i++)
            {
                s_pathNode p = nodes[i];
                Vector2 nodepos = new Vector2(p.position.x, p.position.y);
                float dist = Vector2.Distance(nodepos, pos);
                if (dist < smallest)
                {
                    returnNode = p;
                    smallest = dist;
                }
            }
            return returnNode;
        }
    }

    public class s_intro : MonoBehaviour
    {
        public AudioSource src;

        public void SwitchScene(int i)
        {
            SceneManager.LoadScene(i);
        }

        public void PlayJingleSound()
        {
            src.Play();
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
    public struct customEv
    {
        public delegate void cutsceneFunction();
        public cutsceneFunction func;
        public string name;
        public bool hasString0;
        public bool hasString1;
        public bool hasInt0;
        public bool hasInt1;
        public bool hasInt2;
    }

    public class s_mapBatch : MonoBehaviour {

        public s_mapEventholder[] mapBatch;

        public void FindAndEnableMap(string mapName) {
            foreach (s_mapEventholder mp in mapBatch) {
                if (mp.name == mapName) {
                    mp.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    public class s_mapEventholder : MonoBehaviour
    {
        public List<ev_details> Events;
        public Vector2 mapSize;
        public List<o_character> allChacacters;

        public void ReAddTargets() {

            if (allChacacters.Count == 0)
            {
                allChacacters = new List<o_character>();
                GameObject entit = GameObject.Find("Entities");
                if (entit != null)
                {
                    List<o_character> characterList = new List<o_character>();
                    characterList = entit.GetComponentsInChildren<o_character>().ToList();
                    foreach (o_character c in characterList)
                    {
                        if (c.faction == "")
                            continue;
                        allChacacters.Add(c);
                    }
                }
                else
                {
                    Debug.LogError("No entity object found!");
                }
            }
            o_character pl = s_mapManager.LevEd.player;
            allChacacters.Add(pl);
            pl.AddFactions(allChacacters);
            foreach (o_character c in allChacacters)
            {
                c.AddFactions(allChacacters);
            }
        }

        void Start() {

            if (GameObject.Find("Collision")) {
                Tilemap tmC = GameObject.Find("Collision").GetComponent<Tilemap>();
                if (tmC != null)
                    tmC.color = Color.clear;
            }

            if (allChacacters.Count == 0)
            {
                allChacacters = new List<o_character>();
                   GameObject entit = GameObject.Find("Entities");
                if (entit != null)
                {
                    List<o_character> characterList = new List<o_character>();
                    characterList = entit.GetComponentsInChildren<o_character>().ToList();
                    foreach (o_character c in characterList) {
                        if (c.faction == "")
                            continue;
                        allChacacters.Add(c);
                    }
                }
                else {
                    Debug.LogError("No entity object found!");
                }
            }
            o_character pl = s_mapManager.LevEd.player;
            allChacacters.Add(pl);
            pl.AddFactions(allChacacters);
            foreach (o_character c in allChacacters) {
                c.AddFactions(allChacacters);
            }
        }

        public T AddCharacter<T>(o_character c ,Vector3 pos) 
        {
            GameObject entit = GameObject.Find("Entities");
            o_character cha = Instantiate(c, pos, Quaternion.identity);
            cha.transform.SetParent(entit.transform);
            allChacacters.Add(cha);
            cha.AddFactions(allChacacters);
            return cha.GetComponent<T>();
        }
    }
    
    public class s_triggerhandler : MonoBehaviour
    {
        public List<s_dialogue_choice> dialogueChoices = new List<s_dialogue_choice>();
        public float travSpeed = 22.5f;
        public static s_triggerhandler trig;
        public bool isSkipping = false;
        public bool doingEvents = false;
        public Image fade;
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

        public bool newSystem;

        bool activated_shutters = false;
        public List<customEv> customEvAndFunction;

        public virtual void CreateData() {
            customEvAndFunction = new List<customEv>();
        }

        public void Awake()
        {
            DontDestroyOnLoad(this);
            leveled = GetComponent<s_levelloader>();
        }

        public void SetTrigObject()
        {
            trig = this;
        }
        
        /// <summary>
        /// Reserve event number -1 for label
        /// </summary>
        /// <param name="label"></param>
        public void JumpToEvent(int label, bool staicev)
        {
            if (!doingEvents) {
                pointer = label;
                doStatic = staicev;
                StartCoroutine(EventPlayMast());
            }
        }

        public void JumpToEvent(string label, bool staicev)
        {
            if (!doingEvents) {
                if (FindLabel(label) != int.MinValue)
                    pointer = FindLabel(label);
                doStatic = staicev;
                StartCoroutine(EventPlayMast());
            }
        }

        public void GetMapEvents()
        {
            Events = new List<ev_details>();
            Events = leveled.mapDat.Map_Script;
        }
        public void GetMapEvents(List<ev_label> map_script_labels, List<ev_details> Map_Script)
        {
            Events = new List<ev_details>();
            Events = Map_Script;
        }
        public IEnumerator EventPlayFromTrigger(List<ev_details> evHandler)
        {
            doingEvents = true;
            int evLeng = 0;

            evLeng = evHandler.Count;

            while ((pointer != -1 || pointer < evLeng) && doingEvents)
            {
                current_ev = evHandler[pointer];

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

        public IEnumerator EventPlayMast()
        {
            doingEvents = true;
            int evLeng = 0;

            if (doStatic)
                evLeng = StaticEvents.Count;
            else
                evLeng = Events.Count;

            while ((pointer != -1 || pointer < evLeng) && doingEvents)
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
            isSkipping = false;
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
                #region PLAY SOUND
                case 5:
                    s_soundmanager.sound.PlaySound(current_ev.string0);
                    break;
                #endregion

                #region DISPLAY NOTIFICATION TEXT
                case 23:

                    s_gui.DisplayNotificationText(current_ev.string0, current_ev.float0);
                    break;
#endregion

                #region ANIMATE CHARACTER
                case 4:

                    if (current_ev.string0 != "o_player") {
                        if (GameObject.Find(current_ev.string0) != null) {

                            if (GameObject.Find(current_ev.string0).GetComponent<o_character>() != null)
                            {
                                selobj = GameObject.Find(current_ev.string0).GetComponent<o_character>();
                            }
                            else if (GameObject.Find(current_ev.string0).GetComponent<o_generic>() != null) {
                                selobj = GameObject.Find(current_ev.string0).GetComponent<o_generic>();
                            }
                        }
                        
                    }
                    else
                        selobj = player;
                    selobj.SetAnimation(current_ev.string1, current_ev.boolean);
                    break;
#endregion

                #region MOVE CHARACTER
                case 0:

                    float timer = 1.02f;
                    s_object charaMove = null;
                    o_generic group = null;
                    Vector2 newpos = new Vector2(0,0);
                    GameObject gameObj = null;

                    if (current_ev.string0 != "o_player")
                        gameObj = GameObject.Find(current_ev.string0);
                    else
                    {
                        charaMove = player;
                        gameObj = charaMove.gameObject;
                    }
                    if (gameObj != null) {
                        if (current_ev.string0 != "o_player") {

                            group = gameObj.GetComponent<o_generic>();
                            if (group != null)
                            {
                                newpos = new Vector2(current_ev.float0, current_ev.float1);
                                group.transform.position = new Vector3((int)newpos.x, (int)newpos.y, 0);
                                break;
                            }
                        }

                        charaMove = gameObj.GetComponent<o_character>();

                        // newpos = charaMove.transform.position;
                        newpos = new Vector2(current_ev.float0, current_ev.float1);

                        if (current_ev.boolean1)
                        {
                            s_map.s_tileobj to = s_levelloader.LevEd.mapDat.tilesdata.Find(
                                x => x.TYPENAME == "teleport_object" &&
                                x.name == current_ev.string1);

                            newpos = new Vector2(to.pos_x, to.pos_y);
                        }

                        if (current_ev.boolean)
                        {
                            charaMove.transform.position = new Vector3((int)newpos.x, (int)newpos.y, 0);
                            break;
                        }

                        o_character c = charaMove.GetComponent<o_character>();


                        float dist = Vector2.Distance(charaMove.transform.position, newpos);
                        Vector2 dir = (newpos - new Vector2(charaMove.transform.position.x, charaMove.transform.position.y)).normalized;
                        //print(newpos);


                        while (Vector2.Distance(charaMove.transform.position, newpos)
                            > dist * 0.01f)
                        {
                            dir = (newpos - new Vector2(charaMove.transform.position.x, charaMove.transform.position.y)).normalized;
                            charaMove.transform.position += (Vector3)(dir * charaMove.terminalSpeedOrigin) * Time.deltaTime;
                            if (c != null)
                            {
                                //Set charaacter's anims whilst moving
                                c.direction = dir;
                                c.AnimMove();
                                c.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_MOVING;
                            }
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        c.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;
                    }
                    break;
                #endregion

                #region DIALOUGE
                case 1:
                    textBox.gameObject.SetActive(true);
                    while (Dialogue.text.Length < current_ev.string0.Length)
                    {
                        if (isSkipping) 
                            break;
                        Dialogue.text += current_ev.string0[textNum];
                        if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                        {
                            Dialogue.text = current_ev.string0;
                            continue;
                        }
                        textNum++;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    if (!isSkipping)
                    {
                        while (!Input.GetKeyDown(s_globals.GetKeyPref("select")))
                        {
                            if (isSkipping)
                                break;
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                    }
                    Dialogue.text = "";
                    textNum = 0;
                    if (pointer + 1 < Events.Count)
                    {
                        if(Events[pointer + 1].eventType != 1 && Events[pointer + 1].eventType != 30)
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
                        ch.dashdelay = 0;
                        ch.dashdelayStart = 0;
                        ch.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;
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
                                ch.dashdelay = 0;
                                ch.dashdelayStart = 0;
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
                    Time.timeScale = 1;
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

                    s_object obje = null;

                    //Focusing on character
                    if (current_ev.boolean)
                    {
                        if (GameObject.Find(current_ev.string0) != null && current_ev.string0 != "o_player")
                            obje = GameObject.Find(current_ev.string0).GetComponent<s_object>();

                        if (current_ev.string0 == "o_player")
                            obje = player;
                    }

                    Vector2 pos = new Vector2(0, 0);
                    if (obje != null)
                        pos = obje.transform.position;

                    if (obje != null)
                        ca.GetComponent<s_camera>().targetPos = obje.transform.position;
                    else
                        ca.GetComponent<s_camera>().targetPos = new Vector2(current_ev.posX, current_ev.posY);
                    if (current_ev.boolean2) {
                        ca.transform.position = obje.transform.position;
                        break;
                    }

                    if (current_ev.boolean)
                    {
                        float dista = Vector2.Distance(ca.transform.position, new Vector3(pos.x, pos.y));

                        while (Vector2.Distance(ca.transform.position, new Vector3(pos.x, pos.y))
                            > dista * 0.05f)
                        {
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        if (current_ev.boolean1)
                        {
                            s_camera.cam.SetPlayer(obje.GetComponent<o_character>());
                            ca.GetComponent<s_camera>().focus = true;
                        }
                    }
                    else
                    {

                        float dista = Vector2.Distance(ca.transform.position, new Vector3(current_ev.posX, current_ev.posY));
                        while (Vector2.Distance(ca.transform.position, new Vector3(current_ev.posX, current_ev.posY))
                            > dista * 0.05f)
                        {
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

                    switch (current_ev.string0) {
                        default:
                        case "black":
                        case "Black":
                            col = Color.black;
                            break;

                        case "magenta":
                        case "Magenta":
                            col = Color.magenta;
                            break;

                        case "pink":
                        case "Pink":
                            col = Color.magenta;
                            break;

                        case "blue":
                        case "Blue":
                            col = Color.blue;
                            break;

                        case "green":
                        case "Green":
                            col = Color.green;
                            break;

                        case "red":
                        case "Red":
                            col = Color.red;
                            break;

                        case "yellow":
                        case "Yellow":
                            col = Color.yellow;
                            break;

                    }
                    if (!current_ev.boolean)
                        col.a = 0;
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
                        player.AnimMove();
                        player.dashdelay = 0;
                    }
                    Time.timeScale = 0;

                    float t2 = 0;
                    while (fade.color != Color.black)
                    {
                        t2 += Time.unscaledDeltaTime;
                        fade.color = Color.Lerp(Color.clear, Color.black, t2);
                        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
                    }

                    pointer = -1;
                    doingEvents = false;
                    Events.Clear();


                    if (!newSystem)
                        s_levelloader.LevEd.TriggerSpawn(current_ev.string0, current_ev.string1, player);
                    else {
                        s_mapManager.LevEd.TriggerSpawn(current_ev.string0, current_ev.string1);
                    }

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

                    textBox.gameObject.SetActive(true);
                    int choice = 0, finalchoice = -1;
                    //print(choice);

                    while (finalchoice == -1)
                    {
                        if (Input.GetKeyDown(s_globals.GetKeyPref("up")))
                            choice--;

                        if (Input.GetKeyDown(s_globals.GetKeyPref("down")))
                            choice++;

                        choice = Mathf.Clamp(choice, 0, dialogueChoices.Count - 1);

                        if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                        {
                            //print("Chosen");
                            finalchoice = choice;
                        }
                        Dialogue.text = "Arrow keys to scroll, Z to select" + "\n";
                        //Dialogue.text += current_ev.string0 + "\n";
                        for (int i = 0; i < dialogueChoices.Count; i++)
                        {
                            if (choice == i)
                                Dialogue.text += "-> ";

                            Dialogue.text += dialogueChoices[i].option + "\n";
                        }
                        //print(choice);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    Dialogue.text = "";
                    pointer = dialogueChoices[finalchoice].flagTojump - 1;
                    if (pointer + 1 < Events.Count)
                    {
                        if (Events[pointer + 1].eventType != 1 && Events[pointer + 1].eventType != 30)
                            textBox.gameObject.SetActive(false);
                    }
                    break;
                #endregion

                #region ENABLE/DISABLE OBJECT
                case 26:
                    s_object go = GameObject.Find(current_ev.string0).GetComponent<s_object>();
                    Destroy(go.gameObject);
                    break;
                #endregion

                #region JUMP TO LABEL
                case 11:
                    labelNum = FindLabel(current_ev.string0);
                    pointer = labelNum;
                    break;
                #endregion

                #region SAVE DATA
                case 31:
                    s_globals.globalSingle.SaveData();
                    break;
                #endregion

                #region CUSTOM FUNCTION
                case 32:
                    customEv cuEv = customEvAndFunction.Find(f => f.name == current_ev.funcName);
                    cuEv.func.Invoke();
                    break;
                    #endregion

            }
            yield return new WaitForSeconds(Time.deltaTime);
            //yield return new WaitForSeconds(0.5f);
        }
        
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

        public void Update() {

            if (Input.GetKeyDown(KeyCode.X))
            {
                isSkipping = true;
                Time.timeScale = 2f;
            }

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

    /// <summary>
    /// This relies much more on unity's built-in scene manager instead of the custom stuff i made
    /// </summary>
    [RequireComponent(typeof(s_pathfind))]
    public class s_mapManager : MonoBehaviour
    {
        public o_character player;
        Dictionary<string, Queue<s_object>> objectPoolList = new Dictionary<string, Queue<s_object>>();
        public List<GameObject> objPoolDatabase = new List<GameObject>();
        public static s_mapManager LevEd;
        public List<o_character> allcharacters = new List<o_character>();
        public List<string> maps = new List<string>();

        public List<s_map.s_chara> savedcharalist = new List<s_map.s_chara>();
        public o_trigger mapscript;

        public GameObject objectPoolerObj;
        public GameObject SceneLevelObject;
        s_pathfind pathfind;

        public int id = 0;

        public int boxsize;
        public Vector3 graphsize;
        public Vector3 startpos;
        public LayerMask layerMask;
        public List<s_pathNode> nodes;

        s_nodegraph nodegraph;

        public int current_area;
        public Tilemap collisionTiles;

        public Tilemap tm;
        public Tilemap tm2;
        public Tilemap tm3;
        public Tilemap colmp;
        public Canvas canv;
        public Image fade;

        public string TPName;
        public Vector2 TPPos;

        public GameObject tilesObj;
        public GameObject triggersObj;
        public GameObject entitiesObj;
        public GameObject itemsObj;

        public s_triggerhandler triggerHand;
        public float tilesize;
        int loadingProgress = 0;
        public Text ldingPorgress;

        public s_mapEventholder mapEvHolder;
        public string firstScene;
        public string mapObject;

        public void Awake()
        {
            DontDestroyOnLoad(this);
            InitializeManager();
        }

        public void InitializeLoader()
        {
            //triggerHand = GetComponent<s_triggerhandler>();
            //triggerHand.SetTrigObject();
        }

        public o_trigger[] GetTriggerObjects()
        {
            return triggersObj.GetComponentsInChildren<o_trigger>();
        }
        public o_character[] SetCharacterObjects(Predicate<o_character> func)
        {
            return triggersObj.GetComponentsInChildren<o_character>();
        }

        public virtual void InitializePlayer()
        {
            o_character selobj = null;
            selobj = GameObject.Find("Player").GetComponent<o_character>();
            selobj.control = true;
        }

        public virtual void InitializeManager() {
            gameObject.SetActive(true);
        }

        public void InitializeGameWorld()
        {
            maps.Clear();

            SetList();

            if (s_mainmenu.isload)
            {
                print("Loaded game");
                dat_save sav = (dat_save)s_mainmenu.save;
                foreach (KeyValuePair<string, int> s in sav.gbflg.Flags)
                {
                    s_globals.SetGlobalFlag(s.Key, s.Value);
                }
                InitializePlayer();
                SceneManager.LoadScene(sav.currentmap);
                //LoadMap(maps.Find(x => x.name == s_mainmenu.save.currentmap));
            }
            else
            {
                InitializePlayer();
                SceneManager.LoadScene(firstScene);
            }
            if (GameObject.Find(mapObject) != null)
            {
                s_globals.globalSingle.isMainGame = true;
                   mapEvHolder = GameObject.Find(mapObject).GetComponent<s_mapEventholder>();

                if (triggerHand != null)
                    triggerHand.GetMapEvents(null, mapEvHolder.Events);

                ev_details[] events = triggerHand.Events.ToArray();

                for (int i = 0; i < events.Length; i++)
                {
                    ev_details det = events[i];
                    if (det.eventType == -1 && (
                        det.string0 == "onLoad" ||
                        det.string0 == "onload" ||
                        det.string0 == "Onload" ||
                        det.string0 == "OnLoad"))
                    {
                        triggerHand.JumpToEvent(i, false);
                        break;
                    }
                }
            }
            else
                print("Failed to find mapObject!");

            Screen.SetResolution(1280, 720, false);
        }

        public IEnumerator SetListCoroutine()
        {
            for (int i = 0; i < objPoolDatabase.Count; i++)
            {
                Queue<s_object> objque = new Queue<s_object>();
                GameObject obj = objPoolDatabase[i];
                s_object newobj = Instantiate(obj).GetComponent<s_object>();
                yield return new WaitForSeconds(Time.deltaTime);
                if (newobj == null)
                {
                    print("Fault at " + obj.name);
                    continue;
                }
                objque.Enqueue(newobj);
                newobj.gameObject.SetActive(false);
                newobj.transform.SetParent(objectPoolerObj.transform);
                objectPoolList.Add(obj.name, objque);
            }
        }
        public void SetList()
        {
            if (objectPoolList == null) {
                for (int i = 0; i < objPoolDatabase.Count; i++)
                {
                    Queue<s_object> objque = new Queue<s_object>();
                    GameObject obj = objPoolDatabase[i];
                    s_object newobj = Instantiate(obj).GetComponent<s_object>();
                    if (newobj == null)
                    {
                        print("Fault at " + obj.name);
                        continue;
                    }
                    newobj.transform.SetParent(objectPoolerObj.transform);
                    objque.Enqueue(newobj);
                    newobj.gameObject.SetActive(false);
                    objectPoolList.Add(obj.name, objque);
                }
            }
            
        }

        public void DespawnObject(s_object obj)
        {
            if (objectPoolList.ContainsKey(obj.ID))
                objectPoolList[obj.ID].Enqueue(obj);
            obj.transform.parent = null;
            obj.gameObject.SetActive(false);
            if (objectPoolerObj != null)
                obj.transform.SetParent(objectPoolerObj.transform);
            else
                print("No object pooler set");
        }

        public s_object[] GetTileObjects()
        {
            return tilesObj.GetComponentsInChildren<s_object>();
        }
        public void GetNodeObjects(ref s_map mp)
        {
            List<s_pathNode> nds = mp.nodes;
            o_nodeobj[] nods = tilesObj.GetComponentsInChildren<o_nodeobj>();
            nds = new List<s_pathNode>();
            foreach (o_nodeobj o in nods)
            {
                List<int> niegh = new List<int>();
                foreach (o_nodeobj on in o.nieghbours)
                {
                    niegh.Add(on.nodeID);
                }
                nds.Add(new s_pathNode(o.nodeID, o.transform.position, niegh));
            }
            mp.nodes = nds;
        }

        public void AddCharacter(o_character ch)
        {
            allcharacters.Add(ch);
        }


        public T SpawnObject<T>(string objstr, Vector3 pos, Quaternion quant) where T : s_object
        {
            T ob = null;
            if (!objectPoolList.ContainsKey(objstr))
            {
                Queue<s_object> objque = new Queue<s_object>();
                GameObject obj = objPoolDatabase.Find(x => x.name == objstr);
                s_object newobj = Instantiate(obj).GetComponent<s_object>();
                if (objectPoolerObj != null)
                    newobj.transform.SetParent(objectPoolerObj.transform);
                else
                    print("No object pooler set");
                objque.Enqueue(newobj);
                newobj.gameObject.SetActive(false);
                objectPoolList.Add(obj.name, objque);
            }
            GameObject pd = objPoolDatabase.Find(x => x.name == objstr);

            if (objectPoolList[objstr].Count < 1)
            {
                ob = Instantiate(pd, pos, quant).GetComponent<T>();
                ob.gameObject.SetActive(true);
                ob.ID = objstr;
                ob.transform.SetParent(objectPoolerObj.transform);
                return ob;
            }

            ob = objectPoolList[objstr].Dequeue().GetComponent<T>();
            ob.gameObject.transform.localRotation = quant;
            ob.transform.position = pos;
            ob.gameObject.SetActive(true);
            ob.transform.SetParent(objectPoolerObj.transform);
            ob.ID = objstr;
            return ob;
        }

        public void TriggerSpawnNew(string string0, string map, int teleporterNum, o_character chara)
        {
            SceneManager.LoadScene(string0);
            //triggerHand
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += SceneLoad;
        }

        IEnumerator SceneLoadingEnumerator() {

            s_globals.globalSingle.isMainGame = true;
            GameObject telp = GameObject.Find(TPName);
            if (telp != null)
                TPPos = new Vector3(telp.transform.position.x, telp.transform.position.y);
            if (telp != null)
                if (player != null)
                    player.transform.position = TPPos;
            GameObject mapThing = GameObject.Find(mapObject);

            if (mapThing != null)
            {
                mapEvHolder = mapThing.GetComponent<s_mapEventholder>();
                s_camera.cam.mapSize = mapEvHolder.mapSize;
                s_camera.cam.tileSize = new Vector2(tilesize, tilesize);
                s_camera.cam.transform.position = TPPos;
            }

            float t2 = 0;
            while (triggerHand.fade.color != Color.clear)
            {
                t2 += Time.deltaTime;
                triggerHand.fade.color = Color.Lerp(Color.black, Color.clear, t2);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            //doingEvents = false;
            if (mapThing != null)
            {

                triggerHand = GetComponent<s_triggerhandler>();
                if (triggerHand != null)
                    triggerHand.GetMapEvents(null, mapEvHolder.Events);
                ev_details[] events = mapEvHolder.Events.ToArray();

                for (int i = 0; i < events.Length; i++)
                {
                    ev_details det = events[i];
                    if (det.eventType == -1 && (
                        det.string0 == "onLoad" ||
                        det.string0 == "onload" ||
                        det.string0 == "Onload" ||
                        det.string0 == "OnLoad"))
                    {
                        triggerHand.JumpToEvent(i, false);
                        break;
                    }
                }
                if (player != null)
                    player.control = true;
            }
            else
                print("Failed to find " + mapObject + "!");
        }

        public void SceneLoad(Scene scene, LoadSceneMode sm)
        {
            StartCoroutine(SceneLoadingEnumerator());
        }
        public void TriggerSpawn(string string0, string teleporter)
        {
            Time.timeScale = 1;
            TPName = teleporter;
            SceneManager.LoadScene(string0);
        }
        public virtual void TriggerSpawn(string teleporter)
        {
            o_character selobj = null;
            TPName = teleporter;
            selobj = GameObject.Find("Player").GetComponent<o_character>();

            GameObject mapn = SceneLevelObject;
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

    }

    [RequireComponent(typeof(s_pathfind))]
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

        public GameObject prefab;
        public GameObject SceneLevelObject;
        s_pathfind pathfind;

        public int id = 0;

        public int boxsize;
        public Vector3 graphsize;
        public Vector3 startpos;
        public LayerMask layerMask;
        public List<s_pathNode> nodes;

        s_nodegraph nodegraph;
        public s_save_vector mapsizeToKeep = new s_save_vector(0, 0);
        
        public int current_area;
        protected bool InEditor = true;  //Dictate the loading style
        public bool LoadScene = false;  //Dictate the loading style
        public List<Tile> tilesNew = new List<Tile>();
        public List<Tile> collisionList = new List<Tile>();
        public Tile collisionTile;
        public Tilemap collisionTiles;

        public Tilemap tm;
        public Tilemap tm2;
        public Tilemap tm3;
        public Tilemap colmp;
        public Canvas canv;

        public string TPName;
        public Vector2 TPPos;

        public GameObject tilesObj;
        public GameObject triggersObj;
        public GameObject entitiesObj;
        public GameObject itemsObj;

        s_triggerhandler triggerHand;
        public float tilesize;
        int loadingProgress = 0;
        public Text ldingPorgress;

        public GameObject objectPoolerObj;
        public s_mapEventholder mapEvHolder;
        public string firstScene;
        public string mapObject;

        public void Awake()
        {
            DontDestroyOnLoad(this);
        }

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

        public virtual void InitializePlayer()
        {
            o_character selobj = null;
            selobj = GameObject.Find("Player").GetComponent<o_character>();
            selobj.control = true;
        }

        public void InitializeGameWorld()
        {
            maps.Clear();

            SetList();

            if (s_mainmenu.isload)
            {
                print("Loaded game");

                foreach (KeyValuePair<string, int> s in s_mainmenu.legacySave.gbflg.Flags)
                {
                    s_globals.SetGlobalFlag(s.Key, s.Value);
                }
                if (!LoadScene)
                {
                    LoadMaps();
                    InitializePlayer();
                    StartCoroutine(LoadMapWithDel(maps.Find(x => x.name == s_mainmenu.legacySave.currentmap)));
                } else {
                    InitializePlayer();
                    SceneManager.LoadScene(s_mainmenu.legacySave.currentmap);
                }
                //LoadMap(maps.Find(x => x.name == s_mainmenu.save.currentmap));
            }
            else
            {
                if (!LoadScene)
                {
                    LoadMaps();
                    InitializePlayer();
                    StartCoroutine(LoadMapWithDel(maps[0]));
                }
                else
                {
                    InitializePlayer();
                    SceneManager.LoadScene(firstScene);
                }
                //LoadMap(maps[0]);
            }
            if (GameObject.Find(mapObject) != null)
            {
                mapEvHolder = GameObject.Find(mapObject).GetComponent<s_mapEventholder>();

                if (triggerHand != null)
                    triggerHand.GetMapEvents(null, mapEvHolder.Events);

                ev_details[] events = triggerHand.Events.ToArray();

                for (int i = 0; i < events.Length; i++)
                {
                    ev_details det = events[i];
                    if (det.eventType == -1 && (
                        det.string0 == "onLoad" ||
                        det.string0 == "onload" ||
                        det.string0 == "Onload" ||
                        det.string0 == "OnLoad"))
                    {
                        triggerHand.JumpToEvent(i, false);
                        break;
                    }
                }
            } else
                print("Failed to find mapObject!");

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

        public IEnumerator SetListCoroutine()
        {
            for (int i = 0; i < objPoolDatabase.Count; i++)
            {
                Queue<s_object> objque = new Queue<s_object>();
                GameObject obj = objPoolDatabase[i].gameobject;
                s_object newobj = Instantiate(obj).GetComponent<s_object>();
                yield return new WaitForSeconds(Time.deltaTime);
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
                if (objectPoolerObj != null)
                    newobj.transform.SetParent(objectPoolerObj.transform);
                else
                    print("No object pooler set");
                objque.Enqueue(newobj);
                newobj.gameObject.SetActive(false);
                objectPoolList.Add(obj.name, objque);
            }
        }

        public void DespawnObject(s_object obj)
        {
            if(!objectPoolList.ContainsKey(obj.ID))
                objectPoolList[obj.ID].Enqueue(obj);
            obj.transform.parent = null;
            obj.gameObject.SetActive(false);
            if (objectPoolerObj != null)
                obj.transform.SetParent(objectPoolerObj.transform);
            else
                print("No object pooler set");
        }

        public s_object[] GetTileObjects()
        {
            return tilesObj.GetComponentsInChildren<s_object>();
        }
        public void GetNodeObjects(ref s_map mp)
        {
            List<s_pathNode> nds = mp.nodes;
            o_nodeobj[] nods = tilesObj.GetComponentsInChildren<o_nodeobj>();
            nds = new List<s_pathNode>(); 
            foreach (o_nodeobj o in nods) {
                List<int> niegh = new List<int>();
                foreach (o_nodeobj on in o.nieghbours) {
                    niegh.Add(on.nodeID);
                }
                nds.Add(new s_pathNode(o.nodeID, o.transform.position, niegh));
            }
            mp.nodes = nds;
        }

        public void AddCharacter(o_character ch)
        {
            allcharacters.Add(ch);
        }

        public T SpawnObject<T>(string obj) where T : s_object
        {
            T ob = null;
            if (objectPoolList[obj].Count < 1)
            {
                ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj).gameobject).GetComponent<T>();
                ob.gameObject.SetActive(true);
                ob.ID = obj;
                ob.GetComponent<IPoolerObj>().SpawnStart();
                return ob;
            }

            ob = objectPoolList[obj].Dequeue().GetComponent<T>();
            ob.gameObject.SetActive(true);
            ob.ID = obj;
            ob.GetComponent<IPoolerObj>().SpawnStart();
            return ob;
        }
        public T SpawnObject<T>(string obj, Transform transformp) where T : s_object
        {
            T ob = null;
            if (objectPoolList[obj].Count == 0)
            {
                ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj).gameobject, transformp).GetComponent<T>();
                ob.transform.position = transform.position;
                ob.gameObject.SetActive(true);
                ob.ID = obj;
                ob.GetComponent<IPoolerObj>().SpawnStart();
                return ob;
            }

            ob = objectPoolList[obj].Dequeue().GetComponent<T>();
            ob.gameObject.SetActive(true);
            ob.ID = obj;
            ob.GetComponent<IPoolerObj>().SpawnStart();
            return ob;
        }
        public T SpawnObject<T>(string obj, Vector3 pos, Quaternion quant) where T : s_object
        {
            T ob = null;
            s_pooler_data pd = objPoolDatabase.Find(x => x.gameobject.name == obj);
            if (!pd.single)
            {
                if (objectPoolList[obj].Count == 1)
                {
                    ob = Instantiate(pd.gameobject, pos, quant).GetComponent<T>();
                    ob.gameObject.SetActive(true);
                    ob.ID = obj;
                    ob.GetComponent<IPoolerObj>().SpawnStart();
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
            ob.GetComponent<IPoolerObj>().SpawnStart();
            return ob;
        }
        public T SpawnObject<T>(GameObject obj, Vector3 pos, Quaternion quant) where T : s_object
        {
            T ob = null;
            if (objectPoolList[obj.name].Count == 1)
            {
                ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj.name).gameobject, pos, quant).GetComponent<T>();
                ob.gameObject.SetActive(true);
                ob.ID = obj.name;
                ob.GetComponent<IPoolerObj>().SpawnStart();
                return ob;
            }

            ob = objectPoolList[obj.name].Dequeue().GetComponent<T>();
            ob.gameObject.transform.localRotation = quant;
            ob.transform.position = pos;
            ob.gameObject.SetActive(true);
            ob.ID = obj.name;
            ob.GetComponent<IPoolerObj>().SpawnStart();
            return ob;
        }


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
        public void TriggerSpawn(string string0, string teleporter, o_character chara)
        {
            if (LoadScene)
            {
                SceneManager.LoadScene(string0);
                TPName = teleporter;
            } else {
                NewMap();
                s_map thing = maps.Find(x => x.name == string0);
                LoadMap(thing);
                CheckCharacters();
                s_map.s_tileobj to = thing.tilesdata.Find(x => x.TYPENAME == "teleport_object" && x.name == teleporter);
                chara.transform.position = new Vector3(to.pos_x, to.pos_y);
            }
        }
        public void TriggerSpawnNew(string string0, string map, int teleporterNum, o_character chara)
        {
            if (LoadScene)
            {
                SceneManager.LoadScene(string0);
                //TPName = teleporter;
            }
        }

        void OnEnable() {
            SceneManager.sceneLoaded += SceneLoad;
        }

        public void SceneLoad(Scene scene, LoadSceneMode sm)
        {
            GameObject telp = GameObject.Find(TPName);
            if (telp != null)
                TPPos = new Vector3(telp.transform.position.x, telp.transform.position.y);
            player.transform.position = TPPos;
            if (GameObject.Find(mapObject) != null)
            {
                mapEvHolder = GameObject.Find(mapObject).GetComponent<s_mapEventholder>();
                triggerHand = GetComponent<s_triggerhandler>();
                if (triggerHand != null)
                    triggerHand.GetMapEvents(null, mapEvHolder.Events);
                ev_details[] events = triggerHand.Events.ToArray();

                for (int i = 0; i < events.Length; i++)
                {
                    ev_details det = events[i];
                    if (det.eventType == -1 && (
                        det.string0 == "onLoad" ||
                        det.string0 == "onload" ||
                        det.string0 == "Onload" ||
                        det.string0 == "OnLoad"))
                    {
                        triggerHand.JumpToEvent(i, false);
                        break;
                    }
                }
            }
            else
                print("Failed to find mapObject!");
        }

        public void TriggerSpawn(string string0, string teleporter)
        {
            if (LoadScene)
            {
                TPName = teleporter;
                SceneManager.LoadScene(string0);
            } else {
                NewMap();
                s_map thing = maps.Find(x => x.name == string0);
                LoadMap(thing);
                CheckCharacters();
                s_map.s_tileobj to = thing.tilesdata.Find(x => x.TYPENAME == "teleport_object" && x.name == teleporter);
                player.transform.position = new Vector3(to.pos_x, to.pos_y);
            }
        }
        public virtual void TriggerSpawn(string teleporter)
        {
            o_character selobj = null;
            TPName = teleporter;
            selobj = GameObject.Find("Player").GetComponent<o_character>();

            GameObject mapn = SceneLevelObject;
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

            if (triggerHand != null)
                triggerHand.Events = new List<ev_details>();

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
        public void SaveMap(string dir)
        {
            string mapdat = JsonUtility.ToJson(GetMap());
            //print(mapdat);

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
            //print(mapfil.tilesdata.Count);

            //print(mapfil.tilesdata);
            GetNodeObjects(ref mapfil);

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
            List<s_map.s_trig> Tr = new List<s_map.s_trig>();
            foreach (s_object obj in triggers)
            {
                s_map.s_trig tri = null;
                o_trigger trigga = obj.GetComponent<o_trigger>();
                if (obj.GetComponent<BoxCollider2D>())
                    tri = new s_map.s_trig(obj.name, obj.transform.position, 0, obj.GetComponent<BoxCollider2D>().size, false);
                else
                    tri = new s_map.s_trig(obj.name, obj.transform.position, 0, new Vector2(0, 0), false);

                tri.util = obj.ID; 
                tri.labelToJumpTo = trigga.LabelToJumpTo;
                tri.stringLabelToJumpTo = trigga.stringLabelToJumpTo;
                tri.trigtye = trigga.TRIGGER_T;
                Tr.Add(tri);
            }
            return Tr;
        }

        public virtual GameObject SetTrigger(s_map.s_trig trigger)
        {
            GameObject trigObj = null;
            Vector2 pos = new Vector2(trigger.pos_x, trigger.pos_y);

            if (InEditor)
                trigObj = Instantiate(FindOBJ(trigger.util), pos, Quaternion.identity);
            else
                trigObj = SpawnObject<s_object>(trigger.util, pos, Quaternion.identity).gameObject;
            switch (trigger.util)
            {
                default:
                    break;
            }
            trigObj.transform.SetParent(triggersObj.transform);
            o_trigger trig = trigObj.GetComponent<o_trigger>();
            s_utility util = null;
            if (trig == null)
                util = trigObj.GetComponent<s_utility>();

            //trig.TRIGGER_T = mapdat.triggerdata[i].trigtye;
            if (trigger.util != null)
            {
                string n = trigger.util;
            }
            if (trig != null)
            {
                trig.ev_num = 0;    //TODO: IF THE TRIGGER DOES NOT STATICALLY STORE ITS EVENT NUMBER, SET IT TO 0

                trig.name = trigger.name;
                trig.isactive = false;
                trig.LabelToJumpTo = trigger.labelToJumpTo;
                trig.stringLabelToJumpTo = trigger.stringLabelToJumpTo;
                trig.TRIGGER_T = trigger.trigtye;
                //trig.TRIGGER_T = mapdat.triggerdata[i].trigtye;

                s_save_vector ve = trigger.trigSize;
                trig.GetComponent<BoxCollider2D>().size = new Vector2(ve.x, ve.y);

                trig.transform.SetParent(trig.transform);
            }
            /*
            else if (util != null)
            {
                if (trigger.name != "")
                    util.name = trigger.name;

                s_save_vector ve = trigger.trigSize;

                if (util.GetComponent<BoxCollider2D>())
                    util.GetComponent<BoxCollider2D>().size = new Vector2(ve.x, ve.y);


                util.transform.SetParent(util.transform);
                return util.gameObject;
            }
            */
            return trigObj;
        }
        public virtual void SetItem(s_save_item item)
        {
            o_itemObj itemObj = null;
            if (InEditor)
                itemObj = Instantiate(FindOBJ<o_itemObj>(item.stID), new Vector3(item.pos.x, item.pos.y), Quaternion.identity);
            else
                itemObj = SpawnObject<o_itemObj>(item.stID, new Vector3(item.pos.x, item.pos.y), Quaternion.identity);
            if (itemObj != null)
            {
                itemObj.amount = 1;
                itemObj.transform.SetParent(itemsObj.transform);
            }
        }

        public IEnumerator LoadMapWithDel(s_map mapdat) {
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
                yield return null;
            nodegraph = GetComponent<s_nodegraph>();

            triggerHand = GetComponent<s_triggerhandler>();

            mapsizeToKeep = mapdat.mapsize;

            nodegraph.size = new Vector2Int((int)mapsizeToKeep.x, (int)mapsizeToKeep.y);

            #region SPAWN_ENTITIES
            for (int i = 0; i < mapdat.objectdata.Count; i++)
            {
                s_map.s_chara characterdat = mapdat.objectdata[i];
                SetEntity<o_character>(characterdat);
                yield return new WaitForSeconds(Time.deltaTime);
                loadingProgress++;
                if (ldingPorgress)
                    ldingPorgress.text = "progress: " + loadingProgress;
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
                //print(t.util);
                trigObj = SetTrigger(t);
                yield return new WaitForSeconds(Time.deltaTime);

                loadingProgress++;
                if (ldingPorgress)
                    ldingPorgress.text = "progress: " + loadingProgress;

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

            SetTileMap(mapdat);

            #region SET_NODES
            nodes = mapdat.nodes;
            List<o_nodeobj> debugNodes = new List<o_nodeobj>();
            if (InEditor)
            {
                foreach (s_pathNode pn in mapdat.nodes)
                {
                    ///Spawn the node objects if the editor is in editor mode
                    ///also connect them to each other
                    o_nodeobj nod = Instantiate<o_nodeobj>(objPoolDatabase.Find(x => x.gameobject.GetComponent<o_nodeobj>()).gameobject.GetComponent<o_nodeobj>(), pn.position, Quaternion.identity);
                    nod.nodeID = pn.id;
                    debugNodes.Add(nod);
                    nod.transform.SetParent(tilesObj.transform);
                }
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                s_pathNode p = nodes[i];
                List<int> GetNeighbours = new List<int>();
                /*
                //Gets all the neighbours
                foreach (int ind in p.neighbours)
                {
                    //Find all the custom values with "node" which resemble the neighbours
                    s_pathNode path = nodes.Find(x => x.id == ind);
                    nbrs.Add(path);
                }
                */
                if (InEditor)
                {
                    o_nodeobj currentNode = debugNodes.Find(x => x.nodeID == p.id);
                    //Add neighbours


                    List<int> nod = p.neighbours;

                    foreach (int n in nod)
                    {
                        o_nodeobj nodeOBJ = debugNodes.Find(x => x.nodeID == n);
                        if (nodeOBJ != null)
                        {
                            currentNode.nieghbours.Add(nodeOBJ);
                            GetNeighbours.Add(n);
                        }
                    }
                    /*
                    foreach (s_pathNode n in nbrs)
                    {
                        GetNeighbours.Add(n.id);
                    }
                    p = new s_pathNode(p.id, new Vector2(p.position.x, p.position.y), GetNeighbours);
                    nodes[i] = p;
                    */
                }
                /*
                else
                {
                    foreach (s_pathNode n in nbrs)
                    {
                        GetNeighbours.Add(n.id);
                    }
                    p = new s_pathNode(p.id, new Vector2(p.position.x, p.position.y), GetNeighbours);
                    nodes[i] = p;
                }
                */
            }
            pathfind = GetComponent<s_pathfind>();
            pathfind.SetNodes(ref nodes);
            #endregion

            #region SPAWN_ITEMS
            for (int i = 0; i < mapdat.itemdat.Count; i++)
            {
                s_save_item item = mapdat.itemdat[i];
                SetItem(item);
                yield return new WaitForSeconds(Time.deltaTime);

                loadingProgress++;
                if (ldingPorgress)
                    ldingPorgress.text = "progress: " + loadingProgress;

            }
            #endregion

            foreach (o_character c in allcharacters)
            {
                if (c != null)
                    c.AddFactions(allcharacters);
            }

            #region ADD IN OBJECT DEPENDENCIES
            SetTriggerDependency(ref depndencyReq);
            SetTriggerDependencyAlt(ref mapdat.triggerdata);
            #endregion

            if (nodegraph != null)
                nodegraph.CreateNodeArray(mapdat.tilesdata.ToArray());
            else
                print("Please include the nodegraph.");

            #region CALL ONLOAD
            ev_details[] events = triggerHand.Events.ToArray();

            for (int i = 0; i < events.Length; i++)
            {
                ev_details det = events[i];
                if (det.eventType == -1 && (
                    det.string0 == "onLoad" ||
                    det.string0 == "onload" ||
                    det.string0 == "Onload" ||
                    det.string0 == "OnLoad"))
                {
                    triggerHand.JumpToEvent(i, false);
                    break;
                }
            }
            #endregion
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
                //print(t.util);
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

            if (!LoadScene)  {
                if (triggerHand != null)
                    triggerHand.GetMapEvents(mapdat.map_script_labels, mapdat.Map_Script);
            } else {
                mapEvHolder.Events = mapdat.Map_Script;
                mapEvHolder.mapSize = new Vector2(mapdat.mapsize.x, mapdat.mapsize.y);
            }

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

            SetTileMap(mapdat);
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


            #region SET_NODES
            nodes = mapdat.nodes;
            List<o_nodeobj> debugNodes = new List<o_nodeobj>();
            if (InEditor)
            {
                foreach (s_pathNode pn in mapdat.nodes)
                {
                    ///Spawn the node objects if the editor is in editor mode
                    ///also connect them to each other
                    o_nodeobj nod = Instantiate<o_nodeobj>(objPoolDatabase.Find(x => x.gameobject.GetComponent<o_nodeobj>()).gameobject.GetComponent<o_nodeobj>(), pn.position, Quaternion.identity);
                    nod.nodeID = pn.id;
                    debugNodes.Add(nod);
                    nod.transform.SetParent(tilesObj.transform);
                }
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                s_pathNode p = nodes[i];
                List<int> GetNeighbours = new List<int>();
                /*
                //Gets all the neighbours
                foreach (int ind in p.neighbours)
                {
                    //Find all the custom values with "node" which resemble the neighbours
                    s_pathNode path = nodes.Find(x => x.id == ind);
                    nbrs.Add(path);
                }
                */
                if (InEditor)
                {
                    o_nodeobj currentNode = debugNodes.Find(x => x.nodeID == p.id);
                    //Add neighbours


                    List<int> nod = p.neighbours;

                    foreach (int n in nod)
                    {
                        o_nodeobj nodeOBJ = debugNodes.Find(x => x.nodeID == n);
                        if (nodeOBJ != null)
                        {
                            currentNode.nieghbours.Add(nodeOBJ);
                            GetNeighbours.Add(n);
                        }
                    }
                    /*
                    foreach (s_pathNode n in nbrs)
                    {
                        GetNeighbours.Add(n.id);
                    }
                    p = new s_pathNode(p.id, new Vector2(p.position.x, p.position.y), GetNeighbours);
                    nodes[i] = p;
                    */
                }
                /*
                else
                {
                    foreach (s_pathNode n in nbrs)
                    {
                        GetNeighbours.Add(n.id);
                    }
                    p = new s_pathNode(p.id, new Vector2(p.position.x, p.position.y), GetNeighbours);
                    nodes[i] = p;
                }
                */
            }
            pathfind = GetComponent<s_pathfind>();
            pathfind.SetNodes(ref nodes);
            #endregion

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
            SetTriggerDependencyAlt(ref mapdat.triggerdata);
            #endregion

            if (nodegraph != null)
                nodegraph.CreateNodeArray(mapdat.tilesdata.ToArray());
            else
                print("Please include the nodegraph.");

            #region CALL ONLOAD
            if (!InEditor) {
                ev_details[] events = triggerHand.Events.ToArray();
                for (int i = 0; i < events.Length; i++) {
                    ev_details det = events[i];
                    if (det.eventType == -1 && (
                        det.string0 == "onLoad" ||
                        det.string0 == "onload" ||
                        det.string0 == "Onload" ||
                        det.string0 == "OnLoad"))
                    {
                        triggerHand.JumpToEvent(i, false);
                        break;
                    }
                }
            }
            #endregion
        }

        public virtual void SetTriggerDependency(ref List<Tuple<GameObject, List<s_map.s_customType>>> triggers)
        {
        }
        public virtual void SetTriggerDependencyAlt(ref List<s_map.s_trig> triggers)
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
                        //print("Couldn't find object '" + characterdat.charType + "' in the pool, please add it to the pooler.");
                        continue;
                    }
                    if (FindOBJ<o_character>(characterdat.charType))
                        trig = Instantiate<o_character>(FindOBJ<o_character>(characterdat.charType), characterPos, Quaternion.identity);
                    else
                        continue;
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
                    //print("Couldn't find object '" + characterdat.charType + "' in the pool, please add it to the pooler.");
                    return null;
                }
#if !UNITY_STANDALONE
                if (FindOBJ<o_character>(characterdat.charType))
                    trig = Instantiate<T>(FindOBJ<T>(characterdat.charType), characterPos, Quaternion.identity);
                else
                    return null;
#else
                /*
                if (FindOBJ<o_character>(characterdat.charType))
                    trig = PrefabUtility.InstantiatePrefab(FindOBJ<T>(characterdat.charType)) as T;
                else
                    return null;
                */
#endif
                /*
#if UNITY_EDITOR_WIN
                if (FindOBJ<o_character>(characterdat.charType))
                    trig = PrefabUtility.InstantiatePrefab(FindOBJ<T>(characterdat.charType)) as T;
                else
                    return null;
#elif UNITY_STANDALONE
                if (FindOBJ<o_character>(characterdat.charType))
                    trig = Instantiate<T>(FindOBJ<T>(characterdat.charType), characterPos, Quaternion.identity);
                else
                    return null;
#endif
                */
            }
            else
            {
                //print(characters[i].charType);
                if (characterdat.charType == "")
                    return null;
                trig = SpawnObject<T>(characterdat.charType, characterPos, Quaternion.identity);
            }
            trig.transform.position = characterPos;
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
        public List<AudioClip> clips;

        void Start()
        {
            src = GetComponent<AudioSource>();
            sound = GetComponent<s_soundmanager>();
        }

        public void PlaySound(string sound)
        {
            if (sound == null)
                return;
            src.PlayOneShot(clips.Find(x => x.name== sound));
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
