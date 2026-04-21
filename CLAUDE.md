# 7dtd-shared-storage — CLAUDE.md

## 案件概要

- **案件名**: 7DTD Shared Storage
- **クライアント**: 個人開発（Nexus Mods / GitHub 公開）
- **ステータス**: 進行中
- **開始日**: 2026-04-21

## 要件

7 Days to Die マルチプレイで、複数プレイヤーが同じストレージコンテナを同時に開けるようにする MOD。

- バニラでは 1 人が開くとロックされ他のプレイヤーが弾かれる
- 本 MOD 導入後は何人でも同じコンテナを同時に開ける
- 対象: ストレージボックス・ルートコンテナ・カーゴボックスなど全ストレージ

## ターゲットバージョン

- **7 Days to Die**: V2.6（最新）
- **参照 DLL**: `projects/mod/_reference/7dtd-managed/Assembly-CSharp.dll`（sdtd-test より、V2.6 b14）
- **Harmony**: HarmonyX（`0_TFP_Harmony` 経由）

## 技術スタック

- **C# + HarmonyX**: `TileEntityLootContainer.IsUserAccessing` のパッチ
- **ビルド**: .NET / MSBuild（`dotnet build`）

## ディレクトリ構成

```
7dtd-shared-storage/
└── SharedStorage/           同時アクセス許可 modlet
    ├── ModInfo.xml
    ├── SharedStorage.csproj
    └── Harmony/
        └── SharedStorage.cs Harmony パッチ
```

## 実装方針

### ロック機構

```
Player A が開く → lockedByEntityID = A.entityId
Player B が開こうとする → IsUserAccessing(B.entityId) → true → ブロック
```

### パッチ

```
TileEntityLootContainer.IsUserAccessing → 常に false を返す → 全プレイヤーが開ける
```

### アイテム競合について

7DTD はサーバー権威型。2 人が同じアイテムを同時に取ろうとしてもサーバーが解決し、正しい状態を両クライアントに送信するためアイテム重複・消失は発生しない。

## メモ

- コード修正は必ず GitHub Issue を発行してから着手する
- テスト環境: game01 の sdtd-test.service（Port 26901, V2.6 b14）
