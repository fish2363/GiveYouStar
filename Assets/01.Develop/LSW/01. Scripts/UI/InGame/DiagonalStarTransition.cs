using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01.Develop.LSW._01._Scripts.UI.InGame
{
    public class DiagonalStarTransition : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform transitionRoot;
        [SerializeField] private RectTransform starContainer;
        [SerializeField] private DiagonalFallingStar starPrefab;

        [Header("Settings")]
        [SerializeField] private float starSize = 128f;                 // 별 기준 크기
        [SerializeField] private float spacingMultiplier = 0.7f;        // 격자 간격 배율 (낮을수록 겹침 많음)
        [SerializeField] private Vector2 sizeScaleRange = new(0.95f, 1.15f); // 너무 작으면 구멍 생김
        [SerializeField] private float endPositionJitter = 0f;          // ✅ 도착 위치 지터(구멍 방지) - 웬만하면 0 추천
        [SerializeField] private float startPositionJitter = 30f;       // 시작 위치만 랜덤(자연스러움)
        [SerializeField] private Vector2 durationRange = new(0.8f, 1.2f);

        [Header("Sequential Fill (BottomRight -> TopLeft)")]
        [SerializeField] private float totalStartStaggerTime = 1.2f;    // ✅ 첫 별 시작~마지막 별 시작까지 시간(순차감 강해짐)
        [SerializeField] private float afterActivateHold = 0.08f;

        private readonly List<DiagonalFallingStar> stars = new();
        private readonly List<Tween> starTweens = new();

        private int totalStars;
        private int completedStars;

        private struct SpawnInfo
        {
            public int c, r;
            public Vector2 startPos;
            public Vector2 endPos;
        }

        [ContextMenu("Play Transition")]
        public void Play() => Play("MainGameScene");

        public void Play(string nextScene)
        {
            transitionRoot.gameObject.SetActive(true);
            transitionRoot.SetAsLastSibling();

            Canvas.ForceUpdateCanvases();

            SpawnStarsSequential();
            StartCoroutine(TransitionSequence(nextScene));
        }

        private void SpawnStarsSequential()
        {
            ClearStars();
            starTweens.Clear();
            completedStars = 0;

            float screenW = starContainer.rect.width;
            float screenH = starContainer.rect.height;
            float diagonal = Mathf.Sqrt(screenW * screenW + screenH * screenH);

            float step = starSize * spacingMultiplier;

            int cols = Mathf.CeilToInt(diagonal / step) + 4;
            int rows = Mathf.CeilToInt(diagonal / step) + 4;

            totalStars = cols * rows;

            Vector2 gridOrigin = new Vector2(-(cols - 1) * step * 0.5f, (rows - 1) * step * 0.5f);

            // ✅ 왼쪽 위(화면 밖)에서 날아오게
            Vector2 startOffset = new Vector2(-diagonal * 1.25f, diagonal * 1.25f);

            // 1) 모든 스폰 데이터를 만든 뒤
            List<SpawnInfo> infos = new List<SpawnInfo>(totalStars);

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    // 도착 위치(격자). ✅ 여기 지터를 거의/아예 빼야 구멍이 줄어듦
                    float targetX = gridOrigin.x + (c * step) + Random.Range(-endPositionJitter, endPositionJitter);
                    float targetY = gridOrigin.y - (r * step) + Random.Range(-endPositionJitter, endPositionJitter);
                    Vector2 endPos = new Vector2(targetX, targetY);

                    // 시작 위치는 endPos 기준 왼쪽 위로 + 시작 지터
                    Vector2 randomizedOffset = startOffset + new Vector2(
                        Random.Range(-startPositionJitter, startPositionJitter),
                        Random.Range(-startPositionJitter, startPositionJitter)
                    );
                    Vector2 startPos = endPos + randomizedOffset;

                    infos.Add(new SpawnInfo { c = c, r = r, startPos = startPos, endPos = endPos });
                }
            }

            // 2) ✅ 오른쪽 아래부터 “별 하나씩” 순차 정렬
            // 우선순위:
            // (dx+dy) 작은게 먼저 (BR 근처 먼저)
            // 같은 단계면 더 오른쪽(dx 작은) 먼저, 그 다음 더 아래(dy 작은) 먼저
            infos.Sort((a, b) =>
            {
                int dxA = (cols - 1) - a.c;
                int dyA = (rows - 1) - a.r;
                int stepA = dxA + dyA;

                int dxB = (cols - 1) - b.c;
                int dyB = (rows - 1) - b.r;
                int stepB = dxB + dyB;

                int cmp = stepA.CompareTo(stepB);
                if (cmp != 0) return cmp;

                cmp = dxA.CompareTo(dxB);
                if (cmp != 0) return cmp;

                return dyA.CompareTo(dyB);
            });

            // 3) ✅ delay를 index 기반으로 뿌려서 "진짜 순차"로 만들기
            float perStarDelay = (totalStars <= 1) ? 0f : (totalStartStaggerTime / (totalStars - 1));

            for (int i = 0; i < infos.Count; i++)
            {
                var info = infos[i];

                var star = Instantiate(starPrefab, starContainer);
                stars.Add(star);

                RectTransform rt = star.rect;
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);

                // 크기 랜덤 (너무 작으면 구멍 → 범위 너무 낮게 잡지 마)
                float randomScale = Random.Range(sizeScaleRange.x, sizeScaleRange.y);
                rt.sizeDelta = new Vector2(starSize * randomScale, starSize * randomScale);

                float duration = Random.Range(durationRange.x, durationRange.y);
                float delay = i * perStarDelay;

                Tween tween = star.Fall(info.startPos, info.endPos, duration, delay);

                // 도착했을 때 아주 살짝 '꽉 차는 느낌' (구멍 체감 줄어듦)
                tween.OnComplete(() =>
                {
                    completedStars++;
                    if (rt != null) rt.DOPunchScale(Vector3.one * 0.06f, 0.12f, 1, 0.6f);
                });

                starTweens.Add(tween);
            }
        }

        private IEnumerator TransitionSequence(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            if (op != null) op.allowSceneActivation = false;

            // ✅ 화면이 꽉 찰 때까지(모든 별 도착)
            while (completedStars < totalStars)
                yield return null;

            if (op != null)
            {
                while (op.progress < 0.9f) yield return null;
                op.allowSceneActivation = true;
            }

            yield return new WaitForSeconds(afterActivateHold);

            ClearStars();
            starTweens.Clear();
            transitionRoot.gameObject.SetActive(false);
        }

        private void ClearStars()
        {
            foreach (var t in starTweens)
            {
                if (t != null && t.IsActive()) t.Kill(false);
            }

            foreach (var s in stars)
            {
                if (s != null) Destroy(s.gameObject);
            }

            stars.Clear();
        }
    }
}
