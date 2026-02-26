using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance;

    public CardDatabase rewardDatabase;

    public CardUI[] cardSlots;
    public GameObject rewardPanel;

    public Player player;
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
        Time.timeScale = 0f; // Pause the game while the reward screen is open

        
    }

    public void Roll3Rewards()
    {
        currentRewards.Clear();
        int rolls = 0;
        while (currentRewards.Count < 3)
        {
            Card reward = rewardDatabase.GetRandomReward();
            
            string depedency = reward.dependency;
            if (depedency != "")
            {
                if (!player.activeBuffs.Contains(depedency)) {
                    continue;
                }
            }
            
            if (!currentRewards.Contains(reward) && !player.activeBuffs.Contains(reward.cardID))
                currentRewards.Add(reward);
            rolls++;
            if (rolls == 200) {
                break;
            }
        }

        for (int i = 0; i < currentRewards.Count; i++)
        {
            cardSlots[i].Setup(currentRewards[i]);
        }
    }

    public void OnRewardSelected(Card reward)
    {
        reward.Apply(player);
        rewardPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game after selecting a reward
    }
}