using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceSelector : MonoBehaviour
{ 

    public Face2D Face { get; private set; }

    public void AlignSelectorToFace(Face2D targetFace)
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, targetFace.Normal) * transform.rotation; // Align Normals
        if (Vector3.Angle(transform.rotation * Vector3.right, targetFace.Rotation) == 180)
        {
            transform.rotation = Quaternion.AngleAxis(180, transform.rotation * Vector3.up) * transform.rotation; // 180 degree rotation forced to rotate around normals
        }
        else
        {
            transform.rotation = Quaternion.FromToRotation(transform.rotation * Vector3.right, targetFace.Rotation) * transform.rotation; // Align Rotation. 
        }


        transform.position = targetFace.transform.position;
    }

}
