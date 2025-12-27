using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class UpgradeManager : MonoSingleton<UpgradeManager>
    {
        public int currentFailDistLevel;
        public int currentSpeedLevel;
        public int currentRopeSizeLevel;
    }
}