using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Include the UI namespace

public class PowerCell : MonoBehaviour
{
    public int Health = 100;
    public int MaxHealth = 100; // Keep track of the maximum health for percentage calculation
    public GameObject DestroyFxRocket;
    public float blinkIntensity;
    public float blinkDuration;
    private float blinkTimer;
    private MeshRenderer meshRenderer;

    // Reference to the UI Image
    public Image HealthBarImage;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // Handle blinking effect
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) + 1f;
        meshRenderer.material.color = Color.white * intensity;

        // Update the health bar fill amount
        if (HealthBarImage != null)
        {
            HealthBarImage.fillAmount = (float)Health / MaxHealth;
        }

        // Destroy if health reaches 0
        if (Health <= 0)
        {
            MainShip.PowerDestroyed ++;
            Instantiate(DestroyFxRocket, transform.position, Quaternion.identity);
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
