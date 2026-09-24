````markdown

\# DevVault



\*\*Windows Developer Environment Assistant\*\*



DevVault is a command-line developer utility for inspecting Windows development environments and analyzing local software projects.



It helps developers answer questions such as:



\- Is my development environment ready?

\- What type of project am I working with?

\- Which projects exist inside this directory?

\- What frameworks are being used?

\- Which dependencies does the project have?

\- Where did each dependency come from?

\- Are any dependencies declared more than once?

\- Which local ports are currently in use?



\## Features



\### Environment Diagnostics



```text

devvault doctor

````



Checks the development environment for:



\* Windows

\* PowerShell

\* PATH

\* .NET SDK

\* Git

\* Docker (optional)



\### Project Analysis



```text

devvault analyze

```



Analyzes the current project directory and detects:



\* Project types

\* Frameworks

\* Multiple projects in one directory

\* Git repository

\* Solution files

\* .NET project files

\* `package.json`

\* `requirements.txt`

\* `pyproject.toml`

\* `.env`

\* Dockerfile

\* Project dependencies



A custom project path can be supplied:



```text

devvault analyze --path D:\\MyProject

```



\### Multi-Project Discovery



DevVault can discover multiple project types within the same directory.



Currently supported:



\* C# / .NET

\* Node.js / JavaScript

\* Python



For example, a directory containing a .NET project, Node.js application, and Python service can be analyzed together.



\### Dependency Analysis



```text

devvault dependencies

```



Checks the development tools required by detected project types.



Currently supported ecosystems include:



\* NuGet

\* npm

\* PyPI



DevVault can identify dependency sources such as:



```text

package.json:dependencies

package.json:devDependencies

requirements.txt

pyproject.toml

```



It also detects dependencies that are declared more than once.



\### Port Inspector



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



\## Technology Stack



\* C#

\* .NET 10

\* System.CommandLine

\* xUnit

\* Windows APIs / system commands

\* Git



\## Project Structure



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



\## Getting Started



\### Requirements



\* Windows

\* .NET 10 SDK

\* Git



Docker is optional.



\### Build



Clone the repository and run:



```text

dotnet build

```



\### Run



```text

dotnet run --project src\\DevVault -- --help

```



\### Available Commands



```text

doctor

analyze

dependencies

ports

```



Examples:



```text

dotnet run --project src\\DevVault -- doctor



dotnet run --project src\\DevVault -- analyze



dotnet run --project src\\DevVault -- analyze --path D:\\MyProject



dotnet run --project src\\DevVault -- dependencies



dotnet run --project src\\DevVault -- ports



dotnet run --project src\\DevVault -- ports --port 5000

```



\## Testing



Run the test suite with:



```text

dotnet test

```



\## Current Version



\*\*0.1.0\*\*



This version focuses on environment diagnostics, project discovery, project analysis, dependency inspection, and local port inspection.



\## Roadmap



Future versions may include:



\* Dependency conflict detection

\* More accurate framework detection

\* Better port/process diagnostics

\* Development environment recommendations

\* Safe automated fixes

\* Project health summaries

\* Additional language ecosystems

\* Configuration diagnostics

\* Docker environment analysis

\* Exportable diagnostic reports



\## Why DevVault?



Development environments can become difficult to troubleshoot when multiple languages, frameworks, services, and tools are installed on the same machine.



DevVault aims to provide a single command-line interface for understanding the local development environment and identifying common project-level issues.



\## License



This project is currently intended as a personal portfolio and learning project.



````



Save the file.



Then run \*\*only these two commands\*\*:



```cmd

dotnet build

````



```cmd

dotnet test

```



If both succeed, tell me \*\*`done`\*\*.



We're getting close to the point where I'll tell you to make the \*\*first GitHub push for V0.1.0\*\*.



