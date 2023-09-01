using Cysharp.Threading.Tasks;
using NGOManager.Utility.Singleton;
using Unity.Netcode;

namespace NGOManager
{
    public class NetworkObjectDespawner : SingletonNetworkPersistent<NetworkObjectDespawner>
    {
        private async void Start()
        {
            var cancelToken = this.GetCancellationTokenOnDestroy();

            await UniTask.WaitUntil(() => NetworkManager.Singleton.SpawnManager != null, cancellationToken: cancelToken);

            NetworkManager.Singleton.OnObjectDespawnedCallback += OnObjectDespawned;
        }

        public static void Despawn(NetworkObject networkObject)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                networkObject.Despawn();
            }
            else
            {
                Instance.DespawnServerRpc(networkObject.NetworkObjectId);
            }
        }

        private void OnObjectDespawned(NetworkObject despawnedNetworkObject)
            => NetworkObjectManager.Instance.UnRegisterNetworkObject(despawnedNetworkObject);

        [ServerRpc(RequireOwnership = false)]
        private void DespawnServerRpc(ulong networkObjectId)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                Despawn(networkObject);
            }
        }
    }
}
