using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject LevelSelectLayout;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartButtonClick()
    {
        GetComponent<CanvasGroup>().interactable = false;
        GameManager.Current.StartCoroutine(LayoutTransition(LevelSelectLayout));
    }

    private IEnumerator LayoutTransition(GameObject newLayout)
    {
        float canvasWidth = transform.parent.gameObject.GetComponent<RectTransform>().rect.width;

        RectTransform currentRectTransform = GetComponent<RectTransform>();
        float oldStartPoint = currentRectTransform.anchoredPosition.x;
        float oldEndPoint = currentRectTransform.anchoredPosition.x - canvasWidth;

        GameObject newLayoutObject = Instantiate(newLayout, transform.parent);
        newLayoutObject.GetComponent<CanvasGroup>().interactable = false;
        RectTransform newRectTransform = newLayoutObject.GetComponent<RectTransform>();
        float newStartPoint = canvasWidth;
        newRectTransform.anchoredPosition = new Vector2(newStartPoint, 0);

        float time = 0f;
        while(time <= 1)
        {
            currentRectTransform.anchoredPosition = new Vector2(COMMON.MATHFUNCTIONS.SquaredSmooth(oldStartPoint, oldEndPoint, time), 0);
            newRectTransform.anchoredPosition = new Vector2(COMMON.MATHFUNCTIONS.SquaredSmooth(newStartPoint, 0, time), 0);
            time += 0.01f;
            yield return null;
        }
        newLayoutObject.GetComponent<CanvasGroup>().interactable = true;
        Destroy(gameObject);
    }

    
}
