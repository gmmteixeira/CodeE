using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class FPSCam : MonoBehaviour
{
    public float sensitivity;
    public ScriptableRendererFeature impactFeature;
    private float impactTime = 0f;
    private float xRotation = 0f;
    private InputAction camControl;
    private Vector2 look;

    private EntityManager entityManager;
    private Entity playerEntity;
    private float camShakeTime = 0f;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.5f)/1.5f;

        Cursor.lockState = CursorLockMode.Locked;
        camControl = InputSystem.actions.FindAction("look");
    }
    private void OnEnable()
    {
        WeaponEvents.OnWeaponAltFired += laserFired;
    }
    private void OnDisable()
    {
        WeaponEvents.OnWeaponAltFired -= laserFired;
    }
    private void laserFired()
    {
        camShakeTime = 0.3f;
        impactTime = 0.1f;
        impactFeature.SetActive(true);
    }

    void Update()
    {

        transform.position = Vector3.Lerp(transform.position, transform.parent.TransformPoint(new Vector3(0, 0.5f, 0)), Time.deltaTime * 50);
        if (camShakeTime > 0f)
        {
            camShakeTime -= Time.deltaTime;
            transform.position -= Camera.main.transform.rotation * new Vector3(
            Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
        }
        impactTime -= Time.deltaTime;
        if (impactTime <= 0)
        {
            impactFeature.SetActive(false);
        }

        if (playerEntity != Entity.Null)
        {
            if (entityManager.HasComponent<PlayerSingletonData>(playerEntity))
            {

                if (entityManager.GetComponentData<PlayerSingletonData>(playerEntity).isAlive)
                {
                    look = camControl.ReadValue<Vector2>();

                    float lookX = look.x * sensitivity;
                    float lookY = look.y * sensitivity;

                    xRotation -= lookY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                    transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                    transform.parent.Rotate(Vector3.up * lookX);

                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
        else
        {
            using (var query = entityManager.CreateEntityQuery(typeof(PlayerSingletonData)))
            {
                if (query.CalculateEntityCount() > 0)
                {
                    playerEntity = query.GetSingletonEntity();
                }
            }
        }
        
    }
}