using System;
using _01.Develop.LSW._01._Scripts.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class UpgradeStatCardUI : MonoBehaviour
    {
        public StatType statType;
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI incAmountText;
        [SerializeField] private TextMeshProUGUI upgBtnText;
        [SerializeField] private TextMeshProUGUI coinText;
        
        [Header("Stat Upgrade Settings")]
        public float statIncAmount;
        public float maxIncAmount;
        
        [Header("Cost Settings")]
        public int initCost;
        public int costInc;
        
        public UnityEvent<float> onUpgradeStat;

        private int CurrentCost { get; set; }
        
        private bool _isMax;
        
        private void Awake()
        {
            PlayerStatManager.Instance.onCoinAmountChanged += UpdateCoinUI;
            CurrentCost = initCost;
        }

        private void Start()
        {
            ChangeStatUI();
        }

        public void UpgradeStat()
        {
            if(_isMax)
                return;
            
            if (PlayerStatManager.Instance.ChangeCoinAmount(-CurrentCost) )
            {
                onUpgradeStat?.Invoke(statIncAmount);
                
                CurrentCost += costInc;

                switch (statType)
                {
                    case StatType.FailDistance:
                        PlayerStatManager.Instance.IncreaseFailDist(statIncAmount);
                        break;
                    case StatType.RopeSize:
                        PlayerStatManager.Instance.IncreaseRopeSize(statIncAmount);
                        break;
                    case StatType.Speed:
                        PlayerStatManager.Instance.IncreaseSpeed(statIncAmount);
                        break;
                }
                
                if(PlayerStatManager.Instance.GetCurrentStat(statType) >= maxIncAmount)
                    _isMax = true;
                
                ChangeStatUI();
            }
        }
        
        private void UpdateCoinUI(int curCoin)
            => coinText.SetText(curCoin.ToString());
        
        private void ChangeStatUI()
        {
            float incAmount =
                PlayerStatManager.Instance.GetCurrentStat(statType) - PlayerStatManager.Instance.GetInitStat(statType);
            incAmountText.SetText($"+{incAmount}");
            upgBtnText.SetText(_isMax ? "Upgrade Max" : $"Upgrade\n(Cost : {CurrentCost})");
            coinText.SetText(PlayerStatManager.Instance.GetCurrentCoin().ToString());
        }

        private void OnDestroy()
        {
            if (PlayerStatManager.Instance != null)
                PlayerStatManager.Instance.onCoinAmountChanged -= UpdateCoinUI;
        }
    }
    
    public enum StatType { FailDistance, Speed, RopeSize }
}