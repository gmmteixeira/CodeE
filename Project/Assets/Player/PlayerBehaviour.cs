using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public float speed = 6f;
    public ScriptableRendererFeature deathEffectFeature;
    private InputAction movement;
    private Rigidbody rb;
    private Quaternion yaw;
    private Vector2 move;
    bool isAlive = true;



    private EntityManager entityManager;
    private Entity playerEntity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        deathEffectFeature.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerEvents.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        isAlive = false;
        move = Vector2.zero;
        deathEffectFeature.SetActive(true);
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerDeath -= OnPlayerDeath;
    }

    void Update()
    {

        // Update the LocalTransform component with the current position
        if (playerEntity != Entity.Null)
        {
            if (entityManager.HasComponent<LocalTransform>(playerEntity))
            {
                var localTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
                var playerData = entityManager.GetComponentData<PlayerSingletonData>(playerEntity);
                if (playerData.isAlive)
                {
                    float camYaw = Camera.main.transform.eulerAngles.y;
                    yaw = Quaternion.Euler(0, camYaw, 0);

                    movement = InputSystem.actions.FindAction("move");
                    move = movement.ReadValue<Vector2>();

                    if (localTransform.Position.y < -1f) playerData.isAlive = false;
                }
                else if (isAlive)
                {
                    PlayerEvents.PlayerDeath();
                }

                localTransform.Position = transform.position;
                entityManager.SetComponentData(playerEntity, localTransform);
                entityManager.SetComponentData(playerEntity, playerData);
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }

    void FixedUpdate()
    {
        if (rb != null) rb.AddForce(yaw * new Vector3(move.x, 0, move.y) * speed);
    } 
}