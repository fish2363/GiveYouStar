using UnityEngine;

namespace Assets._01.Develop.CDH.Code.Fasdfags
{
    [DefaultExecutionOrder(10000)]
    public class ArrowUI : MonoBehaviour
    {
        [SerializeField] private float angleOffsetDeg;

        [Header("Canvas")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera worldCamera; // 보통 MainCamera

        [Header("Child (Arrow Image)")]
        [SerializeField] private RectTransform childRect;

        [Header("Scale by world distance (optional)")]
        [SerializeField] private float worldMinDistance = 40f; 
        [SerializeField] private float worldMaxDistance = 100f;
        [SerializeField] private float minScale = 0.0f;
        [SerializeField] private float maxScale = 1.0f;

        [Header("Radius (parent -> child)")]
        [SerializeField] private bool useDynamicRadius = true;   
        [SerializeField] private float fixedRadius = 80f;        
        [SerializeField] private float radiusMin = 40f;          
        [SerializeField] private float radiusMax = 100f;         
        [SerializeField] private Vector2 childAxis = Vector2.up; 

        private RectTransform myRect;
        private RectTransform canvasRect;
        private Camera uiCamera;

        private Transform pivotWorld;
        private Transform targetWorld;

        private void Awake()
        {
            myRect = GetComponent<RectTransform>();

            if (canvas == null) canvas = GetComponentInParent<Canvas>();
            canvasRect = canvas != null ? (RectTransform)canvas.transform : null;

            uiCamera = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? canvas.worldCamera
                : null;

            if (worldCamera == null) worldCamera = Camera.main;

            if (childRect == null && transform.childCount > 0)
                childRect = transform.GetChild(0).GetComponent<RectTransform>();

            Show(false);
        }

        public void SetPivotWorld(Transform pivot) => pivotWorld = pivot;
        public void SetTargetWorld(Transform target) => targetWorld = target;

        // ✅ 외부에서 반지름을 직접 바꾸고 싶을 때
        public void SetFixedRadius(float radius)
        {
            fixedRadius = Mathf.Max(0f, radius);
            useDynamicRadius = false;
        }

        // ✅ 외부에서 반지름 범위를 바꾸고 싶을 때(거리 기반)
        public void SetRadiusRange(float min, float max)
        {
            radiusMin = Mathf.Max(0f, min);
            radiusMax = Mathf.Max(radiusMin, max);
            useDynamicRadius = true;
        }

        private void LateUpdate()
        {
            if (!IsValid()) { Show(false); return; }

            Vector2 pivotPos = WorldToCanvasLocal(pivotWorld.position);
            Vector2 targetPos = WorldToCanvasLocal(targetWorld.position);

            myRect.anchoredPosition = pivotPos;

            Vector2 dir = targetPos - pivotPos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float zRot = angle - 90f + angleOffsetDeg;
            myRect.localRotation = Quaternion.Euler(0f, 0f, zRot);

            float dist = dir.magnitude;
            float t = Mathf.InverseLerp(worldMinDistance, worldMaxDistance, dist);

            // ✅ 반지름(부모->자식 거리) 결정
            float radius = useDynamicRadius
                ? Mathf.Lerp(radiusMin, radiusMax, t)
                : fixedRadius;

            SetChildDistance(radius);

            // 스케일은 거리 기반(원하면 끄거나 값 고정 가능)
            float scale = Mathf.Lerp(minScale, maxScale, t);
            if (childRect != null) childRect.localScale = Vector3.one * scale;

            Show(true);
        }

        private void SetChildDistance(float radius)
        {
            if (childRect == null) return;
            childRect.anchoredPosition = childAxis.normalized * radius;
        }

        private Vector2 WorldToCanvasLocal(Vector3 worldPos)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, uiCamera, out Vector2 localPoint);

            return localPoint;
        }

        private bool IsValid()
        {
            return canvasRect != null && worldCamera != null && childRect != null &&
                   pivotWorld != null && targetWorld != null;
        }

        public void Show(bool active)
        {
            if (childRect != null)
                childRect.gameObject.SetActive(active);
        }
    }
}
