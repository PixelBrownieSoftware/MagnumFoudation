using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MagnumFoudation
{
    [Serializable]
    public class s_anim
    {
        public string name;
        public anim_keyframe[] keyframes;
    }
    [Serializable]
    public struct anim_keyframe
    {
        public float duration;
        public Sprite spr;
    }

    public class s_animhandler : MonoBehaviour
    {
        public string[] animnames;
        public List<s_anim> animations;
        float timer = 0;
        public int animindex = 0;
        s_anim current;
        s_anim lastCurrent;

        public float _speed = 1; //Default is 1

        public Sprite current_sprite { get; set; }
        SpriteRenderer sprRend;
        public bool isLooping = false;

        void Start()
        {
            sprRend = GetComponent<SpriteRenderer>();
        }
        public void SetAnimation(string anim, bool isloop, float speed)
        {
            _speed = speed;
            isLooping = isloop;
            current = animations.Find(x => x.name == anim);
            if (lastCurrent != current)
            {
                animindex = 0;
                lastCurrent = current;
            }
        }

        public void SetAnimation(string anim, bool isloop)
        {
            _speed = 1;
            isLooping = isloop;
            current = animations.Find(x=> x.name == anim);
            if (lastCurrent != current)
            {
                animindex = 0;
                lastCurrent = current;
            }
        }

        public void Update()
        {
            if (!s_globals.isPause)
            {
                if (current != null)
                {
                    current_sprite = current.keyframes[animindex].spr;
                    if (sprRend != null)
                        sprRend.sprite = current_sprite;
                    if (current.keyframes[animindex].duration * _speed < timer)
                    {
                        if (isLooping)
                        {
                            if (animindex == current.keyframes.Length - 1)
                                animindex = 0;
                            else
                                animindex++;
                        }
                        else
                        {
                            if (animindex != current.keyframes.Length - 1)
                                animindex++;
                        }
                        timer = 0;
                    }
                    timer += Time.deltaTime;
                }
                else
                {
                    timer = 0;
                }
            }
        }
    }
}
