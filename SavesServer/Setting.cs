using LinePutScript.Converter;

namespace SavesServer
{
    public class Setting
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        [Line]
        public string ConnectionString { get; set; } = "";
        /// <summary>
        /// 服务器端口
        /// </summary>
        [Line] public ushort Port { get; set; } = 0;
        /// <summary>
        /// 联系方式
        /// </summary>
        [Line] public string ContactInformation { get; set; } = "";
        /// <summary>
        /// 联系方式(多语言)
        /// </summary>
        [Line] public Dictionary<string, string> ContactInformationTrans { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 证书路径(若有)
        /// </summary>
        [Line] public string CertificatePath { get; set; } = "";
        /// <summary>
        /// 证书密码(若有)
        /// </summary>
        [Line] public string CertificatePassword { get; set; } = "";
    }
}
