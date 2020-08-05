using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private bool moving;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        moving = true;
        rigidbody = GetComponent<Rigidbody>();
        //When spawned, the objects are facing directly at the player
        //Slightly adjust the angle to give a more random course
        float xDegrees = Random.Range(-50f, 50f);
        float yDegrees = Random.Range(-50f, 50f);
        Vector3 newDirection = new Vector3(xDegrees, yDegrees, 0);

        transform.Rotate(newDirection);
    }

    // Update is called once per frame
    void Update()
    {
        //Adds a one time force to the object. No gravity or resistance means it
        //moves at a constant speed
        if (moving)
        {
            rigidbody.AddForce(transform.forward * 7f, ForceMode.Impulse);
            moving = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Destroy the object once theyre out of the play area
        if (other.tag == "Shredder")
        {
            Destroy(gameObject);
        }
    }
}
