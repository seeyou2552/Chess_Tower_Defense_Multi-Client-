using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MinionItemDataGenerator : EditorWindow
{
    private const string DefaultMinionSourceFolder = "Assets/06.ScriptableObject/Minion";
    private const string DefaultMinionItemFolder = "Assets/06.ScriptableObject/Item/Minion";

    private string minionSourceFolder = DefaultMinionSourceFolder;
    private string minionItemFolder = DefaultMinionItemFolder;

    [MenuItem("Tools/Shop/Generate Missing Minion Item Data")]
    public static void ShowWindow()
    {
        GetWindow<MinionItemDataGenerator>("Minion Item Generator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("미니언 아이템 데이터 생성기", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        minionSourceFolder = EditorGUILayout.TextField("Minion Source Folder", minionSourceFolder);
        minionItemFolder = EditorGUILayout.TextField("Minion Item Target Folder", minionItemFolder);

        EditorGUILayout.HelpBox("MinionData 에셋을 참고하여 MinionItemData를 생성합니다. 이미 존재하는 MinionData 연결 아이템은 건너뜁니다.", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("Create Missing MinionItemData Assets"))
        {
            CreateMissingMinionItemData();
        }
    }

    private void CreateMissingMinionItemData()
    {
        string[] minionGuids = AssetDatabase.FindAssets("t:MinionData", new[] { minionSourceFolder });
        string[] itemGuids = AssetDatabase.FindAssets("t:MinionItemData", new[] { minionItemFolder });

        if (minionGuids == null || minionGuids.Length == 0)
        {
            EditorUtility.DisplayDialog("생성 실패", $"Minion 데이터가 '{minionSourceFolder}' 에서 발견되지 않았습니다.", "확인");
            return;
        }

        var existingMinionData = new HashSet<MinionData>();
        foreach (string guid in itemGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MinionItemData itemData = AssetDatabase.LoadAssetAtPath<MinionItemData>(path);
            if (itemData != null && itemData.MinionData != null)
            {
                existingMinionData.Add(itemData.MinionData);
            }
        }

        int createdCount = 0;
        foreach (string guid in minionGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MinionData minionData = AssetDatabase.LoadAssetAtPath<MinionData>(path);
            if (minionData == null || existingMinionData.Contains(minionData))
                continue;

            if (!AssetDatabase.IsValidFolder(minionItemFolder))
            {
                AssetDatabase.CreateFolder("Assets/06.ScriptableObject/Item", "Minion");
            }

            string assetName = string.IsNullOrEmpty(minionData.CharacterName) ? "NewMinionItem" : minionData.CharacterName.Replace(" ", "_");
            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{minionItemFolder}/{assetName}_Item.asset");

            MinionItemData newItem = CreateInstance<MinionItemData>();
            newItem.ItemName = minionData.CharacterName;
            newItem.Description = string.IsNullOrEmpty(minionData.CharacterName)
                ? "미니언 구매 아이템"
                : $"{minionData.CharacterName} 미니언을 구매할 수 있는 아이템입니다.";
            newItem.Icon = minionData.MinionSprite;
            newItem.Price = 0;
            newItem.ItemType = ShopItemType.ChessPiece;
            newItem.MaxPurchaseCount = 1;
            newItem.MinionData = minionData;

            AssetDatabase.CreateAsset(newItem, assetPath);
            createdCount++;
        }

        if (createdCount > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("완료", $"{createdCount}개의 MinionItemData 에셋을 생성했습니다.", "확인");
        }
        else
        {
            EditorUtility.DisplayDialog("완료", "새로 생성할 MinionItemData가 없습니다.", "확인");
        }
    }
}
