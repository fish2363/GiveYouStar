using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class RopeLauncher : MonoBehaviour             
{
    public UnityEvent OnRopeFinish;
    public UnityEvent OnRopeCatchStar;

    [Header("References")]
    [SerializeField] private Transform playerTrm;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private RopePullController ropePullController;
    [SerializeField] private Rope ropePrefab;

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
    private Rope curRope;

    private void Awake() => cam = Camera.main;

    public void LaunchByCharge(float charge01)
    {
        if (ropePrefab == null) return; 

        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;
        curRope = Instantiate(ropePrefab, spawnPos, Quaternion.identity);

        // (예시) 초기 방향: 플레이어 오른쪽
        curRope.Launch(playerTrm, Mouse.current.position.value, charge01);
        curRope.OnFinishRope += HandleCurRopeEnd;
        curRope.OnCatchStar += HandleCurRopeCatchStar;

        // ✅ 카메라가 로프 따라가게
        if (cameraManager != null)
            cameraManager.BeginFollowObj(curRope.transform);
    }

    private void HandleCurRopeCatchStar(Transform starTrm)
    {
        ropePullController.SetTarget(starTrm);
        OnRopeCatchStar?.Invoke();
    }

    private void HandleCurRopeEnd()
    {
        curRope.OnFinishRope -= HandleCurRopeEnd;
        curRope.OnCatchStar -= HandleCurRopeCatchStar;
        OnRopeFinish?.Invoke();
    }
}
