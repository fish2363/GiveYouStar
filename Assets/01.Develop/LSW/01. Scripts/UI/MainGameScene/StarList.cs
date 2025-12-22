using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.So;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class StarList : MonoBehaviour
    {
        [Header("Star So List")]
        [SerializeField] private List<StarSo> stars = new List<StarSo>();
        private readonly List<StarSlot> _starSlotList = new List<StarSlot>();
        
        [Header("Instantiate Star Slot")]
        [SerializeField] private StarSlot starSlotPrefab;
        [SerializeField] private Transform starSlotParent;
        
        [Header("Star Information")]
        [SerializeField] private Image starImage;
        [SerializeField] private TextMeshProUGUI starNameText;
        [SerializeField] private TextMeshProUGUI starDescriptionText;
        
        private void Start()
        {
            starImage.gameObject.SetActive(false);
            starNameText.text = string.Empty;
            starDescriptionText.text = string.Empty;
            
            foreach (var star in stars)
            {
                StarSlot starSlot = Instantiate(starSlotPrefab, starSlotParent);
                starSlot.Initialize(star);
                starSlot.OnShowStarInformation += ShowStars;
                _starSlotList.Add(starSlot);
            }
        }

        private void ShowStars(StarSo star)
        {
            starImage.gameObject.SetActive(true);
            starImage.sprite = star.starImage;
            starNameText.text = star.starName;
            starDescriptionText.text = star.description;
        }

        private void OnDestroy()
        {
            foreach (var starSlot in _starSlotList)
            {
                starSlot.OnShowStarInformation -= ShowStars;
            }
        }
    }
}