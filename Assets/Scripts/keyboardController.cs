using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyboardController : MonoBehaviour
{
    public GameObject car;
    private Rigidbody2D rb;

    public Vector3 startPosition;
    public Vector3 startRotation;

    [Header("Car Speed")]
    public float accelerationRate;
    public float steeringRate;


    [Space]
    public bool hitBorder;
    // indicate if car can still move or not

    [SerializeField]
    private float leftSensor, frontSensor, rightSensor;
    // sensors of the car agent

    [Header("Fitness")]
    [SerializeField]
    private float Fitness;
    [SerializeField]
    private float distanceMultipler = 4.0f;
    [SerializeField]
    private float avgSpeedMultiplier = 0.5f;
    private float sensorMultiplier = 1f;

    [Header("Data")]
    public float distanceTravelled;
    public float averageSpeed;
    public float timeLived;

    private Vector3 lastPosition; // position of the car in last frame

    // Neural Network variables
    [Header("Neural Network Status")]
    private NerualNetwork neuralNetwork;
    public int neuralLayers;
    public int neurons;


    void Awake()
    {
        // Car accelaration and steering data initialization
        accelerationRate = 1.5f;
        steeringRate = 3f;

        car = this.gameObject;
        rb = car.GetComponent<Rigidbody2D>();

        hitBorder = false;

        distanceTravelled = 0;

        // Record start postion and rotation
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        lastPosition = startPosition;

        // ignore car object's own collider when raycast
        Physics2D.queriesStartInColliders = false;

        neuralNetwork = GetComponent<NerualNetwork>();
        neuralLayers = 3;
        neurons = 10;

        neuralNetwork.Initialize(neuralLayers, neurons);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // Debug.DrawRay(transform.position,transform.up-transform.right, Color.cyan );
        if (!hitBorder)
        {

            updateSensors();
            var (accelarationInput, steeringInput) = neuralNetwork.RunNetwork(leftSensor, frontSensor, rightSensor);

            float acceleration = accelarationInput * accelerationRate;
            float steeringPower = steeringInput;
            //Input.GetAxis("Horizontal");
            // print("Vertical: " + Input.GetAxis("Vertical") + ", Horizontal: " + Input.GetAxis("Horizontal"));
            //float acceleration = Random.Range(0f, 1f) * accelerationRate;
            //float steeringPower = Random.Range(-1f, 1f);//Input.GetAxis("Horizontal");

            // get the object steering direction
            float direction = Mathf.Sign(Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up)));

            rb.rotation += steeringPower * steeringRate * rb.velocity.magnitude * -direction;

            // accelerate object according to its local transform value
            rb.AddRelativeForce(Vector2.up * acceleration);
            rb.AddRelativeForce(Vector2.right * rb.velocity.magnitude * steeringPower / 2);

            
            calculateFitness();
            //update position value
            lastPosition = transform.position;
            timeLived += Time.deltaTime;
        }
        else {
            Death();
        }
    }

    void Reset()
    {
        timeLived = 0f;
        distanceTravelled = 0f;
        averageSpeed = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        Fitness = 0f;
        lastPosition = startPosition;
        hitBorder = false;
    }

    /* Calculate Fitness of the car:
     * 
     * Current position - last position 
     */
    private void calculateFitness() {

        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        averageSpeed = distanceTravelled / timeLived;

        Fitness = (distanceTravelled * distanceMultipler) + (averageSpeed * avgSpeedMultiplier) + ((leftSensor + frontSensor + rightSensor) / 3 * sensorMultiplier);

        if (timeLived > 20 && Fitness < 20) {
            Death();
        }
        if(transform.position == lastPosition){
            Death();
        }

        // if(Fitness > 1000){
        //     Death();
        // }
    }


    //update 3 sensors values every frame
    private void updateSensors() {
        Vector2 leftDirection = transform.up - transform.right; // direction of the left diagonal of the car
        Vector2 frontDirection = transform.up; // direction of the front diagonal of the car
        Vector2 rightDirection = transform.up + transform.right; // direction of the right diagonal of the car

        Ray2D leftRay = new Ray2D(transform.position, leftDirection);
        Ray2D frontRay = new Ray2D(transform.position, frontDirection);
        Ray2D rightRay = new Ray2D(transform.position, rightDirection);
        Debug.DrawRay(transform.position, leftDirection, Color.cyan);
        RaycastHit2D hit;


        if (hit = Physics2D.Raycast(transform.position, leftDirection))
        {
            leftSensor = hit.distance / 6;
            // print("Left Hit: " + leftSensor);
            Debug.DrawRay(transform.position, leftDirection, Color.green);
            // print(hit.collider.gameObject.name);
        }

        if (hit = Physics2D.Raycast(transform.position, frontDirection))
        {
            frontSensor = hit.distance /6;
            // print("Front Hit: " + frontSensor);
            Debug.DrawRay(transform.position, frontDirection, Color.green);
        }
        if (hit = Physics2D.Raycast(transform.position, leftDirection))
        {
            rightSensor = hit.distance /6;
            // print("Right Hit: " + rightSensor);
            Debug.DrawRay(transform.position, rightDirection, Color.green);
        }
    }

    public void ResetWithNeuralNetwork(NerualNetwork network){
        neuralNetwork = network;
        Reset();
    }

    private void Death(){
        GameObject.FindObjectOfType<CarGenesController>().Death(Fitness, neuralNetwork);
    }
}
