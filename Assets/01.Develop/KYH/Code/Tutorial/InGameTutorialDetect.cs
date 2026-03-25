using UnityEngine;

public class InGameTutorialDetect : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO textChannel;
    [SerializeField] private GameManager manager;

    [Header("Slide Show (CanvasGroup Array)")]
    [SerializeField] private CanvasGroupSlideShow slideShow;

    private void Start()
    {
        if (TutorialManager.Instance.IsPlayEndTutorial)
        {
            TextPanelEvent textPanelEvent = new();
            textPanelEvent
                .AddDialogue("오늘 밤까지 최대한 많은\n별들을 챙겨서 돌아가야 하네.")
                .AddDialogue("최대한 많은 아이들의 동심을\n지켜야 하니 빨리 서두르지!")
                .AddEvent(() =>
                {
                    // 여기서: 슬라이드 쇼 시작 -> 끝나면 FirstStart 실행
                    if (slideShow != null)
                    {
                        slideShow.Begin(() =>
                        {
                            manager.FirstStart();
                        });
                    }
                    else
                    {
                        manager.FirstStart();
                    }
                });

            textChannel.RaiseEvent(textPanelEvent);
        }
        else
        {
            manager.FirstStart();
        }
    }
}