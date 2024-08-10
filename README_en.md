# CloudSavesServer
游戏云存档系统, 支持将存档(纯文本)备份到服务器上, 并可以随时查看和加载

![icon](README.assets/icon.png)

## 服务端部署

先前条件: 需要 [ASP.Net Core RunTime 6](https://dotnet.microsoft.com/download/dotnet/8.0) 和数据库支持

### Windows

前往 [Release](https://github.com/LorisYounger/CloudSavesServer/releases) 下载 `SavesServer.zip` 

运行 `SavesServer.exe` 即可

### Linux

前往 [Release](https://github.com/LorisYounger/CloudSavesServer/releases) 下载 `SavesServer.zip` 

运行 `dotnet SavesServer.dll` 即可

## 客户端支持

Nuget 安装 ` CloudSaves.Client`

```C#
string serverurl = "http://localhost:6655"; //服务端链接
ulong steamid = 114514; //用户唯一标识符,例如SteamID,QQid等
ulong passkey = 191980; //由程序或用户提供的识别码, 类似于密码, 起到验证和不重复的作用
SavesClient sc = new SavesClient(serverurl, steamid, passkey);
```

## 由官方维护的服务器

| 服务器所在地 | 服务端链接                  | 有效期        |
| ------------ | --------------------------- | ------------- |
| 中国宁波     | https://cn.css.exlb.net     | LTS           |
| 中国香港     | https://hkcss.exlb.net:6655 | LTS           |

