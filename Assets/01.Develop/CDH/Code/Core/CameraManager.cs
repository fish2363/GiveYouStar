using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("VCams")]
    [SerializeField] private CinemachineCamera vcamDefault;
    [SerializeField] private CinemachineCamera vcamFollow;

    [Header("Priorities")]
    [SerializeField] private int defaultPriority = 10;
    [SerializeField] private int followPriority = 20;

    [Header("Zoom")]
    [Range(0.5f, 1f)]
    [SerializeField] private float followZoomFactor = 0.85f;

    [Header("Blend Speeds")]
    [Tooltip("따라가기 전환 속도(작을수록 빠름)")]
    [SerializeField] private float followBlendTime = 0.12f;

    [Tooltip("복귀 전환 속도(작을수록 빠름)")]
    [SerializeField] private float returnBlendTime = 0.05f;

    [SerializeField] private CinemachineBlendDefinition.Styles blendStyle = CinemachineBlendDefinition.Styles.EaseInOut;

    private CinemachineBrain brain;
    private CinemachineBlendDefinition savedBlend;

    private float savedDefaultOrtho;
    private int savedDefaultPriority;
    private int savedFollowPriority;

    private Transform currentTarget;
    private bool isFollowing;

    private void Awake()
    {
        if (vcamDefault == null || vcamFollow == null)
        {
            Debug.LogError("[CameraManager] vcamDefault/vcamFollow를 연결해 주세요.");
            enabled = false;
            return;
        }

        // Brain 찾기
        var mainCam = Camera.main;
        if (mainCam != null)
            brain = mainCam.GetComponent<CinemachineBrain>();

        if (brain != null)
            savedBlend = brain.DefaultBlend;

        // 시작 상태 저장
        savedDefaultOrtho = vcamDefault.Lens.OrthographicSize;
        savedDefaultPriority = vcamDefault.Priority;
        savedFollowPriority = vcamFollow.Priority;

        // 시작은 기본 카메라
        vcamDefault.Priority = defaultPriority;
        vcamFollow.Priority = defaultPriority - 1;

        isFollowing = false;
    }

    private void LateUpdate()
    {
        // 따라가던 대상이 사라지면 자동 복귀
        if (isFollowing && currentTarget == null)
            EndFollowObj();
    }

    public void BeginFollowObj(Transform target)
    {
        if (target == null) return;

        currentTarget = target;
        isFollowing = true;

        // “잡기 직전” 기본 상태 저장(원상복구용)
        savedDefaultOrtho = vcamDefault.Lens.OrthographicSize;
        savedDefaultPriority = vcamDefault.Priority;
        savedFollowPriority = vcamFollow.Priority;

        // Follow cam 세팅
        vcamFollow.Follow = target;
        vcamFollow.LookAt = null;
        vcamFollow.Lens.OrthographicSize = savedDefaultOrtho * followZoomFactor;

        // 전환 속도(따라가기)
        SetBlendTime(followBlendTime);

        // Follow cam 활성화
        vcamDefault.Priority = defaultPriority;
        vcamFollow.Priority = followPriority;
    }

    public void EndFollowObj()
    {
        isFollowing = false;
        currentTarget = null;

        // 복귀 전환 속도
        SetBlendTime(returnBlendTime);

        // Follow 해제
        vcamFollow.Follow = null;
        vcamFollow.LookAt = null;

        // 기본 상태 복구
        vcamDefault.Lens.OrthographicSize = savedDefaultOrtho;

        // 원래 Priority로 복귀
        vcamDefault.Priority = savedDefaultPriority;
        vcamFollow.Priority = savedFollowPriority;

        // 원래 블렌드로 되돌리고 싶으면(선택)
        RestoreBlendLater(returnBlendTime + 0.01f);
    }

    private void SetBlendTime(float t)
    {
        if (brain == null) return;
        brain.DefaultBlend = new CinemachineBlendDefinition(blendStyle, Mathf.Max(0f, t));
    }

    private void RestoreBlendLater(float delay)
    {
        if (brain == null) return;
        CancelInvoke(nameof(RestoreBlendNow));
        Invoke(nameof(RestoreBlendNow), delay);
    }

    private void RestoreBlendNow()
    {
        if (brain == null) return;
        brain.DefaultBlend = savedBlend;
    }
}
