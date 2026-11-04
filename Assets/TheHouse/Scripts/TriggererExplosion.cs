using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The script is used for a shock wave from a huge sledgehammer of BigZombie. 
//The reaction of the blast wave occurs due to the collision of the sphere of Sledgehammer and the floor, which is under the terrain ("AxeExplosionField" gameobject in the hierarchy).

public class TriggererExplosion : MonoBehaviour {

    public Transform ExploPrefab;
    public bool Field;

	void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Axe"))
        //{
        //    Instantiate(ExploPrefab,transform.position,transform.rotation);
        //    if (!Field)
        //    Destroy(gameObject);
        //}
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Axe"))
        //{
            
        //    ContactPoint contact = collision.contacts[0];
        //    Vector3 pos = contact.point;
        //    if (Field)
        //        GetComponent<BoxCollider>().enabled = false;
        //    Instantiate(ExploPrefab, pos, transform.rotation);
        //    StartCoroutine(ResetCollider());
        //}
    }

    IEnumerator ResetCollider()
    {
        yield return new WaitForSeconds(3);
        GetComponent<BoxCollider>().enabled = true;
    }
}
