using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MainTitle : MonoBehaviour
{
    [SerializeField] private string mainGameSceneName = "LucasScene"; 


    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas settingsCanvas;


    [SerializeField] private Animator animator;



    void Start()
    {

    }
    void Update()
    {

    }



    public void OnSettingsButtonClick()
    {
        TitleCameraController cameraController = FindFirstObjectByType<TitleCameraController>();
        
        cameraController.LookAtOptions();
        animator.SetTrigger("MainSettings");
    }

    public void OnPlayButtonClick()
    {
        StartGame();
    }

    private void StartGame()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }


    internal string ReturnFromSettings()
    {
        throw new NotImplementedException();
    }
}