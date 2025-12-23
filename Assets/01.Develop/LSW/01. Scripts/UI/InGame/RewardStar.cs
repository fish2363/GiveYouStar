using _01.Develop.LSW._01._Scripts.So;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.InGame
{
    public class RewardStar : MonoBehaviour
    {
        [SerializeField] private Image starImage;
        
        public void Set(StarSo star)
        {
            starImage.sprite = star.starIcon;
        }
    }
}