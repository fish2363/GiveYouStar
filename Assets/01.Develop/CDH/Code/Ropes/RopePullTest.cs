using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopePullTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private LineRenderer ropeLine;
    [SerializeField] private bool autoCreateRopeLine = true;

    [Header("Camera Follow / Collect")]
    [SerializeField] private bool followWhenRoped = true;   // 테스트: 시작부터 이미 걸린 상태
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

    private Rigidbody2D rb;
    private Camera cam;

    private bool isDragging;
    private Vector2 dragStartWorld;
    private float dragStartTime;
    private float nextPullTime;

    private float ropePower;
    private Vector2 pullVel;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        rb.gravityScale = 0f;

        if (driftDirection.sqrMagnitude < 0.0001f)
            driftDirection = Vector2.right;
        driftDirection = driftDirection.normalized;

        ropePower = Mathf.Clamp01(ropePowerStart);

        if (autoCreateRopeLine && ropeLine == null)
            SetupRopeLine();
    }

    private void Start()
    {
        // ✅ 플레이어 따라가지 말고, 스타를 따라가게
        if (followWhenRoped && cameraManager != null)
            cameraManager.BeginFollowObj(transform);
    }

    private void Update()
    {
        UpdateRopeLine();

        // 먹기(플레이어 근처)
        if (player != null && Vector2.Distance(transform.position, player.position) <= collectDistance)
        {
            if (cameraManager != null) cameraManager.EndFollowObj();
            if (destroyOnCollect) Destroy(gameObject);
            return;
        }

        if (cam == null || player == null) return;

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

        // 테스트 편의: R로 로프파워 리셋
        if (Input.GetKeyDown(KeyCode.R))
            ropePower = Mathf.Clamp01(ropePowerStart);
    }

    private void FixedUpdate()
    {
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

        rb.linearVelocity = driftDirection * driftSpeed + pullVel;
    }

    private void TryPullOnRelease(Vector2 dragEndWorld, float dragDuration)
    {
        if (Time.time < nextPullTime) return;
        nextPullTime = Time.time + pullCooldown;

        Vector2 drag = dragEndWorld - dragStartWorld;
        float dist = drag.magnitude;

        if (dist < minDragWorld) return;

        float durationPenalty = Mathf.Clamp01(dragDuration / Mathf.Max(0.001f, minEffectiveDragDuration));

        float len01 = Mathf.Clamp01((dist / Mathf.Max(0.001f, maxDragWorld)) * durationPenalty);
        float lenMul = Mathf.Pow(len01, lengthPower);

        Vector2 dragDir = drag.normalized;
        Vector2 dirToPlayer = ((Vector2)player.position - rb.position).normalized;

        float align01 = Mathf.Clamp01(Vector2.Dot(dragDir, dirToPlayer));
        float alignMul = alignmentCurve != null ? alignmentCurve.Evaluate(align01) : align01;
        alignMul = Mathf.Lerp(minAlignmentMultiplier, 1f, alignMul);

        float impulse = baseImpulse * lenMul * alignMul * ropePower;
        if (impulse <= 0.0001f) return;

        Vector2 outDir = Vector2.Lerp(dragDir, dirToPlayer, pullDirectionToPlayerBlend).normalized;

        Vector2 dv = outDir * (impulse / Mathf.Max(0.001f, rb.mass));
        pullVel += dv;

        // 당길 때마다 약해짐
        ropePower = Mathf.Max(ropePowerMin, ropePower * ropePowerMulPerPull);
    }

    private Vector2 MouseWorld2D()
    {
        Vector3 m = Input.mousePosition;
        m.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(m);
    }

    private void SetupRopeLine()
    {
        GameObject go = new GameObject("RopeLine");
        ropeLine = go.AddComponent<LineRenderer>();
        ropeLine.positionCount = 2;
        ropeLine.useWorldSpace = true;
        ropeLine.startWidth = 0.05f;
        ropeLine.endWidth = 0.05f;
        ropeLine.material = new Material(Shader.Find("Sprites/Default"));
        ropeLine.startColor = Color.white;
        ropeLine.endColor = Color.white;
    }

    private void UpdateRopeLine()
    {
        if (ropeLine == null || player == null) return;
        ropeLine.SetPosition(0, player.position);
        ropeLine.SetPosition(1, transform.position);
    }
}
