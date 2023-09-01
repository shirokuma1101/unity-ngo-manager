using NGOManager.Utility.Singleton;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Netcode;

namespace NGOManager
{
    public class NetworkObjectManager : SingletonPersistent<NetworkObjectManager>
    {
        //TODO: Use reflection to determine if a function is present when added and store it in each list

        public ReadOnlyCollection<NetworkObjectBase> NetworkObjectBases { get; private set; }
        public ReadOnlyCollection<GenericNetworkStateMachine> NetworkStateMachines { get; private set; }
        public NetworkObject LocalPlayer { get; private set; }
        public ReadOnlyCollection<NetworkObject> RemotePlayers { get; private set; }
        public Action<NetworkObject> OnLocalPlayerSpawned = null;
        public Action<NetworkObject> OnLocalPlayerDespawned = null;
        public Action<NetworkObject> OnRemotePlayerSpawned = null;
        public Action<NetworkObject> OnRemotePlayerDespawned = null;
        public Action OnAllPlayersSpawned = null;
		public Action OnAllPlayersDespawned = null;

        private IList<NetworkObjectBase> networkObjectBases;
        private IList<GenericNetworkStateMachine> networkStateMachines;
        private IList<NetworkObject> remotePlayers;

        protected override void Awake()
        {
            base.Awake();

            networkObjectBases = new List<NetworkObjectBase>();
            NetworkObjectBases = new ReadOnlyCollection<NetworkObjectBase>(networkObjectBases);
            networkStateMachines = new List<GenericNetworkStateMachine>();
            NetworkStateMachines = new ReadOnlyCollection<GenericNetworkStateMachine>(networkStateMachines);
            remotePlayers = new List<NetworkObject>();
            RemotePlayers = new ReadOnlyCollection<NetworkObject>(remotePlayers);
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

        public void RegisterNetworkObject(NetworkObject networkObject)
        {
            foreach (var networkObjectBase in networkObject.GetComponentsInChildren<NetworkObjectBase>())
            {
                RegisterNetworkObject(networkObjectBase);
            }

            if (networkObject.IsPlayerObject)
            {
                if (networkObject.IsOwner)
                {
                    LocalPlayer = networkObject;
                    OnLocalPlayerSpawned?.Invoke(LocalPlayer);
                }
                else
                {
                    remotePlayers.Add(networkObject);
                    OnRemotePlayerSpawned?.Invoke(networkObject);
                }

                if (LocalPlayer != null && remotePlayers.Count == NetworkManager.Singleton.ConnectedClientsList.Count - 1)
                {
                    OnAllPlayersSpawned?.Invoke();
                }
            }
        }
        public void RegisterNetworkObject(NetworkObjectBase networkObjectBase)
        {
            networkObjectBases.Add(networkObjectBase);

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

        public void UnregisterNetworkObject(NetworkObject networkObject)
        {
            foreach (var networkObjectBase in networkObject.GetComponentsInChildren<NetworkObjectBase>())
            {
                UnregisterNetworkObject(networkObjectBase);
            }

            if (networkObject.IsPlayerObject)
            {
                if (networkObject.IsOwner)
                {
                    LocalPlayer = null;
                    OnLocalPlayerDespawned?.Invoke(networkObject);
                }
                else
                {
                    remotePlayers.Remove(networkObject);
                    OnRemotePlayerDespawned?.Invoke(networkObject);
                }

                if (LocalPlayer == null && remotePlayers.Count == 0)
                {
                    OnAllPlayersDespawned?.Invoke();
                }
            }
        }
        public void UnregisterNetworkObject(NetworkObjectBase networkObjectBase)
        {
            networkObjectBases.Remove(networkObjectBase);

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
