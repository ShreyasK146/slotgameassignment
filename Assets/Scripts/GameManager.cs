using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[Serializable]
public class Symbol
{
    public string symbolNameTage;
    public int symbolValue;
}

public class GameManager : MonoBehaviour
{
    public List<Symbol> allSymbols = new List<Symbol>();
    [SerializeField] private UIManager uiManager;
    private int score = 50; // starting amount of money what every player has
    private int highScore = 50; // lets consider starting money as initial high score
    List<string> result = new List<string>();
    private int currentBet;
    private void Start()
    {
        EventManager.Instance.onBetClicked += HandleJackPotMachine;
        EventManager.Instance.onScrollCompleted += HandleScrollComplete;
        highScore = PlayerPrefs.GetInt("HIGHSCORE", highScore);
        uiManager.DisplayScores(score,highScore);
    }

    private void HandleScrollComplete()
    {
        result.Clear();
        result.AddRange(uiManager.GetSymbolsInThreeSlots());
        CalculateScore(result);
    }

    // group by count and then see how many groupings we did of differnet symbol
    // 3 same symbol win , 2 same symbol small amount added else 0 is added
    private void CalculateScore(List<string> result)
    {
        var groups = result.GroupBy(tagName => tagName).OrderByDescending(grp => grp.Count());
        int distinctSymbols = groups.Count(); // 3 same symbol meaning we should have count of 1
        var bestGroup = groups.First();

        int value = allSymbols.Find(symbol => symbol.symbolNameTage == bestGroup.Key).symbolValue;

        switch (distinctSymbols)
        {
            case 1:
                score += currentBet * value * 5; // max win
                if (score > highScore)
                {
                    highScore = score;
                    PlayerPrefs.SetInt("HIGHSCORE", highScore);
                }
                uiManager.DisplayScores(score, highScore);
                uiManager.ShowGameEndMessage(true,score,highScore);
                return;
            case 2: 
                score +=  currentBet * 2 + value * 2; // small win
                if (score > highScore)
                {
                    highScore = score;
                    PlayerPrefs.SetInt("HIGHSCORE", highScore);
                }
                uiManager.DisplayScores(score, highScore);
                break;
            default: break;
        }
        if (score >= 5)
        {
            Debug.Log("showing option to bet again");
            uiManager.ShowBetOptions(score);
        }
        else
        {
            Debug.Log("lost");  
            uiManager.ShowGameEndMessage(false, score, highScore);
        }
    }

    private void HandleJackPotMachine(int betAmount)
    {
        uiManager.HideBetOptions();
        currentBet = betAmount;
        highScore = score > highScore ? score : highScore;
        score -= currentBet;
        uiManager.DisplayScores(score, highScore);
        uiManager.StartJackPotMachine();

    }

    private void OnDestroy()
    {
        EventManager.Instance.onBetClicked -= HandleJackPotMachine;
        EventManager.Instance.onScrollCompleted -= HandleScrollComplete;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}

