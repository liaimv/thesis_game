using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The script must be on physical objects that will be destroyed. For example Angel statue on the cemetery.

public class DestructibleProps : MonoBehaviour {

    public bool Lights; //true if we destroy LargeLamp gameObject in the hierarchy, and false for other not emmissive and not light objects
    public GameObject Lamp,MeshEmmisive; //Lamp is a Light gameobject, MeshEmmisive is a mesh with emmissive color


    void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Axe"))
        //{
        //    //if (!Lights)
        //    //{
        //    //    GetComponent<BoxCollider>().isTrigger = false; //by default all destructible gameobjects are triggered and kinematic and if collided with Axe (BigZombie - Sledgehammer - (Sphere gameobject with sphere collider with "Axe" tag))
        //    //    GetComponent<Rigidbody>().isKinematic = false; //then we disable the kinematics, the object will react to collisions
        //    //}
        //    if (Lights)
        //    {
        //        Lamp.SetActive(false);
        //        MeshEmmisive.GetComponent<Renderer>().material.DisableKeyword("_EMISSION"); //important disable Emission keyword of standard material attached on MeshEmmisive gameObject
        //        GetComponent<BoxCollider>().isTrigger = false;
        //        GetComponent<Rigidbody>().isKinematic = false;
        //    }
        //}
    }
}
