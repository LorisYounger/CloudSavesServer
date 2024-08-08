using CloudSaves.Client;
using LinePutScript;
using LinePutScript.Converter;

namespace TestDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Line Data = new Line();
            Data[(gstr)"GameName"] = "TestGame";
            Data[(gstr)"SaveName"] = "TestSave";
            Data[(gstr)"SaveData"] = "TestSaveData";
            Data[(gstr)"Introduce"] = "TestIntroduce";

            Console.WriteLine("SteamID:");
            ulong steamid = ulong.Parse(Console.ReadLine());
            Console.WriteLine("PassKey:");
            ulong passkey = ulong.Parse(Console.ReadLine());

            SavesClient sc = new SavesClient("http://localhost:6655", steamid, passkey);

            while (true)
            {
                Console.Write(">");
                var cmd = Console.ReadLine().Split(' ');
                switch (cmd[0].ToLower())
                {
                    case "exit":
                        return;
                    case "set":
                        Data[(gstr)cmd[1]] = cmd[2];
                        break;
                    case "serveri":
                        Console.WriteLine(LPSConvert.SerializeObject(sc.ServerInfo().Result).ToString());
                        break;
                    case "listgs":
                        Console.WriteLine(LPSConvert.SerializeObject(sc.ListGameSaves(Data[(gstr)"GameName"])).ToString());
                        break;
                    case "getgs":
                        Console.WriteLine(LPSConvert.SerializeObject(sc.GetGameSave(int.Parse(cmd[1]))).ToString());
                        break;
                    case "removegs":
                        Console.WriteLine(sc.RemoveGameSave(int.Parse(cmd[1])));
                        break;
                    case "addgs":
                        Console.WriteLine(sc.AddGameSave(Data[(gstr)"GameName"], Data[(gstr)"Introduce"], Data[(gstr)"SaveName"], Data[(gstr)"SaveData"]).Result);
                        break;
                    case "listg":
                        Console.WriteLine(LPSConvert.SerializeObject(sc.ListGameSaves(Data[(gstr)"GameName"])).ToString());
                        break;
                    case "deletg":
                        Console.WriteLine(sc.DeleteGame(Data[(gstr)"GameName"]));
                        break;
                    case "deletea":
                        Console.WriteLine(sc.DeleteAccount());
                        break;

                }
            }

        }
    }
}
