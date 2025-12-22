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
        
        private void Start()
        {
            StarManager.Instance.onGameEnd += ShowRewardStarUI;
            
            _showRewardUISize = transform.localScale;
            transform.localScale = Vector3.zero;
            bG.SetActive(false);
        }

        private void ShowRewardStarUI(List<StarSo> starList)
        {
            bG.SetActive(true);
            foreach (var star in starList)
            {
                var rewardStar = Instantiate(rewardStarPrefab, rewardStarContainer);
                rewardStar.Set(star);
            }

            transform.DOScale(_showRewardUISize, 1f)
                .SetEase(Ease.InExpo);
        }

        private void OnDestroy()
        {
            if (StarManager.Instance != null)
            {
                StarManager.Instance.onGameEnd -= ShowRewardStarUI;
            }
        }
    }
}