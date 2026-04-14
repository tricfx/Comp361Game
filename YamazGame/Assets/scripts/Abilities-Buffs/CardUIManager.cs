using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance;

    public CardDatabase rewardDatabase;

    public CardUI[] cardSlots;
    public CardUI[] replacementSlots;
    public GameObject rewardPanel;
    public GameObject replacementPanel;
    public Button rerollButton;
    public GameObject BlurOverlay;
    [SerializeField] private AudioSource abilityEquipAudioSource;
    [SerializeField] private float buttonLockDuration = 0.5f;

    public GameObject player; // Assign the player GameObject in the Inspector
    private PlayerBuffs playerBuffs;

    private List<Card> currentRewards = new();

    private AbilityCard pendingReplacementCard;

    private int rerolls = 3;

    public void RerollButton()
    {
        if (rerolls > 0)
        {
            rerolls--;
            Roll3Rewards();
            rerollButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Reroll  ( {rerolls} left )";
            if(rerolls <= 0) rerollButton.interactable = false;
            OpenRewardScreen();
        }
        
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        buttonLockDuration = 0.5f;
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        EnemyManager.OnAllEnemiesDefeated += OpenRewardScreen;
    }

    private void OnDisable()
    {
        EnemyManager.OnAllEnemiesDefeated -= OpenRewardScreen;
    }

    void Start()
    {
        if (player != null)
            playerBuffs = player.GetComponent<PlayerBuffs>();
        BlurOverlay.SetActive(false);
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //      if (CursorManager.Instance != null)
        //     CursorManager.Instance.ShowCursor();
        //     OpenRewardScreen();
        //     BlurOverlay.SetActive(true);
        // }
        // if(Input.GetKeyDown(KeyCode.T))
        // {
        //     if (CursorManager.Instance != null)
        //     CursorManager.Instance.ShowCursor();
        //     OpenReplacementUI(new AbilityCard { cardName = "Test Ability", cardDescription = "This is a test ability.", cardID = "test_ability" });
        // }
    }

    public void OpenRewardScreen()
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.ShowCursor();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player"); 
            if (player != null) playerBuffs = player.GetComponent<PlayerBuffs>();
        }

        if (player == null) {
            Debug.LogError("CardUIManager: No Player found in this scene!");
            return;
        }

        Roll3Rewards();
        rewardPanel.SetActive(true);
        TemporarilyLockPanelButtons(rewardPanel);
        BlurOverlay.SetActive(true);
        var actions = player.GetComponent<PlayerActions>();
        if (actions != null) actions.enabled = false;
        Time.timeScale = 0f;
    }

    public void Roll3Rewards()
    {
        currentRewards.Clear();
        int rolls = 0;

        while (currentRewards.Count < 3)
        {
            Card reward = rewardDatabase.GetRandomReward();
            
            // Check dependency
            string dependency = reward.dependency;
            if (dependency != "" && (playerBuffs == null || !playerBuffs.HasBuff(dependency)))
            {
                rolls++;
                if (rolls >= 200) break;
                continue;
            }

            var actions = player.GetComponent<PlayerActions>();

            if (actions != null)
            {
                if (actions.qAbilityCard != null && actions.qAbilityCard.cardID == reward.cardID) continue;
                if (actions.eAbilityCard != null && actions.eAbilityCard.cardID == reward.cardID) continue;
            }
            // No duplicates and not already owned
            if (!currentRewards.Contains(reward) &&
                (playerBuffs == null || !playerBuffs.HasBuff(reward.cardID)))
            {
                currentRewards.Add(reward);
            }

            rolls++;
            if (rolls >= 200) break;
        }

        if (currentRewards.Count == 0)
        {
            Debug.LogWarning("No valid rewards available to roll.");
            return;
        }

        // Fill remaining slots if fewer than 3 unique rewards exist
        int fillIndex = 0;
        while (currentRewards.Count < 3)
        {
            currentRewards.Add(currentRewards[fillIndex % currentRewards.Count]);
            fillIndex++;
        }

        for (int i = 0; i < cardSlots.Length; i++)
        {
            cardSlots[i].Setup(currentRewards[i]);
        }
    }

    public void OnRewardSelected(Card reward)
    {
        if (reward is AbilityCard abilityCard)
        {
            player.GetComponent<PlayerActions>().TryEquipAbility(abilityCard);
            if (replacementPanel.activeSelf)
            {
                rewardPanel.SetActive(false);
                return;
            }
        }
        else
        {
            reward.Apply(player);
        }

        abilityEquipAudioSource.Play();
        rewardPanel.SetActive(false);
        BlurOverlay.SetActive(false);
        rerolls = 3;
        rerollButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Reroll  ( {rerolls} left )";
        player.GetComponent<PlayerActions>().enabled = true;
        Time.timeScale = 1f;
        if (CursorManager.Instance != null)
        CursorManager.Instance.HideCursor();
    }

    public void OpenReplacementUI(AbilityCard newCard)
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.ShowCursor();
        pendingReplacementCard = newCard;
        replacementSlots[0].Setup(player.GetComponent<PlayerActions>().qAbilityCard);
        replacementSlots[1].Setup(player.GetComponent<PlayerActions>().eAbilityCard);
        replacementPanel.SetActive(true);
        TemporarilyLockPanelButtons(replacementPanel);
        player.GetComponent<PlayerActions>().enabled = false;
        Time.timeScale = 0f;

    }
    public void OnReplaceSelected(bool replaceQ)
    {
        var playerActions = player.GetComponent<PlayerActions>();

        playerActions.ReplaceAbilitySlot(replaceQ, pendingReplacementCard);
        abilityEquipAudioSource.Play();

        pendingReplacementCard = null;
        replacementPanel.SetActive(false);
        player.GetComponent<PlayerActions>().enabled = true;
        Time.timeScale = 1f;
         if (CursorManager.Instance != null)
         CursorManager.Instance.HideCursor();
    }
    private void TemporarilyLockPanelButtons(GameObject panel)
    {
        if (panel == null) return;
        StartCoroutine(TemporarilyLockPanelButtonsRoutine(panel));
    }

    private IEnumerator TemporarilyLockPanelButtonsRoutine(GameObject panel)
    {
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
            button.interactable = false;

        yield return new WaitForSecondsRealtime(buttonLockDuration);

        foreach (Button button in buttons)
            button.interactable = true;
    }

}