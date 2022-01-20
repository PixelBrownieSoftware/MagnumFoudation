using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnumFoudation
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct StringList
    {
        public StringList(string name, List<string> listOfStrings)
        {
            this.name = name;
            this.listOfStrings = listOfStrings;
        }
        public string name;
        public List<string> listOfStrings;
    }
    [System.Serializable]
    public struct IntList
    {
        public IntList(string name, List<int> listOfInts) 
        {
            this.name = name;
            this.listOfInts = listOfInts;
        }
        public string name;
        public List<int> listOfInts;
    }
    [System.Serializable]
    public struct FloatList
    {
        public FloatList(string name, List<float> listOfFloat)
        {
            this.name = name;
            this.listOfFloat = listOfFloat;
        }
        public string name;
        public List<float> listOfFloat;
    }
    public enum LOGIC_TYPE
    {
        VAR_GREATER,
        VAR_EQUAL,
        VAR_LESS,
        ITEM_OWNED,
        CHECK_UTILITY_RETURN_NUM,
        VAR_NOT_EQUAL
    }
    public enum EVENT_TYPES
    {
        LABEL = -1,
        MOVEMNET,
        DIALOGUE,
        SET_HEALTH = 2,
        RUN_CHARACTER_SCRIPT,
        ANIMATION = 4,
        SOUND,
        CUSTOM_FUNCTION,
        SET_FLAG,
        CHECK_FLAG,
        CAMERA_MOVEMENT,
        BREAK_EVENT,
        JUMP_TO_LABEL,
        SET_UTILITY_FLAG,
        FADE,
        CREATE_OBJECT,
        DISPLAY_CHARACTER_HEALTH,
        UTILITY_INITIALIZE,
        UTILITY_CHECK,
        WAIT,
        CHOICE,
        CHANGE_SCENE,
        PUT_SHUTTERS,
        DISPLAY_IMAGE,
        SHOW_TEXT,
        CHANGE_MAP,
        DEPOSSES,
        DELETE_OBJECT,
        SET_OBJ_COLLISION,
        ADD_CHOICE_OPTION,
        CLEAR_CHOICES,
        PRESENT_CHOICES,
        SAVE_DATA
    }
    [System.Serializable]
    public class ev_details
    {
        [System.Serializable]
        public struct s_save_colour
        {
            public float r, g, b, a;
            public s_save_colour(Color colour)
            {
                r = colour.r;
                g = colour.g;
                b = colour.b;
                a = colour.a;
            }
        }
        public enum LOGIC_TYPE
        {
            VAR_GREATER,
            VAR_EQUAL,
            VAR_LESS,
            VAR_NOT_EQUAL,
            ITEM_OWNED,
            CHECK_UTILITY_RETURN_NUM
        }

        /*
        public enum DIRECTION
        {
            NONE,
            LEFT,
            UP_LEFT,
            UP_RIGHT,
            DOWN_LEFT,
            DOWN_RIGHT,
            DOWN,
            UP,
            RIGHT
        }
        public DIRECTION dir;
        public int steps;


        public bool simultaneous;
        
        public Vector2 direcion
        {
            get
            {
                switch (dir)
                {
                    default:
                        return new Vector2(0, 0);

                    case DIRECTION.DOWN:
                        return new Vector2(0, -1);

                    case DIRECTION.DOWN_LEFT:
                        return new Vector2(-1, -1);

                    case DIRECTION.DOWN_RIGHT:
                        return new Vector2(1, -1);

                    case DIRECTION.RIGHT:
                        return new Vector2(1, 0);

                    case DIRECTION.UP:
                        return new Vector2(0, 1);

                    case DIRECTION.UP_LEFT:
                        return new Vector2(-1, 1);

                    case DIRECTION.UP_RIGHT:
                        return new Vector2(1, 1);

                    case DIRECTION.LEFT:
                        return new Vector2(-1, 0);
                }
            }
        }
        */

        public int logic;
        public int eventType;
        public string funcName;

        public bool waitTillDone;
        public int jump;
        public s_save_vector pos;
        public s_save_colour colour;
        public int int0;
        public int int1;
        public int int2;
        public bool boolean;
        public bool boolean1;
        public bool boolean2;
        public string string0;
        public string string1;
        public float posX;
        public float posY;  //So the editor can read it
        public float float0;
        public float float1;
        public string[] stringList;
        public int[] intList;
        public ScriptableObject scrObj;
    }
    
    [System.Serializable]
    public struct s_save_vector
    {
        public float x, y,z;
        public s_save_vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0;
        }
        public s_save_vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public s_save_vector(Vector2 pos)
        {
            x = pos.x;
            y = pos.y;
            z = 0;
        }
        public s_save_vector(Vector3 pos)
        {
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        public static s_save_vector operator +(s_save_vector vec, Vector3 vec2)
        {
            vec.x = vec2.x;
            vec.y = vec2.y;
            vec.z = vec2.z;
            return vec;
        }
    }

    [System.Serializable]
    public struct ev_label
    {
        public ev_label(string _name, int _index)
        {
            name = _name;
            index = _index;
        }
        public string name;
        public int index;
    }

    [System.Serializable]
    public class s_save_item
    {
        public s_save_item(Vector2 vec, int id,
            //o_item it, 
            ITEM_TYPE type, string name, string IDid)
        {
            pos = new s_save_vector(vec.x, vec.y);
            ID = id;
            //it = item;
            this.type = type;
            this.name = name;
            stID = IDid;
        }
        public string name;
        public ITEM_TYPE type;
        public bool iscollected = false;
        public s_save_vector pos;
        public int ID;
        public string stID;
        // public o_item item;
    }

    [System.Serializable]
    public partial class s_map
    {
        public int gemCount = 0;
        public List<string> characterFileNames = new List<string>();
        public List<s_trig> triggerdata = new List<s_trig>();
        public List<s_block> graphicTiles = new List<s_block>();
        public List<s_block> graphicTilesMiddle = new List<s_block>();
        public List<s_block> graphicTilesTop = new List<s_block>();
        public List<s_chara> objectdata = new List<s_chara>();
        public List<s_tileobj> tilesdata = new List<s_tileobj>();
        public List<s_save_item> itemdat = new List<s_save_item>();
        public List<s_pathNode> nodes = new List<s_pathNode>();
        public s_save_vector spawnPoint;
        public s_save_vector mapsize;
        public string FlagNameCheck;
        public string mapscript;

        public List<ev_details> Map_Script = new List<ev_details>();
        public List<ev_label> map_script_labels = new List<ev_label>();
        public bool OnMapScriptCall = false;
        public int onMapScriptCallLabel = 0;

        public int id;
        public string name;
        public string zone;

        public s_map() { }

        public s_map(string name, string script)
        {
            this.name = name;
            mapscript = script;
        }


        [System.Serializable]
        public class s_block
        {
            public s_block(string sprite, Vector2 position)
            {
                this.position = new s_save_vector(position);
                this.sprite = sprite;
            }
            public string sprite;
            public s_save_vector position;
            public s_save_vector rotation;
        }
        
        [System.Serializable]
        public class s_trig
        {
            [System.Serializable]
            public struct bound
            {
                public s_save_vector pos;
                public bound(Vector2 vec)
                {
                    pos = new s_save_vector(vec.x, vec.y);
                }
            }

            public s_trig(string name, Vector2 pos, string bound)
            {
                trigSize = new s_save_vector(20, 20);
                this.name = name;
                util = bound;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                characternames = null;
                boundaryobj = null;
            }
            public s_trig(string name, Vector2 pos)
            {
                trigSize = new s_save_vector(20, 20);
                this.name = name;
                util = null;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                characternames = null;
                boundaryobj = null;
            }
            public s_trig(string name, Vector2 pos, o_trigger.TRIGGER_TYPE trig, Vector2 size, bool disable)
            {
                trigSize = new s_save_vector(size.x, size.y);
                this.name = name;
                util = null;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                characternames = null;
                boundaryobj = null;
                trigtye = trig;
                IsPermanentlyDisabled = disable;
            }
            public s_trig(string name, Vector2 pos, ev_details[] ev, o_trigger.TRIGGER_TYPE trig, Vector2 size, bool disable)
            {
                trigSize = new s_save_vector(size.x, size.y);
                this.name = name;
                util = null;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                listofevents = ev;
                characternames = null;
                boundaryobj = null;
                trigtye = trig;
                IsPermanentlyDisabled = disable;
            }
            public s_trig(string name, Vector2 pos, ev_details[] ev, string util, o_trigger.TRIGGER_TYPE trig, bool disable)
            {
                this.name = name;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                listofevents = ev;
                this.util = util;
                characternames = null;
                boundaryobj = null;
                trigtye = trig;
                IsPermanentlyDisabled = disable;
            }

            public s_trig(string name, Vector2 pos, ev_details[] ev, string util, GameObject[] bound, int gems, o_trigger.TRIGGER_TYPE trig, Vector2 trigSize, bool disable)
            {
                gemreq = gems;
                this.name = name;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                listofevents = ev;
                this.trigSize = new s_save_vector(trigSize.x, trigSize.y);
                this.util = util;
                boundaryobj = new bound[bound.Length];
                for (int i = 0; i < bound.Length; i++)
                {
                    boundaryobj[i] = new bound(bound[i].transform.position);
                }
                trigtye = trig;
                IsPermanentlyDisabled = disable;
            }
            public s_trig(string name, Vector2 pos, ev_details[] ev, string util, string[] charnames, int[] intlist, o_trigger.TRIGGER_TYPE trig, Vector2 trigSize, bool disable)
            {
                this.name = name;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                listofevents = ev;
                this.intlist = intlist;
                this.trigSize = new s_save_vector(trigSize.x, trigSize.y);
                this.util = util;
                characternames = charnames;
                trigtye = trig;
                IsPermanentlyDisabled = disable;
            }
            public void AddCustomTag(string tagname, string tagValue)
            {
                if (CustomTypes != null)
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                else
                {
                    CustomTypes = new List<s_customType>();
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                }
            }
            public void AddCustomTag(string tagname, Vector2Int tagval)
            {
                if (CustomTypes != null)
                    CustomTypes.Add(new s_customType(tagname, tagval.x, tagval.y));
                else
                {
                    CustomTypes = new List<s_customType>();
                    CustomTypes.Add(new s_customType(tagname, tagval.x, tagval.y));
                }
            }
            public void AddCustomTag(string tagname, int tagValue, int tagValue2)
            {
                if (CustomTypes != null)
                    CustomTypes.Add(new s_customType(tagname, tagValue, tagValue2));
                else
                {
                    CustomTypes = new List<s_customType>();
                    CustomTypes.Add(new s_customType(tagname, tagValue, tagValue2));
                }
            }
            public void AddCustomTag(string tagname, int tagValue)
            {
                if (CustomTypes != null)
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                else
                {
                    CustomTypes = new List<s_customType>();
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                }
            }
            public List<int> GetCustomTags(string tagname)
            {
                List<s_customType> ty = CustomTypes.FindAll(x => x.name == tagname);
                List<int> ints = new List<int>();

                foreach (s_customType i in ty)
                {
                    ints.Add(i.type);
                }
                return ints;
            }
            /// <summary>
            /// Gets the first string custom type
            /// </summary>
            /// <param name="tagname"></param>
            /// <returns></returns>
            public string GetCustomStringTag(string tagname)
            {
                s_customType ty = CustomTypes.Find(x => x.name == tagname);
                List<int> ints = new List<int>();

                return ty.type3;
            }

            public s_trig(Vector2 trigSize)
            {
                this.trigSize = new s_save_vector(trigSize.x, trigSize.y);
            }
            public bool IsPermanentlyDisabled; // Set to false by default
            public bool unlocked = true;
            public int labelToJumpTo;
            public string stringLabelToJumpTo;
            public bool requiresDependency = false;

            public int pos_x, pos_y;
            public string util;
            public string name;
            public int gemreq;
            public o_trigger.TRIGGER_TYPE trigtye;
            public ev_details[] listofevents;
            public string[] characternames;
            public int[] intlist;
            public bound[] boundaryobj;
            public s_save_vector trigSize;
            public List<StringList> CustomStrings;
            public List<IntList> CustomInt;

            public List<s_customType> CustomTypes;

            public bool utilbool;
            public int utilint;
            public string utilstring;
        }

        [System.Serializable]
        public struct s_customType
        {
            public s_customType(string name, int type, int type2, string type3)
            {
                this.name = name;
                this.type = type;
                this.type2 = type2;
                this.type3 = type3;
                intList = null;
                stringList = null;
                vecList = null;
            }
            public s_customType(string name, int type, int type2)
            {
                this.name = name;
                this.type = type;
                this.type2 = type2;
                type3 = null;
                intList = null;
                stringList = null;
                vecList = null;
            }
            public s_customType(string name, int type)
            {
                this.name = name;
                this.type = type;
                type2 = 0;
                type3 = null;
                intList = null;
                stringList = null;
                vecList = null;
            }
            public s_customType(string name, string type3)
            {
                this.name = name;
                type = 0;
                type2 = 0;
                this.type3 = type3;
                intList = null;
                stringList = null;
                vecList = null;
            }
            public s_customType(string name, List<Tuple<string, List<int>>> intList)
            {
                this.name = name;
                this.type = 0;
                this.type2 = 0;
                type3 = null;
                this.intList = intList;
                stringList = null;
                vecList = null;
            }

            public string name;
            public List<Tuple<string, List<int>>> intList;
            public List<Tuple<string,List<string>>> stringList;
            public List<Tuple<string, List<Vector2>>> vecList;
            public int type;
            public int type2;
            public string type3;
        }

        [System.Serializable]
        public class s_chara
        {
            public s_chara(Vector2 pos, string mapname, string charname, string charType, bool enabled, bool spawnthis, bool disableStatic, string faction)
            {
                this.faction = faction;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                this.disableStatic = disableStatic;
                this.possesed = spawnthis;
                this.charname = charname;
                this.mapname = mapname;
                this.charType = charType;
            }
            public s_chara(Vector2 pos, string charname, string charType, bool enabled, bool spawnthis, bool disableStatic, string faction)
            {
                this.faction = faction;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                this.disableStatic = disableStatic;
                this.possesed = spawnthis;
                this.charname = charname;
                this.charType = charType;
            }
            public string faction;
            public string mapname;
            public string charname;
            public string charType; //Checks for the type of the character
            public bool possesed = false;  //Checks if the character is defeated or not present
            public bool disableStatic;
            public bool enabled;    //Checks if the character's control is enabled
            public int pos_x, pos_y;

            public List<int> GetCustomTags(string tagname)
            {
                List<s_customType> ty = CustomTypes.FindAll(x => x.name == tagname);
                List<int> ints = new List<int>();

                foreach (s_customType i in ty)
                {
                    ints.Add(i.type);
                }
                return ints;
            }
            /// <summary>
            /// Gets the first string custom type
            /// </summary>
            /// <param name="tagname"></param>
            /// <returns></returns>
            public string GetCustomStringTag(string tagname)
            {
                s_customType ty = CustomTypes.Find(x => x.name == tagname);
                List<int> ints = new List<int>();

                return ty.type3;
            }
            public void AddCustomTag(string tagname, string tagValue)
            {
                if (CustomTypes != null)
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                else
                {
                    CustomTypes = new List<s_customType>();
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                }
            }
            public void AddCustomTag(string tagname, int tagValue)
            {
                if (CustomTypes != null)
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                else
                {
                    CustomTypes = new List<s_customType>();
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                }
            }

            public List<s_customType> CustomTypes;
        }

        [System.Serializable]
        public partial class s_tileobj
        {
            public s_tileobj(string objname)
            {
                pos_x = 0;
                pos_y = 0;
                pos_z = 0;
                enumthing = 0;
                TYPENAME = objname;
            }
            public s_tileobj(Vector3 pos, string objname)
            {
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                pos_z = (int)pos.z;
                enumthing = 0;
                TYPENAME = objname;
            }
            public s_tileobj(Vector3 pos, string objname, int enumth, string cha)
            {
                exceptionChar = cha;
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                pos_z = (int)pos.z;
                enumthing = enumth;
                TYPENAME = objname;
            }
            public s_tileobj(Vector3 pos, string objname, int enumth)
            {
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                pos_z = (int)pos.z;
                enumthing = enumth;
                TYPENAME = objname;
            }
            public s_tileobj(Vector2 pos, string objname, Vector2 teleportpos, string mapname)
            {
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                this.teleportpos.x = teleportpos.x;
                this.teleportpos.y = teleportpos.y;
                this.mapname = mapname;
                TYPENAME = objname;
            }

            public s_tileobj(Vector2 pos, string objname, Vector2 teleportpos, string mapname, string flagname, int[] flagchecks, string[] mapnames)
            {
                pos_x = (int)pos.x;
                pos_y = (int)pos.y;
                this.teleportpos = new s_save_vector(teleportpos.x, teleportpos.y);
                this.mapname = mapname;
                TYPENAME = objname;
                this.flagname = flagname;
                this.flagchecks = flagchecks;
                this.mapnames = mapnames;
            }

            public s_customType FindType(string nameoftype)
            {
                if (CustomTypes != null)
                    return CustomTypes.Find(x => x.name == nameoftype);
                else
                    return new s_customType("Null", 0);
            }
            public void AddCustomTag(string tagname, string tagValue)
            {
                if (CustomTypes != null)
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                else
                {
                    CustomTypes = new List<s_customType>();
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                }
            }
            public void AddCustomTag(string tagname, int tagValue)
            {
                if (CustomTypes != null)
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                else
                {
                    CustomTypes = new List<s_customType>();
                    CustomTypes.Add(new s_customType(tagname, tagValue));
                }
            }

            public string flagname;
            public int[] flagchecks;
            public string[] mapnames;

            public List<s_customType> CustomTypes;
            public List<StringList> CustomStrings;
            public List<IntList> CustomInt;
            public s_save_vector tilemapPos;
            public s_save_vector size;
            public s_save_vector teleportpos;
            public string mapname;
            public string areaname;
            public string TYPENAME;
            public string exceptionChar;
            public string name;
            public string cannotPassChar;
            public bool issolid = true;

            public int enumthing = -1;
            public int pos_x, pos_y, pos_z;
            public float height;
        }
    }

}
