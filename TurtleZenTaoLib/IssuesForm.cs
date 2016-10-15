using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/**
 * @author 张彪<norkts@gmail.com>
 * @version 1.0 2016-10-14
 */
namespace TurtleZenTaoLib
{
    /// <summary>
    /// BUG以及任务筛选
    /// </summary>
    public partial class IssuesForm : Form
    {
        private Plugin plugin;
        private SearchOperate operate = SearchOperate.BUG;

        private bool taskCheckedState = false;
        private bool bugCheckedState = false;

        private Image checkedCheckBoxImage;
        private Image uncheckedCheckBoxImage;

        private List<TaskInfo> tasks;

        private List<BugInfo> bugs;

        public IssuesForm(Plugin plugin)
        {
            InitializeComponent();

            Plugin.lang.langProcess(this);
            
            this.plugin = plugin;

            bugList.Tag = SearchOperate.BUG;
            taskList.Tag = SearchOperate.TASK;
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// 任务选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void editTask(ListViewItem item)
        {
            WorkTimeEditForm editForm = new WorkTimeEditForm();

            string taskId = item.SubItems[1].Text;

            string taskName = "",
                   consumed = "",
                   left = "";
            bool finished = false;

            foreach (TaskInfo task in tasks)
            {
                if (task.id.Equals(taskId))
                {
                    taskName = task.name;
                    consumed = task.consumed;
                    left = task.left;
                    finished = task.isDone;
                    break;
                }
            }

            editForm.setValues(taskId, taskName, consumed, left, finished);


            editForm.ShowDialog();

            foreach (TaskInfo task in tasks)
            {
                if (task.id.Equals(taskId))
                {
                    task.name = editForm.getTaskName();
                    task.consumed = editForm.getConsumed();
                    task.left = editForm.getLeft();
                    task.isDone = editForm.isFinished();

                    item.SubItems[2].Text = task.name;
                    item.SubItems[4].Text = task.consumed;
                    item.SubItems[5].Text = task.left;
                    item.SubItems[6].Tag = task.isDone;

                    drawCheckBox(taskList, item.SubItems[6], task.isDone);
                    
                    break;
                }
            }
        }

        private void renwuTabChange(object sender, EventArgs e)
        {
            if (renwuTab.TabPages[renwuTab.SelectedIndex] == bugTab)
            {
                operate = SearchOperate.BUG;
                searchBug(searchKeyword.Text);

                checkboxOperate(bugCheckedState);
            }
            else
            {
                operate = SearchOperate.TASK;
                searchTask(searchKeyword.Text);
                checkboxOperate(taskCheckedState);
            }
        }

        private void IssuesForm_Closing(object sender, EventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                plugin.enterSiteManageForm();
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            switch (operate)
            {
                case SearchOperate.TASK: searchTask(searchKeyword.Text); break;
                default: searchBug(searchKeyword.Text); break;
            }
            
        }

        public void searchBug(string keyword) {
            bugs = plugin.zenTaoManage.searchBugList(keyword);
            
            bugList.Items.Clear();

            foreach (BugInfo bug in bugs)
            {
                addBugview(bug);
            }
        }

        public void searchTask(string keyword)
        {
            tasks = plugin.zenTaoManage.searchTaskList(keyword);

            taskList.Items.Clear();

            foreach (TaskInfo task in tasks)
            {
                addTaskView(task);
            }
        }

        private void IssuesForm_Load(object sender, EventArgs e)
        {            
            searchBug("");
        }

        public void addBugview(BugInfo bug) {

            bugList.BeginUpdate();
            ListViewItem item = new ListViewItem();

            item.Text = "";

            item.SubItems.Add(bug.id);
            item.SubItems.Add(bug.title);
            item.SubItems.Add("");
            
            bugList.Items.Add(item);
            bugList.EndUpdate();
        }

        public void addTaskView(TaskInfo task) {
            taskList.BeginUpdate();
            ListViewItem item = new ListViewItem();

            item.Text = "";

            item.SubItems.Add(task.id);
            item.SubItems.Add(task.name);
            item.SubItems.Add(task.estimate);
            item.SubItems.Add(task.consumed);
            item.SubItems.Add(task.left);
            item.SubItems.Add("").Tag = task.isDone;

            taskList.Items.Add(item);
            taskList.EndUpdate();
        }


        /// <summary>
        /// 弹窗确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Hide();
        }

        /// <summary>
        /// 弹窗取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 全选按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkboxOperate(selectAllCheckBox.Checked);
        }


        /// <summary>
        /// 全选/全不选
        /// </summary>
        /// <param name="checkState"></param>
        private void checkboxOperate(bool checkState)
        {
            selectAllCheckBox.Checked = checkState;

            selectAllCheckBox.Text = (selectAllCheckBox.Checked ? Plugin.lang.getText("None") : Plugin.lang.getText("All"));
            ListView.ListViewItemCollection items = bugList.Items;

            if (operate == SearchOperate.TASK)
            {
                items = taskList.Items;
                taskCheckedState = selectAllCheckBox.Checked;
            }
            else
            {
                bugCheckedState = selectAllCheckBox.Checked;
            }

            foreach (ListViewItem item in items)
            {
                item.Checked = selectAllCheckBox.Checked;
            }
        }

        /// <summary>
        /// 获取选中的BUG信息
        /// </summary>
        /// <returns></returns>
       public List<BugInfo> getSelecteBugs() {
           List<BugInfo> result = new List<BugInfo>();

           if (bugs == null)
           {
               return result;
           }

           List<string> ids = new List<string>();
           foreach (ListViewItem item in bugList.Items)
           {
               if (item.Checked)
               {
                   ids.Add(item.SubItems[1].Text);
               }

           }

           foreach(BugInfo bug in bugs){
               if (ids.Contains(bug.id))
               {
                   result.Add(bug);
               }
           }

           return result;    
        }

        /// <summary>
        /// 获取选中的任务列表
        /// </summary>
        /// <returns></returns>
       public List<TaskInfo> getSelectTasks() {
           List<TaskInfo> result = new List<TaskInfo>();

           if (tasks == null)
           {
               return result;
           }

           List<string> ids = new List<string>();
           foreach (ListViewItem item in taskList.Items)
           {
               if (item.Checked)
               {
                   TaskInfo info = new TaskInfo();
                   ids.Add(item.SubItems[1].Text);
               }
               
           }

           foreach (TaskInfo task in tasks)
           {
               if (ids.Contains(task.id))
               {
                   result.Add(task);
               }
           }


           return result;       
       }

        public enum SearchOperate
        {
            BUG,
            TASK
        }

        /// <summary>
        /// 任务和BUG下的列表绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listview_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            ListView listView = (ListView)sender;
            
            if (listView.Tag == null)
            {
                e.DrawDefault = true;
                return;
            }
            SearchOperate type = (SearchOperate)listView.Tag;

            //BUG的解决列和任务的完成列，绘制添加一个复选框
            if ((e.ColumnIndex == 3 && type == SearchOperate.BUG) || (type == SearchOperate.TASK && e.ColumnIndex == 6))
            {
                e.DrawBackground();

                drawCheckBox(listView, e.SubItem, e.SubItem.Tag == null ? false : (bool)e.SubItem.Tag);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        /// <summary>
        /// 获取复选框的图像
        /// </summary>
        /// <param name="isChecked">true为选中的样式，false为未选中的样式</param>
        /// <returns></returns>
        private Image getCheckBoxImage(bool isChecked)
        {

            if (isChecked && this.checkedCheckBoxImage != null)
            {
                return checkedCheckBoxImage;
            }
            else if (!isChecked && this.uncheckedCheckBoxImage != null)
            {
                return uncheckedCheckBoxImage;
            }

            CheckBox chk = new CheckBox();
            chk.Checked = isChecked;
            chk.Text = "";
            chk.Margin = new Padding(3, 3, 3, 3);
            chk.Width = 14;
            chk.Height = 14;
            chk.BackColor = Color.Transparent;

            Bitmap bitmap = new Bitmap(chk.Width, chk.Height);

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);


            chk.DrawToBitmap(bitmap, rect);

            if (isChecked)
            {
                checkedCheckBoxImage = bitmap;
            }
            else
            {
                uncheckedCheckBoxImage = bitmap;
            }


            return bitmap;
        }

        /// <summary>
        /// 在ListView的子项里绘制一个checkbox
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="subItem"></param>
        /// <param name="isChecked"></param>
        private void drawCheckBox(ListView listView, ListViewItem.ListViewSubItem subItem, bool isChecked)
        {
            Graphics graph = listView.CreateGraphics();

            Image icon = getCheckBoxImage(isChecked);

            Brush brush = new SolidBrush(subItem.BackColor);
            graph.FillRectangle(brush, subItem.Bounds.X + 1, subItem.Bounds.Y + 1, subItem.Bounds.Width - 2, subItem.Bounds.Height - 2);
            graph.DrawImage(icon, new Point(subItem.Bounds.X + 5, subItem.Bounds.Y));

            graph.Dispose();
        }

        /// <summary>
        /// 任务完成选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listview_MouseDown(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;

            ListViewHitTestInfo hit = listView.HitTest(e.X, e.Y);

            ListViewItem.ListViewSubItem subItem = hit.SubItem;
            if (subItem == null || listView.Tag == null)
            {
                return;
            }

            //当点击的是任务的选择列时，弹出编辑任务信息弹窗
            if (((SearchOperate)listView.Tag == SearchOperate.TASK && hit.Item.SubItems[0] == subItem && !hit.Item.Checked))
            {
                editTask(hit.Item);
                return;
            }

            //非任务的完成列且非BUG的解决列
            if (((SearchOperate)listView.Tag != SearchOperate.TASK || hit.Item.SubItems[6] != subItem) 
                && (((SearchOperate)listView.Tag != SearchOperate.BUG || hit.Item.SubItems[3] != subItem)))
            {
                return;
            }

            //是否绑定了的数据值
            if (subItem.Tag != null && (bool)subItem.Tag)
            {
                subItem.Tag = false;
            }
            else
            {
                subItem.Tag = true;
            }

            if(((SearchOperate)listView.Tag == SearchOperate.BUG && hit.Item.SubItems[3] == subItem)){
                foreach(BugInfo bug in bugs){
                    if(hit.Item.SubItems[1].Text.Equals(bug.id)){
                        bug.isDone = (bool)subItem.Tag;
                    }
                }
            }

            //绘制复选框
            drawCheckBox(listView, subItem, (bool)subItem.Tag);
        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawText();
        }

        private void listView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawBackground();
            e.DrawText();
        }
    }
}
