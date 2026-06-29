#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DOCS_DIR="$(dirname "$SCRIPT_DIR")"
REPO_ROOT="$(dirname "$DOCS_DIR")"
API_OUTPUT_DIR="$DOCS_DIR/src/content/docs/api"
SOURCE_BASE="https://github.com/RocketSurgeonsGuild/Conventions/blob/main/src"

# Locate xmldocmd — prefer mise shim, then dotnet tools global, then PATH
if command -v xmldocmd &>/dev/null; then
  XMLDOCMD="xmldocmd"
elif [[ -x "$HOME/.local/share/mise/shims/xmldocmd" ]]; then
  XMLDOCMD="$HOME/.local/share/mise/shims/xmldocmd"
elif [[ -x "$HOME/.dotnet/tools/xmldocmd" ]]; then
  XMLDOCMD="$HOME/.dotnet/tools/xmldocmd"
else
  echo "ERROR: xmldocmd not found. Install with: dotnet tool install -g xmldocmd" >&2
  exit 1
fi

# Map: src/ directory name → api/ slug
declare -A PACKAGES=(
  ["Conventions"]="conventions"
  ["Conventions.Abstractions"]="abstractions"
  ["Conventions.Analyzers"]="analyzers"
  ["Conventions.Autofac"]="autofac"
  ["Conventions.Configuration.Json"]="configuration-json"
  ["Conventions.Configuration.Yaml"]="configuration-yaml"
  ["Conventions.Diagnostics"]="diagnostics"
  ["Conventions.DryIoc"]="dryioc"
  ["Conventions.Serilog"]="serilog"
  ["CommandLine"]="commandline"
  ["Hosting"]="hosting"
  ["Web.Hosting"]="web-hosting"
  ["WebAssembly.Hosting"]="webassembly-hosting"
  ["Aspire.Hosting"]="aspire-hosting"
  ["Aspire.Hosting.Testing"]="aspire-hosting-testing"
)

# Build all src/ packages first (uses Conventions.src.slnf to avoid the full solution)
echo "==> Building all src/ packages in Release configuration..."
dotnet build "$REPO_ROOT/Conventions.src.slnf" -c Release --nologo

echo ""
echo "==> Generating API reference docs..."

generated=0
skipped=0
fallback_generated=0

for PKG in "${!PACKAGES[@]}"; do
  SLUG="${PACKAGES[$PKG]}"
  PKG_DIR="$REPO_ROOT/src/$PKG"

  if [[ ! -d "$PKG_DIR" ]]; then
    echo "  SKIP  $PKG — directory does not exist"
    ((skipped++)) || true
    continue
  fi

  CSPROJ=$(find "$PKG_DIR" -maxdepth 1 -name "*.csproj" | head -1 || true)
  if [[ -z "$CSPROJ" ]]; then
    echo "  SKIP  $PKG — project file not found"
    ((skipped++)) || true
    continue
  fi

  EXPECTED_ASSEMBLY=$(sed -n 's:.*<AssemblyName>\([^<]*\)</AssemblyName>.*:\1:p' "$CSPROJ" | head -1)
  if [[ -z "$EXPECTED_ASSEMBLY" ]]; then
    EXPECTED_ASSEMBLY="$(basename "$CSPROJ" .csproj)"
  fi

  # Find DLL in preference order. We try net10.0 first because most packages target that.
  # If xmldocmd cannot load it in the current environment, we fall back to XML-only generation.
  DLL=""
  for TFM in net10.0 net8.0 netstandard2.0; do
    TFM_DIR="$PKG_DIR/bin/Release/$TFM"
    CANDIDATE="$TFM_DIR/$EXPECTED_ASSEMBLY.dll"
    if [[ -f "$CANDIDATE" ]]; then
      DLL="$CANDIDATE"
      break
    fi
  done

  if [[ -z "$DLL" ]]; then
    echo "  SKIP  $PKG — no DLL found (tried net10.0, net8.0, netstandard2.0)"
    ((skipped++)) || true
    continue
  fi

  XML="${DLL%.dll}.xml"
  if [[ ! -f "$XML" ]]; then
    echo "  SKIP  $PKG — XML doc file missing: $XML"
    ((skipped++)) || true
    continue
  fi

  OUT_DIR="$API_OUTPUT_DIR/$SLUG"
  mkdir -p "$OUT_DIR"
  rm -rf "${OUT_DIR:?}"/*

  echo "  GEN   $PKG → api/$SLUG/"
  if "${XMLDOCMD}" "$DLL" "$OUT_DIR/" \
    --source "$SOURCE_BASE/$PKG/" \
    --skip-compiler-generated \
    --skip-unbrowsable \
    --clean; then
    ((generated++)) || true
  else
    echo "  WARN  $PKG — xmldocmd failed, using XML fallback generator"
    node "$DOCS_DIR/scripts/generate-api-from-xml.mjs" "$XML" "$OUT_DIR/"
    ((generated++)) || true
    ((fallback_generated++)) || true
  fi
done

echo ""
echo "==> Injecting Starlight frontmatter..."
node "$DOCS_DIR/scripts/add-api-frontmatter.mjs"

echo ""
echo "==> Done. Generated: $generated package(s), skipped: $skipped, fallback-used: $fallback_generated."
