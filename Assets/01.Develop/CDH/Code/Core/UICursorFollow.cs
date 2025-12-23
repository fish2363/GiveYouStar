using _01.Develop.LSW._01._Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

public class UICursorFollow : MonoSingleton<UICursorFollow>
{
    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform cursorRect;
    [SerializeField] private Image cursorImage;

    [Header("Cursor Settings")]
    [Tooltip("커서 텍스처 기준: 왼쪽 위가 (0,0). 예) 화살표 끝이 (2,2)면 2,2")]
    [SerializeField] private Vector2 hotSpotPixels = Vector2.zero;

    [SerializeField] private Vector2 cursorSize = new Vector2(128, 128);

    protected override void Awake()
    {
        base.Awake();

        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!cursorImage && cursorRect) cursorImage = cursorRect.GetComponent<Image>();

        Cursor.visible = false; // 시스템 커서 숨김

        // 크기 고정
        if (cursorRect) cursorRect.sizeDelta = cursorSize;

        // 핫스팟을 "pivot"으로 맞추면, 위치 계산이 깔끔해짐
        ApplyHotspotAsPivot();
    }

    void Update()
    {
        if (!canvas || !cursorRect) return;

        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, Input.mousePosition, cam, out Vector2 localPos))
        {
            // Canvas 중앙 기준(local) 좌표를 그대로 anchoredPosition에 넣으면 됨
            cursorRect.anchoredPosition = localPos;
        }
    }

    void OnApplicationFocus(bool focus)
    {
        // Alt+Tab 등 포커스 바뀔 때 시스템 커서가 필요하면 보여주고,
        // 다시 돌아오면 숨김
        Cursor.visible = !focus;
    }

    private void ApplyHotspotAsPivot()
    {
        if (!cursorRect || !cursorImage || !cursorImage.sprite) return;

        var r = cursorImage.sprite.rect;
        float w = r.width;
        float h = r.height;

        // hotSpotPixels는 "왼쪽 위 기준"
        // UI pivot은 "왼쪽 아래 기준"이라 y를 뒤집어줌
        Vector2 pivot = new Vector2(
            w <= 0 ? 0f : hotSpotPixels.x / w,
            h <= 0 ? 1f : 1f - (hotSpotPixels.y / h)
        );

        cursorRect.pivot = pivot;
        cursorRect.anchorMin = cursorRect.anchorMax = new Vector2(0.5f, 0.5f); // 중앙 기준
    }
}
