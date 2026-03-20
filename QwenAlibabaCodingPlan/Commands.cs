using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.VisualStudio.TextManager.Interop;

namespace QwenAlibabaCodingPlan
{
    internal sealed class QwenChatCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567891");

        private readonly QwenChatPackage _package;

        private QwenChatCommand(QwenChatPackage package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            commandService?.AddCommand(new MenuCommand(Execute, new CommandID(CommandSet, CommandId)));
        }

        public static void Initialize(QwenChatPackage package)
        {
            new QwenChatCommand(package);
        }

        private void Execute(object sender, EventArgs e)
        {
            var toolWindow = _package.FindToolWindow(typeof(QwenChatWindow), 0, true);
            if (toolWindow?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)toolWindow.Frame;
            windowFrame.Show();
        }
    }

    internal sealed class AnalyzeCodeCommand
    {
        public const int CommandId = 0x0101;
        public static readonly Guid CommandSet = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567891");

        private readonly QwenChatPackage _package;

        private AnalyzeCodeCommand(QwenChatPackage package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            commandService?.AddCommand(new MenuCommand(Execute, new CommandID(CommandSet, CommandId)));
        }

        public static void Initialize(QwenChatPackage package)
        {
            new AnalyzeCodeCommand(package);
        }

        private async void Execute(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            if (dte?.ActiveDocument == null)
            {
                VsShellUtilities.ShowMessageBox(
                    _package,
                    "No active document found.",
                    "Qwen Analyze",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK);
                return;
            }

            var document = dte.ActiveDocument;
            var selection = document.Selection as TextSelection;

            string codeToAnalyze;
            if (selection != null && !selection.IsEmpty)
            {
                codeToAnalyze = selection.Text;
            }
            else
            {
                codeToAnalyze = document.Text;
            }

            if (string.IsNullOrWhiteSpace(codeToAnalyze))
            {
                VsShellUtilities.ShowMessageBox(
                    _package,
                    "No code selected or document is empty.",
                    "Qwen Analyze",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK);
                return;
            }

            var toolWindow = _package.FindToolWindow(typeof(QwenChatWindow), 0, true);
            if (toolWindow?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)toolWindow.Frame;
            windowFrame.Show();

            var result = await _package.ApiClient.AnalyzeCodeAsync(codeToAnalyze);

            var outputWindow = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            outputWindow.Visible = true;
            var outputPane = outputWindow.Object as OutputWindow;
            var pane = outputPane.OutputWindowPanes.Add("Qwen Analysis");
            pane.Activate();
            pane.OutputString("=== Qwen Code Analysis ===\n\n");
            pane.OutputString(result);
            pane.OutputString("\n\n");
        }
    }

    internal sealed class RefactorCodeCommand
    {
        public const int CommandId = 0x0102;
        public static readonly Guid CommandSet = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567891");

        private readonly QwenChatPackage _package;

        private RefactorCodeCommand(QwenChatPackage package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            commandService?.AddCommand(new MenuCommand(Execute, new CommandID(CommandSet, CommandId)));
        }

        public static void Initialize(QwenChatPackage package)
        {
            new RefactorCodeCommand(package);
        }

        private async void Execute(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            if (dte?.ActiveDocument == null)
            {
                VsShellUtilities.ShowMessageBox(
                    _package,
                    "No active document found.",
                    "Qwen Refactor",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK);
                return;
            }

            var document = dte.ActiveDocument;
            var selection = document.Selection as TextSelection;

            string codeToRefactor;
            if (selection != null && !selection.IsEmpty)
            {
                codeToRefactor = selection.Text;
            }
            else
            {
                codeToRefactor = document.Text;
            }

            if (string.IsNullOrWhiteSpace(codeToRefactor))
            {
                VsShellUtilities.ShowMessageBox(
                    _package,
                    "No code selected or document is empty.",
                    "Qwen Refactor",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK);
                return;
            }

            var result = await _package.ApiClient.RefactorCodeAsync(codeToRefactor, "improve readability and performance");

            if (selection != null && !selection.IsEmpty)
            {
                selection.Insert(result);
            }
            else
            {
                VsShellUtilities.ShowMessageBox(
                    _package,
                    "Refactored code:\n\n" + result,
                    "Qwen Refactor",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK);
            }
        }
    }
}