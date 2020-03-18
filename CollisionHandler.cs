using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public Primitive3D ParentPrimitive { get; set; }
    private int collisionCheckCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        ParentPrimitive.IsIntersecting = true;
    }

    private void FixedUpdate()
    {
        
        if (collisionCheckCounter >= 2)
        {
            ParentPrimitive.IsCollisionChecked = true;
        }
        else
        {
            collisionCheckCounter += 1;
        }
    }
}
