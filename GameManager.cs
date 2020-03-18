using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Current { get; private set; }
    private const string MAIN_MENU_SCENE_PATH = "Assets/Scenes/MainMenu.unity";

    public class LevelMetadata
    {
        public int LevelNumber { get; set; }
        public bool IsCleared { get; set; }
        public string PathToScene { get; set; }

        public LevelMetadata(int levelNumber, bool isCleared, string pathToScene)
        {
            LevelNumber = levelNumber;
            IsCleared = isCleared;
            PathToScene = pathToScene;
        }
    }

    public static int CurrentLevel { get; set; }

    // TODO: this should be in a save file
    public static readonly LevelMetadata[] LevelList = new LevelMetadata[]
    {
        new LevelMetadata(1, false, "Assets/Scenes/Level_1.unity"),
        new LevelMetadata(2, false, "Assets/Scenes/Level_2.unity")
    };

    private void Awake()
    {
        if(Current == null)
        {
            Current = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static IEnumerator LoadLevelAsync(int levelNumber)
    {
        AsyncOperation sceneLoader = SceneManager.LoadSceneAsync(LevelList[levelNumber - 1].PathToScene);
        while (!sceneLoader.isDone)
        {
            yield return null;
        }
        CurrentLevel = levelNumber;
    }

    public static IEnumerator LoadMainMenu()
    {
        AsyncOperation sceneLoader = SceneManager.LoadSceneAsync(MAIN_MENU_SCENE_PATH);
        while (!sceneLoader.isDone)
        {
            yield return null;
        }
        CurrentLevel = 0;
    }
}
