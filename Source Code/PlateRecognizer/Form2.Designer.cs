namespace PlateRecognizer {
    partial class Form2 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.imgCamUser = new Emgu.CV.UI.ImageBox();
            this.imgCamUser2 = new Emgu.CV.UI.ImageBox();
            this.imgCamUser3 = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser3)).BeginInit();
            this.SuspendLayout();
            // 
            // imgCamUser
            // 
            this.imgCamUser.Location = new System.Drawing.Point(12, 16);
            this.imgCamUser.Name = "imgCamUser";
            this.imgCamUser.Size = new System.Drawing.Size(640, 480);
            this.imgCamUser.TabIndex = 0;
            this.imgCamUser.TabStop = false;
            // 
            // imgCamUser2
            // 
            this.imgCamUser2.Location = new System.Drawing.Point(658, 12);
            this.imgCamUser2.Name = "imgCamUser2";
            this.imgCamUser2.Size = new System.Drawing.Size(320, 240);
            this.imgCamUser2.TabIndex = 1;
            this.imgCamUser2.TabStop = false;
            // 
            // imgCamUser3
            // 
            this.imgCamUser3.Location = new System.Drawing.Point(658, 258);
            this.imgCamUser3.Name = "imgCamUser3";
            this.imgCamUser3.Size = new System.Drawing.Size(320, 240);
            this.imgCamUser3.TabIndex = 4;
            this.imgCamUser3.TabStop = false;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(991, 513);
            this.Controls.Add(this.imgCamUser3);
            this.Controls.Add(this.imgCamUser2);
            this.Controls.Add(this.imgCamUser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form2";
            this.Text = "Plate Recognizer";
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox imgCamUser;
        private Emgu.CV.UI.ImageBox imgCamUser2;
        private Emgu.CV.UI.ImageBox imgCamUser3;
    }
}