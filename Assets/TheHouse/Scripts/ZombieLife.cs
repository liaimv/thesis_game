using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Provide zombie hit reaction
//Interacts with the ZombieLogic and Shooting scripts
//Using on Zombie hit collisions and Zombie gameobject interacts with each other.

public class ZombieLife : MonoBehaviour {

    public bool receive,endShoot,collisions, FirstAction,SledgeHammer, BigZombie, cap; //collisions is true when hit only collisions of zombie (for example "ZombieTargetColliders" in the hierarchy) and false if as holder component ("Zombie" gameobject in the hierarchy)
    public Animation anim;
    public float AnimLen,currTime, bullets, resetPoseTime;
    public GameObject Zombie;
    
    void Start () {

        if (!collisions)
        {
            anim = GetComponent<Animation>();
            AnimLen = anim["Hit"].length;
            
        }
    }
	
	void Update () {

            if (!collisions && !SledgeHammer) //if zombie with no sledgehammer or axe and script attached as holder component on "Zombie" gameobject in the hierarchy.
        {
            currTime = anim["Hit"].normalizedTime * anim["Hit"].clip.length;

            if (receive) //If the player is hit with raycast then receive boolean varialble "receive" as one frame cap from the Shooting script
            {
                if (FirstAction) //check if cuurent action is First Action 
                {
                    GetComponent<ZombieLogic>().firstAction = true; // send this value to the ZombieLogic script
                    
                    FirstAction = false;
                }
                
                endShoot = false;

                if (currTime < AnimLen) //when player hit then we need stop zombie
                {
                    anim["Hit"].normalizedTime = currTime;
                    anim["Hit"].normalizedTime += Random.Range(.1f,.5f);
                    
                    GetComponent<ZombieLogic>().free = false; //set free as false it means dont move
                    GetComponent<ZombieLogic>().agent.enabled = false; //disabling navmesh agent
                    anim.CrossFade("Hit");//play Hit animation
                }

                if (currTime > AnimLen)
                {
                    anim["Hit"].normalizedTime = .3f;
                    anim.CrossFade("Hit");

                }
                receive = false;
                endShoot = true;
            }

            if (!receive && endShoot) //if player don't hit and doesn't shoot
            {

                if (!anim.IsPlaying("Hit"))
                    endShoot = false;//enough playing Hit animation
            }

            if (!receive && !endShoot && !FirstAction) 
            {
                GetComponent<ZombieLogic>().agent.enabled = true; //enable navmesh agent
                GetComponent<ZombieLogic>().free = true; //set zombie free
            }
        }
    }
}
