
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;



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
    private bool gameWon = false;

    private void Start()
    {
        if(EventManager.Instance != null)
        {
            EventManager.Instance.onBetClicked += HandleJackPotMachine;
            EventManager.Instance.onScrollCompleted += HandleScrollComplete;
        }

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
                    PlayerPrefs.Save();
                }
                gameWon = true;
                uiManager.DisplayScores(score, highScore);
                uiManager.ShowGameEndMessage(true,score,highScore);
                return;
            case 2: 
                score +=  Mathf.RoundToInt((currentBet + value) * 1.1f); // small win
                if (score > highScore)
                {
                    highScore = score;
                    PlayerPrefs.SetInt("HIGHSCORE", highScore);
                    PlayerPrefs.Save();
                }
                uiManager.DisplayScores(score, highScore);
                break;
            default: break;
        }
        if (score >= 5)
        {
            uiManager.ShowBetOptions(score);
        }
        else
        {
            uiManager.ShowGameEndMessage(false, score, highScore);
        }
    }

    // pre determining where the symbol will land on
    private string PickSymbol()
    {
        return allSymbols[RandomNumberGenerator.GetInt32(0, allSymbols.Count)].symbolNameTage;
    }

    private void HandleJackPotMachine(int betAmount)
    {
        uiManager.HideBetOptions();
        currentBet = betAmount;
        highScore = score > highScore ? score : highScore;
        score -= currentBet; // handling score after each bet
        uiManager.DisplayScores(score, highScore);
        uiManager.SetRandomSymbolsForAllSlots(PickSymbol(), PickSymbol(), PickSymbol());
        uiManager.StartJackPotMachine();

    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.onBetClicked -= HandleJackPotMachine;
            EventManager.Instance.onScrollCompleted -= HandleScrollComplete;
        }
    }

    public void RestartGame()
    {
        if(!gameWon)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            gameWon = false;
            uiManager.HandleContinueGame(score);
        }
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
    #elif UNITY_WEBGL
        uiManager.ShowThankYouMessage(); //for web build
    #else
        Application.Quit();
    #endif
    }

}

