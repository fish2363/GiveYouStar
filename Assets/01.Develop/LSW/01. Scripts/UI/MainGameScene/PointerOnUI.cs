using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class PointerOnUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Hover Scale")]
        [SerializeField] private float hoverScaleMultiplier = 1.08f; // 1.08 = 8% 커짐
        [SerializeField] private float speed = 14f;                  // 클수록 빠름
        [SerializeField] private bool useUnscaledTime = true;        // 일시정지에도 자연스럽게

        private Vector3 baseScale;
        private Vector3 targetScale;

        private void Awake()
        {
            baseScale = transform.localScale;
            targetScale = baseScale;
        }

        private void OnEnable()
        {
            // 혹시 켜질 때 스케일이 외부에서 바뀌는 구조면 여기서 다시 잡아도 됨
            baseScale = transform.localScale;
            targetScale = baseScale;
        }

        private void Update()
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 1f - Mathf.Exp(-speed * dt));
        }

        public void OnPointerEnter(PointerEventData _)
        {
            targetScale = baseScale * hoverScaleMultiplier;
        }

        public void OnPointerExit(PointerEventData _)
        {
            targetScale = baseScale;
        }

        // (선택) 레이아웃/애니메이션 등으로 baseScale이 바뀌는 경우 외부에서 갱신용
        public void RefreshBaseScale()
        {
            baseScale = transform.localScale;
            targetScale = baseScale;
        }
    }
}
