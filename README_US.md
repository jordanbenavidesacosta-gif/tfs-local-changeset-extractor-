# TFS Local Changeset Extractor

Utility tool developed in **.NET Framework (WinForms)** to extract and recover **local TFS changesets** from a repository workspace.  
This tool allows developers to preserve local work and generate scripts or backups before performing branch changes, workspace cleanups, or migrations.

## Purpose

In some scenarios, developers may have **local changesets or pending changes** stored in their workspace that are not yet committed or easily recoverable.  
This tool helps extract those changes and generate a recoverable structure that can be used as a backup or migrated into another repository.

## Features

- Extract local changesets from a TFS workspace
- Generate exportable file structures
- Preserve pending work before branch operations
- Create backups of local development work
- Simple Windows Forms interface

## Technologies Used

- C#
- .NET Framework 4.8
- Windows Forms
- Git integration for backup storage

## How to Use

1. Clone the repository
2. Open the solution in Visual Studio
3. 3. Build and run the project

4. Use the interface to select the TFS workspace and export local changesets.

## Use Cases

- Recover local changes before switching branches
- Backup development work before workspace cleanup
- Export pending changes for migration
- Preserve code before repository restructuring

## Disclaimer

This project is intended as a **developer utility tool** and may require adjustments depending on the specific TFS workspace configuration.

## Author

Jordan Benavides
