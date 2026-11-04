using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public float speed = 50.0F;
    public Transform Player;

    public GameObject FlashLight; //flashlight gameObject placed on Player 
    public AudioClip[] Footsteps; //array of sounds
    public float audioTime = 0; //footstep repeat audio time
    public int FloorZoneType = 0; //0-carpet,1-parquet,2 - trash, 3-water gets info from trigger 
    public Transform AnimatedObject;
    public bool run, fl, changeMusic, reload, DontMove, PlayerKick, nextAction; //Caps
    public AudioClip[] MusicBackgrounds; //musics
    public AudioClip PlayerKickSound; //hit sound
    public GameObject MainBackgroundAudio, HitEffectObject, DoorHolder, CapBoardHolder, Copter; //gameobjects are using for actions,
    public int idx_mus, ActionNum, Kills, stateAnim; //idx_mus - current music playing | ActionNum - number of action | Kills - number of zombies kills of current action. Condition to start the next action.
                                                     //stateAnim - current animation state and ComboTest script gets this value from PlayerControl script
    public GameObject[] ZombieGroupSelection;//What Zombie gameobjects will participate in the action?
    public GameObject[] ActionBottles; //using for bottle action


    void Update()
    {

        if (reload)
        {
            DontMove = true;
            AnimatedObject.gameObject.GetComponent<Animation>().Stop();
            AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("ReloadingM4");
            StartCoroutine(Reloaded());
            reload = false;
        }

        if (nextAction)
        {
                StartCoroutine(PlayAction(ActionNum));
            nextAction = false;
        }

        if (PlayerKick)
        {
            MainBackgroundAudio.GetComponent<AudioSource>().PlayOneShot(PlayerKickSound, .5f);
            HitEffectObject.GetComponent<Animation>().Play();
            PlayerKick = false;
        }

        if (changeMusic)
        {
            MainBackgroundAudio.GetComponent<AudioSource>().Stop();
            MainBackgroundAudio.GetComponent<AudioSource>().clip = MusicBackgrounds[idx_mus];
            MainBackgroundAudio.GetComponent<AudioSource>().Play();
            changeMusic = false;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {

            fl = !fl;

            if (!fl)
                FlashLight.SetActive(false);

            if (fl)
                FlashLight.SetActive(true);
        }


        if (!DontMove) { 

            Player.transform.position += AnimatedObject.transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        Player.transform.position += AnimatedObject.transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;

            if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0)
            {
                if (Input.GetKey(KeyCode.LeftShift)) run = true;
                if (Input.GetKeyUp(KeyCode.LeftShift)) run = false;

                if (!run)
                {
                    stateAnim = 1;
                    speed = 1;

                    if (Input.GetAxis("Horizontal") > 0)
                    {
                        AnimatedObject.gameObject.GetComponent<Animation>()["Walk"].speed = 1;
                        AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("Walk");
                    }

                    if (Input.GetAxis("Horizontal") < 0)
                    {
                        AnimatedObject.gameObject.GetComponent<Animation>()["Walk"].speed = -1;
                        AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("Walk");
                    }

                    if (Input.GetAxis("Vertical") > 0)
                    {
                        AnimatedObject.gameObject.GetComponent<Animation>()["Walk"].speed = 1;
                        AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("Walk");
                    }

                    if (Input.GetAxis("Vertical") < 0)
                    {
                        AnimatedObject.gameObject.GetComponent<Animation>()["Walk"].speed = -1;
                        AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("Walk");
                    }

                    audioTime += Time.deltaTime;
                    if (audioTime >= .65f)
                    {
                        audioTime = 0;
                        if (FloorZoneType == 0)
                            gameObject.GetComponent<AudioSource>().PlayOneShot(Footsteps[0], 0.7F);
                        if (FloorZoneType == 1)
                            gameObject.GetComponent<AudioSource>().PlayOneShot(Footsteps[1], 0.7F);
                        if (FloorZoneType == 2)
                            gameObject.GetComponent<AudioSource>().PlayOneShot(Footsteps[2], 0.7F);
                        if (FloorZoneType == 3)
                            gameObject.GetComponent<AudioSource>().PlayOneShot(Footsteps[3], 0.7F);
                    }


                }

                if (run)
                {
                    stateAnim = 2;
                    speed = 5;

                    if (Input.GetAxis("Horizontal") > 0)
                    {
                        AnimatedObject.gameObject.GetComponent<Animation>()["Run"].speed = 1;
                        AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("Run");
                    }

                    if (Input.GetAxis("Horizontal") < 0)
                    {
                        AnimatedObject.gameObject.GetComponent<Animation>()["Run"].speed = -1;
                        AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("Run");
                    }

                    if (Input.GetAxis("Vertical") > 0)
                    {
                        AnimatedObject.gameObject.GetComponent<Animation>()["Run"].speed = 1;
                        AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("Run");
                    }

                    if (Input.GetAxis("Vertical") < 0)
                    {
                        AnimatedObject.gameObject.GetComponent<Animation>()["Run"].speed = -1;
                        AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("Run");
                    }

                    audioTime += Time.deltaTime;
                    if (audioTime >= .29f)
                    {
                        audioTime = 0;
                        if (FloorZoneType == 0)
                            gameObject.GetComponent<AudioSource>().PlayOneShot(Footsteps[0], 0.7F);
                        if (FloorZoneType == 1)
                            gameObject.GetComponent<AudioSource>().PlayOneShot(Footsteps[1], 0.7F);
                        if (FloorZoneType == 2)
                            gameObject.GetComponent<AudioSource>().PlayOneShot(Footsteps[2], 0.7F);
                        if (FloorZoneType == 3)
                            gameObject.GetComponent<AudioSource>().PlayOneShot(Footsteps[3], 0.7F);
                    }
                }

            }
            else
            {
                stateAnim = 0;
                AnimatedObject.gameObject.GetComponent<Animation>().CrossFade("Idle");
            }
        }      
}

    IEnumerator Reloaded()
    {
        yield return new WaitForSeconds(1.8f);
        Player.GetComponent<Shooting>().BulletsCount += 100; //sending to Shooting script 100 bullets after animation played
        Player.GetComponent<Shooting>().capacity -= Player.GetComponent<Shooting>().BulletsCount; //reducing capacity
        DontMove = false;
    }

    IEnumerator PlayAction(int ActionNum)
    {
        yield return new WaitForSeconds(5);
        {
            if (ActionNum == 1)
            {
                yield return new WaitForSeconds(2);
                ZombieGroupSelection[0].SetActive(true);
                DoorHolder.GetComponent<Animation>().Play();
            }

            if (ActionNum == 2)
            {
                ActionBottles[1].GetComponent<Animator>().enabled = true;
                for (int i=2;i< ActionBottles.Length; i++)
                {
                    ActionBottles[i].SetActive(true);
                }
                yield return new WaitForSeconds(4);
                ActionBottles[0].SetActive(true);
                Destroy(ActionBottles[0],9);
                idx_mus = 1;
                changeMusic = true;
                yield return new WaitForSeconds(3);
                CapBoardHolder.GetComponent<Animation>().Play();

                yield return new WaitForSeconds(3);
                ZombieGroupSelection[1].SetActive(true);

                yield return new WaitForSeconds(5);
                for (int i=2;i<7;i++)
                    ZombieGroupSelection[i].SetActive(true);
            }

            if (ActionNum == 3)
            {
                yield return new WaitForSeconds(2);
                ZombieGroupSelection[7].SetActive(true);

                yield return new WaitForSeconds(5);
                idx_mus = 1;
                changeMusic = true;

                yield return new WaitForSeconds(7);
                for (int i = 8; i < ZombieGroupSelection.Length; i++)
                    ZombieGroupSelection[i].SetActive(true);

                yield return new WaitForSeconds(7);
                Copter.SetActive(true);


            }
        }
    }
}