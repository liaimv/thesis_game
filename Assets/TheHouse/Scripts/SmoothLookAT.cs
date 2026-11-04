using UnityEngine;
using System.Collections;

//Looking script with a given type

public class SmoothLookAT : MonoBehaviour {

    public GameObject targetObj;
    public int speed = 5;
    public bool BigZombie,copter; //"copter" variable using for copter look and you should attach the Head Gameobject to CopterLookHolder Gameobject from the hierarchy to the inspector | BigZombie - for big zombie look
    public int ch_idx = 0;
    public string GO_Name = "Player";

	void Start () {

        if (!copter)
        targetObj = GameObject.Find(GO_Name); //find gameobjects with wanted names

        }
	
	void FixedUpdate () {

        if (BigZombie) //only if BigZombie look (Head bone)
        {
            var targetRotation_1 = Quaternion.LookRotation(targetObj.transform.position - transform.position);
            targetRotation_1 *= Quaternion.Euler(0, -15.05f, 0);
            targetRotation_1.x = 0;
            targetRotation_1.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation_1, speed * Time.deltaTime);
        }
        else //any objects look
        {
            if (targetObj == null)
                return;
            var targetRotation_1 = Quaternion.LookRotation(targetObj.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation_1, speed * Time.deltaTime);
        }

    }
}