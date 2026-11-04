using UnityEngine;
using System.Collections;

//Turn off this script from main camera if you're turned VR mode
//rotates Main Camera based on mouse movements. Placed on Main Camera gameObject in the inspector

public class CameraRotation : MonoBehaviour
{

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    public float yaw = 0.0f;
    public float pitch = 0.0f;



    void Start()
    {
        transform.eulerAngles = new Vector3(0, 0, 0.0f);

    }

    void LateUpdate()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -20, 30);
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

}
