using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script are using on Bottles or can be use on any destructible objects as you wish.

public class BottleExplosion : MonoBehaviour {

    public Transform GlassPref; //our glass prefab

    void Start()
    {
        Destroy(gameObject,5); //destroy game object after 5 seconds if doesn't collided;
    }

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Explosions")) //set up "Explosions" tag in the inspector if empty
        {
            Instantiate(GlassPref, transform.position, transform.rotation); //spawn glass explosion prefab
            Destroy(gameObject); //destroy current bottle gameobject and will see instanced explosion prefab  
        }
    }
}
