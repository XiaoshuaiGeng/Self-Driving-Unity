using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
    
    // public GameObject object;
    public int population;
    public GameObject car;
    private GameObject SpawnPoint;

    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPoint = GameObject.Find("SpawnPoint");
    }

    // Update is called once per frame
    void Update()
    {
        while(i < population)
        {
            Instantiate(car, SpawnPoint.transform.position, Quaternion.Euler(0,0,-90));
            i++;

        }


    }
}
