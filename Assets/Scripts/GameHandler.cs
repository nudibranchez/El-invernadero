using UnityEngine;
using TMPro;
using System;

public class GameHandler : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI Timer;
    [SerializeField] private float startTime = 60f; //Reminder: its in seconds
    
    [Header("Clue Score Settings")]
    [SerializeField] private TextMeshProUGUI clueScoreText;
    [SerializeField] private int collectedKeys = 0;
    [SerializeField] private int totalKeys = 10;
    
    private float currentTime;
    private bool timerIsRunning = true;

    public static GameHandler Instance { get; private set; }

    [SerializeField] Animator animator;

    [SerializeField] private GameObject goalObject;


    void Awake()
    {
        // Configurar singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = startTime;
        UpdateTimerDisplay();
        UpdateClueScoreDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            currentTime -= Time.deltaTime;

            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        Timer.text = timeString;
    }

    public void AddClue()
    {
        collectedKeys ++;
        UpdateClueScoreDisplay();

        if (collectedKeys == totalKeys)
        {
            animator.SetTrigger("GameWon");
        }
    }

    private void UpdateClueScoreDisplay()
    {
        clueScoreText.text = collectedKeys.ToString();
    }
}
