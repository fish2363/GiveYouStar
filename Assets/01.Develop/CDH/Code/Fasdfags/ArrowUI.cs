using UnityEngine;

namespace Assets._01.Develop.CDH.Code.Fasdfags
{
    public class ArrowUI : MonoBehaviour
    {
        [SerializeField] private float angleOffsetDeg;

        [Header("Canvas")]
        [SerializeField] private Canvas canvas;     // ArrowUI가 속한 Canvas
        [SerializeField] private Camera worldCamera; // 월드 오브젝트를 Screen으로 바꿀 카메라 (보통 MainCamera)

        [Header("Child (Arrow Image)")]
        [SerializeField] private RectTransform childRect;

        [Header("Distance / Scale")]
        [SerializeField] private float minDistance = 40f;
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private float minScale = 0f;
        [SerializeField] private float maxScale = 1f;

        [Header("Child offset axis")]
        [SerializeField] private Vector2 childAxis = Vector2.up;

        private RectTransform myRect;
        private RectTransform canvasRect;
        private Camera uiCamera; // ScreenSpace-Camera/WorldSpace일 때만 필요

        private Transform pivotWorld;   // 부모 위치 기준(월드 오브젝트)
        private Transform targetWorld;  // 가리킬 타겟(월드 오브젝트)
        private bool isShow;

        private void Awake()
        {
            Show(false);
            myRect = GetComponent<RectTransform>();

            if (canvas == null) canvas = GetComponentInParent<Canvas>();
            canvasRect = canvas.transform as RectTransform;

            // UI 카메라(Overlay면 null)
            uiCamera = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

            if (worldCamera == null) worldCamera = Camera.main;

            if (childRect == null && transform.childCount > 0)
                childRect = transform.GetChild(0).GetComponent<RectTransform>();
        }

        // ✅ 부모 위치를 정할 월드 기준점(플레이어 같은 거)
        public void SetPivotWorld(Transform pivot) => pivotWorld = pivot;

        // ✅ 가리킬 월드 타겟
        public void SetTargetWorld(Transform target) => targetWorld = target;

        public void SetChildDistance(float distance)
        {
            if (childRect == null) return;
            childRect.anchoredPosition = childAxis.normalized * distance;
        }

        private void Update()
        {
            if (canvasRect == null || worldCamera == null)
            {
                Show(false);
                return;
            }
            if (pivotWorld == null || targetWorld == null) 
            {
                Show(false);
                return; 
            }

            // 월드 -> 캔버스 로컬
            Vector2 pivotPos = WorldToCanvasLocal(pivotWorld.position);
            Vector2 targetPos = WorldToCanvasLocal(targetWorld.position);

            // 1) 부모(ArrowUI) 위치를 pivot에 고정
            myRect.anchoredPosition = pivotPos;

            // 2) pivot -> target 방향으로 회전
            Vector2 dir = targetPos - pivotPos; // ✅ 이게 맞음
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float zRot = angle - 90f + angleOffsetDeg;
            myRect.localRotation = Quaternion.Euler(0f, 0f, zRot);

            // 3) 거리 기반으로 자식 거리/스케일
            float dist = dir.magnitude;
            float t = Mathf.InverseLerp(minDistance, maxDistance, dist);

            float childDistance = Mathf.Lerp(minDistance, maxDistance, t);
            float scale = Mathf.Lerp(minScale, maxScale, t);

            SetChildDistance(childDistance);
            if (childRect != null) childRect.localScale = Vector3.one * scale;
        }

        private Vector2 WorldToCanvasLocal(Vector3 worldPos)
        {
            // 월드 -> 스크린 (여긴 "월드카메라"가 필요)
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPos);

            // 스크린 -> 캔버스 로컬 (Overlay면 uiCamera=null)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, uiCamera, out Vector2 localPoint);

            return localPoint;
        }

        public void Show(bool active)
        {
            childRect.gameObject.SetActive(active);
        }
    }
}
