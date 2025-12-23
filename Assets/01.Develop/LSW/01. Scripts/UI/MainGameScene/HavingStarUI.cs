using System;
using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.Manager;
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
        private Vector2 dragOffset;
        
        public void SetStar(StarSo star)
        {
            _currentStar = star;
            icon.sprite = star.starIcon;
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

            var cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : parentCanvas.worldCamera;

            // 마우스 위치를 Canvas 로컬 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                eventData.position,
                cam,
                out Vector2 localMousePos
            );

            // 현재 UI 위치와 마우스 위치의 차이를 저장
            dragOffset = rect.anchoredPosition - localMousePos;

            canvasGroup.blocksRaycasts = false;
            rect.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData evt)
        {
            var cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : parentCanvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                evt.position,
                cam,
                out Vector2 localPoint
            );

            // 오프셋을 적용
            rect.anchoredPosition = localPoint + dragOffset;
        }

        public void OnEndDrag(PointerEventData evt)
        {
            // 드래그 끝나면 레이캐스트 복원
            canvasGroup.blocksRaycasts = true;

            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = evt.position };
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            ChildUI target = null;
            StarTrashcan trashcan = null;
            foreach (var r in results)
            {
                target = r.gameObject.GetComponentInParent<ChildUI>();
                trashcan = r.gameObject.GetComponent<StarTrashcan>();
                if (target != null || trashcan != null) 
                    break;
            }

            bool accepted = false;

            if (trashcan != null)
            {
                accepted = true;
                PlayerStatManager.Instance.ChangeCoinAmount(_currentStar.throwPrice);
            }
            
            if (target != null && !accepted)
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