using System;
using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.Manager;
using _01.Develop.LSW._01._Scripts.So;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class DreamHouse : MonoBehaviour
    {
        [SerializeField] private List<ChildUI> childrenUI = new List<ChildUI>();
        [SerializeField] private List<ChildUI> outerChildrenUI;
        
        [SerializeField] private HavingStarUI havingStarUIPrefab;
        [SerializeField] private Transform havingStarParent;

        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI coinIncTextPrefab;
        
        private List<HavingStarUI> _havingStarUIs = new List<HavingStarUI>();
        
        private void Start()
        {
            if (ChildManager.Instance.GetReqStarsEmpty())
                SetInitChild();
            else
                SetChildUI();
            
            List<StarSo> havingStars 
                = new List<StarSo>(StarManager.Instance.GetAllGotStars());
            foreach (var star in havingStars)
            {
                HavingStarUI havingStarUI 
                    = Instantiate(havingStarUIPrefab, havingStarParent);
                havingStarUI.onStarRemoved += RemoveHavingStar;
                havingStarUI.onShowCoinIncText += ShowCoinIncText;
                PlayerStatManager.Instance.onCoinAmountChanged += CoinTextUpdate;
                havingStarUI.SetStar(star);
                _havingStarUIs.Add(havingStarUI);
            }

            foreach (var childUI in childrenUI)
            {
                childUI.onStarGiven += SetChild;
            }
        }

        private void SetChildUI()
        {
            for (int i = 0; i < childrenUI.Count; i++)
            {
                childrenUI[i].SetReqStar(ChildManager.Instance.GetReqStars()[i]);
            }
        }

        private void SetInitChild()
        {
            foreach (var child in childrenUI)
            {
                StarSo randStar = GetRandomStar();
                child.SetReqStar(randStar);
                ChildManager.Instance.AddReqStars(randStar);
            }
        }

        private void SetChild(ChildUI childUI)
        {
            int index = childrenUI.IndexOf(childUI);
            if (index != -1 && index < outerChildrenUI.Count)
            {
                ChildUI outerChild = outerChildrenUI[index];
                
                Vector3 childPos = childUI.transform.position;
                Vector3 outerChildPos = outerChild.transform.position;

                childUI.transform.DOMove(outerChildPos, 0.5f);
                childUI.SetUI(true);
                outerChild.SetUI(true);
                outerChild.transform.DOMove(childPos, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        outerChild.SetReqStar(GetRandomStar());
                    });
                
                childrenUI[index] = outerChild;
                outerChildrenUI[index] = childUI;
            }
        }

        private StarSo GetRandomStar()
        {
            List<StarSo> allStar = StarManager.Instance.GetAllStars();
            StarSo randStar = allStar[Random.Range(0, allStar.Count)];
            return randStar;
        }
        
        public void CoinTextUpdate(int coin)
            => coinText.SetText(coin.ToString());
        
        private void RemoveHavingStar(HavingStarUI havingStar)
        {
            if(_havingStarUIs.Contains(havingStar))
            {
                _havingStarUIs.Remove(havingStar);
                Destroy(havingStar.gameObject);
                havingStar.onStarRemoved -= RemoveHavingStar;
                havingStar.onShowCoinIncText -= ShowCoinIncText;
            }
        }

        public void ShowCoinIncText(int coinInc, Transform trm)
        {
            TextMeshProUGUI coinIncText = Instantiate(coinIncTextPrefab, trm);
            coinIncText.SetText($"+{coinInc}");

            var seq = DOTween.Sequence();
            seq.Append(coinIncText.DOFade(0f, 1.25f));
            seq.Join(coinIncText.transform.DOMove(coinIncText.transform.position + Vector3.up * 10f, 1.25f)
                .SetEase(Ease.OutQuad));
            seq.OnComplete(() => Destroy(coinIncText.gameObject));
        }

        private void OnDestroy()
        {
            PlayerStatManager.Instance.onCoinAmountChanged -= CoinTextUpdate;
            foreach (var remainStar in _havingStarUIs)
            {
                remainStar.onStarRemoved -= RemoveHavingStar;
                remainStar.onShowCoinIncText -= ShowCoinIncText;
            }
            
            foreach (var childUI in childrenUI)
            {
                childUI.onStarGiven -= SetChild;
            }
        }
    }
}