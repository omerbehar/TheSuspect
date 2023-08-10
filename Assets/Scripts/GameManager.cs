using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] public List<LevelDesign> levelDesigns;
    public LevelDesign currentLevel;
    private const string LOADING_SCENE = "LoadingScene";
    private const string INTRO_SCENE1 = "IntroScene1";
    private const string INTRO_SCENE2 = "IntroScene2";
    [SerializeField] private Transform loadingImage;
    [SerializeField] private Button nextButton;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }
    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.isLoaded) return;
        if (SceneManager.GetActiveScene().name == LOADING_SCENE)
            StartCoroutine(LoadGame());
        if (SceneManager.GetActiveScene().name is INTRO_SCENE1 or INTRO_SCENE2)
        {
            nextButton = FindObjectOfType<Button>();
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
        
    }

    private IEnumerator LoadGame()
    {
        //rotate loadingImage half a circle each second for x seconds then load game
        for (int i = 0; i < 30; i++)
        {
            loadingImage.Rotate(0, 0, 18);
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene(INTRO_SCENE1);
    }

    private void OnNextButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
