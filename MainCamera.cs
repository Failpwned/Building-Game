using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private bool isDragging;
    Vector2 touchStart;
    Camera mainCamera;

    // Mouse only
    Vector3 mousePosition;
    Vector3 lastMousePosition;

    bool isCameraLocked = true;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        StartCoroutine(LevelIntro());
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            print("WE HAS A TOUCH");
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    break;

                case TouchPhase.Moved:
                    isDragging = true;
                    break;

                case TouchPhase.Ended:
                    if (isDragging)
                    {

                    }
                    else
                    {
                        Ray ray = mainCamera.ScreenPointToRay(touch.position);
                        RaycastHit hit;
                        if(Physics.Raycast(ray, out hit))
                        {
                            print("AWEFW");
                        }
                    }
                    isDragging = false;
                    break;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            print("AWEFWE");
        }
        */
        if (!isCameraLocked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mousePosition = Input.mousePosition;
                lastMousePosition = Input.mousePosition;
            }



            if (Input.GetMouseButton(0))
            {
                if (isDragging)
                {
                    transform.Translate((Input.mousePosition - lastMousePosition).normalized * 0.1f, Space.Self);
                    lastMousePosition = Input.mousePosition;
                }
                else
                {
                    if (Vector3.Distance(Input.mousePosition, mousePosition) > 10)
                    {
                        isDragging = true;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && Vector3.Distance(Input.mousePosition, mousePosition) < 10)
            {
                if (isDragging)
                {
                    isDragging = false;
                }
                else
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        Primitive3D hitPrimitive = hit.collider.gameObject.GetComponent<CollisionHandler>().ParentPrimitive;
                        LevelManager.Current.SelectFace(hitPrimitive.GetFaceByNormal(hit.normal));
                    }
                }
            }


            transform.LookAt(Vector3.zero);
            transform.position = transform.position.normalized * 12;
        }
    }

    private IEnumerator LevelIntro()
    {
        transform.LookAt(Vector3.zero);
        float startY = transform.position.y + 10;
        float endY = transform.position.y;
        float time = 0;
        while(time <= 1)
        {
            transform.position = new Vector3(transform.position.x, COMMON.MATHFUNCTIONS.SquaredSmooth(startY, endY, time), transform.position.z);
            time += 0.01f;
            yield return null;
        }
        isCameraLocked = false;
    }
}
