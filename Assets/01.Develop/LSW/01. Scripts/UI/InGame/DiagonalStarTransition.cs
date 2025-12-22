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
        [SerializeField] private float starSize = 128f; // 별 크기를 넉넉하게 잡으세요
        [SerializeField] private Vector2 durationRange = new(0.8f, 1.2f);
        [SerializeField] private Vector2 delayRange = new(0f, 0.4f);
        
        private List<DiagonalFallingStar> stars = new();

        [ContextMenu("Play Transition")]
        public void Play() => Play("mainGameScene");

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

            // 1. 컨테이너 실제 크기 확보
            float screenW = starContainer.rect.width;
            float screenH = starContainer.rect.height;

            // 2. 대각선 이동 시 화면 빈틈을 막기 위해 화면보다 훨씬 넓은 범위를 생성
            // 여유분(Extra)을 더해 격자를 짭니다.
            int cols = Mathf.CeilToInt(screenW / starSize) + 8;
            int rows = Mathf.CeilToInt(screenH / starSize) + 8;

            // 시작 오프셋 (왼쪽 위 구석 밖)
            float startX = -(cols * starSize) * 0.5f;
            float startY = (rows * starSize) * 0.5f;

            // 모든 별이 이동할 공통 거리 벡터 (오른쪽 아래로 길게 관통)
            Vector2 moveVector = new Vector2(screenW * 2f, -screenH * 2f);

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    var star = Instantiate(starPrefab, starContainer);
                    stars.Add(star);

                    RectTransform rt = star.rect;
                    
                    // 핵심: 모든 앵커와 피벗을 중앙(0.5)으로 강제 고정하여 계산 오류 차단
                    rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.sizeDelta = new Vector2(starSize, starSize);

                    // 초기 좌표: 중앙 기준 격자 배치
                    float posX = startX + (c * starSize);
                    float posY = startY - (r * starSize);
                    
                    Vector2 startPos = new Vector2(posX, posY);
                    Vector2 endPos = startPos + moveVector;

                    star.Fall(startPos, endPos, 
                             Random.Range(durationRange.x, durationRange.y), 
                             Random.Range(delayRange.x, delayRange.y));
                }
            }
        }

        private IEnumerator TransitionSequence(string sceneName)
        {
            // 별들이 화면을 충분히 가릴 때까지 대기
            yield return new WaitForSeconds(0.6f);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f) yield return null;
            op.allowSceneActivation = true;

            // 씬 전환 후 별들이 완전히 지나가면 정리
            yield return new WaitForSeconds(durationRange.y);
            ClearStars();
            transitionRoot.gameObject.SetActive(false);
        }

        private void ClearStars()
        {
            foreach (var s in stars) if (s != null) Destroy(s.gameObject);
            stars.Clear();
        }
    }
}