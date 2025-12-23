using _01.Develop.LSW._01._Scripts.So;
using Ami.BroAudio;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class StarMover : MonoBehaviour
{
    [Header("SOund")]
    [SerializeField] private SoundID starSoundID;

    [Header("Move")]
    [SerializeField] private float speed = 2f;
    private Vector3 moveDirection;

    [Header("State")]
    public bool isCatch;
    public Action OnDestroy;

    [Header("VFX")]
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ParticleSystem Conflict;

    public StarSo MyInfo { get; private set; }

    // =========================
    // Spawn Pop (Scale) Anim
    // =========================
    [Header("Spawn Pop")]
    [SerializeField] private bool playSpawnPop = true;
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private float overshoot = 1.15f;
    private Vector3 _originScale;
    private Coroutine _popCo;

    // =========================
    // Random Spin Mode
    // =========================
    [Header("Random Spin Mode")]
    [SerializeField, Range(0f, 1f)] private float spinChance = 0.5f; // 회전할 확률(0~1)
    [SerializeField] private float minSpinDegPerSec = 60f;          // 회전할 때 최소 속도(도/초)
    [SerializeField] private float maxSpinDegPerSec = 180f;         // 회전할 때 최대 속도(도/초)

    private bool _doSpin;
    private float _spinSpeedDegPerSec;

    private void OnEnable()
    {
        // 원래 스케일 저장
        _originScale = transform.localScale;

        // 스폰 팝 애니메이션
        if (playSpawnPop)
        {
            transform.localScale = Vector3.zero;
            if (_popCo != null) StopCoroutine(_popCo);
            _popCo = StartCoroutine(SpawnPopRoutine(_originScale));
        }

        _doSpin = UnityEngine.Random.value < spinChance;

        if (_doSpin)
        {
            _spinSpeedDegPerSec = UnityEngine.Random.Range(minSpinDegPerSec, maxSpinDegPerSec);

            // 시계/반시계 랜덤
            if (UnityEngine.Random.value < 0.5f)
                _spinSpeedDegPerSec *= -1f;
        }
        else
        {
            _spinSpeedDegPerSec = 0f;
        }
    }

    public void Initialize(StarSo star)
    {
        MyInfo = star;
        speed = star.speed;
    }

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
    }

    private void Update()
    {
        if (isCatch) return;

        transform.position += moveDirection * speed * Time.deltaTime;

        if (_doSpin && Mathf.Abs(_spinSpeedDegPerSec) > 0.001f)
        {
            transform.Rotate(0f, 0f, _spinSpeedDegPerSec * Time.deltaTime);
        }
    }

    public void SetStop(bool isStop)
    {
        if (isStop)
            particle?.Play();

        var main = Camera.main;
        if (main != null)
        {
            var impulse = main.GetComponent<CinemachineImpulseSource>();
            if (impulse != null) impulse.GenerateImpulse();
        }

        isCatch = isStop;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Star") && isCatch)
        {
            BroAudio.Play(starSoundID);
            Conflict?.Play();
            OnDestroy?.Invoke();
        }
    }

    private IEnumerator SpawnPopRoutine(Vector3 targetScale)
    {
        // 0 -> overshoot -> target
        float t = 0f;
        float half = Mathf.Max(0.01f, popDuration * 0.6f);
        float rest = Mathf.Max(0.01f, popDuration - half);

        Vector3 overScale = targetScale * overshoot;

        // 1) 0 -> overshoot
        while (t < half)
        {
            t += Time.deltaTime;
            float x = Mathf.Clamp01(t / half);
            float eased = EaseOutBack(x);
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, overScale, eased);
            yield return null;
        }

        // 2) overshoot -> target
        t = 0f;
        while (t < rest)
        {
            t += Time.deltaTime;
            float x = Mathf.Clamp01(t / rest);
            float eased = EaseOutQuad(x);
            transform.localScale = Vector3.LerpUnclamped(overScale, targetScale, eased);
            yield return null;
        }

        transform.localScale = targetScale;
        _popCo = null;
    }

    private static float EaseOutQuad(float x)
    {
        return 1f - (1f - x) * (1f - x);
    }

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
