using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance;

    public CardDatabase rewardDatabase;

    public CardUI[] cardSlots;
    public GameObject rewardPanel;

    private Player player;
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

        while (currentRewards.Count < 3)
        {
            Card reward = rewardDatabase.GetRandomReward();

            if (!currentRewards.Contains(reward))
                currentRewards.Add(reward);
        }

        for (int i = 0; i < 3; i++)
        {
            cardSlots[i].Setup(currentRewards[i]);
        }
    }

    public void OnRewardSelected(Card reward)
    {
        //reward.Apply(player);
        rewardPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game after selecting a reward
    }
}