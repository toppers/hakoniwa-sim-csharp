#!/bin/bash

# スクリプトの目的: Plugins/ ディレクトリ内のネイティブライブラリを更新します。
# 使い方: ./update_plugins.sh <ライブラリが格納されているディレクトリへのパス>
#
# 例:
# ./update_plugins.sh /path/to/my/new_libs

set -e

# 引数が1つ指定されているかチェック
if [ "$#" -ne 1 ]; then
    echo "エラー: ライブラリが格納されているディレクトリを引数に指定してください。"
    echo "使い方: $0 <source_directory>"
    exit 1
fi

SOURCE_DIR="$1"
# スクリプトが置かれているディレクトリを基準にプロジェクトルートを決定
PROJECT_ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

DEST_DIR_ARM64="$PROJECT_ROOT/Plugins/arm64"
DEST_DIR_LINUX="$PROJECT_ROOT/Plugins/linux"
DEST_DIR_WIN="$PROJECT_ROOT/Plugins/win"

# コピー元ディレクトリの存在チェック
if [ ! -d "$SOURCE_DIR" ]; then
    echo "エラー: コピー元ディレクトリが見つかりません: $SOURCE_DIR"
    exit 1
fi

# ライブラリをコピーする関数
# 第1引数: ファイル名
# 第2引数: コピー先ディレクトリ
copy_if_exists() {
    if [ -f "$SOURCE_DIR/$1" ]; then
        echo "コピー中: $1 -> $2/"
        cp "$SOURCE_DIR/$1" "$2/"
    else
        echo "警告: $1 がコピー元ディレクトリに見つかりません。スキップします。"
    fi
}

# 各OSのライブラリをコピー
echo "ライブラリの更新を開始します..."
copy_if_exists "libconductor.dylib" "$DEST_DIR_ARM64"
copy_if_exists "libshakoc.dylib"    "$DEST_DIR_ARM64"
copy_if_exists "libshakoc.so"       "$DEST_DIR_LINUX"
copy_if_exists "shakoc.dll"         "$DEST_DIR_WIN"

echo "ライブラリの更新が完了しました。"
