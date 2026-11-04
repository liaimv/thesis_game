using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to control the rain in front of the camera. Attached to the triggers called "RainCameraTriggers" and "RainCameraTriggersIndoorView".
//Shows rain particle in front of camera and turns the properties of physical material into wet or dry on Player character

public class RainCameraTrigger : MonoBehaviour {

    public GameObject CameraRain,PlayerMaterial;
    public bool outdoor; // true if outdoor rain triggers ("RainCameraTriggers" gameobjects in the hierarchy) | false if inside house rain triggers callled "RainCameraTriggersIndoorView" gameobjects

    void Start()
    {
 
        if (outdoor) //set this variable true in the inspector if using RainCameraTriggers.
        {

            CameraRain = GameObject.Find("Rain"); //Find "Rain" Gameobject placed on Main camera as a child gameobject in the hierarchy
            PlayerMaterial = GameObject.Find("RainMaterialParameterControl"); // "PlayerMaterial" variable it's a "RainMaterialParameterControl" gameObject placed on Main camera as a child gameobject in the hierarchy
            CameraRain.GetComponent<ParticleSystem>().Stop();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (!outdoor) //if Player collided with inside house trigger (RainCameraTriggersIndoorView)
            {
                CameraRain.GetComponent<ParticleSystem>().Play();
            }

            if (outdoor) //if Player collided with outdoor trigger (RainCameraTriggers)
            {
                CameraRain.GetComponent<ParticleSystem>().Play();
                PlayerMaterial.GetComponent<RainMaterialParameter>().Rainy = true; //turns the properties of physical material into wet
            }
        }

        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!outdoor)
            {
                CameraRain.GetComponent<ParticleSystem>().Stop();
            }

            if (outdoor)
            {
                CameraRain.GetComponent<ParticleSystem>().Stop();
                PlayerMaterial.GetComponent<RainMaterialParameter>().Rainy = false;//turns the properties of physical material into dry
            }
        }

        
    }
}
