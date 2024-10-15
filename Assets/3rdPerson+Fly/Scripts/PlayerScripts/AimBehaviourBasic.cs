using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Audio;

public class AimBehaviourBasic : GenericBehaviour
{
    public string aimButton = "Aim", shoulderButton = "Aim Shoulder";     // Default aim and switch shoulders buttons.
    public Texture2D crosshair;                                           // Crosshair texture.
    public float aimTurnSmoothing = 0.15f;                                // Speed of turn response when aiming to match camera facing.
    public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0f);         // Offset to repoint the camera when aiming.
    public Vector3 aimCamOffset = new Vector3(0f, 0.4f, -0.7f);         // Offset to relocate the camera when aiming.

    public int FireSpeed = 10;
    public float fireRate = 15f;                    // Fire rate (rounds per second)

    public Camera fpsCam;                           // FPS camera for aiming
    public ParticleSystem muzzleFlash;              // Muzzle flash effect
    public Transform firePoint;                     // Gun's fire point (where the bullet spawns)
    public GameObject bulletPrefab;                 // Bullet prefab

    private bool aim;                               // Boolean to determine if aiming
    private int aimBool;                            // Animator reference for aiming
    private float nextTimeToFire = 0f;

    public AudioSource audioSource;   // Audio source for the shoot sound
    public AudioClip shootSFX;        // The sound effect for shooting
    public ThirdPersonOrbitCamBasic CamShake;

    // Start is always called after Awake.
    void Start()
    {
        aimBool = Animator.StringToHash("Aim");
    }

    // Update function to handle inputs for aiming and shooting.
    void Update()
    {
        
        HandleAimInput();

        // If the player is aiming and the fire button is pressed, shoot.
        if (aim && Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate; // Control fire rate
            Shoot();
        }
    }

    // Handle Aim input to toggle aim on and off.
    void HandleAimInput()
    {
        if (Input.GetAxisRaw(aimButton) != 0 && !aim)
        {
            StartCoroutine(ToggleAimOn());
        }
        else if (aim && Input.GetAxisRaw(aimButton) == 0)
        {
            StartCoroutine(ToggleAimOff());
        }

        // Toggle camera aim position left or right (switch shoulders)
        if (aim && Input.GetButtonDown(shoulderButton))
        {
            aimCamOffset.x = aimCamOffset.x * (-1);
            aimPivotOffset.x = aimPivotOffset.x * (-1);
        }

        // Set aim boolean on the Animator Controller
        behaviourManager.GetAnim.SetBool(aimBool, aim);
    }
    // Shoot method when aiming and shooting.
    void Shoot()
    {
        //CamShake.StartCameraShake(0.5f, .01f, 0.2f);
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
            rb.velocity = direction.normalized * FireSpeed; // Adjust bullet speed as needed
        }
    }


    // Coroutine to start aiming mode with delay.
    private IEnumerator ToggleAimOn()
    {
        yield return new WaitForSeconds(0.05f);
        // Aiming is not possible.
        if (behaviourManager.GetTempLockStatus(this.behaviourCode) || behaviourManager.IsOverriding(this))
            yield return false;

        // Start aiming.
        else
        {
            aim = true;
            int signal = 1;
            aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
            aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourManager.GetAnim.SetFloat(speedFloat, 0);
            // This state overrides the active one.
            behaviourManager.OverrideWithBehaviour(this);
        }
    }

    // Coroutine to end aiming mode with delay.
    private IEnumerator ToggleAimOff()
    {
        aim = false;
        yield return new WaitForSeconds(0.3f);
        behaviourManager.GetCamScript.ResetTargetOffsets();
        behaviourManager.GetCamScript.ResetMaxVerticalAngle();
        yield return new WaitForSeconds(0.05f);
        behaviourManager.RevokeOverridingBehaviour(this);
    }

    // LocalFixedUpdate overrides the virtual function of the base class.
    public override void LocalFixedUpdate()
    {
        // Set camera position and orientation to the aim mode parameters.
        if (aim)
            behaviourManager.GetCamScript.SetTargetOffsets(aimPivotOffset, aimCamOffset);
    }

    // LocalLateUpdate: manager is called here to set player rotation after camera rotates, avoiding flickering.
    public override void LocalLateUpdate()
    {
        AimManagement();
    }

    // Handle aim parameters when aiming is active.
    void AimManagement()
    {
        // Deal with the player orientation when aiming.
        Rotating();
    }

    // Rotate the player to match correct orientation, according to the camera.
    void Rotating()
    {
        Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
        // Player is moving on the ground, Y component of camera facing is not relevant.
        forward.y = 0.0f;
        forward = forward.normalized;

        // Always rotate the player according to the camera's horizontal rotation in aim mode.
        Quaternion targetRotation = Quaternion.Euler(0, behaviourManager.GetCamScript.GetH, 0);

        float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

        // Rotate the entire player to face the camera.
        behaviourManager.SetLastDirection(forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);
    }

    // Draw the crosshair when aiming.
    void OnGUI()
    {
        if (crosshair)
        {
            float mag = behaviourManager.GetCamScript.GetCurrentPivotMagnitude(aimPivotOffset);
            if (mag < 0.05f)
                GUI.DrawTexture(new Rect(Screen.width / 2.0f - (crosshair.width * 0.5f),
                                         Screen.height / 2.0f - (crosshair.height * 0.5f),
                                         crosshair.width, crosshair.height), crosshair);
        }
    }
}
