using UnityEngine;
using UnityEngine.UI;

public class SpaceShip : MonoBehaviour
{
    public int Health = 100;
    public int MaxHealth = 100; // Maximum health for percentage calculation
    public GameObject DestroyFxRocket; // Effects when destroyed
    public float blinkIntensity = 2f; // Intensity of the blink effect
    public float blinkDuration = 0.5f; // Duration of the blink effect
    private float blinkTimer; // Timer for blinking effect
    private MeshRenderer meshRenderer; // Mesh renderer for color manipulation

    // Reference to the UI Image for the health bar
    public Image HealthBarImage;

    public float speed = 1f; // Speed of the spaceship
    public GameObject rocketPrefab; // Prefab for the rocket

    private bool rocketDropped = false; // Flag to track if the rocket has been dropped
    private MainTower mainTower;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (HealthBarImage != null)
        {
            HealthBarImage.fillAmount = (float)Health / MaxHealth; // Initialize health bar
            mainTower = FindObjectOfType<MainTower>();
        }

        mainTower.AlartOn();
    }

    private void Update()
    {
        // Handle blinking effect
        if (blinkTimer > 0)
        {
            blinkTimer -= Time.deltaTime;
            float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
            float intensity = (lerp * blinkIntensity) + 1f;
            meshRenderer.material.color = Color.white * intensity;
        }
        else
        {
            meshRenderer.material.color = Color.white; // Reset to original color
        }

        // Update the health bar fill amount
        if (HealthBarImage != null)
        {
            HealthBarImage.fillAmount = (float)Health / MaxHealth;
        }

        // Destroy if health reaches 0
        if (Health <= 0)
        {
            mainTower.AlartOff();

            Instantiate(DestroyFxRocket, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        // Move the spaceship downwards
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Check if the spaceship is within the z-axis range
        if (transform.position.z >= 0 && !rocketDropped)
        {
            Instantiate(rocketPrefab, transform.position, Quaternion.Euler(-180, 0, 0));
            Debug.Log("Rocket Dropped");
            rocketDropped = true; // Prevent multiple drops
            mainTower.AlartOff();

        }
        // Destroy spaceship if it goes beyond the specified z-position
        if (transform.position.z >= 185)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Health--; // Reduce health
            blinkTimer = blinkDuration; // Start blink timer
        }
    }
}
