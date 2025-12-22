using _01.Develop.LSW._01._Scripts.So;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public float maxTimer;
    float currentTimer;
    [SerializeField] private TextMeshProUGUI timerText;
    public CameraManager cameraManager;

    public UnityEvent StartRopeCharge;
    public UnityEvent EndRopeCharge;

    private bool isRopeCharging;
    private bool isRopeChargeEnd;
    private bool isRopeChargeTurn;
    private bool isCatchStar;
    private bool isGameStart;

    public List<StarSo> getStarList=new();

    private void Awake()
    {
        isRopeCharging = false;
        isRopeChargeEnd = false;
        isRopeChargeTurn = true;
        isCatchStar = false;
    }
    private void Start()
    {
        GameStart();
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
        timerText.text = currentTimer.ToString();
        if (0 >= currentTimer)
        {
            GameEnd();
        }

        if (isRopeChargeTurn && !isCatchStar & !cameraManager.isFullCamActive)
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
    }

    public void SetRopeChargeTurn() => isRopeChargeTurn = true;
    public void SetCatchStar() => isCatchStar = true;
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
