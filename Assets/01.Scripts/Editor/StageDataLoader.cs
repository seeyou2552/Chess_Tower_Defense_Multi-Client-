using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageDataLoader : EditorWindow
{
    private const string StageDataFolder = "Assets/06.ScriptableObject/Stage";
    private bool sortByName = true;

    [MenuItem("Tools/Stage/Load Stage Data to Manager")]
    public static void LoadStageData()
    {
        LoadStagesToManager();
    }

    /// <summary>
    /// StageManager 인스턴스에서 호출할 수 있는 공개 메서드
    /// </summary>
    public static void LoadStagesToManagerPublic(StageManager stageManager)
    {
        LoadStagesToManager(stageManager);
    }

    [MenuItem("Tools/Stage/Open Stage Data Loader Window")]
    public static void ShowWindow()
    {
        GetWindow<StageDataLoader>("Stage Data Loader");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Stage Data 로더", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sortByName = EditorGUILayout.Toggle("정렬 기준 (이름순)", sortByName);
        
        EditorGUILayout.HelpBox($"'{StageDataFolder}' 폴더에서 모든 StageData를 찾아 StageManager의 Stages 리스트에 채웁니다.", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Load Stage Data", GUILayout.Height(40)))
        {
            LoadStagesToManager();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("이 작업은 StageManager 프리팹 또는 씬에 있는 인스턴스를 자동으로 감지합니다.", MessageType.Info);
    }

    private static void LoadStagesToManager()
    {
        // StageManager 찾기
        StageManager stageManager = FindStageManager();
        if (stageManager == null)
        {
            EditorUtility.DisplayDialog("오류", "씬에서 StageManager를 찾을 수 없습니다.\nStageManager가 로드된 씬에서 실행하세요.", "확인");
            return;
        }

        LoadStagesToManager(stageManager);
    }

    private static void LoadStagesToManager(StageManager stageManager)
    {
        // Stage 폴더에서 모든 StageData 찾기
        string[] stageGuids = AssetDatabase.FindAssets("t:StageData", new[] { StageDataFolder });

        if (stageGuids == null || stageGuids.Length == 0)
        {
            EditorUtility.DisplayDialog("오류", $"'{StageDataFolder}' 폴더에서 StageData를 찾을 수 없습니다.", "확인");
            return;
        }

        // 경로와 StageData를 매칭
        var stageDictionary = new Dictionary<string, StageData>();
        foreach (string guid in stageGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            StageData stageData = AssetDatabase.LoadAssetAtPath<StageData>(path);
            if (stageData != null)
            {
                stageDictionary[path] = stageData;
            }
        }

        // 경로 기준으로 정렬 (Stage1, Stage2, ... 순서)
        var sortedPaths = new List<string>(stageDictionary.Keys);
        sortedPaths.Sort();

        // StageManager의 stages 리스트 초기화 및 채우기
        stageManager.Stages.Clear();
        foreach (string path in sortedPaths)
        {
            stageManager.Stages.Add(stageDictionary[path]);
        }

        // 변경 내용 저장
        EditorUtility.SetDirty(stageManager);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("완료", $"{stageManager.Stages.Count}개의 Stage Data가 로드되었습니다.", "확인");
        Debug.Log($"[StageDataLoader] {stageManager.Stages.Count}개의 Stage Data를 StageManager에 로드했습니다.");
    }

    private static StageManager FindStageManager()
    {
        // 현재 씬에 로드된 StageManager 찾기
        StageManager stageManager = FindObjectOfType<StageManager>();
        return stageManager;
    }
}
