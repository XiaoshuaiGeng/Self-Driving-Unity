using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyboardController : MonoBehaviour
{
    public GameObject car;
    private Rigidbody2D rb;
    public float accelerationRate;
    public float steeringRate;

    public bool hitBorder; 
    // indicate if car can still move or not
    private float leftSensor, frontSensor, rightSensor; 
    // sensors of the car agent

    // Start is called before the first frame update
    void Start()
    {
        accelerationRate = 1.5f;
        steeringRate = 3f;
        car = this.gameObject;
        rb = car.GetComponent<Rigidbody2D>();
        hitBorder = false;

        Physics2D.queriesStartInColliders = false; // ignore car object's own collider when raycast
    }

    // Update is called once per frame
    void Update()
    {

        // Debug.DrawRay(transform.position,transform.up-transform.right, Color.cyan );
        if (!hitBorder){
            // print("Vertical: " + Input.GetAxis("Vertical") + ", Horizontal: " + Input.GetAxis("Horizontal"));
            float acceleration = Input.GetAxis("Vertical") * accelerationRate;
            float steeringPower = Input.GetAxis("Horizontal");

            // get the object steering direction
            float direction = Mathf.Sign(Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up)));

            rb.rotation += steeringPower * steeringRate * rb.velocity.magnitude * -direction;
    
            // accelerate object according to its local transform value
            rb.AddRelativeForce(Vector2.up * acceleration);
            rb.AddRelativeForce(Vector2.right * rb.velocity.magnitude * steeringPower/2);   

            Vector2 leftDirection = transform.up - transform.right; // direction of the left diagonal of the car
            Vector2 frontDirection = transform.up; // direction of the front diagonal of the car
            Vector2 rightDirection = transform.up + transform.right; // direction of the right diagonal of the car
            
            Ray2D leftRay = new Ray2D(transform.position, leftDirection);
            Ray2D frontRay = new Ray2D(transform.position, frontDirection);
            Ray2D rightRay = new Ray2D(transform.position, rightDirection);
            Debug.DrawRay(transform.position,leftDirection, Color.cyan );
            RaycastHit2D hit;
            
            if(hit = Physics2D.Raycast(transform.position, leftDirection)){
                leftSensor = hit.distance / 10;
                print("Left Hit: " + leftSensor);
                Debug.DrawRay(transform.position,leftDirection, Color.green );
                print(hit.collider.gameObject.name);
            }

            if(hit = Physics2D.Raycast(transform.position, frontDirection)){
                frontSensor = hit.distance / 10;
                print("Front Hit: " + frontSensor);
                Debug.DrawRay(transform.position,frontDirection, Color.green );
            }
            if(hit = Physics2D.Raycast(transform.position, leftDirection)){
                rightSensor = hit.distance / 10;
                print("Right Hit: " + rightSensor);
                Debug.DrawRay(transform.position,rightDirection, Color.green );
            }
        }
    }
}
