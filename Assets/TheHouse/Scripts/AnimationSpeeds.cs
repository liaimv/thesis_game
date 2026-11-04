using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie animations speed controller. Used in the first frame, the optimal values are set. 
//The script must be attached to the any animated object, to control the playback speed of the specified animation.

public class AnimationSpeeds : MonoBehaviour {

    private Animation anim;

    void Start () {
        anim = GetComponent<Animation>();
        anim["Idle"].speed = .2f;
        anim["Idle2"].speed = .2f;
        anim["Hit"].speed = .7f;
        anim["Death01"].speed = .5f;
        anim["AttackAxe"].speed = .5f;
        anim["Death22"].speed = .5f;
    }

}
