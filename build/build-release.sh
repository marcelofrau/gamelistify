#!/usr/bin/env bash
set -euo pipefail

VERSION="${1:?version required}"
RID="${2:-linux-x64}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PROJECT="$ROOT/Gamelistify/Gamelistify.csproj"
DIST="$ROOT/build/dist"
PUBLISH_DIR="$DIST/publish-$RID"
ZIP_NAME="Gamelistify-v${VERSION#v}-$RID.zip"
ZIP_PATH="$DIST/$ZIP_NAME"

mkdir -p "$DIST"

dotnet publish "$PROJECT" \
  -c Release \
  -r "$RID" \
  --self-contained true \
  -p:Version="${VERSION#v}" \
  -p:InformationalVersion="${VERSION#v}" \
  -o "$PUBLISH_DIR"

rm -f "$ZIP_PATH"
cd "$PUBLISH_DIR"
zip -qr "$ZIP_PATH" .
echo "Release created: $ZIP_PATH"
