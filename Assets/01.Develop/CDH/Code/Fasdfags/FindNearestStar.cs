using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Assets._01.Develop.CDH.Code.Fasdfags
{
    public class Arrow
    {
        public GameObject MyGameObj { get; set; }
        public Transform TargetTrm { get; set; }
        public Transform OriginTrm { get; set; }   // 플레이어
        public float Radius { get; set; }          // arrowWorldRadius

        public float angleOffset = 0f;

        public void Update()
        {
            if (MyGameObj.transform == null || TargetTrm == null || OriginTrm == null) return;

            // 플레이어 -> 타겟 방향
            Vector2 dir = (Vector2)(TargetTrm.position - OriginTrm.position);
            if (dir.sqrMagnitude < 0.0001f) return;

            Vector2 dirN = dir.normalized;

            MyGameObj.transform.position = OriginTrm.position + (Vector3)(dirN * Radius);

            float angleX = Mathf.Atan2(dirN.y, dirN.x) * Mathf.Rad2Deg; // X축 기준
            float angleY = angleX - 90f;                                // Y축 기준으로 변환

            Debug.Log(angleY);
            MyGameObj.transform.rotation = Quaternion.Euler(0f, 0f, angleY + angleOffset);
        }
    }

    public class FindNearestStar : MonoBehaviour
    {
        [Header("Find Settings")]
        [SerializeField] private float maxDistance = 30f;
        [SerializeField] private int maxFindedStar = 3;
        [SerializeField] private LayerMask starLayer;
        [SerializeField] private float refreshInterval = 0.2f;

        [Header("World Arrow")]
        [SerializeField] private Transform playerTrm;
        [SerializeField] private GameObject arrowPrefab;     // SpriteRenderer 달린 화살표 프리팹
        [SerializeField] private Transform arrowRoot;       // (선택) 화살표 정리용 부모
        [SerializeField] private float arrowWorldRadius = 2f; // 플레이어 중심에서 화살표가 떠 있을 반경

        private List<Arrow> arrows;
        private Collider2D[] stars;

        private float timer;

        private void Awake()
        {
            arrows = new();
            stars = new Collider2D[maxFindedStar];
            timer = 0f;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > refreshInterval)
            {
                timer = 0f;
                Refresh();
            }

            foreach (var arrow in arrows)
                arrow.Update();
        }

        private void Refresh()
        {
            stars = Physics2D.OverlapCircleAll(playerTrm.position, maxDistance, starLayer);

            foreach (var arrow in arrows)
                Destroy(arrow.MyGameObj);
            arrows.Clear();

            int count = Mathf.Min(maxFindedStar, stars.Length);
            for (int i = 0; i < count; i++)
            {
                Collider2D star = stars[i];

                GameObject go = Instantiate(arrowPrefab, playerTrm.position, Quaternion.identity, arrowRoot);

                Arrow arrow = new Arrow
                {
                    MyGameObj = go,
                    TargetTrm = star.transform, 
                    OriginTrm = playerTrm,      
                    Radius = arrowWorldRadius,  
                    angleOffset = 0f
                };

                arrows.Add(arrow);
            }
        }
    }
}
