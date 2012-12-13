using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace auto
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ImageBox = new Emgu.CV.UI.ImageBox();
            this.DebugWindow = new Emgu.CV.UI.ImageBox();
            this.StepBack = new System.Windows.Forms.Button();
            this.StepRight = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.PlayStop = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.FrameCount = new System.Windows.Forms.Label();
            this.FrameBar = new System.Windows.Forms.TrackBar();
            this.DelayLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DebugWindow)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FrameBar)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox
            // 
            this.ImageBox.AccessibleName = "";
            this.ImageBox.Location = new System.Drawing.Point(12, 12);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(640, 480);
            this.ImageBox.TabIndex = 3;
            this.ImageBox.TabStop = false;
            // 
            // DebugWindow
            // 
            this.DebugWindow.AccessibleName = "DebugWindow";
            this.DebugWindow.Location = new System.Drawing.Point(668, 12);
            this.DebugWindow.Name = "DebugWindow";
            this.DebugWindow.Size = new System.Drawing.Size(640, 480);
            this.DebugWindow.TabIndex = 4;
            this.DebugWindow.TabStop = false;
            // 
            // StepBack
            // 
            this.StepBack.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.StepBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StepBack.Location = new System.Drawing.Point(354, 12);
            this.StepBack.Name = "StepBack";
            this.StepBack.Size = new System.Drawing.Size(57, 53);
            this.StepBack.TabIndex = 5;
            this.StepBack.Text = "←";
            this.StepBack.UseVisualStyleBackColor = true;
            this.StepBack.Click += new System.EventHandler(this.StepBackClick);
            // 
            // StepRight
            // 
            this.StepRight.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.StepRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StepRight.Location = new System.Drawing.Point(904, 13);
            this.StepRight.Name = "StepRight";
            this.StepRight.Size = new System.Drawing.Size(56, 51);
            this.StepRight.TabIndex = 6;
            this.StepRight.Text = "→";
            this.StepRight.UseVisualStyleBackColor = true;
            this.StepRight.Click += new System.EventHandler(this.StepRightClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.06266F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.87469F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.06266F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tableLayoutPanel1.Controls.Add(this.StepRight, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.StepBack, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.PlayStop, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.DelayLabel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 498);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1317, 77);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // PlayStop
            // 
            this.PlayStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlayStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PlayStop.Location = new System.Drawing.Point(1148, 3);
            this.PlayStop.Name = "PlayStop";
            this.PlayStop.Size = new System.Drawing.Size(166, 71);
            this.PlayStop.TabIndex = 8;
            this.PlayStop.Text = "Play";
            this.PlayStop.UseVisualStyleBackColor = true;
            this.PlayStop.Click += new System.EventHandler(this.PlayStopClick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.FrameCount, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.FrameBar, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(417, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(481, 71);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // FrameCount
            // 
            this.FrameCount.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.FrameCount.AutoSize = true;
            this.FrameCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FrameCount.Location = new System.Drawing.Point(200, 0);
            this.FrameCount.Name = "FrameCount";
            this.FrameCount.Size = new System.Drawing.Size(80, 33);
            this.FrameCount.TabIndex = 9;
            this.FrameCount.Text = "0 / 0";
            // 
            // FrameBar
            // 
            this.FrameBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FrameBar.Location = new System.Drawing.Point(3, 36);
            this.FrameBar.Name = "FrameBar";
            this.FrameBar.Size = new System.Drawing.Size(475, 32);
            this.FrameBar.TabIndex = 10;
            this.FrameBar.ValueChanged += new System.EventHandler(this.FrameBarValueChanged);
            // 
            // DelayLabel
            // 
            this.DelayLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DelayLabel.AutoSize = true;
            this.DelayLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DelayLabel.Location = new System.Drawing.Point(50, 26);
            this.DelayLabel.Name = "DelayLabel";
            this.DelayLabel.Size = new System.Drawing.Size(70, 25);
            this.DelayLabel.TabIndex = 10;
            this.DelayLabel.Text = "label1";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1317, 575);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.DebugWindow);
            this.Controls.Add(this.ImageBox);
            this.Name = "Form1";
            this.Text = "Form";
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DebugWindow)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FrameBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public ImageBox ImageBox;
        public ImageBox DebugWindow;
		private Button StepBack;
        private Button StepRight;
		private TableLayoutPanel tableLayoutPanel1;
        private Button PlayStop;
        private TableLayoutPanel tableLayoutPanel2;
        private Label FrameCount;
        private TrackBar FrameBar;
        private Label DelayLabel;
    }
}

