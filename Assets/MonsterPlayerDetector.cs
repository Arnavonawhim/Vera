using UnityEngine;

public class MonsterPlayerDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float detectionDistance = 3f;
    
    private Transform player;
    private bool hasDetected = false;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
    }
    
    void Update()
    {
        if (hasDetected || player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance <= detectionDistance)
        {
            DetectPlayer();
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            DetectPlayer();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            DetectPlayer();
        }
    }
    
    void DetectPlayer()
    {
        if (hasDetected) return;
        
        hasDetected = true;
        
        if (MonsterSceneManager.Instance != null)
        {
            Debug.Log("Player found!");
        }
    }
}