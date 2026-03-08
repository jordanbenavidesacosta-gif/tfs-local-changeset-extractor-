# Extractor de Changesets Locales de TFS

Herramienta desarrollada en **.NET Framework (WinForms)** para extraer y recuperar **changesets locales de TFS** desde un workspace de desarrollo.  
Permite preservar cambios locales y generar respaldos antes de realizar cambios de rama, limpieza de workspace o migraciones.

## Propósito

En algunos casos los desarrolladores pueden tener **changesets locales o cambios pendientes** en su workspace que aún no han sido enviados al repositorio o que pueden perderse durante cambios de rama o limpieza del entorno.

Esta herramienta permite extraer esos cambios y generar una estructura recuperable que puede utilizarse como respaldo o para migrarlos a otro repositorio.

## Funcionalidades

- Extraer changesets locales desde un workspace de TFS
- Generar estructuras de archivos exportables
- Preservar trabajo pendiente antes de operaciones de ramas
- Crear respaldos del trabajo local
- Interfaz sencilla en Windows Forms

## Tecnologías utilizadas

- C#
- .NET Framework 4.8
- Windows Forms
- Integración con Git para almacenamiento de respaldo


## Cómo usar

1. Clonar el repositorio
2. Abrir la solución en Visual Studio
3. Compilar y ejecutar el proyecto

4. Usar la interfaz para seleccionar el workspace de TFS y exportar los changesets locales.

## Casos de uso

- Recuperar cambios locales antes de cambiar de rama
- Respaldar trabajo antes de limpiar el workspace
- Exportar cambios pendientes para migración
- Preservar código antes de reorganizar el repositorio

## Nota

Este proyecto es una **herramienta utilitaria para desarrolladores** y puede requerir ajustes dependiendo de la configuración específica del workspace de TFS.

## Autor

Jordan Benavides
