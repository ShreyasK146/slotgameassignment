
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private List<Button> betButtons = new List<Button>();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip reelMovementClip;
    [SerializeField] private Image thankyouMessage;

    private int symbolHeight = 250;
    private int scrollSpeed = 1500;

    private string symbol1;
    private string symbol2;
    private string symbol3;

    private readonly int[] betAmmounts = { 5, 10, 20, 50, 100 };

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
            string message = currentScore < highScore ? "Your current score is less than high score continue playing more beat highscore ?" : "You achieved new highscore continue playing?";
            gameEndText.text = "You Won! " + message;
        }
        else
        {
            string message = currentScore < highScore ? "Try again ?": "with new highscore.. too greedy heh";
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
        yield return new WaitForSeconds(0.2f);
        jackPotHandleImage2.gameObject.SetActive(false);
        jackPotHandleImage.gameObject.SetActive(true);
    }

    public void SetRandomSymbolsForAllSlots(string s1, string s2, string s3)
    {
        symbol1 = s1; symbol2 = s2; symbol3 = s3;
    }

    public void StartJackPotMachine()
    {
        StartCoroutine(HandleSpinSlots());

    }

    private IEnumerator HandleSpinSlots()
    {
        var c1 = StartCoroutine(SpinReel(slotReel1, symbol1, 2f));
        audioSource.PlayOneShot(reelMovementClip);
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(reelMovementClip);
        var c2 = StartCoroutine(SpinReel(slotReel2, symbol2, 2.5f));
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(reelMovementClip);
        var c3 = StartCoroutine(SpinReel(slotReel3, symbol3, 3f));
        yield return c1; yield return c2; yield return c3;
        EventManager.Instance?.ScrollCompleted();
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

    //only enable bet buttons if we have enough money to bet on it
    public void HandleButtonInteractible(int currentScore)
    {
        for (int i = 0; i < betAmmounts.Length; i++)
        {
            if (currentScore >= betAmmounts[i])
            {
                betButtons[i].interactable = true;
            }
            else
            {
                betButtons[i].interactable = false;
            }
        }
      
    }

    public void HandleContinueGame(int score)
    {
        gameEndedImage.gameObject.SetActive(false);
        ShowBetOptions(score);
    }

    public void ShowThankYouMessage()
    {
        thankyouMessage.gameObject.SetActive(true);
    }
}
