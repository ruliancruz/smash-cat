using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button unpauseButton;
    public Button restartButton;
    public Button exitButton;
    private bool isPaused;
    public AudioSource audioSource;
    public AudioClip pauseIn;
    public AudioClip pauseOut;
    public AudioClip buttonClickSFX;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1;
        unpauseButton.onClick.AddListener(OnResume);
        restartButton.onClick.AddListener(OnRestart);
        exitButton.onClick.AddListener(OnExit);
    }

    void OnResume()
    {
        audioSource.PlayOneShot(buttonClickSFX);
        ChangePauseState();
    }

    void OnRestart()
    {
        audioSource.PlayOneShot(buttonClickSFX);
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    void OnExit()
    {
        audioSource.PlayOneShot(buttonClickSFX);
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangePauseState();
        }
    }

    void ChangePauseState()
    {
        if (isPaused)
        {
            audioSource.PlayOneShot(pauseOut);
        } else
        {
            audioSource.PlayOneShot(pauseIn);
        }
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

}
