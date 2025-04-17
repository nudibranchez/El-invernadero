using UnityEngine;

public class TitleCameraController : MonoBehaviour
{
    [Header("Parallax Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 maxMovement = new Vector2(1f, 1f);

    
    
    private Vector3 targetPosition;
    private Vector3 initialPosition;
    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = new Vector2(
            Input.mousePosition.x / Screen.width,
            Input.mousePosition.y / Screen.height
        );
        
        Vector2 mouseOffset = new Vector2(
            mousePosition.x - 0.5f,
            mousePosition.y - 0.5f
        );
        
        targetPosition = initialPosition + new Vector3(
            mouseOffset.x * maxMovement.x,
            mouseOffset.y * maxMovement.y,
            0
        ) * mouseSensitivity;
        
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
