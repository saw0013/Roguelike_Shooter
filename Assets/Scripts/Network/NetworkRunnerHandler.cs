using Fusion.Sockets;
using Fusion;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner NetworkRunnerPrefab;

    private NetworkRunner _networkRunner;

    void Start()
    {
        _networkRunner = Instantiate(NetworkRunnerPrefab);
        _networkRunner.name = "net runner";

        var clientTask =
            InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(),
                SceneManager.GetActiveScene().buildIndex, null);

        Debug.Log("Server NetworkRunner started!");
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address,
        SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneObjectProvider =
            runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneObjectProvider == null)
            sceneObjectProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();


        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = "TestRoom",
            Initialized = initialized,
            SceneManager = sceneObjectProvider
        });
    }
}
