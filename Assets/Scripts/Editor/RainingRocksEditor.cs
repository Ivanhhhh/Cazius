using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RainingRocks))]
public class RainingRocksEditor : Editor
{
    SerializedProperty actionType;
    SerializedProperty sfxType;
    SerializedProperty animator;
    SerializedProperty animationTrigger;

    private void OnEnable()
    {
        actionType = serializedObject.FindProperty("actionType");
        sfxType = serializedObject.FindProperty("sfxType");
        animator = serializedObject.FindProperty("animator");
        animationTrigger = serializedObject.FindProperty("animationTrigger");
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
        }
        else if (selectedAction == RainingRocks.ActionType.Animation)
        {
            EditorGUILayout.LabelField("Animator", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(animator);
            EditorGUILayout.PropertyField(animationTrigger);
        }

        serializedObject.ApplyModifiedProperties();
    }
}