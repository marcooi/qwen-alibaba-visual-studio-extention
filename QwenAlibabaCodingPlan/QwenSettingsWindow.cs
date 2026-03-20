using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace QwenAlibabaCodingPlan
{
    public class QwenSettingsWindow : Form
    {
        private TextBox _apiKeyTextBox;
        private TextBox _apiUrlTextBox;
        private ComboBox _modelComboBox;
        private Button _saveButton;
        private Button _cancelButton;

        public QwenSettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Qwen Settings";
            this.Width = 500;
            this.Height = 240;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int top = 20;
            int labelWidth = 100;
            int controlLeft = 130;
            int controlWidth = 320;

            // API Key
            var apiKeyLabel = new Label
            {
                Text = "API Key:",
                Left = 20,
                Top = top,
                Width = labelWidth
            };

            _apiKeyTextBox = new TextBox
            {
                Left = controlLeft,
                Top = top,
                Width = controlWidth,
                PasswordChar = '*'
            };

            top += 35;

            // API URL
            var apiUrlLabel = new Label
            {
                Text = "API URL:",
                Left = 20,
                Top = top,
                Width = labelWidth
            };

            _apiUrlTextBox = new TextBox
            {
                Left = controlLeft,
                Top = top,
                Width = controlWidth,
                Text = "https://coding-intl.dashscope.aliyuncs.com/apps/anthropic"
            };

            top += 35;

            // Model
            var modelLabel = new Label
            {
                Text = "Model:",
                Left = 20,
                Top = top,
                Width = labelWidth
            };

            _modelComboBox = new ComboBox
            {
                Left = controlLeft,
                Top = top,
                Width = controlWidth - 90,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            _modelComboBox.Items.AddRange(new object[]
            {
                "qwen3.5-plus",
                "qwen3-max-2026-01-23",
                "qwen3-coder-next",
                "qwen3-coder-plus",
                "glm-5",
                "glm-4.7",
                "kimi-k2.5",
                "MiniMax-M2.5"
            });
            _modelComboBox.Text = "qwen3.5-plus";

            var presetLabel = new Label
            {
                Text = "(or type custom)",
                Left = controlLeft + controlWidth - 85,
                Top = top + 3,
                Width = 80,
                ForeColor = System.Drawing.Color.Gray
            };

            top += 40;

            var helpLabel = new Label
            {
                Text = "Get API key: https://dashscope.console.aliyun.com/",
                Left = controlLeft,
                Top = top,
                Width = controlWidth,
                ForeColor = System.Drawing.Color.Gray
            };

            top += 30;

            _saveButton = new Button
            {
                Text = "Save",
                Left = 280,
                Top = top,
                Width = 80,
                DialogResult = DialogResult.OK
            };
            _saveButton.Click += SaveButton_Click;

            _cancelButton = new Button
            {
                Text = "Cancel",
                Left = 370,
                Top = top,
                Width = 80,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(apiKeyLabel);
            this.Controls.Add(_apiKeyTextBox);
            this.Controls.Add(apiUrlLabel);
            this.Controls.Add(_apiUrlTextBox);
            this.Controls.Add(modelLabel);
            this.Controls.Add(_modelComboBox);
            this.Controls.Add(presetLabel);
            this.Controls.Add(helpLabel);
            this.Controls.Add(_saveButton);
            this.Controls.Add(_cancelButton);
        }

        private void LoadSettings()
        {
            var settings = QwenSettings.Load();
            _apiKeyTextBox.Text = settings.ApiKey;
            _apiUrlTextBox.Text = settings.ApiUrl;
            _modelComboBox.Text = settings.Model;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var model = _modelComboBox.Text.Trim();
            if (string.IsNullOrEmpty(model))
                model = "qwen-turbo";

            var settings = new QwenSettings
            {
                ApiKey = _apiKeyTextBox.Text,
                ApiUrl = _apiUrlTextBox.Text,
                Model = model
            };
            settings.Save();

            if (QwenChatPackage.Instance != null)
            {
                QwenChatPackage.Instance.ApiClient.Configure(
                    _apiKeyTextBox.Text,
                    _apiUrlTextBox.Text,
                    model);
            }

            this.Close();
        }
    }
}