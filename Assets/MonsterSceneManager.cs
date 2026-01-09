using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MonsterSceneManager : MonoBehaviour
{
    public static MonsterSceneManager Instance;
    
    [Header("UI References")]
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button spareButton;
    [SerializeField] private Button killButton;
    
    [Header("Settings")]
    [SerializeField] private float totalTime = 150f;
    [SerializeField] private float detectionDistance = 3f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private int mainMenuSceneIndex = 0;
    
    private float timeRemaining;
    private bool playerFound = false;
    private Transform player;
    private Transform monsterTransform;
    
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
        timeRemaining = totalTime;
        
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        monsterTransform = GameObject.FindGameObjectWithTag("Monster")?.transform;
        
        if (monsterTransform == null)
        {
            monsterTransform = Camera.main.transform;
        }
        
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
        
        if (spareButton != null)
        {
            spareButton.onClick.AddListener(OnSpareClicked);
        }
        
        if (killButton != null)
        {
            killButton.onClick.AddListener(OnKillClicked);
        }
        
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("KILL KILL KILL");
        }
        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (playerFound) return;
        
        timeRemaining -= Time.deltaTime;
        
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        
        if (timeRemaining <= 0)
        {
            ReturnToMainMenu();
            return;
        }
        
        if (player != null && monsterTransform != null)
        {
            float distance = Vector3.Distance(monsterTransform.position, player.position);
            
            if (distance <= detectionDistance)
            {
                PlayerFound();
            }
        }
    }
    
    void PlayerFound()
    {
        playerFound = true;
        
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }
        
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("Choose their fate");
        }
        
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void OnSpareClicked()
    {
        ReturnToMainMenu();
    }
    
    void OnKillClicked()
    {
        ReturnToMainMenu();
    }
    
    void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
}