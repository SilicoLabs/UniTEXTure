using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TEXTureWindow : EditorWindow
    {
        private string _prompt;
        private Texture2D _texture;
        private static GUIStyle GenerateButton;
        
        [MenuItem("UniTEXTure/Create")]
        private static void ShowWindow()
        {
            var window = GetWindow<TEXTureWindow>();
            window.titleContent = new GUIContent("UniTEXTure Creator");
            window.minSize = new Vector2(400, 400);
            // window.maxSize = new Vector2(400, 400);
            window.Show();
            
            GenerateButton = new GUIStyle("Button")
            {
                fontStyle = FontStyle.BoldAndItalic,
                normal = {textColor = Color.gray},
                hover = {textColor = Color.cyan}, 
                active = {}
            };

        }

       

        private void OnGUI()
        {
            var itemRect = new Rect(20, 100, position.width-40, 18);
            _prompt = EditorGUI.TextField(itemRect,
                new GUIContent("Prompt", "Enter Prompt for Generating New Texture"), _prompt);

            itemRect.y += itemRect.height + 5;
            
            _texture = (Texture2D)EditorGUI.ObjectField(itemRect,
                new GUIContent("Texture (Optional)", "Enter Target Texture"), _texture, typeof(Texture2D), false);
            
            itemRect.y += itemRect.height + 5;

            var filePath = _texture ? Application.dataPath + "/" + AssetDatabase.GetAssetPath(_texture) : "No Texture Selected";
            
            EditorGUI.LabelField(itemRect, filePath, new GUIStyle(EditorStyles.boldLabel){});
            
            itemRect.y += itemRect.height + 5;
            
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_prompt));
            if(GUI.Button(itemRect, "Generate New Texture", GenerateButton)) Generate();
            EditorGUI.EndDisabledGroup();
        }

        private void Generate()
        {
            
            Debug.Log("Generating New Texture");
            
        }
    }
}