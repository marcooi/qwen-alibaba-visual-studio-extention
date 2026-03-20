using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace QwenAlibabaCodingPlan
{
    public class QwenChatWindow : ToolWindowPane
    {
        private QwenChatControl _chatControl;

        public QwenChatWindow() : base(null)
        {
            Caption = "Qwen Chat";
            BitmapResourceMoniker = new Guid("{1DB7D8B7-D1C7-4F6C-A123-5E4F5E4D8B7A}");

            _chatControl = new QwenChatControl();
            Content = _chatControl;
        }
    }

    public partial class QwenChatControl : UserControl
    {
        private TextBox _inputTextBox;
        private Button _sendButton;
        private ListBox _messagesListBox;
        private List<ChatMessage> _conversationHistory = new List<ChatMessage>();
        private Label _statusLabel;

        public QwenChatControl()
        {
            InitializeComponent();
            AddWelcomeMessage();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            _messagesListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                FormattingEnabled = true,
                IntegralHeight = false,
                HorizontalScrollbar = true,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 40
            };
            _messagesListBox.DrawItem += MessagesListBox_DrawItem;

            var inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80
            };

            _inputTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                AcceptsReturn = true,
                PlaceholderText = "Ask Qwen anything about your code..."
            };
            _inputTextBox.KeyDown += InputTextBox_KeyDown;

            _sendButton = new Button
            {
                Dock = DockStyle.Right,
                Width = 80,
                Text = "Send",
                FlatStyle = FlatStyle.Flat
            };
            _sendButton.Click += SendButton_Click;

            _statusLabel = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 20,
                Text = "Ready",
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gray
            };

            inputPanel.Controls.Add(_inputTextBox);
            inputPanel.Controls.Add(_sendButton);

            this.Controls.Add(_messagesListBox);
            this.Controls.Add(inputPanel);
            this.Controls.Add(_statusLabel);

            this.ResumeLayout(false);
        }

        private void AddWelcomeMessage()
        {
            _messagesListBox.Items.Add(new ChatMessageItem
            {
                IsUser = false,
                Message = "Hello! I'm Qwen, your AI coding assistant. Ask me anything about coding, get help with your code, or request code analysis."
            });
        }

        private void MessagesListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();
            var item = _messagesListBox.Items[e.Index] as ChatMessageItem;
            if (item != null)
            {
                var brush = item.IsUser ? Brushes.DodgerBlue : Brushes.DarkGray;
                var textColor = item.IsUser ? Color.White : Color.Black;

                using (var sf = new StringFormat { LineAlignment = StringAlignment.Center })
                {
                    e.Graphics.DrawString((item.IsUser ? "You: " : "Qwen: ") + item.Message,
                        e.Font, new SolidBrush(textColor), e.Bounds, sf);
                }
            }
            e.DrawFocusRectangle();
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            await SendMessageAsync();
        }

        private async void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.ShiftKey)
            {
                e.SuppressKeyPress = true;
                await SendMessageAsync();
            }
        }

        private async Task SendMessageAsync()
        {
            var userMessage = _inputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            _inputTextBox.Clear();
            _statusLabel.Text = "Thinking...";

            _messagesListBox.Items.Add(new ChatMessageItem { IsUser = true, Message = userMessage });
            _conversationHistory.Add(new ChatMessage("user", userMessage));

            var response = await QwenChatPackage.Instance.ApiClient.SendChatMessageAsync(userMessage, _conversationHistory);

            _messagesListBox.Items.Add(new ChatMessageItem { IsUser = false, Message = response });
            _conversationHistory.Add(new ChatMessage("assistant", response));

            _messagesListBox.TopIndex = _messagesListBox.Items.Count - 1;
            _statusLabel.Text = "Ready";
        }
    }

    public class ChatMessageItem
    {
        public bool IsUser { get; set; }
        public string Message { get; set; }
    }
}