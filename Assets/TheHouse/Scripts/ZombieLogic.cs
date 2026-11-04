using UnityEngine;
using System.Collections;

//This script is used for the logic of zombie behavior for a particular type base on navigation agents.


public class ZombieLogic : MonoBehaviour {

    public UnityEngine.AI.NavMeshAgent agent;
    public GameObject[] target;
    public Animation anim;
    public bool HandItem = false;//Check hand items. By default all zombies has no hand items it's okay. If you need to distribute items in zombie hands, use this variable separately.
    public GameObject[] HandItems; //put the items here if using HandItem as true or false state separately
    public int itemID, IDPose,IDAttack; //Inspector Setup
    public bool Cap;
    public float life = 100;
    private float AnimSDeathTime = 0;

    //Waiting - variable using for resetting hit colliders on zombie hands. Plase note, all zombie hand colliders can work incorrect cause turning navigation works inaccurately, try to increase the colliders, or change their scales.
    //free - variable checks if zombie can moving
    //SledgeHammer - if zombie with sledgehammer in the hands
    //Epileptic - if zombie is Epileptic
    //Bottle - if zombie with bottle in the hand
    //Impact - state of sledgehammer impact
    //RandomMoves - CPU change between run,walk animations
    //BottleStanding - zombie with bottle in the hand and always standing at position
    //resetShake - shaking camera until BigZombie hit the ground
    //changeStand - changing animations and using for BigZombie and BottleStanding Zombie at position


    public bool free,firstAction, walkingVoice, Waiting, DeathCap,SledgeHammer, Epileptic,Bottle, ActivateActionAfterFirst, Impact, RandomMoves, BottleStanding, Stopped, changeStand,BigZombie, resetShake, Dead;
    public GameObject BottleMesh, LookHolder, HeadHolder;
    public GameObject SFX,ChannelSFX_1, ChannelSFX_2, MainCam, ThunderSource,EpicAxeSource;
    public AudioClip[] ZombieVoices;
    public AudioClip[] ZombieHurts;
    private float changeTimeVoices,randomTimeVoice, ChangePoseTime,distance;
    public GameObject[] KickColliders;//for activate triggers for checking if zombie kicked the player
    public AudioClip[] HitSwooshes;
    public AudioClip ZombieFoot,ZombieDead, SledgeImpact; //audiosounds
    private float footStepTime,sledgeAnimTime;
    public float ThrowBottleTime; //changing animations and using for BigZombie and BottleStanding Zombie at position
    public GameObject[] ZombieColliders;
    public Transform upperBody;
    private int DeathRandom = 0;

    void Start () {

        SFX = GameObject.Find("SFX");
        MainCam = GameObject.Find("Main Camera");
        Cap = true;

        Waiting = true;
        randomTimeVoice = Random.Range(2,5);
        if (!BottleStanding && !BigZombie)
        target = GameObject.FindGameObjectsWithTag("Player");

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        anim = GetComponent<Animation>();

        if (BottleStanding && !Stopped)
        {
            anim["AttackBottle"].layer = 5;
            anim["AttackBottle"].AddMixingTransform(upperBody); // Adds a mixing transform using the bone transform defined above
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Axe") || other.CompareTag("Rocket"))
        {
            life = 0;
        }
    }

    void Update() {

        if (firstAction)
        {
            SFX.GetComponent<AudioSource>().Play();
            StartCoroutine(PlayAction());
            agent.enabled = true;
            firstAction = false;
            GetComponent<AudioSource>().clip = ZombieVoices[0];//RoarVoice
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().volume = .75f;
            GetComponent<AudioSource>().maxDistance = 50;
            GetComponent<AudioSource>().loop = false;
            GetComponent<AudioSource>().Play();
            ThunderSource.GetComponent<AudioSource>().Play();
            GetComponent<ZombieLife>().FirstAction = firstAction;
            ActivateActionAfterFirst = true;
        }

        if (life <= 0)
            if (Cap)
            {
                if (!DeathCap)
                {
                    free = false;
                    Dead = true;


                    GetComponent<ZombieLife>().enabled = false;
                    KickColliders[0].SetActive(false);
                    KickColliders[1].SetActive(false);

                    for (int i = 0; i < ZombieColliders.Length; i++)
                    {
                        ZombieColliders[i].SetActive(false);
                    }

                    GetComponent<AudioSource>().Stop();
                    GetComponent<AudioSource>().loop = false;
                    GetComponent<AudioSource>().clip = ZombieDead;
                    GetComponent<AudioSource>().Play();

                    DeathRandom = Random.Range(0,3);

                    if (!BigZombie)
                    {
                        if (DeathRandom == 0)
                            anim.CrossFade("Death01");
                        if (DeathRandom == 1)
                            anim.CrossFade("Death22");
                        if (DeathRandom == 2)
                            anim.CrossFade("Death11");
                    }

                    if (BigZombie)
                        anim.CrossFade("Death01");

                    agent.speed = 0;
                    MainCam.GetComponent<PlayerControl>().Kills += 1;
                    if (MainCam.GetComponent<PlayerControl>().ActionNum == 0 && MainCam.GetComponent<PlayerControl>().Kills == 1)
                    {
                        MainCam.GetComponent<PlayerControl>().idx_mus = 0;
                        MainCam.GetComponent<PlayerControl>().changeMusic = true;
                    }

                    if (MainCam.GetComponent<PlayerControl>().ActionNum == 1 && MainCam.GetComponent<PlayerControl>().Kills == 1)
                    {
                        MainCam.GetComponent<PlayerControl>().Kills = 0;
                        MainCam.GetComponent<PlayerControl>().ActionNum = 2;
                        MainCam.GetComponent<PlayerControl>().nextAction = true;
                    }

                    if (MainCam.GetComponent<PlayerControl>().ActionNum == 2 && MainCam.GetComponent<PlayerControl>().Kills == 6)
                    {
                        
                        MainCam.GetComponent<PlayerControl>().idx_mus = 0;
                        MainCam.GetComponent<PlayerControl>().changeMusic = true;
                        MainCam.GetComponent<PlayerControl>().Kills = 0;
                        MainCam.GetComponent<PlayerControl>().ActionNum = 3;
                        MainCam.GetComponent<PlayerControl>().nextAction = true;
                    }

                    if (MainCam.GetComponent<PlayerControl>().ActionNum == 3 && MainCam.GetComponent<PlayerControl>().Kills == 6)
                    {

                        MainCam.GetComponent<PlayerControl>().idx_mus = 0;
                        MainCam.GetComponent<PlayerControl>().changeMusic = true;
                        MainCam.GetComponent<PlayerControl>().Kills = 0;
                        MainCam.GetComponent<PlayerControl>().ActionNum = 3;
                    }

                    if (ActivateActionAfterFirst)
                    {
                        MainCam.GetComponent<PlayerControl>().Kills = 0;
                        MainCam.GetComponent<PlayerControl>().ActionNum = 1;
                        MainCam.GetComponent<PlayerControl>().nextAction = true;
                        ActivateActionAfterFirst = false;
                    }
                    DeathCap = true;
                }

                if (AnimSDeathTime <= 5) //wait for destroy zombie
                    AnimSDeathTime += Time.deltaTime;
                else
                    DestroyImmediate(gameObject);
            }

        if (free)
        {

            if (walkingVoice)
            {
                changeTimeVoices += Time.deltaTime;

                if (changeTimeVoices >= randomTimeVoice)
                {
                    GetComponent<AudioSource>().clip = ZombieVoices[Random.Range(1, ZombieVoices.Length)];
                    GetComponent<AudioSource>().loop = true;
                    GetComponent<AudioSource>().volume = 0.45f;

                    GetComponent<AudioSource>().Play();
                    randomTimeVoice = Random.Range(2, 5);
                    changeTimeVoices = 0;
                }
            }

            if (!Stopped)
            {
                agent.SetDestination(target[0].transform.position);

                distance = Vector3.Distance(target[0].transform.position, transform.position);

            if (!HandItem)
            {
                if (RandomMoves && !BottleStanding)
                {
                    if (Epileptic)
                    {
                        anim["Hit"].layer = 5;
                        anim["Hit"].AddMixingTransform(upperBody); // Adds a mixing transform using the bone transform defined above
                        anim.Blend("Hit");
                    }

                    ChangePoseTime += Time.deltaTime;

                    if (ChangePoseTime >= Random.Range(2, 5))
                    {
                        IDPose = Random.Range(0, 2);
                        ChangePoseTime = 0;
                    }

                    if (distance <= 5f)
                    {
                        if (Cap)
                        {

                            if (Waiting)
                            {
                                StartCoroutine(ResetHitColliders());
                                StartCoroutine(SwooshSounds());
                                Waiting = false;
                            }

                            if (!SledgeHammer && !Bottle && IDPose != 2)
                            {

                                if (IDAttack == 0)
                                {
                                    agent.speed = .5f;
                                    anim.CrossFade("Attack");
                                }
                                if (IDAttack == 1)
                                {
                                    agent.speed = .5f;
                                    anim.CrossFade("Kick");
                                }
                            }

                            if (!SledgeHammer && Bottle && IDPose != 2)
                            {

                                if (IDAttack == 0)
                                {
                                    agent.speed = .5f;
                                    anim.CrossFade("AttackBottle");
                                }
                                if (IDAttack == 1)
                                {
                                    agent.speed = .5f;
                                    anim.CrossFade("Kick");
                                }
                            }

                            if (SledgeHammer && !Bottle)
                            {
                                agent.speed = .5f;
                                sledgeAnimTime = anim["AttackAxe"].length;

                                if (anim["AttackAxe"].time >= sledgeAnimTime - .7f)
                                {
                                    if (!Impact)
                                    {
                                        SFX.GetComponent<AudioSource>().PlayOneShot(SledgeImpact, 1.5f);
                                        GetComponent<CameraShake>().Shaking = true;
                                        Impact = true;
                                    }
                                }
                                if (anim["AttackAxe"].time >= sledgeAnimTime)
                                {
                                    Impact = false;
                                    anim["AttackAxe"].time = 0;
                                }
                                anim.CrossFade("AttackAxe");
                            }
                        }


                    }
                    else
                    {

                        if (IDPose == 0)
                        {

                            footStepTime += Time.deltaTime;
                            if (footStepTime >= .7f)
                            {
                                GetComponent<AudioSource>().PlayOneShot(ZombieFoot, 1);
                                footStepTime = 0;
                            }
                            walkingVoice = true;
                            if (!SledgeHammer && !Bottle)
                                anim.CrossFade("Walk");
                            if (SledgeHammer && !Bottle)
                                anim.CrossFade("WalkAxe");
                            if (!SledgeHammer && Bottle)
                                anim.CrossFade("WalkBottle");

                            agent.speed = 1.5f;
                            agent.acceleration = 8;
                        }

                        if (IDPose == 1)
                        {
                            if (!SledgeHammer && !Bottle)
                            {
                                footStepTime += Time.deltaTime;
                                if (footStepTime >= .3f)
                                {
                                    GetComponent<AudioSource>().PlayOneShot(ZombieFoot, 1);
                                    footStepTime = 0;
                                }

                                agent.speed = 6;
                                agent.acceleration = 10;
                                anim.CrossFade("Run");
                            }

                            if (!SledgeHammer && Bottle)
                            {
                                footStepTime += Time.deltaTime;
                                if (footStepTime >= .3f)
                                {
                                    GetComponent<AudioSource>().PlayOneShot(ZombieFoot, 1);
                                    footStepTime = 0;
                                }

                                agent.speed = 6;
                                agent.acceleration = 10;
                                anim.CrossFade("RunBottle");
                            }

                            if (SledgeHammer && !Bottle)
                            {
                                footStepTime += Time.deltaTime;
                                if (footStepTime >= .7f)
                                {
                                    GetComponent<AudioSource>().PlayOneShot(ZombieFoot, 1);
                                    footStepTime = 0;
                                }

                                agent.speed = 5f;
                                agent.acceleration = 10;
                                anim.CrossFade("WalkAxe");
                            }
                        }

                    }

                }

                if (BottleStanding && !Stopped && !BigZombie)
                {
                    if (distance >= 0)
                    {
                        footStepTime += Time.deltaTime;
                        ThrowBottleTime += Time.deltaTime;


                        if (footStepTime >= .3f)
                        {
                            GetComponent<AudioSource>().PlayOneShot(ZombieFoot, 1);
                            footStepTime = 0;
                        }

                        if (ThrowBottleTime >= 2)
                        {
                            BottleMesh.SetActive(true);
                            ThrowBottleTime = 0;

                        }

                        if (ThrowBottleTime >= 1)
                        {

                            anim.CrossFade("AttackBottle");

                        }

                        if (ThrowBottleTime >= 1.7f)
                            BottleMesh.SetActive(false);

                        if (ThrowBottleTime <= 0)
                        {
                            anim.CrossFade("Run");

                        }

                        agent.speed = 6;
                        agent.acceleration = 10;
                        anim.CrossFade("Run");
                    }
                }
            }
        }

            if (BottleStanding && Stopped && !BigZombie)
            {

                agent.enabled = false;

                if (distance >= 0)
                {
                    ThrowBottleTime += Time.deltaTime;

                    if (ThrowBottleTime >= 5)
                    {
                        if (changeStand)
                        {
                            anim.CrossFade("AttackBottle");

                            changeStand = false;
                        }
                    }

                    if (ThrowBottleTime >= 5.2f)
                    {
                        BottleMesh.SetActive(false);
                    }

                    if (ThrowBottleTime >= 5.6f)
                    {
                        ThrowBottleTime = 0;
                    }

                    if (ThrowBottleTime >= 3f)
                    {
                        BottleMesh.SetActive(true);
                    }

                    if (ThrowBottleTime <= 1)
                    {
                        if (!changeStand)
                        {
                            anim.CrossFade("Idle2");
                            changeStand = true;
                        }
                    }
                }


            }
        }

        if (BigZombie && Stopped && !Dead)
        {
            agent.enabled = false;
            anim["AttackAxe"].speed = 0.2f;
            anim["Idle2"].speed = 0.2f;

            ThrowBottleTime += Time.deltaTime;

                if (ThrowBottleTime >= 5)
                {
                    if (changeStand)
                    {

                        GetComponent<AudioSource>().PlayOneShot(ZombieVoices[Random.Range(0, ZombieVoices.Length)],1);
                        EpicAxeSource.GetComponent<AudioSource>().Play();
                        anim.CrossFade("AttackAxe");

                        changeStand = false;
                    }
                }

            if (ThrowBottleTime >= 8)
            {
                if (resetShake)
                {
                    GetComponent<CameraShake>().SetDensity = 0.2f;
                    GetComponent<CameraShake>().SetDecay = 0.01f;
                    GetComponent<CameraShake>().Shaking = true;
                    resetShake = false;
                }
            }


            if (ThrowBottleTime >= 10.9f)
                {
                    ThrowBottleTime = 0;
                }


                if (ThrowBottleTime <= 1)
                {
                    if (!changeStand)
                    {
                        anim.CrossFade("Idle2");
                        changeStand = true;
                    resetShake = true;
                    }
                }
        }

        if (BigZombie && !Stopped)
        {

            footStepTime += Time.deltaTime;
            if (footStepTime >= 2.8f)
            {
                GetComponent<AudioSource>().Play();
                GetComponent<CameraShake>().Shaking = true;
                footStepTime = 0;
            }

            if (SledgeHammer && !Bottle)
                anim.CrossFade("WalkAxe");

            anim["WalkAxe"].speed = 0.25f;
            agent.speed = 10f;
            agent.acceleration = 8;

            if (distance <= 45)
            {
                LookHolder.GetComponent<SmoothLookAT>().enabled = true;
                HeadHolder.GetComponent<SmoothLookAT>().enabled = true;
                Stopped = true;
            }
        }

    }

        IEnumerator HideBottleMesh()
    {
        yield return new WaitForSeconds(.7f);
        BottleMesh.SetActive(false);
        yield return new WaitForSeconds(.5f);
        BottleMesh.SetActive(true);
    }

    IEnumerator SwooshSounds()
    {
        if (!SledgeHammer && !Bottle) {

            if (IDAttack == 0)
            {
                GetComponent<AudioSource>().PlayOneShot(HitSwooshes[0], 1.5f);
                yield return new WaitForSeconds(.5f);
                ChannelSFX_1.GetComponent<AudioSource>().PlayOneShot(HitSwooshes[1], 1.5f);
                GetComponent<CameraShake>().Shaking = true;
            }

            if (IDAttack == 1)
            {
                ChannelSFX_2.GetComponent<AudioSource>().PlayOneShot(HitSwooshes[0], 1.5f);
                GetComponent<CameraShake>().Shaking = true;
            }

        }
        if (SledgeHammer && !Bottle)
        {
            yield return new WaitForSeconds(.4f);
            GetComponent<AudioSource>().PlayOneShot(HitSwooshes[0], 1.5f);
            yield return new WaitForSeconds(.2f);
            GetComponent<AudioSource>().PlayOneShot(HitSwooshes[1], 1.5f);
        }
        if (!SledgeHammer && Bottle)
        {
            ChannelSFX_2.GetComponent<AudioSource>().PlayOneShot(HitSwooshes[0], 1.5f);
            
        }
    }

    IEnumerator ResetHitColliders()
    {
        if (!SledgeHammer && !Bottle)
        {
            if (IDAttack == 0)
            {
                KickColliders[0].SetActive(true); //activate hand colliders
                KickColliders[1].SetActive(true);
                yield return new WaitForSeconds(1.1f);

                KickColliders[0].SetActive(false); //deactivate hand colliders
                KickColliders[1].SetActive(false);
                IDAttack = Random.Range(0, 2);
            }
            if (IDAttack == 1)
            {
                yield return new WaitForSeconds(1f);
                IDAttack = Random.Range(0, 2);
            }
            
            Waiting = true;
        }

        if (SledgeHammer && !Bottle)
        {
            KickColliders[0].SetActive(true); //activate hand colliders
            KickColliders[1].SetActive(true);
            yield return new WaitForSeconds(2.8f);
            KickColliders[0].SetActive(false); //deactivate hand colliders
            KickColliders[1].SetActive(false);
            Waiting = true;
        }

        if (!SledgeHammer && Bottle)
        {
            KickColliders[1].SetActive(true);
            yield return new WaitForSeconds(.5f);
            GetComponent<CameraShake>().Shaking = true;
            yield return new WaitForSeconds(.55f);
            
            KickColliders[0].SetActive(false); //deactivate hand colliders
            KickColliders[1].SetActive(false);
            Waiting = true;
        }
    }

    IEnumerator PlayAction()
    {
        GetComponent<AudioSource>().maxDistance = 25;
        yield return new WaitForSeconds(4);
        MainCam.GetComponent<PlayerControl>().idx_mus = 1;
        MainCam.GetComponent<PlayerControl>().changeMusic = true;
    }
}
