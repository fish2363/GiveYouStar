using System;
using _01.Develop.LSW._01._Scripts.Manager;
using _01.Develop.LSW._01._Scripts.So;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class ChildUI : MonoBehaviour
    {
        [SerializeField] private StarSo _reqStar;

        [SerializeField] private Image icon;
        
        public event Action<ChildUI> onStarGiven; 
        
        public void SetReqStar(StarSo star)
        {
            _reqStar = star;
            SetUI();
        }
        
        public bool GiveStar(StarSo star)
        {
            if (star != _reqStar)
                return false;
            
            PlayerStatManager.Instance.ChangeCoinAmount(_reqStar.price);
            onStarGiven?.Invoke(this);
            return true;
        }

        public void SetUI()
        {
            icon.sprite = _reqStar.starIcon;
        }
    }
}