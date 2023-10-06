# unity-ngo-manager

`Unity Netcode for GameObjects`の`NetworkObject`を管理するライブラリ

## Features

- Spawn/Despawn時のNetworkObjectを自動管理
- 関数ベースで処理を分けることが可能
- Client側からSpawn(AsPlayerObject/WithOwnership)Async関数を呼ぶことが可能
- Client側からSpawn済みObjectListを取得可能
- Client側から(Local/Remote)Playerを取得可能
- (Local/Remote)PlayerSpawn時のAction(Callback)を追加可能
- GenericNetworkStateMachineに対応

## Instllation

UnityPackageManagerの`Add package from git URL`から以下のURLを追加

- `https://github.com/shirokuma1101/com.unity.netcode.gameobjects.git?path=/com.unity.netcode.gameobjects#1.7.0-extension`

- `https://github.com/shirokuma1101/unity-ngo-manager.git?path=/Assets/unity-ngo-manager`

⚠ このライブラリは`Unity Netcode for GameObjects`のFork先のBranch`1.5.1-extension`の機能を利用しているため、公式Packageでは機能しません。

## Usage

```cs
using System;
using NGOManager;
using Unity.Netcode;
using UnityEngine;

public class SamplePlayer : NetworkObjectBase
{
    public float MoveSpeed { get; set; }

    public override void OnOwnerStart()
    {
        // This function is called only on the owner.
        // etc:
        //      Instantiate a camera that follows the player.

    }

    public override void OnHostStart()
    {
        // This function is called only on the host.
        // etc:
        //      Starting the game progress timer.
    }

    public class ASSampleBase : GenericNetworkStateMachine.StateBase
    {
        public new SamplePlayer Owner { get; private set; }

        public override void Initialize(GenericNetworkStateMachine stateMachine)
        {
            Owner = stateMachine.GetComponent<SamplePlayer>();
            // Don't forget to set base owner.
            base.Owner = Owner;
        }
    }

    // Idle state
    [Serializable]
    public class ASSampleIdle : ASSampleBase
    {
        public override void OnOwnerEnter()
        {
            Owner.MoveSpeed = 0;
        }
    }

    // Walk state
    [Serializable]
    public class ASSampleWalk : ASSampleBase
    {
        public override void OnOwnerEnter()
        {
            Owner.MoveSpeed = 1;
        }

        public override void OnOwnerFixedUpdate()
        {
            Owner.transform.Translate(Owner.MoveSpeed, 0, 0);
        }

        public override void OnOwnerExit()
        {
            Owner.MoveSpeed = 0;
        }
    }
}

public class SampleGame : NetworkBehaviour
{
    [SerializeField]
    private NetworkObject playerPrefab;
    [SerializeField]
    private NetworkObject equipmentPrefab;
    [SerializeField]
    private NetworkObject enemyPrefab;

    private async void Start()
    {
        // Initialize NetworkObjectManager.
        NetworkObjectManager.Instance.Initialize();

        // Add on all players spawned event.
        NetworkObjectManager.Instance.OnAllPlayersSpawned += () =>
        {
            // This callback is invoked when all players are spawned.
            // etc:
            //      Start the game.
        };

        await NetworkObjectSpawner.SpawnAsPlayerObjectAsync(
            playerPrefab, Vector3.zero, Quaternion.identity, NetworkManager.Singleton.LocalClientId);
        await NetworkObjectSpawner.SpawnWithOwnershipAsync(
            equipmentPrefab, Vector3.zero, Quaternion.identity, NetworkManager.Singleton.LocalClientId);
        await NetworkObjectSpawner.SpawnAsync(
            enemyPrefab, Vector3.zero, Quaternion.identity);

        Debug.Log($"Connected client count: {NetworkObjectManager.Instance.ConnectedClientCount}");
        Debug.Log($"NetworkObjectBases count: {NetworkObjectManager.Instance.NetworkObjectBases.Count}");
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        NetworkObjectManager.Instance.Shutdown();
    }
}
```
