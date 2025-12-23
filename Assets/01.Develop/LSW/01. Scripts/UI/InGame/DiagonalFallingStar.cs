using DG.Tweening;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.UI.InGame
{
    public class DiagonalFallingStar : MonoBehaviour
    {
        public RectTransform rect;

        public Tween Fall(Vector2 start, Vector2 end, float duration, float delay)
        {
            // 트위닝 전 즉시 시작 위치로 이동
            rect.anchoredPosition = start;

            // 랜덤 회전 추가 (0 ~ 360도)
            rect.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));

            return rect.DOAnchorPos(end, duration)
                .SetDelay(delay)
                .SetEase(Ease.Linear);
        }
    }
}