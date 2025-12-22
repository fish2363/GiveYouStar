using _01.Develop.LSW._01._Scripts.So;
using System;
using UnityEngine;
using UnityEngine.Events;

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

        if (starTarget != null)
            SetTarget(starTarget);
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
    }

    private void FixedUpdate()
    {
        if (starRb == null) return;

        // 감쇠
        if (pullVelocityDamping > 0f)
        {
            float k = Mathf.Exp(-pullVelocityDamping * Time.fixedDeltaTime);
            pullVel *= k;
        }

        // 드리프트 유지 (옆으로 새는 거 보정)
        if (driftReturnStrength > 0f)
        {
            Vector2 perp = pullVel - driftDirection * Vector2.Dot(pullVel, driftDirection);
            pullVel -= perp * driftReturnStrength * Time.fixedDeltaTime;
        }

        starRb.gravityScale = 0f;
        starRb.linearVelocity = driftDirection * driftSpeed + pullVel;

        // 로프 끊김 조건
        if (maxRopeStretchDistance > 0f)
        {
            float distFromLastPull = Vector2.Distance(starTarget.transform.position, lastPullPosition);
            if (distFromLastPull > maxRopeStretchDistance)
            {
                Vector2 toPlayer = ((Vector2)player.position - (Vector2)starTarget.transform.position).normalized;
                Vector2 currentDir = starRb.linearVelocity.normalized;

                // 플레이어 반대 방향으로 갈 때만 끊기
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
        if (cameraManager != null) cameraManager.EndFollowObj();
        if (destroyOnCollect && starTarget != null)
            starTarget.SetStop(false);

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
        ropeLine.SetPosition(1, starTarget.transform.position);
    }
}

