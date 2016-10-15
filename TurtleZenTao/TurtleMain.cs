using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TurtleZenTaoLib;

/**
 * @author 张彪<norkts@gmail.com>
 * @version 1.0 2016-10-14
 */
namespace TurtleZenTao
{
    static class TurtleMain
    {
        /// <summary>
        /// 安装以及测试程序
        /// </summary>
        [STAThread]
        static void Main(string[] argv)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Plugin plugin = new Plugin();
            plugin.pluginInstalled = false;


            if (plugin.ValidateParameters(System.IntPtr.Zero, "")) {
                string[] param = { };
                string ret = plugin.GetCommitMessage(System.IntPtr.Zero, "Chinese", string.Empty, param, "original message");

                MessageBox.Show(ret);
            }
        }
    }
}
