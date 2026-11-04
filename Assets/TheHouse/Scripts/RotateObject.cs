using UnityEngine;
using System.Collections;

//Rotating any objects for example blades of copter or bottles

public class RotateObject : MonoBehaviour {

    public bool rot_x,rot_y,rot_z,rand;
    public float speed = 0;
    public float speed_X, speed_Y, speed_Z,timeRand;

    void Start () {
	}

    void Update() {

        if (rand) { //randomize

        timeRand += Time.deltaTime;

        if (timeRand>=4)
            {
                speed_X = Random.Range(1,50);
                speed_Y = Random.Range(1, 100);
                speed_Z = Random.Range(1, 1000);
                timeRand = 0;
            }

            }

        if (rot_y)
        {
            if (!rand) //set in inspector
                transform.Rotate(0, speed * Time.deltaTime, 0);
            
                if (rand)
                transform.Rotate(0, speed_Y * Time.deltaTime, 0);
        }

        if (rot_x)
        {
            if (!rand)
                transform.Rotate(speed * Time.deltaTime, 0, 0);
            
                if (rand)
                transform.Rotate(speed_X * Time.deltaTime, 0, 0);
        }
        if (rot_z)
        {
            if (!rand)
                transform.Rotate(0, 0, speed * Time.deltaTime);
            
                if (rand)
                transform.Rotate(0, 0, speed_Z * Time.deltaTime);
        }

    }
}
