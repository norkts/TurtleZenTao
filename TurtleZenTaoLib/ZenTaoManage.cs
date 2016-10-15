using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace TurtleZenTaoLib
{
    /// <summary>
    /// 禅道的API操作
    /// </summary>
    public class ZenTaoManage
    {
        private string url;
        private string username;
        private string password;

        public ZenTaoManage(string url, string username, string password)
        {
            this.url = url;
            this.password = password;
            this.username = username;
        }
        
   
        public Result<string> login() {
            return login(url, username, password);
        }

        /// <summary>
        /// 登录禅道系统
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Result<string> login(string url, string username, string password)
        {
            //每次登录清除Cookie
            HttpClient.container = new CookieContainer();

            string data = "account=" + Uri.EscapeDataString(username) + "&password=" + Uri.EscapeDataString(password) + "&keepLogin%5B%5D=on";
            string loginUrl = getAPIUrl(url, "user-login");

            Result<string> result = new Result<string>();
            try
            {
                string body = HttpClient.post(loginUrl, Encoding.UTF8.GetBytes(data));

                ZentaoResult res = JsonConvert.DeserializeObject<ZentaoResult>(body);

                if (res.isSuccess())
                {
                    result.status = 1;
                    result.msg = Plugin.lang.getText("Login Success");
                    result.data = "";
                }
                else
                {
                    result.status = 0;
                    result.msg = res.reason;
                    result.data = "";
                }

            }
            catch (Exception e)
            {
                result.status = 0;
                result.msg = e.Message;
                result.data = "";
            }

            return result;
        }

        /// <summary>
        /// 获取BUG列表: 此处的结果有数据缓存，最新的数据调用searchBugList
        /// </summary>
        /// <returns></returns>
        public List<BugInfo> getBugList(){

            string bugUrl = this.getAPIUrl("bug-browse-4-bySearch-myQueryID");

            string body = HttpClient.get(bugUrl);

            ZentaoResult result = JsonConvert.DeserializeObject<ZentaoResult>(body);
            if (result.isSuccess())
            {
                BugResult bugResult = JsonConvert.DeserializeObject<BugResult>(result.data);

                return bugResult.bugs;
            }


            return new List<BugInfo>();
        }

        /// <summary>
        /// 搜索BUG
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<BugInfo> searchBugList(string keyword)
        {

            string buildQuery = getAPIUrl("search-buildQuery");

            string data = "andOr1=AND&field1=assignedTo&operator1=%3D&value1=%24%40me&andOr2=and&field2=title&operator2=include&value2="+ Uri.EscapeUriString(keyword) 
                +"&andOr3=and&field3=keywords&operator3=include&value3=&groupAndOr=and&andOr4=AND&field4=status&operator4=%3D&value4=active&andOr5=and&field5=assignedTo&operator5=%3D&value5=&andOr6=and&field6=resolvedBy&operator6=%3D&value6=&module=bug&actionURL=%2Fbug-browse-4-bySearch-myQueryID.html&groupItems=3&queryID=&formType=more";

            string body = HttpClient.post(buildQuery, Encoding.Default.GetBytes(data));

            return getBugList();
        }

        /// <summary>
        /// 获取任务列表，数据有查询缓存
        /// </summary>
        /// <returns></returns>
        public List<TaskInfo> getTaskList()
        {
            string taskUrl = getAPIUrl("project-task-9-bySearch-myQueryID");

            string body = HttpClient.get(taskUrl);
            ZentaoResult result = JsonConvert.DeserializeObject<ZentaoResult>(body);
            if (result.isSuccess())
            {
                TaskResult taskResult = JsonConvert.DeserializeObject<TaskResult>(result.data);

                return taskResult.tasks;
            }

            return new List<TaskInfo>();
        }

        /// <summary>
        /// 搜索任务
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<TaskInfo> searchTaskList(string keyword)
        {
            string buildQuery = getAPIUrl("search-buildQuery");

            string data = "andOr1=AND&field1=assignedTo&operator1=%3D&value1=%24%40me&andOr2=and&field2=status&operator2=%21%3D&value2=done&andOr3=and&field3=status&operator3=%3D&value3=&groupAndOr=and&andOr4=AND&field4=status&operator4=%21%3D&value4=closed&andOr5=and&field5=name&operator5=include&value5="+

                Uri.EscapeUriString(keyword) + "&andOr6=and&field6=pri&operator6=%3D&value6=0&module=task&actionURL=%2Fproject-task-9-bySearch-myQueryID.html&groupItems=3&queryID=&formType=more";

            string body = HttpClient.post(buildQuery, Encoding.Default.GetBytes(data));

            return getTaskList();
        }

        /// <summary>
        /// 更新BUG
        /// </summary>
        /// <param name="bugId"></param>
        /// <returns></returns>
        public Result<string> updateBug(string bugId) {
            string updateUrl = getAPIUrl("bug-resolve-" + bugId);

            string data = "resolution=fixed&resolvedBuild=trunk&resolvedDate=" + DateTime.Now.ToString("yyyy-MM-dd%20hh:mm:ss")
                + "&assignedTo=" + username;

            string body = HttpClient.post(updateUrl, Encoding.Default.GetBytes(data));

            Result<string> result = new Result<string>();
            result.status = 1;
            result.msg = Plugin.lang.getText("OperateSucess");
            result.data = "";

            return result;
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public Result<string> updateTask(string taskId, string consumed, string left)
        {
            Result<string> result = new Result<string>();
            result.status = 1;
            result.msg = Plugin.lang.getText("OperateSucess");
            result.data = "";

            string updateUrl = getAPIUrl("task-start-" + taskId);
            string data = "realStarted=" + DateTime.Now.ToString("yyyy-MM-dd") + "&consumed=" + string.Format("%d", consumed) + "&left=" + string.Format("%d", left) + "&comment=";

            string body = HttpClient.post(updateUrl, Encoding.Default.GetBytes(data));

            return result;
        }

        public string getAPIUrl(string name)
        {
            return url + (url.EndsWith("/") ? "" : "/") + name + ".json";
        }

        public static string getAPIUrl(string url, string name) {
            return url + (url.EndsWith("/") ? "" : "/") + name + ".json";
        }
    }

    /// <summary>
    /// BUG搜索结果
    /// </summary>
    public class BugResult{
        public List<BugInfo> bugs;
    }

    /// <summary>
    /// BUG详情
    /// </summary>
    public class BugInfo
    {
        public string id;
        public string title;
        public bool isDone;
    }

    /// <summary>
    /// 任务结果
    /// </summary>
    public class TaskResult{
        public List<TaskInfo> tasks;
    }

    /// <summary>
    /// 任务详情
    /// </summary>
    public class TaskInfo{
        public string id;
        public string name;
        public string estimate;
        public string consumed;
        public string left;
        public bool isDone;
    }

    /// <summary>
    /// 禅道API接口统一结果类
    /// </summary>
    public class ZentaoResult {
        public string status;
        public string reason;
        public string data;
        public  UserModel user;

        public bool isSuccess() {
            return status != null && status.Equals("success");
        }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserModel {
        public string id;
        public string account;
    }

    /// <summary>
    /// 程序业务结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public string msg;
        public int status;
        public T data;

        public bool isSuccess()
        {
            return status == 1;
        }
    }
}
