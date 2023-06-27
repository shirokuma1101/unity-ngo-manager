using Unity.Netcode;

namespace NGOManager
{
    public class GenericNetworkStateMachine : NetworkBehaviour
    {
        public abstract class StateBase : INetworkObjectEventFunctions
        {
            public NetworkBehaviour Owner { get; private set; }


            public abstract void Initialize(GenericNetworkStateMachine genericNetworkStateMachine);

            public virtual void OnEnter() { }
            public virtual void OnHostEnter() { }
            public virtual void OnOwnerEnter() { }

            public virtual void OnExit() { }
            public virtual void OnHostExit() { }
            public virtual void OnOwnerExit() { }

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


        public StateBase State
        {
            get => state;
            set
            {
                if (state != null)
                {
                    state.OnExit();
                    if (state.Owner.IsHost)
                    {
                        state.OnHostExit();
                    }
                    if (state.Owner.IsOwner)
                    {
                        state.OnOwnerExit();
                    }
                }
                state = value;
                if (state != null)
                {
                    state.OnEnter();
                    if (state.Owner.IsHost)
                    {
                        state.OnHostEnter();
                    }
                    if (state.Owner.IsOwner)
                    {
                        state.OnOwnerEnter();
                    }
                }
            }
        }

        private StateBase state = null;

    }
}
