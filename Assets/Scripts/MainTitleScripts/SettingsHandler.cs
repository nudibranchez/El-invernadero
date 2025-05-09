using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public Button BackButton;
    [SerializeField] public Button VignetteButton;
    [SerializeField] public Button GrainButton;


    [Header("Canvas References")]
    [SerializeField] public Canvas mainCanvas;
    [SerializeField] public Canvas settingsCanvas;


    [Header("Post Processing")]
    [SerializeField] private Volume globalVolume;
    private Vignette vignette;
    private FilmGrain grain;
    private bool vignetteEnabled = true;
    private bool grainEnabled = true;

    [SerializeField] private Animator animator;

    void Start()
    {
        if (globalVolume == null)
        {
            globalVolume = FindFirstObjectByType<Volume>();
        }
        
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out grain);
        }
        
        LoadPostProcessingPreferences();
        ApplyPostProcessingPreferences();
        UpdateButtonVisuals();
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
    
    public void OnBackButtonClick()
    {
        TitleCameraController cameraController = FindFirstObjectByType<TitleCameraController>();
        
        animator.SetTrigger("SettingsMain");
        StartCoroutine(TransitionToMainMenu(cameraController));
    }

    private IEnumerator TransitionToMainMenu(TitleCameraController cameraController)
    {
        cameraController.LookAtMainMenu();
        yield return new WaitForSeconds(0.7f);
    }

    public void OnVignetteButtonClick()
    {
        vignetteEnabled = !vignetteEnabled;
        
        PlayerPrefs.SetInt("VignetteEnabled", vignetteEnabled ? 1 : 0);
        PlayerPrefs.Save();
        
        if (vignette != null)
        {
            vignette.active = vignetteEnabled;
            
            if (globalVolume != null && globalVolume.profile != null)
            {
                globalVolume.profile.isDirty = true;
            }
        }
        

        UpdateButtonVisuals();
    }

    public void OnGrainButtonClick()
    {
        grainEnabled = !grainEnabled;

        PlayerPrefs.SetInt("GrainEnabled", grainEnabled ? 1 : 0);
        PlayerPrefs.Save();

        if (grain != null)
        {
            grain.active = grainEnabled;

            if (globalVolume != null && globalVolume.profile != null)
            {
                globalVolume.profile.isDirty = true;
            }
        }

        UpdateButtonVisuals();
    }
}
