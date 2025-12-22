using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VHierarchy.Libs;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    [RequireComponent(typeof(Button))]
    public class MGBtn : MonoBehaviour
    {
        [SerializeField] private MGBtnType btnType;
        [SerializeField] private GameObject interactionTrm;
        
        [SerializeField] private Image targetUI;
        
        public string targetSceneName;

        private void Start()
        {
            if (btnType == MGBtnType.ShowUI)
            {
                Color color = targetUI.color;
                color.a = 0f;
                targetUI.color = color;
                targetUI.gameObject.SetActive(false);
            }
        }

        public void OnClick()
        {
            switch (btnType)
            {
                case MGBtnType.ShowUI:
                    interactionTrm.SetActive(false);
                    targetUI.gameObject.SetActive(true);
                    targetUI.DOFade(1f, 0.5f);
                    break;
                case MGBtnType.MoveScene:
                    interactionTrm.SetActive(false);
                    // mainUITrm.DOMoveY(-2000f, 3f)
                    //     .SetEase(Ease.OutExpo)
                    //     .OnComplete(() =>
                    SceneManager.LoadSceneAsync(targetSceneName);
                    //    );
                    break;
            }
        }
    }
    
    public enum MGBtnType
    {
        ShowUI,
        MoveScene
    }
}
