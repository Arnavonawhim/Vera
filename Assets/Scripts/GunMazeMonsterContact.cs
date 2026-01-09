using UnityEngine;

public class GunMazeMonsterContact : MonoBehaviour
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
        if (player != null && GunMazeManager.Instance != null && !GunMazeManager.Instance.IsGameOver())
        {
            float distance = Vector3.Distance(transform.position, player.position);
            
            if (distance <= killDistance)
            {
                CheckContact();
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            CheckContact();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            CheckContact();
        }
    }
    
    void CheckContact()
    {
        if (GunManager.Instance != null && GunManager.Instance.HasAllGuns())
        {
            KillMonster();
        }
        else
        {
            KillPlayer();
        }
    }
    
    void KillPlayer()
    {
        if (GunMazeManager.Instance != null)
        {
            GunMazeManager.Instance.TriggerGameOver();
        }
    }
    
    void KillMonster()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("Monster killed! Escaping...");
        }
        
        Destroy(gameObject);
        
        if (GunMazeManager.Instance != null)
        {
            GunMazeManager.Instance.MonsterKilled();
        }
    }
}