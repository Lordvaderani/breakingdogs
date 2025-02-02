using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Lost : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private string message = "You just boinked doggos of ";

    void Start()
    {
        string lastLevel = PlayerPrefs.GetString("LastCompletedLevel");
        //levelText.text = $"{message} {lastLevel} !";
    }

    public void LoadNextLevel()
    {
        string lastLevel = PlayerPrefs.GetString("LastCompletedLevel");
        int nextLevelNumber = int.Parse(lastLevel.Replace("Level", "")) + 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{nextLevelNumber}");
    }

    public void ReloadLevel()
    {
        // Directly reload the level that was last completed
        string lastLevel = PlayerPrefs.GetString("LastCompletedLevel");
        UnityEngine.SceneManagement.SceneManager.LoadScene(lastLevel);
    }

    public void HomeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
}