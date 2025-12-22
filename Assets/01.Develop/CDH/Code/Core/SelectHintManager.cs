using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Assets._01.Develop.CDH.Code.Core
{
    public class SelectHintManager : MonoBehaviour
    {
        [SerializeField] private LayerMask pickMask;
        [SerializeField] private float pickRadius = 0.5f;      // 마우스 근처 탐색 반경(월드 단위)
        [SerializeField] private int maxHits = 16;             // 한 번에 잡을 최대 콜라이더 수

        private Collider2D[] _hits;
        private bool isSelectHint;

        private void Awake()
        {
            isSelectHint = false;
            _hits = new Collider2D[maxHits];
        }

        private void Update()
        {
            if(isSelectHint && Mouse.current.leftButton.wasPressedThisFrame)
            {
                GetMouseSeleted();
            }
        }
        private void GetMouseSeleted()
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);

            // 반경 안에 있는 콜라이더들 가져오기 (NonAlloc이라 GC 적음)
            _hits = Physics2D.OverlapCircleAll(mouseWorld, pickRadius, pickMask);
            int count = _hits.Length;

            Collider2D best = null;
            float bestSqrDist = float.PositiveInfinity;

            for (int i = 0; i < count; i++)
            {
                var col = _hits[i];
                if (col == null) continue;

                // 콜라이더에서 마우스에 가장 가까운 점(콜라이더 표면 포함)
                Vector2 closest = col.ClosestPoint(mouseWorld);

                float sqrDist = (closest - mouseWorld).sqrMagnitude;
                if (sqrDist < bestSqrDist)
                {
                    bestSqrDist = sqrDist;
                    best = col;
                }
            }

            if (best != null)
            {
                GameObject clicked = best.gameObject;
                Debug.Log($"Picked: {clicked.name} dist={Mathf.Sqrt(bestSqrDist)}");

                // 원하는 컴포넌트
                // var comp = clicked.GetComponent<YourComponent>();
            }

            // 다음 프레임을 위해 배열 정리(선택: count만큼만 null)
            for (int i = 0; i < count; i++) _hits[i] = null;
        }

        public void SelectHintStart()
        {
            isSelectHint = true;
        }
    }
}
