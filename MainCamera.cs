using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera Current { get; set; }

    public static class INPUT
    {
        public const int DRAG_THRESHOLD = 400; // squared
        public const float MAX_LOOK_DISTANCE = 20f;
        public const float MIN_LOOK_DISTANCE = 6f;
        public const float ZOOM_SPEED = 0.001f;
        public const float ROTATION_SPEED = 0.001f;
    }

    private bool isDragging;
    private Vector2 touchStart;
    private float initialTouchDistance;
    private Camera mainCamera;

    // Mouse only
    Vector3 mousePosition;
    Vector3 lastMousePosition;

    private bool isCameraLocked = true;
    private float lookDistance;
    private bool secondTouchInitiated = false;

    private Vector3 lookTarget;
    public Vector3 LookTarget
    {
        get
        {
            return lookTarget;
        }
        set
        {
            if(lookTarget != value)
            {
                lookTarget = value;
                if (!isCameraLocked)
                {
                    isCameraLocked = true;
                    StartCoroutine(SwitchLookTargets(lookTarget));
                }
            }
            print(lookTarget);
        }
    }

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        LookTarget = Vector3.zero;
        Current = this;
    }

    void Start()
    {
        StartCoroutine(LevelIntro());
    }

    // Update is called once per frame
    void Update()
    {
        if (false)
        {
            if (!isCameraLocked)
            {
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);

                    if (secondTouchInitiated)
                    {
                        touchStart = touch.position;
                        secondTouchInitiated = false;
                    }

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            touchStart = touch.position;
                            break;

                        case TouchPhase.Moved:
                            if((touch.position - touchStart).sqrMagnitude > INPUT.DRAG_THRESHOLD)
                            {
                                isDragging = true;
                            }
                            if (isDragging)
                            {
                                transform.Translate((touchStart - touch.position) * lookDistance * INPUT.ROTATION_SPEED, Space.Self);
                                touchStart = touch.position; 
                            }
                            break;

                        case TouchPhase.Ended:
                            if (isDragging)
                            {

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
                            isDragging = false;
                            break;
                    }

                    
                }
                else if (Input.touchCount == 2)
                {
                    float touchDistance = (Input.GetTouch(1).position - Input.GetTouch(0).position).magnitude;
                    if (!secondTouchInitiated)
                    {
                        initialTouchDistance = touchDistance;
                        isDragging = true;
                        secondTouchInitiated = true;
                    }
                    lookDistance = Mathf.Clamp(lookDistance + ((touchDistance - initialTouchDistance) * INPUT.ZOOM_SPEED), INPUT.MIN_LOOK_DISTANCE, INPUT.MAX_LOOK_DISTANCE);
                }
                if (!isCameraLocked)
                {
                    transform.LookAt(LookTarget);
                    transform.position = transform.position.normalized * lookDistance; 
                }
            }
        }
        else
        {
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

                if (!isCameraLocked)
                {
                    transform.LookAt(LookTarget);
                    transform.position = transform.position.normalized * 12;
                }
            }
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
        lookDistance = transform.position.magnitude;
    }

    private IEnumerator SwitchLookTargets(Vector3 newLookTarget)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.LookRotation(newLookTarget - transform.position, Vector3.up);
        float time = 0f;
        while(time <= 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, COMMON.MATHFUNCTIONS.SquaredSmooth(0, 1, time));
            time += 0.03f;
            yield return null;
        }
        isCameraLocked = false;
    }
}
