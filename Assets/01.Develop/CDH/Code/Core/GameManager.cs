using _01.Develop.LSW._01._Scripts.So;
using System;
using System.Collections;
using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Ami.BroAudio;

public class GameManager : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private SoundID gameEndBellSoundID;
    [Space]


    public float maxTimer;
    float currentTimer;
    [SerializeField] private SpriteRenderer clickUI;

    [SerializeField] private Transform timerVisual; // ✅ 회전용 이미지 (예: RectTransform or 일반 Transform)
    public CameraManager cameraManager;
    [SerializeField] private TMP_Text countText;
    public UnityEvent<bool> OnStartCharge;

    public UnityEvent StartRopeCharge;
    public UnityEvent EndRopeCharge;

    private bool isRopeCharging;
    private bool isRopeChargeEnd;
    private bool isRopeChargeTurn;
    private bool isCatchStar;
    private bool isGameStart;

    private Coroutine coroutine;

    public List<StarSo> getStarList = new();

    private float initialRotationZ;

    private void Awake()
    {
        isRopeCharging = false;
        isRopeChargeEnd = false;
        isRopeChargeTurn = true;
        isCatchStar = false;

        if (timerVisual != null)
            initialRotationZ = timerVisual.localEulerAngles.z;
    }
    private IEnumerator Start()
    {
        countText.text = "3";
        yield return new WaitForSeconds(1f);
        countText.text = "2";
        yield return new WaitForSeconds(1f);
        countText.text = "1";
        yield return new WaitForSeconds(1f);
        countText.text = "";
        clickUI.enabled = true;
        GameStart();
        yield return new WaitForSeconds(4f);
        clickUI.enabled = false;
    }

    public void AddStar(StarSo star)
    {
        getStarList.Add(star);
        Debug.Log(star.starName);
    }

    public void GameStart()
    {
        isGameStart = true;
        currentTimer = maxTimer;
    }

    private void Update()
    {
        if (!isGameStart) return;

        currentTimer -= Time.deltaTime;
        currentTimer = Mathf.Clamp(currentTimer, 0f, maxTimer);

        // ✅ 이미지 회전 처리
        if (timerVisual != null)
        {
            float t = 1f - (currentTimer / maxTimer); // 0 → 1 로 변함
            float rotX = initialRotationZ + 180f * t; // 점점 180도까지 증가
            Vector3 currentEuler = timerVisual.localEulerAngles;
            timerVisual.localEulerAngles = new Vector3(currentEuler.x, currentEuler.y, rotX);
        }

        if (currentTimer <= 0f)
        {
            GameEnd();
        }

        if (isRopeChargeTurn && !isCatchStar && !cameraManager.isFullCamActive)
        {
            if (!isRopeCharging && Mouse.current.leftButton.wasPressedThisFrame)
            {
                isRopeCharging = true;
                isRopeChargeEnd = false;
                StartRopeCharge?.Invoke();
            }
            if (isRopeCharging && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isRopeCharging = false;
                isRopeChargeEnd = true;
                isRopeChargeTurn = false;
                EndRopeCharge?.Invoke();
            }
        }
    }

    private void GameEnd()
    {
        isGameStart = false;
        StarManager.Instance.EndGame();
        // TODO: 게임 종료 후 행동 정의
        BroAudio.Play(gameEndBellSoundID);
    }

    public void SetRopeChargeTurn()
    {
        if(coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(SetRopeChargeTurnRoutine());
    }

    public IEnumerator SetRopeChargeTurnRoutine()
    {
        clickUI.enabled = false;
        countText.text = "3";
        yield return new WaitForSeconds(1f);
        countText.text = "2";
        yield return new WaitForSeconds(1f);
        countText.text = "1";
        yield return new WaitForSeconds(1f);
        countText.text = "";
        clickUI.enabled = true;
        isRopeChargeTurn = true;
        OnStartCharge?.Invoke(true);
        yield return new WaitForSeconds(4f);
        clickUI.enabled = false;
    }
    public void SetCatchStar()
    {
        isCatchStar = true;
        OnStartCharge?.Invoke(false);
    }

    public void EndCatchStar()
    {
        StartCoroutine(EndCatchStarRoutine());
    }

    private IEnumerator EndCatchStarRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        isCatchStar = false;
    }
}
