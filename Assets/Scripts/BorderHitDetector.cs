using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderHitDetector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col){
        // Debug.Log(col.tag);

        // Car objects' tag has been set to "Player"
        if (col.tag == "Player"){
            
            keyboardController motionController = col.GetComponent<keyboardController>();
            motionController.hitBorder = true;

            //set triggered object's velocity to zero
            Rigidbody2D rd = col.GetComponent<Rigidbody2D>();
            rd.velocity = Vector2.zero;
            rd.angularVelocity = 0;
            
        }

    }
}
