using UnityEngine;
using System.Collections;

//Simple footsteps controller. Playing footstep sounds in the specific areas. This script attached to "FootstepZone" gameObjects in the hierarchy

public class FootStepControls : MonoBehaviour {

    public int FloorZoneType = 0;//default footstep sound carpet
    private GameObject MainCam;

    void Start()
    {
        MainCam = GameObject.Find("Main Camera"); // The PlayerControl script attached to the Main Camera.
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            MainCam.GetComponent<PlayerControl>().FloorZoneType = FloorZoneType; //send FloorZoneType to the PlayerControl script
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            MainCam.GetComponent<PlayerControl>().FloorZoneType = FloorZoneType;
    }

}
