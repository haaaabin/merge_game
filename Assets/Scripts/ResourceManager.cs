using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResourceManager : MonoBehaviour
{

    [Header("---- Energy ----")]
    public int energyCount = 100;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI energyRechargeText;
    public TextMeshProUGUI chargeTimeText;
    public Button energyRechargeBtn;
    public GameObject energyRechargePanel;
    private float countdownTime = 120f;
    private float timeRemaining;
    private int energyBuyDiamond = 20;
    private int displayEnergy = 0;


    [Header("---- Coin ----")]
    public int coins = 100;
    public TextMeshProUGUI coinText;
    private int displayCoins = 0;


    [Header("---- Diamond ----")]
    public int diamondCount = 100;
    public TextMeshProUGUI diamondText;
    private int displayDiamond = 0;

    private Tween tween;

    void Start()
    {
        ResetResources();
        ResetTimer();
    }

    void Update()
    {
        if (energyCount < 100)
        {
            energyRechargePanel.SetActive(true);
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                energyCount++;
                energyText.text = energyCount.ToString();
                ResetTimer();
            }

            DisplayRechargeTime(timeRemaining);
        }
        else
        {
            energyRechargePanel.SetActive(false);
        }
    }

    private void ResetResources()
    {
        energyCount = 100;
        energyText.text = energyCount.ToString();
        displayEnergy = energyCount;

        coins = 100;
        coinText.text = coins.ToString();
        displayCoins = coins;

        diamondCount = 100;
        diamondText.text = diamondCount.ToString();
        displayDiamond = diamondCount;

        energyRechargePanel.SetActive(false);
    }

    private void ResetTimer()
    {
        timeRemaining = countdownTime;
    }

    private void DisplayRechargeTime(float timeToDisplay)
    {
        timeToDisplay = Mathf.Max(0, timeToDisplay);
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        energyRechargeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        chargeTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddCoins(int amount)
    {
        int targetAmount = coins + amount;
        AnimateDisplayText(targetAmount, coinText, displayCoins);
        coins = targetAmount;
    }

    public void UseEnergy()
    {
        if (energyCount > 0)
        {
            energyCount--;
            energyText.text = energyCount.ToString();
        }
        else
        {
            Debug.LogWarning("전기가 부족합니다.");
        }
    }

    public void BuyEnergy()
    {
        if (diamondCount >= energyBuyDiamond)
        {
            diamondCount -= energyBuyDiamond;
            int targetEnergyAmount = energyCount + 100;
            AnimateDisplayText(targetEnergyAmount, energyText, displayEnergy);
            energyCount = targetEnergyAmount;

            diamondText.text = diamondCount.ToString();
            AnimateDisplayText(diamondCount, diamondText, displayDiamond);
        }
        else
        {
            Debug.LogWarning("다이아몬드가 부족합니다.");
        }
    }

    public void AnimateDisplayText(int targetAmount, TextMeshProUGUI displayText, int displayAmount)
    {
        // if (tween != null && tween.IsActive()) tween.Kill();

        // 현재 코인 값 → 목표 코인 값
        tween = DOTween.To(() => displayAmount,
                   x =>
                   {
                       displayAmount = x;
                       displayText.text = displayAmount.ToString();
                   },
                   targetAmount,
                   0.5f
                  ).SetEase(Ease.OutQuad);
    }
}
