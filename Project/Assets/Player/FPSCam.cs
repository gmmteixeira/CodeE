using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCam : MonoBehaviour
{
    public float sensitivity;
    private float xRotation = 0f;
    private InputAction camControl;
    private Vector2 look;

    private EntityManager entityManager;
    private Entity playerEntity;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.5f)/1.5f;

        Cursor.lockState = CursorLockMode.Locked;
        camControl = InputSystem.actions.FindAction("look");
    }

    void Update()
    {
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