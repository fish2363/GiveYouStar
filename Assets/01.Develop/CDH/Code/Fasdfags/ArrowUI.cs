//using UnityEngine;
//using DG.Tweening;

//namespace Assets._01.Develop.CDH.Code.Fasdfags
//{
//    public class ArrowUI : MonoBehaviour
//    {
//        [SerializeField] private float angleOffsetDeg;

//        [Header("about Star")]
//        [SerializeField] private float minDistance = 40f;
//        [SerializeField] private float maxDistance = 100f;
//        [SerializeField] private float minScale = 0f;
//        [SerializeField] private float maxScale = 1f;

//        private Transform targetTrm;
//        private RectTransform myRect;

//        private void Awake()
//        {
//            myRect = GetComponent<RectTransform>();
//        }

//        public void SetTargetTrm(Transform targetTrm)
//            => this.targetTrm = targetTrm;

//        private void Update()
//        {
//            Vector2 dir = targetTrm.position - transform.position;

//            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
//            float zRot = angle - 90f + angleOffsetDeg;              

//            transform.rotation = Quaternion.Euler(0f, 0f, zRot);

//            float distance = Vector3.Distance(targetTrm.position, transform.position);
//            if(distance < minDistance)
//            {
//                transform.localScale = Vector3.one;
//            }
//            else if(distance >= minDistance && distance <= maxDistance)
//            {
//                float dis01 = distance / maxDistance;
//                // EaseOutQubic 사용해서 크기 minScale에서 maxScale로 조절해줘
//            }
//            else if(distance > maxDistance)
//            {
//                transform.localScale = Vector3.zero;
//            }
//        }
//    }
//}
