# unity-ngo-manager

`Unity Netcode for GameObjects`の`NetworkObject`を管理するライブラリ

## Instllation

UnityPackageManagerのAdd package from git URLから以下のURLを追加

- `https://github.com/shirokuma1101/com.unity.netcode.gameobjects.git?path=/com.unity.netcode.gameobjects#1.5.1-extension`

- `https://github.com/shirokuma1101/unity-ngo-manager.git?path=/unity-ngo-manager`

⚠ このライブラリは`Unity Netcode for GameObjects`のFork先のBranch`1.5.1-extension`の機能を利用しているため、公式Packageでは機能しません。

## Features

- クライアント側からSpawn(AsPlayerObject/WithOwnership)Async関数を呼ぶ
- 関数ベースで処理を分ける
- GenericNetworkStateMachineに対応

## Usage

```cs
using System;
using NGOManager;
using Unity.Netcode;
using UnityEngine;

public class SamplePlayer : NetworkObjectBase
{
    [SerializeField]
    private GameObject cameraPrefab;

    public float MoveSpeed { get; set; }

    public override void OnOwnerStart()
    {
        Instantiate(cameraPrefab);
    }

    public override void OnHostStart()
    {
        TimerController.Instance.StartTimer();
    }

    public class ASSampleBase : GenericNetworkStateMachine.StateBase
    {
        protected SamplePlayer Owner { get; private set; }

        public override void Initialize(GenericNetworkStateMachine stateMachine)
        {
            Owner = stateMachine.GetComponent<SamplePlayer>();
            base.Owner = Owner;
        }
    }

    // 待機
    [Serializable]
    public class ASSampleIdle : ASSampleBase
    {
        public override void OnOwnerEnter()
        {
            Owner.MoveSpeed = 0;
        }
    }

    // 歩き
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

public class SampleController : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject equipmentPrefab;
    [SerializeField]
    private GameObject enemyPrefab;

    private async void Start()
    {
        await NetworkObjectSpawner.SpawnAsPlayerAsync(
            playerPrefab, Vector3.zero, Quaternion.identity, NetworkManager.Singleton.LocalClientId);
        await NetworkObjectSpawner.SpawnWithOwnershipAsync(
            equipmentPrefab, Vector3.zero, Quaternion.identity, NetworkManager.Singleton.LocalClientId);
        await NetworkObjectSpawner.SpawnAsync(
            enemyPrefab, Vector3.zero, Quaternion.identity);
    }
}
```
