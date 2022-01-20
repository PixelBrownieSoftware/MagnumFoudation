using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
using System.Collections;

namespace MagnumFoudation
{
    public class s_events
    {

        public enum TRIGGER_TYPE
        {
            CONTACT,
            CONDITION,
            BUTTON_PRESS
        }
        public TRIGGER_TYPE TRIGGER;
        public int index = 0;

        public ev_details[] ev_Details = new ev_details[3];    //Hard-code it to be 3 for now
        public o_trigger trigger;

        void Start()
        {

        }

        void Update()
        {

        }

        /*
        void TriggerFUNC()
        {
            ev_details ev_current = ev_Details[index];
            switch (ev_Details[index].eventType)
            {
                case ev_details.EVENT_TYPES.CHECK_VARIABLES:

                    switch (ev_current.logic)
                    {
                        case ev_details.LOGIC_TYPE.VAR_EQUAL:
                            if (s_leveledit.system_Leveledit.CheckIntegersEqual(ev_current.string0, ev_current.int0))
                            {
                                index = ev_current.jump;
                                ev_current = ev_Details[index];
                            }
                            break;
                        case ev_details.LOGIC_TYPE.VAR_GREATER:
                            if (s_leveledit.system_Leveledit.CheckIntegersGreaterThan(ev_current.string0, ev_current.int0))
                            {
                                index = ev_current.jump;
                                ev_current = ev_Details[index];
                            }
                            break;
                        case ev_details.LOGIC_TYPE.VAR_LESS:
                            if (s_leveledit.system_Leveledit.CheckIntegersLessThan(ev_current.string0, ev_current.int0))
                            {
                                index = ev_current.jump;
                                ev_current = ev_Details[index];
                            }
                            break;

                    }



                    break;
                case ev_details.EVENT_TYPES.SET_VARIABLE:
                    break;
            }
        }
            */

    }

    public class o_trigger : s_object
    {
        public LayerMask layer;
       // [HideInInspector]
       // private gui_dialogue Dialogue;
        public enum TRIGGER_TYPE
        {
            CONTACT,
            CONTACT_INPUT,
            NONE
        }
        public BoxCollider2D colider;
        public TRIGGER_TYPE TRIGGER_T;
        [HideInInspector]
        public bool isactive = false;
        [HideInInspector]
        const float shutterdepth = 1.55f;
        [HideInInspector]
        public o_character charac;
        public int LabelToJumpTo;
        public string stringLabelToJumpTo;

        //[HideInInspector]
        //Image fade;
        public bool callstatic = false;

        public s_events Events;

        [HideInInspector]
        public o_character[] characters;

        [HideInInspector]
        public int ev_num = 0;
        s_object selobj;

        public bool destroyOnTouch = false;


        public void Awake()
        {
            if (useCondition) {

                bool cond = false;
                int flagNum = s_globals.GetGlobalFlag(flagName);

                switch (logicType) {
                    case ev_details.LOGIC_TYPE.VAR_EQUAL:
                        cond = (flagNum == conditionNum);
                        break;

                    case ev_details.LOGIC_TYPE.VAR_GREATER:
                        cond = (flagNum > conditionNum);
                        break;

                    case ev_details.LOGIC_TYPE.VAR_LESS:
                        cond = (flagNum < conditionNum);
                        break;

                    case ev_details.LOGIC_TYPE.VAR_NOT_EQUAL:
                        cond = (flagNum != conditionNum);
                        break;
                }
                if (cond)
                {
                    if (disspearOnConditionTrue)
                        Destroy(gameObject);
                }
                if (!cond)
                {
                    if (!disspearOnConditionTrue)
                        Destroy(gameObject);
                }


            }
        }

        public void Initialize()
        {
            //fade = GameObject.Find("GUIFADE").GetComponent<Image>();
            colider = GetComponent<BoxCollider2D>();
            //if (GameObject.Find("Dialogue"))
                //Dialogue = GameObject.Find("Dialogue").GetComponent<gui_dialogue>();
            rendererObj = GetComponent<SpriteRenderer>();
        }

        public new void Update()
        {
            base.Update();
            if (!s_triggerhandler.trig.doingEvents)
            {
                switch (TRIGGER_T)
                {
                    case TRIGGER_TYPE.CONTACT:

                        o_character c = IfTouchingGetCol<o_character>(collision);
                        if (c != null)
                        {
                            selobj = c.gameObject.GetComponent<s_object>();
                            //print(name + c.name);
                            if (selobj)
                            {
                                o_character posses = selobj.GetComponent<o_character>();
                                //print(name + c.name);
                                o_character ch = c.GetComponent<o_character>();
                                if (ch)
                                {
                                    if (!ch.AI)
                                    {
                                        //print("Activating trigger");
                                        s_triggerhandler.trig.selobj = selobj;
                                        if(stringLabelToJumpTo != "")
                                            s_triggerhandler.trig.JumpToEvent(stringLabelToJumpTo, callstatic);
                                        else
                                            s_triggerhandler.trig.JumpToEvent(LabelToJumpTo, callstatic);
                                        if (destroyOnTouch)
                                            Destroy(gameObject);
                                    }
                                }
                            }
                        }
                        break;

                    case TRIGGER_TYPE.CONTACT_INPUT:
                        if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                        {
                            c = IfTouchingGetCol<o_character>(collision);
                            if (c != null)
                            {
                                selobj = c.gameObject.GetComponent<s_object>();
                                //print(name + c.name);
                                if (selobj)
                                {
                                    o_character posses = selobj.GetComponent<o_character>();
                                    o_character ch = posses.GetComponent<o_character>();
                                    if (ch)
                                        if (!ch.AI)
                                        {
                                            //print("Activating trigger");
                                            s_triggerhandler.trig.selobj = selobj;
                                            if (stringLabelToJumpTo != "")
                                                s_triggerhandler.trig.JumpToEvent(stringLabelToJumpTo, callstatic);
                                            else
                                                s_triggerhandler.trig.JumpToEvent(LabelToJumpTo, callstatic);
                                            if (destroyOnTouch)
                                                Destroy(gameObject);
                                        }
                                }
                            }
                        }
                        break;
                }
            }

        }

        public void CallTrigger()
        {
            s_triggerhandler.trig.JumpToEvent(LabelToJumpTo, callstatic);
            isactive = true;
        }

        TextAsset Loadtextasset(string pathname)
        {
            return (TextAsset)Resources.Load("dialogue/" + pathname);
        }

        public void IncrementEvent()
        {
            ev_num++;
        }
        /*
        public IEnumerator EventPlayMast()
        {
            doingEvents = true;
            Image sh1 = GameObject.Find("Shutter1").GetComponent<Image>();
            Image sh2 = GameObject.Find("Shutter2").GetComponent<Image>();

            for (int i = 0; i < 30; i++) {
                sh1.rectTransform.position += new Vector3(0, -shutterdepth);
                sh2.rectTransform.position += new Vector3(0, shutterdepth);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            while (ev_num < Events.ev_Details.Length)
            {
                ev_details current_ev = Events.ev_Details[ev_num];
                if (current_ev.simultaneous)
                {
                    StartCoroutine(EventPlay(current_ev));
                } else {
                    yield return StartCoroutine(EventPlay(current_ev));
                }
                if (!isactive)
                {
                    break;
                }

                print("Increment");
                ev_num++;
            }
            for (int i = 0; i < 30; i++)
            {
                sh1.rectTransform.position += new Vector3(0, shutterdepth);
                sh2.rectTransform.position += new Vector3(0, -shutterdepth);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            doingEvents = false;
            isskipping = false;
            Time.timeScale = 1;
            first_move_event = true;
        }

        IEnumerator EventPlay(ev_details currnet_ev)
        {
            ev_details current_ev = Events.ev_Details[ev_num];
            switch (current_ev.eventType)
            {

                case ev_details.EVENT_TYPES.ANIMATION:

                    Animator character = selobj.GetComponent<Animator>();
                    character.Play(current_ev.int0);
                    character.speed = current_ev.float0;
                    break;

                case ev_details.EVENT_TYPES.MOVEMNET:

                    float timer = 1.02f;

                    Vector2 newpos = selobj.positioninworld;
                    if (first_move_event)
                        newpos = positioninworld;

                    first_move_event = false;

                    while (timer > 0)
                    {
                        newpos += (current_ev.direcion.normalized * current_ev.float0 * current_ev.float1) * 0.007f;

                        timer -= 0.007f;
                    }

                    float dist = Vector2.Distance(selobj.positioninworld, newpos);
                    Vector2 dir = (newpos - new Vector2(selobj.positioninworld.x, selobj.positioninworld.y)).normalized;
                    print(newpos);


                    while (Vector2.Distance(selobj.positioninworld, newpos)
                        > dist * 0.01f)
                    {
                        selobj.positioninworld += (Vector3)(dir * current_ev.float0 * current_ev.float1) * 0.007f;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    break;

                case ev_details.EVENT_TYPES.DIALOGUE:
                    Dialogue.done_event = false;
                    StartCoroutine(Dialogue.DisplayDialogue(current_ev.string0));
                    while (!Dialogue.done_event)
                    {
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    break;

                case ev_details.EVENT_TYPES.BREAK_EVENT:
                    selobj.GetComponent<o_plcharacter>().control = true;
                    isactive = false;
                    if (current_ev.boolean)     //Does this cutscene reset?
                    {
                        ev_num = 0;   //Reset the number
                    }
                    break;

                case ev_details.EVENT_TYPES.CAMERA_MOVEMENT:

                    GameObject ca = GameObject.Find("Main Camera");
                    ca.GetComponent<s_camera>().focus = false;

                    float spe = current_ev.float0; //SPEED
                    float s = 0;
                    float dista = Vector2.Distance(ca.transform.position, new Vector3(current_ev.pos.x, current_ev.pos.y));
                    while (Vector2.Distance(ca.transform.position, new Vector3(current_ev.pos.x, current_ev.pos.y))
                        > dista * 0.05f)
                    {
                        s += spe * 0.0001f;
                        ca.transform.position = Vector2.Lerp(ca.transform.position, new Vector2(current_ev.pos.x, current_ev.pos.y), s);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    break;

                case ev_details.EVENT_TYPES.CHECK_FLAG:
                    int integr = s_globals.GetGlobalFlag(current_ev.string0);

                    switch (current_ev.logic)
                    {
                        case ev_details.LOGIC_TYPE.ITEM_OWNED:
                            if (s_globals.CheckItem(new o_item(current_ev.string0, (o_item.ITEM_TYPE)current_ev.int0)))
                            {
                                ev_num = current_ev.int1 - 1;
                            }
                            else
                            {
                                if (current_ev.boolean)     //Does this have an "else if"?
                                    ev_num = current_ev.int2;      //Other Label to jump to
                            }
                            break;

                        case ev_details.LOGIC_TYPE.VAR_EQUAL:
                            if (integr== current_ev.int0)  //Check if it is equal to the value
                            {
                                ev_num = current_ev.int1 - 1;   //Label to jump to
                            }
                            else
                            {
                                if (current_ev.boolean)     //Does this have an "else if"?
                                {
                                    ev_num = current_ev.int2;      //Other Label to jump to
                                }
                            }
                            break;

                        case ev_details.LOGIC_TYPE.VAR_GREATER:
                            if (integr > current_ev.int0)  //Check if it is greater
                            {
                                ev_num = current_ev.int1;   //Label to jump to
                            }
                            else
                            {
                                if (current_ev.boolean)     //Does this have an "else if"?
                                {
                                    ev_num = current_ev.int2;      //Other Label to jump to
                                }
                            }
                            break;


                        case ev_details.LOGIC_TYPE.VAR_LESS:
                            if (integr < current_ev.int0)  //Check if it is less
                            {
                                ev_num = current_ev.int1;   //Label to jump to
                            }
                            else
                            {
                                if (current_ev.boolean)     //Does this have an "else if"?
                                {
                                    ev_num = current_ev.int2;      //Other Label to jump to
                                }
                            }
                            break;
                    }
                    if (integr == current_ev.int0)
                    {

                    }
                    break;

                case ev_details.EVENT_TYPES.UTILITY_FOCUS:
                    s_utility utility;
                    o_plcharacter player = GameObject.Find("Player").GetComponent<o_plcharacter>();

                    if (GetComponent<s_utility>() != null)
                    {
                        player.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_NOTHING;
                        utility = GetComponent<s_utility>();
                    }
                    else {
                        break;
                    }

                    utility.istriggered = true;
                    while (utility.istriggered)
                    {
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    break;


                case ev_details.EVENT_TYPES.SET_HEALTH:

                    o_character c = GameObject.Find(current_ev.string0).GetComponent<o_character>();
                    if (c != null)
                        c.health = current_ev.float0;

                    break;

                case ev_details.EVENT_TYPES.UTILITY_CHECK:

                    //Make this like the conditional statements where it checks if the utility is still active

                    break;

                case ev_details.EVENT_TYPES.UTILITY_INITIALIZE:
                    s_utility ut;

                    if (GetComponent<s_utility>() != null)
                    {
                        ut = GetComponent<s_utility>();
                        ut.istriggered = true;
                    }
                    else
                    {
                        break;
                    }

                    break;

                case ev_details.EVENT_TYPES.FADE:

                    Color col = new Color(current_ev.colour.a, current_ev.colour.g, current_ev.colour.b, current_ev.colour.a);
                    float t = 0;

                    while (fade.color != col)
                    {
                        t += Time.deltaTime;
                        fade.color = Color.Lerp(fade.color, col, t);
                        yield return new WaitForSeconds(0.01f);
                    }
                    break;

                case ev_details.EVENT_TYPES.WAIT:

                    yield return new WaitForSeconds(current_ev.float0);

                    break;

                case ev_details.EVENT_TYPES.OBJECT:

                    foreach (string str in current_ev.stringList)
                    {
                        GameObject o = GameObject.Find(str);
                        if (o == null)
                            continue;
                        s_object ob = o.GetComponent<s_object>();
                        if (ob == null)
                            continue;

                        if (current_ev.boolean)
                        {
                            ob.DespawnObject();
                        }
                        else
                        {
                            o.SetActive(true);
                        }
                    }
                    break;

                case ev_details.EVENT_TYPES.DISPLAY_CHARACTER_HEALTH:

                    bool check = current_ev.stringList.Length < 2;
                    if (current_ev.boolean) {

                        if (check)
                        {
                            if (current_ev.boolean)
                                s_gui.AddCharacter(GameObject.Find(current_ev.stringList[0]).GetComponent<o_character>(), false);
                            else
                                s_gui.AddCharacter(GameObject.Find(current_ev.stringList[0]).GetComponent<o_character>(), true);
                        }
                        else
                        {
                            List<o_character> cha = new List<o_character>();
                            foreach (string st in current_ev.stringList)
                            {
                                cha.Add(GameObject.Find(st).GetComponent<o_character>());
                            }
                            s_gui.AddCharacter(cha);
                        }

                    }
                    break;

                case ev_details.EVENT_TYPES.END_EVENT:

                    GameObject cam = GameObject.Find("Main Camera");
                    cam.GetComponent<s_camera>().focus = true;
                    if (selobj)
                    {
                        if (selobj.GetComponent<o_plcharacter>())
                        {
                            selobj.GetComponent<o_plcharacter>().control = true;
                        }
                    }
                    if (current_ev.boolean)
                    {
                        string[] cha = new string[characters.Length];

                    int ind = 0;
                    foreach (o_character chara in characters)
                    {
                        cha[ind] = chara.name;
                        ind++;
                    }
                    s_utility u = GetComponent<s_utility>();
                    u_boundary u_b = GetComponent<u_boundary>();
                        if (u != null)
                        {
                            string uti = u.GetType().ToString();
                            s_leveledit.LevEd.savedtriggerdatalist.Add(new s_map.s_trig(name,transform.position, Events.ev_Details, uti, u_b.bounds, cha, TRIGGER_T, collision.size, true));
                        }
                        else
                            s_leveledit.LevEd.savedtriggerdatalist.Add(new s_map.s_trig(name, transform.position, Events.ev_Details, TRIGGER_T, collision.size, true));
                    }
                    doingEvents = false;

                    Image sh1 = GameObject.Find("Shutter1").GetComponent<Image>();
                    Image sh2 = GameObject.Find("Shutter2").GetComponent<Image>();
                    for (int i = 0; i < 30; i++)
                    {
                        sh1.rectTransform.position += new Vector3(0, shutterdepth);
                        sh2.rectTransform.position += new Vector3(0, -shutterdepth);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    DespawnObject();
                    break;

                case ev_details.EVENT_TYPES.SET_FLAG:
                    s_globals.SetGlobalFlag(current_ev.string0, current_ev.int0);
                    break;

                case ev_details.EVENT_TYPES.CHOICE:
                    int choice = 0, finalchoice = -1;
                    print(choice);
                    while (finalchoice == -1)
                    {
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                            choice--;

                        if (Input.GetKeyDown(KeyCode.DownArrow))
                            choice++;

                        if (Input.GetKeyDown(KeyCode.Z))
                        {
                            print("Chosen");
                            finalchoice = choice;
                        }
                        Dialogue.textthing.text = "";
                        Dialogue.textthing.text += current_ev.string0 + "\n";
                        for (int i = 0; i < current_ev.stringList.Length; i++)
                        {
                            if(choice == i)
                                Dialogue.textthing.text += "-> ";

                            Dialogue.textthing.text += current_ev.stringList[i] + "\n";
                        }
                        print(choice);
                        choice = Mathf.Clamp(choice, 0, current_ev.stringList.Length - 1);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    Dialogue.textthing.text = "";
                    ev_num = current_ev.intList[finalchoice]-1;
                    break;
            }
            //yield return new WaitForSeconds(0.5f);
        }
        */

    }
}
