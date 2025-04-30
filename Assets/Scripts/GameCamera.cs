using UnityEngine;

public class LucasCameraController : MonoBehaviour
{
    [Header("Player Referencing")]
    public Transform player;

    [Header("Camera Settings")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("Mouse Tilt Settings")]
    public float tiltAmount = 5f;
    public float tiltSpeed = 2f;
    public float edgeThreshold = 0.2f;
    private float tiltX = 0f;
    private float tiltY = 0f;

    [Header("Orbit Settings")]
    public float orbitSpeed = 100f;
    public float orbitSmoothness = 10f;
    public float orbitSensitivity = 10f;
    private float orbitAngle = 0f;
    private float targetOrbitAngle = 0f;

    void Start()
    {
        // Initialize position and angle
        orbitAngle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        targetOrbitAngle = orbitAngle;
        UpdateCameraPosition();
    }

    void Update()
    {
        HandleMouseEdgeTilting();
        HandleOrbitInput();
        
        // Smoothly interpolate to target orbit angle
        orbitAngle = Mathf.Lerp(orbitAngle, targetOrbitAngle, orbitSmoothness * Time.deltaTime);
        
        UpdateCameraPosition();
    }

    void HandleMouseEdgeTilting()
    {
        Vector2 mousePosition = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float mouseX = 0f;
        float mouseY = 0f;

        if (mousePosition.x < screenWidth * edgeThreshold) mouseX = -1f;
        if (mousePosition.x > screenWidth * (1 - edgeThreshold)) mouseX = 1f;
        if (mousePosition.y < screenHeight * edgeThreshold) mouseY = -1f;
        if (mousePosition.y > screenHeight * (1 - edgeThreshold)) mouseY = 1f;

        tiltX = Mathf.Lerp(tiltX, mouseX * tiltAmount, tiltSpeed * Time.deltaTime);
        tiltY = Mathf.Lerp(tiltY, mouseY * tiltAmount, tiltSpeed * Time.deltaTime);
    }

    void HandleOrbitInput()
    {
        float orbitInput = Input.GetAxis("Mouse ScrollWheel");

        if (orbitInput != 0)
        {
            targetOrbitAngle -= orbitInput * orbitSpeed * orbitSensitivity * Time.deltaTime;
        }
    }

    void UpdateCameraPosition()
    {
        // Calculate position based on orbit angle
        float distance = new Vector2(offset.x, offset.z).magnitude;
        float height = offset.y;
        
        // Convert angle to radians and calculate new position
        float rad = orbitAngle * Mathf.Deg2Rad;
        Vector3 newOffset = new Vector3(
            Mathf.Sin(rad) * distance,
            height,
            Mathf.Cos(rad) * distance
        );

        // Apply position and look at player
        Vector3 targetPosition = player.position + newOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        
        // Look at player first
        transform.LookAt(player);
        
        // Then apply tilt
        transform.rotation *= Quaternion.Euler(-tiltY, tiltX, 0);
    }
}
