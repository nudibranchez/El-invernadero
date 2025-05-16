using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] public Transform player;
    [SerializeField] public float chaseSpeed = 3.5f;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnDelay = 20f;
    [SerializeField] private float maxSpawnDelay = 40f;
    [SerializeField] private float chaseDuration = 10f;
    [SerializeField] private float minSpawnDistance = 5f;
    [SerializeField] private float maxSpawnDistance = 15f;

    private float nextSpawnTime;


    private bool isChasing = false;
    private bool isVisible = false;

    private NavMeshAgent navAgent;
    private Renderer enemyRenderer;
    private Collider enemyCollider;

    private Animator animator;

    [SerializeField] private AudioClip SpawnSound;
    private AudioSource audioSource;


    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponent<Renderer>();
        enemyCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        navAgent.speed = chaseSpeed;
        navAgent.updateRotation = true;
        navAgent.stoppingDistance = 1.5f;
        
    }

    void Start()
    {
        SetEnemyVisibility(false);
        ScheduleNextSpawn();
    }
    
    void Update()
    {
        if (isChasing && player != null)
        {
            navAgent.SetDestination(player.position);
        }

        if (!isVisible && !isChasing && Time.time >= nextSpawnTime)
        {
            StartCoroutine(ChaseSequence());
        }
    }

    private IEnumerator ChaseSequence()
    {
        Vector3 spawnPosition = FindSpawnPosition();
        transform.position = spawnPosition;
        SetEnemyVisibility(true);
        audioSource.PlayOneShot(SpawnSound);

        isChasing = true;
        yield return new WaitForSeconds(chaseDuration);
        isChasing = false;

        SetEnemyVisibility(false);

        ScheduleNextSpawn();
    }

    private Vector3 FindSpawnPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            Vector3 randomDirection = new Vector3(randomCircle.x, 0, randomCircle.y);
            
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            
            Vector3 potentialPosition = player.position + randomDirection * distance;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialPosition, out hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        
        return transform.position;
    }

    private void ScheduleNextSpawn()
    {
        float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
        nextSpawnTime = Time.time + delay;
    }

    private void SetEnemyVisibility(bool visible)
    {
        isVisible = visible;
 
        if (enemyCollider != null)
            enemyCollider.enabled = visible;
        
        if (navAgent != null)
            navAgent.enabled = visible;

        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            r.enabled = visible;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("Attack");

            // Mostrar pantalla de derrota y pausar el tiempo
            InterfaceHandler ui = FindFirstObjectByType<InterfaceHandler>();
            if (ui != null)
            {
                ui.ShowLoseScreen();
            }
        }
    }
}
