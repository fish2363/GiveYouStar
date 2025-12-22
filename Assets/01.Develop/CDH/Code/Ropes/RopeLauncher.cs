using UnityEngine;

public class RopeLauncher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RopeCharge ropeCharge;
    [SerializeField] private RopeProjectile2D ropePrefab;

    [Header("Initial Aim")]
    [Tooltip("초기 발사 방향: 기본 방향 ↔ 마우스 방향 블렌드")]
    [Range(0f, 1f)]
    [SerializeField] private float initialAimToMouseBlend = 0.6f;

    [Header("Spawn")]
    [SerializeField] private Vector2 spawnOffset = Vector2.zero;

    [Header("Base Direction")]
    [SerializeField] private bool useTransformRightAsBaseDir = true;
    [SerializeField] private Vector2 fixedBaseDir = Vector2.right;

    private Camera cam;

    private void Awake() => cam = Camera.main;

    private void Start()
    {
        if (ropeCharge != null)
            ropeCharge.onReleaseCharge01.AddListener(LaunchByCharge);
    }

    public void LaunchByCharge(float charge01)
    {
        if (ropePrefab == null) return;

        Vector2 baseDir = useTransformRightAsBaseDir ? (Vector2)transform.right : fixedBaseDir;
        if (baseDir.sqrMagnitude < 0.0001f) baseDir = Vector2.right;
        baseDir.Normalize();

        Vector2 mouseDir = baseDir;
        if (cam != null)
        {
            Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 toMouse = mouseWorld - (Vector2)transform.position;
            if (toMouse.sqrMagnitude > 0.0001f) mouseDir = toMouse.normalized;
        }

        Vector2 initialDir = Vector2.Lerp(baseDir, mouseDir, initialAimToMouseBlend).normalized;

        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;
        var rope = Instantiate(ropePrefab, spawnPos, Quaternion.identity);
        rope.Launch(transform, initialDir, charge01);
    }
}
