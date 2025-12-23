using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture; // 인스펙터에서 넣기
    [SerializeField] private Vector2 hotSpot = Vector2.zero; // 커서의 클릭 포인트 (보통 중심이나 왼쪽 위)
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
}
