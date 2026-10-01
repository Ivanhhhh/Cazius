using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RainingRocks))]
public class RainingRocksEditor : Editor
{
    SerializedProperty actionType;
    SerializedProperty sfxType;
    SerializedProperty gravityMultiplier;
    SerializedProperty rockTag;
    SerializedProperty timeBetweenRocks;

    SerializedProperty shakeStrength;


    private void OnEnable()
    {
        actionType = serializedObject.FindProperty("actionType");
        sfxType = serializedObject.FindProperty("sfxType");
        gravityMultiplier = serializedObject.FindProperty("gravityMultiplier");
        rockTag = serializedObject.FindProperty("rockTag");
        timeBetweenRocks = serializedObject.FindProperty("timeBetweenRocks");
        shakeStrength = serializedObject.FindProperty("shakeStrength");
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
            EditorGUILayout.PropertyField(shakeStrength);
        }
        else if (selectedAction == RainingRocks.ActionType.Physics)
        {
            EditorGUILayout.LabelField("Physics", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(sfxType);
            EditorGUILayout.PropertyField(gravityMultiplier);
            EditorGUILayout.PropertyField(rockTag);
            EditorGUILayout.PropertyField(timeBetweenRocks);
        }

        serializedObject.ApplyModifiedProperties();
    }
}