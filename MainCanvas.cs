using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Current { get; private set; }

    public GameObject LevelOverLayout;

    // Start is called before the first frame update
    void Start()
    {
        Current = this;
        LevelOverLayout.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LevelOverTransition()
    {
        GetComponent<CanvasGroup>().interactable = false;
        LevelOverLayout.SetActive(true);
        RectTransform LolRectTransform = LevelOverLayout.GetComponent<RectTransform>();
        float startPoint = GetComponent<RectTransform>().rect.height;
        float endPoint = 0;
        float time = 0;

        while(time <= 1)
        {
            LolRectTransform.anchoredPosition = new Vector2(0, COMMON.MATHFUNCTIONS.SquaredSmooth(startPoint, endPoint, time));
            time += 0.03f;
            yield return null;
        }

        LevelOverLayout.GetComponent<CanvasGroup>().interactable = true;
        
        
    }

    public void NextLevel()
    {
        GameManager.Current.StartCoroutine(GameManager.LoadLevelAsync(GameManager.CurrentLevel + 1));
    }
    
    public void GoToMainMenu()
    {
        GameManager.Current.StartCoroutine(GameManager.LoadMainMenu());
    }
}
