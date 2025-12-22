using DG.Tweening;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.UI.InGame
{
    public class DiagonalFallingStar : MonoBehaviour
    {
        public RectTransform rect;

        public void Fall(Vector2 start, Vector2 end, float duration, float delay)
        {
            // 트위닝 전 즉시 시작 위치로 이동
            rect.anchoredPosition = start;

            rect.DOAnchorPos(end, duration)
                .SetDelay(delay)
                .SetEase(Ease.Linear);
        }
    }
}