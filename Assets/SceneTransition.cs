using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad = "Level2";
    [SerializeField] private bool useSceneIndex = false;
    [SerializeField] private int sceneIndex = 1;
    
    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float transitionDelay = 0f;
    
    [Header("Optional Effects")]
    [SerializeField] private bool showMessage = true;
    [SerializeField] private string message = "Press E to Exit";
    [SerializeField] private bool requireKeyPress = false;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    [Header("Monster Settings")]
    [SerializeField] private GameObject monsterObject;
    [SerializeField] private bool hideMonsterOnInteract = true;
    
    private bool playerInRange = false;
    
    void Start()
    {
        if (monsterObject == null)
        {
            monsterObject = GameObject.FindGameObjectWithTag("Monster");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (requireKeyPress)
            {
                playerInRange = true;
            }
            else
            {
                LoadNextScene();
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }
    
    void Update()
    {
        if (requireKeyPress && playerInRange && Input.GetKeyDown(interactKey))
        {
            if (hideMonsterOnInteract && monsterObject != null)
            {
                monsterObject.SetActive(false);
            }
            
            LoadNextScene();
        }
    }
    
    void LoadNextScene()
    {
        if (transitionDelay > 0)
        {
            Invoke(nameof(PerformSceneLoad), transitionDelay);
        }
        else
        {
            PerformSceneLoad();
        }
    }
    
    void PerformSceneLoad()
    {
        if (useSceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    void OnGUI()
    {
        if (showMessage && playerInRange && requireKeyPress)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            
            GUI.Label(new Rect(0, screenHeight - 100, screenWidth, 50), message, style);
        }
    }
}