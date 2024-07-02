namespace SavesServer
{
    public class Setting
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; } = "";
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort Port { get; set; } = 0;
        /// <summary>
        /// 联系方式
        /// </summary>
        public string ContactInformation { get; set; } = "";
        /// <summary>
        /// 证书路径(若有)
        /// </summary>
        public string? CertificatePath { get; set; } = "";
        /// <summary>
        /// 证书密码(若有)
        /// </summary>
        public string? CertificatePassword { get; set; } = "";
    }
}
