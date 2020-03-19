using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face2D : MonoBehaviour
{
    public int sides;
    public Primitive3D ParentPrimitive { get; set; }
    public HashSet<Quaternion> ValidRotations
    {
        get
        {
            HashSet<Quaternion> validRotations = new HashSet<Quaternion>();
            foreach (Transform childTransform in transform)
            {
                ValidRotation3D vr = childTransform.gameObject.GetComponent<ValidRotation3D>();
                validRotations.Add(Quaternion.AngleAxis(Vector3.Angle(transform.up - Vector3.Project(transform.up, childTransform.up),
                    vr.targetFace.transform.up - Vector3.Project(vr.targetFace.transform.up, childTransform.up)), childTransform.up));
            }
            return validRotations;
        }
    }
    public float SymmetricRotation
    {
        get
        {
            return 360 / sides;
        }
    }

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
