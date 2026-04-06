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
        if (SceneManager.GetActiveScene().buildIndex== 12)
        {
            if (DataPersistenceManager.instance != null){
            DataPersistenceManager.instance.NewGame();
            DataPersistenceManager.instance.SaveGame();
            }
        Destroy(DataPersistenceManager.instance);
        }
        if(nextSceneName == ""){
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
