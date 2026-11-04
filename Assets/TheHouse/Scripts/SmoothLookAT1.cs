using UnityEngine;
using System.Collections;

//Additional looking script

public class SmoothLookAT1 : MonoBehaviour {

    public GameObject targetObj;
    public int speed = 5;
    public bool Free;
	
	void FixedUpdate () {

            if (!Free)
            {
                var targetRotation_1 = Quaternion.LookRotation(targetObj.transform.position - transform.position);
                targetRotation_1.x = 0;
                targetRotation_1.z = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation_1, speed * Time.deltaTime);
            }

            if (Free)
            {
                var targetRotation_1 = Quaternion.LookRotation(targetObj.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation_1, speed * Time.deltaTime);
            }

    }
}