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
    }
    
    void OpenCodePanel()
    {
        if (PaintingManager.Instance != null)
        {
            PaintingManager.Instance.RevealPaintingUI();
        }
        
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("Count all paintings in the maze");
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
        }
        
        if (feedbackText != null)
        {
            feedbackText.text = "Enter the number of paintings in the maze";
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
        if (codeInputField == null || PaintingManager.Instance == null) return;
        
        string enteredCode = codeInputField.text;
        int correctCode = PaintingManager.Instance.GetTotalPaintings();
        
        if (int.TryParse(enteredCode, out int code))
        {
            if (code == correctCode)
            {
                if (PaintingManager.Instance.HasCountedAllPaintings())
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
                        int remaining = correctCode - PaintingManager.Instance.GetPaintingsFound();
                        feedbackText.text = "Correct code, but you need to count " + remaining + " more painting(s)!";
                        feedbackText.color = Color.yellow;
                    }
                    
                    Invoke(nameof(CloseCodePanel), 2f);
                }
            }
            else
            {
                if (feedbackText != null)
                {
                    feedbackText.text = "WRONG CODE! Monster alerted!";
                    feedbackText.color = Color.red;
                }
                
                //AlertMonsterToDoor();
                Invoke(nameof(CloseCodePanel), 1.5f);
            }
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Please enter a valid number";
                feedbackText.color = Color.red;
            }
        }
    }
    
    /*void AlertMonsterToDoor()
    {
        MonsterAI monsterAI = FindObjectOfType<MonsterAI>();
        if (monsterAI != null)
        {
            monsterAI.AlertMonster(transform.position);
        }
    }*/
    
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