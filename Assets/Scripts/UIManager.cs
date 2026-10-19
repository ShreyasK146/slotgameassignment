
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Collections.Generic;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Image jackPotHandleImage;
    [SerializeField] private Image jackPotHandleImage2;
    [SerializeField] private RectTransform slotReel1;
    [SerializeField] private RectTransform slotReel2;
    [SerializeField] private RectTransform slotReel3;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Image betOptionsImage;
    [SerializeField] private Image gameEndedImage;
    [SerializeField] private Image guitGameImage;
    [SerializeField] private TextMeshProUGUI gameEndText;
    [SerializeField] private Button bet_5Button;
    [SerializeField] private Button bet_10Button;
    [SerializeField] private Button bet_20Button;
    [SerializeField] private Button bet_50Button;
    [SerializeField] private Button bet_100Button;
    [SerializeField] private List<string> symbolsymbolNameTags = new List<string>();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip reelMovementClip;

    private int symbolHeight = 250;
    private int scrollSpeed = 1500;

    public void HideBetOptions()
    {
        StartCoroutine(DoJackpotHandleAnimation());
        betOptionsImage.gameObject.SetActive(false);
    }
    public void ShowBetOptions(int score)
    {
        HandleButtonInteractible(score);
        betOptionsImage.gameObject.SetActive(true);
    }

    public void ShowGameEndMessage(bool wonTheGame, int currentScore, int highScore)
    {
        if (wonTheGame)
        { 
            string message = currentScore < highScore ? "your current score is less than high score continue playing more beat highscore ?" : "you achieved new highscore continue playing?";
            gameEndText.text = "You Won! " + message;
        }
        else
        {
            string message = currentScore < highScore ? "try again ?": "with new highscore.. too greedy heh";
            gameEndText.text = "You Lost! " + message;
        }
        gameEndedImage.gameObject.SetActive(true);
    }

    public void DisplayScores(int currentScore, int highScore)
    {
        currentScoreText.text = "In Pocket: " + currentScore.ToString();
        highScoreText.text = "HighScore: " + highScore.ToString();
    }

    public IEnumerator DoJackpotHandleAnimation()
    {
        jackPotHandleImage.gameObject.SetActive(false);
        jackPotHandleImage2.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        jackPotHandleImage2.gameObject.SetActive(false);
        jackPotHandleImage.gameObject.SetActive(true);
    }

    public void StartJackPotMachine()
    {
        StartCoroutine(HandleSpinSlots());

    }

    private IEnumerator HandleSpinSlots()
    {
        StartCoroutine(SpinReel(slotReel1, PickSymbol(), 2f));
        audioSource.PlayOneShot(reelMovementClip);
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(reelMovementClip);
        StartCoroutine(SpinReel(slotReel2, PickSymbol(), 2.5f));
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(reelMovementClip);
        StartCoroutine(SpinReel(slotReel3, PickSymbol(), 3f));
        yield return new WaitForSeconds(4f);
        EventManager.Instance.ScrollCompleted();
    }

    // pre determining where the symbol will land on
    private string PickSymbol()
    {
        return symbolsymbolNameTags[RandomNumberGenerator.GetInt32(0, symbolsymbolNameTags.Count)];
    }

    private IEnumerator SpinReel(RectTransform slotReel, string target, float minTime)
    {
        float startY = slotReel.anchoredPosition.y;
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            slotReel.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            if(slotReel.anchoredPosition.y >= startY + symbolHeight)
            {
                slotReel.GetChild(0).SetAsLastSibling();
                slotReel.anchoredPosition -= Vector2.up * symbolHeight;

                //stop animation after the mintime and only when the symbol lands on predetermined symbol
                if (time >= minTime && slotReel.GetChild(1).tag == target)
                {
                    break;
                }
            }
            yield return null;
        }
        slotReel.anchoredPosition = new Vector2(slotReel.anchoredPosition.x, startY); // properly place it after spin just in case
    }

    // returns 3 slot result after the roll is complete
    public List<string> GetSymbolsInThreeSlots()
    {
        List<string> results = new List<string>();
        results.Add(slotReel1.GetChild(1).tag);
        results.Add(slotReel2.GetChild(1).tag);
        results.Add(slotReel3.GetChild(1).tag);
        return results;
    }

    public void HandleButtonInteractible(int currentScore)
    {
        if (currentScore >= 100)
        {
            bet_5Button.interactable = true;
            bet_10Button.interactable = true;
            bet_20Button.interactable = true;
            bet_50Button.interactable = true;
            bet_100Button.interactable = true;
        }
        else if (currentScore >= 50)
        {
            bet_5Button.interactable = true;
            bet_10Button.interactable = true;
            bet_20Button.interactable = true;
            bet_50Button.interactable = true;
            bet_100Button.interactable = false;
        }
        else if(currentScore >= 20)
        {
            bet_5Button.interactable = true;
            bet_10Button.interactable = true;
            bet_20Button.interactable = true;
            bet_50Button.interactable = false;
            bet_100Button.interactable = false;
        }
        else if(currentScore >= 10)
        {
            bet_5Button.interactable = true;
            bet_10Button.interactable = true;
            bet_20Button.interactable = false;
            bet_50Button.interactable = false;
            bet_100Button.interactable = false;
        }
        else if(currentScore >= 5)
        {
            bet_5Button.interactable = true;
            bet_10Button.interactable = false;
            bet_20Button.interactable = false;
            bet_50Button.interactable = false;
            bet_100Button.interactable = false;
        }
            

    }
}
