using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject quitGameButton;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitGameButton.SetActive(false);
        }
    }
}
