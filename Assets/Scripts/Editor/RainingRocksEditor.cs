using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RainingRocks))]
public class RainingRocksEditor : Editor
{
    SerializedProperty actionType;
    SerializedProperty sfxType;
    SerializedProperty gravityMultipler;
    SerializedProperty rockTag;
    SerializedProperty cameraShake;

    private void OnEnable()
    {
        actionType = serializedObject.FindProperty("actionType");
        sfxType = serializedObject.FindProperty("sfxType");
        gravityMultipler = serializedObject.FindProperty("gravityMultipler");
        rockTag = serializedObject.FindProperty("rockTag");
        cameraShake = serializedObject.FindProperty("cameraShake");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(actionType);

        RainingRocks.ActionType selectedAction =
            (RainingRocks.ActionType)actionType.enumValueIndex;

        EditorGUILayout.Space();

        if (selectedAction == RainingRocks.ActionType.Sound)
        {
            EditorGUILayout.LabelField("Sound", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(sfxType);
            EditorGUILayout.PropertyField(cameraShake);
        }
        else if (selectedAction == RainingRocks.ActionType.Physics)
        {
            EditorGUILayout.LabelField("Physics", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(gravityMultipler);
            EditorGUILayout.PropertyField(rockTag);
            EditorGUILayout.PropertyField(cameraShake);
        }

        serializedObject.ApplyModifiedProperties();
    }
}