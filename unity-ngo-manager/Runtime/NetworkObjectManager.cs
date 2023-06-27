using NGOManager.Utility.Singleton;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NGOManager
{
    public class NetworkObjectManager : SingletonPersistent<NetworkObjectManager>
    {
        //TODO: Use reflection to determine if a function is present when added and store it in each list

        private IList<NetworkObjectBase> networkObjectBases;
        public ReadOnlyCollection<NetworkObjectBase> NetworkObjectBases { get; private set; }


        protected override void Awake()
        {
            base.Awake();

            networkObjectBases = new List<NetworkObjectBase>();
            NetworkObjectBases = new ReadOnlyCollection<NetworkObjectBase>(networkObjectBases);
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
                if (networkObject.StateMachine != null)
                {
                    networkObject.StateMachine.State?.OnPreUpdate();
                    if (networkObject.IsHost)
                    {
                        networkObject.StateMachine.State?.OnHostPreUpdate();
                    }
                    if (networkObject.IsOwner)
                    {
                        networkObject.StateMachine.State?.OnOwnerPreUpdate();
                    }
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
                if (networkObject.StateMachine != null)
                {
                    networkObject.StateMachine.State?.OnUpdate();
                    if (networkObject.IsHost)
                    {
                        networkObject.StateMachine.State?.OnHostUpdate();
                    }
                    if (networkObject.IsOwner)
                    {
                        networkObject.StateMachine.State?.OnOwnerUpdate();
                    }
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
                if (networkObject.StateMachine != null)
                {
                    networkObject.StateMachine.State?.OnLateUpdate();
                    if (networkObject.IsHost)
                    {
                        networkObject.StateMachine.State?.OnHostLateUpdate();
                    }
                    if (networkObject.IsOwner)
                    {
                        networkObject.StateMachine.State?.OnOwnerLateUpdate();
                    }
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
                if (networkObject.StateMachine != null)
                {
                    networkObject.StateMachine.State?.OnFixedUpdate();
                    if (networkObject.IsHost)
                    {
                        networkObject.StateMachine.State?.OnHostFixedUpdate();
                    }
                    if (networkObject.IsOwner)
                    {
                        networkObject.StateMachine.State?.OnOwnerFixedUpdate();
                    }
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
            if (networkObjectBase.StateMachine != null)
            {
                networkObjectBase.StateMachine.State?.OnStart();
                if (networkObjectBase.IsHost)
                {
                    networkObjectBase.StateMachine.State?.OnHostStart();
                }
                if (networkObjectBase.NetworkObject.IsOwner)
                {
                    networkObjectBase.StateMachine.State?.OnOwnerStart();
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
            if (networkObjectBase.StateMachine != null)
            {
                networkObjectBase.StateMachine.State?.OnEnd();
                if (networkObjectBase.IsHost)
                {
                    networkObjectBase.StateMachine.State?.OnHostEnd();
                }
                if (networkObjectBase.IsOwner)
                {
                    networkObjectBase.StateMachine.State?.OnOwnerEnd();
                }
            }
        }

        public void ClearNetworkObjects()
        {
            networkObjectBases.Clear();
        }
    }
}
