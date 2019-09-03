# CoreSystemの解説と使用方法
## SceneLoader
シーンの読み込みを行うスクリプト。CoreSystemの`Awake()`で初期化されています。

### 使用方法
```
SceneLoader sl = SceneLoader.Instance; // インスタンスされたSceneLoaderを取得します
sl.LoadScene(SceneLoader.Scenes);      // 引数に設定したシーンの読み込みを開始します
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
