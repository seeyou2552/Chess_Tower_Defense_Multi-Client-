using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


public static class UpgradeDataEditor
{
    private const string DataPath = "Assets/06.ScriptableObjects/Skill/Upgrade";
    private const string CSVPath = "Assets/CSV/UpgradeData.csv";


    #region Export

    [MenuItem("Tools/Upgrade Data/Export CSV")]
    public static void ExportCSV()
    {
        string[] guids = AssetDatabase.FindAssets(
            "t:UpgradeData",
            new[] { DataPath }
        );

        StringBuilder csv = new();

        csv.AppendLine(
            "AssetName,ID,Type,UpgradeName,Description,IconPath,SpriteName"
        );


        foreach (string guid in guids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);


            UpgradeData data =
                AssetDatabase.LoadAssetAtPath<UpgradeData>(path);


            if (data == null)
                continue;


            csv.AppendLine(
                $"{Escape(data.name)}," +
                $"{Escape(data.ID.ToString())}," +
                $"{Escape(data.GetType().Name)}," +
                $"{Escape(data.UpgradeName)}," +
                $"{Escape(data.UpgradeDescription)}," +
                $"{Escape(GetIconPath(data.UpgradeIcon))}," +
                $"{Escape(GetSpriteName(data.UpgradeIcon))}"
            );
        }


        CreateDirectory();


        File.WriteAllText(
            CSVPath,
            csv.ToString(),
            new UTF8Encoding(true)
        );


        AssetDatabase.Refresh();


        Debug.Log(
            $"Upgrade CSV Export Complete : {CSVPath}"
        );
    }

    #endregion



    #region Import

    [MenuItem("Tools/Upgrade Data/Import CSV")]
    public static void ImportCSV()
    {
        if (!File.Exists(CSVPath))
        {
            Debug.LogError("CSV 파일이 없습니다.");
            return;
        }


        string csvText =
            File.ReadAllText(
                CSVPath,
                Encoding.UTF8
            );


        csvText =
            csvText.Trim('\uFEFF');


        List<string> lines =
            ParseCSVLines(csvText);



        if(lines.Count <= 1)
            return;



        Dictionary<string, Type> typeMap =
            GetUpgradeTypeMap();



        for(int i = 1; i < lines.Count; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i]))
                continue;


            string[] columns =
                ParseCSV(lines[i]);



            if(columns.Length < 7)
            {
                Debug.LogWarning(
                    $"Line {i} 파싱 실패 : {columns.Length}개\n{lines[i]}"
                );

                continue;
            }



            string assetName =
                columns[0].Trim();


            int.TryParse(
                columns[1],
                out int id
            );


            string typeName =
                columns[2].Trim();


            string upgradeName =
                columns[3];


            string description =
                columns[4];


            string iconPath =
                columns[5];


            string spriteName =
                columns[6];



            string assetPath =
                $"{DataPath}/{assetName}.asset";



            UpgradeData data =
                AssetDatabase.LoadAssetAtPath<UpgradeData>(
                    assetPath
                );



            // 신규 생성
            if(data == null)
            {
                if(!typeMap.TryGetValue(
                    typeName,
                    out Type type))
                {
                    Debug.LogError(
                        $"Type 없음 : {typeName}"
                    );

                    continue;
                }


                data =
                    ScriptableObject.CreateInstance(type)
                    as UpgradeData;


                AssetDatabase.CreateAsset(
                    data,
                    assetPath
                );
            }



            // 데이터 적용

            data.ID = id;


            data.UpgradeName =
                upgradeName.Replace("\\n", "\n");


            data.UpgradeDescription =
                description.Replace("\\n", "\n");



            // Sprite 적용

            Sprite sprite =
                LoadSprite(
                    iconPath,
                    spriteName
                );


            if(sprite != null)
            {
                data.UpgradeIcon = sprite;
            }
            else if(!string.IsNullOrEmpty(iconPath))
            {
                Debug.LogWarning(
                    $"Sprite Load 실패 : {iconPath} / {spriteName}"
                );
            }



            EditorUtility.SetDirty(data);
        }



        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();


        Debug.Log(
            "Upgrade CSV Import Complete"
        );
    }


    #endregion



    #region Sprite


    private static Sprite LoadSprite(
        string path,
        string spriteName
    )
    {
        if(string.IsNullOrEmpty(path))
            return null;



        // Multiple Sprite 대응

        UnityEngine.Object[] assets =
            AssetDatabase.LoadAllAssetsAtPath(path);


        foreach(UnityEngine.Object asset in assets)
        {
            if(asset is Sprite sprite)
            {
                if(sprite.name == spriteName)
                    return sprite;
            }
        }



        // 단일 Sprite fallback

        return AssetDatabase.LoadAssetAtPath<Sprite>(
            path
        );
    }



    private static string GetIconPath(Sprite sprite)
    {
        if(sprite == null)
            return "";


        return AssetDatabase.GetAssetPath(sprite);
    }



    private static string GetSpriteName(Sprite sprite)
    {
        if(sprite == null)
            return "";


        return sprite.name;
    }


    #endregion



    #region CSV Parser


    private static List<string> ParseCSVLines(string csv)
    {
        List<string> result = new();


        StringBuilder current = new();


        bool insideQuote = false;


        foreach(char c in csv)
        {
            if(c == '"')
                insideQuote = !insideQuote;



            if(c == '\n' && !insideQuote)
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



        if(current.Length > 0)
            result.Add(
                current.ToString()
            );


        return result;
    }





    private static string[] ParseCSV(string line)
    {
        List<string> result = new();


        StringBuilder current = new();


        bool insideQuote = false;



        for(int i = 0; i < line.Length; i++)
        {
            char c = line[i];


            if(c == '"')
            {
                if(
                    insideQuote &&
                    i + 1 < line.Length &&
                    line[i + 1] == '"'
                )
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    insideQuote =
                        !insideQuote;
                }


                continue;
            }



            if(c == ',' && !insideQuote)
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



    #region Utility


    private static void CreateDirectory()
    {
        if(!Directory.Exists(DataPath))
            Directory.CreateDirectory(DataPath);


        string csvDirectory =
            System.IO.Path.GetDirectoryName(CSVPath);


        if(!Directory.Exists(csvDirectory))
            Directory.CreateDirectory(csvDirectory);
    }





    private static Dictionary<string, Type> GetUpgradeTypeMap()
    {
        Dictionary<string, Type> map = new();


        foreach(Type type in
            TypeCache.GetTypesDerivedFrom<UpgradeData>())
        {
            if(type.IsAbstract)
                continue;


            map[type.Name] = type;
        }


        return map;
    }





    private static string Escape(string value)
    {
        if(string.IsNullOrEmpty(value))
            return "";


        value =
            value
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n");


        value =
            value.Replace("\"", "\"\"");


        return $"\"{value}\"";
    }


    #endregion
}