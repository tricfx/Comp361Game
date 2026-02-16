using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;  
public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float delayTime = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void LoadLevel(int index)
    {
        StartCoroutine(load(index));
    }
    IEnumerator load(int index)
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(index);
    }
}
