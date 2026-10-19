using System;
using UnityEngine;

//game events to de couple the ui and logic(tried my best to do it for now)
public class EventManager : MonoBehaviour
{
    public event Action<int> onBetClicked;
    public event Action onScrollCompleted;

    public static EventManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void BetClicked(int betAmount)
    {
        onBetClicked?.Invoke(betAmount);
    }

    public void ScrollCompleted()
    {
        onScrollCompleted?.Invoke();
    }
}
