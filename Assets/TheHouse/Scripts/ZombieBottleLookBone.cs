using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using on Spine bone when zombie running at start of second action (ZombieRunBottle gameobject in the hierarchy - Spine1)

public class ZombieBottleLookBone : MonoBehaviour {

    public Transform targetLook;
    public Transform lowerSpine;

    void LateUpdate()
    {

        lowerSpine.transform.LookAt(targetLook);

        lowerSpine.eulerAngles = new Vector3(targetLook.eulerAngles.x, targetLook.eulerAngles.y, targetLook.eulerAngles.z);

    }
}
