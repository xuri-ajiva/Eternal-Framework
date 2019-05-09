namespace overlaytests {
    partial class Overlay {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && ( components != null )) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.Updater = new System.Windows.Forms.Timer( this.components );
            this.Display = new overlaytests.Source();
            this.SuspendLayout();
            // 
            // Updater
            // 
            this.Updater.Interval = 1;
            this.Updater.Tick += new System.EventHandler( this.IUpdate );
            // 
            // Display
            // 
            this.Display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Display.Location = new System.Drawing.Point( 0, 0 );
            this.Display.Name = "Display";
            this.Display.Size = new System.Drawing.Size( 717, 489 );
            this.Display.TabIndex = 0;
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 8F, 16F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 717, 489 );
            this.Controls.Add( this.Display );
            this.Name = "Overlay";
            this.Text = "Form1";
            this.Load += new System.EventHandler( this.ILoad );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Timer Updater;
        private Source Display;
    }
}

