using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//General destoying objects. Must be placed on any object in the inspector for destroying.

public class Destroy : MonoBehaviour {

    public float TimeDestroy;

	void Start () {
        Destroy(gameObject, TimeDestroy);
	}

}
