using _01.Develop.LSW._01._Scripts.Manager;
using UnityEngine;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [field:SerializeField] public bool IsFirstTutorial { get; set; } = true;
    [field:SerializeField] public bool IsPlayEndTutorial { get; set; } = false;
}
