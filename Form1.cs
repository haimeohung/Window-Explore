using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Diagnostics;

namespace BT4
{
    public partial class Form1 : Form
    {
        private int iFolder = 0;
        private int iFile = 0;

        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            
        }
        private Stack<string> path_stack = new Stack<string>();
        private Stack<string> path_stack2 = new Stack<string>();        
        private bool IsCopying = false;
        private bool IsCutting = false;
        private bool IsFolder = false;
        private bool IsListView = false;
        private ListViewItem itemPaste;
        private string pathFolder;
        private string pathFile;
        private string pathSource;
        private string pathDestination;
        private string pathnode;
        #region List view
        //Event list view

        private void AddListView(TreeNode root)
        {
            iFolder = 0;
            iFile = 0;
            string fullpath = root.FullPath;
            string selected_node_path = CreatPath(fullpath);
            listView1.Items.Clear();
            DirectoryInfo[] subfolder_info = new DirectoryInfo(selected_node_path).GetDirectories();
            AddFolderListView(subfolder_info);
            FileInfo[] subfile_info = new DirectoryInfo(selected_node_path).GetFiles();
            AddFileListView(subfile_info);
        }
        private int IconListView(string s)
        {
            if (s == ".png") return 4;
            if (s == ".ppt" || s == ".pptx") return 3;
            if (s == ".doc" || s == ".docx" | s == ".txt") return 5;
            if (s == ".rar" || s == ".zip") return 6;
            if (s == ".mp3") return 2;
            return 1;
        }
        private void AddFileListView(FileInfo[] subfile_info)
        {
            foreach (var file in subfile_info)
            {
                string[] value = new string[5];
                value[0] = file.Name.ToString();
                value[1] = file.Extension;
                value[2] = (file.Length / 1024).ToString();
                value[3] = file.LastWriteTime.ToString();
                value[4] = file.FullName.ToString();
                ListViewItem item1 = new ListViewItem(value);
                item1.ImageIndex = IconListView(value[1]);
                iFile++;
                listView1.Items.Add(item1);
            }
        }
        private void AddFolderListView(DirectoryInfo[] subfolder_info)
        {
            foreach (var item in subfolder_info)
            {
                string[] value = new string[5];
                value[0] = item.Name.ToString();
                value[1] = "Folder";
                value[2] = "N/A";
                value[3] = item.LastWriteTime.ToString();
                value[4] = item.FullName.ToString();
                ListViewItem item1 = new ListViewItem(value);
                item1.ImageIndex = 0;
                iFolder++;
                listView1.Items.Add(item1);

            }
        }
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListViewItem item = listView1.FocusedItem;
                string path = item.SubItems[4].Text;
                FileInfo inf = new FileInfo(path);
                if (inf.Exists)
                {
                    Process.Start(path);
                    return;
                }
                DirectoryInfo inf2 = new DirectoryInfo(path + "\\");
                if (inf2.Exists)
                {
                    txt_path.Text = path + "\\";
                    path_stack.Push(txt_path.Text);
                    listView1.Items.Clear();
                    DirectoryInfo[] subfolder_info = new DirectoryInfo(path).GetDirectories();
                    AddFolderListView(subfolder_info);
                    FileInfo[] subfile_info = new DirectoryInfo(path).GetFiles();
                    AddFileListView(subfile_info);
                }
                else
                {
                    MessageBox.Show("Not Found");
                }
            }
            catch (Exception) { MessageBox.Show("Access Denied"); }

        }
        private void listView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                ListViewItem item = listView1.FocusedItem;
                string path = item.SubItems[4].Text;
                FileInfo inf = new FileInfo(path);
                if (inf.Exists)
                {
                    Process.Start(path);
                    return;
                }
                DirectoryInfo inf2 = new DirectoryInfo(path + "\\");
                if (inf2.Exists)
                {
                    txt_path.Text = path + "\\";
                    path_stack.Push(txt_path.Text);
                    listView1.Items.Clear();
                    DirectoryInfo[] subfolder_info = new DirectoryInfo(path).GetDirectories();
                    AddFolderListView(subfolder_info);
                    FileInfo[] subfile_info = new DirectoryInfo(path).GetFiles();
                    AddFileListView(subfile_info);
                }
                else
                {
                    MessageBox.Show("Not Found");
                }
            }
            catch (Exception) { MessageBox.Show("Access Denied"); }

        }
        #endregion

        #region tree view
        //Event tree view
        private void AddTreeView(TreeNode root)
        {
            string fullpath = root.FullPath;
            string selected_node_path = CreatPath(fullpath);
            if (Directory.Exists(selected_node_path))
            {
                string[] sub_folder_path = Directory.GetDirectories(selected_node_path);
                root.Nodes.Clear();
                foreach (var folder in sub_folder_path)
                {
                    string[] ss = folder.Split('\\');
                    TreeNode subnode = new TreeNode(ss[ss.Length - 1]);
                    root.Nodes.Add(subnode);
                }
            }
            root.Expand();
        }
        private int IconTreeView(string s)
        {
            int tmp = 1;
            if (s == "3") tmp = 2;
            if (s == "4") tmp = 1;
            if (s == "5") tmp = 3;
            return tmp;
        }
       
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                //1. Xac dinh node dang chon
                TreeNode selected_node = e.Node;
                //2. Lay thong tin cac folder cua node dang chon
                //2.1 tao duong dan dan folder dang chon
                string selected_node_path = "";
                string fullpath = selected_node.FullPath;
                
                string[] sub_path = fullpath.Split('\\');
                selected_node_path = sub_path[1];
                for (int i = 2; i < sub_path.Length; i++)
                {
                    selected_node_path += sub_path[i] + "\\";
                }
                path_stack.Push(selected_node_path);
                txt_path.Text = selected_node_path;
                //2.2 Lay folder trong thu muc vua tao
                if (Directory.Exists(selected_node_path))
                {
                    string[] sub_folder = Directory.GetDirectories(selected_node_path);
                    selected_node.Nodes.Clear();
                    foreach (var folder in sub_folder)
                    {
                        string[] split_folder = folder.Split('\\');
                        TreeNode node = new TreeNode(split_folder[split_folder.Length - 1]);
                        selected_node.Nodes.Add(node);

                    }
                    selected_node.Expand();
                    listView1.Items.Clear();
                    DirectoryInfo[] sub_folder_info = new DirectoryInfo(selected_node_path).GetDirectories();
                    AddFolderListView(sub_folder_info);                   
                    FileInfo[] sub_file_info = new DirectoryInfo(selected_node_path).GetFiles();
                    AddFileListView(sub_file_info);
                    status_bar.Text = iFolder + " Folder(s) " + iFile + " File(s)";
                    
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        //Event toolstrip
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dlr = MessageBox.Show("Are you sure?", "Message", MessageBoxButtons.YesNo);
            if (dlr == DialogResult.Yes) this.Close();

        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About_me fr = new About_me();
            fr.ShowDialog();
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //kiểm tra có items
                if (listView1.SelectedItems.Count > 0)
                {
                    //tạo đường dẫn đến item
                    string fullpath = treeView1.SelectedNode.FullPath;
                    string selected_node_path = CreatPath(fullpath);
                    // tạo đường dẫn đến node chính bên trong treeview
                    selected_node_path += listView1.SelectedItems[0].SubItems[0].Text;
                    //Console.WriteLine(listView1.SelectedItems[0].SubItems[1].Text);//CompareTo("Folder"));
                    //kiểm rea đương dẫn
                    if (listView1.SelectedItems[0].SubItems[1].Text.CompareTo("Folder") == 0)
                    {
                        if (Directory.Exists(selected_node_path))
                        {
                            Directory.Delete(selected_node_path, true);
                        }
                    }
                    else
                    {
                        if (File.Exists(selected_node_path))
                        {
                            File.Delete(selected_node_path);
                        }
                    }
                    //xóa-  kiểm tra và xóa thư mục/ tập tin
                    listView1.Items.Remove(listView1.Items[0]);
                }
                item_refresh.PerformClick();
            }
            catch (Exception)
            {
                MessageBox.Show("Acess Denied");
            }
        }
        #endregion

        #region button
        //Event button
        private void btn_Go_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_path.Text != "")
                {
                    FileInfo inf = new FileInfo(txt_path.Text.Trim());
                    if (inf.Exists)
                    {
                        System.Diagnostics.Process.Start(txt_path.Text.Trim());
                        DirectoryInfo parent = inf.Directory;
                        txt_path.Text = parent.FullName;
                        return;

                    }
                    else
                    {
                        listView1.Items.Clear();
                        DirectoryInfo[] subfolder_info = new DirectoryInfo(txt_path.ToString()).GetDirectories();
                        AddFolderListView(subfolder_info);
                        FileInfo[] subfile_info = new DirectoryInfo(txt_path.ToString()).GetFiles();
                        AddFileListView(subfile_info);
                    }


                }

            }
            catch (Exception)
            {
                MessageBox.Show("Message", "Link not found!");
            }

        }
        private void btn_cut_Click(object sender, EventArgs e)
        {
            IsCutting = true;
            if (listView1.Focused)
            {
                IsListView = true;
                itemPaste = listView1.FocusedItem;
                itemPaste.ForeColor = Color.Gray;
                if (itemPaste == null) return;
                if (itemPaste.SubItems[1].Text.Trim() == "Folder")
                {
                    IsFolder = true;
                    pathFolder = itemPaste.SubItems[4].Text + "\\";
                }
                else
                {
                    IsFolder = false;
                    pathFile = itemPaste.SubItems[4].Text;
                }
            }          
            btn_paste.Enabled = true;
            item_paste.Enabled = true;
        }
        private void btn_copy_Click(object sender, EventArgs e)
        {
            IsCopying = true;
            if (listView1.Focused)
            {
                IsListView = true;
                itemPaste = listView1.FocusedItem;
                if (itemPaste == null) return;
                if (itemPaste.SubItems[1].Text.Trim() == "Folder")
                {
                    IsFolder = true;
                    pathFolder = itemPaste.SubItems[4].Text + "\\";
                }
                else
                {
                    IsFolder = false;
                    pathFile = itemPaste.SubItems[4].Text;
                }
            }           
            btn_paste.Enabled = true;
            item_paste.Enabled = true;
        }
        private void btn_paste_Click(object sender, EventArgs e)
        {
            
            try
            {               
                if (IsListView)
                {
                    if (IsFolder)
                    {
                        pathSource = pathFolder;
                        pathDestination = txt_path.Text.Substring(0, txt_path.Text.Length-1) + "\\" +  itemPaste.SubItems[0].Text;
                    }
                    else
                    {
                        pathSource = pathFile;
                        pathDestination = txt_path.Text.Substring(0, txt_path.Text.Length - 1) + "\\" + itemPaste.SubItems[0].Text;
                    }
                }
                
                if (IsCopying)
                {
                    if (IsFolder)
                    {
                        FileSystem.CopyDirectory(pathSource, pathDestination);
                    }
                    else
                    {
                        FileSystem.CopyFile(pathSource, pathDestination);
                    }
                    IsCopying = false;
                }
                if (IsCutting)
                {
                    if (IsFolder)
                    {
                        FileSystem.MoveDirectory(pathSource, pathDestination);
                    }
                    else
                    {
                        FileSystem.MoveFile(pathSource, pathDestination);
                    }
                    IsCutting = false;
                }
                btn_paste.Enabled = false;
                item_paste.Enabled = false;
                item_refresh_Click(sender, e);
            }
            catch (Exception) {
                MessageBox.Show("Access Denied");
            }
        }
        private void DirectoryCopy(string pathSourece, string pathDestination)
        {

        }
        private void item_refresh_Click(object sender, EventArgs e)
        {
            btn_Go.PerformClick();
            
        }
        #endregion
        #region other function
        //Other function
        private string CreatPath(string fullpath)
        {
            string selected_node_path = "";
            string[] sub_path = fullpath.Split('\\');
            selected_node_path += sub_path[1];
            for (int i = 2; i < sub_path.Length; i++)
            {
                selected_node_path += sub_path[i] + "\\";
            }

            return selected_node_path;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            txt_path.Width = this.Width - 100;
            //1.Clear all node
            if (treeView1 != null)
            {
                treeView1.Nodes.Clear();
            }
            //2.Add root node
            TreeNode root_node = new TreeNode("My Computer", 4, 4);
            this.treeView1.Nodes.Add(root_node);
            //3.Add node
            //3.1. List disk
            ManagementObjectSearcher query = new ManagementObjectSearcher("Select * from Win32_LogicalDisk");
            ManagementObjectCollection col = query.Get();
            //3.2. Create node and add to root node
            foreach (var disk in col)
            {
                TreeNode disk_node = new TreeNode(disk.GetPropertyValue("Name").ToString() + "\\",
                    IconTreeView((disk["DriveType"].ToString())), IconTreeView((disk["DriveType"].ToString())));
                root_node.Nodes.Add(disk_node);
            }
            root_node.Expand();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (txt_path.Text != "")
            {
                btn_up.Enabled = true;
            }
            else
            {
                btn_up.Enabled = false;
            }
            if (path_stack.Count != 0)
            {
                btn_back.Enabled = true;
            }
            else
            {
                btn_back.Enabled = false;
            }
            if (path_stack2.Count != 0)
            {
                btn_forward.Enabled = true;
            }
            else
            {
                btn_forward.Enabled = false;
            }
            if (listView1.Focused == true)
            {
                ct_open.Enabled = true;
            }
            else
            {
                ct_open.Enabled = false;
            }
        }
        #endregion
        private void txt_path_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) btn_Go.PerformClick();
        }
        private void btn_back_Click(object sender, EventArgs e)
        {
            try
            {
                path_stack2.Push(path_stack.Peek());
                path_stack.Pop();
                txt_path.Text = path_stack.Peek();

                btn_Go_Click(sender, e);


            }
            catch (Exception) {}
        }
        private void btn_forward_Click(object sender, EventArgs e)
        {
            try
            {
                path_stack.Push(path_stack2.Pop());
                txt_path.Text = path_stack.Peek();
                btn_Go_Click(sender, e);


            }
            catch (Exception) { }
        }    
        private void txt_path_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void btn_up_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_path.Text != "")
                {
                    string[] sub_path = txt_path.Text.ToString().Split('\\');

                    txt_path.Text = "";
                    for (int i = 0; i < sub_path.Length - 2; i++)
                    {
                        txt_path.Text += sub_path[i] + "\\";
                    }
                }
                path_stack.Push(txt_path.Text);
                btn_Go_Click(sender, e);
                
            }
            catch (Exception) { }
        }      
        private void item_view_Click_1(object sender, EventArgs e)
        {
            //richTextBox1.Clear();
            //foreach (string i in path_stack)
            //{
            //    richTextBox1.Text += i + Environment.NewLine;

            //}
            //richTextBox2.Clear();
            //foreach (string i in path_stack2)
            //{
            //    richTextBox2.Text += i + Environment.NewLine;

            //}
        }
        private void ct_open_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item = listView1.FocusedItem;
                string path = item.SubItems[4].Text;
                FileInfo inf = new FileInfo(path);
                if (inf.Exists)
                {
                    Process.Start(path);
                    return;
                }
                DirectoryInfo inf2 = new DirectoryInfo(path + "\\");
                
                if (inf2.Exists)
                {
                    txt_path.Text = path + "\\";
                    path_stack.Push(txt_path.Text);
                    listView1.Items.Clear();
                    DirectoryInfo[] subfolder_info = new DirectoryInfo(path).GetDirectories();
                    AddFolderListView(subfolder_info);
                    FileInfo[] subfile_info = new DirectoryInfo(path).GetFiles();
                    AddFileListView(subfile_info);
                }
                else
                {
                  
                }
            }
            catch (Exception) { }
        }
    }
}