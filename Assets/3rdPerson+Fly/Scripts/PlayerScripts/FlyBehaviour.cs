using UnityEngine;

// FlyBehaviour inherits from GenericBehaviour. This class corresponds to the flying behaviour.
public class FlyBehaviour : GenericBehaviour
{
    public float flySpeed = 4.0f;                 // Default flying speed.
    public float sprintFactor = 2.0f;             // How much sprinting affects fly speed.
    public float flyMaxVerticalAngle = 60f;       // Angle to clamp camera vertical movement when flying.

    private int flyBool;                          // Animator variable related to flying.
    private CapsuleCollider col;                  // Reference to the player capsule collider.
    public AudioSource audioSource;
    public ThirdPersonOrbitCamBasic CamShake;

    void Start()
    {
        // Set up the references.
        flyBool = Animator.StringToHash("Fly");
        col = this.GetComponent<CapsuleCollider>();
        // Subscribe this behaviour on the manager.
        behaviourManager.SubscribeBehaviour(this);

        // Automatically start flying.
        StartFlying();
    }
    private void Update()
    {
        if (behaviourManager.IsSprinting())
        {
            audioSource.pitch = 0.4f;  // Sprinting pitch
            CamShake.StartCameraShake(.2f, 2f, Mathf.Infinity);
        }
        else if (behaviourManager.IsMoving())
        {
            CamShake.StartCameraShake(.1f, 1f, Mathf.Infinity);
        }
        else
        {
            audioSource.pitch = 0.5f;  // Normal pitch
            CamShake.StartCameraShake(0f, 0f, 0f);
        }
    }

    // Function to enable flying mode.
    private void StartFlying()
    {
        // Register this behaviour.
        behaviourManager.RegisterBehaviour(this.behaviourCode);

        // Disable gravity.
        behaviourManager.GetRigidBody.useGravity = false;

        // Set fly related variables on the Animator Controller.
        behaviourManager.GetAnim.SetBool(flyBool, true);
    }

    // This function is called when another behaviour overrides the current one.
    public override void OnOverride()
    {
        // Ensure the collider will return to vertical position when behaviour is overridden.
        col.direction = 1;
    }

    // LocalFixedUpdate overrides the virtual function of the base class.
    public override void LocalFixedUpdate()
    {
        // Set camera limit angle related to fly mode.
        behaviourManager.GetCamScript.SetMaxVerticalAngle(flyMaxVerticalAngle);

        // Call the fly manager.
        FlyManagement(behaviourManager.GetH, behaviourManager.GetV);
    }

    // Deal with the player movement when flying.
    void FlyManagement(float horizontal, float vertical)
    {
        // Add a force player's rigidbody according to the fly direction.
        Vector3 direction = Rotating(horizontal, vertical);
        behaviourManager.GetRigidBody.AddForce(direction * (flySpeed * 100 * (behaviourManager.IsSprinting() ? sprintFactor : 1)), ForceMode.Acceleration);
    }

    // Rotate the player to match correct orientation, according to camera and key pressed.
    Vector3 Rotating(float horizontal, float vertical)
    {
        Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
        // Camera forward Y component is relevant when flying.
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        // Calculate target direction based on camera forward and direction key.
        Vector3 targetDirection = forward * vertical + right * horizontal;

        // Rotate the player to the correct fly position.
        if ((behaviourManager.IsMoving() && targetDirection != Vector3.zero))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(behaviourManager.GetRigidBody.rotation, targetRotation, behaviourManager.turnSmoothing);

            behaviourManager.GetRigidBody.MoveRotation(newRotation);
            behaviourManager.SetLastDirection(targetDirection);
        }

        // Player is flying and idle?
        if (!(Mathf.Abs(horizontal) > 0.2 || Mathf.Abs(vertical) > 0.2))
        {
            // Rotate the player to stand position.
            behaviourManager.Repositioning();
            // Set collider direction to vertical.
            col.direction = 1;
        }
        else
        {
            // Set collider direction to horizontal.
            col.direction = 2;
        }

        // Return the current fly direction.
        return targetDirection;
    }
}
