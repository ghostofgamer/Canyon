using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLead : MonoBehaviour
{
    public string name;
    public int score;
    public Text nametext;
    public Text scoretext;

    public void Init(string nameValue ,int scoreValue)
    {
        name = nameValue;
        score = scoreValue;
        Show();
    }

    public void Show()
    {
        nametext.text = name;
        scoretext.text = score.ToString();
    }
}
