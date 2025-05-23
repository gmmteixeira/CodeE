using UnityEngine;

public class ThingyScript : MonoBehaviour
{
    public bool isGrounded = false;
    public float movementSpeed = 6f;
    public float jumpForce = 15f;
    public bool normalize = false;
    public bool bHop = true;

    public PhysicsMaterial physMaterial;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Cube")
        {
            isGrounded = true;
            movementSpeed = 10f;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Cube")
        {
            isGrounded = false;
            movementSpeed = 3f;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
            rb.AddForce(yaw * input * movementSpeed);
        }

        if (bHop && isGrounded && physMaterial.dynamicFriction < 7f)
        {
            physMaterial.dynamicFriction += 30f * Time.deltaTime;
        } 
        physMaterial.dynamicFriction = Mathf.Clamp(physMaterial.dynamicFriction, 0f, 7f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * 200);
            if (bHop) physMaterial.dynamicFriction = 0f;
        }
        
    }
}
