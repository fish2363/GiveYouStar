using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

public class MainMenuLogic : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text pressAnyKeyText;

    [Header("Black Overlay (CanvasGroup)")]
    [SerializeField] private CanvasGroup blackOverlay; // 전체화면 검은 Image에 CanvasGroup 붙여서 연결

    [Header("Blink (Idle)")]
    [SerializeField] private float blinkFadeDuration = 0.6f;
    [SerializeField] private float blinkMinAlpha = 0f;
    [SerializeField] private float blinkMaxAlpha = 1f;

    [Header("On Input Transition")]
    [SerializeField] private float textFadeOutDuration = 0.25f;
    [SerializeField] private float blackFadeInDuration = 0.35f;

    [Header("Input")]
    [SerializeField] private bool allowMouseClick = true;
    [SerializeField] private bool allowSpaceKey = true;

    [Header("Events")]
    public UnityEvent OnTransitionComplete;

    private Tween _blinkTween;
    private Sequence _sequence;
    private bool _started;

    private void Start()
    {
        if (pressAnyKeyText == null)
        {
            Debug.LogError("[MainMenuLogic] pressAnyKeyText가 비어있음!");
            enabled = false;
            return;
        }

        // 초기 상태
        SetTextAlpha(blinkMaxAlpha);

        if (blackOverlay != null)
        {
            blackOverlay.alpha = 0f;
            blackOverlay.blocksRaycasts = true;
            blackOverlay.interactable = false;
        }

        StartBlink();
    }

    private void Update()
    {
        if (_started) return;

        bool input = false;
        if (allowSpaceKey && Input.GetKeyDown(KeyCode.Space)) input = true;
        if (allowMouseClick && Input.GetMouseButtonDown(0)) input = true;

        if (input)
            PlayTransition();
    }

    private void StartBlink()
    {
        _blinkTween?.Kill();

        // 1 <-> 0 무한 깜빡임
        _blinkTween = pressAnyKeyText
            .DOFade(blinkMinAlpha, blinkFadeDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void PlayTransition()
    {
        _started = true;

        _blinkTween?.Kill();
        _blinkTween = null;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        // 텍스트 페이드아웃
        _sequence.Append(pressAnyKeyText.DOFade(0f, textFadeOutDuration).SetEase(Ease.OutQuad));

        // 검은 화면 페이드인(동시에 시작)
        if (blackOverlay != null)
            _sequence.Join(blackOverlay.DOFade(1f, blackFadeInDuration).SetEase(Ease.OutQuad));
        else
            _sequence.AppendInterval(blackFadeInDuration);

        // 완료 이벤트
        _sequence.OnComplete(() =>
        {
            OnTransitionComplete?.Invoke();
        });
    }

    private void SetTextAlpha(float a)
    {
        var c = pressAnyKeyText.color;
        c.a = a;
        pressAnyKeyText.color = c;
    }

    private void OnDisable()
    {
        _blinkTween?.Kill();
        _sequence?.Kill();
    }
}
