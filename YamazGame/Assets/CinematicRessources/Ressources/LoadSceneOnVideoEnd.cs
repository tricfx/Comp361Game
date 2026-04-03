using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadSceneOnVideoEnd : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    public string nextSceneName;

    void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
    }

    void OnEnable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if(nextSceneName == ""){
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
