using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Current { get; private set; }
    
    private Solid3D previewSolid = null;
    private Face2D selectedFace = null;
    private FaceSelector activeFaceSelector = null;
   
    public HashSet<IWinCondition> WinConditions { get; set; }
    public bool InputLocked { get; set; }

    private List<Solid3D> placedSolids = new List<Solid3D>();
    private Solid3D markedDeleteSolid = null;
    private HashSet<Solid3D> markedDeleteSet = new HashSet<Solid3D>();
    private bool deleteMode = false;
    public bool DeleteMode
    {
        get
        {
            return deleteMode;
        }
        set
        {
            deleteMode = value;
            if (deleteMode)
            {
                if(previewSolid != null)
                {
                    Destroy(previewSolid.gameObject);
                }
                if(activeFaceSelector != null)
                {
                    Destroy(activeFaceSelector.gameObject);
                }
                if(selectedFace != null && !selectedFace.ParentPrimitive.ParentSolid.isStatic)
                {
                    MarkSolidForDeletion(selectedFace.ParentPrimitive.ParentSolid);
                }
            }
            else
            {
                UnmarkSolidForDeletion();
            }
        }
    }


    private void Awake()
    {
        WinConditions = new HashSet<IWinCondition>();
        Current = this;
        InputLocked = false;
    }

    void Start()
    {

    }

    private void Update()
    {
        if (!InputLocked)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                InputLocked = true;
                previewSolid.ActivePrimitive.RotateToClosestFace(Vector3.Cross(Vector3.up, MainCamera.Current.transform.forward));
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                InputLocked = true;
                previewSolid.ActivePrimitive.RotateToClosestFace(Vector3.up);
            }
        }
    }
    

    private Solid3D readySolid;
    public Solid3D ReadySolid
    {
        get { return readySolid; }
        set
        {
            readySolid = value;
            if(readySolid == null)
            {
                if (previewSolid != null)
                {
                    ConfirmOrCancelPreviewSolid(false);
                }
            }
            else
            {
                if (selectedFace != null && readySolid != null)
                {
                    CreatePreviewSolidAtSelectedFace();
                }
            }
        }
    }

    public void SelectFace(Face2D face)
    {
        MainCamera.Current.LookTarget = face.transform.position;

        if (DeleteMode)
        {
            if(face.ParentPrimitive == markedDeleteSolid)
            {
                DeleteMarkedSolid();
            }
            else
            {
                if (!face.ParentPrimitive.ParentSolid.isStatic)
                {
                    UnmarkSolidForDeletion();
                    MarkSolidForDeletion(face.ParentPrimitive.ParentSolid);
                }
            }
        }
        else
        {
            if (previewSolid == null)
            {
                if (face == selectedFace)
                {
                    selectedFace = null;
                    Destroy(activeFaceSelector.gameObject);
                }
                else
                {
                    selectedFace = face;
                    CreateFaceSelector(face);
                    if (ReadySolid != null)
                    {
                        CreatePreviewSolidAtSelectedFace();
                    }
                }
            }
            else
            {
                if (face.ParentPrimitive.ParentSolid == previewSolid)
                {
                    if (face.ParentPrimitive = previewSolid.ActivePrimitive)
                    {
                        if (previewSolid.IsCollisionChecked && previewSolid.IsValidPlacement)
                        {
                            ConfirmOrCancelPreviewSolid(true);
                        }
                    }
                    else
                    {
                        previewSolid.ActivePrimitive = face.ParentPrimitive;
                        // TODO: other primitive doesn't have a valid face?
                    }
                }
                else
                {
                    selectedFace = face;
                    CreateFaceSelector(face);
                    CreatePreviewSolidAtSelectedFace();
                }
            }
        }
    }

    private void CreatePreviewSolidAtSelectedFace()
    {
        if (previewSolid != null)
        {
            Destroy(previewSolid.gameObject);
        }

        previewSolid = Instantiate(ReadySolid.gameObject).GetComponent<Solid3D>();
        previewSolid.SetMode(Solid3D.MODE.PREVIEW);
        previewSolid.SelectFaceByNumberOfSides(selectedFace.sides);

        if (previewSolid.ActivePrimitive == null)
        {
            Destroy(previewSolid.gameObject);
            // TODO: no valid face
        }
        else
        {
            previewSolid.AlignToSelectedFace(selectedFace);
            previewSolid.ParentButton = ReadySolid.ParentButton;
        }
    }

    

    private void ConfirmOrCancelPreviewSolid(bool isConfirmed)
    {
        if (isConfirmed)
        {
            previewSolid.SetMode(Solid3D.MODE.DEFAULT);
            if (!selectedFace.ParentPrimitive.ParentSolid.isStatic) { previewSolid.ParentSolid = selectedFace.ParentPrimitive.ParentSolid; }
            if (selectedFace.ParentPrimitive.ParentSolid.IsPowered) { previewSolid.IsPowered = true; }

            selectedFace.ParentPrimitive.ParentSolid.Children.Add(previewSolid);
            placedSolids.Add(previewSolid);

            

            previewSolid.ParentButton.Count -= 1;
            if(previewSolid.ParentButton.Count == 0)
            {
                previewSolid = null; // break reference to newly placed solid so that setting ReadySolid to null doesn't destroy it
                ReadySolid = null;
            }

            previewSolid = null;
            selectedFace = null;


            Destroy(activeFaceSelector.gameObject);
            CheckWinConditions();

            
        }
        else
        {
            Destroy(previewSolid.gameObject);
        }
        previewSolid = null;
    }


    private bool CheckWinConditions()
    {
        foreach(IWinCondition winCondition in WinConditions)
        {
            if (!winCondition.IsFulfilled)
            {
                return false;
            }
        }
        StartCoroutine(MainCanvas.Current.LevelOverTransition());
        return true;
    }

    

    private void CreateFaceSelector(Face2D face)
    {
        if(activeFaceSelector != null)
        {
            Destroy(activeFaceSelector.gameObject);
        }
        switch (face.sides)
        {
            case 3:
                activeFaceSelector = Instantiate(CommonAssets.Current.selector3).GetComponent<FaceSelector>();
                break;

            case 4:
                activeFaceSelector = Instantiate(CommonAssets.Current.selector4).GetComponent<FaceSelector>();
                break;

            default:
                break;
        }

        activeFaceSelector.AlignSelectorToFace(face);
        // activeFaceSelector.transform.Translate(face.Normal.normalized * 0.05f, Space.World);
    }

    private void MarkSolidForDeletion(Solid3D solid)
    {
        UnmarkSolidForDeletion();
        solid.ToggleMarkForDelete(true, true);
        markedDeleteSolid = solid;
        foreach(Solid3D child in solid.Children){
            MarkSolidRecursive(child);
        }
    }

    private void MarkSolidRecursive(Solid3D solid)
    {
        markedDeleteSet.Add(solid);
        solid.ToggleMarkForDelete(true, false);
        foreach(Solid3D child in solid.Children)
        {
            MarkSolidRecursive(child);
        }
    }

    private void UnmarkSolidForDeletion()
    {
        if(markedDeleteSolid != null)
        {
            markedDeleteSolid.ToggleMarkForDelete(false, false);
            foreach(Solid3D child in markedDeleteSet)
            {
                child.ToggleMarkForDelete(false, false);
            }
            markedDeleteSet = new HashSet<Solid3D>();
            markedDeleteSolid = null;
        }
    }

    private void DeleteMarkedSolid()
    {
        markedDeleteSet.Add(markedDeleteSolid);
        foreach(Solid3D solid in markedDeleteSet)
        {
            placedSolids.Remove(solid);
            solid.ParentButton.Count += 1;
            Destroy(solid.gameObject);
        }
        markedDeleteSolid = null;
        markedDeleteSet = new HashSet<Solid3D>();
    }

   

    private void PowerRecursive(Solid3D solid, ref HashSet<Solid3D> checkedSolids)
    {
        solid.IsPowered = true;
        checkedSolids.Add(solid);
        if(solid.ParentSolid != null && !checkedSolids.Contains(solid.ParentSolid))
        {
            PowerRecursive(solid.ParentSolid, ref checkedSolids);
        }
        foreach(Solid3D child in solid.Children)
        {
            if (!checkedSolids.Contains(child))
            {
                PowerRecursive(child, ref checkedSolids);
            }
        }
    }

    public void UndoMove()
    {
        if(placedSolids.Count > 0)
        {
            placedSolids[placedSolids.Count - 1].ParentButton.Count += 1;
            Destroy(placedSolids[placedSolids.Count - 1].gameObject);
            placedSolids.RemoveAt(placedSolids.Count - 1);
        }

    }
}
