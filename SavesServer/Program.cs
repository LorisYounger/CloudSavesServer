using LinePutScript;
using LinePutScript.Converter;
using LinePutScript.Dictionary;
using SavesServer.DataBase;
using System.Security.Cryptography.X509Certificates;

namespace SavesServer
{
    public class Program
    {
        public static IFreeSql FSQL => fsql ?? throw new Exception("fsql is null");
        static IFreeSql? fsql { get; set; }
        public static Setting Set { get; set; } = new Setting();

        public static void Main(string[] args)
        {
            Console.WriteLine("Game Saves Server");

            //加载翻译
            var di = new DirectoryInfo(new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + @"\lang");
            if (di.Exists)
                foreach (FileInfo fi in di.EnumerateFiles("*.lps"))
                {
                    LocalizeCore.AddCulture(fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length), new LPS_D(File.ReadAllText(fi.FullName)));
                }

            LoadSetting();

            fsql = new FreeSql.FreeSqlBuilder()
              .UseConnectionString(FreeSql.DataType.MySql, Set.ConnectionString)
              .UseAutoSyncStructure(true)
              .Build();


            var builder = WebApplication.CreateBuilder(args);
            Console.WriteLine("Contact Information:" + Set.ContactInformation);
            if (File.Exists(Set.CertificatePath))
            {
                var x509ca = new X509Certificate2(File.ReadAllBytes(Set.CertificatePath), Set.CertificatePassword);
                builder.WebHost.UseKestrel(option => option.ListenAnyIP(Set.Port, config => config.UseHttps(x509ca)));
                Console.WriteLine("SSL Enabled");
            }
            else
                builder.WebHost.UseKestrel(option => option.ListenAnyIP(Set.Port));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });
            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                // Add the custom model binder provider at the beginning of the providers list
                options.ModelBinderProviders.Insert(0, new LPSAspModel());
            });
            var app = builder.Build();
            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();


            Console.WriteLine("Server Start at Port:" + Set.Port);
            Console.WriteLine("Press Ctrl+C to exit.");

            app.Run();
        }

        public static void LoadSetting()
        {
            var setpath = Path.Combine(Environment.CurrentDirectory, "setting.lps");
            if (File.Exists(setpath))
            {
                Set = LPSConvert.DeserializeObject<Setting>(new LPS(File.ReadAllText(setpath))) ?? Set;
            }
            if (Set.Port == 0)
            {
                Console.WriteLine("Please Enter Server Port: (default:6655)");
                if (ushort.TryParse(Console.ReadLine(), out var result))
                {
                    Set.Port = result;
                }
                else
                {
                    Set.Port = 6655;
                }
            }
            if (Set.ConnectionString == "")
            {
                Console.WriteLine("Please Enter MYSQL ConnectionString: (default:generation)");
                string? result = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(result))
                {
                    Console.Write("Enter the MySQL server name/ipaddr: ");
                    string server = Console.ReadLine() ?? "localhost";
                    Console.Write("Enter the MySQL database name: ");
                    string database = Console.ReadLine() ?? "gamesavesserver";
                    Console.Write("Enter the MySQL username: ");
                    string username = Console.ReadLine() ?? "root";
                    Console.Write("Enter the MySQL password: ");
                    string password = Console.ReadLine() ?? "password";
                    string connectionString = $"Server={server};Database={database};Uid={username};Pwd={password};";
                    Console.WriteLine("\nGenerated MySQL Connection String:");
                    Console.WriteLine(connectionString);
                    Set.ConnectionString = connectionString;
                }
                else
                {
                    Set.ConnectionString = result;
                }
            }
            if (Set.ContactInformation == "")
            {
                Console.WriteLine("Please Enter Contact Information: ");
                string? result = Console.ReadLine();
                Set.ContactInformation = result ?? "NO Contact Information";
            }
            if (Set.CertificatePath == "")
            {
                Console.WriteLine("if using SSL. Please Enter X509Certificate2 Certificate Path (*.pfx): (default:no ssl)");
                string? result = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = null;
                }
                if (File.Exists(result))
                {
                    Set.CertificatePath = result;
                }
                else if (File.Exists(Path.Combine(Environment.CurrentDirectory, result ?? "")))
                {
                    Set.CertificatePath = Path.Combine(Environment.CurrentDirectory, result ?? "");
                }
                else
                {
                    Set.CertificatePath = null;
                }
                if (Set.CertificatePath != null)
                {
                    Console.WriteLine("Please Enter Certificate Password: ");
                    Set.CertificatePassword = Console.ReadLine();
                }
            }
            File.WriteAllText(setpath, LPSConvert.SerializeObject(Set).ToString());
        }

        /// <summary>
        /// 发送日志
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="content">内容</param>
        public static void Log(string type, string content)
        {
            Console.WriteLine($"{type}:{content}");
            FSQL.Insert(new db_Log()
            {
                Type = type,
                Content = content
            }).ExecuteAffrows();
        }

    }
}
