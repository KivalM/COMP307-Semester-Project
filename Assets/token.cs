using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class token : MonoBehaviour
{
    // Start is called before the first frame update
    public int segments = 50;
    public float outerRadius = 5f;
    public float innerRadius = 4f;
    public float height = 0.2f;
    public float rotationSpeed = 20f;
    public float destroyRange = 5f; // Set your desired destroy range
    public float destroyDelay = 2f;
    private Vector3 centerPosition;
    public Vector3 posInit = new Vector3();
   
    


    void Start()
    {
        
        CreateRing();
        transform.position = posInit;
        transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        centerPosition = transform.position + new Vector3(0f, height / 2f * transform.localScale.y, 0f);
        

    }
    private void Update()
    {
        transform.RotateAround(centerPosition, Vector3.up, rotationSpeed * Time.deltaTime);
        
    }
   

    void CreateRing()
    {
        
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x_outer = outerRadius * Mathf.Cos(angle);
            float y_outer = outerRadius * Mathf.Sin(angle);

            float x_inner = innerRadius * Mathf.Cos(angle);
            float y_inner = innerRadius * Mathf.Sin(angle);

            Vector3 outerPoint = centerPosition + new Vector3(x_outer, 0f, y_outer);
            Vector3 innerPoint = centerPosition + new Vector3(x_inner, 0f, y_inner);

            // Create a cylinder segment between outer and inner points
            CreateCylinderSegment(outerPoint, innerPoint);
        }
    }

    void CreateCylinderSegment(Vector3 start, Vector3 end)
    {
        Vector3 position = (start + end) / 2f;
        Quaternion rotation = Quaternion.LookRotation(end - start, Vector3.up);
        float distance = Vector3.Distance(start, end);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = position;
        cylinder.transform.rotation = rotation;
        cylinder.transform.localScale = new Vector3(cylinder.transform.localScale.x, distance / 2f, cylinder.transform.localScale.z);

        
        // Set the current GameObject as the parent
        cylinder.transform.SetParent(transform);


    }
    

}
