using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float pickupDistance = 3f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    
    private bool playerInRange = false;
    private Transform player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= pickupDistance;
        
        if (playerInRange && Input.GetKeyDown(pickupKey))
        {
            Pickup();
        }
    }
    
    void Pickup()
    {
        if (GunManager.Instance != null)
        {
            GunManager.Instance.CollectGun();
        }
        
        Destroy(gameObject);
    }
    
    void OnGUI()
    {
        if (playerInRange)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            
            GUI.Label(new Rect(0, screenHeight - 150, screenWidth, 50), "Press E to Pickup Gun", style);
        }
    }
}