//using System.Collections.Generic;
//using UnityEngine;

//namespace Assets._01.Develop.CDH.Code.Fasdfags
//{
//    /// <summary>
//    /// Refresh 없이 Trigger 기반으로 별을 추적하는 UI 화살표 매니저
//    /// </summary>
//    [RequireComponent(typeof(CircleCollider2D))]
//    public class FindNearestStar : MonoBehaviour
//    {
//        [SerializeField] private float maxDistance = 30f;
//        [SerializeField] private LayerMask starLayer;

//        private List<ArrowUI> arrowUIs;

//        private void Awake()
//        {
//            arrowUIs = new();
//        }

//        private void Update()
//        {
//            // 타겟이 파괴된 경우 정리
//            if (_active.Count > 0)
//            {
//                // foreach 중 제거 방지용 임시 리스트
//                List<Transform> toRemove = null;

//                foreach (var kv in _active)
//                {
//                    var star = kv.Key;
//                    var arrow = kv.Value;

//                    if (star == null)
//                    {
//                        toRemove ??= new List<Transform>();
//                        toRemove.Add(star);
//                        continue;
//                    }

//                    arrow.Tick();
//                }

//                if (toRemove != null)
//                {
//                    for (int i = 0; i < toRemove.Count; i++)
//                        RemoveStar(toRemove[i]);
//                }
//            }
//        }

//        private void OnTriggerEnter2D(Collider2D other)
//        {
//            if (((1 << other.gameObject.layer) & starLayer) == 0) return;

//            Transform starTrm = other.transform;
//            if (_active.ContainsKey(starTrm)) return;

//            RectTransform rt = GetArrowFromPool();
//            var arrow = new ArrowUI();

//            arrow.Bind(
//                rect: rt,
//                target: starTrm,
//                player: playerTrm,
//                canvasRect: _canvasRect,
//                worldCam: worldCamera,
//                uiCam: _uiCam,
//                uiRadius: arrowUiRadius,
//                angleOffset: angleOffset,
//                minScale: minArrowScale,
//                maxScale: maxArrowScale,
//                minScaleDist: minScaleDistance,
//                maxScaleDist: maxScaleDistance,
//                tweenDuration: tweenDuration
//            );

//            _active.Add(starTrm, arrow);
//        }

//        private void OnTriggerExit2D(Collider2D other)
//        {
//            if (((1 << other.gameObject.layer) & starLayer) == 0) return;
//            RemoveStar(other.transform);
//        }

//        private void RemoveStar(Transform starTrm)
//        {
//            if (starTrm == null) return;
//            if (!_active.TryGetValue(starTrm, out var arrow)) return;

//            arrow.KillTween();
//            arrow.SetActive(false);

//            // 풀로 반환
//            if (arrowPrefab != null && arrowRoot != null)
//            {
//                // ArrowUI가 들고 있는 RectTransform을 풀로 돌리려면 여기서 가져와야 하는데,
//                // ArrowUI.Rect가 private set이라 getter 사용
//                // => arrow가 가진 Rect를 꺼내기 위해 아래처럼 한 줄 추가로 처리합니다.
//            }

//            // ArrowUI 내부 RectTransform을 풀에 넣기
//            // (ArrowUI.Rect getter를 써서 가져옵니다)
//            var rectField = typeof(ArrowUI).GetProperty("Rect");
//            var rt = rectField?.GetValue(arrow) as RectTransform;
//            if (rt != null) _pool.Push(rt);

//            _active.Remove(starTrm);
//        }

//        private RectTransform GetArrowFromPool()
//        {
//            RectTransform rt = null;

//            while (_pool.Count > 0 && rt == null)
//                rt = _pool.Pop(); // 파괴된 오브젝트일 수도 있으니 null 체크

//            if (rt == null)
//            {
//                rt = Instantiate(arrowPrefab, arrowRoot);
//            }
//            else
//            {
//                rt.SetParent(arrowRoot, false);
//            }

//            rt.anchoredPosition = Vector2.zero;
//            rt.localRotation = Quaternion.identity;
//            rt.localScale = Vector3.one;
//            rt.gameObject.SetActive(true);
//            return rt;
//        }
//    }
//}
