using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : Singleton<SceneFlowManager>
{
    [Header("Scene Names")]
    [SerializeField] private string _lobbySceneName = "Lobby";

    private bool _allPlayerLoaded = false;

    void Start()
    {
        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.LoadedScene,
            HandleLoadedSceneNotify
        );
    }

    private IEnumerator LoadSceneRoutine(string sceneName, Action onLoaded = null)
    {
        Time.timeScale = 1f; // 배속 초기화
        
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneFlowManager: sceneName is null or empty.");
                yield break;
        }

        ObjectPoolManager.Instance.ReturnAllActiveObject();

        SoundManager.Instance.StopBGM(1f);

        yield return CommonUIManager.Instance.LoadingUI.ShowLoadingUI();
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
            yield return null;

        // 스테이지 진입 시 처리
        if (sceneName != _lobbySceneName)
        {
            SendLoadSceneRequest();
            while (!_allPlayerLoaded)
                yield return null;
        }

        onLoaded?.Invoke();
        yield return CommonUIManager.Instance.LoadingUI.HideLoadingUI();
        _allPlayerLoaded = false;
    }

    public void LoadScene(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"SceneFlowManager: buildIndex {buildIndex} is out of range.");
            return;
        }

        SceneManager.LoadScene(buildIndex);
    }

    public void ReloadCurrentScene(Action onLoaded = null)
    {
        StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().name, onLoaded));
    }

    public void LoadStageScene(int selectStage, Action onLoaded = null)
    {
        string selectSceneName = "Stage" + selectStage;

        Debug.Log(selectSceneName);

        if (Application.CanStreamedLevelBeLoaded(selectSceneName))
        {
            StartCoroutine(LoadSceneRoutine(selectSceneName, onLoaded));
        }
        else
        {
            Debug.Log("No next stage. Returning to lobby.");
            LoadLobbyScene();
        }
    }
    
    public void LoadLobbyScene(Action onLoaded = null)
    {
        // Lobby로 씬 전환 시 Stage 정리
        StartCoroutine(LoadSceneRoutine(_lobbySceneName, onLoaded));
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    private async Task SendLoadSceneRequest()
    {
        LoadSceneRequest request = new LoadSceneRequest
        {

        };

        try
        {

            bool sent = await NetworkManager.Instance.SendAsync(
                PacketType.LoadSceneRequest,
                request
            );

            if (!sent)
            {
                Debug.LogError(
                    "Load Scene request failed: not connected."
                );

                return;
            }

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Load request failed: {e.Message}"
            );
        }
    }

    private void HandleLoadedSceneNotify(byte[] payload)
    {
        try
        {
            LoadedSceneNotify notify =
                NetworkManager.BytesToStruct<LoadedSceneNotify>(
                    payload
                );

            if (notify.errCode != ErrorCode.None)
            {
                Debug.LogError(
                    $"Load Failed: {notify.errCode}"
                );

                return;
            }

            NetworkManager.Instance.AddAction(()=>
            {
                _allPlayerLoaded = true;
            });


        }
        catch (Exception e)
        {
            Debug.LogError(
                $"JoinStageResponse Parse Failed: {e.Message}"
            );
        }
    }
}
