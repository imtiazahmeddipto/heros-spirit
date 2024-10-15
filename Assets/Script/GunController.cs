using UnityEngine;

public class GunController : MonoBehaviour
{
    public float fireRate = 15f;                    // Fire rate (rounds per second)
    public int fireSpeed = 10;                      // Bullet speed
    public Camera fpsCam;                           // FPS camera for aiming
    public ParticleSystem muzzleFlash;              // Muzzle flash effect
    public Transform firePoint;                     // Gun's fire point (where the bullet spawns)
    public GameObject bulletPrefab;                 // Bullet prefab

    private float nextTimeToFire = 0f;

    public AudioSource audioSource;   // Audio source for the shoot sound
    public AudioClip shootSFX;        // The sound effect for shooting

    // Update method to check for shooting input.
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate; // Control fire rate
            Shoot();
        }
    }

    // Shoot method to handle bullet instantiation and firing.
    void Shoot()
    {
        // Play the shooting sound effect
        if (audioSource != null && shootSFX != null)
        {
            audioSource.PlayOneShot(shootSFX);
        }

        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Raycast from the camera to detect where the player is aiming
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, Mathf.Infinity))
        {
            // If we hit something, the bullet will aim towards the hit point
            targetPoint = hit.point;
        }
        else
        {
            // If nothing is hit, the bullet will travel forward to the maximum range
            targetPoint = fpsCam.transform.position + fpsCam.transform.forward * 1000f;
        }

        // Calculate direction from firePoint to targetPoint
        Vector3 direction = targetPoint - firePoint.position;

        // Instantiate the bullet and set its velocity
        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * fireSpeed; // Adjust bullet speed as needed
        }
    }
}
