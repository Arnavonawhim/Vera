using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;
    
    [Header("UI References")]
    [SerializeField] private Text objectiveText;
    
    [Header("Objectives")]
    [SerializeField] private string initialObjective = "Find the exit";
    
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
        SetObjective(initialObjective);
    }
    
    public void SetObjective(string newObjective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = "Objective: " + newObjective;
        }
    }
}