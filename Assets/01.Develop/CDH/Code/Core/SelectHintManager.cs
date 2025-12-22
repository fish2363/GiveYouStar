using Assets._01.Develop.CDH.Code.Fasdfags;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets._01.Develop.CDH.Code.Core
{
    public class SelectHintManager : MonoBehaviour
    {
        [Header("World")]
        [SerializeField] private Transform playerTrm;
        [SerializeField] private LayerMask pickMask;

        [Header("Arrow UI (player -> star)")]
        [SerializeField] private ArrowUI arrowUI;

        [Header("Under Star UI")]
        [SerializeField] private Image arrowImageForNearstStar;
        [SerializeField] private Canvas uiCanvas;              // arrowImageForNearstStar가 속한 Canvas (비우면 자동 탐색)
        [SerializeField] private Camera worldCamera;           // 월드 -> 스크린용 (비우면 Camera.main)
        [SerializeField] private float underStarPaddingPx = 18f; // 별 아래로 내릴 픽셀

        [Header("Pick")]
        [SerializeField] private float pickRadius = 0.5f;      // 마우스 근처 탐색 반경(월드)
        [SerializeField] private int maxHits = 16;

        private Collider2D[] _hits;
        private bool isSelectHint;

        // NonAlloc 대체용
        private ContactFilter2D _filter;

        // UI 변환용
        private RectTransform _canvasRect;
        private Camera _uiCamera; // Overlay면 null
        private RectTransform _underStarRect;

        private void Awake()
        {
            _hits = new Collider2D[maxHits];
            isSelectHint = false;

            if (worldCamera == null) worldCamera = Camera.main;

            if (arrowImageForNearstStar != null)
            {
                _underStarRect = arrowImageForNearstStar.rectTransform;

                if (uiCanvas == null)
                    uiCanvas = arrowImageForNearstStar.GetComponentInParent<Canvas>();

                if (uiCanvas != null)
                {
                    _canvasRect = (RectTransform)uiCanvas.transform;
                    _uiCamera = (uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : uiCanvas.worldCamera;
                }

                arrowImageForNearstStar.gameObject.SetActive(false);
            }

            // ContactFilter 세팅 (레이어마스크 적용 + 트리거 포함)
            _filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = pickMask,
                useTriggers = true
            };

            // 시작은 꺼둠
            if (arrowUI != null) arrowUI.Show(false);
        }

        // 카메라(Cinemachine) 이후에 UI 위치 갱신하는 게 덜 흔들려서 LateUpdate 추천
        private void LateUpdate()
        {
            if (!isSelectHint)
            {
                HideAll();
                return;
            }

            UpdateHover();
        }

        private void UpdateHover()
        {
            if (worldCamera == null || Mouse.current == null)
            {
                HideAll();
                return;
            }

            Vector2 mouseWorld = worldCamera.ScreenToWorldPoint(Mouse.current.position.value);

            // ✅ NonAlloc 대체: OverlapCircle + ContactFilter + results 배열
            int count = Physics2D.OverlapCircle(mouseWorld, pickRadius, _filter, _hits);

            if (count <= 0)
            {
                HideAll();
                return;
            }

            Collider2D best = null;
            float bestSqrDist = float.PositiveInfinity;

            for (int i = 0; i < count; i++)
            {
                var col = _hits[i];
                if (col == null) continue;

                Vector2 closest = col.ClosestPoint(mouseWorld);
                float sqrDist = (closest - mouseWorld).sqrMagnitude;

                if (sqrDist < bestSqrDist)
                {
                    bestSqrDist = sqrDist;
                    best = col;
                }

                _hits[i] = null; // 다음 프레임 대비 정리
            }

            if (best == null)
            {
                HideAll();
                return;
            }

            // 1) arrowUI: 플레이어 기준으로 방향 표시
            if (arrowUI != null && playerTrm != null)
            {
                arrowUI.SetPivotWorld(playerTrm);
                arrowUI.SetTargetWorld(best.transform);
                arrowUI.Show(true);
            }

            // 2) 별 아래 UI 표시
            UpdateUnderStarUI(best);
        }

        private void UpdateUnderStarUI(Collider2D starCol)
        {
            if (arrowImageForNearstStar == null || _underStarRect == null || _canvasRect == null)
                return;

            // 별의 바닥(bounds.min.y) 기준
            Bounds b = starCol.bounds;
            Vector3 bottomWorld = new Vector3(b.center.x, b.min.y, starCol.transform.position.z);

            // 월드 -> 스크린
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, bottomWorld);
            screenPos.y -= underStarPaddingPx;

            // 스크린 -> 캔버스 로컬(anchoredPosition)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, screenPos, _uiCamera, out Vector2 localPos);

            _underStarRect.anchoredPosition = localPos;
            arrowImageForNearstStar.gameObject.SetActive(true);
        }

        private void HideAll()
        {
            if (arrowUI != null) arrowUI.Show(false);
            if (arrowImageForNearstStar != null) arrowImageForNearstStar.gameObject.SetActive(false);
        }

        public void SetHintSelect(bool isCan)
        {
            isSelectHint = isCan;
            if (!isCan) HideAll();
        }
    }
}
