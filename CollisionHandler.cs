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

    private void OnTriggerEnter(Collider other)
    {
        ParentPrimitive.CollisionCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        ParentPrimitive.CollisionCount--;
    }

    private void FixedUpdate()
    {
        if (!ParentPrimitive.IsCollisionChecked)
        {
            if (collisionCheckCounter >= 2)
            {
                ParentPrimitive.IsCollisionChecked = true;
                ParentPrimitive.ParentSolid.CheckPreviewValid();
            }
            else
            {
                collisionCheckCounter += 1;
            } 
        }
    }
}
