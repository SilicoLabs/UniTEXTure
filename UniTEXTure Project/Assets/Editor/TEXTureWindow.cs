using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TEXTureWindow : EditorWindow
    {
        private string _prompt = "";
        private string _expName = "";
        private int _seed;
        private Texture2D _texture;
        private static GUIStyle GenerateButton = new GUIStyle("Button")
        {
            fontStyle = FontStyle.BoldAndItalic,
            normal = { textColor = Color.gray },
            hover = { textColor = Color.cyan },
            active = { }
        };

        [MenuItem("UniTEXTure/Create")]
        private static void ShowWindow()
        {
            var window = GetWindow<TEXTureWindow>();
            window.titleContent = new GUIContent("UniTEXTure Creator");
            window.minSize = new Vector2(400, 400);
            window.Show();
        }

        private void OnGUI()
        {
            var padding = 5;
            var itemRect = new Rect(20, 100, position.width-40, 18);

            _prompt = DrawTextField(itemRect, "Prompt", "Enter Prompt for Generating New Texture", _prompt);
            itemRect.y += itemRect.height + padding;

            _expName = DrawTextField(itemRect, "Experiment Name", "Enter Experiment Name", _expName);
            itemRect.y += itemRect.height + padding;

            _seed = DrawIntField(itemRect, "Seed", "Enter Seed for Generating New Texture", _seed);
            itemRect.y += itemRect.height + padding;

            _texture = (Texture2D)DrawObjectField(itemRect, "Texture (Optional)", "Enter Initial Texture", _texture, typeof(Texture2D), false);
            itemRect.y += itemRect.height + padding;

            var filePath = _texture ? Application.dataPath + "/" + AssetDatabase.GetAssetPath(_texture) : "No Texture Selected";
            EditorGUI.LabelField(itemRect, filePath, new GUIStyle(EditorStyles.boldLabel) { });
            itemRect.y += itemRect.height + padding;

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_prompt));
            if (GUI.Button(itemRect, "Generate New Texture", GenerateButton)) Generate();
            EditorGUI.EndDisabledGroup();
        }

        private string DrawTextField(Rect rect, string label, string tip, string value)
        {
            return EditorGUI.TextField(rect, new GUIContent(label, tip), value);
        }

        private int DrawIntField(Rect rect, string label, string tip, int value)
        {
            return EditorGUI.IntField(rect, new GUIContent(label, tip), value);
        }

        private Object DrawObjectField(Rect rect, string label, string tip, Object value, System.Type type, bool allowSceneObjects)
        {
            return EditorGUI.ObjectField(rect, new GUIContent(label, tip), value, type, allowSceneObjects);
        }

        private void Generate()
        {
            Debug.Log("Generating New Texture"); 
        }
    }
}