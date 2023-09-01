using NGOManager.Utility.Singleton;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NGOManager
{
    public class NetworkObjectManager : SingletonPersistent<NetworkObjectManager>
    {
        //TODO: Use reflection to determine if a function is present when added and store it in each list

        public ReadOnlyCollection<NetworkObjectBase> NetworkObjectBases { get; private set; }
        public ReadOnlyCollection<GenericNetworkStateMachine> NetworkStateMachines { get; private set; }
        public NetworkObjectBase LocalPlayer { get; private set; }
        public ReadOnlyCollection<NetworkObjectBase> RemotePlayers { get; private set; }
        public Action<NetworkObjectBase> LocalPlayerSpawned;
        public Action<NetworkObjectBase> LocalPlayerDespawned;
        public Action<NetworkObjectBase> RemotePlayerSpawned;
        public Action<NetworkObjectBase> RemotePlayerDespawned;

        private IList<NetworkObjectBase> networkObjectBases;
        private IList<GenericNetworkStateMachine> networkStateMachines;
        private IList<NetworkObjectBase> remotePlayers;

        protected override void Awake()
        {
            base.Awake();

            networkObjectBases = new List<NetworkObjectBase>();
            NetworkObjectBases = new ReadOnlyCollection<NetworkObjectBase>(networkObjectBases);
            networkStateMachines = new List<GenericNetworkStateMachine>();
            NetworkStateMachines = new ReadOnlyCollection<GenericNetworkStateMachine>(networkStateMachines);
            remotePlayers = new List<NetworkObjectBase>();
            RemotePlayers = new ReadOnlyCollection<NetworkObjectBase>(remotePlayers);
        }

        private void Update()
        {
            foreach (var networkObject in NetworkObjectBases)
            {
                networkObject.OnPreUpdate();
                if (networkObject.IsHost)
                {
                    networkObject.OnHostPreUpdate();
                }
                if (networkObject.IsOwner)
                {
                    networkObject.OnOwnerPreUpdate();
                }
            }
            foreach (var networkStateMachine in NetworkStateMachines)
            {
                networkStateMachine.State?.OnPreUpdate();
                if (networkStateMachine.IsHost)
                {
                    networkStateMachine.State?.OnHostPreUpdate();
                }
                if (networkStateMachine.IsOwner)
                {
                    networkStateMachine.State?.OnOwnerPreUpdate();
                }
            }
            foreach (var networkObject in NetworkObjectBases)
            {
                networkObject.OnUpdate();
                if (networkObject.IsHost)
                {
                    networkObject.OnHostUpdate();
                }
                if (networkObject.IsOwner)
                {
                    networkObject.OnOwnerUpdate();
                }
            }
            foreach (var networkStateMachine in NetworkStateMachines)
            {
                networkStateMachine.State?.OnUpdate();
                if (networkStateMachine.IsHost)
                {
                    networkStateMachine.State?.OnHostUpdate();
                }
                if (networkStateMachine.IsOwner)
                {
                    networkStateMachine.State?.OnOwnerUpdate();
                }
            }
        }

        private void LateUpdate()
        {
            foreach (var networkObject in NetworkObjectBases)
            {
                networkObject.OnLateUpdate();
                if (networkObject.IsHost)
                {
                    networkObject.OnHostLateUpdate();
                }
                if (networkObject.IsOwner)
                {
                    networkObject.OnOwnerLateUpdate();
                }
            }
            foreach (var networkStateMachine in NetworkStateMachines)
            {
                networkStateMachine.State?.OnLateUpdate();
                if (networkStateMachine.IsHost)
                {
                    networkStateMachine.State?.OnHostLateUpdate();
                }
                if (networkStateMachine.IsOwner)
                {
                    networkStateMachine.State?.OnOwnerLateUpdate();
                }
            }
        }

        private void FixedUpdate()
        {
            foreach (var networkObject in NetworkObjectBases)
            {
                networkObject.OnFixedUpdate();
                if (networkObject.IsHost)
                {
                    networkObject.OnHostFixedUpdate();
                }
                if (networkObject.IsOwner)
                {
                    networkObject.OnOwnerFixedUpdate();
                }
            }
            foreach (var networkStateMachine in NetworkStateMachines)
            {
                networkStateMachine.State?.OnFixedUpdate();
                if (networkStateMachine.IsHost)
                {
                    networkStateMachine.State?.OnHostFixedUpdate();
                }
                if (networkStateMachine.IsOwner)
                {
                    networkStateMachine.State?.OnOwnerFixedUpdate();
                }
            }
        }

        public void RegisterNetworkObject(NetworkObjectBase networkObjectBase)
        {
            networkObjectBases.Add(networkObjectBase);

            if (networkObjectBase.NetworkObject.IsPlayerObject)
            {
                if (networkObjectBase.NetworkObject.IsOwner)
                {
                    LocalPlayer = networkObjectBase;
                    LocalPlayerSpawned?.Invoke(LocalPlayer);
                }
                else
                {
                    remotePlayers.Add(networkObjectBase);
                    RemotePlayerSpawned?.Invoke(networkObjectBase);
                }
            }

            networkObjectBase.OnStart();
            if (networkObjectBase.IsHost)
            {
                networkObjectBase.OnHostStart();
            }
            if (networkObjectBase.NetworkObject.IsOwner)
            {
                networkObjectBase.OnOwnerStart();
            }

            foreach (var networkStateMachine in networkObjectBase.GetComponentsInChildren<GenericNetworkStateMachine>())
            {
                networkStateMachines.Add(networkStateMachine);

                networkStateMachine.State?.OnStart();
                if (networkStateMachine.IsHost)
                {
                    networkStateMachine.State?.OnHostStart();
                }
                if (networkStateMachine.IsOwner)
                {
                    networkStateMachine.State?.OnOwnerStart();
                }
            }
        }

        public void UnregisterNetworkObject(NetworkObjectBase networkObjectBase)
        {
            networkObjectBases.Remove(networkObjectBase);

            if (networkObjectBase.NetworkObject.IsPlayerObject)
            {
                if (networkObjectBase.NetworkObject.IsOwner)
                {
                    LocalPlayer = null;
                    LocalPlayerDespawned?.Invoke(networkObjectBase);
                }
                else
                {
                    remotePlayers.Remove(networkObjectBase);
                    RemotePlayerDespawned?.Invoke(networkObjectBase);
                }
            }

            networkObjectBase.OnEnd();
            if (networkObjectBase.IsHost)
            {
                networkObjectBase.OnHostEnd();
            }
            if (networkObjectBase.IsOwner)
            {
                networkObjectBase.OnOwnerEnd();
            }

            foreach (var networkStateMachine in networkObjectBase.GetComponentsInChildren<GenericNetworkStateMachine>())
            {
                networkStateMachines.Remove(networkStateMachine);

                networkStateMachine.State?.OnEnd();
                if (networkStateMachine.IsHost)
                {
                    networkStateMachine.State?.OnHostEnd();
                }
                if (networkStateMachine.IsOwner)
                {
                    networkStateMachine.State?.OnOwnerEnd();
                }
            }
        }

        public void ClearNetworkObjects()
        {
            networkObjectBases.Clear();
            networkStateMachines.Clear();
        }
    }
}
