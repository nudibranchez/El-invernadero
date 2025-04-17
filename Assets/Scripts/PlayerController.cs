using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Speed Manager")]
    public float speed = 5f;
    public float acceleration = 10f;
    public float deceleration = 5f;
    public float rotationSpeed = 2f;

    [Header("Models (for rotation)")]
    public Transform playerModel;
    public Transform flashlight;
    private Vector3 velocity = Vector3.zero;

    [Header("Animation")]
    private Animator animator;

    void Start()
    {
        animator = playerModel.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Using the old Input system
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        //This is to make the player move relative to the camera
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();
        
        // Calculate direction relative to camera orientation
        var dir = (cameraForward * moveZ + cameraRight * moveX).normalized;

        //For the animator xd
        bool isMoving = dir.magnitude > 0;
        animator.SetBool("isWalking", isMoving);
        
        if (isMoving)
        {
            flashlight.rotation = Quaternion.Slerp(flashlight.rotation, playerModel.rotation, rotationSpeed * 0.5f * Time.deltaTime);
            velocity = Vector3.MoveTowards(velocity, dir * speed, acceleration * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        flashlight.rotation = Quaternion.Slerp(flashlight.rotation, playerModel.rotation, (rotationSpeed * 0.5f) * Time.deltaTime);
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }
}
