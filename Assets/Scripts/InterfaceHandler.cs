using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InterfaceHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;

    [Header("UI Elements")]
    [SerializeField] private Image PageImage;
    [SerializeField] private Canvas UICanvas;

    [Header("Pause Menu")]
    [SerializeField] private Canvas PauseMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsHovering", false);

        PauseMenu.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("IsHovering", true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("IsHovering", false);
    }
}
