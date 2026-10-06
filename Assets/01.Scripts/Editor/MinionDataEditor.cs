using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class MinionDataEditor
{
    private const string DataPath =
        "Assets/06.ScriptableObjects/Minion";

    private const string ItemDataPath =
        "Assets/06.ScriptableObjects/Item/Minion/MinionItem";

    private const string CSVPath =
        "Assets/CSV/MinionData.csv";


    #region Export

    [MenuItem("Tools/Minion Data/Export CSV")]
    public static void ExportCSV()
    {
        string[] guids =
            AssetDatabase.FindAssets(
                "t:MinionData",
                new[] { DataPath }
            );


        List<MinionData> list =
            new();


        foreach(string guid in guids)
        {
            MinionData data =
                AssetDatabase.LoadAssetAtPath<MinionData>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(data != null)
                list.Add(data);
        }


        list.Sort((a, b) =>
            a.MinionIndex.CompareTo(b.MinionIndex)
        );


        StringBuilder csv =
            new();


        csv.AppendLine(
            "MinionIndex," +
            "CharacterName," +
            "MinionSpritePath," +
            "MinionSpriteName," +
            "MinionAnim," +
            "MaxAC," +
            "AttackRange," +
            "AttackPower," +
            "AttackCooldown," +
            "Cost," +
            "SkillID," +
            "UpgradeData," +
            "DeliveryType," +
            "SearchScope," +
            "MaxTargets," +
            "AtkSpritePath," +
            "AtkSpriteName," +
            "AtkAnimClip," +
            "AtkSFX," +
            "ProjectileType," +
            "ProjectileHitType," +
            "ProjectileSpeed," +
            "ProjectileDuration," +
            "MinionType"
        );


        foreach(MinionData data in list)
        {
            csv.AppendLine(
                string.Join(",",
                    data.MinionIndex,

                    Escape(data.CharacterName),

                    Escape(
                        GetPath(data.MinionSprite)
                    ),

                    Escape(
                        data.MinionSprite != null
                            ? data.MinionSprite.name
                            : ""
                    ),

                    Escape(
                        GetPath(data.MinionAnim)
                    ),

                    data.MaxAC,
                    data.AttackRange,
                    data.AttackPower,
                    data.AttackCooldown,
                    data.Cost,

                    data.Skill != null
                        ? data.Skill.ID
                        : 0,

                    Escape(
                        SerializeUpgrade(
                            data.Upgrades
                        )
                    ),

                    data.DeliveryType,
                    data.SearchScope,
                    data.MaxTargets,


                    Escape(
                        GetPath(data.AtkSprite)
                    ),

                    Escape(
                        data.AtkSprite != null
                            ? data.AtkSprite.name
                            : ""
                    ),


                    Escape(
                        GetPath(data.AtkAnimClip)
                    ),

                    Escape(
                        GetPath(data.AtkSFX)
                    ),

                    data.ProjectileType,
                    data.ProjectileHitType,
                    data.ProjectileSpeed,
                    data.ProjectileDuration,
                    data.MinionType
                )
            );
        }


        Directory.CreateDirectory(
            System.IO.Path.GetDirectoryName(CSVPath)
        );


        File.WriteAllText(
            CSVPath,
            csv.ToString(),
            Encoding.UTF8
        );


        AssetDatabase.Refresh();


        Debug.Log(
            $"Minion Export Complete : {CSVPath}"
        );
    }

    #endregion



    #region Import


    [MenuItem("Tools/Minion Data/Import CSV")]
    public static void ImportCSV()
    {
        if(!File.Exists(CSVPath))
        {
            Debug.LogError(
                "CSV 파일 없음"
            );

            return;
        }


        List<string[]> rows =
            ParseCSV(
                File.ReadAllText(
                    CSVPath,
                    Encoding.UTF8
                )
            );


        Dictionary<int, SkillData> skillMap =
            LoadSkillMap();


        Dictionary<int, UpgradeData> upgradeMap =
            LoadUpgradeMap();



        for(int i = 1; i < rows.Count; i++)
        {
            string[] c =
                rows[i];


            if(c.Length != 24)
            {
                Debug.LogWarning(
                    $"CSV Column Error Line:{i} Count:{c.Length}"
                );

                continue;
            }


            if(!int.TryParse(
                c[0],
                out int index))
            {
                continue;
            }


            MinionData data =
                FindMinion(index);



            if(data == null)
            {
                data =
                    ScriptableObject.CreateInstance<MinionData>();


                data.MinionIndex =
                    index;


                Directory.CreateDirectory(
                    DataPath
                );


                AssetDatabase.CreateAsset(
                    data,
                    $"{DataPath}/Minion_{index}.asset"
                );


                Debug.Log(
                    $"Minion 생성 : {index}"
                );
            }


            data.CharacterName =
                c[1];


            data.MinionSprite =
                LoadSprite(
                    c[2],
                    c[3]
                );


            data.MinionAnim =
                LoadAsset<AnimationClip>(
                    c[4]
                );


            int.TryParse(
                c[5],
                out data.MaxAC
            );


            float.TryParse(
                c[6],
                out data.AttackRange
            );


            int.TryParse(
                c[7],
                out data.AttackPower
            );


            float.TryParse(
                c[8],
                out data.AttackCooldown
            );


            int.TryParse(
                c[9],
                out data.Cost
            );


            if(int.TryParse(
                c[10],
                out int skillID))
            {
                skillMap.TryGetValue(
                    skillID,
                    out data.Skill
                );
            }


            data.Upgrades =
                ParseUpgrade(
                    c[11],
                    upgradeMap
                );

                            Enum.TryParse(
                c[12],
                out data.DeliveryType
            );


            Enum.TryParse(
                c[13],
                out data.SearchScope
            );


            int.TryParse(
                c[14],
                out data.MaxTargets
            );


            data.AtkSprite =
                LoadSprite(
                    c[15],
                    c[16]
                );


            data.AtkAnimClip =
                LoadAsset<AnimationClip>(
                    c[17]
                );


            data.AtkSFX =
                LoadAsset<AudioClip>(
                    c[18]
                );


            Enum.TryParse(
                c[19],
                out data.ProjectileType
            );


            Enum.TryParse(
                c[20],
                out data.ProjectileHitType
            );


            float.TryParse(
                c[21],
                out data.ProjectileSpeed
            );


            float.TryParse(
                c[22],
                out data.ProjectileDuration
            );


            Enum.TryParse(
                c[23],
                out data.MinionType
            );


            EditorUtility.SetDirty(data);


            CreateOrUpdateMinionItem(
                data
            );
        }


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        Debug.Log(
            "Minion Import Complete"
        );
    }


    #endregion



    #region Upgrade


    private static string SerializeUpgrade(
        Upgrade[] upgrades)
    {
        if(upgrades == null)
            return "";


        List<string> result =
            new();


        foreach(Upgrade upgrade in upgrades)
        {
            if(upgrade == null ||
               upgrade.UpgradeData == null)
            {
                continue;
            }


            result.Add(
                $"{upgrade.UpgradeData.ID}|" +
                $"{upgrade.MaxLevel}|" +
                $"{upgrade.Increase}|" +
                $"{upgrade.Cost}"
            );
        }


        return string.Join(
            ";",
            result
        );
    }



    private static Upgrade[] ParseUpgrade(
        string value,
        Dictionary<int, UpgradeData> map)
    {
        Upgrade[] result =
            new Upgrade[3];


        if(string.IsNullOrEmpty(value))
            return result;



        string[] upgrades =
            value.Split(';');


        for(int i = 0;
            i < upgrades.Length && i < 3;
            i++)
        {
            string[] data =
                upgrades[i].Split('|');


            if(data.Length < 4)
                continue;



            if(!int.TryParse(
                data[0],
                out int id))
            {
                continue;
            }



            if(!map.TryGetValue(
                id,
                out UpgradeData upgradeData))
            {
                continue;
            }



            Upgrade upgrade =
                new();


            upgrade.UpgradeData =
                upgradeData;


            int.TryParse(
                data[1],
                out upgrade.MaxLevel
            );


            float.TryParse(
                data[2],
                out upgrade.Increase
            );


            int.TryParse(
                data[3],
                out upgrade.Cost
            );


            result[i] =
                upgrade;
        }


        return result;
    }



    private static Dictionary<int, UpgradeData> LoadUpgradeMap()
    {
        Dictionary<int, UpgradeData> map =
            new();


        string[] guids =
            AssetDatabase.FindAssets(
                "t:UpgradeData"
            );


        foreach(string guid in guids)
        {
            UpgradeData data =
                AssetDatabase.LoadAssetAtPath<UpgradeData>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(data != null)
            {
                map[data.ID] =
                    data;
            }
        }


        return map;
    }


    #endregion



    #region Minion Item


    private static void CreateOrUpdateMinionItem(
        MinionData minion)
    {
        if(minion == null)
            return;


        Directory.CreateDirectory(
            ItemDataPath
        );


        string[] guids =
            AssetDatabase.FindAssets(
                "t:MinionItemData"
            );


        foreach(string guid in guids)
        {
            MinionItemData item =
                AssetDatabase.LoadAssetAtPath<MinionItemData>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(item == null)
                continue;



            if(item.MinionData == minion)
            {
                UpdateItemData(
                    item,
                    minion
                );


                EditorUtility.SetDirty(item);


                Debug.Log(
                    $"MinionItem Update : {item.ItemIndex}"
                );


                return;
            }
        }



        MinionItemData newItem =
            ScriptableObject.CreateInstance<MinionItemData>();


        newItem.ItemIndex =
            GetNextItemIndex();



        UpdateItemData(
            newItem,
            minion
        );



        string path =
            $"{ItemDataPath}/MinionItem_{newItem.ItemIndex}.asset";



        AssetDatabase.CreateAsset(
            newItem,
            path
        );


        Debug.Log(
            $"MinionItem 생성 : {path}"
        );
    }



    private static void UpdateItemData(
        MinionItemData item,
        MinionData minion)
    {
        item.ItemName =
            minion.CharacterName;


        item.Description =
            $"{minion.CharacterName} 구매";


        item.Icon =
            minion.MinionSprite;


        item.Price =
            minion.Cost;


        item.MinionData =
            minion;
    }



    private static int GetNextItemIndex()
    {
        int maxID = 0;


        string[] guids =
            AssetDatabase.FindAssets(
                "t:ItemData"
            );



        foreach(string guid in guids)
        {
            ItemData item =
                AssetDatabase.LoadAssetAtPath<ItemData>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(item == null)
                continue;



            maxID =
                Mathf.Max(
                    maxID,
                    item.ItemIndex
                );
        }


        return maxID + 1;
    }


    #endregion
        #region Utility


    private static MinionData FindMinion(
        int index)
    {
        string[] guids =
            AssetDatabase.FindAssets(
                "t:MinionData"
            );


        foreach(string guid in guids)
        {
            MinionData data =
                AssetDatabase.LoadAssetAtPath<MinionData>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(data != null &&
               data.MinionIndex == index)
            {
                return data;
            }
        }


        return null;
    }



    private static Dictionary<int, SkillData> LoadSkillMap()
    {
        Dictionary<int, SkillData> map =
            new();


        string[] guids =
            AssetDatabase.FindAssets(
                "t:SkillData"
            );


        foreach(string guid in guids)
        {
            SkillData data =
                AssetDatabase.LoadAssetAtPath<SkillData>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(data != null)
            {
                map[data.ID] =
                    data;
            }
        }


        return map;
    }



    private static T LoadAsset<T>(
        string path)
        where T : UnityEngine.Object
    {
        if(string.IsNullOrEmpty(path))
            return null;



        T asset =
            AssetDatabase.LoadAssetAtPath<T>(
                path
            );


        if(asset != null)
            return asset;



        UnityEngine.Object[] objects =
            AssetDatabase.LoadAllAssetsAtPath(
                path
            );


        foreach(UnityEngine.Object obj in objects)
        {
            if(obj is T)
                return obj as T;
        }



        Debug.LogWarning(
            $"Asset Load 실패 : {path}"
        );


        return null;
    }



    /// <summary>
    /// Multiple Sprite 대응
    /// Texture 안의 Sub Asset Sprite 검색
    /// </summary>
    private static Sprite LoadSprite(
        string path,
        string spriteName)
    {
        if(string.IsNullOrEmpty(path))
            return null;



        UnityEngine.Object[] assets =
            AssetDatabase.LoadAllAssetsAtPath(
                path
            );



        foreach(UnityEngine.Object asset in assets)
        {
            if(asset is Sprite sprite &&
               sprite.name == spriteName)
            {
                return sprite;
            }
        }



        Debug.LogWarning(
            $"Sprite Load 실패 : {path}/{spriteName}"
        );


        return null;
    }



    private static string GetPath(
        UnityEngine.Object obj)
    {
        if(obj == null)
            return "";


        return AssetDatabase.GetAssetPath(obj);
    }



    private static string Escape(
        string value)
    {
        if(string.IsNullOrEmpty(value))
            return "";


        if(value.Contains(",") ||
           value.Contains("\"") ||
           value.Contains("\n"))
        {
            value =
                value.Replace(
                    "\"",
                    "\"\""
                );


            return $"\"{value}\"";
        }


        return value;
    }



    #endregion



    #region CSV Parser


    private static List<string[]> ParseCSV(
        string text)
    {
        List<string[]> rows =
            new();


        List<string> columns =
            new();


        StringBuilder field =
            new();


        bool quote =
            false;



        for(int i = 0; i < text.Length; i++)
        {
            char c =
                text[i];



            if(c == '"')
            {
                if(quote &&
                   i + 1 < text.Length &&
                   text[i + 1] == '"')
                {
                    field.Append('"');
                    i++;
                }
                else
                {
                    quote =
                        !quote;
                }


                continue;
            }



            if(c == ',' &&
               !quote)
            {
                columns.Add(
                    field.ToString()
                );


                field.Clear();


                continue;
            }



            if((c == '\n' ||
                c == '\r') &&
               !quote)
            {
                if(c == '\r' &&
                   i + 1 < text.Length &&
                   text[i + 1] == '\n')
                {
                    i++;
                }


                columns.Add(
                    field.ToString()
                );


                field.Clear();



                if(columns.Count > 1 ||
                   columns[0] != "")
                {
                    rows.Add(
                        columns.ToArray()
                    );
                }



                columns.Clear();


                continue;
            }



            field.Append(c);
        }



        if(field.Length > 0 ||
           columns.Count > 0)
        {
            columns.Add(
                field.ToString()
            );


            rows.Add(
                columns.ToArray()
            );
        }



        return rows;
    }


    #endregion
}