﻿using System.Windows.Forms;

namespace DBSysReport
{
    partial class AppForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppForm));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.appMenu = new System.Windows.Forms.MenuStrip();
            this.exitMenuOption = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statisticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugMenuOption = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuOption = new System.Windows.Forms.ToolStripMenuItem();
            this.userPanel = new System.Windows.Forms.Panel();
            this.usersList = new System.Windows.Forms.ListBox();
            this.adminPanel = new System.Windows.Forms.Panel();
            this.sqlPanelBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sqlOutput = new System.Windows.Forms.TextBox();
            this.sqlInput = new System.Windows.Forms.TextBox();
            this.reportPanelBox = new System.Windows.Forms.GroupBox();
            this.reportTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.createReportButton = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.reportTypeComboBox = new System.Windows.Forms.ComboBox();
            this.challengeDates = new System.Windows.Forms.ListBox();
            this.pdfViewer = new AxAcroPDFLib.AxAcroPDF();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.label13 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label16 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.usersPanelBox = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.passwordInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.loginInput = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.departmentInput = new System.Windows.Forms.TextBox();
            this.patronymicNameInput = new System.Windows.Forms.TextBox();
            this.nameInput = new System.Windows.Forms.TextBox();
            this.surnameInput = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.postInput = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.setEmployerDataButton = new System.Windows.Forms.Button();
            this.addEmployerDataButton = new System.Windows.Forms.Button();
            this.dumpFilePanelBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.staticDataFileInput = new System.Windows.Forms.TextBox();
            this.currentDumpFileInput = new System.Windows.Forms.TextBox();
            this.selectDumpFileButton = new System.Windows.Forms.Button();
            this.selectStaticDataFileButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.sourceDumpFileInput = new System.Windows.Forms.TextBox();
            this.mergeButton = new System.Windows.Forms.Button();
            this.selectSourceButton = new System.Windows.Forms.Button();
            this.appMenu.SuspendLayout();
            this.adminPanel.SuspendLayout();
            this.sqlPanelBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.reportPanelBox.SuspendLayout();
            this.reportTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pdfViewer)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.usersPanelBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.dumpFilePanelBox.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // appMenu
            // 
            this.appMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitMenuOption,
            this.reportsToolStripMenuItem,
            this.statisticToolStripMenuItem,
            this.debugMenuOption,
            this.infoToolStripMenuItem1,
            this.infoToolStripMenuItem});
            this.appMenu.Location = new System.Drawing.Point(0, 0);
            this.appMenu.Name = "appMenu";
            this.appMenu.Size = new System.Drawing.Size(1041, 24);
            this.appMenu.TabIndex = 0;
            this.appMenu.Text = "menuStrip1";
            // 
            // exitMenuOption
            // 
            this.exitMenuOption.Name = "exitMenuOption";
            this.exitMenuOption.Size = new System.Drawing.Size(54, 20);
            this.exitMenuOption.Text = "Выход";
            this.exitMenuOption.Click += new System.EventHandler(this.LogoutAndExit);
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.reportsToolStripMenuItem.Text = "Отчёты";
            // 
            // statisticToolStripMenuItem
            // 
            this.statisticToolStripMenuItem.Name = "statisticToolStripMenuItem";
            this.statisticToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.statisticToolStripMenuItem.Text = "Статистика";
            // 
            // debugMenuOption
            // 
            this.debugMenuOption.Name = "debugMenuOption";
            this.debugMenuOption.Size = new System.Drawing.Size(64, 20);
            this.debugMenuOption.Text = "Отладка";
            // 
            // infoToolStripMenuItem1
            // 
            this.infoToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1});
            this.infoToolStripMenuItem1.Name = "infoToolStripMenuItem1";
            this.infoToolStripMenuItem1.Size = new System.Drawing.Size(63, 20);
            this.infoToolStripMenuItem1.Text = "справка";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(147, 22);
            this.aboutToolStripMenuItem1.Text = "о программе";
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuOption});
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.infoToolStripMenuItem.Text = "помощь";
            // 
            // aboutMenuOption
            // 
            this.aboutMenuOption.Name = "aboutMenuOption";
            this.aboutMenuOption.Size = new System.Drawing.Size(143, 22);
            this.aboutMenuOption.Text = "руководство";
            // 
            // userPanel
            // 
            this.userPanel.AutoSize = true;
            this.userPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.userPanel.Location = new System.Drawing.Point(0, 0);
            this.userPanel.Name = "userPanel";
            this.userPanel.Size = new System.Drawing.Size(1041, 0);
            this.userPanel.TabIndex = 1;
            // 
            // usersList
            // 
            this.usersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usersList.FormattingEnabled = true;
            this.usersList.Location = new System.Drawing.Point(3, 3);
            this.usersList.Name = "usersList";
            this.usersList.ScrollAlwaysVisible = true;
            this.usersList.Size = new System.Drawing.Size(132, 272);
            this.usersList.TabIndex = 0;
            this.usersList.SelectedIndexChanged += new System.EventHandler(this.usersList_SelectedIndexChanged);
            // 
            // adminPanel
            // 
            this.adminPanel.Controls.Add(this.sqlPanelBox);
            this.adminPanel.Controls.Add(this.reportPanelBox);
            this.adminPanel.Controls.Add(this.usersPanelBox);
            this.adminPanel.Controls.Add(this.dumpFilePanelBox);
            this.adminPanel.Controls.Add(this.userPanel);
            this.adminPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adminPanel.Location = new System.Drawing.Point(0, 24);
            this.adminPanel.Name = "adminPanel";
            this.adminPanel.Size = new System.Drawing.Size(1041, 566);
            this.adminPanel.TabIndex = 2;
            // 
            // sqlPanelBox
            // 
            this.sqlPanelBox.Controls.Add(this.tableLayoutPanel1);
            this.sqlPanelBox.Location = new System.Drawing.Point(462, 351);
            this.sqlPanelBox.Name = "sqlPanelBox";
            this.sqlPanelBox.Size = new System.Drawing.Size(576, 212);
            this.sqlPanelBox.TabIndex = 5;
            this.sqlPanelBox.TabStop = false;
            this.sqlPanelBox.Text = "Панель вызова SQL комманд";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.sqlOutput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sqlInput, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(570, 193);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // sqlOutput
            // 
            this.sqlOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlOutput.Location = new System.Drawing.Point(3, 3);
            this.sqlOutput.Multiline = true;
            this.sqlOutput.Name = "sqlOutput";
            this.sqlOutput.ReadOnly = true;
            this.sqlOutput.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.sqlOutput.Size = new System.Drawing.Size(564, 161);
            this.sqlOutput.TabIndex = 0;
            // 
            // sqlInput
            // 
            this.sqlInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlInput.Location = new System.Drawing.Point(3, 170);
            this.sqlInput.Name = "sqlInput";
            this.sqlInput.Size = new System.Drawing.Size(564, 20);
            this.sqlInput.TabIndex = 1;
            this.sqlInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            // 
            // reportPanelBox
            // 
            this.reportPanelBox.Controls.Add(this.reportTabControl);
            this.reportPanelBox.Location = new System.Drawing.Point(462, 3);
            this.reportPanelBox.Name = "reportPanelBox";
            this.reportPanelBox.Size = new System.Drawing.Size(576, 342);
            this.reportPanelBox.TabIndex = 4;
            this.reportPanelBox.TabStop = false;
            this.reportPanelBox.Text = "Управление отчётами";
            // 
            // reportTabControl
            // 
            this.reportTabControl.Controls.Add(this.tabPage1);
            this.reportTabControl.Controls.Add(this.tabPage2);
            this.reportTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportTabControl.Location = new System.Drawing.Point(3, 16);
            this.reportTabControl.Name = "reportTabControl";
            this.reportTabControl.SelectedIndex = 0;
            this.reportTabControl.Size = new System.Drawing.Size(570, 323);
            this.reportTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(562, 297);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Генерация отчётов";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tableLayoutPanel6);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pdfViewer);
            this.splitContainer2.Size = new System.Drawing.Size(556, 291);
            this.splitContainer2.SplitterDistance = 119;
            this.splitContainer2.TabIndex = 0;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.createReportButton, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.label11, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.reportTypeComboBox, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.challengeDates, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 5;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.Size = new System.Drawing.Size(119, 291);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Дата испытания";
            // 
            // createReportButton
            // 
            this.createReportButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.createReportButton.Enabled = false;
            this.createReportButton.Location = new System.Drawing.Point(9, 265);
            this.createReportButton.Name = "createReportButton";
            this.createReportButton.Size = new System.Drawing.Size(100, 23);
            this.createReportButton.TabIndex = 1;
            this.createReportButton.Text = "создать отчёт";
            this.createReportButton.UseVisualStyleBackColor = true;
            this.createReportButton.Click += new System.EventHandler(this.createReportButton_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(3, 222);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(113, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Тип отчёта";
            // 
            // reportTypeComboBox
            // 
            this.reportTypeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportTypeComboBox.FormattingEnabled = true;
            this.reportTypeComboBox.Items.AddRange(new object[] {
            "штатный",
            "приёмо-сдаточный"});
            this.reportTypeComboBox.Location = new System.Drawing.Point(3, 238);
            this.reportTypeComboBox.Name = "reportTypeComboBox";
            this.reportTypeComboBox.Size = new System.Drawing.Size(113, 21);
            this.reportTypeComboBox.TabIndex = 3;
            // 
            // challengeDates
            // 
            this.challengeDates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.challengeDates.FormattingEnabled = true;
            this.challengeDates.Location = new System.Drawing.Point(3, 16);
            this.challengeDates.Name = "challengeDates";
            this.challengeDates.Size = new System.Drawing.Size(113, 203);
            this.challengeDates.TabIndex = 4;
            this.challengeDates.SelectedIndexChanged += new System.EventHandler(this.challengeDates_SelectedIndexChanged);
            // 
            // pdfViewer
            // 
            this.pdfViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pdfViewer.Enabled = true;
            this.pdfViewer.Location = new System.Drawing.Point(0, 0);
            this.pdfViewer.Name = "pdfViewer";
            this.pdfViewer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("pdfViewer.OcxState")));
            this.pdfViewer.Size = new System.Drawing.Size(433, 291);
            this.pdfViewer.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(562, 297);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Просмотр статистики";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tableLayoutPanel8);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.chart1);
            this.splitContainer3.Size = new System.Drawing.Size(556, 291);
            this.splitContainer3.SplitterDistance = 119;
            this.splitContainer3.TabIndex = 0;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Controls.Add(this.label13, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.button3, 0, 4);
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel9, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.checkedListBox1, 0, 3);
            this.tableLayoutPanel8.Controls.Add(this.label16, 0, 2);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 5;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.Size = new System.Drawing.Size(119, 291);
            this.tableLayoutPanel8.TabIndex = 0;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Location = new System.Drawing.Point(3, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(113, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Период просмотра";
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button3.Location = new System.Drawing.Point(3, 265);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(113, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "показать статистику";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Controls.Add(this.label14, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.label15, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.dateTimePicker1, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.dateTimePicker2, 1, 1);
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 2;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(113, 52);
            this.tableLayoutPanel9.TabIndex = 2;
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 6);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(13, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "с";
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 32);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(19, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "по";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dateTimePicker1.Location = new System.Drawing.Point(28, 3);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(82, 20);
            this.dateTimePicker1.TabIndex = 2;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dateTimePicker2.Location = new System.Drawing.Point(28, 29);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(82, 20);
            this.dateTimePicker2.TabIndex = 3;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "БУ",
            "ПУ",
            "МБА",
            "МСН",
            "МР",
            "МУ",
            "МВУ",
            "БЧЭ"});
            this.checkedListBox1.Location = new System.Drawing.Point(3, 87);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(113, 172);
            this.checkedListBox1.TabIndex = 3;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Location = new System.Drawing.Point(3, 71);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(113, 13);
            this.label16.TabIndex = 4;
            this.label16.Text = "Категория проверок";
            // 
            // chart1
            // 
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Name = "Legend1";
            this.chart1.Legends.Add(legend3);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(433, 291);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // usersPanelBox
            // 
            this.usersPanelBox.Controls.Add(this.splitContainer1);
            this.usersPanelBox.Location = new System.Drawing.Point(3, 3);
            this.usersPanelBox.Name = "usersPanelBox";
            this.usersPanelBox.Size = new System.Drawing.Size(453, 342);
            this.usersPanelBox.TabIndex = 3;
            this.usersPanelBox.TabStop = false;
            this.usersPanelBox.Text = "Управление данными пользователей";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 16);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox5);
            this.splitContainer1.Size = new System.Drawing.Size(447, 323);
            this.splitContainer1.SplitterDistance = 144;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tableLayoutPanel5);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(144, 323);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Список пользователей";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.usersList, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.label12, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(138, 304);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 278);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 26);
            this.label12.TabIndex = 4;
            this.label12.Text = "Пользователь\r\nне выбран";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tableLayoutPanel7);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(0, 0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(299, 323);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Управление учётными данными пользователя";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.passwordInput, 1, 6);
            this.tableLayoutPanel7.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.loginInput, 1, 5);
            this.tableLayoutPanel7.Controls.Add(this.label8, 0, 6);
            this.tableLayoutPanel7.Controls.Add(this.departmentInput, 1, 4);
            this.tableLayoutPanel7.Controls.Add(this.patronymicNameInput, 1, 2);
            this.tableLayoutPanel7.Controls.Add(this.nameInput, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.surnameInput, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.label6, 0, 4);
            this.tableLayoutPanel7.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel7.Controls.Add(this.postInput, 1, 3);
            this.tableLayoutPanel7.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.label7, 0, 5);
            this.tableLayoutPanel7.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel2, 1, 7);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 8;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(293, 304);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // passwordInput
            // 
            this.passwordInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordInput.Location = new System.Drawing.Point(74, 237);
            this.passwordInput.Name = "passwordInput";
            this.passwordInput.Size = new System.Drawing.Size(216, 20);
            this.passwordInput.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Имя";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loginInput
            // 
            this.loginInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.loginInput.Location = new System.Drawing.Point(74, 199);
            this.loginInput.Name = "loginInput";
            this.loginInput.Size = new System.Drawing.Size(216, 20);
            this.loginInput.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 240);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Пароль";
            // 
            // departmentInput
            // 
            this.departmentInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.departmentInput.Location = new System.Drawing.Point(74, 161);
            this.departmentInput.Name = "departmentInput";
            this.departmentInput.Size = new System.Drawing.Size(216, 20);
            this.departmentInput.TabIndex = 11;
            // 
            // patronymicNameInput
            // 
            this.patronymicNameInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.patronymicNameInput.Location = new System.Drawing.Point(74, 85);
            this.patronymicNameInput.Name = "patronymicNameInput";
            this.patronymicNameInput.Size = new System.Drawing.Size(216, 20);
            this.patronymicNameInput.TabIndex = 9;
            // 
            // nameInput
            // 
            this.nameInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.nameInput.Location = new System.Drawing.Point(74, 47);
            this.nameInput.Name = "nameInput";
            this.nameInput.Size = new System.Drawing.Size(216, 20);
            this.nameInput.TabIndex = 8;
            // 
            // surnameInput
            // 
            this.surnameInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.surnameInput.Location = new System.Drawing.Point(74, 9);
            this.surnameInput.Name = "surnameInput";
            this.surnameInput.Size = new System.Drawing.Size(216, 20);
            this.surnameInput.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 164);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Отдел";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Должность";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // postInput
            // 
            this.postInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.postInput.FormattingEnabled = true;
            this.postInput.Location = new System.Drawing.Point(74, 122);
            this.postInput.Name = "postInput";
            this.postInput.Size = new System.Drawing.Size(216, 21);
            this.postInput.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Отчество";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 202);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Логин";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Фамилия";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.setEmployerDataButton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.addEmployerDataButton, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(74, 269);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(216, 32);
            this.tableLayoutPanel2.TabIndex = 17;
            // 
            // setEmployerDataButton
            // 
            this.setEmployerDataButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.setEmployerDataButton.AutoSize = true;
            this.setEmployerDataButton.Enabled = false;
            this.setEmployerDataButton.Location = new System.Drawing.Point(138, 4);
            this.setEmployerDataButton.Name = "setEmployerDataButton";
            this.setEmployerDataButton.Size = new System.Drawing.Size(75, 23);
            this.setEmployerDataButton.TabIndex = 0;
            this.setEmployerDataButton.Text = "изменить";
            this.setEmployerDataButton.UseVisualStyleBackColor = true;
            this.setEmployerDataButton.Click += new System.EventHandler(this.setEmployerDataButton_Click);
            // 
            // addEmployerDataButton
            // 
            this.addEmployerDataButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.addEmployerDataButton.Enabled = false;
            this.addEmployerDataButton.Location = new System.Drawing.Point(30, 4);
            this.addEmployerDataButton.Name = "addEmployerDataButton";
            this.addEmployerDataButton.Size = new System.Drawing.Size(75, 23);
            this.addEmployerDataButton.TabIndex = 1;
            this.addEmployerDataButton.Text = "добавить";
            this.addEmployerDataButton.UseVisualStyleBackColor = true;
            this.addEmployerDataButton.Click += new System.EventHandler(this.addEmployerDataButton_Click);
            // 
            // dumpFilePanelBox
            // 
            this.dumpFilePanelBox.Controls.Add(this.tableLayoutPanel3);
            this.dumpFilePanelBox.Location = new System.Drawing.Point(3, 351);
            this.dumpFilePanelBox.Name = "dumpFilePanelBox";
            this.dumpFilePanelBox.Size = new System.Drawing.Size(453, 212);
            this.dumpFilePanelBox.TabIndex = 2;
            this.dumpFilePanelBox.TabStop = false;
            this.dumpFilePanelBox.Text = "Управление файлами дампов";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label9, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label10, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.staticDataFileInput, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.currentDumpFileInput, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.selectDumpFileButton, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.selectStaticDataFileButton, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.button1, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.groupBox2, 0, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(447, 193);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(128, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Текущий рабочий файл:";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Файл статических данных:";
            // 
            // staticDataFileInput
            // 
            this.staticDataFileInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.staticDataFileInput.Enabled = false;
            this.staticDataFileInput.Location = new System.Drawing.Point(153, 33);
            this.staticDataFileInput.Name = "staticDataFileInput";
            this.staticDataFileInput.Size = new System.Drawing.Size(210, 20);
            this.staticDataFileInput.TabIndex = 4;
            this.staticDataFileInput.Text = "static.xls";
            // 
            // currentDumpFileInput
            // 
            this.currentDumpFileInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.currentDumpFileInput.Enabled = false;
            this.currentDumpFileInput.Location = new System.Drawing.Point(153, 4);
            this.currentDumpFileInput.Name = "currentDumpFileInput";
            this.currentDumpFileInput.Size = new System.Drawing.Size(210, 20);
            this.currentDumpFileInput.TabIndex = 3;
            // 
            // selectDumpFileButton
            // 
            this.selectDumpFileButton.Location = new System.Drawing.Point(369, 3);
            this.selectDumpFileButton.Name = "selectDumpFileButton";
            this.selectDumpFileButton.Size = new System.Drawing.Size(75, 23);
            this.selectDumpFileButton.TabIndex = 5;
            this.selectDumpFileButton.Text = "выбрать...";
            this.selectDumpFileButton.UseVisualStyleBackColor = true;
            this.selectDumpFileButton.Click += new System.EventHandler(this.selectDumpFileButton_Click);
            // 
            // selectStaticDataFileButton
            // 
            this.selectStaticDataFileButton.Enabled = false;
            this.selectStaticDataFileButton.Location = new System.Drawing.Point(369, 32);
            this.selectStaticDataFileButton.Name = "selectStaticDataFileButton";
            this.selectStaticDataFileButton.Size = new System.Drawing.Size(75, 23);
            this.selectStaticDataFileButton.TabIndex = 6;
            this.selectStaticDataFileButton.Text = "выбрать...";
            this.selectStaticDataFileButton.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(369, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "обновить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.LoadStaticTests);
            // 
            // groupBox2
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox2, 3);
            this.groupBox2.Controls.Add(this.tableLayoutPanel4);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(441, 100);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Объединение файлов дампов";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.sourceDumpFileInput, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.mergeButton, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.selectSourceButton, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(435, 81);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // sourceDumpFileInput
            // 
            this.sourceDumpFileInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceDumpFileInput.Enabled = false;
            this.sourceDumpFileInput.Location = new System.Drawing.Point(3, 3);
            this.sourceDumpFileInput.Multiline = true;
            this.sourceDumpFileInput.Name = "sourceDumpFileInput";
            this.tableLayoutPanel4.SetRowSpan(this.sourceDumpFileInput, 2);
            this.sourceDumpFileInput.Size = new System.Drawing.Size(348, 75);
            this.sourceDumpFileInput.TabIndex = 1;
            // 
            // mergeButton
            // 
            this.mergeButton.Location = new System.Drawing.Point(357, 32);
            this.mergeButton.Name = "mergeButton";
            this.mergeButton.Size = new System.Drawing.Size(75, 23);
            this.mergeButton.TabIndex = 2;
            this.mergeButton.Text = "объединить";
            this.mergeButton.UseVisualStyleBackColor = true;
            // 
            // selectSourceButton
            // 
            this.selectSourceButton.Location = new System.Drawing.Point(357, 3);
            this.selectSourceButton.Name = "selectSourceButton";
            this.selectSourceButton.Size = new System.Drawing.Size(75, 23);
            this.selectSourceButton.TabIndex = 3;
            this.selectSourceButton.Text = "выбрать...";
            this.selectSourceButton.UseVisualStyleBackColor = true;
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1041, 590);
            this.Controls.Add(this.adminPanel);
            this.Controls.Add(this.appMenu);
            this.MainMenuStrip = this.appMenu;
            this.Name = "AppForm";
            this.Text = "Визуализация данных ТКДА";
            this.Load += new System.EventHandler(this.AppForm_Load);
            this.appMenu.ResumeLayout(false);
            this.appMenu.PerformLayout();
            this.adminPanel.ResumeLayout(false);
            this.adminPanel.PerformLayout();
            this.sqlPanelBox.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.reportPanelBox.ResumeLayout(false);
            this.reportTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pdfViewer)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.usersPanelBox.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.dumpFilePanelBox.ResumeLayout(false);
            this.dumpFilePanelBox.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip appMenu;
        private System.Windows.Forms.ToolStripMenuItem exitMenuOption;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuOption;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.Panel userPanel;
        private System.Windows.Forms.ListBox usersList;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statisticToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugMenuOption;
        private System.Windows.Forms.Panel adminPanel;
        private System.Windows.Forms.GroupBox usersPanelBox;
        private System.Windows.Forms.GroupBox dumpFilePanelBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TextBox currentDumpFileInput;
        private System.Windows.Forms.TextBox staticDataFileInput;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox sqlPanelBox;
        private System.Windows.Forms.GroupBox reportPanelBox;
        private System.Windows.Forms.TabControl reportTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private GroupBox groupBox5;
        private TableLayoutPanel tableLayoutPanel7;
        private TextBox passwordInput;
        private Label label3;
        private TextBox loginInput;
        private Label label8;
        private TextBox departmentInput;
        private TextBox patronymicNameInput;
        private TextBox nameInput;
        private TextBox surnameInput;
        private Label label6;
        private Label label5;
        private ComboBox postInput;
        private Label label4;
        private Label label7;
        private Label label2;
        private Button setEmployerDataButton;
        private Button selectDumpFileButton;
        private Button selectStaticDataFileButton;
        private Button mergeButton;
        private Button selectSourceButton;
        private Button button1;
        private TextBox sourceDumpFileInput;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox sqlOutput;
        private TextBox sqlInput;
        private TableLayoutPanel tableLayoutPanel2;
        private Button addEmployerDataButton;
        private SplitContainer splitContainer2;
        private TableLayoutPanel tableLayoutPanel6;
        private Label label1;
        private Button createReportButton;
        private Label label11;
        private ComboBox reportTypeComboBox;
        private ListBox challengeDates;
        private SplitContainer splitContainer3;
        private TableLayoutPanel tableLayoutPanel8;
        private Label label13;
        private Button button3;
        private TableLayoutPanel tableLayoutPanel9;
        private Label label14;
        private Label label15;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private AxAcroPDFLib.AxAcroPDF pdfViewer;
        private CheckedListBox checkedListBox1;
        private Label label16;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}