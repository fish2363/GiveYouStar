using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeProjectile2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 18f;
    [SerializeField] private float speedByCharge = 10f;
    [SerializeField] private float maxLifeTime = 2.5f;
    [SerializeField] private float maxDistance = 18f;

    [Header("Diagonal Turn Bias (Screen): y>x면 왼쪽 / y<x면 오른쪽)")]
    [Tooltip("초당 최대 회전량(도). 클수록 좌/우로 더 빨리 휜다")]
    [SerializeField] private float biasTurnDegPerSec = 180f;

    [Tooltip("대각선(y=x) 근처에서 흔들림 방지(0.03~0.12 추천)")]
    [Range(0f, 0.49f)]
    [SerializeField] private float deadZone = 0.07f;

    [Tooltip("구석으로 갈수록 더 강해지는 곡선(1=선형, 2~3=끝에서 강해짐)")]
    [Range(0.2f, 4f)]
    [SerializeField] private float exponent = 1.4f;

    [Tooltip("true면 |x-y|가 클수록 더 많이 휨, false면 방향만 보고 항상 일정하게 휨")]
    [SerializeField] private bool scaleByDistanceFromDiagonal = true;

    [Header("Rope Visual")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private float lineWidth = 0.05f;

    private Rigidbody2D rb;
    private Transform origin;
    private Vector2 launchDir;
    private float speed;

    private Vector2 startPos;
    private float alive;

    public void Launch(Transform originTransform, Vector2 initialDir, float charge01)
    {
        origin = originTransform;
        launchDir = initialDir.sqrMagnitude > 0.0001f ? initialDir.normalized : Vector2.right;
        speed = baseSpeed + speedByCharge * Mathf.Clamp01(charge01);

        startPos = rb.position;
        alive = 0f;

        rb.linearVelocity = launchDir * speed;

        SetupLineIfNeeded();
        UpdateLine();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        SetupLineIfNeeded();
    }

    private void FixedUpdate()
    {
        if (origin == null)
        {
            Destroy(gameObject);
            return;
        }

        alive += Time.fixedDeltaTime;
        if (alive >= maxLifeTime || Vector2.Distance(startPos, rb.position) >= maxDistance)
        {
            Destroy(gameObject);
            return;
        }

        // 현재 진행 방향
        Vector2 v = rb.linearVelocity;
        Vector2 curDir = (v.sqrMagnitude > 0.0001f) ? v.normalized : launchDir;

        // Screen space: y>x면 왼쪽, y<x면 오른쪽
        float nx = (Screen.width > 0) ? Mathf.Clamp01(Input.mousePosition.x / Screen.width) : 0.5f;
        float ny = (Screen.height > 0) ? Mathf.Clamp01(Input.mousePosition.y / Screen.height) : 0.5f;

        float delta = nx - ny;                 // delta>0 => 오른쪽, delta<0 => 왼쪽
        float abs = Mathf.Abs(delta);

        // 대각선 근처 흔들림 방지
        float t = 0f;
        if (abs > deadZone)
            t = (abs - deadZone) / (1f - deadZone); // 0~1

        t = Mathf.Clamp01(Mathf.Pow(t, exponent));

        // 방향(좌/우)
        float sign = (delta >= 0f) ? 1f : -1f;

        // 강도(원하면 거리 기반, 아니면 항상 1)
        float strength01 = scaleByDistanceFromDiagonal ? t : (abs > deadZone ? 1f : 0f);

        // 이번 프레임 회전량(도)
        float turnThisFrame = sign * biasTurnDegPerSec * strength01 * Time.fixedDeltaTime;

        // 방향 회전
        Vector2 newDir = (Quaternion.Euler(0f, 0f, turnThisFrame) * curDir).normalized;

        // 속도 유지
        rb.linearVelocity = newDir * speed;
    }

    private void LateUpdate() => UpdateLine();

    private void SetupLineIfNeeded()
    {
        if (line != null) return;

        var go = new GameObject("RopeLine");
        go.transform.SetParent(transform, worldPositionStays: true);

        line = go.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;
    }

    private void UpdateLine()
    {
        if (line == null || origin == null) return;
        line.SetPosition(0, origin.position);
        line.SetPosition(1, transform.position);
    }
}
