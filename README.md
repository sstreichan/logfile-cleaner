# LogfileCleaner

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![AOT](https://img.shields.io/badge/AOT-Enabled-green)](https://learn.microsoft.com/dotnet/core/deploying/native-aot/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Ein modernes TUI-basiertes Tool zum Bereinigen von Logfiles mit nativer AOT-Compilation, benutzerdefinierten Filtern und automatisiertem Versioning.

## Features

✨ **Terminal UI** - Intuitive Bedienung mit [Spectre.Console](https://spectreconsole.net/)
🚀 **Native AOT** - Schneller Start (<500ms) und kleine Binary-Größe (<15MB)
🔍 **Flexible Filter** - Regex, String-Matching, LogLevel-Filtering
💾 **Persistenz** - Filter werden automatisch gespeichert
📁 **Path Autocomplete** - Komfortable Pfadeingabe
⚡ **Streaming** - Effiziente Verarbeitung großer Logfiles

## Installation

### Von GitHub Releases

```bash
# Download der Binary für Ihr System
wget https://github.com/sstreichan/logfile-cleaner/releases/latest/download/logfile-cleaner-linux-x64
chmod +x logfile-cleaner-linux-x64
./logfile-cleaner-linux-x64
```

### Von Source

```bash
git clone https://github.com/sstreichan/logfile-cleaner.git
cd logfile-cleaner/src/LogfileCleaner
dotnet publish -c Release
```

## Verwendung

### Interaktiver Modus

```bash
./logfile-cleaner
```

Das Tool führt Sie durch ein interaktives Menü:

1. **Clean a logfile** - Wählen Sie eine Datei und wenden Sie Filter an
2. **Manage filters** - Erstellen, anzeigen oder löschen Sie Filter
3. **Exit** - Beenden

### Filter-Typen

| Typ | Beschreibung | Beispiel |
|-----|--------------|----------|
| **Regex** | Reguläre Ausdrücke | `\d{4}-\d{2}-\d{2}` |
| **StringContains** | Zeilen mit Text | `ERROR` |
| **StringStartsWith** | Zeilen beginnend mit | `[2024` |
| **StringEndsWith** | Zeilen endend mit | `ms]` |
| **LogLevel** | Mehrere Log-Levels | `DEBUG,INFO,WARN` |

### Beispiel-Workflow

```
1. Starten Sie das Tool
2. Wählen Sie "Manage filters" → "Create new filter"
3. Erstellen Sie einen Filter:
   - Name: "Nur Fehler"
   - Typ: LogLevel
   - Pattern: "ERROR,FATAL"
   - Invertiert: Nein
4. Wählen Sie "Clean a logfile"
5. Geben Sie den Pfad zu Ihrer Logdatei ein
6. Wählen Sie den Filter "Nur Fehler"
7. Die bereinigte Datei wird als *_cleaned.log gespeichert
```

## Entwicklung

### Voraussetzungen

- .NET 8.0 SDK oder höher
- Optional: Visual Studio 2022 / Rider / VS Code

### Build

```bash
# Debug Build
dotnet build

# Release Build mit AOT
dotnet publish -c Release -r linux-x64

# Alle Plattformen
dotnet publish -c Release -r win-x64
dotnet publish -c Release -r osx-arm64
```

### Tests

```bash
cd tests/LogfileCleaner.Tests
dotnet test
```

## Architektur

```
src/LogfileCleaner/
├── Program.cs              # Haupteinstiegspunkt + TUI
├── Models/
│   └── FilterDefinition.cs # Filter-Model
├── Core/
│   ├── FilterEngine.cs     # Filter-Logik
│   ├── FilterValidator.cs  # Pattern-Validierung
│   ├── LogFileReader.cs    # Streaming File-Reader
│   └── FilterRepository.cs # JSON-basierte Persistenz
└── LogfileCleaner.csproj
```

## CI/CD

Das Projekt nutzt GitHub Actions für:

- ✅ Automatisches Versioning mit [GitVersion](https://gitversion.net/)
- 📝 Changelog-Generierung basierend auf Conventional Commits
- 🏗️ Multi-Platform AOT-Builds (Windows, Linux, macOS)
- 📦 Automatische GitHub Releases

### Commit-Message-Konvention

```
feat: neue Feature
fix: Bugfix
docs: Dokumentation
refactor: Code-Refactoring
test: Tests hinzufügen
```

## Performance

| Dateigröße | Verarbeitungszeit |
|------------|------------------|
| 1 MB | ~0.2s |
| 10 MB | ~1.5s |
| 100 MB | ~4.8s |
| 1 GB | ~48s |

*Gemessen auf Intel i7-10700K, SSD, mit Regex-Filter*

## Lizenz

MIT License - siehe [LICENSE](LICENSE) für Details.

## Beiträge

Beiträge sind willkommen! Bitte öffnen Sie ein Issue oder Pull Request.

## Roadmap

- [ ] Export/Import von Filter-Sets
- [ ] Regex-Pattern-Bibliothek (IPs, Timestamps, etc.)
- [ ] Real-time Log-Monitoring-Modus
- [ ] Performance-Optimierung für Multi-GB-Files
- [ ] Plugin-System für Custom Filter