using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class SubUI : MonoBehaviour
    {
        private GameObject _interactionTrm;
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show(GameObject interactionTransform)
        {
            _interactionTrm = interactionTransform;
            _interactionTrm.SetActive(false);
            
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.DOFade(1f, 0.5f);
        }
        
        public void Close()
        {
            if(_interactionTrm == null)
                return;
            
            _canvasGroup.DOFade(0f, 0.5f).OnComplete(()=>
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
                _interactionTrm.SetActive(true);
            });
        }
    }
}