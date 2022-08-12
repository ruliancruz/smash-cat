using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameScene : MonoBehaviour
{
    public Button startButton;
    private AudioSource audioSource;
    public AudioClip buttonClickSFX;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startButton.onClick.AddListener(OnStart);
    }
    void OnStart()
    {
        audioSource.PlayOneShot(buttonClickSFX);
        SceneManager.LoadScene(1);
    }

}
