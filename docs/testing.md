# テスト手順

## SharedStorage — game01 での動作確認

### 前提

- game01 に SSH 接続できること: `ssh -i ~/.ssh/private_infra root@162.43.7.73`
- game01 の `0_TFP_Harmony` が導入済み（確認済み）
- テストサーバー: sdtd-test.service（Port 26901, V2.6 b14）

### 1. DLL ビルド

```bash
cd projects/mod/7dtd-shared-storage/SharedStorage
/home/vscode/.dotnet/dotnet build SharedStorage.csproj
# → bin/SharedStorage.dll が生成される
```

### 2. game01 へ転送

```bash
ssh -i ~/.ssh/private_infra root@162.43.7.73 "mkdir -p /home/sdtd-test/7dtd_server/Mods/SharedStorage"

scp -i ~/.ssh/private_infra \
  SharedStorage/ModInfo.xml \
  root@162.43.7.73:/home/sdtd-test/7dtd_server/Mods/SharedStorage/

scp -i ~/.ssh/private_infra \
  SharedStorage/bin/SharedStorage.dll \
  root@162.43.7.73:/home/sdtd-test/7dtd_server/Mods/SharedStorage/
```

### 3. サーバー再起動

```bash
ssh -i ~/.ssh/private_infra root@162.43.7.73 "systemctl restart sdtd-test"
```

### 4. ゲーム内確認チェックリスト

- [ ] サーバーログに `[SharedStorage] Harmony patches applied.` が出る
- [ ] Player A がストレージボックスを開く
- [ ] Player B が**同じ**ストレージボックスを開ける（「使用中」メッセージが出ない）
- [ ] Player A がアイテムを取ると Player B の UI でも即座に消える
- [ ] Player B がアイテムを取ると Player A の UI でも即座に消える
- [ ] 同じアイテムを同時に取ろうとした場合、片方のみ取得できる（重複しない）

### 5. 対象コンテナ確認

- [ ] 木製ストレージボックス（woodenCrate）
- [ ] 金属ストレージボックス（metalCrate）
- [ ] カーゴボックス（cargoBox）
- [ ] バックパック（死亡時）
- [ ] ルートコンテナ（ゾンビドロップ等）

### 6. ログ確認

```bash
ssh -i ~/.ssh/private_infra root@162.43.7.73 \
  "grep -i 'SharedStorage\|IsUserAccessing' \
  \$(ls -t /home/sdtd-test/7dtd_server/output_log*.txt | head -1) | tail -10"
```

---

## トラブルシューティング

| 症状 | 対処 |
|------|------|
| まだ「使用中」が表示される | `IsUserAccessing` メソッド名が V2.6 で変わった可能性。ログでパッチエラーを確認 |
| アイテムが重複する | サーバーがシングルプレイ設定になっていないか確認 |
| DLL ロードエラー | `0_TFP_Harmony` が導入済みか確認 |
