using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script generates a new object after a specified time, and gives it an ejection force of bottle (BottleSpawner)
//First 4 gameobjects in the hierarchy called BottleSpawner using this script when the player moves to the second action scenario with Bottles. The bottles are flying into the house and exploding.

public class BottleSpawner : MonoBehaviour {

    public Transform BottlePref; //our bottle prefab

    public bool OnZombie, Spawn; //if OnZombie? This variable using for zombie object and the BottleSpawner must be on zombie object in hierarchy. 
                                 //In fact, when a zombie drops a bottle, he does not throw it, and the bottle in his hand becomes inactive, and at this time, the object BottleSpawner is spawning and eject the bottle.
                                 //if Spawn? then BottleSpawner can be placed anywhere in the hierarchy

    public float timeSpawn, MaxTimeSpawn,speed; //time after bottle will spawn, MaxTimeSpawn can be set in the hierarchy, speed - ejection force
    private bool spawned; //check if spawned

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!spawned && !OnZombie)
        {
            timeSpawn += Time.deltaTime;

            if (timeSpawn >= MaxTimeSpawn)
            {
                Transform clone = Instantiate(BottlePref, transform.position, transform.rotation) as Transform;
                clone.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
                timeSpawn = 0;
                spawned = true;
                Destroy(gameObject);
            }
        }

        if (OnZombie)
        {
            timeSpawn += Time.deltaTime;

            if (timeSpawn >= MaxTimeSpawn)
            {
                speed = Random.Range(2700,3500);
                Transform clone = Instantiate(BottlePref, transform.position, transform.rotation) as Transform;
                clone.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
                timeSpawn = 0;
            }

        }

    }
}
