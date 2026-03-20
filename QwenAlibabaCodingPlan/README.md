# Qwen Alibaba Coding Plan - Visual Studio 2022 Extension

An AI-powered coding assistant extension for Visual Studio 2022 that integrates Alibaba's Qwen AI to provide code completion, chat, analysis, and refactoring capabilities.

## UI/UX Overview

### Chat Window (Tool Window)
```
+------------------------------------------+
|  Qwen Chat                               |
+------------------------------------------+
|  [You: Hello, help me with this code]   |
|  [Qwen: Sure, here's how to...]         |
|  [You: Thanks! Can you explain?]        |
|  [Qwen: Of course! The code...]         |
|                                          |
|                                          |
|                                          |
|                                          |
|                                          |
+------------------------------------------+
|  Enter your message...                  |
|                              [Send]      |
+------------------------------------------+
|  Ready                                   |
+------------------------------------------+
```

- **Title:** "Qwen Chat"
- **Message Area:** Scrollable list with colored backgrounds
  - User: Blue background, white text
  - Qwen: Gray background, black text
- **Input:** Multi-line TextBox + Send button
- **Status:** Shows "Ready" / "Thinking..."

### Settings Dialog
```
+--------------------------------------------------+
|  Qwen Settings                                   |
+--------------------------------------------------+
|  API Key:    [************************]          |
|                                                  |
|  API URL:    [https://coding-intl.dashscope... |
|                                                  |
|  Model:      [qwen3.5-plus            ▼]         |
|              (or type custom)                    |
|                                                  |
|  Get API key: https://dashscope.console.aliyun |
|                                                  |
|                              [Save]  [Cancel]   |
+--------------------------------------------------+
```

- **Size:** 500 x 240 pixels (fixed)
- **Controls:** API Key (masked), API URL, Model dropdown
- **Model Options:**
  ```
  qwen3.5-plus, qwen3-max-2026-01-23, qwen3-coder-next
  qwen3-coder-plus, glm-5, glm-4.7, kimi-k2.5, MiniMax-M2.5
  ```

### Menu Items
```
Tools
├── Open Qwen Chat        → Opens chat tool window
└── Qwen Settings        → Opens settings dialog

Edit
├── Analyze Code with Qwen    → Analyzes selected code
└── Refactor Code with Qwen   → Refactors selected code
```

## Features

### 🤖 AI Chat
- Interactive chat interface to ask Qwen coding questions
- Context-aware conversations with code understanding
- Access via **Tools > Open Qwen Chat**

### ✨ Code Completion
- AI-powered code suggestions as you type
- Trigger by typing `//qwen` in your code
- Supports multiple completion types: analyze, explain, refactor, complete, test, doc

### 📊 Code Analysis
- Analyze selected code for issues and improvements
- Get detailed explanations of code functionality
- Access via **Edit > Analyze Code with Qwen**

### 🔧 Code Refactoring
- AI-assisted code refactoring suggestions
- Improve readability and performance
- Access via **Edit > Refactor Code with Qwen**

## Requirements

- Visual Studio 2022 (Windows)
- .NET Framework 4.7.2
- Qwen API Key (free from [DashScope](https://dashscope.console.aliyun.com/))

## Installation

### From Source
1. Clone or download this repository
2. Open `QwenAlibabaCodingPlan.csproj` in Visual Studio 2022
3. Ensure Visual Studio SDK is installed
4. Build the solution (Ctrl+Shift+B)
5. Run (F5) to test in experimental instance

### Building VSIX
1. Build in Release mode
2. Find `QwenAlibabaCodingPlan.vsix` in the output folder
3. Double-click to install in Visual Studio

## Configuration

1. Get your API key from [DashScope Console](https://dashscope.console.aliyun.com/)
2. Go to **Tools > Qwen Settings**
3. Enter your API key
4. (Optional) Customize the API URL and model
5. Click Save

### Settings Options

| Setting | Default | Description |
|---------|---------|-------------|
| API Key | - | Your DashScope API key (required) |
| API URL | `https://coding-intl.dashscope.aliyuncs.com/apps/anthropic` | Endpoint URL |
| Model | `qwen-turbo` | AI model name (select from dropdown or type custom) |

### Available Models

| Brand | Model | Capabilities |
|-------|-------|--------------|
| Qwen | `qwen3.5-plus` | Text Generation, Deep Thinking, Visual Understanding |
| Qwen | `qwen3-max-2026-01-23` | Text Generation, Deep Thinking |
| Qwen | `qwen3-coder-next` | Text Generation |
| Qwen | `qwen3-coder-plus` | Text Generation |
| Zhipu | `glm-5` | Text Generation, Deep Thinking |
| Zhipu | `glm-4.7` | Text Generation, Deep Thinking |
| Kimi | `kimi-k2.5` | Text Generation, Deep Thinking, Visual Understanding |
| MiniMax | `MiniMax-M2.5` | Text Generation, Deep Thinking |

You can also type any custom model name directly in the Model field.

## Usage

### Chat Interface
1. Open the chat window: **Tools > Open Qwen Chat**
2. Type your question in the input box
3. Press Enter or click Send
4. View AI responses in the conversation

### Analyzing Code
1. Select code in the editor
2. Go to **Edit > Analyze Code with Qwen**
3. Results appear in the Output window under "Qwen Analysis"

### Refactoring Code
1. Select code in the editor
2. Go to **Edit > Refactor Code with Qwen**
3. Refactored code replaces the selection

### Code Completion
1. Type `//qwen` at any point in your code
2. A completion list appears with options
3. Select an action (analyze, explain, refactor, etc.)

## Project Structure

```
QwenAlibabaCodingPlan/
├── QwenAlibabaCodingPlan.csproj    # Project file
├── source.extension.vsixmanifest   # VSIX manifest
├── QwenChatPackage.cs              # Main package
├── QwenChatWindow.cs               # Chat tool window
├── QwenApiClient.cs                # Qwen API client
├── QwenCompletionSource.cs         # Intellisense provider
├── Commands.cs                     # Menu commands
├── QwenSettings.cs                 # Settings storage
├── QwenSettingsWindow.cs           # Settings dialog
├── Resources/
│   ├── Strings.resx                # Localized strings
│   └── icon.png                    # Extension icon
├── Menus.ctmenu                    # Menu definitions
└── License.txt                     # MIT License
```

## API Reference

This extension uses the [DashScope API](https://dashscope.console.aliyun.com/) from Alibaba Cloud. The default model is `qwen-turbo`.

### Supported Models
- qwen-turbo (default, fast)
- qwen-plus (balanced)
- qwen-max (most capable)

To change the model, edit `QwenApiClient.cs` and modify the `model` parameter in the `SendChatMessageAsync` method.

## Troubleshooting

### "API key not configured" error
- Go to **Tools > Qwen Settings**
- Enter your DashScope API key

### Extension not loading
- Ensure Visual Studio 2022 SDK is installed
- Check the Output window for error messages

### Chat not responding
- Verify internet connectivity
- Check API key is valid in DashScope console

## License

MIT License - see [License.txt](License.txt)

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

---

Built with ❤️ by Alibaba