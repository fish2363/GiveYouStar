using UnityEngine;
using UnityEngine.Events;

namespace Assets._01.Develop.CDH.Code.Core
{
    public class SelectHintManager : MonoBehaviour
    {

        [SerializeField] private LayerMask pickMask;

        private bool isSelectHint;

        private void Awake()
        {
            isSelectHint = false;
        }

        private void Update()
        {
            if(isSelectHint)
            {
                GetMouseSeleted();
            }
        }

        private void GetMouseSeleted()
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 점(마우스 위치)에 겹치는 콜라이더 1개 찾기
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld, pickMask);

            if (hit != null)
            {
                GameObject clicked = hit.gameObject;

                // 예) 컴포넌트 가져오기
                // var comp = clicked.GetComponent<YourComponent>();
            }
        }

        public void SelectHintStart()
        {
            isSelectHint = true;
        }
    }
}
