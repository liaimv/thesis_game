using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Destroying an object for a specific case

public class DestroyObject : MonoBehaviour {

    public float TimeDestroy, MaxTimeParticle;
    private float TimeDestroyParticle;
    public bool isParticle;
    public GameObject Particle;

	void Start () {

        if (!isParticle)
            Destroy(gameObject, TimeDestroy);

    }
	
	void Update () {

        TimeDestroyParticle += Time.deltaTime;
        if (TimeDestroyParticle>= MaxTimeParticle)
        {
            if (isParticle)
            {
                Particle.GetComponent<ParticleSystem>().Stop();
                Destroy(gameObject, TimeDestroy);
            }
        }

    }
}
