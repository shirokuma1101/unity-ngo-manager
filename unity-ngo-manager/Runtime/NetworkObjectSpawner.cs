using Cysharp.Threading.Tasks;
using NGOManager.Utility.Singleton;
using Unity.Netcode;
using UnityEngine;

namespace NGOManager
{
    public class NetworkObjectSpawner : SingletonNetworkPersistent<NetworkObjectSpawner>
    {
        private async void Start()
        {
            var cancelToken = this.GetCancellationTokenOnDestroy();

            await UniTask.WaitUntil(() => NetworkManager.Singleton.SpawnManager != null, cancellationToken: cancelToken);

            NetworkManager.Singleton.OnObjectSpawnedCallback += OnObjectSpawned;
        }

        public static GameObject Spawn(NetworkObject networkPrefab, bool destroyWithScene = false)
            => Spawn(networkPrefab, Vector3.zero, Quaternion.identity, destroyWithScene);
        public static GameObject Spawn(NetworkObject networkPrefab, Vector3 position, bool destroyWithScene = false)
            => Spawn(networkPrefab, position, Quaternion.identity, destroyWithScene);
        public static GameObject Spawn(NetworkObject networkPrefab, Vector3 position, Quaternion rotation, bool destroyWithScene = false)
            => SpawnAsync(networkPrefab, position, rotation, destroyWithScene).GetAwaiter().GetResult();
        public static async UniTask<GameObject> SpawnAsync(NetworkObject networkPrefab, Vector3 position, Quaternion rotation, bool destroyWithScene = false)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkObject networkObjectInst = Instantiate(networkPrefab, position, rotation);
                networkObjectInst.Spawn(destroyWithScene);
                return networkObjectInst.gameObject;
            }
            else
            {
                var cancelToken = Instance.GetCancellationTokenOnDestroy();

                Instance.SpawnServerRpc(networkPrefab.PrefabIdHash, position, rotation, destroyWithScene);
                await UniTask.WaitUntil(() => NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(networkPrefab.NetworkObjectId), cancellationToken: cancelToken);
                return NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkPrefab.NetworkObjectId].gameObject;
            }
        }

        public static GameObject SpawnAsPlayerObject(NetworkObject networkPrefab, ulong clientId, bool destroyWithScene = false)
            => SpawnAsPlayerObject(networkPrefab, Vector3.zero, Quaternion.identity, clientId, destroyWithScene);
        public static GameObject SpawnAsPlayerObject(NetworkObject networkPrefab, Vector3 position, ulong clientId, bool destroyWithScene = false)
            => SpawnAsPlayerObject(networkPrefab, position, Quaternion.identity, clientId, destroyWithScene);
        public static GameObject SpawnAsPlayerObject(NetworkObject networkPrefab, Vector3 position, Quaternion rotation, ulong clientId, bool destroyWithScene = false)
            => SpawnAsPlayerObjectAsync(networkPrefab, position, rotation, clientId, destroyWithScene).GetAwaiter().GetResult();
        public static async UniTask<GameObject> SpawnAsPlayerObjectAsync(NetworkObject networkPrefab, Vector3 position, Quaternion rotation, ulong clientId, bool destroyWithScene = false)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkObject networkObjectInst = Instantiate(networkPrefab, position, rotation);
                networkObjectInst.SpawnAsPlayerObject(clientId, destroyWithScene);
                return networkObjectInst.gameObject;
            }
            else
            {
                var cancelToken = Instance.GetCancellationTokenOnDestroy();

                Instance.SpawnAsPlayerObjectServerRpc(networkPrefab.PrefabIdHash, position, rotation, clientId, destroyWithScene);
                await UniTask.WaitUntil(() => NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(networkPrefab.NetworkObjectId), cancellationToken: cancelToken);
                return NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkPrefab.NetworkObjectId].gameObject;
            }
        }

        public static GameObject SpawnWithOwnership(NetworkObject networkPrefab, ulong clientId, bool destroyWithScene = false)
            => SpawnWithOwnership(networkPrefab, Vector3.zero, Quaternion.identity, clientId, destroyWithScene);
        public static GameObject SpawnWithOwnership(NetworkObject networkPrefab, Vector3 position, ulong clientId, bool destroyWithScene = false)
            => SpawnWithOwnership(networkPrefab, position, Quaternion.identity, clientId, destroyWithScene);
        public static GameObject SpawnWithOwnership(NetworkObject networkPrefab, Vector3 position, Quaternion rotation, ulong clientId, bool destroyWithScene = false)
            => SpawnWithOwnershipAsync(networkPrefab, position, rotation, clientId, destroyWithScene).GetAwaiter().GetResult();
        public static async UniTask<GameObject> SpawnWithOwnershipAsync(NetworkObject networkPrefab, Vector3 position, Quaternion rotation, ulong clientId, bool destroyWithScene = false)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkObject networkObjectInst = Instantiate(networkPrefab, position, rotation);
                networkObjectInst.SpawnAsPlayerObject(clientId, destroyWithScene);
                return networkObjectInst.gameObject;
            }
            else
            {
                var cancelToken = Instance.GetCancellationTokenOnDestroy();

                Instance.SpawnWithOwnershipServerRpc(networkPrefab.PrefabIdHash, position, rotation, clientId, destroyWithScene);
                await UniTask.WaitUntil(() => NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(networkPrefab.NetworkObjectId), cancellationToken: cancelToken);
                return NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkPrefab.NetworkObjectId].gameObject;
            }
        }

        private void OnObjectSpawned(NetworkObject spawnedNetworkObject)
        {
            foreach (var networkObjectBase in spawnedNetworkObject.GetComponents<NetworkObjectBase>())
            {
                NetworkObjectManager.Instance.RegisterNetworkObject(networkObjectBase);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(uint networkPrefabIdHash, Vector3 position, Quaternion rotation, bool destroyWithScene = false)
        {
            foreach (var networkPrefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
            {
                var networkObject = networkPrefab.Prefab.GetComponent<NetworkObject>();
                if (networkObject.PrefabIdHash.Equals(networkPrefabIdHash))
                {
                    Spawn(networkObject, position, rotation, destroyWithScene);
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
                    SpawnAsPlayerObject(networkObject, position, rotation, clientId, destroyWithScene);
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
                    SpawnWithOwnership(networkObject, position, rotation, clientId, destroyWithScene);
                    break;
                }
            }
        }
    }
}
