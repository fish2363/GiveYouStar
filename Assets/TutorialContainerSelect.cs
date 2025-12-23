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

    private bool isGiveGift;
    private bool isCanPowerUp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(TutorialManager.Instance.IsPlayEndTutorial)
        {
            goStart.SetActive(false);
            TextPanelEvent textPanelEvent = new();
            textPanelEvent.AddDialogue("자, 지체할 시간이 없네.").
            AddDialogue("이 아름다운 별들을 기다리고 있을 아이들에게\n서둘러 선물을 나눠주러 가세나! 허허허!")
            .AddEvent(() =>
            {
                isGiveGift = true;
                pressGift.SetActive(true);
            });

            textChannel.RaiseEvent(textPanelEvent);
        }
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
        else if (!TutorialManager.Instance.IsPlayEndTutorial && !TutorialManager.Instance.IsFirstTutorial)
            goStart.SetActive(true);
    }

    public void GoCollector()
    {
        if (!isCollector) return;
        pressCollect.SetActive(false);
        TextPanelEvent textPanelEvent = new();
        textPanelEvent.AddDialogue("허허, 이걸 보게!")
        .AddDialogue("자네가 앞으로 모으게 될 별들의 정보가\n모두 담겨있는 소중한 기록지라네.")
        .AddDialogue("아직 도감에는 어떤 별도 볼 수 없지만\n별을 발견한 뒤, 도감에 그려진 별을 누르면\n그 별에 대해 더 알 수 있을 걸세")
        .AddEvent(() =>
        {
            isPowerUp = true;
        });

        textChannel.RaiseEvent(textPanelEvent);
    }

    public void GoPowerUp()
    {
        if (isPowerUp)
        {
            pressPower.SetActive(false);
            TextPanelEvent textPanelEvent = new();
            textPanelEvent.AddDialogue("여기선 별을 더 빠르고 정확하게 낚을 수\n있도록 도구를 강화할 수 있다네. ")
            .AddRestMinute(1f)
            .AddDialogue("참!")
            .AddDialogue("필요한 재화는 아이들에게 별을 선물하고 받은\n'감사의 마음'으로 바꿀 수 있으니 참고하게나.")
            .AddEvent(() =>
            {

            });

            textChannel.RaiseEvent(textPanelEvent);
        }
        else if(isCanPowerUp)
        {
            pressPower.SetActive(false);
            TextPanelEvent textPanelEvent = new();
            textPanelEvent.AddDialogue("아이들의 감사표시가 모이면\n바로 도구를 강화할 수 있다네.")
            .AddRestMinute(1f)
            .AddDialogue("참!")
            .AddDialogue("최대로 강화할 수 있는 한계치가\n있는 거 명심하고 신중히 선택하게나!")
            .AddEvent(() =>
            {

            });

            textChannel.RaiseEvent(textPanelEvent);
        }
    }


    public void GoGiftKid()
    {
        if (isExplainGift)
        {
            TextPanelEvent textPanelEvent = new();
            textPanelEvent.AddDialogue("아이들이 각자 갖고 싶은\n별을 이야기하고 있구먼!")
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
        else if(isGiveGift)
        {
            TextPanelEvent textPanelEvent = new();
            textPanelEvent.AddDialogue("밑을 보게나, 자네가 챙겨온\n선물들이 차례대로 모여있구만!")
            .AddDialogue("선물을 끌어서 그 선물을 간절히 원하는 아이에게 전해주면,\n아이들이 참 기뻐할 게야.")
            .AddDialogue("원하는 아이가 없다면\n오른쪽 밑, 선물상자에 넣게!")
            .AddEvent(() =>
            {
                isCanPowerUp = true;
            });

            textChannel.RaiseEvent(textPanelEvent);
        }
    }

    public void SetActivePressGiftDown()
    {
        if (!TutorialManager.Instance.IsFirstTutorial && !TutorialManager.Instance.IsPlayEndTutorial) return;

        if (isExplainGift)
            pressCollect.SetActive(true);
        else if(isGiveGift)
            pressPower.SetActive(true);

        isExplainGift = false;
        isGiveGift = false;

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
        if (!TutorialManager.Instance.IsFirstTutorial && !TutorialManager.Instance.IsPlayEndTutorial) return;
        isPowerUp = false;
        isCanPowerUp = false;

        goStart.SetActive(true);
    }

    public void StartGame()
    {
        if (isPowerUp || isCollector || isExplainGift) return;

        TutorialManager.Instance.IsFirstTutorial = false;
        TutorialManager.Instance.IsPlayEndTutorial = true;
    }
}
