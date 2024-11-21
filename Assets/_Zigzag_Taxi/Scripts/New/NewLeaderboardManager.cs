using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;

public class NewLeaderboardManager : MonoBehaviour
{
    public List<PlayerData> leaderboard = new List<PlayerData>();

    [SerializeField] private GameObject container;
    [SerializeField] private ItemLead itemLead;

    public void Open()
    {

    }

public void Close()
    {

    }

    public void DisplayLeaderboard()
    {
        leaderboard = new List<PlayerData>();

        leaderboard.Add(new PlayerData { name = "Игорь", score = 1000 });
        leaderboard.Add(new PlayerData { name = "Сашка", score = 3000 });
        leaderboard.Add(new PlayerData { name = "Степан", score = 5000 });
        leaderboard.Add(new PlayerData { name = "Андрей", score = 6000 });

        string name = PlayerPrefs.GetString("PlayerName","Name");
        int score = PlayerPrefs.GetInt("PlayerScore" ,0);
        leaderboard.Add(new PlayerData { name = name, score = score });

        leaderboard.Sort((p1, p2) => p2.score.CompareTo(p1.score));

        foreach (var player in leaderboard)
        {
            Debug.Log("Созданипе ");
           ItemLead item = Instantiate(itemLead, container.transform);
            item.Init(player.name, player.score);
        }
    }
}
