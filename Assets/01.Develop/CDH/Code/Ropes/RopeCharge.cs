using Assets._01.Develop.CDH.Code.Ropes;
using UnityEngine;
using UnityEngine.Events;

public class RopeCharge : MonoBehaviour
{
    [Header("Charge Settings")]
    [Tooltip("차지 속도(초당 몇 번 왕복하느냐 느낌). 값이 클수록 더 빨리 왔다갔다.")]
    [SerializeField] private float pingPongSpeed = 1.2f;

    [Tooltip("차지 최소/최대(0~1). 예: 0.1~1이면 완전 0은 안 나오게 가능")]
    [Range(0f, 1f)] [SerializeField] private float minCharge01 = 0f;
    [Range(0f, 1f)] [SerializeField] private float maxCharge01 = 1f;

    [Tooltip("차지 곡선(선형이 싫으면 커브로 맛 조절)")]
    [SerializeField] private ChargingDataSO[] chargingDatas;

    [Header("UI (Visual Scale)")]
    [SerializeField] private Transform chargeVisual; // X축 스케일로 표현할 오브젝트

    [Header("Rotation Limit")]
    [Tooltip("마우스를 바라보는 회전 제한 각도 (Z축 기준)")]
    [SerializeField] private float minRotationAngle = -60f;
    [SerializeField] private float maxRotationAngle = 60f;

    [Header("Events")]
    [Tooltip("마우스 뗐을 때 호출(0~1 차지 값)")]
    public UnityEvent<float> onReleaseCharge01;

    public float Charge01 => charge01;
    public bool IsCharging => isCharging;

    private bool isCharging;
    private float t;
    private float charge01;

    public void ChargeStart()
    {
        StartCharge();
    }
    public void ShowVisual(bool isShow)
    {
        chargeVisual.GetComponent<SpriteRenderer>().enabled = isShow;
    }
    public void ChargeRelease()
    {
        ReleaseCharge();
    }

    private void Update()
    {
        if (isCharging)
        {
            UpdateCharge();
        }

        RotateVisualToMouse();
    }

    private void StartCharge()
    {
        isCharging = true;
        t = 0f;
        charge01 = minCharge01;
        ApplyVisual(charge01);
    }

    private void UpdateCharge()
    {
        t += Time.deltaTime * pingPongSpeed;

        float raw = Mathf.PingPong(t, 1f);
        float ranged = Mathf.Lerp(minCharge01, maxCharge01, raw);

        int randValue = Random.Range(0, chargingDatas.Length);
        AnimationCurve chargeCurve = chargingDatas[randValue].chargeCurve;

        float curved = chargeCurve != null ? chargeCurve.Evaluate(raw) : raw;
        charge01 = Mathf.Lerp(minCharge01, maxCharge01, curved);

        ApplyVisual(charge01);
    }

    private void ReleaseCharge()
    {
        isCharging = false;
        ApplyVisual(charge01);
        onReleaseCharge01?.Invoke(charge01);
    }

    private void ApplyVisual(float value01)
    {
        if (chargeVisual != null)
        {
            Vector3 scale = chargeVisual.localScale;
            scale.x = Mathf.Lerp(0f, 25f, value01); // X축만 0~25로 조절
            chargeVisual.localScale = scale;
        }
    }

    private void RotateVisualToMouse()
    {
        if (chargeVisual == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = chargeVisual.position.z;

        Vector3 dir = (mouseWorldPos - chargeVisual.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 제한 각도 적용
        float clampedAngle = Mathf.Clamp(angle, minRotationAngle, maxRotationAngle);
        chargeVisual.rotation = Quaternion.Euler(0f, 0f, clampedAngle);
    }
}
