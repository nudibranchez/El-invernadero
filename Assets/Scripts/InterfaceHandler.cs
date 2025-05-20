using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class InterfaceHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;

    [Header("UI Elements")]
    [SerializeField] private Image PageImage;
    [SerializeField] private Canvas UICanvas;
    [SerializeField] private TMPro.TextMeshProUGUI LeaveText;

    [Header("Pause Menu")]
    [SerializeField] private Canvas PauseMenu;

    [Header("Win Screen")]
    [SerializeField] private Canvas winScreen;
    [Header("Lose Screen")]
    [SerializeField] private Canvas loseScreen;

    [Header("Instructions")]
    [SerializeField] private Canvas instructions;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsHovering", false);
        Time.timeScale = 0f;

        PauseMenu.enabled = false;
        winScreen.enabled = false;
        loseScreen.enabled = false;
        instructions.enabled = true;
    }

    void Update()
    {
        if (instructions.enabled && Input.GetKeyDown(KeyCode.Escape))
        {
            // Llama a la corrutina para cerrar instrucciones con animación
            StartCoroutine(CloseInstructions());
        }
        else if (!instructions.enabled && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private IEnumerator CloseInstructions()
    {
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1f);
        instructions.enabled = false;
        UICanvas.enabled = true;
    }

    public void TogglePauseMenu()
    {
        if (PauseMenu.enabled)
        {
            PauseMenu.enabled = false;
            Time.timeScale = 1f;
        }
        else
        {
            animator.SetBool("Paused", true);
            PauseMenu.enabled = true;
            Time.timeScale = 0f;
        }
    }

    public void backToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LucasTitle");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("IsHovering", true);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("IsHovering", false);
    }

    public void ShowLoseScreen()
    {
        loseScreen.enabled = true;
        Time.timeScale = 0f;
    }

    public void ShowWinScreen()
    {
        winScreen.enabled = true;
        Time.timeScale = 0f;
    }
}
