using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using UnityEngine.InputSystem.LowLevel;
using System;

public class PlayerBehaviour : MonoBehaviour
{
    private bool isGrounded = false;


    public float movementSpeed = 6f;
    public float jumpForce = 15f;
    public float friction = 10f;
    public bool normalize = false;
    public bool bHop = true;
    public GameObject arenaObj;
    public PhysicsMaterial physMaterial;
    

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == arenaObj)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == arenaObj)
        {
            isGrounded = false;
            if (bHop) physMaterial.dynamicFriction = 0f;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        if (input.magnitude > 0)
        {
            if (normalize)
            {
                input.Normalize();
            }
            if (isGrounded) rb.AddForce(yaw * input * movementSpeed); else rb.AddForce(yaw * input * movementSpeed / 2f);
        }

        if (bHop && isGrounded && physMaterial.dynamicFriction < friction)
        {
            physMaterial.dynamicFriction += friction * 3 * Time.deltaTime;
        }
        physMaterial.dynamicFriction = Mathf.Clamp(physMaterial.dynamicFriction, 0f, friction);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * 200);
        }

    }
}
