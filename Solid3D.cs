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

    private bool isValidPlacement;
    public bool IsValidPlacement
    {
        get
        {
            return isValidPlacement;
        }
        set
        {
            isValidPlacement = value;
            TogglePreviewValid(isValidPlacement);
        }
    }

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


    public void AlignToSelectedFace(Face2D targetFace)
    {
        if (ActivePrimitive != null && ActivePrimitive.SelectedFace != null)
        {
            ApplyRotation(Quaternion.FromToRotation(ActivePrimitive.SelectedFace.Normal, -targetFace.Normal)); // Align Normals
            if (Vector3.Angle(ActivePrimitive.SelectedFace.Rotation, targetFace.Rotation) == 180)
            {
                ApplyRotation(Quaternion.AngleAxis(180, ActivePrimitive.SelectedFace.Normal)); // 180 degree rotation forced to rotate around normals
            }
            else
            {
                ApplyRotation(Quaternion.FromToRotation(ActivePrimitive.SelectedFace.Rotation, targetFace.Rotation)); // Align Rotation. 
            }
            transform.position = targetFace.transform.position - ActivePrimitive.SelectedFace.transform.position; 
        }
    }

    private void ApplyRotation(Quaternion rotation)
    {
        rotation.ToAngleAxis(out float angle, out Vector3 axis);
        transform.RotateAround(ActivePrimitive.transform.position, axis, angle);
    }

    public void SelectFaceByNumberOfSides(int sides)
    {
        foreach(Primitive3D primitive in childPrimitives)
        {
            primitive.SelectFaceByNumberOfSides(sides);
            if(primitive.SelectedFace != null)
            {
                ActivePrimitive = primitive;
                return;
            }
        }
        ActivePrimitive = null;
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
                IsValidPlacement = false;
                return;
            }
        }
        IsValidPlacement = true;
    }

}
