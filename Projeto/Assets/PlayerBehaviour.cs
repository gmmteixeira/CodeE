using Unity.Entities;
using UnityEngine;
using Unity.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    public float speed = 6f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var ecb = new EntityCommandBuffer(Allocator.Persistent);

        float camYaw = Camera.main.transform.eulerAngles.y;
        Quaternion yaw = Quaternion.Euler(0, camYaw, 0);

        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) input += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) input += Vector3.back;
        if (Input.GetKey(KeyCode.A)) input += Vector3.left;
        if (Input.GetKey(KeyCode.D)) input += Vector3.right;

        if (input.magnitude > 0) rb.AddForce(yaw * input * speed);
    }
}
