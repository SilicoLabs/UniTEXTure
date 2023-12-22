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
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace Editor
{
    public class TEXTureWindow : EditorWindow
    {
        // Fields to store user input and file information
        private string _prompt = "";
        private string _expName = "";
        private int _seed;
        private Texture2D _texture;
        private string _textureFilePath;
        private UnityEngine.Object _objFile;
        private string _objFilePath;
        private static GUIStyle GenerateButton;

        // MenuItem attribute to create a menu item for opening the window (in the Unity window)
        [MenuItem("UniTEXTure/Create")]
        private static void ShowWindow()
        {
            // Create and show the TEXTureWindow
            var window = GetWindow<TEXTureWindow>();
            window.titleContent = new GUIContent("UniTEXTure Creator");
            window.minSize = new Vector2(400, 400);
            window.Show();

            // Define and customize the style for the GenerateButton
            GenerateButton = new GUIStyle("Button")
            {
                fontStyle = FontStyle.BoldAndItalic,
                normal = { textColor = Color.gray },
                hover = { textColor = Color.cyan },
                active = { }
            };
        }

        // Define the OnGUI method, which is called to render the GUI for the TEXTureWindow
        private async void OnGUI()
        {
            // Define padding and initial itemRect for layout
            var padding = 5;
            var itemRect = new Rect(20, 100, position.width-40, 18);

            // Draw a text field for the prompt and update its value
            _prompt = DrawTextField(itemRect, "Prompt", "Enter Prompt for Generating New Texture", _prompt);
            itemRect.y += itemRect.height + padding;

            // Draw a text field for the experiment name and update its value
            _expName = DrawTextField(itemRect, "Experiment Name", "Enter Experiment Name", _expName);
            itemRect.y += itemRect.height + padding;

            // Draw an integer field for the seed and update its value
            _seed = DrawIntField(itemRect, "Seed", "Enter Seed for Generating New Texture", _seed);
            itemRect.y += itemRect.height + padding;

            // Button to select a texture (optional)
            if (GUI.Button(itemRect, "Select Texture (Optional)"))
            {
                _texture = (Texture2D) SelectObject("Select Texture (Optional)", "png,jpg,jpeg");
            }
            itemRect.y += itemRect.height + padding;

            // Display the file path for the selected texture
            DisplayFilePath(itemRect, _texture);
            itemRect.y += itemRect.height + padding;

            // Button to select an OBJ file
            if (GUI.Button(itemRect, "Select OBJ File"))
            {
                _objFile = SelectObject("Select OBJ File", "obj");
            }
            itemRect.y += itemRect.height + padding;

            // Display the file path for the selected OBJ file
            DisplayFilePath(itemRect, _objFile);
            itemRect.y += itemRect.height + padding;

            // Disable the generate button if required fields are empty or invalid
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_prompt) || string.IsNullOrEmpty(_expName) || _objFile == null || _seed < 0);
            
            // Button to generate a new texture (async)
            if (GUI.Button(itemRect, "Generate New Texture", GenerateButton))
            {
                await Generate();
            }
            
            EditorGUI.EndDisabledGroup();
        }

        // Method to draw a text field in the Unity editor GUI
        private string DrawTextField(Rect rect, string label, string tip, string value)
        {
            return EditorGUI.TextField(rect, new GUIContent(label, tip), value);
        }

        // Method to draw an integer field in the Unity editor GUI
        private int DrawIntField(Rect rect, string label, string tip, int value)
        {
            return EditorGUI.IntField(rect, new GUIContent(label, tip), value);
        }

        // Method to open a file dialog and select an object (file) based on title and file type
        private UnityEngine.Object SelectObject(string title, string type)
        {
            // Open a file panel to select a file with a given title and file type
            string path = EditorUtility.OpenFilePanel(title, "", type);

            // Check if a file was selected
            if (!string.IsNullOrEmpty(path))
            {
                // Convert the absolute path to a relative path within the Unity project
                string projectPath = Application.dataPath;
                if (path.StartsWith(projectPath))
                {
                    // Make sure the path starts with "Assets/"
                    path = "Assets" + path.Substring(projectPath.Length);

                    // Update file paths based on the selected file type
                    if (type == "obj")
                    {
                        _objFilePath = $"{Application.dataPath}/" + path.Replace("Assets/", "");
                    }
                    else if (type == "png,jpg,jpeg")
                    {
                        _textureFilePath = $"{Application.dataPath}/" + path.Replace("Assets/", "");
                    }
                    else
                    {
                        // Handle other file types or log an error
                        UnityEngine.Debug.LogError($"Unsupported file type: {type}");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Selected texture is not within the project folder. Try again.");
                }

                // Load the selected asset (object) at the specified path
                return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            }

            // If no file was selected
            return null;
        }

        // Method to display the file path of a Unity object in the Unity editor GUI
        private void DisplayFilePath(Rect rect, UnityEngine.Object obj)
        {
            // Check if a Unity object is provided
            string filePath = obj
                ? $"{Application.dataPath}/{AssetDatabase.GetAssetPath(obj).Replace("Assets/", "")}"
                : "No File Selected";

            // Display the file path in the Unity editor GUI
            EditorGUI.LabelField(rect, filePath);
        }
        
        // Method for generating a new texture by sending a request to the Flask server
        private async Task Generate()
        {
            UnityEngine.Debug.Log("Generating New Texture");

            // Set the address of the Flask server (replace with the correct address)
            string flaskServerAddress = "http://bf83-34-236-55-223.ngrok-free.app/process_texture"; // THIS IS NOT THE FIXED ADDRESS

            try
            {
                // Create an HttpClient with a timeout of 20 minutes
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(20); // Not sure if this is the correct timeout time based on TEXTure

                    // Serialize UniTexture proto object to bytes
                    UniTexture message = new UniTexture
                    {
                        ExpName = _expName,
                        Prompt = _prompt,
                        Seed = _seed,
                        InitTexture = ByteString.CopyFrom(File.ReadAllBytes(_textureFilePath)),
                        ObjFile = ByteString.CopyFrom(File.ReadAllBytes(_objFilePath)),
                    };

                    byte[] bytes = message.ToByteArray();

                    // Create StreamContent with the serialized UniTexture object
                    using (StreamContent content = new StreamContent(new MemoryStream(bytes)))
                    {
                        // Set the content type to application/octet-stream
                        content.Headers.TryAddWithoutValidation("Content-Type", "application/octet-stream");

                        // Send a POST request to the Flask server
                        HttpResponseMessage response = await client.PostAsync(flaskServerAddress, content);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            UnityEngine.Debug.Log("Texture generation request sent to Python server.");
                            string responseContent = await response.Content.ReadAsStringAsync();
                            UnityEngine.Debug.Log($"Server response: {responseContent}");
                            
                            // Save the received texture file locally
                            string fileName = $"{_expName}_generated_texture.png";

                            // Create the file and copy the response content to it
                            using (Stream fileStream = File.Create(fileName))
                            {
                                await response.Content.CopyToAsync(fileStream);
                                UnityEngine.Debug.Log("Saved file.");
                            }
                        }
                        else
                        {
                            // Log an error if the request was not successful
                            UnityEngine.Debug.LogError($"Error sending data to Python: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // General exception log
                UnityEngine.Debug.LogError($"Error sending data to Python: {ex.Message}");
            }

            UnityEngine.Debug.Log("Texture generation request successfully sent to Python server, file successfully received.");
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
    }
}