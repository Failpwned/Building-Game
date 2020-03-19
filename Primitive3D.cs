using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Primitive3D : MonoBehaviour
{
    public Material LightDefault;
    public Material LightPowered;
    public Material LightDeleteMark;
    public Material LightDeleteSub;

    public Material PreviewValid;
    public Material PreviewInvalid;

    private HashSet<Face2D> allFaces = new HashSet<Face2D>();

    // Modes
    public GameObject lightingObject;
    public GameObject previewObject;
    public GameObject defaultObject;

    // Collision
    private CollisionHandler collisionHandler;
    public bool IsCollisionChecked { get; set; }

    public bool IsValidPlacement
    {
        get
        {
            return CollisionCount == 0;
        }
    }
    public int CollisionCount { get; set; }

    private Material[] lightMaterials = new Material[Solid3D.LIGHTING.LIST_LENGTH];


    public Solid3D ParentSolid { get; set; }
    public Face2D SelectedFace { get; set; }

    private void Awake()
    {
        foreach (Face2D face in GetComponentsInChildren<Face2D>())
        {
            allFaces.Add(face);
            face.ParentPrimitive = this;
        }

        lightMaterials[Solid3D.LIGHTING.DEFAULT] = LightDefault;
        lightMaterials[Solid3D.LIGHTING.POWERED] = LightPowered;
        lightMaterials[Solid3D.LIGHTING.DELETE_MARK] = LightDeleteMark;
        lightMaterials[Solid3D.LIGHTING.DELETE_SUB] = LightDeleteSub;

        CollisionCount = 0;
        IsCollisionChecked = false;
        collisionHandler = GetComponentInChildren<CollisionHandler>();
        if (collisionHandler != null)
        {
            collisionHandler.ParentPrimitive = this;
        }

    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SelectFaceByNumberOfSides(int sides)
    {
        foreach (Face2D face in allFaces)
        {
            if (face.sides == sides)
            {
                SelectedFace = face;
                return;
            }
        }
        SelectedFace = null;
    }

    public void SelectFaceByNormal(Vector3 normal)
    {
        Face2D result = null;
        float angle = 181;
        foreach (Face2D face in allFaces)
        {
            if (Vector3.Angle(normal, face.Normal) < angle)
            {
                result = face;
                angle = Vector3.Angle(normal, face.Normal);
            }
        }
        SelectedFace = result;
    }

    public void RotateToClosestFace(Vector3 rotationAxis)
    {
        Quaternion closestRotation = Quaternion.identity;
        Vector3 closestAxis = Vector3.zero;
        foreach(Quaternion rotation in SelectedFace.ValidRotations)
        {
            rotation.ToAngleAxis(out float angle, out Vector3 axis);
            if (closestAxis == Vector3.zero) {
                if(Vector3.Angle(axis, rotationAxis) < MainCamera.INPUT.ROTATION_AXIS_THRESHOLD)
                {
                    closestAxis = axis;
                    closestRotation = rotation;
                }
            }
            else if(Vector3.Angle(axis, rotationAxis) < Vector3.Angle(closestAxis, rotationAxis))
            {
                closestAxis = axis;
                closestRotation = rotation; 
            }
        }

        if(closestRotation == Quaternion.identity)
        {
            if(Vector3.Angle(SelectedFace.Normal, rotationAxis) < MainCamera.INPUT.ROTATION_AXIS_THRESHOLD)
            {
                StartCoroutine(RotationWithAnimation(Quaternion.AngleAxis(SelectedFace.SymmetricRotation, SelectedFace.Normal))); 
            }
            else if (180 - Vector3.Angle(SelectedFace.Normal, rotationAxis) < MainCamera.INPUT.ROTATION_AXIS_THRESHOLD)
            {
                StartCoroutine(RotationWithAnimation(Quaternion.AngleAxis(SelectedFace.SymmetricRotation, -SelectedFace.Normal)));
            }
            else
            {
                // no valid rotation
            }
        }
        else
        {
            StartCoroutine(RotationWithAnimation(closestRotation));
        }

        
    }

    private IEnumerator RotationWithAnimation(Quaternion rotation)
    {
        Vector3 originalFaceNormal = SelectedFace.Normal;

        rotation.ToAngleAxis(out float angle, out Vector3 axis);
        float lastAngle = 0;
        float currentAngle = 0;
        float time = 0f;
        while (time <= 1.0f)
        {
            time += 0.05f;
            currentAngle = COMMON.MATHFUNCTIONS.SquaredSmooth(0, angle, time);
            ParentSolid.transform.RotateAround(transform.position, axis, currentAngle - lastAngle);
            lastAngle = currentAngle;
            yield return null;
        }
        SelectFaceByNormal(originalFaceNormal);
        ParentSolid.CheckPreviewValid();
        LevelManager.Current.InputLocked = false;
    }

    public void SetLight(int materialIndex)
    {
        Material[] materials = lightingObject.GetComponent<Renderer>().materials;
        materials[0] = lightMaterials[materialIndex];
        lightingObject.GetComponent<Renderer>().materials = materials;
    }

    public void SetMode(int mode)
    {
        switch (mode)
        {
            case Solid3D.MODE.DEFAULT:
                ToggleMeshRenderers(defaultObject, true);
                ToggleMeshRenderers(previewObject, false);
                collisionHandler.GetComponent<Collider>().isTrigger = false;
                break;
            case Solid3D.MODE.PREVIEW:
                ToggleMeshRenderers(defaultObject, false);
                ToggleMeshRenderers(previewObject, true);
                collisionHandler.GetComponent<Collider>().isTrigger = true;
                break;
        }
    }

    private void ToggleMeshRenderers(GameObject root, bool enabled)
    {
        foreach (MeshRenderer meshRenderer in root.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.enabled = enabled;
        }
    }

    public void TogglePreviewValid(bool isValid)
    {
        foreach (Transform childTransform in previewObject.transform)
        {
            Material[] materials = childTransform.gameObject.GetComponent<Renderer>().materials;
            if (isValid)
            {
                materials[0] = PreviewValid;
            }
            else
            {
                materials[0] = PreviewInvalid;
            }

            childTransform.gameObject.GetComponent<Renderer>().materials = materials;
        }
    }
}
