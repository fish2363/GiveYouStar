using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum Mode { Idle, FollowStar, ReturnToSaved }

    [Header("References")]
    [SerializeField] private Camera targetCamera;

    [Header("Follow")]
    [SerializeField] private Vector3 starOffset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float followSmoothTime = 0.18f;
    [SerializeField] private float zoomSmoothTime = 0.20f;

    [Tooltip("스타 따라갈 때 확대(줌인) 비율. 0.85면 15% 확대(ortho size 감소)")]
    [Range(0.5f, 1f)]
    [SerializeField] private float followZoomFactor = 0.85f;

    private Mode mode = Mode.Idle;
    private Transform star;

    private Vector3 savedPos;
    private float savedOrtho;

    private Vector3 velPos;
    private float velZoom;

    private void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null)
        {
            Debug.LogError("[CameraManager] Camera가 없습니다.");
            enabled = false;
            return;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        // 스타가 파괴/사라졌는데 Follow중이면 자동 복귀
        if (mode == Mode.FollowStar && star == null)
            EndFollowStar();

        Vector3 targetPos = targetCamera.transform.position;
        float targetOrtho = targetCamera.orthographicSize;

        if (mode == Mode.FollowStar && star != null)
        {
            targetPos = star.position + starOffset;
            targetOrtho = savedOrtho * followZoomFactor; // 저장된 기본 사이즈 기준으로 줌인
        }
        else if (mode == Mode.ReturnToSaved)
        {
            targetPos = savedPos;
            targetOrtho = savedOrtho;

            bool posDone = Vector3.Distance(targetCamera.transform.position, targetPos) < 0.03f;
            bool zoomDone = Mathf.Abs(targetCamera.orthographicSize - targetOrtho) < 0.03f;
            if (posDone && zoomDone)
                mode = Mode.Idle;
        }
        else
        {
            // Idle: 아무것도 안 함(현재 카메라 상태 유지)
            return;
        }

        targetCamera.transform.position = Vector3.SmoothDamp(
            targetCamera.transform.position, targetPos, ref velPos, followSmoothTime);

        targetCamera.orthographicSize = Mathf.SmoothDamp(
            targetCamera.orthographicSize, targetOrtho, ref velZoom, zoomSmoothTime);
    }

    /// <summary>
    /// "지금 카메라 상태"를 저장한 뒤 스타를 따라감(줌인 포함)
    /// </summary>
    public void BeginFollowStar(Transform starTransform)
    {
        if (targetCamera == null) return;

        // ✅ “처음 카메라 상태” 저장(잡기 직전 상태)
        savedPos = targetCamera.transform.position;
        savedOrtho = targetCamera.orthographicSize;

        star = starTransform;
        mode = Mode.FollowStar;
    }

    /// <summary>
    /// 저장해둔 상태(잡기 전)로 복귀
    /// </summary>
    public void EndFollowStar()
    {
        star = null;
        mode = Mode.ReturnToSaved;
    }
}
