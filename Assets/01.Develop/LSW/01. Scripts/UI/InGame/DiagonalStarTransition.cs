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
        [SerializeField] private float starSize = 128f; // 격자 배치 및 별 크기 기준
        [SerializeField] private float spacingMultiplier = 0.7f; // 격자 간격 배율 (낮을수록 더 많이 겹침)
        [SerializeField] private Vector2 sizeScaleRange = new(0.8f, 1.2f); // 크기 배율 랜덤 범위
        [SerializeField] private float positionJitter = 40f; // 위치 랜덤 오프셋 강도
        [SerializeField] private Vector2 durationRange = new(0.8f, 1.2f);
        [SerializeField] private Vector2 delayRange = new(0f, 0.4f);
        
        private List<DiagonalFallingStar> stars = new();
        private List<Tween> starTweens = new();

        [ContextMenu("Play Transition")]
        public void Play() => Play("MainGameScene");

        public void Play(string nextScene)
        {
            transitionRoot.gameObject.SetActive(true);
            transitionRoot.SetAsLastSibling();

            // 캔버스 크기 즉시 갱신
            Canvas.ForceUpdateCanvases();

            SpawnStars();
            StartCoroutine(TransitionSequence(nextScene));
        }

        private void SpawnStars()
        {
            ClearStars();
            starTweens.Clear();

            // 1. 컨테이너 실제 크기 확보
            float screenW = starContainer.rect.width;
            float screenH = starContainer.rect.height;

            // 2. 대각선 이동 시 화면 빈틈을 막기 위해 범위를 충분히 확보
            // 화면 크기를 대각선으로 가로지르는 길이를 기준으로 격자를 생성
            float diagonal = Mathf.Sqrt(screenW * screenW + screenH * screenH);
            
            // 실제 배치 간격
            float step = starSize * spacingMultiplier;

            // 격자 크기를 화면 대각선보다 넉넉하게 잡음
            int cols = Mathf.CeilToInt(diagonal / step) + 4;
            int rows = Mathf.CeilToInt(diagonal / step) + 4;

            // 격자의 시작 위치 (도착 지점 기준)
            // 화면 전체를 덮도록 중앙 정렬된 격자 계산
            Vector2 gridOrigin = new Vector2(-(cols - 1) * step * 0.5f, (rows - 1) * step * 0.5f);

            // moveVector: 오른쪽 아래 방향 (45도 대각선)
            // 별이 화면 밖에서 안으로 들어오기 위한 오프셋
            Vector2 moveVector = new Vector2(diagonal * 1.2f, -diagonal * 1.2f);

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    var star = Instantiate(starPrefab, starContainer);
                    stars.Add(star);

                    RectTransform rt = star.rect;
                    
                    rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                    
                    // 크기 랜덤화 (기준 크기 starSize 사용)
                    float randomScale = Random.Range(sizeScaleRange.x, sizeScaleRange.y);
                    rt.sizeDelta = new Vector2(starSize * randomScale, starSize * randomScale);

                    // 격자 배치 (step 간격으로 배치하여 겹침 유도)
                    float targetX = gridOrigin.x + (c * step) + Random.Range(-positionJitter, positionJitter);
                    float targetY = gridOrigin.y - (r * step) + Random.Range(-positionJitter, positionJitter);
                    
                    Vector2 endPos = new Vector2(targetX, targetY);
                    
                    // 이동 벡터에 약간의 무작위성 추가 (각도 및 거리 미세 조정)
                    Vector2 randomizedMoveVector = moveVector + new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
                    Vector2 startPos = endPos - randomizedMoveVector;

                    var tween = star.Fall(startPos, endPos, 
                             Random.Range(durationRange.x, durationRange.y), 
                             Random.Range(delayRange.x, delayRange.y));
                    
                    starTweens.Add(tween);
                }
            }
        }

        private IEnumerator TransitionSequence(string sceneName)
        {
            // 씬 비동기 로드 시작
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            
            if(op != null)
                op.allowSceneActivation = false;

            // 모든 별이 목적지에 도착할 때까지 대기
            foreach (var t in starTweens)
            {
                if (t != null && t.IsActive())
                {
                    yield return t.WaitForCompletion();
                }
            }

            // 씬 로딩 완료 대기
            if (op != null)
            {
                while (op.progress < 0.9f) yield return null;
                op.allowSceneActivation = true;
            }

            // 씬 전환 후 약간의 대기 후 정리 (씬 전환 시점의 부드러움을 위해)
            yield return new WaitForSeconds(0.1f);
            
            ClearStars();
            starTweens.Clear();
            transitionRoot.gameObject.SetActive(false);
        }

        private void ClearStars()
        {
            foreach (var s in stars) if (s != null) Destroy(s.gameObject);
            stars.Clear();
        }
    }
}