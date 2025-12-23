using System;
using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.So;
using _01.Develop.LSW._01._Scripts.UI.InGame;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class StarManager : MonoSingleton<StarManager>
    {
        [SerializeField] private List<StarSo> starList = new List<StarSo>();
        
        public event Action<List<StarSo>> onGameEnd;
        
        private Dictionary<StarSo, bool> _stars = new Dictionary<StarSo, bool>();
        public List<StarSo> _gotStarContainer = new List<StarSo>();
        
        protected override void Awake()
        {
            base.Awake();
            foreach (var star in starList)
            {
                _stars.Add(star, false);
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
        }

        public List<StarSo> GetAllGotStars()
            => _gotStarContainer;
        
        public Dictionary<StarSo, bool> GetStarList()
            => _stars;

        public (StarSo, bool) GetStar(StarSo starSo)
            => (starSo, _stars[starSo]);
        
        public void EndGame()
        {
            onGameEnd?.Invoke(_gotStarContainer);
        }
    }
}