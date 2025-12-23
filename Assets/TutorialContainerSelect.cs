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
        if (!isPowerUp) return;
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
        TutorialManager.Instance.IsPlayEndTutorial = true;
    }
}
