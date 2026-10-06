using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


public static class SkillDataEditor
{
    private const string DataPath =
        "Assets/06.ScriptableObjects/Skill";

    private const string CSVPath =
        "Assets/CSV/SkillData.csv";



    #region Export


    [MenuItem("Tools/Skill Data/Export CSV")]
    public static void ExportCSV()
    {
        string[] guids =
            AssetDatabase.FindAssets(
                "t:SkillData",
                new[] { DataPath }
            );


        List<SkillData> skills = new();


        foreach(string guid in guids)
        {
            SkillData data =
                AssetDatabase.LoadAssetAtPath<SkillData>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(data != null)
                skills.Add(data);
        }



        skills.Sort(
            (a,b)=>a.ID.CompareTo(b.ID)
        );



        StringBuilder csv = new();


        csv.AppendLine(
            "ID,SkillName,Description," +
            "SkillSprite,SkillAnimClip,SkillSFX," +
            "SkillType,SearchScope,MaxTargets," +
            "ProjectileType,ProjectileHitType," +
            "ProjectileSpeed,ProjectileDuration," +
            "BuffList," +
            "AfterAnimClip,AfterSFX," +
            "SkillDamage,SkillScale," +
            "HitEvents,KillEvents"
        );



        foreach(SkillData skill in skills)
        {
            csv.AppendLine(
                string.Join(",",
                    skill.ID,
                    Escape(skill.SkillName),
                    Escape(skill.Description),

                    Escape(GetPath(skill.SkillSprite)),
                    Escape(GetPath(skill.SkillAnimClip)),
                    Escape(GetPath(skill.SkillSFX)),

                    skill.SkillType,
                    skill.SearchScope,
                    skill.MaxTargets,

                    skill.ProjectileType,
                    skill.ProjectileHitType,

                    skill.ProjectileSpeed,
                    skill.ProjectileDuration,

                    Escape(GetBuffCSV(skill.BuffList)),

                    Escape(GetPath(skill.AfterAnimClip)),
                    Escape(GetPath(skill.AfterSFX)),

                    skill.SkillDamage,
                    skill.SkillScale,

                    Escape(GetHitCSV(skill.HitEvents)),
                    Escape(GetKillCSV(skill.KillEvents))
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
            $"Skill Export Complete : {CSVPath}"
        );
    }



    #endregion





    #region Import



    [MenuItem("Tools/Skill Data/Import CSV")]
    public static void ImportCSV()
    {
        if(!File.Exists(CSVPath))
        {
            Debug.LogError(
                "CSV 파일 없음"
            );

            return;
        }



        string text =
            File.ReadAllText(
                CSVPath,
                Encoding.UTF8
            );



        List<string[]> rows =
            ParseCSV(text);



        Dictionary<int,BuffData> buffMap =
            LoadIDMap<BuffData>();


        Dictionary<int,ChangeDefaultAttack> attackMap =
            LoadIDMap<ChangeDefaultAttack>();


        Dictionary<int,HitEvent> hitMap =
            LoadIDMap<HitEvent>();


        Dictionary<int,KillEvent> killMap =
            LoadIDMap<KillEvent>();




        for(int i = 1; i < rows.Count; i++)
        {
            string[] c = rows[i];


            if(c.Length != 20)
            {
                Debug.LogWarning(
                    $"CSV Column Error Line : {i} Count : {c.Length}"
                );

                continue;
            }



            if(!int.TryParse(
                c[0],
                out int id))
            {
                continue;
            }




            SkillData skill =
                FindSkill(id);



            // 신규 생성
            if(skill == null)
            {
                skill =
                    ScriptableObject.CreateInstance<SkillData>();


                skill.ID = id;


                string path =
                    $"{DataPath}/Skill_{id}.asset";


                AssetDatabase.CreateAsset(
                    skill,
                    path
                );


                Debug.Log(
                    $"Skill 생성 : {path}"
                );
            }




            skill.SkillName = c[1];

            skill.Description = c[2];



            skill.SkillSprite =
                LoadAsset<Sprite>(c[3]);


            skill.SkillAnimClip =
                LoadAsset<AnimationClip>(c[4]);


            skill.SkillSFX =
                LoadAsset<AudioClip>(c[5]);



            Enum.TryParse(
                c[6],
                out skill.SkillType
            );


            Enum.TryParse(
                c[7],
                out skill.SearchScope
            );


            int.TryParse(
                c[8],
                out skill.MaxTargets
            );



            Enum.TryParse(
                c[9],
                out skill.ProjectileType
            );


            Enum.TryParse(
                c[10],
                out skill.ProjectileHitType
            );



            float.TryParse(
                c[11],
                out skill.ProjectileSpeed
            );


            float.TryParse(
                c[12],
                out skill.ProjectileDuration
            );



            skill.BuffList =
                ParseBuff(
                    c[13],
                    buffMap,
                    attackMap
                );



            skill.AfterAnimClip =
                LoadAsset<AnimationClip>(c[14]);


            skill.AfterSFX =
                LoadAsset<AudioClip>(c[15]);



            int.TryParse(
                c[16],
                out skill.SkillDamage
            );


            float.TryParse(
                c[17],
                out skill.SkillScale
            );



            skill.HitEvents =
                ParseHit(
                    c[18],
                    hitMap
                );



            skill.KillEvents =
                ParseKill(
                    c[19],
                    killMap
                );



            EditorUtility.SetDirty(skill);
        }



        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        Debug.Log(
            "Skill Import Complete"
        );
    }



    #endregion





    #region Serialize



    private static string GetBuffCSV(
        List<Buff> list)
    {
        if(list == null)
            return "";


        List<string> result = new();



        foreach(Buff buff in list)
        {
            if(buff == null ||
               buff.Data == null)
                continue;


            int attackID =
                buff.DefaultAttackData == null
                ? 0
                : buff.DefaultAttackData.ID;



            result.Add(
                $"{buff.Data.ID};{buff.Increase};{buff.Duration};{attackID}"
            );
        }


        return string.Join("|", result);
    }




    private static string GetHitCSV(
        List<OnHitEvent> list)
    {
        if(list == null)
            return "";


        List<string> result = new();



        foreach(OnHitEvent hit in list)
        {
            if(hit == null ||
               hit.HitEvent == null)
                continue;


            result.Add(
                $"{hit.HitEvent.ID};{hit.Duration};{hit.Value}"
            );
        }


        return string.Join("|", result);
    }




    private static string GetKillCSV(
        List<OnKillEvent> list)
    {
        if(list == null)
            return "";


        List<string> result = new();



        foreach(OnKillEvent kill in list)
        {
            if(kill == null ||
               kill.KillEvent == null)
                continue;


            result.Add(
                $"{kill.KillEvent.ID};{kill.Duration};{kill.Value}"
            );
        }


        return string.Join("|", result);
    }



    #endregion





    #region Deserialize



    private static List<Buff> ParseBuff(
        string value,
        Dictionary<int,BuffData> buffMap,
        Dictionary<int,ChangeDefaultAttack> attackMap)
    {
        List<Buff> result = new();


        if(string.IsNullOrEmpty(value))
            return result;



        foreach(string data in value.Split('|'))
        {
            string[] v =
                data.Split(';');


            if(v.Length < 4)
                continue;



            int.TryParse(v[0],out int buffID);
            float.TryParse(v[1],out float increase);
            float.TryParse(v[2],out float duration);
            int.TryParse(v[3],out int attackID);



            buffMap.TryGetValue(
                buffID,
                out BuffData buffData
            );


            attackMap.TryGetValue(
                attackID,
                out ChangeDefaultAttack attack
            );



            result.Add(
                new Buff
                {
                    Data = buffData,
                    Increase = increase,
                    Duration = duration,
                    DefaultAttackData = attack
                }
            );
        }


        return result;
    }





    private static List<OnHitEvent> ParseHit(
        string value,
        Dictionary<int,HitEvent> map)
    {
        List<OnHitEvent> result = new();


        if(string.IsNullOrEmpty(value))
            return result;



        foreach(string data in value.Split('|'))
        {
            string[] v =
                data.Split(';');


            if(v.Length < 3)
                continue;



            if(!int.TryParse(
                v[0],
                out int id))
                continue;



            if(!map.TryGetValue(
                id,
                out HitEvent hit))
                continue;



            float.TryParse(
                v[1],
                out float duration
            );


            float.TryParse(
                v[2],
                out float valueFloat
            );



            result.Add(
                new OnHitEvent
                {
                    HitEvent = hit,
                    Duration = duration,
                    Value = valueFloat
                }
            );
        }


        return result;
    }





    private static List<OnKillEvent> ParseKill(
        string value,
        Dictionary<int,KillEvent> map)
    {
        List<OnKillEvent> result = new();


        if(string.IsNullOrEmpty(value))
            return result;



        foreach(string data in value.Split('|'))
        {
            string[] v =
                data.Split(';');


            if(v.Length < 3)
                continue;



            if(!int.TryParse(
                v[0],
                out int id))
                continue;



            if(!map.TryGetValue(
                id,
                out KillEvent kill))
                continue;



            float.TryParse(
                v[1],
                out float duration
            );


            float.TryParse(
                v[2],
                out float valueFloat
            );



            result.Add(
                new OnKillEvent
                {
                    KillEvent = kill,
                    Duration = duration,
                    Value = valueFloat
                }
            );
        }


        return result;
    }



    #endregion





    #region Utility



    private static SkillData FindSkill(int id)
    {
        foreach(string guid in
            AssetDatabase.FindAssets("t:SkillData"))
        {
            SkillData data =
                AssetDatabase.LoadAssetAtPath<SkillData>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(data != null &&
               data.ID == id)
                return data;
        }


        return null;
    }





    private static Dictionary<int,T> LoadIDMap<T>()
        where T : ScriptableObject
    {
        Dictionary<int,T> result = new();



        foreach(string guid in
            AssetDatabase.FindAssets($"t:{typeof(T).Name}"))
        {
            T obj =
                AssetDatabase.LoadAssetAtPath<T>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );


            if(obj == null)
                continue;



            var field =
                obj.GetType()
                .GetField("ID");



            if(field == null)
                continue;



            int id =
                (int)field.GetValue(obj);



            result[id] = obj;
        }



        return result;
    }





    private static T LoadAsset<T>(string path)
        where T : UnityEngine.Object
    {
        if(string.IsNullOrWhiteSpace(path))
            return null;



        T obj =
            AssetDatabase.LoadAssetAtPath<T>(path);



        if(obj != null)
            return obj;



        foreach(UnityEngine.Object o in
            AssetDatabase.LoadAllAssetsAtPath(path))
        {
            if(o is T)
                return o as T;
        }


        return null;
    }





    private static string GetPath(
        UnityEngine.Object obj)
    {
        return obj == null
            ? ""
            : AssetDatabase.GetAssetPath(obj);
    }




    private static string Escape(string value)
    {
        if(value == null)
            return "\"\"";


        return "\"" +
            value.Replace("\"","\"\"") +
            "\"";
    }




    private static List<string[]> ParseCSV(string text)
    {
        List<string[]> result = new();

        List<string> row = new();

        StringBuilder sb = new();


        bool quote = false;



        foreach(char c in text)
        {
            if(c == '"')
            {
                quote = !quote;
            }
            else if(c == ',' && !quote)
            {
                row.Add(sb.ToString());
                sb.Clear();
            }
            else if(c == '\n' && !quote)
            {
                row.Add(sb.ToString());
                sb.Clear();

                result.Add(row.ToArray());

                row.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }


        if(sb.Length > 0)
        {
            row.Add(sb.ToString());
            result.Add(row.ToArray());
        }


        return result;
    }



    #endregion
}