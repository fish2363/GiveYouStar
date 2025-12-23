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
    [SerializeField, Range(0f, 1f)] private float spinChance = 0.5f;
    [SerializeField] private float minSpinDegPerSec = 60f;
    [SerializeField] private float maxSpinDegPerSec = 180f;

    private bool _doSpin;
    private float _spinSpeedDegPerSec;

    // =========================
    // Repel Nearby Stars
    // =========================
    [Header("Repel Nearby Stars (on Catch)")]
    [SerializeField] private bool repelOnCatch = true;
    [SerializeField] private float repelRadius = 2.5f;
    [SerializeField] private float repelSpeedMultiplier = 2.0f;  // 밀쳐낼 때 속도 배수
    [SerializeField] private float repelDuration = 0.35f;        // 밀쳐진 상태 유지 시간
    [SerializeField] private LayerMask starLayerMask;            // Star 레이어로 설정 권장
    [SerializeField] private bool useTagCheck = true;            // 레이어 세팅 귀찮으면 태그로 필터

    private static readonly Collider2D[] _overlap = new Collider2D[32];
    private bool _repelTriggered;

    private void OnEnable()
    {
        _repelTriggered = false;

        _originScale = transform.localScale;

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
            if (UnityEngine.Random.value < 0.5f) _spinSpeedDegPerSec *= -1f;
        }
        else _spinSpeedDegPerSec = 0f;
    }

    public void Initialize(StarSo star)
    {
        MyInfo = star;
        speed = star.speed;
    }

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector3.right;
    }

    private void Update()
    {
        if (isCatch) return;

        transform.position += moveDirection * speed * Time.deltaTime;

        if (_doSpin && Mathf.Abs(_spinSpeedDegPerSec) > 0.001f)
            transform.Rotate(0f, 0f, _spinSpeedDegPerSec * Time.deltaTime);
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

        // 잡히는 순간 주변 별 밀쳐내기 (1회만)
        if (isStop && repelOnCatch && !_repelTriggered)
        {
            _repelTriggered = true;
            RepelNearbyStars();
        }

        isCatch = isStop;
    }

    private void RepelNearbyStars()
    {
        Vector2 center = transform.position;

        int count = Physics2D.OverlapCircleNonAlloc(center, repelRadius, _overlap, starLayerMask);
        for (int i = 0; i < count; i++)
        {
            var col = _overlap[i];
            if (col == null) continue;

            var other = col.GetComponent<StarMover>();
            if (other == null) continue;
            if (other == this) continue;
            if (other.isCatch) continue; // 이미 잡힌 애는 건드리지 않기

            if (useTagCheck && !col.CompareTag("Star"))
                continue;

            Vector3 dir = (other.transform.position - transform.position);
            if (dir.sqrMagnitude < 0.0001f)
                dir = UnityEngine.Random.insideUnitCircle.normalized;

            other.PushAway(dir, speed * repelSpeedMultiplier, repelDuration);
        }

        // 배열 클린(안 해도 되지만 깔끔하게)
        for (int i = 0; i < count; i++) _overlap[i] = null;
    }

    // "밀쳐내기" 동작: 일정 시간 빠르게 날아가게 했다가 원래 speed로 복구
    public void PushAway(Vector3 dir, float pushSpeed, float duration)
    {
        StartCoroutine(PushAwayRoutine(dir, pushSpeed, duration));
    }

    private IEnumerator PushAwayRoutine(Vector3 dir, float pushSpeed, float duration)
    {
        float prevSpeed = speed;
        Vector3 prevDir = moveDirection;

        SetMoveDirection(dir);
        speed = pushSpeed;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // 원래 상태로 복구
        speed = prevSpeed;
        moveDirection = prevDir;
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
        float t = 0f;
        float half = Mathf.Max(0.01f, popDuration * 0.6f);
        float rest = Mathf.Max(0.01f, popDuration - half);

        Vector3 overScale = targetScale * overshoot;

        while (t < half)
        {
            t += Time.deltaTime;
            float x = Mathf.Clamp01(t / half);
            float eased = EaseOutBack(x);
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, overScale, eased);
            yield return null;
        }

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

    private static float EaseOutQuad(float x) => 1f - (1f - x) * (1f - x);

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, repelRadius);
    }
#endif
}