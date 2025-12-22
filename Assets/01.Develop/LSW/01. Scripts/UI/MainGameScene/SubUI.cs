using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class SubUI : MonoBehaviour
    {
        private GameObject _interactionTrm;
        private Image _mainBg;
        
        public void Show(GameObject interactionTransform, Image mainBackGround)
        {
            _interactionTrm = interactionTransform;
            _mainBg = mainBackGround;
            
            _interactionTrm.SetActive(false);
            mainBackGround.DOFade(0f, 0.5f).OnComplete(()=>
            {
                mainBackGround.raycastTarget = false;
            });
        }
        
        public void Close()
        {
            if(_interactionTrm == null || _mainBg == null)
                return;
            
            _mainBg.DOFade(1f, 0.5f).OnComplete(()=>
            {
                _mainBg.raycastTarget = true;
                _interactionTrm.SetActive(true);
                gameObject.SetActive(false);
            });
        }
    }
}