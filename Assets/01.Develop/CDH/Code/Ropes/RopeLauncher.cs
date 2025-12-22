using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RopeLauncher : MonoBehaviour
{
    public UnityEvent OnRopeFinish;
    public UnityEvent OnRopeCatchStar;

    [Header("References")]
    [SerializeField] private Transform playerTrm;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private RopePullController ropePullController;
    [SerializeField] private Rope ropePrefab;

    [Header("Spawn")]
    [SerializeField] private Vector2 spawnOffset = Vector2.zero;

    private Camera cam;
    private Rope curRope;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void LaunchByCharge(float charge01)
    {
        if (ropePrefab == null) return;

        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;
        curRope = Instantiate(ropePrefab, spawnPos, Quaternion.identity);

        // 마우스 위치 → 방향 계산
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.value);
        Vector2 dir = (mouseWorld - (Vector2)spawnPos).normalized;

        curRope.Launch(playerTrm, dir, charge01);
        curRope.OnFinishRope += HandleCurRopeEnd;
        curRope.OnCatchStar += HandleCurRopeCatchStar;

        if (cameraManager != null)
            cameraManager.BeginFollowObj(curRope.transform);
    }

    private void HandleCurRopeCatchStar(StarMover starTrm)
    {
        starTrm.SetStop();
        ropePullController.SetTarget(starTrm.transform);

        OnRopeCatchStar?.Invoke();
    }

    private void HandleCurRopeEnd()
    {
        curRope.OnFinishRope -= HandleCurRopeEnd;
        curRope.OnCatchStar -= HandleCurRopeCatchStar;
        OnRopeFinish?.Invoke();
    }
}
