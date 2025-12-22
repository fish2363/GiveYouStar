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

        public event Action<StarSo> OnShowStarInformation;

        public void Initialize(StarSo star)
        {
            starSo = star;
            if (star.isUnlocked)
            {
                starIcon.sprite = star.starImage;
            }
            else
            {
                // starIcon.sprite = 
            }
        }
        
        public void ShowInformation()
        {
            if (!starSo.isUnlocked || starSo == null)
                return;
            
            OnShowStarInformation?.Invoke(starSo);
        }
    }
}