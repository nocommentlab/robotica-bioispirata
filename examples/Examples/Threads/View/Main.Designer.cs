namespace NocommentLab.Threads.View
{
    partial class Main
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiBtnStartStop = new System.Windows.Forms.Button();
            this.uiLblDateTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // uiBtnStartStop
            // 
            this.uiBtnStartStop.Location = new System.Drawing.Point(122, 13);
            this.uiBtnStartStop.Name = "uiBtnStartStop";
            this.uiBtnStartStop.Size = new System.Drawing.Size(87, 25);
            this.uiBtnStartStop.TabIndex = 0;
            this.uiBtnStartStop.Text = "START";
            this.uiBtnStartStop.UseVisualStyleBackColor = true;
            this.uiBtnStartStop.Click += new System.EventHandler(this.uiBtnStartStop_Click);
            // 
            // uiLblDateTime
            // 
            this.uiLblDateTime.Font = new System.Drawing.Font("Source Code Pro", 14F);
            this.uiLblDateTime.Location = new System.Drawing.Point(2, 134);
            this.uiLblDateTime.Name = "uiLblDateTime";
            this.uiLblDateTime.Size = new System.Drawing.Size(329, 25);
            this.uiLblDateTime.TabIndex = 1;
            this.uiLblDateTime.Text = "n.d.";
            this.uiLblDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 281);
            this.Controls.Add(this.uiLblDateTime);
            this.Controls.Add(this.uiBtnStartStop);
            this.Font = new System.Drawing.Font("Source Code Pro", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Main";
            this.Text = "Threads :: Main";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button uiBtnStartStop;
        private System.Windows.Forms.Label uiLblDateTime;
    }
}

