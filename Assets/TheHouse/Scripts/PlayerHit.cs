using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If Zombie hits the Player

public class PlayerHit : MonoBehaviour {

    public GameObject MainCam;

    void Start()
    {
        MainCam = GameObject.Find("Main Camera");
    }

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            MainCam.GetComponent<PlayerControl>().PlayerKick = true; //send Player kicked to the Main Camera
        }
    }
}
