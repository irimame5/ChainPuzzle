using System;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif 

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
            }else
            {
                property.stringValue = sceneNames[nextIndex];
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}

#endif