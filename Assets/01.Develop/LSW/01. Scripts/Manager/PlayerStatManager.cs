using System;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class PlayerStatManager : MonoSingleton<PlayerStatManager>
    {
        public int initCoin = 10;
        public float initFailDist = 5f;
        public float initSpeed = 10f;
        public float initRopeSize = 10f;

        public int Coin { get; private set; }
        private float FailDist { get; set; }
        private float Speed { get; set; }
        private float RopeSize { get; set; }
        
        public event Action onStatsChanged;

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
            
            FailDist = Mathf.Max(0f, FailDist + amount);
            onStatsChanged?.Invoke();
        }

        public void IncreaseSpeed(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            Speed = Mathf.Max(0f, Speed + amount);
            onStatsChanged?.Invoke();
        }

        public void IncreaseRopeSize(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            RopeSize = Mathf.Max(0f, RopeSize + amount);
            onStatsChanged?.Invoke();
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