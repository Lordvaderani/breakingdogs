using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private string message = "You just boinked doggos of level";

    void Start()
    {
        string lastLevel = PlayerPrefs.GetString("LastCompletedLevel");
        levelText.text = $"{message} {lastLevel} !";
    }

    public void LoadNextLevel()
    {
        string lastLevel = PlayerPrefs.GetString("LastCompletedLevel");
        int nextLevelNumber = int.Parse(lastLevel.Replace("Level", "")) + 1;
        // Save the next level as the completed level
        PlayerPrefs.SetString("LastCompletedLevel", $"Level{nextLevelNumber}");
        UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{nextLevelNumber}");
    }

    public void ReloadLevel()
    {
        // Reload the exact level that was last completed
        string lastLevel = PlayerPrefs.GetString("LastCompletedLevel");
        UnityEngine.SceneManagement.SceneManager.LoadScene(lastLevel);
    }

    public void HomeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
}