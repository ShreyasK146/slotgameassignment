using System;
using UnityEngine;

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
