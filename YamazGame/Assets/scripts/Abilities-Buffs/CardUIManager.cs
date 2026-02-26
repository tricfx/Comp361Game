using UnityEngine;
using System.Collections.Generic;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance;

    public CardDatabase rewardDatabase;

    public CardUI[] cardSlots;
    public GameObject rewardPanel;

    public GameObject player; // Assign the player GameObject in the Inspector
    private PlayerBuffs playerBuffs;

    private List<Card> currentRewards = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (player != null)
            playerBuffs = player.GetComponent<PlayerBuffs>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            OpenRewardScreen();
        }
    }

    public void OpenRewardScreen()
    {
        Roll3Rewards();
        rewardPanel.SetActive(true);
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
        reward.Apply(player);
        rewardPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}