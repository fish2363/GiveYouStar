using _01.Develop.LSW._01._Scripts.So;
using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RopePullController : MonoBehaviour
{
    public UnityEvent OnChainBreak;
    public UnityEvent<StarSo> OnGetStar;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private bool autoFindMainCamera = true;

    [Header("Target (현재 잡힌 스타)")]
    [SerializeField] private StarMover starTarget;

    [Header("Rope Line")]
    [SerializeField] private LineRenderer ropeLine;
    [SerializeField] private bool autoCreateRopeLine = true;
    [SerializeField] private float lineWidth = 0.05f;

    [Header("Camera Follow / Collect")]
    [SerializeField] private bool followWhenRoped = true;
    [SerializeField] private float collectDistance = 1.2f;
    [SerializeField] private bool destroyOnCollect = true;

    [Header("Star Drift")]
    [SerializeField] private Vector2 driftDirection = new Vector2(1, 1);
    [SerializeField] private float driftSpeed = 8f;

    [Tooltip("옆으로 새는 속도를 드리프트 방향으로 되돌리는 힘")]
    [SerializeField] private float driftReturnStrength = 3f;

    [Header("Pull Input")]
    [SerializeField] private float minDragWorld = 0.8f;
    [SerializeField] private float maxDragWorld = 5f;
    [Range(1f, 4f)] [SerializeField] private float lengthPower = 2.4f;
    [SerializeField] private float minEffectiveDragDuration = 0.12f;
    [SerializeField] private float pullCooldown = 0.12f;

    [Header("Pull Strength")]
    [SerializeField] private float baseImpulse = 12f;

    [Header("Pull Velocity Damping")]
    [SerializeField] private float pullVelocityDamping = 8f;

    [Header("Rope Break Settings")]
    [SerializeField] private float maxRopeStretchDistance = 10f;

    [Header("PostProcessing - Lens Distortion (URP Volume)")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private float lensDistortionTarget = -0.35f;
    [SerializeField] private float lensDownDuration = 0.08f;
    [SerializeField] private float lensReturnDuration = 0.18f;
    [SerializeField] private Ease lensDownEase = Ease.OutQuad;
    [SerializeField] private Ease lensReturnEase = Ease.OutQuad;

    private LensDistortion lensDistortion;
    private Tween lensTween;

    private Rigidbody2D starRb;
    private Camera cam;

    private bool isDragging;
    private Vector2 dragStartWorld;
    private float dragStartTime;
    private float nextPullTime;
    private Vector2 pullVel;
    private Vector2 lastPullPosition;

    private void Awake()
    {
        if (autoFindMainCamera)
            cam = Camera.main;

        if (driftDirection.sqrMagnitude < 0.0001f)
            driftDirection = Vector2.right;
        driftDirection = driftDirection.normalized;

        if (autoCreateRopeLine && ropeLine == null)
            SetupRopeLine();

        CacheLensDistortion();

        if (starTarget != null)
            SetTarget(starTarget);
    }

    private void CacheLensDistortion()
    {
        lensDistortion = null;

        if (postProcessVolume == null || postProcessVolume.profile == null)
            return;

        postProcessVolume.profile.TryGet(out lensDistortion);
    }

    private void Update()
    {
        if (starTarget == null) return;

        UpdateRopeLine();

        if (player != null && Vector2.Distance(starTarget.transform.position, player.position) <= collectDistance)
        {
            OnGetStar?.Invoke(starTarget.MyInfo);
            EndPull();
            return;
        }

        if (cam == null || player == null || starRb == null) return;

        // ✅ 드래그 시작: 렌즈 왜곡 "내리기"
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartWorld = MouseWorld2D();
            dragStartTime = Time.time;

            PlayLensDistortionDown();
        }

        // ✅ 드래그 끝: 렌즈 왜곡 "복귀"
        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging) return;
            isDragging = false;

            PlayLensDistortionReturn();

            TryPullOnRelease(MouseWorld2D(), Time.time - dragStartTime);
        }
    }

    private void FixedUpdate()
    {
        if (starRb == null) return;

        if (pullVelocityDamping > 0f)
        {
            float k = Mathf.Exp(-pullVelocityDamping * Time.fixedDeltaTime);
            pullVel *= k;
        }

        if (driftReturnStrength > 0f)
        {
            Vector2 perp = pullVel - driftDirection * Vector2.Dot(pullVel, driftDirection);
            pullVel -= perp * driftReturnStrength * Time.fixedDeltaTime;
        }

        starRb.gravityScale = 0f;
        starRb.linearVelocity = driftDirection * driftSpeed + pullVel;

        if (maxRopeStretchDistance > 0f)
        {
            float distFromLastPull = Vector2.Distance(starTarget.transform.position, lastPullPosition);
            if (distFromLastPull > maxRopeStretchDistance)
            {
                Vector2 toPlayer = ((Vector2)player.position - (Vector2)starTarget.transform.position).normalized;
                Vector2 currentDir = starRb.linearVelocity.normalized;

                if (Vector2.Dot(toPlayer, currentDir) < 0f)
                {
                    EndPull();
                    return;
                }
            }
        }
    }

    private void EndPull()
    {
        // 안전하게 렌즈 복귀
        PlayLensDistortionReturn(force: true);

        if (cameraManager != null) cameraManager.EndFollowObj();
        if (destroyOnCollect && starTarget != null)
            starTarget.SetStop(false);

        if (starTarget != null)
            starTarget.OnDestroy -= HandleDestroy;

        OnChainBreak?.Invoke();
        ClearTarget();
    }

    public void SetTarget(StarMover newTarget)
    {
        starTarget = newTarget;
        starRb = null;
        pullVel = Vector2.zero;
        nextPullTime = 0f;

        if (starTarget == null) return;

        starRb = starTarget.GetComponent<Rigidbody2D>();
        if (starRb == null)
        {
            Debug.LogError("[RopePullController] Star Target에 Rigidbody2D가 없습니다.");
            starTarget = null;
            return;
        }

        starTarget.OnDestroy += HandleDestroy;
        lastPullPosition = starTarget.transform.position;

        if (followWhenRoped && cameraManager != null)
            cameraManager.CatchFollowObj(starTarget.transform);

        UpdateRopeLine();
    }

    private void HandleDestroy()
    {
        EndPull();
    }

    public void ClearTarget()
    {
        starTarget = null;
        starRb = null;
        pullVel = Vector2.zero;
        isDragging = false;

        if (ropeLine != null)
            ropeLine.enabled = false;
    }

    private void TryPullOnRelease(Vector2 dragEndWorld, float dragDuration)
    {
        if (Time.time < nextPullTime) return;
        nextPullTime = Time.time + pullCooldown;

        if (starRb == null || starTarget == null) return;

        Vector2 drag = dragEndWorld - dragStartWorld;
        float dist = drag.magnitude;
        if (dist < minDragWorld) return;

        float durationPenalty = Mathf.Clamp01(dragDuration / Mathf.Max(0.001f, minEffectiveDragDuration));
        float len01 = Mathf.Clamp01((dist / Mathf.Max(0.001f, maxDragWorld)) * durationPenalty);
        float lenMul = Mathf.Pow(len01, lengthPower);

        Vector2 dirToPlayer = ((Vector2)player.position - starRb.position).normalized;
        float impulse = baseImpulse * lenMul;

        Vector2 dv = dirToPlayer * (impulse / Mathf.Max(0.001f, starRb.mass));
        pullVel = dv;

        lastPullPosition = starTarget.transform.position;
    }

    // ---------------- Lens Distortion ----------------

    // 드래그 중에 -0.35까지 "내리기"
    private void PlayLensDistortionDown(bool force = false)
    {
        if (lensDistortion == null) CacheLensDistortion();
        if (lensDistortion == null) return;

        float target = Mathf.Clamp(lensDistortionTarget, -1f, 1f);

        // 이미 트윈 중이면 씹기(원래 요구) / force면 강제로 교체
        if (!force && lensTween != null && lensTween.IsActive() && lensTween.IsPlaying())
            return;

        lensTween?.Kill();
        lensTween = DOTween.To(
                () => lensDistortion.intensity.value,
                x => lensDistortion.intensity.value = x,
                target,
                lensDownDuration
            )
            .SetEase(lensDownEase);
    }

    // 드래그를 떼면 0으로 "복귀"
    private void PlayLensDistortionReturn(bool force = false)
    {
        if (lensDistortion == null) CacheLensDistortion();
        if (lensDistortion == null) return;

        // 복귀는 "못 돌아가서 왜곡 남는" 상황 생기면 짜증나니까
        // force 아니더라도 기존 트윈은 끊고 복귀시키는 쪽이 안정적임
        lensTween?.Kill();

        lensTween = DOTween.To(
                () => lensDistortion.intensity.value,
                x => lensDistortion.intensity.value = x,
                0f,
                lensReturnDuration
            )
            .SetEase(lensReturnEase);
    }

    // -------------------------------------------------

    private Vector2 MouseWorld2D()
    {
        if (cam == null) cam = Camera.main;

        Vector3 m = Input.mousePosition;
        m.z = (cam != null) ? -cam.transform.position.z : 0f;
        return (cam != null) ? (Vector2)cam.ScreenToWorldPoint(m) : Vector2.zero;
    }

    private void SetupRopeLine()
    {
        GameObject go = new GameObject("RopeLine");
        ropeLine = go.AddComponent<LineRenderer>();
        ropeLine.positionCount = 2;
        ropeLine.useWorldSpace = true;
        ropeLine.startWidth = lineWidth;
        ropeLine.endWidth = lineWidth;
        ropeLine.material = new Material(Shader.Find("Sprites/Default"));
        Gradient brownGradient = new Gradient();

        brownGradient.SetKeys(
            new GradientColorKey[]
            {
        new GradientColorKey(new Color(0.36f, 0.20f, 0.09f), 0f), // Dark Brown
        new GradientColorKey(new Color(0.59f, 0.29f, 0.00f), 0.5f), // Brown
        new GradientColorKey(new Color(0.76f, 0.60f, 0.42f), 1f), // Light Brown
            },
            new GradientAlphaKey[]
            {
        new GradientAlphaKey(1f, 0f),
        new GradientAlphaKey(1f, 1f),
            }
        );
        ropeLine.colorGradient = brownGradient;
        ropeLine.enabled = false;
    }

    private void UpdateRopeLine()
    {
        if (ropeLine == null || player == null || starTarget == null) return;

        ropeLine.enabled = true;
        ropeLine.SetPosition(0, player.position);
        ropeLine.SetPosition(1, starTarget.transform.position);
    }
}
