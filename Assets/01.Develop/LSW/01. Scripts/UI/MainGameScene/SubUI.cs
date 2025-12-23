using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class SubUI : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.DOFade(1f, 0.5f);
        }
        
        public void Close()
        {
            _canvasGroup.DOFade(0f, 0.5f).OnComplete(()=>
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            });
        }
    }
}