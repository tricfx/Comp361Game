using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public AudioSource audioSource;
    public AudioSource shopkeeperMusic;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    public float typingSpeed = 0.2f;
    public float fadeDuration = 2f;

    public Animator animator;
    public GameObject choicePanel;
    public GameObject continueButton;
    public Button yesButton;
    public Button noButton;

    public PlayerInput playerInput;
    public Rigidbody2D playerRb;
    public Button exitShopButton;

    public SlideFader fader;
    public CanvasGroup dialogueCG;
    public CanvasGroup shopCG;
    public CanvasGroup GameplayCG;
    public AudioSource shopMenuMusic;
    public AudioSource shermaSong;

    private Queue<DialogueLine> lines = new();
    private Action yesAction;
    private Action noAction;
    private bool closeInstant;
    private bool shopOpen;
    private Coroutine shopFadeCoroutine;
    private float shopOgVolume;
    public VideoPlayer shopkeeperVideo;
    [SerializeField] private ShopLogic shopLogic;

    public AudioSource levelMusic;
    public AudioFadeMemory audioFade;

    [Header("UI SFX")]
    public AudioSource uiSfxSource;
    public AudioClip dialogueOpenSfx;
    public AudioClip dialogueCloseSfx;
    private bool dialogueWasVisible = false;

    private Coroutine openShopCo;
    private Coroutine closeShopCo;
    private bool videoPrepared = false;
    public bool IsShopTransitioning => openShopCo != null || closeShopCo != null;

    public GameObject interactPrompt;


    public bool isDialogueActive { get; private set; }

    private IEnumerator Start()
    {
        if (shopCG != null)
        {
            shopCG.gameObject.SetActive(true);
            shopCG.alpha = 0f;
            shopCG.interactable = false;
            shopCG.blocksRaycasts = false;
        }

        if (shopkeeperVideo != null)
        {
            shopkeeperVideo.gameObject.SetActive(true);
            shopkeeperVideo.enabled = true;
            shopkeeperVideo.playOnAwake = false;
            shopkeeperVideo.waitForFirstFrame = false;

            shopkeeperVideo.Stop();
            shopkeeperVideo.time = 0;
            shopkeeperVideo.Prepare();

            while (!shopkeeperVideo.isPrepared)
                yield return null;

            videoPrepared = true;

            shopkeeperVideo.Pause();
            shopkeeperVideo.time = 0;
        }
    }


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        audioFade ??= GetComponent<AudioFadeMemory>();
        shopOgVolume = shopkeeperMusic.volume;

        continueButton.SetActive(false);
        choicePanel.SetActive(false);

        yesButton.onClick.AddListener(ChooseYes);
        noButton.onClick.AddListener(ChooseNo);
        exitShopButton.onClick.AddListener(CloseShop);

        playerInput ??= FindFirstObjectByType<PlayerInput>();
        playerRb ??= playerInput.GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (isDialogueActive)
        {
        if (CursorManager.Instance != null)
            CursorManager.Instance.ShowCursor();
        }
    }

    private void SetPlayerLocked(bool locked)
    {
        if (locked) { playerInput.DeactivateInput(); playerRb.linearVelocity = Vector2.zero; }
        else playerInput.ActivateInput();
    }

    private void BeginSession()
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.ShowCursor();
        Time.timeScale = 0f;
        interactPrompt.SetActive(false);
        if (!dialogueWasVisible && uiSfxSource != null && dialogueOpenSfx != null)
            uiSfxSource.PlayOneShot(dialogueOpenSfx);

        dialogueWasVisible = true;
        isDialogueActive = true;
        dialogueCG.gameObject.SetActive(true);
        dialogueCG.alpha = 1f;
        dialogueCG.interactable = true;
        dialogueCG.blocksRaycasts = true;
        animator.Play("BoxIn");
        SetPlayerLocked(true);
        audioSource.mute = true;
        choicePanel.SetActive(false);

        audioFade.FadeOutAndPause(levelMusic, fadeDuration);
        audioFade.FadeOutAndPause(shermaSong, fadeDuration);

        audioFade.FadeInAndResume(shopkeeperMusic, fadeDuration);

        if (shopFadeCoroutine != null) StopCoroutine(shopFadeCoroutine);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.ShowCursor();
        BeginSession();
        continueButton.SetActive(true);

        lines.Clear();
        foreach (var line in dialogue.dialogueLines) lines.Enqueue(line);

        DisplayNextDialogueLine();
    }

    public void StartChoice(string prompt, Action onYes, Action onNo)
    {
        BeginSession();
        continueButton.SetActive(false);
        lines.Clear();
        StopAllCoroutines();

        dialogueArea.text = prompt;
        yesAction = onYes;
        noAction = onNo;
        choicePanel.SetActive(true);
    }

    public void StartSingleLine(string line)
    {
        BeginSession();
        continueButton.SetActive(true);
        lines.Clear();
        StopAllCoroutines();
        StartCoroutine(TypeLine(line));
    }

    public void DisplayNextDialogueLine()
    {
        if (closeInstant) { closeInstant = false; EndDialogue(); return; }
        if (lines.Count == 0) { EndDialogue(); return; }

        var current = lines.Dequeue();
        characterIcon.sprite = current.character.icon;
        characterName.text = current.character.name;

        StopAllCoroutines();
        StartCoroutine(TypeLine(current.line));
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueArea.text = "";
        foreach (char letter in line)
        {
            dialogueArea.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    private IEnumerator FadeOut(AudioSource src, float duration)
    {
        float startVol = src.volume;
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            src.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        src.volume = 0f;
        src.Stop();
    }

    public void ChooseYes()
    {
        choicePanel.SetActive(false);
        yesAction?.Invoke();
        yesAction = noAction = null;
        if (CursorManager.Instance != null)
            CursorManager.Instance.ShowCursor();
    }

    public void ChooseNo()
    {
        choicePanel.SetActive(false);
        closeInstant = true;
        noAction?.Invoke();
        yesAction = noAction = null;
    }

    private void EndDialogue()
    {
        Time.timeScale = 1f;
        interactPrompt.SetActive(true);
        isDialogueActive = false;
        if (uiSfxSource != null && dialogueCloseSfx != null)
            uiSfxSource.PlayOneShot(dialogueCloseSfx);

        dialogueWasVisible = false; ;

        animator.Play("BoxOut");
        choicePanel.SetActive(false);
        continueButton.SetActive(false);
        if (!shopOpen) SetPlayerLocked(false);

        if (shopFadeCoroutine != null) StopCoroutine(shopFadeCoroutine);
        audioFade.FadeOutAndPause(shopkeeperMusic, fadeDuration);
        if (!shopOpen)
        {
            audioFade.FadeInAndResume(levelMusic, fadeDuration);
            audioFade.FadeInAndResume(shermaSong, fadeDuration);
        }

        audioSource.mute = false;
        if (CursorManager.Instance != null)
            CursorManager.Instance.HideCursor();
    }

    public void OpenShop()
    {
        if (shopOpen || openShopCo != null || closeShopCo != null) return;
        openShopCo = StartCoroutine(OpenShopRoutine());
        if (CursorManager.Instance != null)
            CursorManager.Instance.ShowCursor();
    }

    private IEnumerator OpenShopRoutine()
    {
        bool finished = false;
        
        try
        {
            shopOpen = true;
            isDialogueActive = false;
            continueButton.SetActive(false);
            choicePanel.SetActive(false);
            animator.Play("BoxOut");
            shopLogic.PrepareShop();

            var rt = shopCG.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0f, fader.slideOffset);

            shopCG.gameObject.SetActive(true);
            shopCG.alpha = 0f;
            yield return null;
            shopCG.alpha = 1f;

            audioFade.FadeOutAndPause(shopkeeperMusic, fadeDuration);
            audioFade.FadeOutAndPause(levelMusic, fadeDuration);
            audioFade.FadeOutAndPause(shermaSong, fadeDuration);

            audioFade.FadeInAndResumeAllOnObject(shopMenuMusic, fadeDuration);

            fader.SlideIn(shopCG);

            if (shopkeeperVideo != null)
            {
                shopkeeperVideo.gameObject.SetActive(true);
                shopkeeperVideo.enabled = true;

                if (!videoPrepared)
                {
                    shopkeeperVideo.Stop();
                    shopkeeperVideo.time = 0;
                    shopkeeperVideo.Prepare();
                    while (!shopkeeperVideo.isPrepared)
                        yield return null;

                    videoPrepared = true;
                }

                shopkeeperVideo.time = 0;
                shopkeeperVideo.Play();
            }


            dialogueCG.blocksRaycasts = false;
            dialogueCG.interactable = false;
            dialogueCG.alpha = 0f;

            yield return new WaitForSecondsRealtime(fader.slideDuration);
            Time.timeScale = 0f;

            finished = true;
        }
        finally
        {
            openShopCo = null;

            if (!finished)
            {
                Time.timeScale = 1f;
                shopOpen = false;
            }
        }
    }

    public void CloseShop()
    {
        if (!shopOpen || closeShopCo != null) return;
        closeShopCo = StartCoroutine(CloseShopRoutine());
        if (CursorManager.Instance != null)
        CursorManager.Instance.HideCursor();
    }


    private IEnumerator CloseShopRoutine()
    {
        try
        {
            if (openShopCo != null)
            {
                StopCoroutine(openShopCo);
                openShopCo = null;
            }

            Time.timeScale = 1f;

            audioFade.FadeOutAndPauseAllOnObject(shopMenuMusic, fadeDuration);
            audioFade.FadeInAndResume(levelMusic, fadeDuration);
            audioFade.FadeInAndResume(shermaSong, fadeDuration);

            GameplayCG.gameObject.SetActive(true);
            SetPlayerLocked(false);

            fader.SlideOut(shopCG);
            yield return new WaitForSecondsRealtime(fader.slideDuration);

            if (shopkeeperVideo.isActiveAndEnabled)
            {
                shopkeeperVideo.Pause();
            }
            shopkeeperVideo.time = 0;
            
            shopCG.gameObject.SetActive(false);
            shopOpen = false;

            shermaSong.mute = false;

            dialogueCG.gameObject.SetActive(true);
            dialogueCG.alpha = 0f;
            dialogueCG.interactable = false;
            dialogueCG.blocksRaycasts = false;
            isDialogueActive = false;
            dialogueWasVisible = false;
        }
        finally
        {
            closeShopCo = null;
            Time.timeScale = 1f;
        }
    }

}