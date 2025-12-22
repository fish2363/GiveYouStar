using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.Manager;
using _01.Develop.LSW._01._Scripts.So;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class StarList : MonoBehaviour
    {
        [Header("Star Slot List")]
        private readonly List<StarSlot> _starSlotList = new List<StarSlot>();
        
        [Header("Instantiate Star Slot")]
        [SerializeField] private StarSlot starSlotPrefab;
        [SerializeField] private Transform starSlotParent;
        
        [Header("Star Information")]
        [SerializeField] private Image starIcon;
        [SerializeField] private TextMeshProUGUI starNameText;
        [SerializeField] private TextMeshProUGUI starDescriptionText;
        
        private void Start()
        {
            starIcon.gameObject.SetActive(false);
            starNameText.text = string.Empty;
            starDescriptionText.text = string.Empty;
            
            foreach (var star in StarManager.Instance.GetStarList())
            {
                StarSlot starSlot = Instantiate(starSlotPrefab, starSlotParent);
                starSlot.Initialize(star.Key, star.Value);
                starSlot.onShowStarInformation += ShowStars;
                _starSlotList.Add(starSlot);
            }
        }

        private void ShowStars(StarSo star)
        {
            starIcon.gameObject.SetActive(true);
            starIcon.sprite = star.starImage;
            starNameText.text = star.starName;
            starDescriptionText.text = star.description;
        }

        private void OnDestroy()
        {
            foreach (var starSlot in _starSlotList)
            {
                starSlot.onShowStarInformation -= ShowStars;
            }
        }
    }
}