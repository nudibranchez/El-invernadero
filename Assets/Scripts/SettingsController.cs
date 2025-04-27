using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public Button BackButton;
    [SerializeField] public Button VignetteButton;
    [SerializeField] public Button GrainButton;

    [SerializeField] private Image fadePanel;
    
    [Header("Fade Settings")]
    [SerializeField] private float initialFadeOutDuration = 1.5f;
    [SerializeField] private float sceneTransitionFadeDuration = 1.0f;
    
    private Dictionary<Button, Vector3> originalScales = new Dictionary<Button, Vector3>();
    private Dictionary<Button, Vector3> targetScales = new Dictionary<Button, Vector3>();
    private bool isTransitioning = false;

    [Header("Canvas References")]
    [SerializeField] public Canvas mainCanvas;
    [SerializeField] public Canvas settingsCanvas;

    [Header("Visual Effects")]
    [SerializeField] private float hoverScaleMultiplier = 1.2f;
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Post Processing")]
    [SerializeField] private Volume globalVolume;
    private Vignette vignette;
    private FilmGrain grain;


    private bool vignetteEnabled = true;
    private bool grainEnabled = true;

    [SerializeField] private Animator animator;

    void Start()
    {
        LoadPostProcessingPreferences();

        BackButton.onClick.AddListener(OnBackButtonClick);
        VignetteButton.onClick.AddListener(OnVignetteButtonClick);
        GrainButton.onClick.AddListener(OnGrainButtonClick);

        ConfigureButtonHoverEffects(BackButton);
        ConfigureButtonHoverEffects(VignetteButton);
        ConfigureButtonHoverEffects(GrainButton);

        UpdateButtonVisuals();

        if (fadePanel != null)
        {
            Color startColor = fadePanel.color;
            startColor.a = 1f;
            fadePanel.color = startColor;
            
            StartCoroutine(FadeOut(initialFadeOutDuration));
        }
    }

    private void UpdateButtonVisuals()
    {
        TMPro.TextMeshProUGUI vignetteTMP = VignetteButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (vignetteTMP != null)
        {
            vignetteTMP.text = "Vignette: " + (vignetteEnabled ? "ON" : "OFF");
        }
        
        TMPro.TextMeshProUGUI grainTMP = GrainButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (grainTMP != null)
        {
            grainTMP.text = "Film Grain: " + (grainEnabled ? "ON" : "OFF");
        }
    }

    void Update()
    {
        if (!isTransitioning){
                // Actualizar la escala de todos los botones registrados
                foreach (var buttonEntry in targetScales)
                {
                    Button button = buttonEntry.Key;
                    Vector3 targetScale = buttonEntry.Value;
                    
                    if (button != null)
                    {
                        button.transform.localScale = Vector3.Lerp(
                            button.transform.localScale, 
                            targetScale, 
                            Time.deltaTime * smoothSpeed
                        );
                    }
                }
            }
    }

     private void LoadPostProcessingPreferences()
    {
        // Cargar preferencias guardadas (por defecto activadas)
        vignetteEnabled = PlayerPrefs.GetInt("VignetteEnabled", 1) == 1;
        grainEnabled = PlayerPrefs.GetInt("GrainEnabled", 1) == 1;
    }

    private void ApplyPostProcessingPreferences()
    {
        // Aplicar a los componentes si existen
        if (vignette != null)
        {
            vignette.active = vignetteEnabled;
        }
        
        if (grain != null)
        {
            grain.active = grainEnabled;
        }
    }
    
    private void OnBackButtonClick()
    {
        TitleCameraController cameraController = FindFirstObjectByType<TitleCameraController>();
        
        animator.SetTrigger("SettingsMain");
        StartCoroutine(TransitionToMainMenu(cameraController));
    }

    private IEnumerator TransitionToMainMenu(TitleCameraController cameraController)
    {
        // Iniciar la rotación
        cameraController.LookAtMainMenu();
        
        // Esperar un tiempo aproximado a la duración de la rotación
        yield return new WaitForSeconds(0.7f); // Mismo valor que rotationDuration
        
        // Cambiar los paneles
        //settingsCanvas.gameObject.SetActive(false);
        //mainCanvas.gameObject.SetActive(true);
    }

    private void OnVignetteButtonClick()
    {
        vignetteEnabled = !vignetteEnabled;
        
        if (vignette != null)
        {
            vignette.active = vignetteEnabled;
        }
        
        PlayerPrefs.SetInt("VignetteEnabled", vignetteEnabled ? 1 : 0);
        PlayerPrefs.Save();
        UpdateButtonVisuals();
    }

    private void OnGrainButtonClick()
    {
        grainEnabled = !grainEnabled;

        if (grain != null)
        {
            grain.active = grainEnabled;
        }

        PlayerPrefs.SetInt("GrainEnabled", grainEnabled ? 1 : 0);
        PlayerPrefs.Save();
        UpdateButtonVisuals();
    }


    private void ConfigureButtonHoverEffects(Button button)
    {
        if (button == null) return;

        // Obtener o añadir el EventTrigger al botón específico
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();
            
        // Limpiar triggers existentes para evitar duplicados
        trigger.triggers.Clear();
            
        // Almacenar la escala original del botón específico si no existe
        if (!originalScales.ContainsKey(button))
        {
            originalScales[button] = button.transform.localScale;
            targetScales[button] = originalScales[button];
        }
            
        // Configurar el evento de entrada del mouse
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { 
            targetScales[button] = originalScales[button] * hoverScaleMultiplier; 
        });
        trigger.triggers.Add(entryEnter);
        
        // Configurar el evento de salida del mouse
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { 
            targetScales[button] = originalScales[button]; 
        });
        trigger.triggers.Add(entryExit);
    }


    private IEnumerator FadeOut(float duration)
    {
        if (fadePanel == null) yield break;
        
        isTransitioning = true;
        float elapsedTime = 0f;
        Color currentColor = fadePanel.color;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            
            currentColor.a = newAlpha;
            fadePanel.color = currentColor;
            
            yield return null;
        }
        
        // Asegurarse de que el alpha llegue a 0
        currentColor.a = 0f;
        fadePanel.color = currentColor;
        isTransitioning = false;
    }
}
