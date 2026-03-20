# QWEN.md - Project Context

## Project Overview

**Project Name:** Qwen Alibaba Coding Plan
**Type:** Visual Studio 2022 Extension (VSIX)
**Purpose:** AI-powered coding assistant that integrates Alibaba's Qwen AI into Visual Studio 2022

## Technology Stack

- **Target Framework:** .NET Framework 4.7.2
- **Language:** C#
- **Build Tools:** Microsoft.VSSDK.BuildTools v17.0.5232
- **Dependencies:** Newtonsoft.Json v13.0.3

## Building and Running

### Prerequisites
- Visual Studio 2022 (Windows)
- Visual Studio SDK installed

### Build Commands
```bash
# Open in Visual Studio 2022
# Build: Ctrl+Shift+B
# Run (experimental instance): F5
```

### Output
- VSIX package: `QwenAlibabaCodingPlan.vsix` (after Release build)

## Project Structure

```
QwenAlibabaCodingPlan/
├── QwenAlibabaCodingPlan.csproj    # Project file (net472)
├── source.extension.vsixmanifest   # VSIX manifest
├── QwenChatPackage.cs              # Main package (AsyncPackage)
├── QwenChatWindow.cs               # Chat tool window UI (WinForms)
├── QwenApiClient.cs                # DashScope API client
├── QwenCompletionSource.cs         # Intellisense completion provider
├── Commands.cs                     # Menu commands (chat, analyze, refactor)
├── QwenSettings.cs                 # Settings persistence (JSON)
├── QwenSettingsWindow.cs           # Settings dialog (WinForms)
├── Resources/
│   ├── Strings.resx                # Localized strings
│   └── Strings.Designer.cs         # Generated resource class
├── Menus.ctmenu                    # Menu definitions
├── License.txt                     # MIT License
└── README.md                       # Documentation
```

## Key Configuration

| Setting | Default | Location |
|---------|---------|----------|
| API URL | `https://coding-intl.dashscope.aliyuncs.com/apps/anthropic` | QwenSettings.cs, QwenApiClient.cs |
| Model | `qwen3.5-plus` | QwenSettings.cs, QwenApiClient.cs |
| Settings File | `%APPDATA%\QwenAlibabaCodingPlan\settings.json` | QwenSettings.cs |

## Available AI Models

| Brand | Model | Capabilities |
|-------|-------|--------------|
| Qwen | qwen3.5-plus | Text Generation, Deep Thinking, Visual Understanding |
| Qwen | qwen3-max-2026-01-23 | Text Generation, Deep Thinking |
| Qwen | qwen3-coder-next | Text Generation |
| Qwen | qwen3-coder-plus | Text Generation |
| Zhipu | glm-5 | Text Generation, Deep Thinking |
| Zhipu | glm-4.7 | Text Generation, Deep Thinking |
| Kimi | kimi-k2.5 | Text Generation, Deep Thinking, Visual Understanding |
| MiniMax | MiniMax-M2.5 | Text Generation, Deep Thinking |

## Development Notes

- Uses WinForms for tool window UI (QwenChatWindow.cs, QwenSettingsWindow.cs)
- AsyncPackage for VS extension loading
- Settings stored in JSON format at user AppData folder
- API client uses HttpClient with Bearer token authentication