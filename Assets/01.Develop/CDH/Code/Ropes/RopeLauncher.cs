using Assets._01.Develop.CDH.Code.Fasdfags;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RopeLauncher : MonoBehaviour
{
    public UnityEvent OnRopeFinish;
    public UnityEvent OnRopeCatchStar;

    [Header("References")]
    [SerializeField] private ArrowUI arrowUI;
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

        Vector2 worldMouse = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (worldMouse - (Vector2)spawnPos).normalized;

        curRope.Launch(playerTrm, dir, charge01);
        curRope.OnFinishRope += HandleCurRopeEnd;
        curRope.OnCatchStar += HandleCurRopeCatchStar;

        arrowUI.SetPivotWorld(curRope.transform);

        if (cameraManager != null)
            cameraManager.BeginFollowObj(curRope.transform);
    }

    private void HandleCurRopeCatchStar(StarMover starTrm)
    {
        starTrm.SetStop(true);
        ropePullController.SetTarget(starTrm);
        OnRopeCatchStar?.Invoke();
        arrowUI.Show(false);
    }

    private void HandleCurRopeEnd()
    {
        if (curRope == null) return;
        curRope.OnFinishRope -= HandleCurRopeEnd;
        curRope.OnCatchStar -= HandleCurRopeCatchStar;
        OnRopeFinish?.Invoke();
        arrowUI.Show(false);
    }
}