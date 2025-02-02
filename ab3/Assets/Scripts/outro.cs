using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class outro : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Assign this in the Inspector

    private void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer not assigned!");
            return;
        }

        // Add listener for when the video ends
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void Update()
    {
        // Check if the user presses the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeScene();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        ChangeScene();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("Level7");
    }
}
