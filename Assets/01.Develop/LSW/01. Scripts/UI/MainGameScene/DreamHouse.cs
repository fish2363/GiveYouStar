using System;
using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.Manager;
using _01.Develop.LSW._01._Scripts.So;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class DreamHouse : MonoBehaviour
    {
        [SerializeField] private List<StarSo> requiredStars = new List<StarSo>();
        [SerializeField] private List<ChildUI> childrenUI = new List<ChildUI>();
        
        [SerializeField] private HavingStarUI havingStarUIPrefab;
        [SerializeField] private Transform havingStarParent;
        
        private List<HavingStarUI> _havingStars = new List<HavingStarUI>();
        
        private void Start()
        {
            SetChild();
            List<StarSo> havingStars 
                = new List<StarSo>(StarManager.Instance.GetAllGotStars());
            foreach (var star in havingStars)
            {
                HavingStarUI havingStarUI 
                    = Instantiate(havingStarUIPrefab, havingStarParent);
                havingStarUI.onStarRemoved += RemoveHavingStar;
                havingStarUI.SetStar(star);
                _havingStars.Add(havingStarUI);
            }
        }

        public void SetChild()
        {
            foreach (var child in childrenUI)
            {
                StarSo star = requiredStars[Random.Range(0, requiredStars.Count)];
                child.SetReqStar(star);
            }
        }
        
        public void RemoveHavingStar(HavingStarUI havingStar)
        {
            if(_havingStars.Contains(havingStar))
            {
                _havingStars.Remove(havingStar);
                Destroy(havingStar.gameObject);
                havingStar.onStarRemoved -= RemoveHavingStar;
            }
        }

        private void OnDestroy()
        {
            foreach (var remainStar in _havingStars)
            {
                remainStar.onStarRemoved -= RemoveHavingStar;
            }
        }
    }
}