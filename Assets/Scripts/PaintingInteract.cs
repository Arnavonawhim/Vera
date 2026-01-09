using UnityEngine;

public class PaintingInteract : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    [Header("Visual Feedback")]
    [SerializeField] private Material countedMaterial;
    
    private bool isCounted = false;
    private bool playerInRange = false;
    private Transform player;
    private Renderer paintingRenderer;
    private Material originalMaterial;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        paintingRenderer = GetComponent<Renderer>();
        
        if (paintingRenderer != null)
        {
            originalMaterial = paintingRenderer.material;
        }
    }
    
    void Update()
    {
        if (player == null || isCounted) return;
        
        if (!PaintingManager.Instance.HasFoundExit()) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= interactDistance;
        
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            CountPainting();
        }
    }
    
    void CountPainting()
    {
        if (isCounted) return;
        
        isCounted = true;
        
        if (PaintingManager.Instance != null)
        {
            PaintingManager.Instance.CountPainting(transform.position);
        }
        
        if (paintingRenderer != null && countedMaterial != null)
        {
            paintingRenderer.material = countedMaterial;
        }
        else if (paintingRenderer != null)
        {
            Color darkerColor = paintingRenderer.material.color * 0.5f;
            paintingRenderer.material.color = darkerColor;
        }
    }
    
    void OnGUI()
    {
        if (playerInRange && !isCounted && PaintingManager.Instance.HasFoundExit())
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            
            GUI.Label(new Rect(0, screenHeight - 150, screenWidth, 50), "Press E to Count Painting", style);
        }
    }
    
    public bool IsCounted() => isCounted;
}