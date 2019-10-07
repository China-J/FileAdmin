using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace FileAdmin
{
    public partial class Form1 : Form
    {
        List<string> filePathList = new List<string>();
        List<string> fileNameList = new List<string>();
        Image image;
        DataTable dt = new DataTable();
        int gap = 40;
        string configePath = Application.StartupPath + "//data.data";
        DataTable mydt = new DataTable("myTableName");
        string filePath, fileName;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.AllowDrop = true;
            readConfige();
            sizeChange();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            label1.Left = 0;
            //获取拖放数据
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] content = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < content.Length; i++)
                {
                    //这是全路径
                   filePath = content[i];

                   label1.Text = filePath;
                   fileName = System.IO.Path.GetFileName(filePath);
                   if (Directory.Exists(filePath))
                   {
                       image = SystemIcon.GetDirectoryIcon(filePath, true).ToBitmap();
                   }
                   else
                   {
                       image = SystemIcon.GetIcon(filePath, true).ToBitmap();
                   }
                   addList();
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        void sizeChange()
        {
            label1.Left = (this.Width - label1.Width) / 2;

            tabControl1.Left = gap;
            tabControl1.Width = this.Width - 2 * gap;
            tabControl1.Height = this.Height - tabControl1.Top - gap;
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            sizeChange();
        }
        void addList()
        {
            
            dt.Rows.Add(image ,fileName, filePath);
            
            File.AppendAllText(configePath, fileName+"|"+ filePath+"\r\n");
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            if (label1.Text != "向此拖入文件")
            {
                //取文件名
                addList();


            }
        }
        void readConfige()
        {
            if (!File.Exists(configePath))
            {
                FileStream fs;
                fs = File.Create(configePath);
                fs.Close();
            }
            
            dt = new DataTable();
            System.IO.StreamReader mysr = new System.IO.StreamReader(configePath);
            string strline;

            filePathList = new List<string>();
            fileNameList = new List<string>();
            string[] aryline;
            dt.Columns.Add("  ");
            dt.Columns[0].DataType = typeof(Bitmap);
            
            dt.Columns.Add("文件名");
            dt.Columns.Add("路径");
            while ((strline = mysr.ReadLine()) != null)
            {
                aryline = strline.Split(new char[] { '|' });
                if (Directory.Exists(aryline[1]))
                {
                    image = SystemIcon.GetDirectoryIcon(aryline[1], true).ToBitmap();
                }
                else
                {
                    image = SystemIcon.GetIcon(aryline[1], true).ToBitmap();
                }
                dt.Rows.Add(image,aryline[0],aryline[1]);
                
            }
            mysr.Close();
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Resizable = DataGridViewTriState.False;

            dataGridView1.Columns[0].Width = 10;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        void writeConfige(string str)
        {
            try
            {
                
            File.WriteAllText(configePath, str);
            button3.Text = "已保存";
            }
            catch (Exception)
            {
                
            }

 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            readConfige();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string str="";
            int index = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                str += dt.Rows[i][1] +"|"+ dt.Rows[i][2]+ "\r\n";
            }
            foreach (var item in dt.Columns)
            {

                index++;
            }
            writeConfige(str);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        DataTable deleteData;
        private void button4_Click(object sender, EventArgs e)
        {
                deleteData = (DataTable)dataGridView1.DataSource;
                for (int j = 0; j < this.dataGridView1.Rows.Count; j++)
                {
                    if (this.dataGridView1.SelectedRows.Count > 0)
                    {
                        DataRowView drv = this.dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
                        try
                        {

                            drv.Delete();
                            button3.Text = "未保存";
                        }
                        catch (Exception)
                        {

                        }
                    }

            }
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            button3.Text = "未保存";
        }


    }
    class SystemIcon
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        [DllImport("Shell32.dll", EntryPoint = "SHGetFileInfo", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
        [DllImport("User32.dll", EntryPoint = "DestroyIcon")]
        public static extern int DestroyIcon(IntPtr hIcon);
        #region API 参数的常量定义
        public enum FileInfoFlags : uint
        {
            SHGFI_ICON = 0x000000100, // get icon
            SHGFI_DISPLAYNAME = 0x000000200, // get display name
            SHGFI_TYPENAME = 0x000000400, // get type name
            SHGFI_ATTRIBUTES = 0x000000800, // get attributes
            SHGFI_ICONLOCATION = 0x000001000, // get icon location
            SHGFI_EXETYPE = 0x000002000, // return exe type
            SHGFI_SYSICONINDEX = 0x000004000, // get system icon index
            SHGFI_LINKOVERLAY = 0x000008000, // put a link overlay on icon
            SHGFI_SELECTED = 0x000010000, // show icon in selected state
            SHGFI_ATTR_SPECIFIED = 0x000020000, // get only specified attributes
            SHGFI_LARGEICON = 0x000000000, // get large icon
            SHGFI_SMALLICON = 0x000000001, // get small icon
            SHGFI_OPENICON = 0x000000002, // get open icon
            SHGFI_SHELLICONSIZE = 0x000000004, // get shell size icon
            SHGFI_PIDL = 0x000000008, // pszPath is a pidl
            SHGFI_USEFILEATTRIBUTES = 0x000000010, // use passed dwFileAttribute
            SHGFI_ADDOVERLAYS = 0x000000020, // apply the appropriate overlays
            SHGFI_OVERLAYINDEX = 0x000000040 // Get the index of the overlay
        }
        public enum FileAttributeFlags : uint
        {
            FILE_ATTRIBUTE_READONLY = 0x00000001,
            FILE_ATTRIBUTE_HIDDEN = 0x00000002,
            FILE_ATTRIBUTE_SYSTEM = 0x00000004,
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
            FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
            FILE_ATTRIBUTE_DEVICE = 0x00000040,
            FILE_ATTRIBUTE_NORMAL = 0x00000080,
            FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
            FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
            FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
            FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
            FILE_ATTRIBUTE_OFFLINE = 0x00001000,
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
            FILE_ATTRIBUTE_ENCRYPTED = 0x00004000
        }
        #endregion
        /// <summary>
        /// 获取文件类型的关联图标
        /// </summary>
        /// <param name="fileName">文件类型的扩展名或文件的绝对路径</param>
        /// <param name="isLargeIcon">是否返回大图标</param>
        /// <returns>获取到的图标</returns>
        public static Icon GetIcon(string fileName, bool isLargeIcon)
        {
            SHFILEINFO shfi = new SHFILEINFO();
            IntPtr hI;
            if (isLargeIcon)
                hI = SHGetFileInfo(fileName, 0, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)FileInfoFlags.SHGFI_ICON | (uint)FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (uint)FileInfoFlags.SHGFI_LARGEICON);
            else
                hI = SHGetFileInfo(fileName, 0, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)FileInfoFlags.SHGFI_ICON | (uint)FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (uint)FileInfoFlags.SHGFI_SMALLICON);
            Icon icon = Icon.FromHandle(shfi.hIcon).Clone() as Icon;
            DestroyIcon(shfi.hIcon); //释放资源
            return icon;
        }
        /// <summary> 
        /// 获取文件夹图标
        /// </summary> 
        /// <returns>图标</returns> 
        public static Icon GetDirectoryIcon(string filePath, bool isLargeIcon)
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();
            IntPtr _IconIntPtr;
            if (isLargeIcon)
            {
                _IconIntPtr = SHGetFileInfo(filePath, 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), ((uint)FileInfoFlags.SHGFI_ICON | (uint)FileInfoFlags.SHGFI_LARGEICON));
            }
            else
            {
                _IconIntPtr = SHGetFileInfo(filePath, 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), ((uint)FileInfoFlags.SHGFI_ICON | (uint)FileInfoFlags.SHGFI_SMALLICON));
            }
            if (_IconIntPtr.Equals(IntPtr.Zero)) return null;
            Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);
            return _Icon;
        }
    }
}