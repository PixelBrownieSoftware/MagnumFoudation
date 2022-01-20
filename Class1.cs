
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagnumFoudation
{
    [System.Serializable]
    public class o_item
    {
        public o_item(string name, ITEM_TYPE type, int points)
        {
            this.name = name;
            this.points = points;
            TYPE = type;
        }
        public o_item(string name, ITEM_TYPE type)
        {
            this.name = name;
            TYPE = type;
        }
        public string name;
        public enum ITEM_TYPE
        {
            CONSUMABLE,
            WEAPON,
            KEY_ITEM
        }
        public ITEM_TYPE TYPE;
        public int points;  //This could be how much health it heals or how much damage it deals
        public int quantity;
    };

    [System.Serializable]
    public class o_weapon : o_item
    {
        public o_weapon(int attackPow, string name, WEAPON_TYPE weap) : base(name, ITEM_TYPE.WEAPON, 1)
        {
            level = 1;
            this.attackPow = attackPow;
            weapon_type = weap;
        }
        public enum WEAPON_TYPE
        {
            WEAPON_MELEE,
            WEAPON_RANGED
        }
        public WEAPON_TYPE weapon_type;
        public int attackPow;
        public int level;
    };

    [System.Serializable]
    public struct o_shopItem
    {
        public o_shopItem(o_item item, int price)
        {
            this.item = item;
            this.price = price;
        }
        public o_item item;
        public int price;
    }

    public enum COLLISION_T
    {
        NONE = -1,
        WALL,
        FALLING,
        FALLING_ON_LAND,
        CLIMBING,
        DITCH,
        DAMAGE,
        STAIRS,
        MOVING_PLATFORM,
        LANDING_DOWN,
        WATER_TILE,
        NO_DEPOSEESS,
        LANDING_LEFT,
        LANDING_RIGHT,
        LANDING_UP,
        NO_LASTPOSITION
    }

    public class o_itemObj : s_object
    {
        public int amount;
        /*
        public enum ITEM_TYPE
        {
            MONEY,
            HEALTH,
            MAX_HEALTH,
            AMMO,
            COLLECTIBLE,
            WEAPON
        }
        public ITEM_TYPE it;
        */
        public o_item ItemContain;
        public int indexID = -1; //Used for despawning items, Set to -1 to check if it's not intialised
        public o_weapon WeaponContain;

        new void Start()
        {
            base.Start();
        }

        public virtual void CollectItem(o_character ob) {

            DespawnObject();
        }


        new void Update()
        {
            o_character col = IfTouchingGetCol<o_character>(collision);

            if (col != null)
            {
                o_character p = col.GetComponent<o_character>();

                if (p != null)
                {
                    if (p.GetComponent<o_character>() || !p.AI) {
                        CollectItem(p);
                    }
                    /*
                switch (it)
                {
                    case ITEM_TYPE.MONEY:
                        s_globals.Money += 1;
                        s_map mp = GameObject.Find("General").GetComponent<s_levelloader>().mapDat;
                        mp.gemCount++;
                        s_save_item it = mp.itemdat.Find(x => x.ID == indexID);
                        it.iscollected = true;
                        DespawnObject();
                        break;

                    case ITEM_TYPE.HEALTH:
                        if (p.health < p.maxHealth)
                        {
                            p.health += amount;
                            DespawnObject();
                        }
                        break;

                }
            */
                }
                //DespawnObject();
            }
        }
    }

    public class o_generic : s_object
    {
        public Vector2 uppercollisionsize { get; set; }
        public Vector2 uppercollision { get; set; }
        public GameObject graphic;
        public bool issolid = true;
        public LayerMask layuer;
        public string character;
        public string characterCannot = null;

        public COLLISION_T collision_type;

        public Color32 fallingColour = new Color32(0, 150, 10, 145);
        public Color32 climbingColour = new Color32(153, 10, 10, 145);
        public Color32 defaultColour = new Color32(255, 0, 255, 145);
        public Color32 fallingOnGroundColour = new Color32(88, 230, 250, 145);
        public Color32 ditchColour = new Color32(30, 20, 40, 145);

        public COLLISION_T GetCollisionType()
        {
            return collision_type;
        }

        new private void Start()
        {
            base.Start();
            if (transform.Find("sprite_obj"))
                animHand = transform.Find("sprite_obj").GetComponent<s_animhandler>();
        }

        new void Update()
        {
            base.Update();
            /*

            switch (collision_type)
            {
                case COLLISION_T.STAIRS:
                    cha = GetCharacter(collision, layuer, "PlayerHitbox");
                    if (cha != null)
                    {
                        o_character pl = cha.transform.parent.GetComponent<o_character>();
                        if (Input.GetAxisRaw("Horizontal") > 0)
                        {
                            pl.positioninworld.y += 0.2f;
                        }
                        if (Input.GetAxisRaw("Horizontal") < 0)
                        {
                            pl.positioninworld.y -= 0.2f;
                        }
                    }
                    break;

                case COLLISION_T.FALLING:

                    cha = GetCharacter(collision, layuer);
                    if (cha != null)
                    {

                        o_character pl = cha.GetComponent<o_character>();
                        pl.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;

                        if (pl.grounded == true)
                            pl.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_FALLING;
                    }
                    break;
                case COLLISION_T.FALLING_ON_LAND:
                    cha = GetCharacter(collision, layuer, "Player");
                    if (cha != null)
                    {
                        o_character pl = cha.GetComponent<o_character>();
                        if (pl.grounded)
                        {
                            pl.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_FALLING;
                        }
                    }
                    break;

                case COLLISION_T.CLIMBING:
                    cha = GetCharacter(collision, layuer, "Player");
                    if (cha != null)
                    {
                        o_character pl = cha.GetComponent<o_character>();
                        if (pl.grounded)
                        {
                            pl.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_CLIMBING;
                        }
                    }
                    break;

                case COLLISION_T.DITCH:

                    cha = GetCharacter(collision, layuer, "Player");
                    if (cha != null)
                    {
                        o_character pl = cha.GetComponent<o_character>();
                        if (pl.grounded)
                        {
                            pl.GetComponent<o_character>().TeleportAfterFall();
                        }
                    }
                    break;

                case COLLISION_T.LANDING_DOWN:
                case COLLISION_T.LANDING_UP:
                case COLLISION_T.LANDING_LEFT:
                case COLLISION_T.LANDING_RIGHT:

                    issolid = false;
                    break;
            }
            */
            //transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = (int)(positioninworld.y/height) - 1;
            //Collider2D collision = transform.GetChild(1).GetComponent<BoxCollider2D>();
            //Bounds bound = collision.bounds;

            //bound.center = new Vector3(positioninworld.x, positioninworld.y, 0) ;

        }

        private void OnDrawGizmos()
        {
            if (collision == null)
                collision = GetComponent<BoxCollider2D>();
            else
            {
                if (character != "")
                    Gizmos.DrawWireCube(transform.position, collision.size);
            }
        }

    }

    public class s_object : MonoBehaviour
    {
        public string ID; //To return back to the object pooler
        [HideInInspector]
        public GameObject shadow;
        public float _Z_offset;  //To show the character jumping
        public int Z_floor;
        [HideInInspector]
        public BoxCollider2D collision;
        [HideInInspector]
        public s_nodegraph nodegraph;
        public float terminalSpeedOrigin;
        protected float terminalspd;
        public float gravity;
        [HideInInspector]
        public Rigidbody2D rbody2d;
        protected s_object parentTrans;
        public SpriteRenderer rendererObj;
        protected s_animhandler animHand;
        protected Camera cam;

        public bool useCondition = false;
        public bool disspearOnConditionTrue = false;
        public ev_details.LOGIC_TYPE logicType;
        public string flagName;
        public int conditionNum;

        const float wldgravity = 3.98f;
        public bool isHover = false;
        public bool grounded = true;

        public void Start()
        {
            collision = GetComponent<BoxCollider2D>();
            if (rendererObj != null)
                animHand = rendererObj.GetComponent<s_animhandler>();
        }

        public void DespawnObject()
        {
            if(s_levelloader.LevEd != null)
                s_levelloader.LevEd.DespawnObject(this);
            else
                s_mapManager.LevEd.DespawnObject(this);
        }

        private void OnDrawGizmos()
        {
            if (collision != null)
                Gizmos.DrawWireCube(transform.position, collision.size);
        }

        public s_node CheckNode(float x, float y)
        {
            return CheckNode(new Vector2(x, y));
        }
        public s_node CheckNode(Vector2 v)
        {
            if (nodegraph == null)
                return null;
            s_node no = nodegraph.PosToNode(v);
            if (no == null)
                return null;
            else
                return no;
        }
        public void SetAnimation(string anim, bool isloop, float speed)
        {
            if (animHand != null)
                animHand.SetAnimation(anim, isloop,speed);
        }
        public void SetAnimation(string anim, bool isloop)
        {
            if (animHand != null)
                animHand.SetAnimation(anim, isloop);
        }
        protected void Update()
        {
            if (!isHover)
                _Z_offset += gravity;

            if (_Z_offset > 0)
            {
                grounded = false;
                if (!isHover)
                    gravity -= Time.deltaTime * wldgravity;
            }
            else
            {
                if (!grounded)
                    OnGround();
                grounded = true;
                _Z_offset = 0;
            }
        }
        public virtual void OnGround()
        {

        }


        public Collider2D GetCharacter(BoxCollider2D collisn) { return Physics2D.OverlapBox(transform.position, collisn.size, 0); }
        public Collider2D GetCharacter(BoxCollider2D collisn, string name)
        {
            Collider2D col = Physics2D.OverlapBox(collisn.transform.position, collisn.size, 0);
            if (col == null)
                return null;

            if (col.name == name)
                return col;
            else return null;
        }
        public Collider2D GetCharacter(BoxCollider2D collisn, int lay)
        {
            return Physics2D.OverlapBox(collisn.transform.position, collisn.size, 0, lay);
        }
        public Collider2D GetCharacter(BoxCollider2D collisn, int lay, string nameofobj)
        {
            Collider2D col = Physics2D.OverlapBox(collisn.transform.position, collisn.size, 0, lay);

            if (col == null)
                return null;
            //print(col.name);
            if (col.name == nameofobj)
                return col;
            else return null;
        }

        public Collider2D GetAllCharacters(BoxCollider2D collisn) { return Physics2D.OverlapBox(transform.position, collisn.size, 0); }

        protected T IfTouchingGetCol<T>(BoxCollider2D collisn) where T: s_object
        {
            if (collisn == null)
                return null;

            Collider2D[] chara = Physics2D.OverlapBoxAll(collisn.transform.position, collisn.size, 0);

            if (chara == null)
                return null;
            for (int i = 0; i < chara.Length; i++)
            {
                Collider2D co = chara[i];
                if (co.gameObject == gameObject)
                    continue;

                T obj = co.gameObject.GetComponent<T>();
                if (obj == null)
                    continue;
                return obj;
            }
            return null;
        }
        protected Collider2D IfTouchingGetCol<T>(BoxCollider2D collisn, string character) where T : s_object
        {
            if (collisn == null)
                return null;

            Collider2D[] chara = Physics2D.OverlapBoxAll( collisn.transform.position + (Vector3)collisn.offset, collisn.size, 0);

            if (chara == null)
                return null;
            for (int i = 0; i < chara.Length; i++)
            {
                Collider2D co = chara[i];
                if (co.gameObject == gameObject)
                    continue;

                //print(chara.gameObject.GetComponent<s_object>().GetType());
                s_object obj = chara[i].gameObject.GetComponent<T>();
                if (obj == null)
                    continue;
                if (obj.name == character)
                    return co;
            }
            return null;
        }
        protected Collider2D IfTouchingGetCol(BoxCollider2D collisn, float size_multip, int layer)
        {
            if (collisn == null)
                return null;

            return Physics2D.OverlapBox( collisn.transform.position + (Vector3)collisn.offset, collisn.size * size_multip, 0, layer);
        }
        
        internal bool IfTouching(BoxCollider2D collisn)
        {
            if (collisn == null)
                return false;

            return Physics2D.OverlapBox(transform.position, collisn.size, 0);
        }
        protected bool IfTouching(BoxCollider2D collisn, string nameofobj)
        {
            Collider2D col = Physics2D.OverlapBox(transform.position + collisn.transform.position, collisn.size, 0);
            if (collisn == null)
                return false;

            if (col == null)
                return false;

            if (col.gameObject == gameObject)
                return false;
            if (col)
                return col.name == nameofobj;

            return false;
        }
        protected bool IfTouching(BoxCollider2D collisn, object characterdata)
        {
            if (collisn == null)
                return false;

            Collider2D chara = Physics2D.OverlapBox(collisn.transform.position, collisn.size, 0);

            if (chara == null)
                return false;
            if (chara.gameObject == gameObject)
                return false;

            //print(chara.gameObject.GetComponent<s_object>().GetType());
            s_object obj = chara.gameObject.GetComponent<s_object>();
            if (obj == null)
                return false;

            if (obj.GetType() == characterdata.GetType())
                return true;

            return false;
        }
        protected bool IfTouching(BoxCollider2D collisn, int layer)
        {
            if (collisn == null)
                return false;

            return Physics2D.OverlapBox(transform.position, collisn.size, 0, layer);
        }
        protected bool IfTouching(BoxCollider2D collisn, float size_multip, int layer)
        {
            if (collisn == null)
                return false;

            return Physics2D.OverlapBox(transform.position, collisn.size * size_multip, 0, layer);
        }
        protected bool IfTouching(BoxCollider2D collisn, int layer, string nameofobj)
        {
            if (collisn == null)
                return false;
            Collider2D cap = Physics2D.OverlapBox(transform.position, collisn.size, 0, layer);

            if (cap == null)
                return false;

            if (cap == this)
                return false;

            if (cap.name == name)
                return cap;

            return false;
        }
    }

    public class o_bullet : s_object
    {
        public o_character parent;
        public float attack_pow;
        public bool isbullet = true;
        public Vector2 direction { get; set; }
        protected float timer = 0;

        public void SetTimer(float tmr)
        {
            timer = tmr;
        }

        new protected void Start()
        {
            base.Start();
        }

        public virtual void CheckCharacterIntersect<T>() where T : o_character
        {
            T c = IfTouchingGetCol<T>(collision);
            if (c != null)
            {
                if (c != parent)
                {
                    if (parent.GetTargets().Find(x => x == c) != null)
                    {
                        if (c != parent)
                        {
                            if (c.CHARACTER_STATE != o_character.CHARACTER_STATES.STATE_DASHING)
                            {
                                if(!c.isInvicible)
                                    OnImpact(c);
                            }
                        }
                    }
                }
            }
        }

        new protected void Update()
        {
            base.Update();
            if (isbullet)
            {
                transform.Translate(direction * terminalSpeedOrigin * 60 * Time.deltaTime);

                //For now we'll just hard-code it to be a certain NPC
                /*
                */

                if (timer < 0) OnImpact(); else timer -= Time.deltaTime;
            }
        }

        public virtual void OnImpact()
        {
            if (isbullet)
                DespawnObject();
        }
        public virtual void OnImpact<T>(T character) where T : o_character
        {
            if (isbullet)
                DespawnObject();
            character.OnHit(this);
        }
    }

    [System.Serializable]
    public class s_node
    {
        public int g_cost, h_cost;
        public int f_cost
        {
            get
            {
                return g_cost + h_cost;
            }
        }
        public string type;
        public string characterExclusive;
        public bool walkable = true;
        public Vector2 realPosition;
        public s_node parent;
        public int COLTYPE;
    }

    public class o_nodeobj : o_generic
    {
        public int nodeID;
        public List<o_nodeobj> nieghbours = new List<o_nodeobj>();

        private void OnDrawGizmos()
        {
            foreach (o_nodeobj nod in nieghbours)
            {
                if (nod != null)
                    Gizmos.DrawLine(transform.position, nod.transform.position);
            }

#if UNITY_EDITOR_WIN
        if (Application.isEditor)
            Handles.Label(transform.position, "Node ID: " + nodeID);
#endif
        }
    }
    public class o_character : s_object
    {
        #region variables
        [System.Serializable]
        public struct ai_behaviour
        {
            public ai_behaviour(float timer, ai_function func)
            {
                this.func = func;
                this.timer = timer;
            }
            public ai_function func;
            public float timer;
        }
        public List<o_character> targets = new List<o_character>();

        public bool isInvicible = false;
        public float dashDivider = 2.5f;    //Used for when the controlled character can stop dashing
        public bool detailedAnim = false;

        public Vector2 direction;
        [HideInInspector]
        public Vector2 dashDirection;
        public LayerMask collisionLayer;

        public bool IS_KINEMATIC = true;
        public delegate void ai_function();
        //public Stack<ai_behaviour> AI_QUEUED_STATES = new Stack<ai_behaviour>();
        public ai_function currentAIFunction;
        protected float ai_timer = 0;

        [HideInInspector]
        public float damage_timer = 0;
        protected float fallposy = 0;
        protected float dashSpeed = 0;

        public enum CHARACTER_STATES
        {
            STATE_IDLE,
            STATE_MOVING,
            STATE_DASHING,
            STATE_HURT,
            STATE_ATTACK,
            STATE_JUMPING,
            STATE_NOTHING,
            STATE_FALLING,
            STATE_CLIMBING,
            STATE_DEFEAT
        };
        public CHARACTER_STATES CHARACTER_STATE;
        [HideInInspector]
        public GameObject sensro;
        public o_character target;

        public float health;
        public float maxHealth;

        [HideInInspector]
        public Collider2D hitbox;

        [HideInInspector]
        public GameObject SpriteObj;
        protected Vector2 spriteOffset = new Vector2(0,0);
        
        protected float velSlip = 0.85f;
        public GameObject attackobject;

        protected float crashTimer = 0;
        public float dashdelayStart = 0;
        public float dashdelay = 0;
        protected float angle;
        protected Vector3 mouse;
        protected Vector3 spawnpoint;
        protected Vector3 lastposbeforefall;
        public Vector3 offsetCOL;

        public bool AI;
        public bool control = true;
        public string faction;
        bool afterDefeatPlayed = false;

        protected s_node CurrentNode;

        public bool AI_timerUp { get { return ai_timer <= 0; } }

        public float walktimer { get; private set; }
        #endregion
        
        public void Initialize()
        {
            rbody2d = GetComponent<Rigidbody2D>();
            if (transform.Find("sprite_obj") != null)
                SpriteObj = transform.Find("sprite_obj").gameObject;
            collision = GetComponent<BoxCollider2D>();
            if (SpriteObj != null)
                rendererObj = SpriteObj.GetComponent<SpriteRenderer>();
            health = maxHealth;
            terminalspd = terminalSpeedOrigin;
            nodegraph = GameObject.Find("General").GetComponent<s_nodegraph>();
            cam = GameObject.Find("CameraGame").GetComponent<Camera>();
        }

        public T CharacterType<T>() where T : o_character
        {
            return GetComponent<T>();
        }
        public T[] CharacterTypeS<T>(List<o_character> charaList) where T : o_character
        {
            List<T> charaListAr = new List<T>();
            for (int i = 0; i < charaList.Count; i++)
            {
                T chara = charaList[i].GetComponent<T>();
                if(chara != null)
                    charaListAr.Add(chara);
            }
            T[] returnVal = charaListAr.ToArray();
            return returnVal;
        }

        public void SetSpawnPoint(Vector2 vec)
        {
            spawnpoint = vec;
        }

        public void HurtFunction(float damage)
        {
            health -= damage;
            damage_timer = 0.4f;
            //Perhaps spawn particles and things here
        }

        protected void SetAIFunction(float timer, ai_function function)
        {
            ai_timer = timer;
            currentAIFunction = function;
        }

        public Vector2 MouseAng()
        {
            Vector2 mouseRet = new Vector2();
            if (cam != null)
                mouseRet = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            else
            {
                cam = GameObject.Find("CameraGame").GetComponent<Camera>();
                mouseRet = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            }
            angle = ReturnAngle(new Vector3(mouse.x, mouse.y, 0));
            return mouseRet;
        }

        /*
        protected void PopAIFunction()
        {
            AI_QUEUED_STATES.Pop();
        }
        protected void AddAIFunction(float timer, ai_function function)
        {
            if (ai_timer >= 0)
                ai_timer = timer;
            AI_QUEUED_STATES.Push(new ai_behaviour(timer, function));
        }

        public void SwapAIFunction(float timer, ai_function function)
        {
            if (ai_timer >= 0)
                ai_timer = timer;
            AI_QUEUED_STATES.Pop();
            AI_QUEUED_STATES.Push(new ai_behaviour(timer, function));
        }
        */

        public Vector3 LookAtTarget(o_character target)
        {
            return (target.transform.position - transform.position).normalized;
        }
        public Vector3 LookAtTarget(Vector2 target)
        {
            return (target - (Vector2)transform.position).normalized;
        }

        public bool CheckTargetDistance(o_character target, float distance)
        {
            float calcdis = Vector2.Distance(target.transform.position, transform.position);
            if (Physics2D.Linecast(target.transform.position, transform.position, collisionLayer))
                return false;
            return calcdis <= distance;
        }
        public bool CheckTargetDistance(Vector2 target, float distance)
        {
            float calcdis = Vector2.Distance(target, transform.position);
            if (Physics2D.Linecast(target, transform.position, collisionLayer))
                return false;
            return calcdis <= distance;
        }

        public void SetTransformPar(s_object o)
        {
            parentTrans = o;
        }
        public bool ArrowKeyControlPref()
        {
            if (Input.GetKey(s_globals.GetKeyPref("right")))
            {
                direction.x = 1;
            }
            if (Input.GetKey(s_globals.GetKeyPref("left")))
            {
                direction.x = -1;
            }
            if (Input.GetKey(s_globals.GetKeyPref("up")))
            {
                direction.y = 1;
            }
            if (Input.GetKey(s_globals.GetKeyPref("down")))
            {
                direction.y = -1;
            }
            if (!Input.GetKey(s_globals.GetKeyPref("left")) && !Input.GetKey(s_globals.GetKeyPref("right"))
                && (Input.GetKey(s_globals.GetKeyPref("up")) || Input.GetKey(s_globals.GetKeyPref("down"))))
            {
                direction.x = 0;
            }
            if (!Input.GetKey(s_globals.GetKeyPref("up")) && !Input.GetKey(s_globals.GetKeyPref("down"))
                && (Input.GetKey(s_globals.GetKeyPref("left")) || Input.GetKey(s_globals.GetKeyPref("right"))))
            {
                direction.y = 0;
            }
            if (Input.GetKey(s_globals.GetKeyPref("up")) ||
                Input.GetKey(s_globals.GetKeyPref("down")) ||
                Input.GetKey(s_globals.GetKeyPref("left")) ||
                Input.GetKey(s_globals.GetKeyPref("right")))
            {
                return true;
            }
            return false;
        }

        public bool ArrowKeyControl()
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
                return true;
            }
            //direction = new Vector2(0, 0);
            return false;
        }

        public Vector2 ArrowKeyControlPerfGetVec()
        {
            Vector2 v = new Vector2();
            if (Input.GetKeyDown(s_globals.GetKeyPref("right")))
            {
                v.x = 1;
            }
            if (Input.GetKeyDown(s_globals.GetKeyPref("left")))
            {
                v.x = -1;
            }
            if (!Input.GetKeyDown(s_globals.GetKeyPref("left")) && !Input.GetKeyDown(s_globals.GetKeyPref("right")))
            {
                v.x = 0;
            }
            if (Input.GetKeyDown(s_globals.GetKeyPref("up")))
            {
                v.y = 1;
            }
            if (Input.GetKeyDown(s_globals.GetKeyPref("down")))
            {
                v.y = -1;
            }
            if (!Input.GetKeyDown(s_globals.GetKeyPref("up")) && !Input.GetKeyDown(s_globals.GetKeyPref("down")))
            {
                v.y = 0;
            }
            //direction = new Vector2(0, 0);
            return v;
        }
        public Vector2 ArrowKeyControlGetVec()
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }
            //direction = new Vector2(0, 0);
            return Vector2.zero;
        }

        public void Dash(float delay)
        {
            if (dashdelay > 0)
                return;
            dashdelayStart = delay;
            dashdelay = delay;
            dashSpeed = 1;
            dashDirection = direction;
            CHARACTER_STATE = CHARACTER_STATES.STATE_DASHING;
        }
        public void Dash(float delay, float sped)
        {
            if (dashdelay > 0)
                return;
            dashdelayStart = delay;
            dashdelay = delay;
            dashSpeed = sped;
            dashDirection = direction;
            CHARACTER_STATE = CHARACTER_STATES.STATE_DASHING;
        }

        public bool Jump(float gravity)
        {
            if (grounded)
            {
                this.gravity = gravity;
                return true;
            }
            return false;
        }

        public virtual void OnHit(o_bullet b)
        {
        }
        public bool JumpWithoutGround(float gravity, float ZPOSlimit)
        {
            if (ZPOSlimit > _Z_offset)
            {
                this.gravity = gravity;
                return true;
            }
            return false;
        }

        public virtual void WalkControl()
        {
            if (walktimer >= 0)
            {
                walktimer -= Time.deltaTime;
                /*
                if (Physics2D.Raycast(transform.position, direction * 4))
                {
                    direction *= -1;
                }
                */
            }
            else
            {
                int random = UnityEngine.Random.Range(0, 2);
                switch (random)
                {
                    case 0:
                        Move();
                        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                        direction = new Vector2(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2)).normalized;
                        break;

                    case 1:
                        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                        break;
                }
                walktimer = UnityEngine.Random.Range(0.6f, 3);
            }
        }

        public void PlaySound(string sound) {
            s_soundmanager.sound.PlaySound(sound);
        }

        public virtual void AnimMove()
        {
            if (detailedAnim)
            {
                int verticalDir = Mathf.RoundToInt(direction.y);
                int horizontalDir = Mathf.RoundToInt(direction.x);

                if (CHARACTER_STATE == CHARACTER_STATES.STATE_MOVING)
                {
                    if (verticalDir == -1 && horizontalDir == 0)
                        SetAnimation("walk_d", true);
                    else if (verticalDir == 1 && horizontalDir == 0)
                        SetAnimation("walk_u", true);
                    else if (horizontalDir == -1 && verticalDir == 1 ||
                        horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
                    {
                        rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        SetAnimation("walk_s", true);
                    }
                    else if (horizontalDir == 1 && verticalDir == 1 ||
                        horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
                    {
                        rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                        SetAnimation("walk_s", true);
                    }
                }
                if (CHARACTER_STATE == CHARACTER_STATES.STATE_IDLE)
                {
                    if (verticalDir == -1 && horizontalDir == 0)
                        SetAnimation("idle_d", true);
                    else if (verticalDir == 1 && horizontalDir == 0)
                        SetAnimation("idle_u", true);
                    else if (horizontalDir == -1 && verticalDir == 1 ||
                        horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
                    {
                        rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        SetAnimation("idle_s", true);
                    }
                    else if (horizontalDir == 1 && verticalDir == 1 ||
                        horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
                    {
                        rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                        SetAnimation("idle_s", true);
                    }

                }
            }
            else
            {

                if (CHARACTER_STATE == CHARACTER_STATES.STATE_MOVING)
                {
                    SetAnimation("walk_d", true);
                }
                if (CHARACTER_STATE == CHARACTER_STATES.STATE_IDLE)
                {
                    SetAnimation("idle_d", true);
                }
            }
        }
        public void CheckForVelLimits()
        {
            if (Mathf.Abs(rbody2d.velocity.y) > terminalspd)
            {
                float signedVel = Mathf.Sign(rbody2d.velocity.y);
                rbody2d.velocity = new Vector2(rbody2d.velocity.x, terminalspd * signedVel);
            }
            if (Mathf.Abs(rbody2d.velocity.x) > terminalspd)
            {
                float signedVel = Mathf.Sign(rbody2d.velocity.x);
                rbody2d.velocity = new Vector2(terminalspd * signedVel, rbody2d.velocity.y);
            }
        }

        protected void SetAttackObject<T>(string attkObName, float pow) where T : o_bullet
        {
            if (transform.Find(attkObName) != null)
            {
                attackobject = transform.Find(attkObName).gameObject;
                attackobject.GetComponent<o_bullet>().attack_pow = pow;
                attackobject.GetComponent<o_bullet>().parent = this;
            }
        }
        protected void SetAttackObject<T>(string attkObName) where T : o_bullet
        {
            if (transform.Find(attkObName) != null)
            {
                attackobject = transform.Find(attkObName).gameObject;
                attackobject.GetComponent<o_bullet>().attack_pow = 1;
                attackobject.GetComponent<o_bullet>().parent = this;
            }
        } 
        protected void SetAttackObject<T>() where T : o_bullet
        {
            if (transform.Find("attack_object") != null)
            {
                attackobject = transform.Find("attack_object").gameObject;
                attackobject.GetComponent<o_bullet>().attack_pow = 1;
                attackobject.GetComponent<o_bullet>().parent = this;
            }
        } 
        protected void SetAttackObject<T>(int pow) where T : o_bullet
        {
            if (transform.Find("attack_object") != null)
            {
                attackobject = transform.Find("attack_object").gameObject;
                attackobject.GetComponent<o_bullet>().attack_pow = pow;
                attackobject.GetComponent<o_bullet>().parent = this;
            }
        } 

        protected void AddForce(Vector2 mouse, float power)
        {

        }

        public List<o_character> GetTargets()
        {
            return targets;
        }
        public T GetClosestTarget<T>(float range) where T : o_character
        {
            T targ = null;
            float smallest = float.MaxValue;
            foreach (T c in targets)
            {
                if (c == null)
                    continue;
                if (c == this)
                    continue;
                if (!c.gameObject.activeSelf)
                    continue;
                float dist = TargetDistance(c);
                if (dist > range)
                    continue;
                //Get this target for now if nothing else is better
                if (targ == null)
                    targ = c;
                if (dist <= smallest) {
                    targ = c;
                    smallest = dist;
                }
            }
            return targ;
        }
        public T GetClosestTarget<T>() where T : o_character
        {
            T targ = null;
            float smallest = float.MaxValue;
            foreach (T c in targets)
            {
                if (c == this)
                    continue;
                float dist = TargetDistance(c);
                if (dist < smallest)
                {
                    targ = c;
                    smallest = dist;
                }
            }
            return targ;
        }

        public float TargetDistance(o_character targ)
        {
            return Vector2.Distance(targ.transform.position, transform.position);
        }
        
        public void AddFactions(List<o_character> alc)
        {
            targets.Clear();
            foreach (o_character c in alc)
            {
                if (c == null)
                    continue;
                if (c == this)
                    continue;
                if (c.faction == "")
                {
                    targets.Add(c);
                    continue;
                }
                if (c.faction == faction
                    || c.faction == "N/A"
                    || c.faction == "N/a"
                    || c.faction == "n/A"
                    || c.faction == "n/a")
                    continue;
                targets.Add(c);
            }
        }

        protected void EnableAttack(BoxCollider2D col)
        {
            if (col != null)
            {
                col.gameObject.SetActive(true);
                attackobject.transform.localPosition = direction * 20;
            }
        }
        protected void EnableAttack()
        {
            if (attackobject != null)
            {
                attackobject.SetActive(true);
                attackobject.transform.localPosition = direction * 20;
            }
        }
        protected void EnableAttack(Vector2 dir)
        {
            if (attackobject != null)
            {
                attackobject.SetActive(true);
                attackobject.transform.localPosition = dir * 20;
                attackobject.GetComponent<o_bullet>().direction = dir;
            }
        }
        protected void DisableAttack()
        {
            if (attackobject != null)
                attackobject.SetActive(false);
        }
        protected void DisableAttack(BoxCollider2D col)
        {
            if (col != null)
                col.gameObject.SetActive(false);
        }

        protected float ReturnAngle2(Vector3 position)
        {
            Vector2 unitvec = position.normalized;
            return Mathf.Atan2(unitvec.x, unitvec.y) * Mathf.Rad2Deg;
        }
        protected float ReturnAngle(Vector3 position)
        {
            return 360 - Mathf.Atan2(position.x, position.y) * Mathf.Rad2Deg;
        }

        public void AssignToFaction()
        {

        }

        /// <summary>
        /// This is the state when the enemy does not really see their target. They don't really do much other than just walk around.
        /// Maybe if a sound is heard they could walk towards it.
        /// </summary>
        public virtual void IdleState() { }

        /// <summary>
        /// This is the state which the enemy would enter into when they are done attacking or possibly low on health. This is so that they do not just
        /// run aimlessly towards the player and constantly attack.
        /// </summary>
        public virtual void RetreatState() { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void AttackState() { }

        /// <summary>
        /// This is the state where the AI would chase their target, it would often lead into the attack state or the retreat state
        /// </summary>
        public virtual void ActiveState() { }

        Vector2 colPos;
        public bool CheckIfCornered(Vector3 dir)
        {
            Vector2 siz = collision.size * 0.9f;

            Vector2 offset =
                new Vector2(
                    collision.transform.position.x + collision.offset.x,
                    collision.transform.position.y + collision.offset.y);

            colPos = (Vector3)offset + dir * 2;
            Collider2D col = Physics2D.OverlapBox(colPos, siz, 0, collisionLayer);

            if (col != null)
                return true;
            return false;
        }

        void Move()
        {
            switch (CHARACTER_STATE)
            {
                case CHARACTER_STATES.STATE_NOTHING:
                    if (crashTimer > 0)
                        crashTimer -= Time.deltaTime;
                    if (crashTimer <= 0)
                    {
                        dashdelay = 0;
                        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                        AfterDash();
                    }
                    break;

                case CHARACTER_STATES.STATE_IDLE:
                    if (rbody2d != null)
                        rbody2d.velocity *= 0.85f;
                    break;

                case CHARACTER_STATES.STATE_MOVING:
                    if (rbody2d != null)
                        rbody2d.velocity = direction * terminalspd;
                    //transform.Translate(velocity * Time.deltaTime);
                    break;

                case CHARACTER_STATES.STATE_FALLING:
                    transform.position -= new Vector3(0, 5, 0);
                    if (transform.position.y <= fallposy)
                    {
                        dashdelay = 0;
                        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                    }
                    break;

                case CHARACTER_STATES.STATE_HURT:
                    if (damage_timer <= 0)
                    {
                        rendererObj.color = Color.white;
                        dashdelay = 0;
                        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                    }
                    else {
                        rendererObj.color = Color.red;
                    }
                    break;

                case CHARACTER_STATES.STATE_DASHING:
                    if (dashdelay <= 0) {
                        AfterDash();
                    } else {
                        rbody2d.velocity = dashDirection * terminalspd * dashSpeed;
                        if (dashdelay < dashdelayStart / dashDivider)
                            if (!AI)
                                if (!Input.GetKey(s_globals.GetKeyPref("dash")))
                                    dashdelay = 0;

                        dashdelay -= Time.deltaTime;
                        //Check for a wall in front if NOCLIP mode isn't on
                        if (!collision.isTrigger && CheckIfCornered(dashDirection * 1.5f)) {
                            CrashAfterDash();
                        }
                    }
                    break;
            }
            
            if (SpriteObj != null) {
                SpriteObj.transform.position = new Vector3(transform.position.x + spriteOffset.x, transform.position.y + spriteOffset.y + _Z_offset);
            }
            if (IS_KINEMATIC) {
                collision.isTrigger = false;
                COLLISIONDET();
            } else {
                collision.isTrigger = true;
            }
        }

        public virtual void AfterDash()
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        }

        public virtual void AfterDefeat()
        {
        }

        public virtual void CrashAfterDash()
        {
            crashTimer = 0.38f;
            rbody2d.velocity = Vector2.zero;
            CHARACTER_STATE = CHARACTER_STATES.STATE_NOTHING;
        }
        public void CrashOnDash()
        {
        }

        public new void Update()
        {
            base.Update();
            if (control)
            {
                if (health > 0)
                {
                    if (AI)
                        ArtificialIntelleginceControl();
                    else
                        PlayerControl();
                }
            }
        }

        protected void FixedUpdate()
        {
            if (damage_timer > 0)
            {
                damage_timer -= Time.deltaTime;
                rendererObj.color = Color.red;
            }

            if (ai_timer > 0)
                ai_timer = ai_timer - Time.deltaTime;

            if (health <= 0)
            {
                if (CHARACTER_STATE != CHARACTER_STATES.STATE_DEFEAT)
                {
                    if (!afterDefeatPlayed)
                    {
                        afterDefeatPlayed = true;
                        AfterDefeat();
                    }
                    CHARACTER_STATE = CHARACTER_STATES.STATE_DEFEAT;
                }
            }
            else {
                afterDefeatPlayed = false;
            }
            if (CHARACTER_STATE == CHARACTER_STATES.STATE_IDLE)
            {
                rbody2d.velocity *= velSlip;
            }
            Move();

        }


        
        public virtual void PlayerControl()
        {

        }

        public virtual void ArtificialIntelleginceControl()
        {

        }

        public void Pushforce(Vector2 dir, float force)
        {
            rbody2d.velocity += dir * force;
        }

        public void TeleportAfterFall()
        {
            transform.position = lastposbeforefall;
        }

        void COLLISIONDET()
        {

            if (nodegraph != null)
            {
                if (IS_KINEMATIC)
                {
                    CurrentNode = CheckNode(transform.position + offsetCOL);
                    // s_gui.AddText(nodeg.PosToVec(new Vector2( positioninworld.x + collision.size.x / 2,  positioninworld.y + collision.size.y / 2)).ToString());

                    if (CurrentNode != null)
                    {
                        NodeCollisionResponse(CurrentNode);
                    }

                }
            }
        }

        public void Hurt(o_bullet b)
        {
            //CHARACTER_STATE = CHARACTER_STATES.STATE_HURT;
            HurtFunction(b.attack_pow);
            Pushforce(b.direction, 135f);
        }

        void NodeCollisionResponse(s_node nod)
        {

        }

        protected T GetBullet<T>(BoxCollider2D collisn) where T : o_bullet
        {
            if (collisn == null)
                return null;

            Collider2D[] chara = Physics2D.OverlapBoxAll(transform.position, collisn.size, 0);

            if (chara == null)
                return null;
            for (int i = 0; i < chara.Length; i++)
            {
                Collider2D co = chara[i];
                if (co.gameObject == gameObject)
                    continue;
                
                T b = co.gameObject.GetComponent<T>();
                if (b == null)
                    continue;
                if (targets.Find(x => x == b.parent) != null)
                {
                    o_character host = b.parent;
                    if (host != this)
                    {
                        return b;
                    }
                }
            }
            return null;
        }

    }

    public class s_camera : MonoBehaviour
    {
        public o_character player;
        public static s_camera cam;
        public Vector2 offset;
        float offset_multiplier;
        public float orthGraphicSize = 0;

        public Vector2 mapSize;
        public Vector2 tileSize;

        public Vector2 targetPos;
        Vector2 startPos;
        float speedProg = 0;
        public float speedProgInc = 0.15f;
        
        public bool lerping = false;
        public bool focus = true;
        public bool camWithMouseOffset = false;
        public bool debug = false;
        public static Vector2 position;

        private float camerashakeDel;
        private float cameraoffset_X;
        private float cameraoffset_Y;

        void Awake()
        {
            if (cam == null)
                cam = this;
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            if (GetComponent<Camera>() != null)
                orthGraphicSize = GetComponent<Camera>().orthographicSize * 2;
        }

        public void SetPlayer(o_character cha)
        {
            player = cha;
        }

        public void ResetSpeedProg()
        {
            speedProg = 0;
        }

        void FixedUpdate()
        { 
            if (s_globals.globalSingle != null)
            {
                if (s_globals.globalSingle.isMainGame)
                {

                    position = transform.position;
                    if (lerping)
                    {
                        transform.position = Vector3.Lerp(transform.position, targetPos, speedProg) + new Vector3(0, 0, -15);
                        speedProg += speedProgInc * Time.deltaTime;
                    }
                    else if (focus)
                    {
                        if (player != null)
                        {
                            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                            offset = (mouse - player.transform.position).normalized;

                            if (Vector2.Distance(mouse, player.transform.position) > 9)
                            {
                                offset_multiplier = Vector3.Distance(player.transform.position, mouse) * 0.95f;
                            }

                            offset_multiplier = Mathf.Clamp(offset_multiplier, 0, 50f);
                            offset *= offset_multiplier;
                            Vector3 vec = Vector3.Lerp(player.transform.position, transform.position, 0.6f);

                            vec.y = Mathf.RoundToInt(vec.y);
                            vec.x = Mathf.RoundToInt(vec.x);

                            if (vec.x < orthGraphicSize)
                                vec.x = orthGraphicSize;

                            if (camWithMouseOffset)
                                vec = new Vector3(vec.x, vec.y, -10) + (Vector3)offset;
                            else
                                vec = new Vector3(vec.x, vec.y, -10);

                            if (s_levelloader.LevEd != null)
                            {
                                if (vec.x > s_levelloader.LevEd.mapsizeToKeep.x * s_levelloader.LevEd.tilesize - orthGraphicSize)
                                    vec.x = s_levelloader.LevEd.mapsizeToKeep.x * s_levelloader.LevEd.tilesize - orthGraphicSize;

                                if (vec.y < orthGraphicSize / 2)
                                    vec.y = orthGraphicSize / 2;
                                if (vec.y > s_levelloader.LevEd.mapsizeToKeep.y * s_levelloader.LevEd.tilesize + orthGraphicSize / 2)
                                    vec.y = s_levelloader.LevEd.mapsizeToKeep.y * s_levelloader.LevEd.tilesize + orthGraphicSize / 2;
                            }
                            else
                            {
                                if (vec.x > mapSize.x * tileSize.x - orthGraphicSize)
                                    vec.x = mapSize.x * tileSize.x - orthGraphicSize;

                                if (vec.y < orthGraphicSize / 2)
                                    vec.y = orthGraphicSize / 2;
                                if (vec.y > mapSize.y * tileSize.y + orthGraphicSize / 2)
                                    vec.y = mapSize.y * tileSize.y + orthGraphicSize / 2;
                            }
                            transform.position = new Vector3(vec.x, vec.y, -10);
                        }
                    }

                    if (camerashakeDel > 0)
                    {
                        float cam_off_x = UnityEngine.Random.Range(-cameraoffset_X, cameraoffset_X);
                        float cam_off_y = UnityEngine.Random.Range(-cameraoffset_Y, cameraoffset_Y);

                        transform.position = new Vector3(transform.position.x + cam_off_x, transform.position.y + cam_off_y, -23);

                        camerashakeDel = camerashakeDel - Time.deltaTime;

                    }
                }
            }
        }

        public void CameraShake(float x, float y, float delay)
        {
            cameraoffset_X = x;
            cameraoffset_Y = y;

            camerashakeDel = delay;
        }

        public bool CameraLerp()
        {
            if ((Vector2)transform.position == targetPos)
            {
                lerping = false;
                return true;
            }
            return false;
        }

        public void CameraLerpInit(Vector2 _startpos, Vector2 _targetpos)
        {
            startPos = _startpos + (Vector2)new Vector3(0, 0, -15);
            targetPos = _targetpos + (Vector2)new Vector3(0, 0, -15);
            speedProg = 0;
            lerping = true;
        }
    }
    /*
    public void ShootBullet(o_weapon weap)
    {
        o_bullet bullt = s_leveledit.LevEd.SpawnObject("Bullet", transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<o_bullet>();
        bullt.attack_pow = weap.attackPow;
        bullt.SetTimer();
        bullt.parent = this;
    }
    public void ShootBullet(int pow)
    {
        o_bullet bullt = s_leveledit.LevEd.SpawnObject("Bullet", transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<o_bullet>();
        bullt.attack_pow = pow;
        bullt.SetTimer();
        bullt.parent = this;
    }
    */
}

public interface IPoolerObj {
    void SpawnStart();
}
