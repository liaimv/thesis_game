using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is designed to control the remaining bullets, shots and hits at enemies. It contains variables for controlling particles, flickering light, flashing.
//This script is always accessed by a PlayerControl script.

public class Shooting : MonoBehaviour {

    public GameObject Light, PlayerAnim;
    public ParticleSystem Flashes;
    public float TimingQueue,MaxTiming,LightIntensity, LitQueue, DelayHurt;
    public bool charging, Shoot,reloading;
    public Light Lit;
    public Transform Spawner, ExploBlood;
    public AudioClip ShootSound,ReloadingSound;
    public AudioClip[] BulletsFlyBy, EnoughSound;
    public AudioClip[] BloodHits;
    public int BulletsCount,capacity;

    void Start () {
        BulletsCount = 100;
        Lit = Light.GetComponent<Light>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Axe")
        {

            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>()); //ignoring Player collision with a sphere collision placed in the BigZombie - Sledgehammer - Sphere with "Axe" tag to avoid pushing beyond the level
        }
    }

    void FixedUpdate()
    {
        Vector3 fwd = Spawner.transform.TransformDirection(Vector3.forward); //Spawner placed in the hierarchy of the Player - M4_Walk_Fwd - Spine1 - RightShoulder - RightHand - RightHandIndex1 - M4 - Clip - Spawner
        RaycastHit hit;

        if (Physics.Raycast(Spawner.transform.position, fwd, out hit) && Shoot)
        {

            if (hit.transform.tag == "Enemy")
            {
                Instantiate(ExploBlood, hit.point, Quaternion.identity); //instance BloodExplosion with automatically destoying script by time
                GetComponent<AudioSource>().PlayOneShot(BloodHits[Random.Range(0, BloodHits.Length)], .5f);
                DelayHurt += Time.deltaTime;
                if (DelayHurt >= 0.05f)
                {
                    if (!hit.transform.gameObject.GetComponent<ZombieLife>().Zombie.GetComponent<ZombieLogic>().BigZombie)
                    hit.transform.gameObject.GetComponent<ZombieLife>().Zombie.GetComponent<AudioSource>().PlayOneShot(hit.transform.gameObject.GetComponent<ZombieLife>().Zombie.GetComponent<ZombieLogic>().ZombieHurts[Random.Range(0, 2)], 1); //Play zombie hurts.
                    DelayHurt = 0;
                }
                hit.transform.gameObject.GetComponent<ZombieLife>().Zombie.GetComponent<ZombieLogic>().life -= 10;
                hit.transform.gameObject.GetComponent<ZombieLife>().Zombie.GetComponent<ZombieLife>().bullets += 1;
                hit.transform.gameObject.GetComponent<ZombieLife>().Zombie.GetComponent<ZombieLife>().receive = true; //we calling colliders first and they contains zombie parent gameobject with ZombieLife script and calling animations. One script for Zombie colliders and for Zombie as gameobject.
            }

            Shoot = false;
        }
    }


    void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            if (BulletsCount <= 0)
            {
                GetComponent<AudioSource>().PlayOneShot(EnoughSound[0], .7f); //if bullets count not enough then play enough sound

                reloading = true;
            }
        }

        if (reloading)
        {
            if (BulletsCount <= 0 && capacity>0)
            {
                PlayerAnim.GetComponent<PlayerControl>().reload = true;
                GetComponent<AudioSource>().PlayOneShot(ReloadingSound, 1);
            }
            reloading = false;
        }

        if (Input.GetMouseButton(0))
        {

                if (BulletsCount > 0)
            {
                
                charging = true;

            if (charging)
            {

                TimingQueue += Time.deltaTime;
                LitQueue += Time.deltaTime;
            }

            if (LitQueue >= 0.05f)
            {
                Lit.intensity = LightIntensity;

            }

            if (LitQueue >= 0.1f)
            {
                Lit.intensity = 0;
                LitQueue = 0;
            }

            if (TimingQueue >= MaxTiming)
            {
                Shoot = true;
                    if (BulletsCount >= 1)
                    {
                        GetComponent<AudioSource>().Play();
                        GetComponent<AudioSource>().PlayOneShot(BulletsFlyBy[Random.Range(0, BulletsFlyBy.Length)], .7f);
                    }
                    
                Flashes.GetComponent<ParticleSystem>().Play();
                    BulletsCount -= 1;

                    if (BulletsCount <= 0)
                    {
                        GetComponent<AudioSource>().PlayOneShot(EnoughSound[1], 1f);
                        Lit.intensity = 0;
                    }
                    charging = false;
                TimingQueue = 0;
            }

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (BulletsCount > 0)
            {
                Light.GetComponent<Light>().intensity = 0;
                TimingQueue = 0;
                Shoot = false;
                charging = false;
            }
        }

    }
}
