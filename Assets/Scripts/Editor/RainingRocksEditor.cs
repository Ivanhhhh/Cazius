
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RainingRocks))]
public class RainingRocksEditor : Editor
{
    private SerializedProperty actionType;
    private SerializedProperty sfxType;
    private SerializedProperty animators;

    private SerializedProperty initialShakeStrength;
    private SerializedProperty initialShakeDuration;

    private SerializedProperty impactShakeStrength;
    private SerializedProperty impactShakeDuration;

    private SerializedProperty impactDustParticles;

    private void OnEnable()
    {
        actionType = serializedObject.FindProperty("actionType");
        sfxType = serializedObject.FindProperty("sfxType");
        animators = serializedObject.FindProperty("animators");

        initialShakeStrength =
            serializedObject.FindProperty("_initialShakeStrength");

        initialShakeDuration =
            serializedObject.FindProperty("_initialShakeDuration");

        impactShakeStrength =
            serializedObject.FindProperty("_impactShakeStrength");

        impactShakeDuration =
            serializedObject.FindProperty("_impactShakeDuration");

        impactDustParticles =
            serializedObject.FindProperty("_impactDustParticles");
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
            EditorGUILayout.LabelField(
                "Sound",
                EditorStyles.boldLabel
            );

            EditorGUILayout.PropertyField(sfxType);

            DrawInitialShake();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Animators",
                EditorStyles.boldLabel
            );

            EditorGUILayout.PropertyField(
                animators,
                new GUIContent("Animators"),
                true
            );

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Destruction Particles",
                EditorStyles.boldLabel
            );

            EditorGUILayout.PropertyField(
                impactDustParticles,
                new GUIContent("Impact Dust Particles"),
                true
            );
        }
        else if (selectedAction == RainingRocks.ActionType.Animator)
        {
            EditorGUILayout.LabelField(
                "Animators",
                EditorStyles.boldLabel
            );

            EditorGUILayout.PropertyField(
                animators,
                new GUIContent("Animators"),
                true
            );

            DrawInitialShake();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Impact Shake",
                EditorStyles.boldLabel
            );

            EditorGUILayout.PropertyField(impactShakeStrength);
            EditorGUILayout.PropertyField(impactShakeDuration);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Sound",
                EditorStyles.boldLabel
            );

            EditorGUILayout.PropertyField(sfxType);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Destruction Particles",
                EditorStyles.boldLabel
            );

            EditorGUILayout.PropertyField(
                impactDustParticles,
                new GUIContent("Impact Dust Particles"),
                true
            );
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawInitialShake()
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Initial Shake",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(initialShakeStrength);
        EditorGUILayout.PropertyField(initialShakeDuration);
    }
}
