using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MinionDataManager))]
public class MinionDataEditorManager : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MinionDataManager db = (MinionDataManager)target;

        if (GUILayout.Button("Minion 데이터 자동 채우기"))
        {
            LoadAllMinions(db);
        }
    }

    void LoadAllMinions(MinionDataManager db)
    {
        db.MinionDatas = new List<MinionData>();

        string[] guids = AssetDatabase.FindAssets("t:MinionData", new[] { "Assets/06.ScriptableObjects/Minion" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MinionData data = AssetDatabase.LoadAssetAtPath<MinionData>(path);

            db.MinionDatas.Add(data);
        }

        EditorUtility.SetDirty(db);
        Debug.Log("Minion 데이터 자동 등록 완료");
    }
}