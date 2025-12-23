using System;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueType
{
    Text,
    Wait,
    Event
}

[Serializable]
public class DialogueSetting
{
    public DialogueType type;
    public string text;
    public float value;
    public Action onEvent;

    public DialogueSetting(DialogueType type, string text = "", float value = 0f,Action onEvent = null)
    {
        this.type = type;
        this.text = text;
        this.value = value;
        this.onEvent = onEvent;
    }
}

public class TextPanelEvent : GameEvent
{
    public List<DialogueSetting> Dialogue = new();
    public static bool IsRUNNING;

    public TextPanelEvent AddDialogue(string text)
    {
        Dialogue.Add(new DialogueSetting(DialogueType.Text,text));
        return this;
    }

    public TextPanelEvent AddRestMinute(float value)
    {
        Dialogue.Add(new DialogueSetting(DialogueType.Wait, value: value));
        return this;
    }

    public TextPanelEvent AddEvent(Action action)
    {
        Dialogue.Add(new DialogueSetting(DialogueType.Event,onEvent:action));
        return this;
    }
}
