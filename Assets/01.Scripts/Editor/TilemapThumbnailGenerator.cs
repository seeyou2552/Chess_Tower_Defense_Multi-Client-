using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;

public class TilemapThumbnailGenerator : EditorWindow
{
    private Tilemap tilemap;
    private Camera captureCamera;
    private int resolution = 512;

    [MenuItem("Tools/Tilemap Thumbnail Generator")]
    static void Open()
    {
        GetWindow<TilemapThumbnailGenerator>("Thumbnail Generator");
    }

    void OnGUI()
    {
        tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);
        captureCamera = (Camera)EditorGUILayout.ObjectField("Camera", captureCamera, typeof(Camera), true);

        resolution = EditorGUILayout.IntField("Resolution", resolution);

        if (GUILayout.Button("Generate Thumbnail"))
        {
            Generate();
        }
    }

    void Generate()
    {
        if (tilemap == null || captureCamera == null)
        {
            Debug.LogError("Tilemap 또는 Camera가 없습니다.");
            return;
        }

        // 1. Tilemap Bounds 가져오기
        Bounds bounds = tilemap.localBounds;

        // 2. 카메라 위치 맞추기
        captureCamera.transform.position = new Vector3(
            bounds.center.x,
            bounds.center.y,
            -10f
        );

        // 3. 카메라 사이즈 맞추기
        float size = Mathf.Max(
            bounds.size.y / 2f,
            bounds.size.x / (2f * captureCamera.aspect)
        );

        captureCamera.orthographic = true;
        captureCamera.orthographicSize = size;

        // 4. RenderTexture 생성
        RenderTexture rt = new RenderTexture(resolution, resolution, 24);
        captureCamera.targetTexture = rt;

        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);

        captureCamera.Render();
        RenderTexture.active = rt;

        tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        tex.Apply();

        captureCamera.targetTexture = null;
        RenderTexture.active = null;

        // 5. 저장
        string path = EditorUtility.SaveFilePanel(
            "Save Thumbnail",
            Application.dataPath,
            tilemap.name + "_thumbnail",
            "png"
        );

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Debug.Log("썸네일 저장 완료: " + path);
        }
    }
}