using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    public float speed = 6f;
    private InputAction movement;
    private Rigidbody rb;
    private Quaternion yaw;
    private Vector2 move;

    public EntityManager entityManager;
    public Entity playerPositionEntity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype archetype = entityManager.CreateArchetype(typeof(PlayerPosition));
        playerPositionEntity = entityManager.CreateEntity(archetype);
        
        entityManager.AddComponentData(playerPositionEntity, new PlayerPosition
        {
            vector3 = transform.position
        });
    }

    // Update is called once per frame
    void Update()
    {
        float camYaw = Camera.main.transform.eulerAngles.y;
        yaw = Quaternion.Euler(0, camYaw, 0);

        movement = InputSystem.actions.FindAction("move");
        move = movement.ReadValue<Vector2>();

        entityManager.SetComponentData(playerPositionEntity, new PlayerPosition
        {
            vector3 = transform.position
        });
    }

    void FixedUpdate()
    {
        if (rb != null) rb.AddForce(yaw * new Vector3(move.x, 0, move.y) * speed);
    }
}