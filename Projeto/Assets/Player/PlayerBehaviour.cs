using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class PlayerBehaviour : MonoBehaviour
{
    public float speed = 6f;
    private InputAction movement;
    private Rigidbody rb;
    private Quaternion yaw;
    private Vector2 move;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        float camYaw = Camera.main.transform.eulerAngles.y;
        yaw = Quaternion.Euler(0, camYaw, 0);
        
        movement = InputSystem.actions.FindAction("move");
        move = movement.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (rb != null) rb.AddForce(yaw * new Vector3(move.x, 0, move.y) * speed);
    }
}
