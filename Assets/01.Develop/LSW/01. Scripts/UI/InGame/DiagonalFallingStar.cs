using DG.Tweening;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.UI.InGame
{
    public class DiagonalFallingStar : MonoBehaviour
    {
        public RectTransform rect;

        public Tween Fall(Vector2 start, Vector2 end, float duration, float delay)
        {
            rect.anchoredPosition = start;
            rect.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));

            return rect.DOAnchorPos(end, duration)
                .SetDelay(delay)
                .SetEase(Ease.Linear);
        }
    }
}
