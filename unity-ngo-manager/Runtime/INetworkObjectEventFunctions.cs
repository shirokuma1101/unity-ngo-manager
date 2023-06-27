namespace NGOManager
{
    public interface INetworkObjectEventFunctions : INetworkObjectHostEventFunctions, INetworkObjectOwnerEventFunctions
    {
        public abstract void OnStart();
        public abstract void OnFixedUpdate();
        public abstract void OnPreUpdate();
        public abstract void OnUpdate();
        public abstract void OnLateUpdate();
        public abstract void OnEnd();
    }

    public interface INetworkObjectHostEventFunctions
    {
        public abstract void OnHostStart();
        public abstract void OnHostFixedUpdate();
        public abstract void OnHostPreUpdate();
        public abstract void OnHostUpdate();
        public abstract void OnHostLateUpdate();
        public abstract void OnHostEnd();
    }

    public interface INetworkObjectOwnerEventFunctions
    {
        public abstract void OnOwnerStart();
        public abstract void OnOwnerFixedUpdate();
        public abstract void OnOwnerPreUpdate();
        public abstract void OnOwnerUpdate();
        public abstract void OnOwnerLateUpdate();
        public abstract void OnOwnerEnd();
    }
}
