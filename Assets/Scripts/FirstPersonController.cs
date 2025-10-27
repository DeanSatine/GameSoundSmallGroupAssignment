using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.1f;
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float rotationX = 0f;
    private float bobTimer = 0f;
    private Vector3 cameraStartPosition;
    private bool canMove = true;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
        {
            cameraStartPosition = cameraTransform.localPosition;
        }
    }

    void Update()
    {
        if (canMove)
        {
            HandleMovement();
            HandleCameraRotation();
            HandleCameraBob();
        }
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * walkSpeed * Time.deltaTime);
        controller.Move(Vector3.down * 9.81f * Time.deltaTime);
    }

    void HandleCameraRotation()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCameraBob()
    {
        if (moveInput.magnitude > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmount;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * bobAmount * 0.5f;

            cameraTransform.localPosition = cameraStartPosition + new Vector3(bobOffsetX, bobOffsetY, 0f);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraStartPosition, Time.deltaTime * bobSpeed);
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
}
