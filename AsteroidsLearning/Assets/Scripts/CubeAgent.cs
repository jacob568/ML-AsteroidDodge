using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class CubeAgent : Agent
{
    private int health;
    private Rigidbody rigidbody;

    public ObjectSpawner objectSpawner;

    private int goldNeeded = 5;
    private int goldCollected = 0;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Sets up the episode
    /// </summary>
    public override void OnEpisodeBegin()
    {
        goldCollected = 0;

        //If the agent dies, then must reset its health and location
        this.rigidbody.angularVelocity = Vector3.zero;
        this.rigidbody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(-50f, -35f, -33f);

        health = 100;

        objectSpawner.RemoveAllObjects();
    }

    /// <summary>
    /// Provides the agents with all the information it needs to succeed
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.health);
        sensor.AddObservation(this.transform.localPosition);

        sensor.AddObservation(rigidbody.velocity.x);
        sensor.AddObservation(rigidbody.velocity.y);
        sensor.AddObservation(rigidbody.velocity.z);

        sensor.AddObservation(rigidbody.rotation.x);
        sensor.AddObservation(rigidbody.rotation.y);
        sensor.AddObservation(rigidbody.rotation.z);

        sensor.AddObservation(goldCollected);

        Transform gold = objectSpawner.GetGoldGameObject();
        sensor.AddObservation(gold.localPosition.x);
        sensor.AddObservation(gold.localPosition.y);
        sensor.AddObservation(gold.localPosition.z);
    }

    /// <summary>
    /// Handles the agents controls, assigning a control to each possible input the agents can input
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void OnActionReceived(float[] vectorAction)
    {
        //Small punishment every frame, to encourage the ship to complete the task faster
        AddReward(-0.00001f);
        //Actions, size 3, probably 4 when shooting included
        int moveForward = Mathf.FloorToInt(vectorAction[0]);
        int rotateX = Mathf.FloorToInt(vectorAction[1]);
        int rotateY = Mathf.FloorToInt(vectorAction[2]);

        if (moveForward == 1)
        {
            MoveForward();
        }

        if (moveForward == 2)
        {
            MoveBackwards();
        }

        if (rotateX == 1)
        {
            RotateXAxis(1f);
        }

        if (rotateX == 2)
        {
            RotateXAxis(-1f);
        }

        if (rotateY == 1)
        {
            RotateYAxis(1f);
        }

        if (rotateY == 2)
        {
            RotateYAxis(-1f);
        }
    }

    private float speed = 15f;
    /// <summary>
    /// Moves the spaceship forwards
    /// </summary>
    void MoveForward()
    {
        rigidbody.AddForce(-transform.up * speed);

    }

    private float backwardsSpeed = 6f;
    /// <summary>
    /// Moves the spaceship backwards
    /// </summary>
    void MoveBackwards()
    {
        rigidbody.AddForce(transform.up * backwardsSpeed);
    }

    private float rotateSpeed = 70f;
    /// <summary>
    /// Rotates the ship along the y axis.
    /// </summary>
    /// <param name="direction">Which direction to rotate</param>
    void RotateYAxis(float direction)
    {

        transform.Rotate(Vector3.forward * (direction * rotateSpeed) * Time.deltaTime);

    }

    /// <summary>
    /// Rotates the ship along the x axis.
    /// </summary>
    /// <param name="direction">Which direction to rotate</param>
    void RotateXAxis(float direction)
    {
        transform.Rotate(Vector3.left * (direction * rotateSpeed) * Time.deltaTime);
    }

    /// <summary>
    /// Applies damage to the spaceship, and ends the episode if health reaches zero.
    /// </summary>
    public void TakeDamage()
    {
        health -= 20;
        //Small punishment for hitting an asteroid and losing health
        AddReward(-.2f);
        if (health <= 0)
        {
            SetReward(-1f);
            EndEpisode();
        }
    }


    public override float[] Heuristic()
    {
        var action = new float[3];

        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[2] = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            action[2] = 2f;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            action[1] = 1f;
        }
        if (Input.GetKey(KeyCode.C))
        {
            action[1] = 2f;
        }
        return action;
    }

    /// <summary>
    /// Punishes the player for bashing into the sides
    /// </summary>
    /// <param name="collision">The object collided with</param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Edge")
        {
            AddReward(-.3f);
        }
    }

    /// <summary>
    /// Handles collisions with asteroids and gold.
    /// </summary>
    /// <param name="collision">The object that hit the trigger</param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Object")
        {
            TakeDamage();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Collectible")
        {
            goldCollected++;
            //Gain large reward for getting gold
            AddReward(2f);
            objectSpawner.MoveGold();
            if (goldCollected >= goldNeeded)
            {
                //Slightly larger award and episode ends when gold needed is reached.
                SetReward(2.5f);
                EndEpisode();
            }
        }
    }
}
