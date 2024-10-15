using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatManNuke : MonoBehaviour
{
    public int Health = 8;
    public GameObject DestroyFxRocket;
    public GameObject DestroyFxBuilding;
    public float blinkIntensity;
    public float blinkDuration;
    public float fallSpeed = -5f; // Speed at which the nuke falls
    private float blinkTimer;
    private MeshRenderer meshRenderer;
    private Rigidbody rb; // Reference to Rigidbody

    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component

        // If Rigidbody is missing, add it
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    void Update()
    {
        // Handle blinking effect when hit
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) + 1f;
        meshRenderer.material.color = Color.white * intensity;
    }

    void FixedUpdate()
    {
        // Make the object fall at a specified speed
        rb.velocity = new Vector3(0, fallSpeed, 0); // Apply downward velocity

        if (Health <= 0)
        {
            Instantiate(DestroyFxRocket, transform.position, Quaternion.identity);
            Destroy(gameObject); // Destroy the nuke
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MainBuilding"))
        {
            Vector3 offset = new Vector3(0, 15, 0);
            Instantiate(DestroyFxBuilding, collision.transform.position + offset, Quaternion.identity);
            Destroy(collision.gameObject); // Destroy the building
            Destroy(gameObject);  // Destroy the rocket

            // Trigger Game Over
            GameOver gameOverScript = FindObjectOfType<GameOver>();
            gameOverScript.Game_Over();
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Health--;
            blinkTimer = blinkDuration; // Start blink effect
        }
    }
}
