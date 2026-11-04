using UnityEngine;
using System.Collections;

//This script is used on the bone of the main characters (Spine1)
//Allows the skeleton to rotate in the desired axes by motions (idle,walk,run)

public class ComboTest : MonoBehaviour {

    public GameObject Parent, MainCamera; //Parent is a Player gameObject in hierarchy and sets in the inspector | Main Camera placed on Player as child gameobject

    public Transform[] target; //You need to set 3 viewpoints placed on Main Camera in hierarchy

    public int state = 0; //get the value of the variable state from the script PlayerControl where lowerSpine (Spine1) will looking if state = 0 then it means the Player is currently stoped
                          //if state = 1 then the Player is currently walking and state = 2 then the Player running. Must be public.

    public Transform lowerSpine; //is a bone that will looking on viewpoint targets


    void LateUpdate()
    {

        state = MainCamera.GetComponent<PlayerControl>().stateAnim; //get the state number from the PlayerControl

        // set fast rotation
        float rotSpeed = 100f;

        // distance between target viewpoint and the actual rotating object/bone
        Vector3 D = target[state].position - Parent.transform.position;


        // calculate the Quaternion for the rotation
        Quaternion rot = Quaternion.Slerp(Parent.transform.rotation, Quaternion.LookRotation(D), rotSpeed * Time.deltaTime);

        //Apply the rotation 
        Parent.transform.rotation = rot;

        // put 0 on the axys you do not want for the rotation object to rotate. We rotate full Player gameObject placed on the hierarchy with all bones only Y axis by mouse horizontal movement
        Parent.transform.eulerAngles = new Vector3(0, Parent.transform.eulerAngles.y,0);

        //Now we set view target for our bone (Spine1). This bone rotates only by vertical mouse movement
        lowerSpine.transform.LookAt(target[state]);

        //converting angles for optimal looking
        lowerSpine.eulerAngles = new Vector3(target[state].eulerAngles.x, target[state].eulerAngles.y, target[state].eulerAngles.z);



    }

}
