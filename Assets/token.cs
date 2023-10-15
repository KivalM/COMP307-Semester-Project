using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class token : MonoBehaviour
{
    // Start is called before the first frame update
    public int segments = 10;
    public float radius = 0.5f;
   
    
    private float rotationSpeed = 5f;
    private float lerpSpeed = 5f;

    private float angle = 0f;


    void Start()
    {
        
        CreateRing();

        // add a collider
        gameObject.AddComponent<SphereCollider>();

        // add a rigidbody and set it to kinematic so it doesn't react to gravity
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        
        rigidbody.isKinematic = true;

        // add a trigger
        SphereCollider collider = gameObject.GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = radius*2;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

             // add the token to the player's inventory
            other.gameObject.GetComponent<Inventory>().AddToken();

            // destroy the token
            Destroy(gameObject);
        }
    }




    private void Update()
    {

        // rotate the ring around the center position
        angle += rotationSpeed * Time.deltaTime;

        if (angle >= 360f)
        {
            angle = 0f;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, angle, 0f), Time.deltaTime * lerpSpeed);

    }
   

    void CreateRing()
    {

        // get the coordinates of the center position
        Vector3 centerPosition = transform.position;



        // draw a ring of cylinder segments around the center position
        for (int i = 0; i < segments; i++)
        {
            float angle = (i * 360f / segments) * Mathf.Deg2Rad;
            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;
            Vector3 position = centerPosition + new Vector3(x, 0f, z);
            Vector3 position2 = centerPosition + new Vector3(x, 0f, z);
            drawSegment(position, position2);
        }
    }

    void drawSegment( Vector3 position, Vector3 position2)
    {
        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        segment.transform.position = position;
        segment.transform.localScale = new Vector3(radius, 0.5f, radius);
        segment.transform.LookAt(position2);
        segment.transform.Rotate(90f, 0f, 0f);
        segment.transform.parent = transform;
    }
    

}
