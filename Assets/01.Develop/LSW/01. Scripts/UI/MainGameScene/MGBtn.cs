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
        
        [SerializeField] private SubUI targetUI;
        [SerializeField] private Image mainBackGround;
        
        public string targetSceneName;

        private void Start()
        {
            if (btnType == MGBtnType.ShowUI)
            {
                targetUI.gameObject.SetActive(false);
            }
        }

        public void OnClick()
        {
            switch (btnType)
            {
                case MGBtnType.ShowUI:
                    targetUI.gameObject.SetActive(true);
                    targetUI.Show(interactionTrm, mainBackGround);
                    break;
                case MGBtnType.MoveScene:
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
