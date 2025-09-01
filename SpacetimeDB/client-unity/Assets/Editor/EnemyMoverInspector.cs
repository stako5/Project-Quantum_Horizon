using UnityEditor;
using UnityEngine;

namespace MMORPG.Client.EditorTools
{
    [CustomEditor(typeof(MMORPG.Client.Enemies.EnemyMover))]
    public class EnemyMoverInspector : Editor
    {
        SerializedProperty baseSpeed;
        SerializedProperty rotationSpeed;
        SerializedProperty stoppingDistance;
        SerializedProperty leashRadius;
        SerializedProperty home;

        void OnEnable()
        {
            baseSpeed = serializedObject.FindProperty("baseSpeed");
            rotationSpeed = serializedObject.FindProperty("rotationSpeed");
            stoppingDistance = serializedObject.FindProperty("stoppingDistance");
            leashRadius = serializedObject.FindProperty("leashRadius");
            home = serializedObject.FindProperty("home");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(baseSpeed);
            EditorGUILayout.PropertyField(rotationSpeed);
            EditorGUILayout.PropertyField(stoppingDistance);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Leash & Home", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(leashRadius);
            EditorGUILayout.PropertyField(home);

            var mover = (MMORPG.Client.Enemies.EnemyMover)target;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Home = Current"))
            {
                Undo.RecordObject(mover, "Set Home Position");
                mover.home = mover.transform.position;
                EditorUtility.SetDirty(mover);
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("Move To Home"))
            {
                Undo.RecordObject(mover.transform, "Move To Home");
                mover.transform.position = mover.home;
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

