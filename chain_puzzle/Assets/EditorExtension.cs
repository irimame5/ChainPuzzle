using System;
using System.Linq;
using System.IO;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public static class EditorExtension
{
    static Vector3 vector3;//tmp用
    #region Transform拡張
    public static void SetX(this Transform transform, float x)
        {
            vector3.Set(x, transform.position.y, transform.position.z);
            transform.position = vector3;
        }
        public static void SetY(this Transform transform, float y)
        {
            vector3.Set(transform.position.x, y, transform.position.z);
            transform.position = vector3;
        }
        public static void SetZ(this Transform transform, float z)
        {
            vector3.Set(transform.position.x, transform.position.y, z);
            transform.position = vector3;
        }
        public static void SetLocalX(this Transform transform, float x)
        {
            vector3.Set(x, transform.localPosition.y, transform.localPosition.z);
            transform.localPosition = vector3;
        }
        public static void SetLocalY(this Transform transform, float y)
        {
            vector3.Set(transform.localPosition.x, y, transform.localPosition.z);
            transform.localPosition = vector3;
        }
        public static void SetLocalZ(this Transform transform, float z)
        {
            vector3.Set(transform.localPosition.x, transform.localPosition.y, z);
            transform.localPosition = vector3;
        }
    #endregion

    public static void SetWidth(this SpriteRenderer spriteRenderer,float width)
    {
        vector3.Set(width,spriteRenderer.size.y,0);
        spriteRenderer.size = vector3;
    }
    public static void SetHeight(this SpriteRenderer spriteRenderer, float height)
    {
        vector3.Set(spriteRenderer.size.x,height, 0);
        spriteRenderer.size = vector3;
    }

    /// <summary>
    /// OnDrawGizmos内でしか呼び出せない
    /// </summary>
    /// <param name="from">始点</param>
    /// <param name="direction">方向と長さ</param>
    public static void DrawAllow(Vector3 from, Vector3 direction)
    {
        const float AllowTopLength = 0.3f;
        const float AllowRadius = 20f;

        var toPosition = from + direction;

        var tmp = Quaternion.Euler(0, 0, AllowRadius) * -direction;
        tmp = tmp.normalized * AllowTopLength;
        Gizmos.DrawRay(toPosition, tmp);

        tmp = Quaternion.Euler(0, 0, -AllowRadius) * -direction;
        tmp = tmp.normalized * AllowTopLength;
        Gizmos.DrawRay(toPosition, tmp);

        Gizmos.DrawRay(from, direction);
    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/MyGame/Create Text File")]
    static void CreateTextFile()
    {
        string createPath = GetCurrentDirectory()+@"/newText.txt";
        var streamWriter = new StreamWriter(createPath);
        streamWriter.Close();
        AssetDatabase.Refresh();
    }
    static string GetCurrentDirectory()
    {
        var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        var asm = Assembly.Load("UnityEditor.dll");
        var typeProjectBrowser = asm.GetType("UnityEditor.ProjectBrowser");
        var projectBrowserWindow = EditorWindow.GetWindow(typeProjectBrowser);
        return (string)typeProjectBrowser.GetMethod("GetActiveFolderPath", flag).Invoke(projectBrowserWindow, null);
    }

    [MenuItem("MyGame/NextScene %KP.")]
    public static void LoadNextScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) { return; }//シーンが保存できなきゃやめる

        int currentSceneIndex = EditorSceneManager.GetActiveScene().buildIndex;
        var scenes = EditorBuildSettings.scenes;
        int nextSceneIndex;
        if (scenes.Length <= currentSceneIndex + 1)
        {
            nextSceneIndex = 0;
        }else
        {
            nextSceneIndex = currentSceneIndex + 1;
        }
        EditorSceneManager.OpenScene(scenes[nextSceneIndex].path);
    }
    [MenuItem("MyGame/BackScene %KP,")]
    public static void LoadBackScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) { return; }//シーンが保存できなきゃやめる

        int currentSceneIndex = EditorSceneManager.GetActiveScene().buildIndex;
        var scenes = EditorBuildSettings.scenes;
        int nextSceneIndex;
        if (currentSceneIndex - 1 < 0)
        {
            nextSceneIndex = scenes.Length - 1;
        }
        else
        {
            nextSceneIndex = currentSceneIndex - 1;
        }
        EditorSceneManager.OpenScene(scenes[nextSceneIndex].path);
    }

#endif
}

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
public sealed class EnumFlagsAttribute : PropertyAttribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class DisableAttribute : PropertyAttribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class SceneNameAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( EnumFlagsAttribute ) )]
public sealed class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position,
        SerializedProperty prop,
        GUIContent label
    )
    {
        prop.intValue = EditorGUI.MaskField(
            position,
            label,
            prop.intValue,
            prop.enumNames
        );
    }
}

[CustomPropertyDrawer(typeof(DisableAttribute))]
public sealed class DisableAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position,SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label);
        EditorGUI.EndDisabledGroup();
    }
}

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public sealed class SceneNameAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position,SerializedProperty property, GUIContent label)
    {
        var scenePath = EditorBuildSettings.scenes.Select(x => x.path).ToArray();
        if (scenePath.Length == 0)
        {
            EditorGUI.PropertyField(position, property, label);
        }

        var sceneNames = scenePath.Select(x => x.Split('/').Last().Split('.').First()).ToArray();

        if (property.propertyType == SerializedPropertyType.String)
        {
            int currentIndex = Array.IndexOf(sceneNames, property.stringValue);

            var nextIndex = EditorGUI.Popup(position,label.text , currentIndex, sceneNames);
            if (nextIndex == -1)
            {
                property.stringValue = "";
                return;
            }
            property.stringValue = sceneNames[nextIndex];
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}

#endif