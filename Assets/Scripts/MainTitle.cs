using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MainTitle : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public Button playButton;
    [SerializeField] private string mainGameSceneName = "LucasScene"; 
    [SerializeField] private Image fadePanel; // Imagen negra para el fade

    [Header("Visual Effects")]  
    [SerializeField] private float hoverScaleMultiplier = 1.2f;
    [SerializeField] private float smoothSpeed = 10f;
    
    [Header("Fade Settings")]
    [SerializeField] private float initialFadeOutDuration = 1.5f;
    [SerializeField] private float sceneTransitionFadeDuration = 1.0f;
    
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isTransitioning = false;

    void Start()
    {
        if (playButton == null) 
        {
            Debug.LogError("Play button no asignado");
            return;
        }
        
        if (fadePanel == null)
        {
            Debug.LogError("Fade panel no asignado");
            return;
        }

        Color startColor = fadePanel.color;
        startColor.a = 1f;
        fadePanel.color = startColor;
        
        playButton.onClick.AddListener(OnPlayButtonClick);
        
        originalScale = playButton.transform.localScale;
        targetScale = originalScale;
        
        ConfigureButtonHoverEffects();
        
        StartCoroutine(FadeOut(initialFadeOutDuration));
    }
    
    
    private void ConfigureButtonHoverEffects()
    {
        EventTrigger trigger = playButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = playButton.gameObject.AddComponent<EventTrigger>();
            
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { targetScale = originalScale * hoverScaleMultiplier; });
        trigger.triggers.Add(entryEnter);
        
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { targetScale = originalScale; });
        trigger.triggers.Add(entryExit);
    }
    
    void Update()
    {
        if (playButton != null && !isTransitioning)
        {
            // Suavizar la transición de escala
            playButton.transform.localScale = Vector3.Lerp(
                playButton.transform.localScale, 
                targetScale, 
                Time.deltaTime * smoothSpeed
            );
        }
    }

    public void OnPlayButtonClick()
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(FadeInAndLoadScene(sceneTransitionFadeDuration));
        }
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
}