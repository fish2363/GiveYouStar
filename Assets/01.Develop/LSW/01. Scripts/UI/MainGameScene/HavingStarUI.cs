using System;
using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.So;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class HavingStarUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image icon;

        public event Action<HavingStarUI> onStarRemoved;

        private StarSo _currentStar;

        private RectTransform rect;
        private Vector2 originalPos;
        private Canvas parentCanvas;
        private GraphicRaycaster raycaster;
        private CanvasGroup canvasGroup;

        public void SetStar(StarSo star)
        {
            _currentStar = star;
            icon.sprite = star.starImage;
        }

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();
            raycaster = parentCanvas.GetComponent<GraphicRaycaster>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            originalPos = rect.anchoredPosition;
            // 드래그 중 자신이 레이캐스트를 막지 않도록
            canvasGroup.blocksRaycasts = false;
            // 최상단으로 올려 시각적 우선순위 확보
            rect.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData evt)
        {
            // Canvas renderMode에 맞는 카메라 전달 (ScreenSpaceOverlay는 null)
            var cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                evt.position,
                cam,
                out Vector2 localPoint
            );
            rect.anchoredPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData evt)
        {
            // 드래그 끝나면 레이캐스트 복원
            canvasGroup.blocksRaycasts = true;

            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = evt.position };
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            ChildUI target = null;
            foreach (var r in results)
            {
                target = r.gameObject.GetComponentInParent<ChildUI>();
                if (target != null) break;
            }

            bool accepted = false;
            if (target != null)
            {
                accepted = target.GiveStar(_currentStar);
            }

            if (!accepted)
            {
                rect.anchoredPosition = originalPos;
            }
            else
            {
                onStarRemoved?.Invoke(this);
            }
        }
    }
}