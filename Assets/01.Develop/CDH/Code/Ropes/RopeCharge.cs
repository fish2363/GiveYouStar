using Assets._01.Develop.CDH.Code.Ropes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RopeCharge : MonoBehaviour
{
    [Header("Charge Settings")]
    [Tooltip("차지 속도(초당 몇 번 왕복하느냐 느낌). 값이 클수록 더 빨리 왔다갔다.")]
    [SerializeField] private float pingPongSpeed = 1.2f;

    [Tooltip("차지 최소/최대(0~1). 예: 0.1~1이면 완전 0은 안 나오게 가능")]
    [Range(0f, 1f)][SerializeField] private float minCharge01 = 0f;
    [Range(0f, 1f)][SerializeField] private float maxCharge01 = 1f;

    [Tooltip("차지 곡선(선형이 싫으면 커브로 맛 조절)")]
    [SerializeField] private ChargingDataSO[] chargingDatas;

    [Header("UI (Optional)")]
    [SerializeField] private Image chargeFillImage;    // Slider 대신 Image.fillAmount 써도 됨
    [SerializeField] private TMPro.TMP_Text chargeTMP; // TMP 쓰면 이걸로

    [Header("Events")]
    [Tooltip("마우스 뗐을 때 호출(0~1 차지 값)")]
    public UnityEvent<float> onReleaseCharge01;

    public float Charge01 => charge01;
    public bool IsCharging => isCharging;

    private bool isCharging;
    private float t;          // 시간 누적
    private float charge01;   // 0~1

    public void ChargeStart()
    {
        StartCharge();
    }

    private void Update()
    {
        if (isCharging)
        {
            UpdateCharge();
        }
    }

    public void ChargeRelease()
    {
        ReleaseCharge();
    }

    private void StartCharge()
    {
        isCharging = true;
        t = 0f; // 매번 0에서 시작(원하면 유지하게 바꿀 수 있음)
        charge01 = minCharge01;
        ApplyUI(charge01);
    }

    private void UpdateCharge()
    {
        t += Time.deltaTime * pingPongSpeed;

        // 0→1→0→1… (핑퐁)
        float raw = Mathf.PingPong(t, 1f);

        // min~max 구간으로 제한
        float ranged = Mathf.Lerp(minCharge01, maxCharge01, raw);

        // 커브로 느낌 조절(입력은 0~1, 출력도 0~1이라 가정)

        int randValue = Random.Range(0, chargingDatas.Length);
        AnimationCurve chargeCurve = chargingDatas[randValue].chargeCurve;
        float curved = chargeCurve != null ? chargeCurve.Evaluate(raw) : raw;
        charge01 = Mathf.Lerp(minCharge01, maxCharge01, curved);

        ApplyUI(charge01);
    }

    private void ReleaseCharge()
    {
        isCharging = false;

        // 마지막 UI 반영
        ApplyUI(charge01);

        // 이벤트 발사(로프 발사 연결)
        onReleaseCharge01?.Invoke(charge01);
    }

    private void ApplyUI(float value01)
    {
        if (chargeFillImage != null)
            chargeFillImage.fillAmount = value01;

        int val100 = Mathf.RoundToInt(value01 * 100f);

        if (chargeTMP != null)
            chargeTMP.text = val100.ToString();
    }
}
