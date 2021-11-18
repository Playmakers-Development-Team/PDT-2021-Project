using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    internal class CropGenerator : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Image maskImage;
        
        [SerializeField] private Texture image;
        [SerializeField] private Sprite mask;
        
        [SerializeField] private Vector2 centre = new Vector2(0.5f, 0.5f);
        [SerializeField] private float size;
        
        internal CropInfo Generate()
        {
            CropInfo info = ScriptableObject.CreateInstance<CropInfo>();
            info.Apply(rawImage.texture, rawImage.uvRect);

            return info;
        }

        internal void UpdateParameters()
        {
            rawImage.texture = image;
            maskImage.sprite = mask;

            Vector2 newPosition = centre - Vector2.one * (0.5f * size);
            Rect uvRect = new Rect(newPosition.x, newPosition.y, size, size);
            rawImage.uvRect = uvRect;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Rect rawImageRect = rawImage.rectTransform.rect;
            Vector2 scale = new Vector2(rawImage.rectTransform.rect.width, rawImage.rectTransform.rect.height);
            rawImageRect.center += (Vector2) rawImage.rectTransform.position;
            rawImageRect.center -= Vector2.Scale(centre, scale) / size;
            rawImageRect.center += scale * 0.5f;

            rawImageRect.width /= size;
            rawImageRect.height /= size;

            Vector3[] imageRectCorners =
            {
                new Vector3(rawImageRect.xMin, rawImageRect.yMin),
                new Vector3(rawImageRect.xMin, rawImageRect.yMax),
                new Vector3(rawImageRect.xMax, rawImageRect.yMax),
                new Vector3(rawImageRect.xMax, rawImageRect.yMin)
            };
            
            Handles.DrawDottedLines(imageRectCorners, new[] {0, 1, 1, 2, 2, 3, 3, 0}, 3f);

            
            Vector3[] maskCorners = new Vector3[4];
            maskImage.rectTransform.GetWorldCorners(maskCorners);
            
            Handles.DrawLines(maskCorners, new [] {0, 1, 1, 2, 2, 3, 3, 0});
        }
#endif
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(CropGenerator))]
    public class CropGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty rawImage;
        private SerializedProperty maskImage;
        
        private SerializedProperty image;
        private SerializedProperty mask;
        
        private SerializedProperty centre;
        private SerializedProperty size;
        
        private string fileName = "";
        private bool foldout;

        private const string subDirectory = "/ScriptableObjects/UI/CropInfo/";

        
        private void OnEnable()
        {
            rawImage = serializedObject.FindProperty("rawImage");
            maskImage = serializedObject.FindProperty("maskImage");
            
            image = serializedObject.FindProperty("image");
            mask = serializedObject.FindProperty("mask");
            
            centre = serializedObject.FindProperty("centre");
            size = serializedObject.FindProperty("size");
        }

        public override void OnInspectorGUI()
        {
            if (!(target is CropGenerator t))
                return;
            
            // File name field
            fileName = EditorGUILayout.TextField("File Name", fileName);
            
            // Image
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.ObjectField(image);
            
            // Mask
            EditorGUILayout.ObjectField(mask);
            
            // Offset
            centre.vector2Value = EditorGUILayout.Vector2Field(centre.displayName, centre.vector2Value);
            
            // Size
            size.floatValue = EditorGUILayout.FloatField(size.displayName, size.floatValue);

            bool parametersChanged = EditorGUI.EndChangeCheck();
            
            // Generate button
            string path = Application.dataPath + subDirectory + fileName + ".asset";
            bool fileExists = Directory.Exists(path);
            bool isFileNameValid = fileName != "";
            GUI.enabled = isFileNameValid;
            if (GUILayout.Button("Save"))
            {
                CropInfo info = t.Generate();

                if (info != null)
                {
                    Directory.CreateDirectory(Application.dataPath + subDirectory);
                    AssetDatabase.CreateAsset(info, "Assets" + subDirectory + fileName + ".asset");

                    EditorGUIUtility.PingObject(info);

                    fileName = "";
                }
            }

            GUI.enabled = true;
            
            // Warning help box
            if (fileExists && !isFileNameValid)
                EditorGUILayout.HelpBox("A file with that name already exists.", MessageType.Warning);

            if (foldout = EditorGUILayout.Foldout(foldout, "Settings"))
            {
                EditorGUILayout.ObjectField(rawImage);
                EditorGUILayout.ObjectField(maskImage);
            }

            serializedObject.ApplyModifiedProperties();
            
            if (parametersChanged)
                t.UpdateParameters();
        }
    }
#endif
}
