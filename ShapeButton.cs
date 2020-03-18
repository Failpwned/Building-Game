using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeButton : MonoBehaviour
{
    public const string LAYOUT_TAG = "ButtonLayout";
    public static ShapeButton SelectedButton { get; set; }

    public Sprite SelectedSprite;
    public Solid3D solid;
    public int solidCount;

    private Sprite defaultSprite;

    Text text;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
        Count = solidCount;
        defaultSprite = GetComponent<Image>().sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        solid.ParentButton = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SelectSolid()
    {
        // GetComponent<Button>().Select();
        if(SelectedButton == this)
        {
            SelectedButton = null;
            LevelManager.Current.ReadySolid = null;
            GetComponent<Image>().sprite = defaultSprite;
        }
        else
        {
            SelectedButton = this;
            LevelManager.Current.ReadySolid = solid;
            LevelManager.Current.DeleteMode = false;
            GetComponent<Image>().sprite = SelectedSprite;
        }
    }

    private int count;
    public int Count
    {
        get { return count; }
        set
        {
            count = value;
            text.text = count.ToString();
            if (count == 0)
            {
                GetComponent<Button>().enabled = false;
            }
            else
            {
                GetComponent<Button>().enabled = true;
            }
        }
    }
}
