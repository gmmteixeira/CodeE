using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public float aceleration;
    public float speed;
    public float jumpForce;
    public float jumpBoost;
    public ScriptableRendererFeature deathEffectFeature;
    public PhysicsMaterial physicsMaterial;
    public float targetFriction;
    private InputAction movementAction;
    private InputAction jumpAction;
    private InputAction quitAction;
    private Rigidbody rb;
    private AudioSource audioSource;
    private Quaternion yaw;
    private Vector2 move;
    private bool isAlive = true;
    public bool floored = false;


    private EntityManager entityManager;
    private Entity playerEntity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        deathEffectFeature.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            floored = true;
            audioSource.Play();
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            floored = false;
        }
    }

    private void OnEnable()
    {
        PlayerEvents.OnPlayerDeath += OnPlayerDeath;
        WeaponEvents.OnWeaponAltFired += AltFired;
    }

    private void AltFired()
    {
        if (isAlive && Vector3.Dot(Camera.main.transform.forward.normalized, Vector3.down) > 0.9f)
        {
            rb.AddForce(transform.up * 20f, ForceMode.Impulse);
        }
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
        WeaponEvents.OnWeaponAltFired -= AltFired;
    }

    void Update()
    {
        if (floored)
        {
            physicsMaterial.dynamicFriction += 1.5f * Time.deltaTime;
            physicsMaterial.dynamicFriction = Mathf.Clamp(physicsMaterial.dynamicFriction, 0f, targetFriction);
        }
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

                        movementAction = InputSystem.actions.FindAction("move");
                        jumpAction = InputSystem.actions.FindAction("jump");
                        move = movementAction.ReadValue<Vector2>();

                        // Use button press interaction for jump
                        if (jumpAction.triggered && floored)
                        {
                            floored = false;
                            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                            rb.AddForce(yaw * new Vector3(move.x, 0, move.y) * jumpBoost, ForceMode.Impulse);
                            physicsMaterial.dynamicFriction = 0f;
                        }

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

        
        quitAction = InputSystem.actions.FindAction("quit");
        if (quitAction.triggered)
        {
            SceneManager.LoadScene("MainMenu");
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector3 inputDir = yaw * new Vector3(move.x, 0, move.y);

            if (floored)
            {
                Vector3 desiredVelocity = inputDir * speed;
                Vector3 velocityChange = desiredVelocity - rb.linearVelocity;
                velocityChange.y = 0;
                rb.AddForce(velocityChange * aceleration, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(inputDir * speed/2, ForceMode.Acceleration);
            }
        }
    } 
}