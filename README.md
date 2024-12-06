# **hakoniwa-sim-csharp**

このリポジトリでは、Unityアプリケーションを箱庭アセットとして活用するための機能を提供します。
箱庭（Hakoniwa）は、仮想環境でロボットやIoTデバイスをシミュレートするための強力なプラットフォームです。

## 箱庭の特徴

箱庭は現実世界と仮想環境を統合し、すべてを箱庭アセットという概念で管理するシミュレーションプラットフォームです。主な特徴は以下の通りです：

- **箱庭が提供する主要機能**
  - [箱庭時刻同期](https://github.com/toppers/hakoniwa-core-cpp-client/tree/main/math)
  - [箱庭PDU通信機能](https://github.com/toppers/hakoniwa-core-cpp-client)
- **箱庭PDU通信の特徴**
  - データ定義の標準化/抽象化: ROS IDLベースでPDUデータを定義。
  - データ通信APIの標準化: 統一された箱庭PDU APIを提供。
  - 通信プロトコルの柔軟性: 使用するプロトコルを選択可能（例：WebSocket、共有メモリなど）。


## **概要**

以下の図は、このリポジトリが提供する**箱庭Unityフレームワーク**のアーキテクチャを示しています。

![Hakoniwa Unity Architecture](https://github.com/user-attachments/assets/8e95d5cc-e2e1-46ee-9b6a-455eb13d5dd3)

- 最上位層には、ユーザーが作成するUnityアプリケーションがあります。
- Hakoniwa PDU APIを通じて、外部ソフトウェアとPDUデータを交換可能です。
- 追加機能として、以下のオプションが利用可能です：
  - 共有メモリベースの通信: Linuxサーバーなどでの高速通信に最適。
  - 箱庭時刻同期機能: 分散シミュレーションにおける正確な時間管理を実現。

## **利用シーン別に選べる通信方式**

- **UnityアプリケーションをAndroid/iOSに組み込む場合**： [PDU通信パッケージ](https://github.com/toppers/hakoniwa-pdu-csharp)を利用することで、モバイル環境でも簡単にPDU通信を実現可能です。
- **Linuxサーバーでの高速分散シミュレーション**： 共有メモリベースのPDU通信と時刻同期機能を活用することで、大規模なシミュレーションを効率的に実行できます（本リポジトリが該当機能を提供）。

---

## **前提条件**

以下の環境で動作を確認しています：
- **OS**: macOS(AppleSilicon)
- **Unity**: 2022.3.31f1
- **依存ライブラリ**:
  - WebSocketサポート
  - Hakoniwa Core Library (`libshakoc.so`)

事前に箱[庭コア機能](https://github.com/toppers/hakoniwa-core-cpp-client)をインストールしてください：


---

## **セットアップ**

- あなたのUnityプロジェクトを開きます。
- `Package Manager` の `Add package from git URL...` をクリックし、以下を指定します。
  - https://github.com/toppers/hakoniwa-sim-csharp.git
- 成功すると、以下の２つのパッケージがインストールされます。
  - ![スクリーンショット 2024-12-06 15 30 10](https://github.com/user-attachments/assets/7aa75072-bb9f-405b-87cb-ff4141238219)


---

## **基本的な使い方**

### 1. **PDU APIのセットアップ**
Hakoniwa PDU APIを利用して通信の初期化を行います。

以下は、UnityでのPDU初期化サンプルコードです：
```csharp
using Hakoniwa.Core;

void Start()
{
    // PDUマネージャの初期化
    PduManager pdu = PduManager.Create("example_pdu");
    pdu.SetData(new byte[] { 0x01, 0x02, 0x03 }); // データ送信
}
```

### 2. **通信プロセスの実行**
WebSocket通信を有効化し、箱庭のシミュレーションとデータをやり取りします。

---

## **サンプルプロジェクト**
本リポジトリには、以下のサンプルプロジェクトが含まれています：
1. **基本シーン**: 箱庭とのPDU通信を確認するシンプルなUnityシーン。
2. **高度な例**: 複雑なPDUデータや時間同期を伴うプロジェクト例。

サンプルの使い方：
```bash
# サンプルプロジェクトの起動
$ python run_simulation.py
```
