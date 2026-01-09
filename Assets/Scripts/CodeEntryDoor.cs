using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CodeEntryDoor : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "Level2";
    
    [Header("UI References")]
    [SerializeField] private GameObject codeEntryPanel;
    [SerializeField] private InputField codeInputField;
    [SerializeField] private Text feedbackText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button submitButton;
    
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    [Header("Monster Settings")]
    [SerializeField] private GameObject monsterObject;
    
    private bool playerInRange = false;
    private Transform player;
    private bool isCodePanelOpen = false;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        
        if (monsterObject == null)
        {
            monsterObject = GameObject.FindGameObjectWithTag("Monster");
        }
        
        if (codeEntryPanel != null)
        {
            codeEntryPanel.SetActive(false);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseCodePanel);
        }
        
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SubmitCode);
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= interactDistance;
        
        if (playerInRange && Input.GetKeyDown(interactKey) && !isCodePanelOpen)
        {
            OpenCodePanel();
        }
        
        if (isCodePanelOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCodePanel();
        }
        
        if (isCodePanelOpen && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            SubmitCode();
        }
    }
    
    void OpenCodePanel()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("Count The Paintings");
        }
        
        isCodePanelOpen = true;
        
        if (codeEntryPanel != null)
        {
            codeEntryPanel.SetActive(true);
        }
        
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (codeInputField != null)
        {
            codeInputField.text = "";
            codeInputField.ActivateInputField();
            codeInputField.Select();
        }
        
        if (feedbackText != null)
        {
            feedbackText.text = "Enter the code to escape";
            feedbackText.color = Color.white;
        }
    }
    
    void CloseCodePanel()
    {
        isCodePanelOpen = false;
        
        if (codeEntryPanel != null)
        {
            codeEntryPanel.SetActive(false);
        }
        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void SubmitCode()
    {
        if (codeInputField == null) return;
        
        string enteredCode = codeInputField.text;
        
        if (enteredCode == "11")
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Correct! Escaping...";
                feedbackText.color = Color.green;
            }
            
            if (monsterObject != null)
            {
                monsterObject.SetActive(false);
            }
            
            Invoke(nameof(LoadNextScene), 1f);
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text = "WRONG CODE!";
                feedbackText.color = Color.red;
            }
        }
    }
    
    void LoadNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
    
    void OnGUI()
    {
        if (playerInRange && !isCodePanelOpen)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            
            GUI.Label(new Rect(0, screenHeight - 100, screenWidth, 50), "Press E to Enter Code", style);
        }
    }
}