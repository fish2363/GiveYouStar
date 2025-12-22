using UnityEngine;

/// <summary>
/// 스타(타겟)를 외부에서 받아서 로프 당김/드리프트/수집/카메라 팔로우/라인렌더러를 제어하는 컨트롤러.
/// 스타 오브젝트에는 Rigidbody2D만 있으면 됨.
/// </summary>
public class RopePullController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private bool autoFindMainCamera = true;

    [Header("Target (현재 잡힌 스타)")]
    [SerializeField] private Transform starTarget; // 시작부터 잡힌 상태 테스트용(원하면 비워도 됨)

    [Header("Rope Line")]
    [SerializeField] private LineRenderer ropeLine;
    [SerializeField] private bool autoCreateRopeLine = true;
    [SerializeField] private float lineWidth = 0.05f;

    [Header("Camera Follow / Collect")]
    [SerializeField] private bool followWhenRoped = true;
    [SerializeField] private float collectDistance = 1.2f;
    [SerializeField] private bool destroyOnCollect = true;

    [Header("Star Drift (기본: 한 방향으로 계속 이동)")]
    [SerializeField] private Vector2 driftDirection = Vector2.right;
    [SerializeField] private float driftSpeed = 8f;

    [Tooltip("옆으로 새는 속도를 드리프트 방향으로 되돌리는 힘")]
    [SerializeField] private float driftReturnStrength = 3f;

    [Header("Pull Input (드래그)")]
    [SerializeField] private float minDragWorld = 0.8f;
    [SerializeField] private float maxDragWorld = 5f;

    [Range(1f, 4f)]
    [SerializeField] private float lengthPower = 2.4f;

    [SerializeField] private float minEffectiveDragDuration = 0.12f;
    [SerializeField] private float pullCooldown = 0.12f;

    [Header("Pull Strength")]
    [SerializeField] private float baseImpulse = 12f;

    [Range(0f, 1f)]
    [SerializeField] private float minAlignmentMultiplier = 0.1f;

    [Range(0f, 1f)]
    [SerializeField] private float pullDirectionToPlayerBlend = 0.85f;

    [SerializeField] private AnimationCurve alignmentCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Rope Power (당길 때마다 약해짐)")]
    [SerializeField] private float ropePowerStart = 1f;

    [Range(0.5f, 1f)]
    [SerializeField] private float ropePowerMulPerPull = 0.92f;

    [Range(0f, 1f)]
    [SerializeField] private float ropePowerMin = 0.1f;

    [Header("Pull Velocity Damping")]
    [SerializeField] private float pullVelocityDamping = 1.2f;

    private Rigidbody2D starRb;
    private Camera cam;

    private bool isDragging;
    private Vector2 dragStartWorld;
    private float dragStartTime;
    private float nextPullTime;

    private float ropePower;
    private Vector2 pullVel;

    private void Awake()
    {
        if (autoFindMainCamera)
            cam = Camera.main;

        if (driftDirection.sqrMagnitude < 0.0001f)
            driftDirection = Vector2.right;
        driftDirection = driftDirection.normalized;

        ropePower = Mathf.Clamp01(ropePowerStart);

        if (autoCreateRopeLine && ropeLine == null)
            SetupRopeLine();

        // 인스펙터에 스타가 들어있으면 시작부터 타겟으로 세팅
        if (starTarget != null)
            SetTarget(starTarget);
    }

    private void Update()
    {
        if (starTarget == null) return;

        UpdateRopeLine();

        // 수집(플레이어 근처)
        if (player != null && Vector2.Distance(starTarget.position, player.position) <= collectDistance)
        {
            if (cameraManager != null) cameraManager.EndFollowObj();

            if (destroyOnCollect && starTarget != null)
                Destroy(starTarget.gameObject);

            ClearTarget();
            return;
        }

        if (cam == null || player == null || starRb == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartWorld = MouseWorld2D();
            dragStartTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging) return;
            isDragging = false;

            TryPullOnRelease(MouseWorld2D(), Time.time - dragStartTime);
        }

        if (Input.GetKeyDown(KeyCode.R))
            ropePower = Mathf.Clamp01(ropePowerStart);
    }

    private void FixedUpdate()
    {
        if (starRb == null) return;

        if (pullVelocityDamping > 0f)
        {
            float k = Mathf.Exp(-pullVelocityDamping * Time.fixedDeltaTime);
            pullVel *= k;
        }

        // 한 방향 유지 느낌(옆으로 샌 성분 되돌림)
        if (driftReturnStrength > 0f)
        {
            Vector2 perp = pullVel - driftDirection * Vector2.Dot(pullVel, driftDirection);
            pullVel -= perp * driftReturnStrength * Time.fixedDeltaTime;
        }

        starRb.gravityScale = 0f;
        starRb.linearVelocity = driftDirection * driftSpeed + pullVel;
    }

    /// <summary>
    /// 외부에서 잡힌 스타(타겟)를 세팅하는 함수
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        starTarget = newTarget;
        starRb = null;

        pullVel = Vector2.zero;
        ropePower = Mathf.Clamp01(ropePowerStart);
        nextPullTime = 0f;

        if (starTarget == null) return;

        starRb = starTarget.GetComponent<Rigidbody2D>();
        if (starRb == null)
        {
            Debug.LogError("[RopePullController] Star Target에 Rigidbody2D가 없습니다.");
            starTarget = null;
            return;
        }

        if (followWhenRoped && cameraManager != null)
            cameraManager.BeginFollowObj(starTarget);

        UpdateRopeLine();
    }

    /// <summary>
    /// 타겟 해제(로프 끊김 등)
    /// </summary>
    public void ClearTarget()
    {
        starTarget = null;
        starRb = null;
        pullVel = Vector2.zero;
        isDragging = false;

        // 라인 숨기고 싶으면 여기서 끄면 됨
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

        Vector2 dragDir = drag.normalized;
        Vector2 dirToPlayer = ((Vector2)player.position - starRb.position).normalized;

        float align01 = Mathf.Clamp01(Vector2.Dot(dragDir, dirToPlayer));
        float alignMul = alignmentCurve != null ? alignmentCurve.Evaluate(align01) : align01;
        alignMul = Mathf.Lerp(minAlignmentMultiplier, 1f, alignMul);

        float impulse = baseImpulse * lenMul * alignMul * ropePower;
        if (impulse <= 0.0001f) return;

        Vector2 outDir = Vector2.Lerp(dragDir, dirToPlayer, pullDirectionToPlayerBlend).normalized;

        Vector2 dv = outDir * (impulse / Mathf.Max(0.001f, starRb.mass));
        pullVel += dv;

        // 당길 때마다 약해짐
        ropePower = Mathf.Max(ropePowerMin, ropePower * ropePowerMulPerPull);
    }

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
        ropeLine.startColor = Color.white;
        ropeLine.endColor = Color.white;
        ropeLine.enabled = false;
    }

    private void UpdateRopeLine()
    {
        if (ropeLine == null || player == null || starTarget == null) return;

        ropeLine.enabled = true;
        ropeLine.SetPosition(0, player.position);
        ropeLine.SetPosition(1, starTarget.position);
    }
}
