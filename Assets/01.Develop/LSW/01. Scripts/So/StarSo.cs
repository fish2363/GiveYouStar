using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.So
{
    [CreateAssetMenu(fileName = "StarSo", menuName = "SO/StarSo")]
    public class StarSo : ScriptableObject
    {
        public Sprite starImage;
        public string starName;
        public string description;
        
        public float speed;
        public GameObject starPrefab;
        
        public bool isUnlocked = false;
    }
}
