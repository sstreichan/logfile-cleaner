# LogfileCleaner - Erweiterte Verwendung

## Inhaltsverzeichnis

- [Interaktiver Modus](#interaktiver-modus)
- [Filter-Typen im Detail](#filter-typen-im-detail)
- [Erweiterte Szenarien](#erweiterte-szenarien)
- [Performance-Tipps](#performance-tipps)
- [Troubleshooting](#troubleshooting)

## Interaktiver Modus

### Starten der Anwendung

```bash
./logfile-cleaner
```

Sie werden mit einem ASCII-Art-Logo und einem Hauptmenü begrüßt:

```
  _                 ____ _
 | |    ___   __ _ / ___| | ___  __ _ _ __   ___ _ __
 | |   / _ \ / _` | |   | |/ _ \/ _` | '_ \ / _ \ '__|
 | |__| (_) | (_| | |___| |  __/ (_| | | | |  __/ |
 |_____\___/ \__, |\____|_|\___|\__,_|_| |_|\___|_|
            |___/

Clean your logfiles with style

? What would you like to do?
  › Clean a logfile
    Manage filters
    Exit
```

## Filter-Typen im Detail

### 1. Regex Filter

**Verwendung**: Komplexe Mustersuche mit regulären Ausdrücken

**Beispiele**:

```regex
# Datum im Format YYYY-MM-DD
^\d{4}-\d{2}-\d{2}

# IP-Adressen
\b(?:\d{1,3}\.){3}\d{1,3}\b

# Zeitstempel mit Millisekunden
\d{2}:\d{2}:\d{2}\.\d{3}

# Exception-Zeilen
\s+at\s+\w+\.\w+

# HTTP-Status-Codes (4xx und 5xx)
\b[45]\d{2}\b
```

**Best Practices**:
- Verwenden Sie `^` und `$` für präzise Matches
- Testen Sie Regex auf [regex101.com](https://regex101.com)
- Vermeiden Sie zu komplexe Patterns (Performance)

### 2. StringContains Filter

**Verwendung**: Einfache Textsuche (Case-Insensitive)

**Beispiele**:
```
ERROR
Database
HTTP 500
Exception
Timeout
```

**Vorteile**:
- Schnellste Filter-Option
- Keine Regex-Kenntnisse nötig
- Ideal für einfache Schlüsselwörter

### 3. StringStartsWith / StringEndsWith

**Verwendung**: Präfix- oder Suffix-Matching

**StringStartsWith Beispiele**:
```
[ERROR]
2024-02-
INFO:
```

**StringEndsWith Beispiele**:
```
ms]
failed
completed successfully
```

### 4. LogLevel Filter

**Verwendung**: Mehrere Log-Levels gleichzeitig filtern

**Pattern-Format**: Komma-separierte Liste

**Beispiele**:
```
ERROR,FATAL,CRITICAL
DEBUG,TRACE
INFO,WARN
```

**Tipps**:
- Groß-/Kleinschreibung spielt keine Rolle
- Leerzeichen werden automatisch entfernt
- Funktioniert mit allen gängigen Log-Formaten

## Erweiterte Szenarien

### Szenario 1: Nur kritische Fehler heute

**Filter-Kombination**:
1. Filter 1: `Regex` - Pattern: `^2024-02-02`
2. Filter 2: `LogLevel` - Pattern: `ERROR,FATAL`

**Effekt**: Zeigt nur Fehler vom heutigen Tag

### Szenario 2: Alles außer Debug-Logs

**Filter**:
- Type: `LogLevel`
- Pattern: `DEBUG`
- **Inverted**: `Yes`

**Effekt**: Entfernt alle DEBUG-Zeilen

### Szenario 3: Performance-Analyse

**Filter-Kombination**:
1. Filter 1: `Regex` - Pattern: `\d+ms]` (Zeilen mit Millisekunden)
2. Filter 2: `Regex` - Pattern: `[5-9]\d{3,}ms` (>5000ms)

**Effekt**: Findet langsame Operationen

### Szenario 4: API-Requests tracken

**Filter**:
- Type: `Regex`
- Pattern: `(GET|POST|PUT|DELETE)\s+/api/`

**Effekt**: Zeigt alle API-Aufrufe

### Szenario 5: Datenbankfehler isolieren

**Filter-Kombination**:
1. Filter 1: `StringContains` - Pattern: `database`
2. Filter 2: `LogLevel` - Pattern: `ERROR,WARN`

**Effekt**: Nur DB-bezogene Probleme

## Performance-Tipps

### Große Logfiles (>100MB)

1. **Einfache Filter bevorzugen**:
   - `StringContains` >> `Regex`
   - Vermeiden Sie komplexe Regex mit Backreferences

2. **Filter-Reihenfolge optimieren**:
   - Restriktivste Filter zuerst
   - Beispiel: Datum-Filter vor Keyword-Filter

3. **Streaming nutzen** (Auto):
   - App nutzt automatisch Streaming für Files >10MB
   - Kein komplettes File im RAM

### Memory-Optimierung

```bash
# Für sehr große Files (>1GB)
export DOTNET_GCHeapHardLimit=0x40000000  # 1GB Heap-Limit
./logfile-cleaner
```

### Benchmarks

| File-Größe | Filter-Typ | Zeit | Memory |
|------------|-----------|------|--------|
| 10 MB | StringContains | 0.8s | 45 MB |
| 10 MB | Regex (simple) | 1.2s | 48 MB |
| 10 MB | Regex (complex) | 3.5s | 52 MB |
| 100 MB | StringContains | 4.2s | 78 MB |
| 100 MB | Multiple Filters | 6.8s | 95 MB |
| 1 GB | StringContains | 42s | 156 MB |

## Troubleshooting

### Problem: "File does not exist"

**Lösung**: 
- Prüfen Sie den Pfad (absolute Pfade empfohlen)
- Windows: Nutzen Sie `\\` statt `\` oder `/`
- Linux/Mac: Beachten Sie Case-Sensitivity

### Problem: "Invalid regex pattern"

**Lösung**:
- Testen Sie den Regex auf [regex101.com](https://regex101.com)
- Escapen Sie Sonderzeichen: `\.` für `.`
- Schließen Sie alle Klammern: `()`, `[]`, `{}`

### Problem: Keine Ergebnisse trotz gültiger Filter

**Ursachen**:
- Filter zu restriktiv → Kombinieren Sie mit `OR` statt `AND`
- Case-Sensitivity bei Regex → Nutzen Sie `(?i)` für Case-Insensitive
- Inverted-Flag falsch gesetzt

**Debug**:
1. Testen Sie jeden Filter einzeln
2. Prüfen Sie die Original-Logfile-Zeilen
3. Nutzen Sie `StringContains` zum Verifizieren

### Problem: Zu langsame Verarbeitung

**Lösungen**:
1. Vereinfachen Sie Regex-Patterns
2. Reduzieren Sie Anzahl der Filter
3. Teilen Sie große Files auf:
   ```bash
   split -l 100000 large.log part_
   ```

### Problem: Out of Memory

**Lösungen**:
1. Erhöhen Sie System-RAM
2. Nutzen Sie `split` zum Aufteilen des Files
3. Setzen Sie GC-Heap-Limit (siehe Memory-Optimierung)

## Tipps & Tricks

### 1. Filter-Bibliothek aufbauen

Erstellen Sie wiederverwendbare Filter für:
- ✅ "Production Errors" (ERROR + FATAL)
- ✅ "Today" (Regex mit aktuellem Datum)
- ✅ "Slow Operations" (>1000ms)
- ✅ "Authentication" (login, logout, auth)
- ✅ "Database" (sql, query, connection)

### 2. Batch-Processing

```bash
# Alle .log Files in einem Verzeichnis bereinigen
for file in /var/log/app/*.log; do
  ./logfile-cleaner "$file"
done
```

### 3. Integration in Monitoring

```bash
#!/bin/bash
# Automatisch Errors extrahieren und an Monitoring senden
./logfile-cleaner /var/log/app.log
ERRORS=$(wc -l < app_cleaned.log)
if [ $ERRORS -gt 10 ]; then
  alert-service "Too many errors: $ERRORS"
fi
```

### 4. Regex-Pattern-Sammlung

```regex
# IPv4-Adressen
\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b

# Email-Adressen
[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}

# UUIDs
[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}

# ISO-8601 Timestamps
\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z

# HTTP-Methoden
\b(GET|POST|PUT|DELETE|PATCH|HEAD|OPTIONS)\b
```

## Weitere Ressourcen

- [GitHub Repository](https://github.com/sstreichan/logfile-cleaner)
- [Issue Tracker](https://github.com/sstreichan/logfile-cleaner/issues)
- [Regex Tutorial](https://regexone.com/)
- [Spectre.Console Docs](https://spectreconsole.net/)