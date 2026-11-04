using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////This script are using on Rocket analog BottleExplosion or can be use on any destructible objects as you wish.

public class CopterBulletsExplosions : MonoBehaviour {

    public Transform ExploPref;

    void Start()
    {
        Destroy(gameObject,4);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Explos"))
        {

            ContactPoint contact = collision.contacts[0];
            Vector3 pos = contact.point;
            Instantiate(ExploPref, pos, transform.rotation);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * 300 * Time.deltaTime);
    }
}
