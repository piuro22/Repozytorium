using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [SerializeField] private float longTapDuration;


    [SerializeField] private float swipeRightAngle = 337.5f;
    [SerializeField] private float swipeUpperRightAngle = 22.5f;
    [SerializeField] private float swipeUpAngle = 67.5f;
    [SerializeField] private float swipeUpperLeftAngle = 112.5f;
    [SerializeField] private float swipeLeftAngle = 157.5f;
    [SerializeField] private float swipeLowerLeftAngle = 202.5f;
    [SerializeField] private float swipeDownAngle = 247.5f;
    [SerializeField] private float swipeLowerRightAngle = 292.5f;

    public event Action<Vector2> OnSwipeLeft;
    public event Action<Vector2> OnSwipeRight;
    public event Action<Vector2> OnSwipeUp;
    public event Action<Vector2> OnSwipeDown;
    public event Action<Vector2> OnSwipeUpperLeft;
    public event Action<Vector2> OnSwipeUpperRight;
    public event Action<Vector2> OnSwipeLowerLeft;
    public event Action<Vector2> OnSwipeLowerRight;
    public event Action<Vector2> OnTap;
    public event Action<Vector2> OnLongTap;
    public event Action<Vector2> OnDoubleTap;
    public event Action<Vector2> OnDrag;
    public event Action<Vector2> OnDragEnd;

    private bool isDragging = false;
    private bool isLongTap = false;
    private Vector2 startTouch;
    private Vector2 swipeDelta;
    private int tapCount = 0;
    private float lastTapTime = 0f;
    private float doubleTapInterval = 0.3f;
    private float tripleTapInterval = 0.3f;
    private float currentTapDuration;
    private bool isSwiped = false;

    private void Update()
    {
        HandleTouchInput();
        HandleSwipe();
        HandleDrag();
        if (isDragging)
        {
            currentTapDuration += Time.deltaTime;
            if (currentTapDuration >= longTapDuration && isLongTap)
            {
                OnLongTap?.Invoke(startTouch);
                ResetSwipe();
            }
        }
    }

    private void HandleTouchInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSwipe(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            if (isDragging)
            {

                currentTapDuration += Time.deltaTime;

                if (currentTapDuration >= longTapDuration && isLongTap)
                {
                    return;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (currentTapDuration < longTapDuration)
            {
                EndSwipe();
            }
            else if (isLongTap)
            {
                OnLongTap?.Invoke(startTouch);
            }
        }
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButton(0) && currentTapDuration > 0.3f)
        {
            OnDrag?.Invoke(swipeDelta);
        }
        if (Input.GetMouseButtonUp(0) && currentTapDuration > 0.3f)
        {
            OnDragEnd?.Invoke(swipeDelta);
        }
    }


    private void StartSwipe(Vector2 startPosition)
    {
        currentTapDuration = 0;
        isDragging = true;
        startTouch = startPosition;
        isLongTap = true;
        tapCount++;
        isSwiped = false; // Initialize the swipe flag to false

        if (tapCount == 2 && Time.time - lastTapTime < doubleTapInterval)
        {
            OnDoubleTap?.Invoke(startTouch);
            tapCount = 0;
        }
        else if (tapCount >= 3 || Time.time - lastTapTime > tripleTapInterval)
        {
            tapCount = 1;
        }

        lastTapTime = Time.time;
    }

    private void EndSwipe()
    {
        if (isLongTap)
        {
            if (currentTapDuration < longTapDuration)
            {
                if (tapCount == 1)
                {
                    StartCoroutine(DelayedTapInvocation());
                }
                else if (tapCount == 2)
                {
                    OnDoubleTap?.Invoke(startTouch);
                }
            }
            else if (tapCount < 2)
            {
                OnLongTap?.Invoke(startTouch);
            }
        }
        else
        {
            if (tapCount == 1 && !isSwiped)
            {
                StartCoroutine(DelayedTapInvocation());
            }
            else if (tapCount == 2)
            {
                OnDoubleTap?.Invoke(startTouch);
            }
        }

        ResetSwipe();
    }

    private System.Collections.IEnumerator DelayedTapInvocation()
    {
        yield return new WaitForSeconds(0.1f);

        if (tapCount == 1 && currentTapDuration < longTapDuration)
        {
            OnTap?.Invoke(startTouch);
        }
    }
    public float lastAngle;
    private void HandleSwipe()
    {
        if (isDragging)
        {
            if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
            else if (Input.touches.Length > 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
        }

        if (swipeDelta.magnitude >= 125f)
        {
            isSwiped = true;
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            if (angle < 0)
            {
                angle += 360;
            }
            lastAngle = angle;

            if (angle >= swipeRightAngle || angle < swipeUpperRightAngle)
            {
                OnSwipeRight?.Invoke(startTouch);
            }
            else if (angle >= swipeUpperRightAngle && angle < swipeUpAngle)
            {
                OnSwipeUpperRight?.Invoke(startTouch);
            }
            else if (angle >= swipeUpAngle && angle < swipeUpperLeftAngle)
            {
                OnSwipeUp?.Invoke(startTouch);
            }
            else if (angle >= swipeUpperLeftAngle && angle < swipeLeftAngle)
            {
                OnSwipeUpperLeft?.Invoke(startTouch);
            }
            else if (angle >= swipeLeftAngle && angle < swipeLowerLeftAngle)
            {
                OnSwipeLeft?.Invoke(startTouch);
            }
            else if ((angle >= swipeLowerLeftAngle && angle < swipeDownAngle))
            {
                OnSwipeLowerLeft?.Invoke(startTouch);
            }
            else if ((angle >= swipeDownAngle && angle < swipeLowerRightAngle))
            {
                OnSwipeDown?.Invoke(startTouch);
            }
            else if (angle >= swipeLowerRightAngle && angle < swipeRightAngle)
            {
                OnSwipeLowerRight?.Invoke(startTouch);
            }

            ResetSwipe();
        }
    }

    private void ResetSwipe()
    {
        isDragging = false;
        isLongTap = false;
        startTouch = swipeDelta = Vector2.zero;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw swipe angle arcs
        Vector3 position = transform.position;
        float radius = 1.0f;
        float arcWidth = 1f;

        Handles.color = Color.green;
        DrawArc(position, radius, swipeRightAngle, swipeUpperRightAngle, arcWidth);

        Handles.color = Color.cyan;
        DrawArc(position, radius, swipeUpperRightAngle, swipeUpAngle, arcWidth);

        Handles.color = Color.blue;
        DrawArc(position, radius, swipeUpAngle, swipeUpperLeftAngle, arcWidth);

        Handles.color = Color.magenta;
        DrawArc(position, radius, swipeUpperLeftAngle, swipeLeftAngle, arcWidth);

        Handles.color = Color.red;
        DrawArc(position, radius, swipeLeftAngle, swipeLowerLeftAngle, arcWidth);

        Handles.color = Color.yellow;
        DrawArc(position, radius, swipeLowerLeftAngle, swipeDownAngle, arcWidth);

        Handles.color = Color.grey;
        DrawArc(position, radius, swipeDownAngle, swipeLowerRightAngle, arcWidth);

        Handles.color = Color.white;
        DrawArc(position, radius, swipeLowerRightAngle, swipeRightAngle, arcWidth);


        Handles.DrawWireCube(position, new Vector2(4, 2));
    }
    private void DrawArc(Vector3 position, float radius, float startAngle, float endAngle, float arcWidth)
    {
        int segments = 60;
        float angleStep = (endAngle - startAngle) / segments;
        float currentAngle = startAngle;

        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float x = position.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
            float y = position.y + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;
            Vector3 currentPoint = new Vector3(x, y, position.z);

            if (i > 0)
            {
                Handles.DrawSolidArc(position, Vector3.forward, prevPoint - position, angleStep, arcWidth);
            }

            prevPoint = currentPoint;
            currentAngle += angleStep;
        }
    }
#endif
}

