using _01.Develop.LSW._01._Scripts.Manager;
using UnityEngine;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    public static bool IsTutorial { get; set; } = true;
}
