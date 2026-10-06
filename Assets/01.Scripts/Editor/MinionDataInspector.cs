using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MinionData))]
public class MinionDataInspector : Editor
{
    private MinionData minionData;

    private void OnEnable()
    {
        minionData = (MinionData)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        if (minionData.Skill == null)
        {
            if (GUILayout.Button("Create SkillData Asset"))
            {
                CreateSkillDataAsset();
            }
        }
        else
        {
            EditorGUILayout.LabelField("Skill Data", minionData.Skill.name);
            if (GUILayout.Button("Select Skill Asset"))
            {
                Selection.activeObject = minionData.Skill;
            }
        }
    }

    private void CreateSkillDataAsset()
    {
        string minionAssetPath = AssetDatabase.GetAssetPath(minionData);
        if (string.IsNullOrEmpty(minionAssetPath))
        {
            Debug.LogWarning("MinionDataInspector: MinionData asset path not found.");
            return;
        }

        string folderPath = System.IO.Path.GetDirectoryName(minionAssetPath);
        if (string.IsNullOrEmpty(folderPath))
        {
            folderPath = "Assets";
        }

        string skillAssetName = minionData.name + "_SkillData.asset";
        string skillAssetPath = System.IO.Path.Combine(folderPath, skillAssetName).Replace("\\", "/");
        skillAssetPath = AssetDatabase.GenerateUniqueAssetPath(skillAssetPath);

        SkillData newSkillData = CreateInstance<SkillData>();
        AssetDatabase.CreateAsset(newSkillData, skillAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        minionData.Skill = newSkillData;
        EditorUtility.SetDirty(minionData);
        AssetDatabase.SaveAssets();

        Selection.activeObject = newSkillData;
    }
}
