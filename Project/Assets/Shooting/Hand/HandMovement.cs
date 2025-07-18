using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandMovement : MonoBehaviour
{
    public GameObject Player;
    public Material handMaterial;
    private InputAction camControl;
    private Vector2 look;
    private new Rigidbody rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = Player.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        camControl = InputSystem.actions.FindAction("look");
    }

    private void OnEnable()
    {
        WeaponEvents.OnWeaponFired += TriggerRecoil;
    }

    private void OnDisable()
    {
        WeaponEvents.OnWeaponFired -= TriggerRecoil;
    }

    private void TriggerRecoil()
    {
        transform.position -= Camera.main.transform.rotation * new Vector3(0, 0f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        look = camControl.ReadValue<Vector2>();

        transform.position = Vector3.Lerp(transform.position, transform.parent.position + Camera.main.transform.rotation * new Vector3(0, -0.8f, 0.2f) - rigidbody.linearVelocity * 0.02f, Time.deltaTime * 10);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, quaternion.Euler(new Vector3(-look.y * 0.01f - 1.5f, 0, look.x * 0.01f)), Time.deltaTime * 10);

        var world = World.DefaultGameObjectInjectionWorld;
        
        if (world != null)
        {
            var entityManager = world.EntityManager;
            try
            {
                if (entityManager.Exists(entityManager.CreateEntityQuery(typeof(WeaponProperties)).GetSingletonEntity()))
                {
                    // visual recoil
                    var weaponProps = entityManager.CreateEntityQuery(typeof(WeaponProperties)).GetSingleton<WeaponProperties>();
                    if (weaponProps.cooldownTimer > 0f)
                    {
                        transform.position -= Camera.main.transform.rotation * new Vector3(0, 0f, 0.005f);
                    }
                }
                if (entityManager.Exists(entityManager.CreateEntityQuery(typeof(WeaponProperties)).GetSingletonEntity()))
                {
                    var weaponProps = entityManager.CreateEntityQuery(typeof(WeaponProperties)).GetSingleton<WeaponProperties>();
                    int level = weaponProps.powerupLevel;
                    transform.position += new Vector3
                    (
                        UnityEngine.Random.Range(-0.001f, 0.001f) * Mathf.Pow(level, 2),
                        UnityEngine.Random.Range(-0.001f, 0.001f) * Mathf.Pow(level, 2),
                        UnityEngine.Random.Range(-0.001f, 0.001f) * Mathf.Pow(level, 2)

                    );
                    handMaterial.SetFloat("_Level", level);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}