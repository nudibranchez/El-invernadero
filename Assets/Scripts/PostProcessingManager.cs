using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    
    void Start()
    {
        // Obtener el volumen global si no se ha asignado
        if (globalVolume == null)
        {
            globalVolume = FindFirstObjectByType<Volume>();
        }
        
        // Aplicar configuraciones guardadas
        ApplyPostProcessingSettings();
    }
    
    private void ApplyPostProcessingSettings()
    {
        if (globalVolume != null)
        {
            // Leer las preferencias guardadas
            bool vignetteEnabled = PlayerPrefs.GetInt("VignetteEnabled", 1) == 1;
            bool grainEnabled = PlayerPrefs.GetInt("GrainEnabled", 1) == 1;
            
            // Obtener y configurar Vignette
            if (globalVolume.profile.TryGet(out Vignette vignette))
            {
                vignette.active = vignetteEnabled;
            }
            
            // Obtener y configurar Film Grain
            if (globalVolume.profile.TryGet(out FilmGrain grain))
            {
                grain.active = grainEnabled;
            }
        }
    }
}