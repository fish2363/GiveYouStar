using System.Collections;
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

        private Vector2 _showRewardUISize;
        
        private void Start()
        {
            _showRewardUISize = transform.localScale;
            transform.localScale = Vector2.zero;
            bG.SetActive(false);
            
            StarManager.Instance.onGameEnd += ShowRewardStarUI;
        }

        private void ShowRewardStarUI(List<StarSo> starList)
        {
            Debug.Log("ShowRewardStarUI");
            foreach (Transform child in rewardStarContainer)
            {
                Destroy(child.gameObject);
            }

            bG.SetActive(true);
            foreach (var star in starList)
            {
                var rewardStar = Instantiate(rewardStarPrefab, rewardStarContainer);
                rewardStar.Set(star);
            }

            transform.localScale = Vector3.zero;
            transform.DOScale(_showRewardUISize, 0.5f)
                .SetEase(Ease.OutBack);
        }

        public void HideRewardStarUI()
        {
            transform.DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    bG.SetActive(false);
                });
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