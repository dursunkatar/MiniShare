using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MiniShare
{
    public partial class FrmMain : Form
    {
        private readonly ShareEngine shareEngine;

        public FrmMain()
        {
            InitializeComponent();
            shareEngine = new ShareEngine();
        }

        private void tsMenuItemAddFile_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            file.Multiselect = true;
            file.Title = "Select Files";

            if (file.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in file.FileNames)
                {
                    var fileInfo = new SharedFileInfo(fileName);
                    var lvi = new ListViewItem();
                    lvi.Text = fileInfo.Name;
                    lvi.SubItems.Add("/" + fileInfo.SharePath);
                    lvi.SubItems.Add(fileInfo.Path);
                    listView.Items.Add(lvi);
                    shareEngine.AddFile(fileInfo);
                }
            }
            file.Dispose();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            shareEngine.Close();
        }

        private void tsMenuItemRemoveFile_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                shareEngine.RemoveFile(item.SubItems[2].Text);
                listView.Items.Remove(item);
            }
        }

        private void tsMenuItemShowShareLink_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                MessageBox.Show("http://localhost:8085" + listView.SelectedItems[0].SubItems[1].Text);
            }
        }
    }
}
