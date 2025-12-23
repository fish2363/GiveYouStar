using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO _textChannelSO;

    [Header("UI")]
    [SerializeField] private Image textPanel;
    [SerializeField] private Image dontTouchPanel;
    [SerializeField] private TextMeshProUGUI tmpText;

    [Header("--텍스트 설정--")]
    [SerializeField, Range(0.05f, 0.8f)] private float typeTime;
    [SerializeField, ColorUsage(true, true)] private Color startColor;
    [SerializeField, ColorUsage(true, true)] private Color endColor;

    private Sequence animSequence;

    private void Awake()
    {
        _textChannelSO.AddListener<TextPanelEvent>(TextPanelEventHandle);
        animSequence = DOTween.Sequence();
    }

    private void TextPanelEventHandle(TextPanelEvent obj)
    {
        if (TextPanelEvent.IsRUNNING)
        {
            Debug.LogError("텍스트가 중첩 실행되려 하고 있습니다.");
            return;
        }
        TextPanelEvent.IsRUNNING = true;
        dontTouchPanel.gameObject.SetActive(true);
        StartCoroutine(TextRoutine(obj));
    }

    private IEnumerator TextRoutine(TextPanelEvent obj)
    {
        StartPanelDialogueUI();
        foreach (DialogueSetting dialogue in obj.Dialogue)
        {
            switch (dialogue.type)
            {
                case DialogueType.Text:
                    yield return StartEffect(dialogue.text);
                    break;
                case DialogueType.Event:
                    dialogue.onEvent?.Invoke();
                    break;
                case DialogueType.Wait:
                    EndPanelDialogueUI();
                    yield return new WaitForSeconds(dialogue.value);
                    StartPanelDialogueUI();
                    break;
            }
        }
        EndPanelDialogueUI();
        dontTouchPanel.gameObject.SetActive(false);
        TextPanelEvent.IsRUNNING = false;
    }

    private IEnumerator StartEffect(string msg)
    {
        tmpText.color = endColor;
        tmpText.SetText(msg);
        tmpText.ForceMeshUpdate();
        tmpText.maxVisibleCharacters = 0;
        TMP_TextInfo textInfo = tmpText.textInfo;

        bool skip = false;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!skip && Mouse.current.leftButton.wasPressedThisFrame)
            {
                skip = true;
                tmpText.maxVisibleCharacters = textInfo.characterCount;
                tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                break;
            }

            tmpText.maxVisibleCharacters = i + 1;

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
            {
                float t = 0f;
                while (t < typeTime)
                {
                    if (!skip && Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        skip = true;
                        tmpText.maxVisibleCharacters = textInfo.characterCount;
                        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                        break;
                    }
                    t += Time.deltaTime;
                    yield return null;
                }
                if (skip) break;
            }
            else
            {
                Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                Color32[] vertexColor = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

                int vIndex0 = charInfo.vertexIndex;
                int vIndex1 = vIndex0 + 1;
                int vIndex2 = vIndex0 + 2;
                int vIndex3 = vIndex0 + 3;

                Vector3 v1Origin = vertices[vIndex1];
                Vector3 v2Origin = vertices[vIndex2];

                float currentTime = 0;
                float percent = 0;
                while (percent < 1)
                {
                    if (!skip && Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        skip = true;
                        tmpText.maxVisibleCharacters = textInfo.characterCount;
                        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                        break;
                    }

                    currentTime += Time.deltaTime;
                    percent = currentTime / typeTime;

                    float yDelta = Mathf.Lerp(2f, 0, percent);
                    vertices[vIndex1] = v1Origin + new Vector3(0, yDelta, 0);
                    vertices[vIndex2] = v2Origin + new Vector3(0, yDelta, 0);

                    for (int j = 0; j < 4; j++)
                        vertexColor[vIndex0 + j] = Color.Lerp(startColor, endColor, percent);

                    tmpText.UpdateVertexData(
                        TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);

                    yield return null;
                }
                if (skip) break;
            }
        }
        yield return null;
        yield return new WaitUntil(() => Mouse.current.leftButton.wasPressedThisFrame);
        yield return null;
    }

    private void StartPanelDialogueUI()
    {
        animSequence?.Kill();
        animSequence = DOTween.Sequence();
        animSequence.Append(textPanel.transform.DOScaleY(1f, 0.2f).SetEase(Ease.OutBack));
    }

    private void EndPanelDialogueUI()
    {
        animSequence?.Kill();
        animSequence = DOTween.Sequence();
        animSequence.Append(textPanel.transform.DOScaleY(0f, 0.2f).SetEase(Ease.OutBack));
    }

    private void OnDestroy()
    {
        _textChannelSO.RemoveListener<TextPanelEvent>(TextPanelEventHandle);
    }
}
