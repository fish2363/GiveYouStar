using TMPro;
using UnityEngine;
using DG.Tweening;

public class TMPBlinkFade : MonoBehaviour
{
    [Header("Alpha")]
    [Range(0f, 1f)] public float minAlpha = 0f;
    [Range(0f, 1f)] public float maxAlpha = 1f;

    [Header("Timing")]
    public float fadeOutDuration = 0.35f;
    public float fadeInDuration = 0.35f;
    public float delayBetween = 0.05f; // °¢ ÆäÀÌµå »çÀÌ Àá±ñ ¸ØÃã

    [Header("Options")]
    public bool playOnEnable = true;
    public bool ignoreTimeScale = true;

    [SerializeField] private CanvasGroup cg;
    private Tween blinkTween;


    private void OnEnable()
    {
        if (playOnEnable) Play();
    }

    private void OnDisable()
    {
        Stop();
    }

    public void Play()
    {
        Stop();

        cg.alpha = maxAlpha;

        // ÆäÀÌµå 2°³ (Out -> In) ¸¦ ¹«ÇÑ ¹Ýº¹
        blinkTween = DOTween.Sequence()
            .Append(cg.DOFade(minAlpha, fadeOutDuration))
            .AppendInterval(delayBetween)
            .Append(cg.DOFade(maxAlpha, fadeInDuration))
            .AppendInterval(delayBetween)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(ignoreTimeScale) // true¸é TimeScale=0¿¡¼­µµ ±ôºý
            .Play();
    }

    public void Stop(bool resetAlpha = true)
    {
        if (blinkTween != null && blinkTween.IsActive())
            blinkTween.Kill();

        blinkTween = null;

        if (cg != null && resetAlpha)
            cg.alpha = maxAlpha;
    }
}