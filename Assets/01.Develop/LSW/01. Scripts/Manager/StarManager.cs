using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.So;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class StarManager : MonoSingleton<StarManager>
    {
        [SerializeField] private List<StarSo> starList = new List<StarSo>();
        
        private Dictionary<StarSo, bool> _stars = new Dictionary<StarSo, bool>();

        protected override void Awake()
        {
            base.Awake();
            foreach (var star in starList)
            {
                _stars.Add(star, true);
            }
        }
        
        public void UnlockStar(StarSo star)
        {
            if (_stars.ContainsKey(star) && !_stars[star])
            {
                _stars[star] = true;
            }
        }

        public Dictionary<StarSo, bool> GetStarList()
            => _stars;

        public (StarSo, bool) GetStar(StarSo starSo)
            => (starSo, _stars[starSo]);
    }
}