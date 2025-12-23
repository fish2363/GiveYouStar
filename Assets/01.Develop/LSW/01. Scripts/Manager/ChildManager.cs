using System.Collections.Generic;
using _01.Develop.LSW._01._Scripts.So;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public class ChildManager : MonoSingleton<ChildManager>
    {
        private List<StarSo> _requiredStars = new List<StarSo>();
        
        public bool GetReqStarsEmpty()
            => _requiredStars.Count == 0;
        
        public void AddReqStars(StarSo stars)
        {
            _requiredStars.Add(stars);
        }
        
        public void RemoveReqStar(StarSo star)
        {
            _requiredStars.Remove(star);
        }
    }
}