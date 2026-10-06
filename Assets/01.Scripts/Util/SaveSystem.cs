using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography; // 암호화 기능을 위해 추가
using System.Text;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static string _playerPath = Application.persistentDataPath + "/player.json";
    private static string _shopPath = Application.persistentDataPath + "/shop.json";
    private static string _settingPath = Application.persistentDataPath + "/setting.json";

    // 데이터 명시적 마커
    private const string Base64Marker = "BASE64:";
    private const string AESMarker = "AES256:"; // 새로 사용할 AES 마커

    // Private Key
    private const string PrivateKey = "Priv@teSecretKey32ByteOnly1213su"; 
    private const string PrivateIV = "Priv@teSecret16B";               

    /// <summary>
    /// JSON 문자열을 AES256으로 암호화하고 Base64로 인코딩한 뒤 마커를 추가
    /// </summary>
    private static string EncryptAES(string plainText)
    {
        try
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(PrivateKey);
            byte[] ivBytes = Encoding.UTF8.GetBytes(PrivateIV);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    
                    string encrypted = Convert.ToBase64String(ms.ToArray());
                    return AESMarker + encrypted;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Encryption Failed: {e.Message}");
            return plainText; // 실패 시 평문 반환 (안전장치)
        }
    }

    /// <summary>
    /// 마커를 확인하여 AES256 복호화, Base64 디코딩 또는 평문 처리
    /// </summary>
    private static string DecryptAESIfNeeded(string content)
    {
        // AES256 암호화된 데이터인 경우
        if (content.StartsWith(AESMarker))
        {
            try
            {
                string base64Data = content.Substring(AESMarker.Length);
                byte[] cipherText = Convert.FromBase64String(base64Data);

                byte[] keyBytes = Encoding.UTF8.GetBytes(PrivateKey);
                byte[] ivBytes = Encoding.UTF8.GetBytes(PrivateIV);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream(cipherText))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"AES Decryption Failed: {e.Message}");
                return content.Substring(AESMarker.Length);
            }
        }
        // 기존 Base64 데이터인 경우
        else if (content.StartsWith(Base64Marker))
        {
            try
            {
                string base64Data = content.Substring(Base64Marker.Length);
                byte[] data = Convert.FromBase64String(base64Data);
                return Encoding.UTF8.GetString(data);
            }
            catch
            {
                return content.Substring(Base64Marker.Length);
            }
        }
        // 마커가 없는 완전 평문인 경우
        else
        {
            return content;
        }
    }

    #region Save Methods
    public static void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string encrypted = EncryptAES(json);
        File.WriteAllText(_playerPath, encrypted);
    }

    public static void Save(ShopData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string encrypted = EncryptAES(json);
        File.WriteAllText(_shopPath, encrypted);
    }

    public static void Save(SettingData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string encrypted = EncryptAES(json);
        File.WriteAllText(_settingPath, encrypted);
    }
    #endregion

    #region Load Methods
    public static PlayerData LoadPlayerData()
    {
        try
        {
            string encoded = File.ReadAllText(_playerPath);
            string json = DecryptAESIfNeeded(encoded);

            return JsonUtility.FromJson<PlayerData>(json);
        }
        catch
        {
            Debug.Log("Save file corrupted or missing, creating default PlayerData");
            PlayerData newData = new PlayerData();
            
            newData.CurrentStage = 1;
            newData.Gold = 0;
            newData.MinionList = new List<int> { 1001, 1002 };

            return newData;
        }
    }

    public static ShopData LoadShopData()
    {
        try
        {
            string encoded = File.ReadAllText(_shopPath);
            string json = DecryptAESIfNeeded(encoded);

            return JsonUtility.FromJson<ShopData>(json);
        }
        catch
        {
            Debug.Log("Save file corrupted or missing, creating default ShopData");
            ShopData newData = new ShopData();
            
            return newData;
        }
    }

    public static SettingData LoadSettingData()
    {
        try
        {
            string encoded = File.ReadAllText(_settingPath);
            string json = DecryptAESIfNeeded(encoded);

            return JsonUtility.FromJson<SettingData>(json);
        }
        catch
        {
            Debug.Log("Save file corrupted or missing, creating default SettingData");
            SettingData newData = new SettingData();
            
            return newData;
        }
    }
    #endregion
}