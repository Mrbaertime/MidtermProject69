using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputReader : MonoBehaviour
{
    private NEW_Input inputActions;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [SerializeField] private float minSwipeDistance = 50f;

    [SerializeField] private PlayerController playerMovement;

    [SerializeField] private Transform swipeTrailOBJ;
    [SerializeField] private TrailRenderer swipeTrail;
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        inputActions = new NEW_Input();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (swipeTrail != null)
            swipeTrail.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Touch.PrimaryContact.started += OnTouchStarted;
        inputActions.Touch.PrimaryContact.canceled += OnTouchEnded;
    }

    private void OnDisable()
    {
        inputActions.Touch.PrimaryContact.started -= OnTouchStarted;
        inputActions.Touch.PrimaryContact.canceled -= OnTouchEnded;

        inputActions.Disable();
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        startTouchPosition = inputActions.Touch.PrimaryPosition.ReadValue<Vector2>();

        if (swipeTrail != null)
            swipeTrail.Clear();

        if (swipeTrailOBJ != null)
        {
            Vector3 startWorld = ScreenToWorldPoint(startTouchPosition);
            swipeTrailOBJ.position = startWorld;
            swipeTrailOBJ.gameObject.SetActive(true);
        }
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        endTouchPosition = inputActions.Touch.PrimaryPosition.ReadValue<Vector2>();

        if (swipeTrailOBJ != null)
            swipeTrailOBJ.gameObject.SetActive(false);

        DetectSwipe();
    }

    private void DetectSwipe()
    {
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

        if (swipeDelta.magnitude < minSwipeDistance)
        {
            Debug.Log("Swipe too short");
            playerMovement.SwipeStop();
            return;
        }

        // แยกแกน X/Y
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            // ซ้าย-ขวา
            if (swipeDelta.x > 0)
            {
                Debug.Log("Swipe Right");
                playerMovement.SwipeMoveRight();
            }
            else
            {
                Debug.Log("Swipe Left");
                playerMovement.SwipeMoveLeft();
            }
        }
        else
        {
            // บน-ล่าง (Topdown)
            if (swipeDelta.y > 0)
            {
                Debug.Log("Swipe Up");
                playerMovement.SwipeMoveUp();
            }
            else
            {
                Debug.Log("Swipe Down");
                playerMovement.SwipeMoveDown();
            }
        }
    }

    private Vector3 ScreenToWorldPoint(Vector2 screenPosition)
    {
        Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, 10f);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        worldPoint.z = 0f;
        return worldPoint;
    }

    private void Update()
    {
        bool isTouching = inputActions.Touch.PrimaryContact.IsPressed();

        if (isTouching && swipeTrailOBJ != null && swipeTrailOBJ.gameObject.activeSelf)
        {
            Vector2 currentTouchPosition = inputActions.Touch.PrimaryPosition.ReadValue<Vector2>();
            Vector3 currentWorld = ScreenToWorldPoint(currentTouchPosition);

            swipeTrailOBJ.position = currentWorld;
        }
    }
}
