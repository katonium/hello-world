using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrigamiJpegFix
{
    public partial class Form1 : Form
    {
        private bool dirtyFlag = false;
        private bool readOnlyFlag = false;
        private string editFilePath = "";

        public Form1()
        {
            InitializeComponent();
        }


        //[ファイル(&F)]メニュー関連のイベントハンドラ 
        #region FileMenu handlers

        private void setDirty(bool flag)
        {
            dirtyFlag = flag;
            menuSave.Enabled = (readOnlyFlag) ? false : flag;
        }

        private bool confirmDestructionText(string msgboxTitle)
        {
            const string MSG_BOX_STRING = "ファイルは保存されていません。" +
                "\n\n編集中のテキストは破棄されます\n\nよろしいですか?";
            if (!dirtyFlag) return true;
            return (MessageBox.Show(MSG_BOX_STRING, msgboxTitle, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes);

        }
        private void menuNew_Click(object sender, EventArgs e)
        {
            const string MSG_BOX_TITLE = "ファイルの新規作成";
            if (confirmDestructionText(MSG_BOX_TITLE))
            {
                this.Text = "新規ファイル";
                textBox1.Clear();
                editFilePath = "";
                setDirty(false);
            }

        }

        //ToDo:ファイルオープン時に編集中のデータを破棄するか確認or新規タブで開くように設定
        private void openFileDlg_FileOk(object sender, CancelEventArgs e)
        {
            const string TITLE_EXTN_ReadOnly = "(読み取り専用)";
            const string MSGBOX_TITLE = "ファイルオープン";

            editFilePath = openFileDlg.FileName;
            readOnlyFlag = openFileDlg.ReadOnlyChecked;

            this.Text = (readOnlyFlag)
                ? openFileDlg.SafeFileName + TITLE_EXTN_ReadOnly : openFileDlg.SafeFileName;

            try
            {
                textBox1.Text = File.ReadAllText(editFilePath, Encoding.Default);
                setDirty(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, MSGBOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            openFileDlg.ShowDialog(this);
        }

        private void ShowSaveDateTime()
        {
            const string STATUS_STRING = "に保存しました";

            toolStripStatusLabel1.Text = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") +
                STATUS_STRING;
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            const string MSGBOX_TITLE = "ファイルの上書き保存";

            if (File.Exists(editFilePath))
            {
                try
                {
                    File.WriteAllText(editFilePath, textBox1.Text, Encoding.Default);
                    setDirty(false);
                    ShowSaveDateTime();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, MSGBOX_TITLE, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                string MSG_BOX_STRING = "ファイル\"" + editFilePath
                    + "\"のパスは正しくありません。\n\nディレクトリが存在するか確認してください。";
                MessageBox.Show(MSG_BOX_STRING, MSGBOX_TITLE, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private void menuSaveAdd_Click(object sender, EventArgs e)
        {
            const string NEW_FILE_NAME = "新規テキストファイル.txt";
            string fileNameString = GetFileNameString(editFilePath, '\\');

            saveFileDlg.FileName = (fileNameString == "") ? NEW_FILE_NAME : fileNameString;
            saveFileDlg.ShowDialog(this);
        }

        private string GetFileNameString(string filePath, char separateChar)
        {
            try
            {
                string[] strArray = filePath.Split(separateChar);
                return strArray[strArray.Length - 1];
            }
            catch
            {
                return "";
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            setDirty(true);
        }

        private void saveFileDlg_FileOk(object sender, CancelEventArgs e)
        {
            const string MSGBOX_TITLE = "名前を付けて保存";
            editFilePath = saveFileDlg.FileName;
            try
            {
                File.WriteAllText(editFilePath, textBox1.Text, Encoding.Default);
                this.Text = GetFileNameString(editFilePath, '\\');
                setDirty(false);
                ShowSaveDateTime();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, MSGBOX_TITLE,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void menuEnd_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string MSGBOX_TITLE = "アプリケーションの終了";

            if (confirmDestructionText(MSGBOX_TITLE))
            {
                // Form1の破棄
                this.Dispose();
            }
            else
            {
                // ウィンドウを閉じるのをやめる
                e.Cancel = true;
            }
        }

        private void menuUndo_Click(object sender, EventArgs e)
        {
            if(textBox1.CanUndo)
            {
                textBox1.Undo();
                textBox1.ClearUndo();
            }
        }

        private void menuCut_Click(object sender, EventArgs e)
        {
            if(textBox1.SelectedText!="")
            {
                textBox1.Cut();
            }
        }

        private void menuCopy_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectedText != "")
            {
                textBox1.Copy();
            }
        }

        private void menuPaste_Click(object sender, EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                textBox1.Paste();
            }




        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            textBox1.Cut();
            Clipboard.Clear();
        }

        private void menuAllSelect_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }
        #endregion



    }
}
