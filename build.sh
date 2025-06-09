#!/bin/bash
set -e

# === Config ===
MOD_NAME="SafeWarLogPatch"
BANNERLORD_GAME_DIR="$HOME/.local/share/Steam/steamapps/common/Mount & Blade II Bannerlord"
PROJECT_DIR="$(pwd)"
BUILD_DIR="$PROJECT_DIR/bin"
PACKAGE_DIR="$PROJECT_DIR/_Package/$MOD_NAME"
ZIP_FILE="$PROJECT_DIR/${MOD_NAME}.zip"
REFS_DIR="$PROJECT_DIR/refs"
HARMONY_DLL="$PROJECT_DIR/packages/0Harmony.dll"

export BANNERLORD_GAME_DIR

source ~/.bashrc

# === Step 1: Prepare DLL references ===
echo "📁 Setting up TaleWorlds and Harmony references..."
mkdir -p "$REFS_DIR" "$PROJECT_DIR/packages"

# Optional: if not using ReferenceAssemblies, copy TW refs
cp -u "$BANNERLORD_GAME_DIR/bin/Win64_Shipping_Client/TaleWorlds."*.dll "$REFS_DIR/"

# Download Harmony if missing
if [ ! -f "$HARMONY_DLL" ]; then
  echo "⬇ Downloading HarmonyLib..."
  curl -L -o "packages/HarmonyLib.2.2.2.nupkg" https://www.nuget.org/api/v2/package/Lib.Harmony/2.2.2
  unzip -j "packages/HarmonyLib.2.2.2.nupkg" 'lib/net472/0Harmony.dll' -d packages
  mv "packages/0Harmony.dll" "$HARMONY_DLL"
fi

# ls "$BANNERLORD_GAME_DIR/bin/Win64_Shipping_Client/"

# === Step 2: Build ===
#dotnet clean
msbuild /t:Clean
#dotnet restore
msbuild /t:Restore SafeWarLogPatch.csproj
echo "🔧 Building mod..."
#dotnet build "$PROJECT_DIR/$MOD_NAME.csproj" -c Release /p:BANNERLORD_GAME_DIR="$BANNERLORD_GAME_DIR"
msbuild "$PROJECT_DIR/$MOD_NAME.csproj" /p:Configuration=Release /p:TargetFramework=net472 /p:BANNERLORD_GAME_DIR="$BANNERLORD_GAME_DIR"

# === Step 3: Package ===
echo "🧹 Cleaning old package..."
rm -rf "$PACKAGE_DIR" "$ZIP_FILE"

echo "📦 Creating package structure..."
mkdir -p "$PACKAGE_DIR/bin/Win64_Shipping_Client"
cp "$BUILD_DIR/$MOD_NAME.dll" "$PACKAGE_DIR/bin/Win64_Shipping_Client/"
[ -f "$BUILD_DIR/$MOD_NAME.pdb" ] && cp "$BUILD_DIR/$MOD_NAME.pdb" "$PACKAGE_DIR/bin/Win64_Shipping_Client/"
cp "$PROJECT_DIR/SubModule.xml" "$PACKAGE_DIR/"
[ -d "$PROJECT_DIR/ModuleData" ] && cp -r "$PROJECT_DIR/ModuleData" "$PACKAGE_DIR/"
cp "$BUILD_DIR/0Harmony.dll" "$PACKAGE_DIR/bin/Win64_Shipping_Client/"


# === Step 4: Zip ===
echo "🗜️ Creating ZIP archive..."
cd "$PROJECT_DIR/_Package"
zip -rq "../$MOD_NAME.zip" "$MOD_NAME"
cd "$PROJECT_DIR"

# === Step 5: Install to Bannerlord Modules ===
MOD_DIR="$BANNERLORD_GAME_DIR/Modules/$MOD_NAME"

echo "🚮 Removing old mod from Modules/..."
rm -rf "$MOD_DIR"

echo "📂 Installing new mod..."
mkdir -p "$MOD_DIR/bin/Win64_Shipping_Client"
cp -r "$PACKAGE_DIR/"* "$MOD_DIR/"


echo "✅ Mod installed to: $MOD_DIR"
