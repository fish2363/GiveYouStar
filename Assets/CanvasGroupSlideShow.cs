using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasGroupSlideShow : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CanvasGroup[] slides;

    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private bool useUnscaledTime = true;

    private int index = -1;
    private bool playing;
    private bool transitioning;
    private Action onFinished;
    private Coroutine co;

    private void Awake()
    {
        HideAllImmediate();
    }

    private void OnDisable()
    {
        if (co != null) StopCoroutine(co);
        co = null;
        playing = false;
        transitioning = false;
    }

    public void Begin(Action finishedCallback = null)
    {
        onFinished = finishedCallback;

        HideAllImmediate();
        index = -1;
        playing = true;

        Next(); // Ã¹ Àå
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!playing) return;
        Next();
    }

    private void Update()
    {
        if (!playing) return;
        if (Input.GetMouseButtonDown(0)) Next();
    }

    private void Next()
    {
        if (!playing || transitioning) return;

        int nextIndex = index + 1;

        if (slides == null || slides.Length == 0 || nextIndex >= slides.Length)
        {
            playing = false;
            HideAllImmediate();
            onFinished?.Invoke();
            return;
        }

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(CoSwitch(index, nextIndex));
        index = nextIndex;
    }

    private IEnumerator CoSwitch(int from, int to)
    {
        transitioning = true;

        if (from >= 0 && from < slides.Length)
            yield return CoFade(slides[from], 0f);

        ShowSlideImmediate(to);
        yield return CoFade(slides[to], 1f);

        transitioning = false;
    }

    private IEnumerator CoFade(CanvasGroup cg, float target)
    {
        if (cg == null) yield break;

        float start = cg.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;
            float p = fadeDuration <= 0f ? 1f : Mathf.Clamp01(t / fadeDuration);
            cg.alpha = Mathf.Lerp(start, target, p);
            yield return null;
        }

        cg.alpha = target;

        bool on = target >= 0.999f;
        cg.interactable = on;
        cg.blocksRaycasts = on;
    }

    private void HideAllImmediate()
    {
        if (slides == null) return;

        for (int i = 0; i < slides.Length; i++)
        {
            if (slides[i] == null) continue;
            slides[i].alpha = 0f;
            slides[i].interactable = false;
            slides[i].blocksRaycasts = false;
        }
    }

    private void ShowSlideImmediate(int i)
    {
        if (slides == null || i < 0 || i >= slides.Length) return;

        for (int k = 0; k < slides.Length; k++)
        {
            if (slides[k] == null) continue;
            slides[k].alpha = 0f;
            slides[k].interactable = false;
            slides[k].blocksRaycasts = false;
        }

        if (slides[i] != null)
        {
            slides[i].alpha = 0f;
            slides[i].interactable = false;
            slides[i].blocksRaycasts = false;
        }
    }
}
