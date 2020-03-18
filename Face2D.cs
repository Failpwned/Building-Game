using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face2D : MonoBehaviour
{
    public int sides;
    public Primitive3D ParentPrimitive { get; set; }

    public Vector3 Normal
    {
        get
        {
            return transform.up;
        }
    }

    public Vector3 Rotation
    {
        get
        {
            return transform.right;
        }
    }

}
