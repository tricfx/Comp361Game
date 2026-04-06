using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ShopShuffleButton : MonoBehaviour
{
    [SerializeField] private ShopLogic shopLogic;
    [SerializeField] private TextMeshProUGUI rerollCountText;
    [SerializeField] private int maxRerolls = 3;
    [SerializeField] private Animator gongHammerAnimator;

    [SerializeField] private AudioSource clickAudioSource;
    [SerializeField] private AudioSource noRerollsAudioSource;
    [SerializeField] private Image shuffleImage;
    [SerializeField] private Color enabledColor = Color.white;
    [SerializeField] private Color disabledColor = new Color(1f, 1f, 1f, 0.45f);

    private static string prevSceneName = "";
    private static int rerollsUsedThisLevel = 0;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnShuffleClicked);

        SyncLevelState();
        RefreshUI();
    }

    private void OnEnable()
    {
        SyncLevelState();
        gongHammerAnimator.ResetTrigger("Hit");
        gongHammerAnimator.Play("GongIdle", 0, 0f);
        gongHammerAnimator.Update(0f);
        RefreshUI();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnShuffleClicked);
    }

    private void SyncLevelState()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (prevSceneName != currentSceneName)
        {
            prevSceneName = currentSceneName;
            rerollsUsedThisLevel = 0;
        }
    }

    private void OnShuffleClicked()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        SyncLevelState();

        if (rerollsUsedThisLevel >= maxRerolls)
        {
            noRerollsAudioSource.Play();
            RefreshUI();
            return;
        }

        clickAudioSource.Play();
        gongHammerAnimator.SetTrigger("Hit");
    }

    private void RefreshUI()
    {
        int rerollsLeft = Mathf.Max(0, maxRerolls - rerollsUsedThisLevel);
        bool greyOut = rerollsLeft <= 0;
        rerollCountText.text = $"Rerolls left: {rerollsLeft}/{maxRerolls}";

        shuffleImage.color = greyOut ? disabledColor : enabledColor;
        rerollCountText.color = greyOut ? disabledColor : enabledColor;
        button.interactable = true;
    }
    public void ApplyReroll()
    {
        bool success = shopLogic.RerollAbilities();
        if (!success) return;

        rerollsUsedThisLevel++;
        RefreshUI();
    }
}