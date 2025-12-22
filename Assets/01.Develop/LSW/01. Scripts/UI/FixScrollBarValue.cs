using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class FixScrollBarValue : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float targetValue = 0f;

        private ScrollRect _scrollRect;

        void OnEnable()
        {
            _scrollRect = GetComponent<ScrollRect>();

            Canvas.ForceUpdateCanvases();

            if (_scrollRect.verticalScrollbar != null)
                _scrollRect.verticalScrollbar.value = targetValue;

            if (_scrollRect.horizontalScrollbar != null)
                _scrollRect.horizontalScrollbar.value = targetValue;

            _scrollRect.verticalNormalizedPosition = targetValue;
            _scrollRect.horizontalNormalizedPosition = targetValue;
        }

    }
}