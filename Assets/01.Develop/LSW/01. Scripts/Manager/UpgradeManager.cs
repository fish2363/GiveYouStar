using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class UpgradeManager : MonoSingleton<UpgradeManager>
    {
        public int currentFailDistLevel = 0;
        public int currentSpeedLevel = 0;
        public int currentRopeSizeLevel = 0;
    }
}