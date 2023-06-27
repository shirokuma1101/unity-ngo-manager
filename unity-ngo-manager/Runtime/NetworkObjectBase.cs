using Unity.Netcode;

namespace NGOManager
{
    public class NetworkObjectBase : NetworkBehaviour, INetworkObjectEventFunctions
    {
        public GenericNetworkStateMachine StateMachine { get; }


        public virtual void OnStart() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnPreUpdate() { }
        public virtual void OnUpdate() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnEnd() { }

        public virtual void OnHostStart() { }
        public virtual void OnHostFixedUpdate() { }
        public virtual void OnHostPreUpdate() { }
        public virtual void OnHostUpdate() { }
        public virtual void OnHostLateUpdate() { }
        public virtual void OnHostEnd() { }

        public virtual void OnOwnerStart() { }
        public virtual void OnOwnerFixedUpdate() { }
        public virtual void OnOwnerPreUpdate() { }
        public virtual void OnOwnerUpdate() { }
        public virtual void OnOwnerLateUpdate() { }
        public virtual void OnOwnerEnd() { }
    }
}
