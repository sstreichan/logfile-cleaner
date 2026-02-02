# Contributing to LogfileCleaner

Vielen Dank für Ihr Interesse an LogfileCleaner! Wir freuen uns über Beiträge.

## Entwicklungsumgebung

### Voraussetzungen

- .NET 8.0 SDK oder höher
- Git
- IDE Ihrer Wahl (Visual Studio, Rider, VS Code)

### Setup

```bash
# Repository klonen
git clone https://github.com/sstreichan/logfile-cleaner.git
cd logfile-cleaner

# Dependencies wiederherstellen
dotnet restore

# Build
dotnet build

# Tests ausführen
dotnet test
```

## Commit-Message-Konvention

Wir verwenden [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>: <description>

[optional body]

[optional footer]
```

### Types

- `feat`: Neues Feature
- `fix`: Bugfix
- `docs`: Nur Dokumentationsänderungen
- `style`: Code-Formatierung (keine funktionalen Änderungen)
- `refactor`: Code-Refactoring
- `test`: Tests hinzufügen oder ändern
- `chore`: Build-Prozess oder Tooling

### Beispiele

```
feat: add export filter functionality

fix: correct regex validation for edge cases

docs: update README with new filter examples

test: add unit tests for FilterEngine
```

## Pull Request Prozess

1. **Fork** das Repository
2. **Branch erstellen**: `git checkout -b feature/amazing-feature`
3. **Änderungen committen**: `git commit -m 'feat: add amazing feature'`
4. **Branch pushen**: `git push origin feature/amazing-feature`
5. **Pull Request öffnen**

### PR-Checkliste

- [ ] Code folgt dem bestehenden Stil
- [ ] Tests wurden hinzugefügt/aktualisiert
- [ ] Alle Tests laufen durch (`dotnet test`)
- [ ] Dokumentation wurde aktualisiert (falls nötig)
- [ ] Commit-Messages folgen der Konvention
- [ ] Keine Merge-Konflikte mit `main`

## Code-Standards

### C# Coding Style

- Verwenden Sie C# 12 Features wo sinnvoll
- Nullable Reference Types aktiviert
- Async/Await für I/O-Operationen
- LINQ für Collection-Operationen
- XML-Kommentare für public APIs

### Naming Conventions

```csharp
// Classes: PascalCase
public class FilterEngine { }

// Interfaces: IPascalCase
public interface IFilterRepository { }

// Methods: PascalCase
public void ApplyFilter() { }

// Properties: PascalCase
public string FilterName { get; set; }

// Private fields: _camelCase
private readonly string _configPath;

// Local variables: camelCase
var filterResult = ...;
```

## Testing

### Unit Tests

- Verwenden Sie xUnit als Test-Framework
- FluentAssertions für Assertions
- Arrange-Act-Assert-Pattern
- Ein Test = Eine Assertion (wo möglich)

```csharp
[Fact]
public void MethodName_StateUnderTest_ExpectedBehavior()
{
    // Arrange
    var sut = new ClassUnderTest();
    
    // Act
    var result = sut.Method();
    
    // Assert
    result.Should().Be(expected);
}
```

### Test Coverage

Ziel: >80% Code Coverage für Core-Logik

```bash
# Coverage Report generieren
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```

## AOT-Kompatibilität

### Was zu beachten ist

- Kein `Reflection.Emit`
- Keine dynamischen Assemblies
- JSON-Serialization nur mit Source Generators oder bekannten Typen
- Vermeiden Sie `MakeGenericType`

### AOT-Test

```bash
# Testen Sie immer mit PublishAot=true
dotnet publish -c Release -r linux-x64
./bin/Release/net8.0/linux-x64/publish/LogfileCleaner
```

## Neue Features vorschlagen

Öffnen Sie ein **Issue** mit:

1. **Problem-Statement**: Welches Problem löst das Feature?
2. **Proposed Solution**: Wie soll es funktionieren?
3. **Alternatives**: Andere Ansätze?
4. **Additional Context**: Screenshots, Code-Beispiele, etc.

## Bug Reports

Verwenden Sie das Bug-Template mit:

1. **Beschreibung**: Was ist passiert?
2. **Reproduktion**: Schritte zum Reproduzieren
3. **Erwartetes Verhalten**: Was sollte passieren?
4. **Environment**: OS, .NET Version, etc.
5. **Logs**: Relevante Fehlermeldungen

## Lizenz

Mit Ihrem Beitrag stimmen Sie zu, dass Ihr Code unter der MIT-Lizenz veröffentlicht wird.

## Fragen?

Öffnen Sie ein Issue mit dem Label `question` oder kontaktieren Sie die Maintainer direkt.

Vielen Dank für Ihre Beiträge! 🎉