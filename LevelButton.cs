using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelButton : MonoBehaviour
{
    public int levelNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel()
    {
        GameManager.Current.StartCoroutine(GameManager.LoadLevelAsync(levelNumber));
    }

    
}
