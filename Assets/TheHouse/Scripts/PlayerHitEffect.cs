using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//On Screen blood controlled by Canvas. Attached to PlayerHitEffect gameobject in the inspector

public class PlayerHitEffect : MonoBehaviour
{

    public float intensity;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.parent.gameObject.GetComponent<Image>().color = new Color(255,255,255, intensity);

    }
}
