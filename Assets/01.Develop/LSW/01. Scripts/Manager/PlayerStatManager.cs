using System;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class PlayerStatManager : MonoSingleton<PlayerStatManager>
    {
        public int initCoin = 10;
        public float initSight = 5f;
        public float initLaunchPower = 10f;
        public float initPullPower = 10f;

        public int Coin { get; private set; }
        private float Sight { get; set; }
        private float LaunchPower { get; set; }
        private float PullPower { get; set; }

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

        public void IncreaseSight(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            Sight = Mathf.Max(0f, Sight + amount);
            onStatsChanged?.Invoke();
        }

        public void IncreaseLaunchPower(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            LaunchPower = Mathf.Max(0f, LaunchPower + amount);
            onStatsChanged?.Invoke();
        }

        public void IncreasePullPower(float amount)
        {
            if (Mathf.Approximately(amount, 0f)) 
                return;
            
            PullPower = Mathf.Max(0f, PullPower + amount);
            onStatsChanged?.Invoke();
        }

        private void ResetAllStat()
        {
            Sight = initSight;
            LaunchPower = initLaunchPower;
            PullPower = initPullPower;
            Coin = initCoin;
            onStatsChanged?.Invoke();
        }
    }
}