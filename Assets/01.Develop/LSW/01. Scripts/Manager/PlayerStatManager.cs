using System;
using _01.Develop.LSW._01._Scripts.UI.MainGameScene;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class PlayerStatManager : MonoSingleton<PlayerStatManager>
    {
        public int initCoin = 10;
        public float initFailDist = 10f;
        public float initSpeed = 1f;
        public float initRopeSize = 1f;

        [field:SerializeField] public int Coin { get; private set; }
        [field:SerializeField] private float FailDist { get; set; }
        [field:SerializeField] private float Speed { get; set; }
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
        
        public float GetCurrentSpeed()
            => Speed;

        public float GetCurrentRopeSize()
            => RopeSize;
        
        public void IncreaseFailDist(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            FailDist = MathF.Round(Mathf.Max(0f, FailDist + amount) * 10) / 10;
            onStatsChanged?.Invoke();
        }

        public void IncreaseSpeed(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            Speed = MathF.Round(Mathf.Max(0f, Speed + amount) * 10) / 10;
            onStatsChanged?.Invoke();
        }

        public void IncreaseRopeSize(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            RopeSize = MathF.Round(Mathf.Max(0f, RopeSize + amount) * 10) / 10;
            onStatsChanged?.Invoke();
        }

        public float GetCurrentStat(StatType statType)
        {
            switch (statType)
            {
                case StatType.FailDistance:
                    return FailDist;
                case StatType.Speed:
                    return Speed;
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
                case StatType.Speed:
                    return initSpeed;
                case StatType.RopeSize:
                    return initRopeSize;
            }

            return 0f;
        }

        private void ResetAllStat()
        {
            FailDist = initFailDist;
            Speed = initSpeed;
            RopeSize = initRopeSize;
            Coin = initCoin;
            onStatsChanged?.Invoke();
        }
    }
}