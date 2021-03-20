using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyboardController : MonoBehaviour
{
    public GameObject car;
    public float accelerationRate;
    public float steeringRate;

    public bool hitBorder; // indicate if car can still move or not
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        car = this.gameObject;
        rb = car.GetComponent<Rigidbody2D>();
        hitBorder = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitBorder){
            float acceleration = Input.GetAxis("Vertical") * accelerationRate;
            float steeringPower = Input.GetAxis("Horizontal");

            // get the object steering direction
            float direction = Mathf.Sign(Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up)));

            rb.rotation += steeringPower * steeringRate * rb.velocity.magnitude * -direction;
    
            // accelerate object according to its local transform value
            rb.AddRelativeForce(Vector2.up * acceleration);
            rb.AddRelativeForce(Vector2.right * rb.velocity.magnitude * steeringPower/2);   
        }
        
    }
}
