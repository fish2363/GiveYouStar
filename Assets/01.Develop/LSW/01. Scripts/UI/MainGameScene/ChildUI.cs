using _01.Develop.LSW._01._Scripts.Manager;
using _01.Develop.LSW._01._Scripts.So;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class ChildUI : MonoBehaviour
    {
        public int giveCoinAmt;
        
        private StarSo _reqStar;
        private bool _alreadyGiven;
        
        public void SetReqStar(StarSo star)
        {
            _reqStar = star;
            _alreadyGiven = false;
        }
        
        public bool GiveStar(StarSo star)
        {
            if (star != _reqStar)
                return false;

            _alreadyGiven = true;
            PlayerStatManager.Instance.ChangeCoinAmount(giveCoinAmt);
            return true;
        }
    }
}