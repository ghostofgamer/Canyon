using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour
{
    private void Update()
    {

        //Get time rewmains till next reward
        double timeRemains = ServicesManager.Instance.DailyRewardManager.TimeRemainsTillNextReward();

        //Update the texts and buttons
        if (timeRemains > 0)
        {

            gameObject.SetActive(false);
           
        }
        else
        {
           
           gameObject.SetActive(true);
        }

    }
}
