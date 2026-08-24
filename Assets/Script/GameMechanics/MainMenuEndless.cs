using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEndless : MonoBehaviour
{
    public static MainMenuEndless Instance;
    public bool isEndlessing = false;
    public void PlayGame()
    {
        isEndlessing = true;
        SceneManager.LoadScene("MainGame");
    }
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
