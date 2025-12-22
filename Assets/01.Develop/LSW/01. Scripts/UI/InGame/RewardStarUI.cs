using System;
using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.Manager;
using _01.Develop.LSW._01._Scripts.So;
using DG.Tweening;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.UI.InGame
{
    public class RewardStarUI : MonoBehaviour
    {
        [SerializeField] private RewardStar rewardStarPrefab;
        [SerializeField] private GameObject bG;
        [SerializeField] private Transform rewardStarContainer;

        private Vector3 _showRewardUISize;
        
        private void Awake()
        {
            StarManager.Instance.onGameEnd += ShowRewardStarUI;
            
            _showRewardUISize = transform.localScale;
            transform.localScale = Vector3.zero;
            bG.SetActive(false);
        }

        public void ShowRewardStarUI(List<StarSo> starList)
        {
            bG.SetActive(true);
            rewardStarContainer.DOScale(_showRewardUISize, 0.5f).OnComplete(() =>
            {
                foreach (var star in starList)
                {
                    var rewardStar = Instantiate(rewardStarPrefab, rewardStarContainer);
                    rewardStar.Set(star);
                }
            });
        }

        private void OnDestroy()
        {
            StarManager.Instance.onGameEnd -= ShowRewardStarUI;
        }
    }
}