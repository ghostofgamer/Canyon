using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    public InputField nameInputField;
    public NewLeaderboardManager leaderboardManager;
    public LeaderboardViewController leaderboardViewController;

    public void OnSubmitName()
    {
        string playerName = nameInputField.text;
        int playerScore = 0;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt("PlayerScore", playerScore);
        //leaderboardManager.AddPlayer(playerName, playerScore);
        leaderboardManager.DisplayLeaderboard();
    }
}
