using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QwenAlibabaCodingPlan
{
    public class CodeGeneratorWindow : Form
    {
        private TextBox _promptTextBox;
        private ComboBox _languageComboBox;
        private ComboBox _templateComboBox;
        private Button _generateButton;
        private Button _cancelButton;
        private Label _promptLabel;
        private Label _languageLabel;
        private Label _templateLabel;

        public string Prompt => _promptTextBox.Text;
        public string Language => _languageComboBox.Text;

        private readonly Dictionary<string, string> _templates = new Dictionary<string, string>
        {
            // Custom
            { "Custom", "" },

            // MS SQL Templates
            { "SQL - Create Table", "Create a MS SQL table for [table_name] with columns: [columns]" },
            { "SQL - Select Query", "Write a MS SQL SELECT query to fetch [columns] from [table_name] with WHERE clause" },
            { "SQL - Insert Query", "Write a MS SQL INSERT statement for [table_name]" },
            { "SQL - Update Query", "Write a MS SQL UPDATE statement for [table_name]" },
            { "SQL - Delete Query", "Write a MS SQL DELETE statement for [table_name]" },
            { "SQL - Stored Procedure", "Create a MS SQL stored procedure for [operation] on [table_name]" },

            // C# Templates
            { "C# - SQL Connection", "Create a C# method to connect to MS SQL Server database using connection string" },
            { "C# - SQL Reader", "Create C# code to execute a SQL query and read results using SqlDataReader" },
            { "C# - Entity Model", "Create a C# entity model class for [table_name] with properties for all columns" },
            { "C# - Repository", "Create a C# repository class for [table_name] with CRUD operations using ADO.NET" },
            { "C# - Dapper Query", "Create C# code using Dapper to query [table_name] and return list of entities" },
            { "C# - EF Core Context", "Create Entity Framework Core DbContext for [table_name]" },
            { "C# - SQL Parameterized Query", "Create C# parameterized SQL query to prevent SQL injection" },

            // Python Templates
            { "Python - SQL Connection", "Create Python code to connect to MS SQL Server using pyodbc" },
            { "Python - SQL Query", "Create Python code to execute SQL query and fetch results from MS SQL" },

            // JavaScript/TypeScript Templates
            { "JS - MSSQL Query", "Create JavaScript/TypeScript code to query MS SQL database using tedious or mssql library" },
        };

        public CodeGeneratorWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Qwen Code Generator";
            this.Width = 600;
            this.Height = 340;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int left = 15;
            int top = 15;
            int labelWidth = 100;
            int controlLeft = 120;
            int controlWidth = 450;

            // Template Label
            _templateLabel = new Label
            {
                Text = "Template:",
                Left = left,
                Top = top,
                Width = labelWidth
            };

            // Template ComboBox
            _templateComboBox = new ComboBox
            {
                Left = controlLeft,
                Top = top,
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            foreach (var key in _templates.Keys)
            {
                _templateComboBox.Items.Add(key);
            }
            _templateComboBox.Text = "Custom";
            _templateComboBox.SelectedIndexChanged += TemplateComboBox_SelectedIndexChanged;

            top += 30;

            // Prompt Label
            _promptLabel = new Label
            {
                Text = "Describe what you want:",
                Left = left,
                Top = top,
                Width = controlWidth
            };

            top += 22;

            // Prompt TextBox
            _promptTextBox = new TextBox
            {
                Left = left,
                Top = top,
                Width = controlWidth + 15,
                Height = 80,
                Multiline = true,
                AcceptsReturn = true,
                ScrollBars = ScrollBars.Vertical,
                PlaceholderText = "e.g., Create a REST API controller for user management with CRUD operations in C#"
            };

            top += 90;

            // Language Label
            _languageLabel = new Label
            {
                Text = "Language:",
                Left = left,
                Top = top,
                Width = labelWidth
            };

            // Language ComboBox
            _languageComboBox = new ComboBox
            {
                Left = controlLeft,
                Top = top,
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            _languageComboBox.Items.AddRange(new object[]
            {
                "C#",
                "Python",
                "JavaScript",
                "TypeScript",
                "Java",
                "Go",
                "Rust",
                "SQL"
            });
            _languageComboBox.Text = "C#";

            top += 35;

            var helpLabel = new Label
            {
                Text = "Tip: Select a template above or write custom prompt",
                Left = left,
                Top = top,
                Width = controlWidth + 15,
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Italic)
            };

            top += 35;

            // Generate Button
            _generateButton = new Button
            {
                Text = "Generate",
                Left = 380,
                Top = top,
                Width = 90,
                Height = 30,
                DialogResult = DialogResult.OK
            };

            // Cancel Button
            _cancelButton = new Button
            {
                Text = "Cancel",
                Left = 480,
                Top = top,
                Width = 90,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(_templateLabel);
            this.Controls.Add(_templateComboBox);
            this.Controls.Add(_promptLabel);
            this.Controls.Add(_promptTextBox);
            this.Controls.Add(_languageLabel);
            this.Controls.Add(_languageComboBox);
            this.Controls.Add(helpLabel);
            this.Controls.Add(_generateButton);
            this.Controls.Add(_cancelButton);

            this.AcceptButton = _generateButton;
            this.CancelButton = _cancelButton;
        }

        private void TemplateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = _templateComboBox.Text;
            if (_templates.TryGetValue(selected, out var template) && !string.IsNullOrEmpty(template))
            {
                _promptTextBox.Text = template;
            }
        }
    }
}