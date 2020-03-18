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
        RectTransform currentRectTransform = GetComponent<RectTransform>();
        float oldStartPoint = currentRectTransform.anchoredPosition.x;
        float oldEndPoint = currentRectTransform.anchoredPosition.x - currentRectTransform.rect.width;

        GameObject newLayoutObject = Instantiate(newLayout);
        newLayout.GetComponent<CanvasGroup>().interactable = false;
        newLayoutObject.transform.SetParent(transform.parent);
        RectTransform newRectTransform = newLayoutObject.GetComponent<RectTransform>();
        float newStartPoint = newRectTransform.rect.width;
        newRectTransform.anchoredPosition = new Vector2(newStartPoint, 0);

        float time = 0f;
        while(time <= 1)
        {
            currentRectTransform.anchoredPosition = new Vector2(COMMON.MATHFUNCTIONS.SquaredSmooth(oldStartPoint, oldEndPoint, time), 0);
            newRectTransform.anchoredPosition = new Vector2(COMMON.MATHFUNCTIONS.SquaredSmooth(newStartPoint, 0, time), 0);
            time += 0.01f;
            yield return null;
        }
        newLayout.GetComponent<CanvasGroup>().interactable = true;
        Destroy(gameObject);
    }

    
}
