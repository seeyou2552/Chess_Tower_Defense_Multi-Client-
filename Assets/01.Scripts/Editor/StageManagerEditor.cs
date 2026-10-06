using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageManager))]
public class StageManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스팩터 그리기
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        EditorGUILayout.Space();

        StageManager stageManager = (StageManager)target;

        EditorGUILayout.LabelField("Stage Data 관리", EditorStyles.boldLabel);
        
        if (GUILayout.Button("📁 Load Stage Data from Folder", GUILayout.Height(35)))
        {
            StageDataLoader.LoadStagesToManagerPublic(stageManager);
        }

        EditorGUILayout.HelpBox($"현재 로드된 Stage: {stageManager.Stages.Count}개", MessageType.Info);
    }
}
