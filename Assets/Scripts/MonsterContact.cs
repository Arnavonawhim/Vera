using UnityEngine;

public class MonsterContact : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float killDistance = 2f;
    
    private Transform player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
    }
    
    void Update()
    {
        if (player != null && !GameManager.Instance.IsGameOver())
        {
            float distance = Vector3.Distance(transform.position, player.position);
            
            if (distance <= killDistance)
            {
                KillPlayer();
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            KillPlayer();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            KillPlayer();
        }
    }
    
    void KillPlayer()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }
}