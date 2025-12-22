using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Rope : MonoBehaviour
{
    public event Action OnFinishRope;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 18f;
    [SerializeField] private float speedByCharge = 10f;
    [SerializeField] private float maxLifeTime = 2.5f;
    [SerializeField] private float maxDistance = 18f;

    [Header("Bias Turn (Screen): y>x면 왼쪽 / y<x면 오른쪽")]
    [Tooltip("초당 회전량(도/초). 클수록 더 빨리 휩니다.")]
    [SerializeField] private float biasTurnDegPerSec = 180f;

    [Tooltip("최대 회전량(도). 초기 발사 방향 기준. 0이면 무제한")]
    [SerializeField] private float maxBiasAngleDeg = 35f;

    [Tooltip("대각선(y=x) 근처에서 흔들림 방지 (0.03~0.12 추천)")]
    [Range(0f, 0.49f)]
    [SerializeField] private float deadZone = 0.07f;

    [Tooltip("구석으로 갈수록 더 강해지는 곡선(1=선형, 2~3=끝에서 강해짐)")]
    [Range(0.2f, 4f)]
    [SerializeField] private float exponent = 1.4f;

    [Tooltip("true면 |y-x|가 클수록 더 많이 휨, false면 방향만 보고 일정하게 휨")]
    [SerializeField] private bool scaleByDistanceFromDiagonal = true;

    [Tooltip("혹시 또 반대로 느껴지면 체크해서 뒤집기")]
    [SerializeField] private bool invertLeftRight = false;

    [Header("Rope Visual")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private float lineWidth = 0.05f;

    private Rigidbody2D rb;
    private Transform origin;

    private Vector2 launchDir;   // 초기 발사 방향(최대 회전량 기준)
    private float speed;

    private Vector2 startPos;
    private float alive;

    private float biasAngleDeg;  // launchDir 기준 누적 회전각(클램프 대상)

    public void Launch(Transform originTransform, Vector2 initialDir, float charge01)
    {
        origin = originTransform;
        launchDir = (initialDir.sqrMagnitude > 0.0001f) ? initialDir.normalized : Vector2.right;

        speed = baseSpeed + speedByCharge * Mathf.Clamp01(charge01);

        startPos = rb.position;
        alive = 0f;

        biasAngleDeg = 0f;
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
            OnFinishRope?.Invoke();
            Destroy(gameObject);
            return;
        }

        alive += Time.fixedDeltaTime;
        if (alive >= maxLifeTime || Vector2.Distance(startPos, rb.position) >= maxDistance)
        {
            OnFinishRope?.Invoke();
            Destroy(gameObject);
            return;
        }

        // Screen 좌표 정규화
        float nx = (Screen.width > 0) ? Mathf.Clamp01(Input.mousePosition.x / Screen.width) : 0.5f;
        float ny = (Screen.height > 0) ? Mathf.Clamp01(Input.mousePosition.y / Screen.height) : 0.5f;

        // y>x면 "왼쪽", y<x면 "오른쪽"
        // 왼쪽 = +각도(반시계), 오른쪽 = -각도(시계)
        float delta = ny - nx; // 양수면 y>x
        float abs = Mathf.Abs(delta);

        // 데드존 + 강도(0~1)
        float t = 0f;
        if (abs > deadZone)
            t = (abs - deadZone) / (1f - deadZone); // 0~1

        t = Mathf.Clamp01(Mathf.Pow(t, exponent));

        float strength01 = scaleByDistanceFromDiagonal ? t : (abs > deadZone ? 1f : 0f);

        // 방향 부호: y>x면 +1(왼쪽/CCW), y<x면 -1(오른쪽/CW)
        float sign = (delta >= 0f) ? 1f : -1f;
        if (invertLeftRight) sign *= -1f;

        float turnThisFrame = sign * biasTurnDegPerSec * strength01 * Time.fixedDeltaTime;

        // 누적 회전각 업데이트 + 최대 회전량 클램프
        biasAngleDeg += turnThisFrame;
        if (maxBiasAngleDeg > 0f)
            biasAngleDeg = Mathf.Clamp(biasAngleDeg, -maxBiasAngleDeg, maxBiasAngleDeg);

        // 최종 방향 = launchDir을 biasAngleDeg만큼 회전 (속도 유지)
        Vector2 newDir = (Quaternion.Euler(0f, 0f, biasAngleDeg) * launchDir).normalized;
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
