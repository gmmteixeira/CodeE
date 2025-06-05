using UnityEngine;
using UnityEngine.InputSystem;

public class PFCam : MonoBehaviour
{
    public float sensitivity = 3f;
    private float xRotation = 0f;
    private InputAction camControl;
    private Vector2 look;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camControl = InputSystem.actions.FindAction("look");
    }

    void Update()
    {
        look = camControl.ReadValue<Vector2>();

        float lookX = look.x * sensitivity;
        float lookY = look.y * sensitivity;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * lookX);

    }
}
