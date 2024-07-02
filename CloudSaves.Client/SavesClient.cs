using System;
using System.Collections.Generic;
using System.Text;

namespace CloudSaves.Client
{
    /// <summary>
    /// 存档客户端
    /// </summary>
    public class SavesClient
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerUrl { get; set; }
        /// <summary>
        /// 新建 存档客户端
        /// </summary>
        /// <param name="serverUrl"></param>
        public SavesClient(string serverUrl)
        {
            ServerUrl = serverUrl;
        }
    }
}
