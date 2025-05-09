using UnityEngine;
using System.Collections;

public class TitleCameraController : MonoBehaviour
{
    [Header("Parallax Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 maxMovement = new Vector2(1f, 1f);
    [SerializeField] private bool useParallaxInSettings = true; 

    [Header("Menu Transition")]
    [SerializeField] private float rotationDuration = 0.7f;
    [SerializeField] private float rotationAmount = 30f; 
    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool parallaxEnabled = true;
    private bool isRotating = false;
    private bool isInSettingsScreen = false; 

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition;
        initialRotation = transform.rotation;
        targetRotation = initialRotation;
    }

    void Update()
    {
        if (parallaxEnabled && !isRotating && (!isInSettingsScreen || useParallaxInSettings))
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
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );
    }

    public void LookAtOptions()
    {
        StopAllCoroutines();
        StartCoroutine(RotateToOptions());
    }

    public void LookAtMainMenu()
    {
        StopAllCoroutines();
        StartCoroutine(RotateToMainMenu());
    }

    private IEnumerator RotateToOptions()
    {
        parallaxEnabled = false;
        isRotating = true;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = initialRotation * Quaternion.Euler(0, rotationAmount, 0);
        
        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotationDuration);
            float smoothT = EaseOutQuad(t);
            
            targetRotation = Quaternion.Slerp(startRot, endRot, smoothT);
            
            yield return null;
        }
        
        targetRotation = endRot;
        
        isRotating = false;
        isInSettingsScreen = true;

        yield return new WaitForSeconds(0.05f);
        parallaxEnabled = true;
    }

    private IEnumerator RotateToMainMenu()
    {
        parallaxEnabled = false;
        isRotating = true;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = initialRotation;

        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotationDuration);
            float smoothT = EaseOutQuad(t);
            
            targetRotation = Quaternion.Slerp(startRot, endRot, smoothT);
            
            yield return null;
        }

        targetRotation = endRot;
        
        isRotating = false;
        isInSettingsScreen = false;

        yield return new WaitForSeconds(0.05f);
        parallaxEnabled = true;
    }

    private float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }

    public bool IsParallaxEnabled()
    {
        return parallaxEnabled && (!isInSettingsScreen || useParallaxInSettings) && !isRotating;
    }

    public void ForceParallaxInSettings(bool enabled)
    {
        useParallaxInSettings = enabled;
    }
}
