using System;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandMovement : MonoBehaviour
{
    Rigidbody playerRB;
    private InputAction camControl;
    private Vector2 look;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRB = Game.staticVariables.player.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        camControl = InputSystem.actions.FindAction("look");
    }

    // Update is called once per frame
    void Update()
    {
        look = camControl.ReadValue<Vector2>();

        //this works
        transform.position = Vector3.Lerp(transform.position, transform.parent.position + Camera.main.transform.rotation * new Vector3(0, -0.8f, 0.2f) - playerRB.linearVelocity * 0.02f, Time.deltaTime * 10);

        //this doesnt work
        transform.localRotation = Quaternion.Lerp(transform.localRotation, quaternion.Euler(new Vector3(-look.y * 0.01f - 1.7f, 0, look.x * 0.01f)), Time.deltaTime * 10);
    }
}
