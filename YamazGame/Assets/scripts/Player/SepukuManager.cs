using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class SepukuManager : MonoBehaviour, IDataPersistence
{

    [SerializeField] private int AgroEnnmies = 0;
    [SerializeField] private bool SepukuEnabled = false;
    public CanvasGroup SepukuExplanation;
    public AudioSource music;
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.LoadGame();
        }
        if (SepukuEnabled)
        {
            StartCoroutine(HandleSepuku());
        }

    }


    public void AddEnnemy()
    { 
    AgroEnnmies++;
    }
    public void RemoveEnnemy()
    {
        AgroEnnmies--;
    }


    public void SaveData(ref GameData data)
    {
        if (AgroEnnmies != 0)
            data.left_during_combat = true;
        else
            data.left_during_combat = false;
    }

    public void LoadData(GameData data)
    {
        SepukuEnabled = data.left_during_combat;
    }


    private IEnumerator HandleSepuku()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var input = player.GetComponent<PlayerController2D>();

        if (input != null)
            input.enabled = false;

        var animator = player.GetComponent<Animator>();

        if (animator != null)
            animator.enabled = false;

        yield return new WaitForSeconds(1f);
        animator.enabled = true;
 
        if (player != null)
        {
            var animController = player.GetComponent<PlayerAnimatorController>();
            if (animController != null)
            {
                //Replace this by the sepuku animation
                animController.TriggerSepuku();
            }
        }
        yield return new WaitForSeconds(2f);

        music.Play();

        // 2️ Show explanation UI with fade
        if (SepukuExplanation != null)
        {
            SepukuExplanation.gameObject.SetActive(true);
            SepukuExplanation.alpha = 0f;
            SepukuExplanation.interactable = false;
            SepukuExplanation.blocksRaycasts = false;

            float fadeDuration = 2f;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                SepukuExplanation.alpha = Mathf.Clamp01(timer / fadeDuration);
                yield return null;
            }

            SepukuExplanation.alpha = 1f;
            SepukuExplanation.interactable = true;
            SepukuExplanation.blocksRaycasts = true;
        }

        // 3️ Wait 5 seconds fully visible
        yield return new WaitForSeconds(6f);


        ForceOverwriteSaveWithDefaults();

        //Load scene 2 (first level)
        SceneManager.LoadScene(2);
        
    }


    private void ForceOverwriteSaveWithDefaults()
    {
        var dpm = DataPersistenceManager.instance;
        if (dpm == null) return;

        // 1) Reset in-memory
        dpm.NewGame();

        // 2) Grab the private 'dataHandler' field
        var dpmType = typeof(DataPersistenceManager);
        FieldInfo handlerField = dpmType.GetField("dataHandler", BindingFlags.Instance | BindingFlags.NonPublic);
        object handlerObj = handlerField?.GetValue(dpm);
        if (handlerObj == null)
        {
            Debug.LogError("ForceOverwriteSaveWithDefaults: couldn't access dataHandler.");
            return;
        }

        // 3) Call FileDataHandler.Save(gameData) directly (no IDataPersistence objects involved)
        MethodInfo saveMethod = handlerObj.GetType().GetMethod("Save", BindingFlags.Instance | BindingFlags.Public);
        if (saveMethod == null)
        {
            Debug.LogError("ForceOverwriteSaveWithDefaults: couldn't find Save(GameData) on FileDataHandler.");
            return;
        }

        // Get the fresh gameData field value and save it
        FieldInfo gameDataField = dpmType.GetField("gameData", BindingFlags.Instance | BindingFlags.Public);
        var freshData = gameDataField?.GetValue(dpm);

        saveMethod.Invoke(handlerObj, new object[] { freshData });

        Debug.Log("ForceOverwriteSaveWithDefaults: overwrote save file with defaults.");
    }

}
