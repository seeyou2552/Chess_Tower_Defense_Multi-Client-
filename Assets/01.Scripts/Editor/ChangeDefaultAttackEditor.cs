using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


public static class ChangeDefaultAttackEditor
{
    private const string DataPath =
        "Assets/06.ScriptableObjects/Skill/ChangeDefaultAttack";

    private const string CSVPath =
        "Assets/CSV/ChangeDefaultAttack.csv";



    #region Export


    [MenuItem("Tools/Change Default Attack/Export CSV")]
    public static void ExportCSV()
    {
        string[] guids =
            AssetDatabase.FindAssets(
                "t:ChangeDefaultAttack",
                new[] { DataPath }
            );


        StringBuilder csv = new();


        csv.AppendLine(
            "ID,DeliveryType,SearchScope," +
            "AtkSprite,AtkAnimClip,AtkSFX," +
            "ProjectileType,ProjectileHitType," +
            "ProjectileSpeed,ProjectileDuration"
        );


        foreach(string guid in guids)
        {
            ChangeDefaultAttack data =
                AssetDatabase.LoadAssetAtPath<ChangeDefaultAttack>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(data == null)
                continue;



            csv.AppendLine(
                $"{data.ID}," +
                $"{data.DeliveryType}," +
                $"{data.SearchScope}," +
                $"{Escape(GetPath(data.AtkSprite))}," +
                $"{Escape(GetPath(data.AtkAnimClip))}," +
                $"{Escape(GetPath(data.AtkSFX))}," +
                $"{data.ProjectileType}," +
                $"{data.ProjectileHitType}," +
                $"{data.ProjectileSpeed}," +
                $"{data.ProjectileDuration}"
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
            $"ChangeDefaultAttack Export Complete : {CSVPath}"
        );
    }


    #endregion




    #region Import


    [MenuItem("Tools/Change Default Attack/Import CSV")]
    public static void ImportCSV()
    {
        if(!File.Exists(CSVPath))
        {
            Debug.LogError(
                "CSV 파일 없음"
            );

            return;
        }



        string[] lines =
            File.ReadAllLines(
                CSVPath,
                Encoding.UTF8
            );



        for(int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i]))
                continue;



            string[] columns =
                ParseCSV(lines[i]);



            if(columns.Length < 10)
            {
                Debug.LogWarning(
                    $"CSV 파싱 실패 Line : {i}, Count : {columns.Length}"
                );

                continue;
            }



            int id =
                int.Parse(columns[0]);



            ChangeDefaultAttack data =
                FindData(id);



            if(data == null)
            {
                Debug.LogWarning(
                    $"ChangeDefaultAttack 없음 ID : {id}"
                );

                continue;
            }



            Enum.TryParse(
                columns[1],
                out AttackDeliveryType deliveryType
            );

            data.DeliveryType =
                deliveryType;



            Enum.TryParse(
                columns[2],
                out SearchScope searchScope
            );

            data.SearchScope =
                searchScope;



            data.AtkSprite =
                LoadAsset<Sprite>(
                    columns[3]
                );


            data.AtkAnimClip =
                LoadAsset<AnimationClip>(
                    columns[4]
                );


            data.AtkSFX =
                LoadAsset<AudioClip>(
                    columns[5]
                );



            Enum.TryParse(
                columns[6],
                out ProjectileType projectileType
            );

            data.ProjectileType =
                projectileType;



            Enum.TryParse(
                columns[7],
                out ProjectileHitType hitType
            );

            data.ProjectileHitType =
                hitType;



            float.TryParse(
                columns[8],
                out float speed
            );

            data.ProjectileSpeed =
                speed;



            float.TryParse(
                columns[9],
                out float duration
            );

            data.ProjectileDuration =
                duration;



            EditorUtility.SetDirty(data);
        }



        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();


        Debug.Log(
            "ChangeDefaultAttack Import Complete"
        );
    }


    #endregion




    #region Utility



    private static ChangeDefaultAttack FindData(int id)
    {
        string[] guids =
            AssetDatabase.FindAssets(
                "t:ChangeDefaultAttack",
                new[] { DataPath }
            );


        foreach(string guid in guids)
        {
            ChangeDefaultAttack data =
                AssetDatabase.LoadAssetAtPath<ChangeDefaultAttack>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(data != null && data.ID == id)
                return data;
        }


        return null;
    }





    private static string GetPath(
        UnityEngine.Object obj)
    {
        if(obj == null)
            return "";

        return AssetDatabase.GetAssetPath(obj);
    }





    private static T LoadAsset<T>(
        string path)
        where T : UnityEngine.Object
    {
        if(string.IsNullOrEmpty(path))
            return null;


        T asset =
            AssetDatabase.LoadAssetAtPath<T>(path);


        if(asset == null)
        {
            Debug.LogWarning(
                $"Asset Load 실패 : {path}"
            );
        }


        return asset;
    }





    private static string Escape(
        string value)
    {
        if(string.IsNullOrEmpty(value))
            return "";


        if(value.Contains(",") ||
           value.Contains("\""))
        {
            value =
                value.Replace("\"", "\"\"");

            return $"\"{value}\"";
        }


        return value;
    }





    private static string[] ParseCSV(
        string line)
    {
        List<string> result = new();

        StringBuilder current = new();

        bool quote = false;


        foreach(char c in line)
        {
            if(c == '"')
            {
                quote = !quote;
                continue;
            }


            if(c == ',' && !quote)
            {
                result.Add(
                    current.ToString()
                );

                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }


        result.Add(
            current.ToString()
        );


        return result.ToArray();
    }


    #endregion
}