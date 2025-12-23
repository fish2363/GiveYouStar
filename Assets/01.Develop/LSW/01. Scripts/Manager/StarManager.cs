using System;
using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.So;
using _01.Develop.LSW._01._Scripts.UI.InGame;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class StarManager : MonoSingleton<StarManager>
    {
        [SerializeField] private List<StarSo> allStarList = new List<StarSo>();
        
        public event Action<List<StarSo>> onGameEnd;
        
        private Dictionary<StarSo, bool> _stars = new Dictionary<StarSo, bool>();
        public List<StarSo> _gotStarContainer = new List<StarSo>();
        
        protected override void Awake()
        {
            base.Awake();
            foreach (var star in allStarList)
            {
                _stars.Add(star, false); ;
            }
        }
        
        public void UnlockStar(StarSo star)
        {
            if (_stars.ContainsKey(star) && !_stars[star])
            {
                _stars[star] = true;
            }
        }
        
        public void AddGotStar(StarSo star)
        {
            _gotStarContainer.Add(star);
            UnlockStar(star);
        }

        public List<StarSo> GetAllGotStars()
            => _gotStarContainer;
        
        public Dictionary<StarSo, bool> GetStarList()
            => _stars;

        public (StarSo, bool) GetStarStatue(StarSo starSo)
            => (starSo, _stars[starSo]);
        
        public List<StarSo> GetAllStars()
            => allStarList;
        
        public void ClearGotStars()
            => _gotStarContainer.Clear();
        
        public void EndGame()
        {
            onGameEnd?.Invoke(_gotStarContainer);
        }
    }
}