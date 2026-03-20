using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;

namespace QwenAlibabaCodingPlan
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("code")]
    [Name("Qwen Completion")]
    internal class QwenCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new QwenCompletionSource(textBuffer);
        }
    }

    internal class QwenCompletionSource : ICompletionSource
    {
        private readonly ITextBuffer _textBuffer;
        private bool _isDisposed;

        public QwenCompletionSource(ITextBuffer textBuffer)
        {
            _textBuffer = textBuffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(QwenCompletionSource));

            var triggerPoint = session.GetTriggerPoint(_textBuffer.CurrentSnapshot);
            if (triggerPoint == null)
                return;

            var snapshot = _textBuffer.CurrentSnapshot;
            var line = snapshot.GetLineFromPosition(triggerPoint.Value);
            var textBefore = line.Snapshot.GetText(line.Start, triggerPoint.Value);

            if (!textBefore.EndsWith("//qwen") && !textBefore.EndsWith("//q"))
                return;

            var completions = new List<Completion>
            {
                new Completion("analyze", "Analyze Code", "Analyze the selected code with Qwen AI", null, "Analyzes your code for issues and improvements"),
                new Completion("explain", "Explain Code", "Explain what the selected code does", null, "Explains the functionality of your code"),
                new Completion("refactor", "Refactor Code", "Refactor the selected code", null, "Suggests refactored versions of your code"),
                new Completion("complete", "Complete Code", "Get code completion suggestions", null, "Suggests code completions based on context"),
                new Completion("test", "Generate Tests", "Generate unit tests for the code", null, "Creates unit tests for selected code"),
                new Completion("doc", "Add Documentation", "Add documentation comments", null, "Generates XML documentation for your code")
            };

            var applicableTo = snapshot.CreateTrackingSpan(triggerPoint.Value, 0, SpanTrackingMode.EdgeInclusive);
            completionSets.Add(new CompletionSet("Qwen", "Qwen", applicableTo, completions, null));
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }

    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("Qwen Controller")]
    [ContentType("code")]
    internal class QwenControllerProvider : IIntellisenseControllerProvider
    {
        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> textBuffers)
        {
            return new QwenController(textView, textBuffers);
        }
    }

    internal class QwenController : IIntellisenseController
    {
        private readonly ITextView _textView;
        private readonly IList<ITextBuffer> _textBuffers;
        private ICompletionSession _currentSession;

        public QwenController(ITextView textView, IList<ITextBuffer> textBuffers)
        {
            _textView = textView;
            _textBuffers = textBuffers;
            _textView.TextBuffer.Changed += TextBuffer_Changed;
        }

        private async void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            if (_currentSession != null && _currentSession.IsStarted)
                return;

            var snapshot = e.After;
            if (snapshot.Length == 0)
                return;

            var lastLine = snapshot.GetLineFromPosition(snapshot.Length - 1);
            var text = snapshot.GetText(lastLine.Start, snapshot.Length);

            if (text.Contains("//qwen") || text.Contains("//q"))
            {
                await Task.Delay(500);
                TriggerCompletion();
            }
        }

        private void TriggerCompletion()
        {
            var broker = ServiceProvider.GetService(typeof(ICompletionBroker)) as ICompletionBroker;
            if (broker == null)
                return;

            var triggerPoint = _textView.TextBuffer.CurrentSnapshot.CreatePoint(
                _textView.Caret.Position.BufferPosition.Position);

            _currentSession = broker.CreateCompletionSession(
                _textView,
                _textView.TextBuffer.CreateTrackingSpan(triggerPoint, 0, SpanTrackingMode.EdgeInclusive),
                true);

            _currentSession.Dismissed += (s, e) => _currentSession = null;
            _currentSession.Start();
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void Detach(ITextView textView)
        {
            if (_textView == textView)
            {
                _textView.TextBuffer.Changed -= TextBuffer_Changed;
            }
        }
    }
}