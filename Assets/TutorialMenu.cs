using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMenu : MonoBehaviour
{
    void Start()
    {
        Invoke("GoToGame", 5f);
    }

    void GoToGame()
    {
        SceneManager.LoadScene(2);
    }
}
