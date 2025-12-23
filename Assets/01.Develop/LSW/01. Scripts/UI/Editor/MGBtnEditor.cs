using _01.Develop.LSW._01._Scripts.UI.MainGameScene;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace _01.Develop.LSW._01._Scripts.UI.Editor
{
    [CustomEditor(typeof(MGBtn))]
    public class MGBtnEditor : UnityEditor.Editor
    {
        private VisualElement _moveSceneField;
        private VisualElement _showUIField;
        private SerializedProperty _btnTypeProp;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            _moveSceneField = new VisualElement();
            _showUIField = new VisualElement();
            
            _btnTypeProp = serializedObject.FindProperty("btnType");
            
            var btnTypeField = new PropertyField(_btnTypeProp);
            
            root.Add(btnTypeField);
            root.Add(new PropertyField(serializedObject.FindProperty("interactionTrm")));
            
            #region showUI
            
            _showUIField.Add(new PropertyField(serializedObject.FindProperty("targetUI")));
            _showUIField.Add(new PropertyField(serializedObject.FindProperty("mainBackGround")));
            root.Add(_showUIField);
            
            #endregion

            #region moveScene
            
            _moveSceneField.Add(new PropertyField(serializedObject.FindProperty("targetSceneName")));
            root.Add(_moveSceneField);
            
            #endregion

            btnTypeField.RegisterValueChangeCallback(_ =>
            {
                serializedObject.Update(); 
                UpdateFieldVisibility((MGBtnType)_btnTypeProp.enumValueIndex);
            });
            
            UpdateFieldVisibility((MGBtnType)_btnTypeProp.enumValueIndex);
            return root;
        }

        private void UpdateFieldVisibility(MGBtnType currentType)
        {
            _showUIField.style.display = (currentType == MGBtnType.ShowUI) 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
            
            _moveSceneField.style.display = (currentType == MGBtnType.MoveScene) 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }
    }
}