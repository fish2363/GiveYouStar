using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class BookUIAnimation : MonoBehaviour
    {
        [SerializeField] private Transform hideBookTrm;
        [SerializeField] private Transform showBookTrm;
        
        public UnityEvent onBookShowAnimationEnd;
        public UnityEvent onBookHideAnimationEnd;
        
        public void Show()
        {
            transform.DOMove(showBookTrm.position, 0.5f)
                .OnComplete(() =>
                {
                    onBookShowAnimationEnd?.Invoke();
                });
        }
        
        public void Hide()
        {
            transform.DOMove(hideBookTrm.position, 0.5f)
                .OnComplete(() =>
                {
                    onBookHideAnimationEnd?.Invoke();
                });
        }
    }
}