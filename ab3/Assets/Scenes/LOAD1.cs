using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LOAD1 : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("cutscene");
    }
}
