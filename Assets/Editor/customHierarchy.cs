using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[UnityEditor.InitializeOnLoad]
#endif
public class customHierarchy
{

    private static Vector2 offset = new Vector2(20, 1);

    static customHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {

        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj != null)
        {
            Color backgroundColor = Color.white;
            Color textColor = Color.white;
            Texture2D texture = null;

            //add conditions
            // obj.name -- obj,tag
            
            //if (obj.name == "Player")
            //{
            //    backgroundColor = new Color(0.2f, 0.6f, 0.1f);
            //    textColor = new Color(0.9f, 0.9f, 0.9f);
            //}
            
            if ((obj as GameObject).GetComponent<ColorInHierarchy>())
            {
                // CREATE A C# SCRIPT "ColorInHierarchy" with a public color variable
                backgroundColor = (obj as GameObject).GetComponent<ColorInHierarchy>().color;
                textColor = new Color(0.9f, 0.9f, 0.9f);
            }

            if (backgroundColor != Color.white)
            {
                Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
                Rect bgRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.width + 50, selectionRect.height);

                EditorGUI.DrawRect(bgRect, backgroundColor);
                EditorGUI.LabelField(offsetRect, obj.name, new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = textColor },
                    fontStyle = FontStyle.Bold
                }
                );

                if (texture != null)
                    EditorGUI.DrawPreviewTexture(new Rect(selectionRect.position, new Vector2(selectionRect.height, selectionRect.height)), texture);
            }
        }
    }
}