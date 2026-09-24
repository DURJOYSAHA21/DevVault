# DevVault

**Windows Developer Environment Assistant**

DevVault is a command-line developer utility for inspecting development environments and analyzing local software projects — so cloning someone else's repo doesn't turn into an hour of guessing what's missing.

It helps developers answer questions such as:

- Is my development environment ready?
- What type of project am I working with?
- Which projects exist inside this directory?
- What frameworks are being used?
- Which dependencies does the project have?
- Where did each dependency come from?
- Are any dependencies declared more than once?
- Which local ports are currently in use?
- I just cloned this project — what do I need to install to run it?
- Can DevVault set up what's missing for me?

## Features

### Environment Diagnostics

```text
devvault doctor
```

Checks the development environment for:

- Windows
- PowerShell
- PATH
- .NET SDK
- Git
- Docker (optional)

### Project Analysis

```text
devvault analyze
```

Analyzes the current project directory and detects:

- Project types
- Frameworks
- Multiple projects in one directory
- Git repository
- Solution files
- .NET project files
- `package.json`
- `requirements.txt`
- `pyproject.toml`
- `.env`
- Dockerfile
- Project dependencies

A custom project path can be supplied:

```text
devvault analyze --path D:\MyProject
```

### Multi-Project Discovery

DevVault can discover multiple project types within the same directory.

Currently supported:

- C# / .NET
- Node.js / JavaScript
- Python

For example, a directory containing a .NET project, Node.js application, and Python service can be analyzed together.

### Dependency Analysis

```text
devvault dependencies
```

Checks the development tools required by detected project types.

Currently supported ecosystems include:

- NuGet
- npm
- PyPI

DevVault can identify dependency sources such as:

```text
package.json:dependencies
package.json:devDependencies
requirements.txt
pyproject.toml
```

It also detects dependencies that are declared more than once.

### Port Inspector

```text
devvault ports
```

Displays currently listening local ports and their associated process IDs.

A specific port can also be checked:

```text
devvault ports --port 8080
```

Example:

```text
Port 8080 is currently in use.
PID: 5284
Process: AgentService
```

### Setup / Onboarding (Planned)

```text
devvault setup
```

The flagship command DevVault is being built around. Instead of running `doctor`, `analyze`, and `dependencies` separately, `setup` combines them into one actionable checklist for a freshly cloned project:

```text
✅ .NET 8 SDK found (required: 8.0)
❌ Node.js not found (required: 18.x) → install from nodejs.org
⚠️  node_modules missing → run: npm install
❌ .env missing → copy .env.example to .env and fill in DATABASE_URL
⚠️  Docker not running → required for docker-compose.yml → start Docker Desktop
```

Two modes of automation are planned, split by risk:

- `devvault setup --fix` — automatically runs safe, project-scoped fixes (`npm install`, `dotnet restore`, `pip install -r requirements.txt`, copying `.env.example` to `.env`). These never touch anything outside the project folder, so they run without confirmation.
- `devvault setup --fix -y` — additionally installs missing machine-level tools (a specific SDK version, Docker, a required global CLI), prompting per item unless `-y` is passed. Machine-level installs are never silent by default, since they can affect other projects on the same machine.

On Windows, machine-level installs are done through `winget`, which ships with Windows 10/11 and provides consistent, scriptable installs for most SDKs and developer tools.

## Technology Stack

- C#
- .NET 10
- System.CommandLine
- xUnit
- Windows APIs / system commands
- Git

## Project Structure

```text
DevVault/
├── src/
│   └── DevVault/
│       ├── Commands/
│       ├── Models/
│       ├── Services/
│       └── Program.cs
│
├── tests/
│   └── DevVault.Tests/
│
├── DevVault.slnx
└── README.md
```

The application uses separate services and analyzers so additional project types and developer tools can be added without rewriting the entire application.

## Getting Started

### Requirements

- Windows
- .NET 10 SDK
- Git

Docker is optional.

### Build

Clone the repository and run:

```text
dotnet build
```

### Run

```text
dotnet run --project src\DevVault -- --help
```

### Available Commands

```text
doctor
analyze
dependencies
ports
```

Examples:

```text
dotnet run --project src\DevVault -- doctor

dotnet run --project src\DevVault -- analyze

dotnet run --project src\DevVault -- analyze --path D:\MyProject

dotnet run --project src\DevVault -- dependencies

dotnet run --project src\DevVault -- ports

dotnet run --project src\DevVault -- ports --port 5000
```

## Testing

Run the test suite with:

```text
dotnet test
```

## Current Version

**0.1.0**

This version focuses on environment diagnostics, project discovery, project analysis, dependency inspection, and local port inspection.

## Roadmap

DevVault currently focuses on Windows, with support for .NET, Node.js, and Python projects. Planned expansions fall into six areas:

### Core Architecture

- A pluggable ecosystem-detector pattern: each language/ecosystem is a small module exposing its signature files, required vs. installed version, dependency parser, and install commands — so adding a new ecosystem doesn't require changing the core engine
- Every detector answers the same four questions: what version is required, what version is installed, are dependencies installed, is required config (env vars, etc.) present

### One-Command Setup & Auto-Fix

- `devvault setup` — a single command that reports everything missing to run a freshly cloned project
- `devvault setup --fix` — automatically applies safe, project-scoped fixes
- `devvault setup --fix -y` — additionally installs missing machine-level tools, with per-item confirmation unless `-y` is passed

### Environment Coverage

- WSL detection (which distro is installed, whether a project should be inspected from inside it)
- macOS support (Homebrew, Xcode CLI tools, shell profile checks)
- Native Linux support (package manager detection: apt, dnf, pacman)
- A shared OS-abstraction layer so `doctor`, `analyze`, `ports`, and `setup` behave consistently across platforms

### Additional Language Ecosystems

Built on the detector pattern above, so each is a self-contained addition:

- Java / Kotlin (Maven, Gradle)
- Go (`go.mod`)
- Rust (`Cargo.toml`)
- Ruby (`Gemfile`)
- PHP (`composer.json`)

### New Diagnostics

- SDK/runtime version mismatch detection
- IDE and editor detection (VS Code, Rider, Visual Studio, relevant extensions)
- Cloud CLI presence and auth status (`az`, `aws`, `gcloud`)
- Environment variable auditing against `.env.example`
- Git health checks (uncommitted changes, stale branches, accidentally committed large files)
- Dependency conflict detection
- More accurate framework detection
- Better port/process diagnostics
- Project health summaries
- Docker environment analysis

### Distribution & Integrations

- One-command install via `dotnet tool install -g devvault` (NuGet global tool)
- Self-contained, no-dependency binaries for win-x64/linux-x64/osx-x64/osx-arm64, with a one-line install script per OS
- Package manager listings (`winget`, `choco`, `brew`) once the project has enough users to justify submission
- `--json` output mode for CI pipelines
- GitHub Actions integration to run `devvault doctor` on pull requests
- Exportable HTML/Markdown diagnostic reports

## Why DevVault?

Cloning a new project and getting it running is one of the most common sources of friction in software development — missing SDKs, wrong tool versions, uninstalled dependencies, and missing configuration files all show up as confusing runtime errors instead of a clear checklist.

DevVault aims to close that gap: one command to see exactly what a project needs, and (eventually) one command to get it set up — regardless of what language or framework the project is written in.

## License

This project is currently intended as a personal portfolio and learning project.
