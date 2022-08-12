using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    void Start()
    {
        Invoke("GoToGame", 10f);
    }

    void GoToGame()
    {
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            GoToGame();
        }
    }
}
