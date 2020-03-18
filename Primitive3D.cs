using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Primitive3D : MonoBehaviour
{
    public Material LightDefault;
    public Material LightPowered;
    public Material LightDeleteMark;
    public Material LightDeleteSub;

    private HashSet<Face2D> allFaces = new HashSet<Face2D>();

    // Modes
    public GameObject lightingObject;
    public GameObject previewObject;
    public GameObject defaultObject;

    // Collision
    private CollisionHandler collisionHandler;
    public bool IsCollisionChecked { get; set; }
    public bool IsIntersecting { get; set; }


    private Material[] lightMaterials = new Material[Solid3D.LIGHTING.LIST_LENGTH];


    public Solid3D ParentSolid { get; set; }

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

        IsIntersecting = false;
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


    public Face2D GetFaceByNumberOfSides(int sides)
    {
        foreach (Face2D face in allFaces)
        {
            if (face.sides == sides)
            {
                return face;
            }
        }
        return null;
    }

    public Face2D GetFaceByNormal(Vector3 normal)
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
        return result;
    }

    public Quaternion GetRotationToClosestFace(Face2D start, Vector3 rotationAxis)
    {
        Face2D closestFace = null;
        foreach (Face2D face in allFaces)
        {
            if (face != start && face.sides == start.sides &&
                (Vector3.Angle(Vector3.Cross(start.Normal, face.Normal), rotationAxis) < COMMON.INPUT.ROTATION_AXIS_THRESHOLD) ||
                Vector3.Angle(start.Normal, face.Normal) == 180)
            {
                if (closestFace == null || Vector3.Angle(start.Normal, closestFace.Normal) > Vector3.Angle(start.Normal, face.Normal))
                {
                    closestFace = face;
                }
            }
        }
        if (Vector3.Angle(start.Normal, closestFace.Normal) == 180)
        {
            return Quaternion.AngleAxis(180, rotationAxis);
        }
        else
        {
            return Quaternion.FromToRotation(start.Normal, closestFace.Normal);
        }
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
                break;
            case Solid3D.MODE.COLLIDER:
                ToggleMeshRenderers(defaultObject, false);
                ToggleMeshRenderers(previewObject, false);
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
}
