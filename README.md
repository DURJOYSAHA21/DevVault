# DevVault

**Windows Developer Environment Assistant**

DevVault is a command-line developer utility for inspecting Windows development environments and analyzing local software projects.

It helps developers answer questions such as:

- Is my development environment ready?
- What type of project am I working with?
- Which projects exist inside this directory?
- What frameworks are being used?
- Which dependencies does the project have?
- Where did each dependency come from?
- Are any dependencies declared more than once?
- Which local ports are currently in use?

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
