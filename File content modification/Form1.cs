using DocumentFormat.OpenXml.Packaging;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using DocumentFormat.OpenXml.Wordprocessing;

namespace File_content_modification
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 選擇資料夾
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.CheckFileExists = false;
                openFileDialog.ValidateNames = false;
                openFileDialog.FileName = "選擇資料夾";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                    lblFolder.Text += folderPath;
                }
            }

        }

        #endregion

        #region 批次替換
        private void btnReplace_Click(object sender, EventArgs e)
        {
            string folderPath = lblFolder.Text.Replace("資料夾選擇：", "").Trim();
            string find = txtFind.Text;
            string replace = txtReplace.Text;

            ReplaceInAllWordDocuments(folderPath, find, replace);
        }

        // 批次處理資料夾
        private void ReplaceInAllWordDocuments(string folderPath, string oldText, string newText)
        {
            string[] files = Directory.GetFiles(folderPath, "*.docx"); // Open XML 僅支援 .docx
            foreach (var file in files)
            {
                try
                {
                    ReplaceTextInWord(file, oldText, newText);
                    lblStatus.Text += $"\r\n {Path.GetFileName(file)} ✅";
                }
                catch (Exception ex)
                {
                    lblStatus.Text += $"\r\n {Path.GetFileName(file)} ❌ {ex.Message}";
                }
            }
        }


        // 替換 Word 文件文字
        private void ReplaceTextInWord(string filePath, string oldText, string newText)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, true))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;

                foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                {
                    if (text.Text.Contains(oldText))
                    {
                        text.Text = text.Text.Replace(oldText, newText);
                    }
                }
                wordDoc.MainDocumentPart.Document.Save();
            }
        }
        #endregion

    }
}
