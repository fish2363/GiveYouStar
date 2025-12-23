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
        [SerializeField] private List<ChildUI> childrenUI = new List<ChildUI>();
        
        [SerializeField] private HavingStarUI havingStarUIPrefab;
        [SerializeField] private Transform havingStarParent;
        
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
            childUI.SetReqStar(GetRandomStar());
        }

        private StarSo GetRandomStar()
        {
            List<StarSo> allStar = StarManager.Instance.GetAllStars();
            StarSo randStar = allStar[Random.Range(0, allStar.Count)];
            return randStar;
        }
        
        private void RemoveHavingStar(HavingStarUI havingStar)
        {
            if(_havingStarUIs.Contains(havingStar))
            {
                _havingStarUIs.Remove(havingStar);
                Destroy(havingStar.gameObject);
                havingStar.onStarRemoved -= RemoveHavingStar;
            }
        }

        private void OnDestroy()
        {
            foreach (var remainStar in _havingStarUIs)
            {
                remainStar.onStarRemoved -= RemoveHavingStar;
            }
            
            foreach (var childUI in childrenUI)
            {
                childUI.onStarGiven -= SetChild;
            }
        }
    }
}