using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;

public class MainTitle : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public Button playButton;
    [SerializeField] public Button settingsButton;

    [SerializeField] private string mainGameSceneName = "LucasScene"; 
    [SerializeField] private Image fadePanel; // Imagen negra para el fade



    [Header("Visual Effects")]  
    [SerializeField] private float hoverScaleMultiplier = 1.2f;
    [SerializeField] private float smoothSpeed = 10f;
    
    [Header("Fade Settings")]


    [SerializeField] private float initialFadeOutDuration = 1.5f;
    [SerializeField] private float sceneTransitionFadeDuration = 1.0f;
    
    private Dictionary<Button, Vector3> originalScales = new Dictionary<Button, Vector3>();
    private Dictionary<Button, Vector3> targetScales = new Dictionary<Button, Vector3>();
    private bool isTransitioning = false;


    [Header("Canvas References")]
    [SerializeField] public Canvas mainCanvas;
    [SerializeField] public Canvas settingsCanvas;



    void Start()
    {
        Color startColor = fadePanel.color;
        startColor.a = 1f;
        fadePanel.color = startColor;
        
        playButton.onClick.AddListener(OnPlayButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);

        ConfigureButtonHoverEffects(playButton);
        ConfigureButtonHoverEffects(settingsButton);
        
        StartCoroutine(FadeOut(initialFadeOutDuration));


        if (settingsCanvas != null)
        {
            settingsCanvas.gameObject.SetActive(false);
        }
        
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(true);
        }
        
        StartCoroutine(FadeOut(initialFadeOutDuration));
    }
        void Update()
    {
        if (!isTransitioning)
        {
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



    private void OnSettingsButtonClick()
    {
        mainCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(true);
    }

    public void OnPlayButtonClick()
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(FadeInAndLoadScene(sceneTransitionFadeDuration));
        }
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

    
    private void StartGame()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }
    
    private IEnumerator FadeOut(float duration)
    {
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
        
        currentColor.a = 0f;
        fadePanel.color = currentColor;
    }
    
    private IEnumerator FadeInAndLoadScene(float duration)
    {
        float elapsedTime = 0f;
        Color currentColor = fadePanel.color;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            
            currentColor.a = newAlpha;
            fadePanel.color = currentColor;
            
            yield return null;
        }
        
        // Asegúrate de que el alpha llegue exactamente a 1
        currentColor.a = 1f;
        fadePanel.color = currentColor;
        
        // Cargar la escena
        StartGame();
    }

    internal string ReturnFromSettings()
    {
        throw new NotImplementedException();
    }
}