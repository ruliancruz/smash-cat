using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public Button restartButton;
    public Button exitButton;
    private AudioSource audioSource;
    public AudioClip buttonClickSFX;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        restartButton.onClick.AddListener(OnRestart);
        exitButton.onClick.AddListener(OnExit);
    }

    void OnRestart()
    {
        audioSource.PlayOneShot(buttonClickSFX);
        SceneManager.LoadScene(2);
    }

    void OnExit()
    {
        audioSource.PlayOneShot(buttonClickSFX);
        SceneManager.LoadScene(1);
    }
}
