using _01.Develop.LSW._01._Scripts.Manager;
using _01.Develop.LSW._01._Scripts.UI.InGame;
using Assets._01.Develop.CDH.Code.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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

        private bool _moveScene;
        
        private void Start()
        {
            if (btnType == MGBtnType.ShowUI)
            {
                targetUI.gameObject.SetActive(false);
            }
        }

        public void OnClick()
        {
            if(_moveScene)
                return;
            
            switch (btnType)
            {
                case MGBtnType.ShowUI:
                    targetUI.gameObject.SetActive(true);
                    targetUI.Show(interactionTrm, mainBackGround);
                    break;
                case MGBtnType.MoveScene:
                    _moveScene = true;
                    StarManager.Instance.ClearGotStars();
                    TransitionManager.Instance.NextScene(targetSceneName);
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
