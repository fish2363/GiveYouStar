using UnityEngine;

public class RopePullController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private bool autoFindMainCamera = true;

    [Header("Target (현재 잡힌 스타)")]
    [SerializeField] private Transform starTarget;

    [Header("Rope Line")]
    [SerializeField] private LineRenderer ropeLine;
    [SerializeField] private bool autoCreateRopeLine = true;
    [SerializeField] private float lineWidth = 0.05f;

    [Header("Camera Follow / Collect")]
    [SerializeField] private bool followWhenRoped = true;
    [SerializeField] private float collectDistance = 1.2f;
    [SerializeField] private bool destroyOnCollect = true;

    [Header("Star Drift")]
    [SerializeField] private Vector2 driftDirection = new Vector2(1f, 1f);
    [SerializeField] private float driftSpeed = 8f;

    [Header("Pull Settings")]
    [SerializeField] private float pullSpeed = 12f;

    private Rigidbody2D starRb;
    private Camera cam;

    private bool isDragging;
    private bool isPulling;
    private Vector2 dragStartWorld;
    private float dragStartTime;

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

        if (player != null && Vector2.Distance(starTarget.position, player.position) <= collectDistance)
        {
            if (cameraManager != null) cameraManager.EndFollowObj();
            if (destroyOnCollect && starTarget != null)
                Destroy(starTarget.gameObject);

            ClearTarget();
            return;
        }

        if (cam == null || player == null || starRb == null) return;

        // 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartWorld = MouseWorld2D();
            dragStartTime = Time.time;
            isPulling = true;
        }

        // 드래그 끝
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            isPulling = false;
        }
    }

    private void FixedUpdate()
    {
        if (starRb == null || player == null) return;

        if (isPulling)
        {
            Vector2 toPlayer = ((Vector2)player.position - starRb.position).normalized;
            starRb.linearVelocity = toPlayer * pullSpeed;
        }
        else
        {
            starRb.linearVelocity = driftDirection.normalized * driftSpeed;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        starTarget = newTarget;
        starRb = null;

        isPulling = false;

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

    public void ClearTarget()
    {
        starTarget = null;
        starRb = null;
        isDragging = false;
        isPulling = false;

        if (ropeLine != null)
            ropeLine.enabled = false;
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
