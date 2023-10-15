using Cysharp.Threading.Tasks;
using NGOManager.Utility.Singleton;
using Unity.Netcode;
using UnityEngine;

namespace NGOManager
{
    public class NetworkObjectSpawner : SingletonNetworkPersistent<NetworkObjectSpawner>
    {
        public static async void InitializeAsync()
        {
            var cancelToken = NetworkObjectManager.Instance.GetCancellationTokenOnDestroy();
            await UniTask.WaitUntil(() => NetworkManager.Singleton.SpawnManager != null);

            NetworkManager.Singleton.OnObjectSpawnedCallback += OnObjectSpawned;
            NetworkManager.Singleton.OnObjectDespawnedCallback += OnObjectDespawned;
        }

        public static void Shutdown() {}

        public static async UniTask<GameObject> SpawnAsync(NetworkObject networkPrefab, bool destroyWithScene = false)
            => await SpawnAsync(networkPrefab, Vector3.zero, Quaternion.identity, destroyWithScene);
        public static async UniTask<GameObject> SpawnAsync(NetworkObject networkPrefab, Vector3 position, bool destroyWithScene = false)
            => await SpawnAsync(networkPrefab, position, Quaternion.identity, destroyWithScene);
        public static async UniTask<GameObject> SpawnAsync(NetworkObject networkPrefab, Vector3 position, Quaternion rotation, bool destroyWithScene = false)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkObject networkObjectInst = NetworkManager.Instantiate(networkPrefab, position, rotation);
                networkObjectInst.Spawn(destroyWithScene);
                return networkObjectInst.gameObject;
            }
            else
            {
                var cancelToken = NetworkObjectManager.Instance.GetCancellationTokenOnDestroy();

                Instance.SpawnServerRpc(networkPrefab.PrefabIdHash, position, rotation, destroyWithScene);
                await UniTask.WaitUntil(() => NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(networkPrefab.NetworkObjectId), cancellationToken: cancelToken);
                return NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkPrefab.NetworkObjectId].gameObject;
            }
        }

        public static async UniTask<GameObject> SpawnAsPlayerObjectAsync(NetworkObject networkPrefab, ulong clientId, bool destroyWithScene = false)
            => await SpawnAsPlayerObjectAsync(networkPrefab, Vector3.zero, Quaternion.identity, clientId, destroyWithScene);
        public static async UniTask<GameObject> SpawnAsPlayerObjectAsync(NetworkObject networkPrefab, Vector3 position, ulong clientId, bool destroyWithScene = false)
            => await SpawnAsPlayerObjectAsync(networkPrefab, position, Quaternion.identity, clientId, destroyWithScene);
        public static async UniTask<GameObject> SpawnAsPlayerObjectAsync(NetworkObject networkPrefab, Vector3 position, Quaternion rotation, ulong clientId, bool destroyWithScene = false)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkObject networkObjectInst = NetworkManager.Instantiate(networkPrefab, position, rotation);
                networkObjectInst.SpawnAsPlayerObject(clientId, destroyWithScene);
                return networkObjectInst.gameObject;
            }
            else
            {
                var cancelToken = NetworkObjectManager.Instance.GetCancellationTokenOnDestroy();

                Instance.SpawnAsPlayerObjectServerRpc(networkPrefab.PrefabIdHash, position, rotation, clientId, destroyWithScene);
                await UniTask.WaitUntil(() => NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(networkPrefab.NetworkObjectId), cancellationToken: cancelToken);
                return NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkPrefab.NetworkObjectId].gameObject;
            }
        }

        public static async UniTask<GameObject> SpawnWithOwnershipAsync(NetworkObject networkPrefab, ulong clientId, bool destroyWithScene = false)
            => await SpawnWithOwnershipAsync(networkPrefab, Vector3.zero, Quaternion.identity, clientId, destroyWithScene);
        public static async UniTask<GameObject> SpawnWithOwnershipAsync(NetworkObject networkPrefab, Vector3 position, ulong clientId, bool destroyWithScene = false)
            => await SpawnWithOwnershipAsync(networkPrefab, position, Quaternion.identity, clientId, destroyWithScene);
        public static async UniTask<GameObject> SpawnWithOwnershipAsync(NetworkObject networkPrefab, Vector3 position, Quaternion rotation, ulong clientId, bool destroyWithScene = false)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkObject networkObjectInst = NetworkManager.Instantiate(networkPrefab, position, rotation);
                networkObjectInst.SpawnWithOwnership(clientId, destroyWithScene);
                return networkObjectInst.gameObject;
            }
            else
            {
                var cancelToken = NetworkObjectManager.Instance.GetCancellationTokenOnDestroy();

                Instance.SpawnWithOwnershipServerRpc(networkPrefab.PrefabIdHash, position, rotation, clientId, destroyWithScene);
                await UniTask.WaitUntil(() => NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(networkPrefab.NetworkObjectId), cancellationToken: cancelToken);
                return NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkPrefab.NetworkObjectId].gameObject;
            }
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

        private static void OnObjectSpawned(NetworkObject spawnedNetworkObject)
            => NetworkObjectManager.Instance.RegisterNetworkObject(spawnedNetworkObject);

        private static void OnObjectDespawned(NetworkObject despawnedNetworkObject)
            => NetworkObjectManager.Instance.UnregisterNetworkObject(despawnedNetworkObject);

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(uint networkPrefabIdHash, Vector3 position, Quaternion rotation, bool destroyWithScene = false)
        {
            foreach (var networkPrefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
            {
                var networkObject = networkPrefab.Prefab.GetComponent<NetworkObject>();
                if (networkObject.PrefabIdHash.Equals(networkPrefabIdHash))
                {
                    SpawnAsync(networkObject, position, rotation, destroyWithScene).Forget();
                    break;
                }
            }
        }
        [ServerRpc(RequireOwnership = false)]
        private void SpawnAsPlayerObjectServerRpc(uint networkPrefabIdHash, Vector3 position, Quaternion rotation, ulong clientId, bool destroyWithScene = false)
        {
            foreach (var networkPrefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
            {
                var networkObject = networkPrefab.Prefab.GetComponent<NetworkObject>();
                if (networkObject.PrefabIdHash.Equals(networkPrefabIdHash))
                {
                    SpawnAsPlayerObjectAsync(networkObject, position, rotation, clientId, destroyWithScene).Forget();
                    break;
                }
            }
        }
        [ServerRpc(RequireOwnership = false)]
        private void SpawnWithOwnershipServerRpc(uint networkPrefabIdHash, Vector3 position, Quaternion rotation, ulong clientId, bool destroyWithScene = false)
        {
            foreach (var networkPrefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
            {
                var networkObject = networkPrefab.Prefab.GetComponent<NetworkObject>();
                if (networkObject.PrefabIdHash.Equals(networkPrefabIdHash))
                {
                    SpawnWithOwnershipAsync(networkObject, position, rotation, clientId, destroyWithScene).Forget();
                    break;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnServerRpc(ulong networkObjectId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                Despawn(networkObject);
            }
        }
    }
}
