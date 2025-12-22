using System;
using _01.Develop.LSW._01._Scripts.So;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class StarSlot : MonoBehaviour
    {
        [SerializeField] private StarSo starSo;
        
        [SerializeField] private Image starIcon;

        public event Action<StarSo> onShowStarInformation;

        private bool _isUnlocked;
        
        public void Initialize(StarSo star, bool isUnlocked)
        {
            starSo = star;
            if (isUnlocked)
            {
                starIcon.sprite = star.starImage;
            }
            else
            {
                // starIcon.sprite = 
            }
            _isUnlocked = isUnlocked;
        }
        
        public void ShowInformation()
        {
            if (!_isUnlocked || starSo == null)
                return;
            
            onShowStarInformation?.Invoke(starSo);
        }
    }
}