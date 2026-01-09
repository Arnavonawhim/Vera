using UnityEngine;
using UnityEngine.UI;

public class PaintingManager : MonoBehaviour
{
    public static PaintingManager Instance;
    
    [Header("UI References")]
    [SerializeField] private GameObject paintingCountUI;
    [SerializeField] private Text countText;
    
    [Header("Settings")]
    [SerializeField] private int totalPaintings = 11;
    
    private int paintingsFound = 0;
    private bool hasFoundExit = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (paintingCountUI != null)
        {
            paintingCountUI.SetActive(false);
        }
        
        UpdateUI();
    }
    
    public void CountPainting(Vector3 paintingPosition)
    {
        paintingsFound++;
        UpdateUI();
        
        MonsterAI monsterAI = FindObjectOfType<MonsterAI>();
        if (monsterAI != null)
        {
           monsterAI.AlertMonster(paintingPosition);
        }
        
        Debug.Log($"Paintings counted: {paintingsFound}/{totalPaintings}");
    }
    
    public void RevealPaintingUI()
    {
        hasFoundExit = true;
        
        if (paintingCountUI != null)
        {
            paintingCountUI.SetActive(true);
        }
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (countText != null)
        {
            countText.text = $"Paintings: {paintingsFound}";
        }
    }
    
    public int GetPaintingsFound() => paintingsFound;
    public int GetTotalPaintings() => totalPaintings;
    public bool HasFoundExit() => hasFoundExit;
    public bool HasCountedAllPaintings() => paintingsFound >= totalPaintings;
}