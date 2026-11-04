using UnityEngine;
using System.Collections;

//Shake effect. This script placed on Zombie gameObject in the inspector. Used when an action has occurred (Attack, Kick, Axe, Bottle)

public class CameraShake : MonoBehaviour
{
    public bool Shaking;
    public float ShakeDecay,SetDecay, ShakeIntensity, SetDensity; //you should set only SetDecay and SetDensity in the inspector for shaking intensity.

    public Vector3 OriginalPos;
    public Quaternion OriginalRot;
    public Transform Cam;

    void Start()
    {
        Cam = GameObject.Find("Main Camera").transform;
    }


    // Update is called once per frame
    void LateUpdate()
    {

        if (ShakeIntensity > 0)
        {
            Cam.gameObject.GetComponent<CameraRotation>().enabled = false; //we need turn off any scripts using for camera Rotations before shaking action

            Cam.transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                            OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                            OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                            OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f);

            ShakeIntensity -= ShakeDecay;
        }
        else if (Shaking) //Shaking variable using as Cap cause we must update shake every frame.
        {
            
            DoShake(); //let's shake
            Shaking = false;
        }
        if (ShakeIntensity <= 0)
        {
            Cam.gameObject.GetComponent<CameraRotation>().enabled = true; //shakes ended and enable our camera rotations by mouse movement
        }
    }

    public void DoShake()
    {
        OriginalPos = Cam.transform.position; //current position of Main Camera
        OriginalRot = Cam.transform.rotation; //current rotation of Main Camera

        ShakeIntensity = SetDensity;
        ShakeDecay = SetDecay;
        Shaking = true;
    }
}