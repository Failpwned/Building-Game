using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid3D : MonoBehaviour
{

    public static class LIGHTING
    {
        public const int DEFAULT = 0;
        public const int POWERED = 1;
        public const int DELETE_MARK = 2;
        public const int DELETE_SUB = 3;
        public const int LIST_LENGTH = 4;
    }

    public static class MODE
    {
        public const int DEFAULT = 0;
        public const int PREVIEW = 1;
    }




    public bool isStatic;
    public bool isIntiallyPowered;


    public bool IsCollisionChecked
    {
        get
        {
            foreach(Primitive3D primitive in childPrimitives)
            {
                if (!primitive.IsCollisionChecked)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public bool IsValidPlacement { get; private set; }

    private bool isPowered;
    public bool IsPowered
    {
        get
        {
            return isPowered;
        }
        set
        {
            isPowered = value;
            if (isPowered)
            {
                SetLight(LIGHTING.POWERED);
            }
            else
            {
                SetLight(LIGHTING.DEFAULT);
            }
        }
    }


    

    public Solid3D ParentSolid { get; set; }
    public HashSet<Solid3D> Children { get; set; }
    public ShapeButton ParentButton { get; set; }


    private HashSet<Primitive3D> childPrimitives = new HashSet<Primitive3D>();
    public Primitive3D ActivePrimitive { get; set; }

    private void Awake()
    {
        foreach (Primitive3D primitive in GetComponentsInChildren<Primitive3D>())
        {
            childPrimitives.Add(primitive);
            ActivePrimitive = primitive; // hacky solution to get any primitive
            primitive.ParentSolid = this;
        }

        ParentSolid = null;
        Children = new HashSet<Solid3D>();

        if (isStatic)
        {
            isPowered = isIntiallyPowered;
        }
        else
        {
            IsPowered = isIntiallyPowered;
            SetLight(LIGHTING.DEFAULT);
            
        }
    }




    public void AlignToFace(Face2D selfFace, Face2D targetFace)
    {
        ApplyRotation(Quaternion.FromToRotation(selfFace.Normal, -targetFace.Normal)); // Align Normals
        if (Vector3.Angle(selfFace.Rotation, targetFace.Rotation) == 180)
        {
            ApplyRotation(Quaternion.AngleAxis(180, selfFace.Normal)); // 180 degree rotation forced to rotate around normals
        }
        else
        {
            ApplyRotation(Quaternion.FromToRotation(selfFace.Rotation, targetFace.Rotation)); // Align Rotation. 
        }


        transform.position = targetFace.transform.position - selfFace.transform.position;
    }

    public void ApplyRotation(Quaternion rotation)
    {
        rotation.ToAngleAxis(out float angle, out Vector3 axis);
        transform.RotateAround(ActivePrimitive.transform.position, axis, angle);
    }

    // TODO: Fix this
    public IEnumerator ApplyRotationWithAnimation(Quaternion rotation)
    { 
        rotation.ToAngleAxis(out float angle, out Vector3 axis);
        float lastAngle = 0;
        float currentAngle = 0;
        float time = 0f;
        while(time <= 1.0f)
        {
            time += 0.05f;
            currentAngle = COMMON.MATHFUNCTIONS.SquaredSmooth(0, angle, time);
            transform.RotateAround(ActivePrimitive.transform.position, axis, currentAngle - lastAngle);
            lastAngle = currentAngle;
            yield return null;
        }
        CheckPreviewValid();
        LevelManager.Current.InputLocked = false;
    }

    public Face2D GetFaceByNumberOfSides(int sides)
    {
        Face2D foundSide = null;
        foreach(Primitive3D primitive in childPrimitives)
        {
            foundSide = primitive.GetFaceByNumberOfSides(sides);
            if(foundSide != null)
            {
                return foundSide;
            }
        }
        return null;
    }

    private void SetLight(int materialIndex)
    {
        foreach(Primitive3D primitive in childPrimitives)
        {
            primitive.SetLight(materialIndex);
        }
    }

    private void TogglePreviewValid(bool isValid)
    {
        foreach (Primitive3D primitive in childPrimitives)
        {
            primitive.TogglePreviewValid(isValid);
        }
    }

    public void SetMode(int mode)
    {
        foreach(Primitive3D primitive in childPrimitives)
        {
            primitive.SetMode(mode);
        }
    }

    

    public void ToggleMarkForDelete(bool isDeleting, bool isRoot)
    {
        if (isDeleting)
        {
            if (isRoot)
            {
                SetLight(LIGHTING.DELETE_MARK);
            }
            else
            {
                SetLight(LIGHTING.DELETE_SUB);
            }
        }
        else
        {
            if (isPowered)
            {
                SetLight(LIGHTING.POWERED);
            }
            else
            {
                SetLight(LIGHTING.DEFAULT);
            }
        }
    }

    public void CheckPreviewValid()
    {
        foreach (Primitive3D primitive in childPrimitives)
        {
            if (!primitive.IsValidPlacement)
            {
                TogglePreviewValid(false);
                IsValidPlacement = false;
                return;
            }
        }
        TogglePreviewValid(true);
        IsValidPlacement = true;
    }

}
