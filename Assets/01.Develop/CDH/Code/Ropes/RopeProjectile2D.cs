using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeProjectile2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 18f;
    [SerializeField] private float speedByCharge = 10f;
    [SerializeField] private float maxLifeTime = 2.5f;
    [SerializeField] private float maxDistance = 15f;

    [Header("Aim Assist (비행 중 조정)")]
    [Range(0f, 1f)]
    [SerializeField] private float aimBlendToMouse = 0.25f;

    [SerializeField] private float maxTurnDegPerSec = 720f;

    [Header("Diagonal Bias (좌상단=왼쪽, 우하단=오른쪽)")]
    [Tooltip("0=비활성, 값이 클수록 좌/우 편향이 강해짐")]
    [Range(0f, 3f)]
    [SerializeField] private float diagonalBiasStrength = 0.8f;

    [Tooltip("0=편향만(수평), 1=마우스 방향 기반 + 편향 섞기")]
    [Range(0f, 1f)]
    [SerializeField] private float diagonalBiasBlendIntoMouseDir = 0.6f;

    [Tooltip("대각선 값 커브(1=선형, 2=끝쪽(좌상/우하)에서 더 세짐)")]
    [Range(0.2f, 4f)]
    [SerializeField] private float diagonalBiasExponent = 1.2f;

    [Tooltip("화면 중앙 근처에서 편향을 덜 주고 싶으면 0.1~0.3 정도")]
    [Range(0f, 0.49f)]
    [SerializeField] private float diagonalDeadZone = 0.05f;

    [Header("Rope Visual")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private float lineWidth = 0.05f;

    private Rigidbody2D rb;
    private Camera cam;

    private Transform origin;
    private Vector2 launchDir;
    private float speed;
    private Vector2 startPos;
    private float alive;
    public void StartRope(float charge01)
    {
        Vector2 dir = new Vector2(transform.position.x - Mouse.current.position.value.x, transform.position.z - Mouse.current.position.value.y);
        Launch(transform, dir, charge01);
    }

    public void Launch(Transform originTransform, Vector2 initialDir, float charge01)
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        origin = originTransform;
        launchDir = initialDir.normalized;
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
        cam = Camera.main;
        SetupLineIfNeeded();
    }

    private void FixedUpdate()
    {

        Vector2 v = rb.linearVelocity;
        Vector2 curDir = (v.sqrMagnitude > 0.0001f) ? v.normalized : launchDir;

        // 1) 기본 마우스 방향(월드 기준)
        Vector2 mouseDir = curDir;
        if (cam != null)
        {
            Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 toMouse = mouseWorld - rb.position;
            if (toMouse.sqrMagnitude > 0.0001f)
                mouseDir = toMouse.normalized;
        }

        // 2) 좌상단↔우하단 편향으로 “수평 방향” 만들기
        Vector2 biasDir = GetDiagonalBiasHorizontalDir();

        // 3) 마우스 방향에 편향을 섞어 “목표 방향” 만들기
        // - diagonalBiasBlendIntoMouseDir가 높을수록 마우스 방향 기반 + 편향
        // - diagonalBiasStrength가 높을수록 좌/우로 더 꺾임
        Vector2 biasedMouseDir = mouseDir;
        if (diagonalBiasStrength > 0f)
        {
            // 마우스 방향에 수평 편향을 더해 목표 방향 생성
            Vector2 add = biasDir * diagonalBiasStrength;
            Vector2 mixed = (mouseDir + add).normalized;

            // mouseDir과 mixed 사이를 블렌드(원하는 느낌 조절)
            biasedMouseDir = Vector2.Lerp(mouseDir, mixed, diagonalBiasBlendIntoMouseDir).normalized;
        }

        // 4) “현재 진행 방향 ↔ (편향된) 마우스 목표” 블렌드로 최종 목표
        Vector2 blendedTarget = Vector2.Lerp(curDir, biasedMouseDir, aimBlendToMouse).normalized;

        // 5) 회전 속도 제한
        float maxTurn = maxTurnDegPerSec * Time.fixedDeltaTime;
        Vector2 newDir = RotateToward(curDir, blendedTarget, maxTurn);

        rb.linearVelocity = newDir * speed;
    }

    private Vector2 GetDiagonalBiasHorizontalDir()
    {
        // 우하단이면 +1(오른쪽), 좌상단이면 -1(왼쪽)
        // diag01 = (nx + (1-ny)) / 2
        Vector3 mp = Input.mousePosition;
        float nx = (Screen.width > 0) ? Mathf.Clamp01(mp.x / Screen.width) : 0.5f;
        float ny = (Screen.height > 0) ? Mathf.Clamp01(mp.y / Screen.height) : 0.5f;

        float diag01 = (nx + (1f - ny)) * 0.5f; // 좌상=0, 우하=1

        // 데드존: 중앙 근처 편향 약화
        float centered = diag01 - 0.5f;
        float abs = Mathf.Abs(centered);
        if (abs < diagonalDeadZone) centered = 0f;
        else centered = Mathf.Sign(centered) * ((abs - diagonalDeadZone) / (0.5f - diagonalDeadZone));

        // 0~1로 복구
        float shaped01 = Mathf.Clamp01(centered * 0.5f + 0.5f);

        // 커브(지수)
        shaped01 = Mathf.Pow(shaped01, diagonalBiasExponent);

        float xBias = Mathf.Lerp(-1f, 1f, shaped01);
        return new Vector2(Mathf.Sign(xBias), 0f); // “좌/우” 방향 자체
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

    private static Vector2 RotateToward(Vector2 from, Vector2 to, float maxDeg)
    {
        float a = Vector2.SignedAngle(from, to);
        float clamped = Mathf.Clamp(a, -maxDeg, maxDeg);
        return (Quaternion.Euler(0f, 0f, clamped) * from).normalized;
    }
}
