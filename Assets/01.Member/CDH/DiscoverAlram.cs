using _01.Develop.LSW._01._Scripts.So;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._01.Member.CDH.Code.Synergies
{
    public class DiscoverAlram : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private float downDuration;
        [SerializeField] private float waitingDuration;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration;

        [Header("TestVariables")]
        [SerializeField] private StarSo testStarSo;

        [ContextMenu("test")]
        private void Test()
        {
            Discover(testStarSo);
        }

        private Vector2 startPos;
        private Vector2 endPos;
        private RectTransform rectTrm;
        private Awaitable curAwaitable;

        private void Awake()
        {
            rectTrm = transform as RectTransform;
            startPos = new Vector2(-50f, 150);
            endPos = new Vector2(-50f, -25f);
        }

        public async void Discover(StarSo starSo)
        {
            if (curAwaitable != null)
                curAwaitable.Cancel();

            rectTrm.DOKill();
            canvasGroup.DOKill();

            canvasGroup.alpha = 1.0f;

            iconImage.sprite = starSo.starImage;
            nameText.SetText(starSo.starName);
            descriptionText.SetText(starSo.description);

            rectTrm.localScale = Vector3.one;
            rectTrm.anchoredPosition = startPos;

            rectTrm.DOAnchorPos(endPos, downDuration)
                   .SetEase(Ease.OutQuad);

            try
            {
                await (curAwaitable = Awaitable.WaitForSecondsAsync(
                    downDuration + waitingDuration,
                    destroyCancellationToken
                ));
            }
            catch (OperationCanceledException)
            {
                // 이전 호출이 취소되거나 오브젝트가 파괴될 때 정상적으로 뜨는 예외 → 무시
                return;
            }
            finally
            {
                curAwaitable = null;
            }

            // UI면 position 말고 anchoredPosition으로 되돌리는 게 맞습니다
            canvasGroup.DOFade(0, fadeDuration)
                .OnComplete(() => rectTrm.anchoredPosition = startPos);
        }
    }
}
