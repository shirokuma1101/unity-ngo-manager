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
public class SamplePlayer : NetworkObjectBase
{
    [SerializeField]
    private GameObject cameraPrefab;


    public override void OnOwnerStart()
    {
        Instantiate(cameraPrefab);
    }

    public override void OnHostStart()
    {
        GameController.Instance.StartGame();
    }

    public override void OnOwnerUpdate()
    {
        // do something
    }
}
```
