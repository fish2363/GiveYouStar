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

        private int _currentLevel = 0;
        private int CurrentCost { get; set; }
        private float _currentStatInc;
        private bool _isMax;
        
        private void Awake()
        {
            PlayerStatManager.Instance.onCoinAmountChanged += UpdateCoinUI;
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            int level = 1;
            switch (statType)
            {
                case StatType.FailDistance:
                    level = UpgradeManager.Instance.currentFailDistLevel;
                    break;
                case StatType.MaxTimer:
                    level = UpgradeManager.Instance.currentSpeedLevel;
                    break;
                case StatType.RopeSize:
                    level = UpgradeManager.Instance.currentRopeSizeLevel;
                    break;
            }

            _currentLevel = level;
            CurrentCost = initCost + costInc * level;
            _currentStatInc = statIncAmount * level;
            
            if(_currentStatInc >= maxIncAmount)
                _isMax = true;

            ChangeStatUI();
        }

        public void UpgradeStat()
        {
            if(_isMax)
                return;

            if (PlayerStatManager.Instance.ChangeCoinAmount(-CurrentCost) )
            {
                _currentLevel++;
                onUpgradeStat?.Invoke(statIncAmount);
                
                CurrentCost += costInc;
                _currentStatInc += statIncAmount;

                switch (statType)
                {
                    case StatType.FailDistance:
                        PlayerStatManager.Instance.IncreaseFailDist(statIncAmount);
                        break;
                    case StatType.RopeSize:
                        PlayerStatManager.Instance.IncreaseRopeSize(statIncAmount);
                        break;
                    case StatType.MaxTimer:
                        PlayerStatManager.Instance.IncreaseSpeed(statIncAmount);
                        break;
                }
                
                if(_currentStatInc >= maxIncAmount)
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
            switch (statType)
            {
                case StatType.FailDistance:
                    UpgradeManager.Instance.currentFailDistLevel = _currentLevel;
                    break;
                case StatType.MaxTimer:
                    UpgradeManager.Instance.currentSpeedLevel = _currentLevel;
                    break;
                case StatType.RopeSize:
                    UpgradeManager.Instance.currentRopeSizeLevel = _currentLevel;
                    break;
            }
        }
    }
    
    public enum StatType { FailDistance, MaxTimer, RopeSize }
}