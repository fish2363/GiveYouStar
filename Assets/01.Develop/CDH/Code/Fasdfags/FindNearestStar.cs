using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Assets._01.Develop.CDH.Code.Fasdfags
{
    public class ArrowUI
    {
        public RectTransform Rect { get; set; }
        public Transform TargetTrm { get; set; }
        public Transform PlayerTrm { get; set; }

        public RectTransform CanvasRect { get; set; }
        public Camera WorldCamera { get; set; }   // 월드->스크린 변환용 (보통 MainCamera)
        public Camera UICamera { get; set; }      // ScreenPointToLocalPointInRectangle용 (Overlay면 null)

        public float UiRadius { get; set; } = 120f;
        public float AngleOffset { get; set; } = 0f; // 화살표 이미지 기본 방향 보정 (기본이 위면 0)

        public void Update()
        {
            if (Rect == null || TargetTrm == null || PlayerTrm == null || CanvasRect == null) return;
            if (WorldCamera == null) WorldCamera = Camera.main;
            if (WorldCamera == null) return;

            // 1) 월드 -> 스크린 좌표
            Vector2 playerScreen = WorldCamera.WorldToScreenPoint(PlayerTrm.position);
            Vector2 targetScreen = WorldCamera.WorldToScreenPoint(TargetTrm.position);

            Vector2 dir = targetScreen - playerScreen;
            if (dir.sqrMagnitude < 0.0001f) return;

            Vector2 dirN = dir.normalized;

            // 2) 플레이어 화면 위치를 중심으로 반경 UiRadius 원 위에 화살표 배치
            Vector2 arrowScreen = playerScreen + dirN * UiRadius;

            // 3) 스크린 -> 캔버스 로컬(anchoredPosition)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                CanvasRect,
                arrowScreen,
                UICamera, // Screen Space Overlay면 null
                out Vector2 localPoint
            );

            Rect.anchoredPosition = localPoint;

            // 4) 회전: "위(+Y)"가 타겟 방향을 보게
            float angleX = Mathf.Atan2(dirN.y, dirN.x) * Mathf.Rad2Deg; // X축 기준
            float angleY = angleX - 90f;                                // Y축(+Y) 기준
            Rect.localRotation = Quaternion.Euler(0f, 0f, angleY + AngleOffset);
        }
    }

    public class FindNearestStar : MonoBehaviour
    {
        [Header("Find Settings")]
        [SerializeField] private float maxDistance = 30f;
        [SerializeField] private int maxFindedStar = 3;
        [SerializeField] private LayerMask starLayer;
        [SerializeField] private float refreshInterval = 0.2f;

        [Header("UI Arrow")]
        [SerializeField] private Transform playerTrm;
        [SerializeField] private GameObject arrowPrefab;   
        [SerializeField] private RectTransform arrowRoot; 
        [SerializeField] private Canvas canvas;               
        [SerializeField] private Camera worldCamera;          
        [SerializeField] private float arrowUiRadius = 120f;  

        private readonly List<ArrowUI> arrows = new();
        private float timer = 100000f;

        private void Awake()
        {
            if (canvas == null) canvas = GetComponentInParent<Canvas>();
            if (worldCamera == null) worldCamera = Camera.main;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > refreshInterval)
            {
                timer = 0f;
                Refresh();
            }

            foreach (var arrow in arrows)
                arrow.Update();
        }

        private void Refresh()
        {
            var found = Physics2D.OverlapCircleAll(playerTrm.position, maxDistance, starLayer);

            // ✅ 기존 화살표 오브젝트 제거 (안 하면 화면에 “멈춘 화살표”가 남아 보일 수 있음)
            for (int i = arrowRoot.childCount - 1; i >= 0; i--)
                Destroy(arrowRoot.GetChild(i).gameObject);

            arrows.Clear();

            int count = Mathf.Min(maxFindedStar, found.Length);

            // UICamera는 Canvas RenderMode에 따라:
            // - Screen Space Overlay : null
            // - Screen Space Camera / World Space : canvas.worldCamera
            Camera uiCam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? canvas.worldCamera
                : null;

            RectTransform canvasRect = canvas.transform as RectTransform;

            for (int i = 0; i < count; i++)
            {
                var star = found[i];

                RectTransform rectTrm = arrowPrefab.GetComponent<RectTransform>();
                RectTransform rt = Instantiate(rectTrm, arrowRoot);
                rt.anchoredPosition = Vector2.zero;
                rt.localRotation = Quaternion.identity;

                var arrow = new ArrowUI
                {
                    Rect = rt,
                    TargetTrm = star.transform,   // ✅ 별 Transform을 “참조”로 저장
                    PlayerTrm = playerTrm,
                    CanvasRect = canvasRect,
                    WorldCamera = worldCamera,
                    UICamera = uiCam,
                    UiRadius = arrowUiRadius,
                    AngleOffset = 0f
                };

                arrows.Add(arrow);
            }
        }
    }
}
