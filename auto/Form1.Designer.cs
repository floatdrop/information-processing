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
            this.ImageBox = new ImageBox();
            this.DebugWindow = new ImageBox();
            ((ISupportInitialize)(this.ImageBox)).BeginInit();
            ((ISupportInitialize)(this.DebugWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox
            // 
            this.ImageBox.AccessibleName = "";
            this.ImageBox.Location = new Point(12, 12);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new Size(640, 480);
            this.ImageBox.TabIndex = 3;
            this.ImageBox.TabStop = false;
            // 
            // DebugWindow
            // 
            this.DebugWindow.AccessibleName = "DebugWindow";
            this.DebugWindow.Location = new Point(668, 12);
            this.DebugWindow.Name = "DebugWindow";
            this.DebugWindow.Size = new Size(640, 480);
            this.DebugWindow.TabIndex = 4;
            this.DebugWindow.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1317, 501);
            this.Controls.Add(this.DebugWindow);
            this.Controls.Add(this.ImageBox);
            this.Name = "Form1";
            this.SizeGripStyle = SizeGripStyle.Show;
            this.Text = "Form";
            ((ISupportInitialize)(this.ImageBox)).EndInit();
            ((ISupportInitialize)(this.DebugWindow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public ImageBox ImageBox;
        public ImageBox DebugWindow;
    }
}

