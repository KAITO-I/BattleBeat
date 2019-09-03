# CoreSystemの解説と使用方法
以下で紹介するスクリプトはすべて、`CoreSystem.cs` の `Awake()` で初期化されています。

## SceneLoader
シーンの読み込みを行うスクリプトです。

### 使用方法
```
SceneLoader sl = SceneLoader.Instance; // インスタンスされたSceneLoaderを取得します。
sl.LoadScene(SceneLoader.Scenes);      // 引数に設定したシーンの読み込みを開始します。
```

### enum SceneLoader.Scenes

|列挙値|シーン概要|
|---|---|
|Title|タイトル画面|
|MainMenu|モード選択|
|CharacterSelect|キャラクター選択|
|MainGame|インゲーム|
|Result|リザルト|
|Training|練習モード|

※登録されていないシーンには、この関数からは飛ぶことができません。

#### Scenesには残ってるけど使わないシーン

|列挙値|シーン概要|理由|
|---|---|---|
|Config|設定画面|モード選択に統合|
|Credit|クレジット画面|モード選択に統合|

※上記は使用しません。削除次第、こちらも反映します。

## SoundManager
音楽再生、音量設定を行うスクリプトです。

### 使用方法
```
SoundManager sm  = SoundManager.Instance; // インスタンスされたSoundManagerを取得します。
Sound        bgm = sm.BGM;  // BGMのスクリプトを呼び出します。
SoundEffect  se  = sm.SE; // SEのスクリプトを呼び出します。
```

#### Soundリファレンス
```
Sound Play(AudioClip);
```
BGMを再生します。

※何らかの音楽を再生している場合で同じ音源の場合は実行されません（継続再生）。
違う音源の場合は上書きして実行されます。

**返り値**：BGMを再生しているクラスを返します。

```
void  Stop();          // BGMを停止します。
```
※`Play(AidioClip)`：BGMにおいて

#### SoundEffectリファレンス
```
Sound Play(AudioClip);     // SEを再生します。`Instantiate()` でクローンされた自身を返します。
void  Stop();              // SEを停止します。同時に、自身がアタッチされているオブジェクトを `Destroy()` で削除します。
```
※`Play(AudioClip)`：SEにおいて、何らかの音楽を再生している場合で同じ音源でも複数実行できます。
