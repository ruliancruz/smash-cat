using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;
    public Canvas canvas;
    public GameObject tutorial;
    private AudioSource audioSource;
    public AudioClip buttonClickSFX;
    private bool tutorialOn = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startButton.onClick.AddListener(OnStart);
        exitButton.onClick.AddListener(OnExit);
    }
    void OnStart()
    {
        audioSource.PlayOneShot(buttonClickSFX);
        tutorial.SetActive(true);
        canvas.enabled = false;
        tutorialOn = true;
        Invoke("GoToGame", 10f);
    }

    void GoToGame()
    {
        SceneManager.LoadScene(2);
    }

    void OnExit()
    {
        audioSource.PlayOneShot(buttonClickSFX);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
         Application.Quit();
        #endif
    }

    private void Update()
    {
        if (tutorialOn)
        {
            if (Input.anyKeyDown)
            {
                GoToGame();
            }
        }
    }
}
