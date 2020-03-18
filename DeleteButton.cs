using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleDeleteMode()
    {
        LevelManager.Current.DeleteMode = !LevelManager.Current.DeleteMode;
    }
}
