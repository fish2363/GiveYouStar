using System;
using _01.Develop.LSW._01._Scripts.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class UpgradeStatCardUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI incAmountText;
        [SerializeField] private TextMeshProUGUI upgBtnText;
        [SerializeField] private TextMeshProUGUI coinText;
        
        [Header("Stat Upgrade Settings")]
        public float statIncAmount;
        
        [Header("Cost Settings")]
        public int initCost;
        public int costInc;
        
        public UnityEvent<float> onUpgradeStat;

        private int CurrentCost { get; set; }
        private float _incAmount;
        
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
            if (PlayerStatManager.Instance.ChangeCoinAmount(-CurrentCost))
            {
                onUpgradeStat?.Invoke(statIncAmount);
                
                _incAmount += statIncAmount;
                CurrentCost += costInc;
                
                ChangeStatUI();
            }
        }
        
        private void UpdateCoinUI(int curCoin)
            => coinText.SetText(curCoin.ToString());
        
        private void ChangeStatUI()
        {
            incAmountText.SetText($"+{_incAmount}");
            upgBtnText.SetText($"Upgrade\n(Cost : {CurrentCost})");
            coinText.SetText(PlayerStatManager.Instance.GetCurrentCoin().ToString());
        }

        private void OnDestroy()
        {
            PlayerStatManager.Instance.onCoinAmountChanged -= UpdateCoinUI;
        }
    }
}