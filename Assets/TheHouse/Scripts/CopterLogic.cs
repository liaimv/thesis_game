using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains the logic of the behavior of the helicopter. The copter shoots with rockets at random time.

public class CopterLogic : MonoBehaviour {

    public Transform[] Spawners,Bullets; // the copter has 2 spawners for example of rockets or shooting bullets may contain any spawners as you wish | Bullets is a gameobjects (for example Rockets)
    public float RandomSpawnerTime, MaxTime, delayingTime; //RandomSpawnerTime let the CPU choose when to shoot | MaxTime has random time value | delayingTime is a time between rockets launches
    public int BulletType,countBullets, MaxBullets, OrderSpawn; //BulletType (0-rocket,1-bullets (bullets are not include)) | if countBullets less then MaxBullets (random value) then increase countBullets number until there is a maximum value
                                                                //OrderSpawn is a selection between Spawners
    public bool SpawnRockets; //cap 

    void Start () {
        MaxTime = Random.Range(5, 10);
        MaxBullets = Random.Range(1, 4);
    }
	
	void Update () {

        RandomSpawnerTime += Time.deltaTime;

        if (RandomSpawnerTime>= MaxTime)
        {
            BulletType = Random.Range(0, Bullets.Length);

            if (BulletType == 0)
                SpawnRockets = true;

            MaxTime = Random.Range(2,5);
            RandomSpawnerTime = 0;
        }

        if (SpawnRockets)
        {
            delayingTime += Time.deltaTime;

            if (delayingTime >= .2f) { 

            countBullets += 1;
            OrderSpawn += 1;

            if (OrderSpawn >= 2)
            {
                OrderSpawn = 0;
            }

            Instantiate(Bullets[BulletType], Spawners[OrderSpawn].transform.position, Spawners[OrderSpawn].transform.rotation);

            if (countBullets >= MaxBullets)
            {
                SpawnRockets = false;
                MaxBullets = Random.Range(1,4);
                countBullets = 0;
            }

                delayingTime = 0;
            }
        }
    }
}
