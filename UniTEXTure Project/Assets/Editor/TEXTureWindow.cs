using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Editor
{
    public class TEXTureWindow : EditorWindow
    {
        private string _prompt = "";
        private string _expName = "";
        private int _seed;
        private Texture2D _texture;
        private Object _objFile;
        private static GUIStyle GenerateButton;

        [MenuItem("UniTEXTure/Create")]
        private static void ShowWindow()
        {
            var window = GetWindow<TEXTureWindow>();
            window.titleContent = new GUIContent("UniTEXTure Creator");
            window.minSize = new Vector2(400, 400);
            window.Show();

            GenerateButton = new GUIStyle("Button")
            {
                fontStyle = FontStyle.BoldAndItalic,
                normal = { textColor = Color.gray },
                hover = { textColor = Color.cyan },
                active = { }
            };
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

            if (GUI.Button(itemRect, "Select Texture (Optional)"))
            {
                _texture = (Texture2D) SelectObject("Select Texture (Optional)", "png,jpg,jpeg");
            }
            itemRect.y += itemRect.height + padding;

            DisplayFilePath(itemRect, _texture);
            itemRect.y += itemRect.height + padding;

            if (GUI.Button(itemRect, "Select OBJ File"))
            {
                _objFile = SelectObject("Select OBJ File", "obj");
            }
            itemRect.y += itemRect.height + padding;

            DisplayFilePath(itemRect, _objFile);
            itemRect.y += itemRect.height + padding;

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_prompt) || string.IsNullOrEmpty(_expName) || _objFile == null || _seed < 0);
            if (GUI.Button(itemRect, "Generate New Texture", GenerateButton))
            {
                Generate();
            }
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

        // Currently an unused function (has been replaced with SelectObject(string, string))
        private Object DrawObjectField(Rect rect, string label, string tip, Object value, System.Type type, bool allowSceneObjects)
        {
            return EditorGUI.ObjectField(rect, new GUIContent(label, tip), value, type, allowSceneObjects);
        }

        private Object SelectObject(string title, string type)
        {
            string path = EditorUtility.OpenFilePanel(title, "", type);

            if (!string.IsNullOrEmpty(path))
            {
                // Convert absolute path to relative path
                string projectPath = Application.dataPath;
                if (path.StartsWith(projectPath))
                {
                    // Make sure that the path starts with "Assets/"
                    path = "Assets" + path.Substring(projectPath.Length);
                }
                else
                {
                    UnityEngine.Debug.LogError("Selected texture is not within the project folder. Try again.");
                }

                return AssetDatabase.LoadAssetAtPath<Object>(path);
            }

            return null;
        }

        private void DisplayFilePath(Rect rect, Object obj)
        {
            string filePath = obj
                ? $"{Application.dataPath}/{AssetDatabase.GetAssetPath(obj).Replace("Assets/", "")}"
                : "No File Selected";
            EditorGUI.LabelField(rect, filePath);
        }


        private void Generate()
        {
            UnityEngine.Debug.Log("Generating New Texture");

            // Error Checking
            if (_objFile == null || !IsObjFile(_objFile))
            {
                UnityEngine.Debug.LogError("Please select a valid .obj file.");
                return;
            }

            string objFilePath = AssetDatabase.GetAssetPath(_objFile);

            string pythonScriptPath = "C:\\Users\\bagli\\OneDrive\\Desktop\\test.py";

            string command = $"python \"{pythonScriptPath}\" \"{_prompt}\" \"{_expName}\" {_seed} \"{objFilePath}\"";

            // Start a process to execute the Python script
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            Process process = new Process { StartInfo = psi };
            process.Start();

            // Pass the command to the command prompt
            process.StandardInput.WriteLine(command);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            // Read the output of the Python script
            string output = process.StandardOutput.ReadToEnd();

            // Close the process
            process.WaitForExit();
            process.Close();

            // Log the Python script's output
            UnityEngine.Debug.Log(output);
        }

        private bool IsObjFile(Object obj)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            string extension = System.IO.Path.GetExtension(assetPath);
            return extension.Equals(".obj", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}