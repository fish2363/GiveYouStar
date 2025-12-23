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

        private bool _canGiveStar;
        
        public void SetReqStar(StarSo star)
        {
            _reqStar = star;
            SetUI();
            _canGiveStar = true;
        }
        
        public bool GiveStar(StarSo star)
        {
            if (star != _reqStar || !_canGiveStar)
                return false;
            
            PlayerStatManager.Instance.ChangeCoinAmount(_reqStar.price);
            onStarGiven?.Invoke(this);
            _canGiveStar = false;
            return true;
        }

        public void SetUI(bool disable = false)
        {
            if (!disable)
            {
                icon.enabled = true;
                icon.sprite = _reqStar.starIcon;
            }
            else
                icon.enabled = false;
        }
    }
}