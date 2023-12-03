using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Google.Protobuf;
using System.Net.Sockets;

namespace Editor
{
    public class TEXTureWindow : EditorWindow
    {
        private string _prompt = "";
        private string _expName = "";
        private int _seed;
        private Texture2D _texture;
        private UnityEngine.Object _objFile;
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

        private UnityEngine.Object SelectObject(string title, string type)
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

                return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            }

            return null;
        }

        private void DisplayFilePath(Rect rect, UnityEngine.Object obj)
        {
            string filePath = obj
                ? $"{Application.dataPath}/{AssetDatabase.GetAssetPath(obj).Replace("Assets/", "")}"
                : "No File Selected";
            EditorGUI.LabelField(rect, filePath);
        }
        
        private void Generate()
        {
            UnityEngine.Debug.Log("Generating New Texture");

            string configFilePath = CreateYAML();

            UniTexture message = new UniTexture {
                YamlPath = configFilePath
            };

            byte[] bytes = message.ToByteArray();
            
            Console.WriteLine("Hello, World!");
            
            // Assume Python server is running on localhost, port 5555
            string pythonServerAddress = "127.0.0.1";
            int pythonServerPort = 5555;

            using (TcpClient client = new TcpClient(pythonServerAddress, pythonServerPort))
            using (NetworkStream stream = client.GetStream())
            {
                // Send the serialized protobuf message to Python
                stream.Write(bytes, 0, bytes.Length);
            }
            
            UnityEngine.Debug.Log("Texture generation request sent to Python server.");
        }

        // Currently an unused function but checks whether the passed file is a .obj file
        private bool IsObjFile(UnityEngine.Object obj)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            string extension = System.IO.Path.GetExtension(assetPath);
            return extension.Equals(".obj", System.StringComparison.OrdinalIgnoreCase);
        }

        private string CreateYAML()
        {
            string n = Environment.NewLine;

            // Define the YAML content
            string yamlContent =
                "log:" + n +
                    "\texp_name: " + _expName + n +

                "guide:" + n +
                    "\ttext: " + _prompt + n +
                    ((_texture != null) ? "\tinital_texture: " + _texture.name + ".png" + n : "") +
                    "\tshape_path: " + _objFile.name + ".obj" + n +

                "optim:" + n +
                    "\tseed: " + _seed + n;

            // Write the YAML to a file
            string fileName = _expName + ".yaml";

            string relativePath = Path.Combine(
                "..", // Go up one directory
                "TEXTurePaper", // Navigate to the TEXTurePaper directory
                "configs", // Navigate to the configs directory
                "text_guided" // Navigate to the text_guided directory
            );

            UnityEngine.Debug.Log(relativePath);

            string filePath = Path.Combine(relativePath, fileName);
            filePath = filePath.Replace("\\", "/");
            File.WriteAllText(filePath, yamlContent);

            UnityEngine.Debug.Log("YAML file created: " + filePath);

            return filePath;
        }
    }
}