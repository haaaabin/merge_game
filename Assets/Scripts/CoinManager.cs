using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int coins = 1000;
    public TextMeshProUGUI coinText;

    private int displayCoins = 0;
    private Tween coinTween;

    void Start()
    {
        coins = 1000;
        coinText.text = coins.ToString();
        displayCoins = coins;
    }

    public void AddCoins(int amount)
    {
        int targetAmount = coins + amount;
        AnimateCoinText(targetAmount);
        coins = targetAmount;
    }

    public void AnimateCoinText(int targetAmount)
    {
        if (coinTween != null && coinTween.IsActive()) coinTween.Kill();

        // 현재 코인 값 → 목표 코인 값
        coinTween = DOTween.To(() => displayCoins,
                   x =>
                   {
                       displayCoins = x;
                       coinText.text = displayCoins.ToString();
                   },
                   targetAmount,
                   0.5f
                  ).SetEase(Ease.OutQuad);
    }
}

