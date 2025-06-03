using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    public float speed;
    InputAction movement;
    private Vector3 force;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        float camYaw = Camera.main.transform.eulerAngles.y;
        Quaternion yaw = Quaternion.Euler(0, camYaw, 0);
        Vector2 moveValue = movement.ReadValue<Vector2>();
        force = yaw * new Vector3(moveValue.x, 0f, moveValue.y) * speed;

    }
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
    }
}
