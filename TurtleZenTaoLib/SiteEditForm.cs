using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TurtleZenTaoLib
{
    public partial class SiteEditForm : Form
    {
        private SiteManageForm manageForm;

        private SiteOperate operate = SiteOperate.NEW;

        private int editIndex = 0;

        public SiteEditForm(SiteManageForm manageForm)
        {
            this.manageForm = manageForm;
            InitializeComponent();

            Plugin.lang.langProcess(this);
        }

        /// <summary>
        /// 添加站点btn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addWebsiteBtnClick(object sender, EventArgs e)
        {
            if (operate == SiteOperate.NEW)
            {
                manageForm.addWebSite(this.websiteUrl.Text, this.username.Text, this.password.Text);
            }
            else {
                manageForm.editWebSite(editIndex, websiteUrl.Text, username.Text, password.Text);
            }
            
            setOperate(SiteOperate.NEW);
            this.Hide();
        }

        /// <summary>
        /// 设置弹窗是编辑还是新增
        /// </summary>
        /// <param name="operate"></param>
        public void setOperate(SiteOperate operate) {
            this.operate = operate;

            switch (this.operate)
            {
                case SiteOperate.EDIT: this.addWebsiteBtn.Text = Plugin.lang.getText("Edit"); break;
                default: {
                    this.addWebsiteBtn.Text = Plugin.lang.getText("Add");
                    this.websiteUrl.Text = "";
                    this.username.Text = "";
                    this.password.Text = "";
                } break;
            }
        }

        /// <summary>
        /// 设置编辑的初始化数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void setEditData(int index, string url, string username, string password) {

            this.editIndex = index;

            this.websiteUrl.Text = url;
            this.username.Text = username;
            this.password.Text = password;

            setOperate(SiteOperate.EDIT);

            this.addWebsiteBtn.Text = Plugin.lang.getText("OK");
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Result<string> result = ZenTaoManage.login(websiteUrl.Text, username.Text, password.Text);
            if (result.isSuccess())
            {
                MessageBox.Show(Plugin.lang.getText("Login success"));
            }else
            {
                MessageBox.Show(result.msg);
            }
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }


    }

    public enum SiteOperate
    {
        NEW,
        EDIT
    }
}
