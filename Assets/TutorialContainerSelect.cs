using UnityEngine;

public class TutorialContainerSelect : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO textChannel;
    [SerializeField] private GameObject pressGift;
    [SerializeField] private GameObject pressCollect;
    [SerializeField] private GameObject pressPower;
    [SerializeField] private GameObject goStart;

    private bool isExplainGift;
    private bool isCollector;
    private bool isPowerUp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(TutorialManager.Instance.IsFirstTutorial)
        {
            goStart.SetActive(false);
            TextPanelEvent textPanelEvent = new();
            textPanelEvent.AddDialogue("허허허! 자, 이제 우리 귀여운 아이들에게\n반짝이는 별을 선물하러 떠나볼까나!").
            AddDialogue("먼저 저기 오른쪽에 있는 집 방향으로 가서,\n어떤 선물이 필요한지 확인해보세나.")
            .AddEvent(()=>
            {
                isExplainGift = true;
                pressGift.SetActive(true);
            });

            textChannel.RaiseEvent(textPanelEvent);
        }
        else
            goStart.SetActive(true);
    }

    public void GoCollector()
    {
        if (!isCollector) return;
        pressCollect.SetActive(false);
        TextPanelEvent textPanelEvent = new();
        textPanelEvent.AddDialogue("각자 가지고 싶은\n별을 이야기 하고 있군요..")
        .AddDialogue("바로 출발해봅시다!")
        .AddRestMinute(1f)
        .AddDialogue("근데 저게 다\n무슨 별이죠..?")
        .AddDialogue("종류를 알아야 선물을 할텐데..\n우선 도감을 확인해볼까요?")
        .AddEvent(() =>
        {
            isPowerUp = true;
        });

        textChannel.RaiseEvent(textPanelEvent);
    }

    public void GoPowerUp()
    {
        if (!isPowerUp) return;
        pressPower.SetActive(false);
        TextPanelEvent textPanelEvent = new();
        textPanelEvent.AddDialogue("각자 가지고 싶은\n별을 이야기 하고 있군요..")
        .AddDialogue("바로 출발해봅시다!")
        .AddRestMinute(1f)
        .AddDialogue("근데 저게 다\n무슨 별이죠..?")
        .AddDialogue("종류를 알아야 선물을 할텐데..\n우선 도감을 확인해볼까요?")
        .AddEvent(() =>
        {
            
        });

        textChannel.RaiseEvent(textPanelEvent);
    }


    public void GoGiftKid()
    {
        if (!isExplainGift) return;
        TextPanelEvent textPanelEvent = new();
        textPanelEvent.AddDialogue("아이들이 각자 갖고 싶은\n별을 재잘거리고 있구먼!")
        .AddDialogue("당장이라도 출발하고 싶지만...")
        .AddRestMinute(1f)
        .AddDialogue("허허, 잠시만 기다려보게.")
        .AddDialogue("저 별들이 다 어떤 별인지\n우리 친구는 알고 있나?")
        .AddDialogue("어떤 종류인지 정확히 알아야 실수\n없이 선물을 나눠줄 수 있을 게야. ")
        .AddDialogue("우선 서둘러 도감을 펼쳐서 확인해보고\n가도록 하세! 준비됐으면 말해주게나, 허허허!")
        .AddEvent(() =>
        {
            isCollector = true;
        });

        textChannel.RaiseEvent(textPanelEvent);
    }

    public void SetActivePressGiftDown()
    {
        if (!TutorialManager.Instance.IsFirstTutorial || !isExplainGift) return;
        isExplainGift = false;

        pressCollect.SetActive(true);
        pressGift.SetActive(false);
    }

    public void SetActivePressCollectDown()
    {
        if (!TutorialManager.Instance.IsFirstTutorial || !isCollector) return;
        isCollector = false;

        pressPower.SetActive(true);
    }

    public void SetActivePressPowerDown()
    {
        if (!TutorialManager.Instance.IsFirstTutorial ||  !isPowerUp) return;
        isPowerUp = false;

        goStart.SetActive(true);
    }

    public void StartGame()
    {
        if (isPowerUp || isCollector || isExplainGift) return;

        TutorialManager.Instance.IsFirstTutorial = false;
    }
}
