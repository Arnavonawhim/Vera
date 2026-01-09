using UnityEngine;
using UnityEngine.UI;

public class GunManager : MonoBehaviour
{
    public static GunManager Instance;
    
    [Header("UI References")]
    [SerializeField] private Text gunCountText;
    
    [Header("Settings")]
    [SerializeField] private int totalGunsNeeded = 3;
    
    private int gunsCollected = 0;
    
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
        UpdateUI();
        
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("Collect guns 0/3");
        }
    }
    
    public void CollectGun()
    {
        gunsCollected++;
        UpdateUI();
        
        if (ObjectiveManager.Instance != null)
        {
            if (gunsCollected < totalGunsNeeded)
            {
                ObjectiveManager.Instance.SetObjective("Collect guns " + gunsCollected + "/" + totalGunsNeeded);
            }
            else
            {
                ObjectiveManager.Instance.SetObjective("All guns collected! Find the monster");
            }
        }
    }
    
    void UpdateUI()
    {
        if (gunCountText != null)
        {
            gunCountText.text = "Guns: " + gunsCollected + "/" + totalGunsNeeded;
        }
    }
    
    public bool HasAllGuns()
    {
        return gunsCollected >= totalGunsNeeded;
    }
    
    public int GetGunsCollected()
    {
        return gunsCollected;
    }
}