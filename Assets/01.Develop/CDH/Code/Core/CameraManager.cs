using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    [Header("VCams")]
    [SerializeField] private CinemachineCamera vcamDefault;
    [SerializeField] private CinemachineCamera vcamFollow;
    [SerializeField] private CinemachineCamera vFullCam;
    private CinemachineCamera currentCam;

    private int activePriority = 10;
    private int backPriority = 1;

    [Header("Zoom")]
    [Range(0.5f, 5f)]
    [SerializeField] private float followZoomFactor = 0.85f;
    [SerializeField] private float catchZoomFactor = 0.85f;

    [Header("Blend Speeds")]
    [Tooltip("따라가기 전환 속도(작을수록 빠름)")]
    [SerializeField] private float followBlendTime = 0.12f;

    [Tooltip("복귀 전환 속도(작을수록 빠름)")]
    [SerializeField] private float returnBlendTime = 0.05f;

    [SerializeField] private CinemachineBlendDefinition.Styles blendStyle = CinemachineBlendDefinition.Styles.EaseInOut;

    private CinemachineBrain brain;
    private CinemachineBlendDefinition savedBlend;

    private float savedDefaultOrtho;

    private Transform currentTarget;
    private bool isFollowing;
    public  bool isFullCamActive = false;

    private void Awake()
    {
        if (vcamDefault == null || vcamFollow == null || vFullCam == null)
        {
            Debug.LogError("[CameraManager] 모든 VCam을 연결해 주세요.");
            enabled = false;
            return;
        }

        var mainCam = Camera.main;
        if (mainCam != null)
            brain = mainCam.GetComponent<CinemachineBrain>();

        if (brain != null)
            savedBlend = brain.DefaultBlend;

        savedDefaultOrtho = vcamDefault.Lens.OrthographicSize;

        vcamDefault.Priority = activePriority;
        vcamFollow.Priority = backPriority;
        vFullCam.Priority = backPriority;

        isFollowing = false;
    }

    private void LateUpdate()
    {
        // 따라가던 대상이 사라지면 복귀
        if (isFollowing && currentTarget == null)
            EndFollowObj();

        // Tab 키로 FullCam 전환
        if (!isFollowing)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ActivateFullCam();
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                DeactivateFullCam();
            }
        }
    }

    public void BeginFollowObj(Transform target)
    {
        if (target == null) return;

        currentTarget = target;
        isFollowing = true;

        savedDefaultOrtho = vcamDefault.Lens.OrthographicSize;

        vcamFollow.Follow = target;
        vcamFollow.LookAt = null;

        float targetSize = savedDefaultOrtho * followZoomFactor;

        DOTween.To(
            () => vcamFollow.Lens.OrthographicSize,
            x => vcamFollow.Lens.OrthographicSize = x,
            targetSize,
            2f
        ).SetEase(Ease.Linear);

        SetBlendTime(followBlendTime);

        vcamDefault.Priority = backPriority;
        vcamFollow.Priority = activePriority;
        vFullCam.Priority = backPriority;
    }

    public void CatchFollowObj(Transform target)
    {
        if (target == null) return;

        currentTarget = target;
        isFollowing = true;

        savedDefaultOrtho = vcamDefault.Lens.OrthographicSize;

        vcamFollow.Follow = target;
        vcamFollow.LookAt = null;

        float targetSize = savedDefaultOrtho * catchZoomFactor;

        DOTween.To(
            () => vcamFollow.Lens.OrthographicSize,
            x => vcamFollow.Lens.OrthographicSize = x,
            targetSize,
            0.4f
        ).SetEase(Ease.OutQuad);

        SetBlendTime(followBlendTime);

        vcamDefault.Priority = backPriority;
        vcamFollow.Priority = activePriority;
        vFullCam.Priority = backPriority;
    }

    public void EndFollowObj()
    {
        isFollowing = false;
        currentTarget = null;

        SetBlendTime(returnBlendTime);

        vcamFollow.Follow = null;
        vcamFollow.LookAt = null;

        vcamDefault.Lens.OrthographicSize = savedDefaultOrtho;

        vcamDefault.Priority = activePriority;
        vcamFollow.Priority = backPriority;
        vFullCam.Priority = backPriority;

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

    private void ActivateFullCam()
    {
        if (isFullCamActive) return;
        isFullCamActive = true;

        vFullCam.Priority = activePriority + 1;
        vcamDefault.Priority = backPriority;
        vcamFollow.Priority = backPriority;
    }

    private void DeactivateFullCam()
    {
        if (!isFullCamActive) return;
        isFullCamActive = false;

        vFullCam.Priority = backPriority;

        if (isFollowing)
        {
            vcamDefault.Priority = backPriority;
            vcamFollow.Priority = activePriority;
        }
        else
        {
            vcamDefault.Priority = activePriority;
            vcamFollow.Priority = backPriority;
        }
    }
}