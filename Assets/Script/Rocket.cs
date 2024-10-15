
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 10f;       // Rocket movement speed
    public float rotationSpeed = 5f; // How quickly the rocket turns towards its target
    private Transform target;
    public int Health = 3;
    public GameObject DestroyFxRocket;
    public GameObject DestroyFxBuilding;

    private Rigidbody rb;  // Rigidbody component
    public float blinkIntensity;
    public float blinkDuration;
    float blinkTimer;
    MeshRenderer meshRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Get the Rigidbody component
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        FindRandomTarget();
    }

    void Update()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) +1f;
        meshRenderer.material.color = Color.white * intensity;
    }
    void FixedUpdate()
    {
        if (target != null)
        {
            // Move towards the target
            Vector3 direction = (target.position - transform.position).normalized;

            // Rotate the rocket towards the target smoothly
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed));

            // Move the rocket forward
            rb.velocity = transform.forward * speed;
        }

        if (Health <= 0)
        {
            Instantiate(DestroyFxRocket, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void FindRandomTarget()
    {
        // Find all objects with the "Building" tag
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Building");

        if (targets.Length > 0)
        {
            // Select a random target from the array
            target = targets[Random.Range(0, targets.Length)].transform;
        }
        else
        {
            Debug.LogWarning("No targets found!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Building"))
        {
            MainShip.BuildingDestroyed++;
            Vector3 offset = new Vector3(0, 15, 0);
            Instantiate(DestroyFxBuilding, collision.transform.position + offset, Quaternion.identity);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Health--;
            blinkTimer = blinkDuration;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Health = 0;
        }
    }
}
