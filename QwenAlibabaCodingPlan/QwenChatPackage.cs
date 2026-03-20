using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace QwenAlibabaCodingPlan
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowBackgroundLoading = true)]
    [Guid(QwenChatPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(QwenChatWindow))]
    public sealed class QwenChatPackage : AsyncPackage
    {
        public const string PackageGuidString = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";

        public static QwenChatPackage Instance { get; private set; }

        public QwenApiClient ApiClient { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Instance = this;
            ApiClient = new QwenApiClient();

            // Load saved settings
            var settings = QwenSettings.Load();
            if (!string.IsNullOrEmpty(settings.ApiKey))
            {
                ApiClient.Configure(settings.ApiKey, settings.ApiUrl, settings.Model);
            }

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var mcs = await GetServiceAsync(typeof(IComponentModel)) as IComponentModel;
            if (mcs != null)
            {
                mcs.DefaultCompositionService.ComposeParts(this);
            }

            QwenChatCommand.Initialize(this);
            AnalyzeCodeCommand.Initialize(this);
            RefactorCodeCommand.Initialize(this);
        }

        public override IVsAsyncToolWindowSession CreateToolWindowSessionAsync(Type toolWindowType, IVsToolWindowSession session, CancellationToken cancellationToken)
        {
            return base.CreateToolWindowSessionAsync(toolWindowType, session, cancellationToken);
        }
    }
}