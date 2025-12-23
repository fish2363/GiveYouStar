using System;
using _01.Develop.LSW._01._Scripts.UI.MainGameScene;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class PlayerStatManager : MonoSingleton<PlayerStatManager>
    {
        public int initCoin = 10;
        public float initFailDist = 10f;
        public float initMaxTimer = 1f;
        public float initRopeSize = 1f;

        [field:SerializeField] public int Coin { get; private set; }
        [field:SerializeField] private float FailDist { get; set; }
        [field:SerializeField] private float MaxTimer { get; set; }
        [field:SerializeField] private float RopeSize { get; set; }
        
        public event Action onStatsChanged;
        public event Action<int> onCoinAmountChanged; 
        
        protected override void Awake()
        {
            base.Awake();
            ResetAllStat();
        }

        public int GetCurrentCoin()
            => Coin;
        
        public bool ChangeCoinAmount(int coinAmount)
        {
            if (Coin + coinAmount < 0)
                return false;
            
            Coin += coinAmount;
            onCoinAmountChanged?.Invoke(Coin);

            return true;
        }

        public float GetCurrentFailDist()
            => FailDist;
        
        public float GetCurrentMaxTimer()
            => MaxTimer;

        public float GetCurrentRopeSize()
            => RopeSize;
        
        public void IncreaseFailDist(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            FailDist = Mathf.Max(0f, FailDist + amount);
            onStatsChanged?.Invoke();
        }

        public void IncreaseSpeed(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            MaxTimer = Mathf.Max(0f, MaxTimer + amount);
            onStatsChanged?.Invoke();
        }

        public void IncreaseRopeSize(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            RopeSize = Mathf.Max(0f, RopeSize + amount);
            onStatsChanged?.Invoke();
        }

        public float GetCurrentStat(StatType statType)
        {
            switch (statType)
            {
                case StatType.FailDistance:
                    return FailDist;
                case StatType.MaxTimer:
                    return MaxTimer;
                case StatType.RopeSize:
                    return RopeSize;
            }

            return 0f;
        }
        
        public float GetInitStat(StatType statType)
        {
            switch (statType)
            {
                case StatType.FailDistance:
                    return initFailDist;
                case StatType.MaxTimer:
                    return initMaxTimer;
                case StatType.RopeSize:
                    return initRopeSize;
            }

            return 0f;
        }

        private void ResetAllStat()
        {
            FailDist = initFailDist;
            MaxTimer = initMaxTimer;
            RopeSize = initRopeSize;
            Coin = initCoin;
            onStatsChanged?.Invoke();
        }
    }
}