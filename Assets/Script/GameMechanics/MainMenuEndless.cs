using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEndless : MonoBehaviour
{
    public static MainMenuEndless Instance;
    public bool isEndlessing = false;

    public TextMeshProUGUI endlessText;
    public void PlayGame()
    {
        if (PlayerPrefs.GetInt("EndlessModeUnlocked", 1) == 1)
        {
            return;
        }
        isEndlessing = true;
        SceneManager.LoadScene("MainGame");
    }
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void CheckIfCanEndLess()
    {
        if (PlayerPrefs.GetInt("EndlessModeUnlocked", 1) == 2)
        {
            return;
        }
        else
        {
            endlessText.color = Color.grey;
        }
    }
}
