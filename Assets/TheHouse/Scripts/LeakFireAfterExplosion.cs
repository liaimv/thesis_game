using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script can be using with Leak fire effects but in order to optimization, we skip this function, and make it an alternative method of ordinary explosion. Use this script only to obtain an acceptable visual effect.

public class LeakFireAfterExplosion : MonoBehaviour {

    public Transform FirePref;
    public AudioClip[] ExploSounds;
    private int idSound;
    public bool SpawnLeakFire;

    // Use this for initialization
    void Start()
    {

        idSound = Random.Range(0, ExploSounds.Length);
        if (SpawnLeakFire)
            Instantiate(FirePref, transform.position, transform.rotation);
        GetComponent<AudioSource>().PlayOneShot(ExploSounds[idSound], 1);
        Destroy(gameObject, 1);

    }
}
