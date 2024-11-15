﻿using CloudSaves.Client;
using LinePutScript;
using LinePutScript.Converter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SavesServer.DataBase;
using System.Collections.Generic;
using static CloudSaves.Client.ReturnStructure;
using static SavesServer.Controllers.UserController;
using static SavesServer.Program;
namespace SavesServer.Controllers
{
    [Route("Saves")]
    [ApiController]
    public class SavesController : ControllerBase
    {
        [HttpGet("")]
        public string Get() => "Saves Server v" + Program.Version;
        /// <summary>
        /// 列出所有该游戏的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("ListGameSaves")]
        public string ListGameSaves(DataStructure.GameData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            var saves = FSQL.Select<db_Save>().Where(a => a.Uid == user.Uid).Where(a => a.GameName == data.GameName).ToList(
                x => new db_Save()
                {
                    GameName = x.GameName,
                    Introduce = x.Introduce,
                    SaveID = x.SaveID,
                    SaveName = x.SaveName,
                    SaveTime = x.SaveTime,
                    Uid = x.Uid
                });
            LPS lps = new LPS();
            foreach (var save in saves)
            {
                var line = LPSConvert.SerializeObject(save, "Saves", convertNoneLineAttribute: true);
                line.Remove("SaveData");
                lps.Add(line);
            }
            return lps.ToString();
        }
        /// <summary>
        /// 获取指定ID的存档
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetGameSave")]
        public string GetGameSave(DataStructure.SaveIDsData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            var saves = FSQL.Select<db_Save>().Where(a => a.Uid == user.Uid).Where(a => data.SaveIDs.Contains(a.SaveID)).ToList(
                x => new db_Save()
                {
                    GameName = x.GameName,
                    Introduce = x.Introduce,
                    SaveID = x.SaveID,
                    SaveName = x.SaveName,
                    SaveTime = x.SaveTime,
                    Uid = x.Uid
                });
            LPS lps = new LPS();
            foreach (var save in saves)
            {
                lps.Add(LPSConvert.SerializeObject(save, "Saves", convertNoneLineAttribute: true));
            }
            return lps.ToString();
        }
        /// <summary>
        /// 删除指定ID的存档
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("RemoveGameSave")]
        public string RemoveGameSave(DataStructure.SaveIDsData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            FSQL.Delete<db_Save>().Where(a => a.Uid == user.Uid).Where(a => data.SaveIDs.Contains(a.SaveID)).ExecuteAffrows();
            return "Success";
        }
        /// <summary>
        /// 添加存档
        /// </summary>
        /// <param name="data"></param>
        [HttpPost("AddGameSave")]
        public string AddGameSave(DataStructure.SaveData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            if (!user.ListGames.Contains(data.GameName))
            {
                var list = user.ListGames;
                list.Add(data.GameName);
                user.ListGames = list;
                FSQL.Update<db_User>().SetSource(user).ExecuteAffrows();
            }
            data.IsAutoSave ??= data.SaveName.Contains("Automatic archiving") || data.SaveName.Contains("自动存档")
                    || data.SaveName.Contains("自動存檔");
            var save = new db_Save()
            {
                GameName = data.GameName,
                SaveData = data.GameSaveData,
                IsAutoSave = data.IsAutoSave ?? false,
                SaveTime = DateTime.Now,
                Introduce = data.Introduce,
                SaveName = data.SaveName,
                Uid = user.Uid
            };
            FSQL.Insert(save).ExecuteAffrows();
            Task.Run(() => ManageGameSaves(user.Uid, data.GameName));
            return "Success";
        }
        /// <summary>
        /// 管理用户所有存档
        /// </summary>
        /// <param name="userId">用户id</param>
        public static void ManageGameSaves(int userId, string gamename)
        {
            int deleteMCount = 0;
            int deleteACount = 0;
            // 获取该用户的所有存档
            var totalsaves = FSQL.Select<db_Save>()
                            .Where(s => s.Uid == userId && s.GameName == gamename)
                            .OrderByDescending(s => s.SaveTime)
                            .ToList(x => new { x.SaveID, x.SaveTime, x.IsAutoSave });
            var savesauto = totalsaves.FindAll(x => x.IsAutoSave);
            var savesmanual = totalsaves.FindAll(x => !x.IsAutoSave);
            if (savesauto.Count > Set.BackupMaxAutoperUser)
            {
                if (savesauto.Count <= Set.BackupMaxAutoperUser) return;

                // 计算各个时间段需要保留的存档数量
                int recentCount = (int)(Set.BackupMaxAutoperUser * 0.1);
                int hourlyCount = (int)(Set.BackupMaxAutoperUser * 0.3);
                int dailyCount = (int)(Set.BackupMaxAutoperUser * 0.3);
                int remainingCount = (int)(Set.BackupMaxAutoperUser * 0.3);

                // 1. 保留最近的10%
                var recentSaves = savesauto.Take(recentCount).ToList();

                // 2. 过去24小时的存档
                var cutoffTime24Hours = DateTime.Now.AddHours(-24);
                var hourlySaves = savesauto.Where(s => s.SaveTime >= cutoffTime24Hours).ToList();
                int hourlyAverage = Math.Max(1, hourlySaves.Count / hourlyCount);
                var selectedHourlySaves = hourlySaves
                    .GroupBy(s => new
                    {
                        s.SaveTime.Date,
                        s.SaveTime.Hour
                    })
                    .SelectMany(g => g.Take(hourlyAverage))
                    .ToList();

                // 3. 过去一个月的存档
                var cutoffTime30Days = DateTime.Now.AddDays(-30);
                var dailySaves = savesauto.Where(s => s.SaveTime >= cutoffTime30Days).ToList();
                int dailyAverage = Math.Max(1, dailySaves.Count / dailyCount);
                var selectedDailySaves = dailySaves
                    .GroupBy(s => s.SaveTime.Date)
                    .SelectMany(g => g.Take(dailyAverage))
                    .ToList();

                // 4. 处理剩余存档
                var remainingSaves = savesauto.Except(recentSaves).Except(selectedHourlySaves).Except(selectedDailySaves).ToList();
                int remainingAverage = Math.Max(1, remainingSaves.Count / remainingCount);
                var selectedRemainingSaves = remainingSaves
                    .GroupBy(s => s.SaveTime.Date)
                    .SelectMany(g => g.Take(remainingAverage))
                    .ToList();

                // 5. 合并所有需要保留的存档
                var allSelectedSaves = recentSaves
                    .Concat(selectedHourlySaves)
                    .Concat(selectedDailySaves)
                    .Concat(selectedRemainingSaves)
                    .Distinct()
                    .ToList();

                // 删除不需要的存档
                var savesToDelete = savesauto.Except(allSelectedSaves).ToList();
                foreach (var save in savesToDelete)
                {
                    FSQL.Delete<db_Save>().Where(s => s.SaveID == save.SaveID).ExecuteAffrows();
                    deleteACount++;
                }
            }
            if (savesmanual.Count > Set.BackupMaxManualperUser)
            {
                if (savesmanual.Count <= Set.BackupMaxManualperUser) return;

                // 计算各个时间段需要保留的存档数量
                int recentCount = (int)(Set.BackupMaxManualperUser * 0.1);
                int hourlyCount = (int)(Set.BackupMaxManualperUser * 0.3);
                int dailyCount = (int)(Set.BackupMaxManualperUser * 0.3);
                int remainingCount = (int)(Set.BackupMaxManualperUser * 0.3);

                // 1. 保留最近的10%
                var recentSaves = savesmanual.Take(recentCount).ToList();

                // 2. 过去24小时的存档
                var cutoffTime24Hours = DateTime.Now.AddHours(-24);
                var hourlySaves = savesmanual.Where(s => s.SaveTime >= cutoffTime24Hours).ToList();
                int hourlyAverage = Math.Max(1, hourlySaves.Count / hourlyCount);
                var selectedHourlySaves = hourlySaves
                    .GroupBy(s => new
                    {
                        s.SaveTime.Date,
                        s.SaveTime.Hour
                    })
                    .SelectMany(g => g.Take(hourlyAverage))
                    .ToList();

                // 3. 过去一个月的存档
                var cutoffTime30Days = DateTime.Now.AddDays(-30);
                var dailySaves = savesmanual.Where(s => s.SaveTime >= cutoffTime30Days).ToList();
                int dailyAverage = Math.Max(1, dailySaves.Count / dailyCount);
                var selectedDailySaves = dailySaves
                    .GroupBy(s => s.SaveTime.Date)
                    .SelectMany(g => g.Take(dailyAverage))
                    .ToList();

                // 4. 处理剩余存档
                var remainingSaves = savesmanual.Except(recentSaves).Except(selectedHourlySaves).Except(selectedDailySaves).ToList();
                int remainingAverage = Math.Max(1, remainingSaves.Count / remainingCount);
                var selectedRemainingSaves = remainingSaves
                    .GroupBy(s => s.SaveTime.Date)
                    .SelectMany(g => g.Take(remainingAverage))
                    .ToList();

                // 5. 合并所有需要保留的存档
                var allSelectedSaves = recentSaves
                    .Concat(selectedHourlySaves)
                    .Concat(selectedDailySaves)
                    .Concat(selectedRemainingSaves)
                    .Distinct()
                    .ToList();

                // 删除不需要的存档
                var savesToDelete = savesmanual.Except(allSelectedSaves).ToList();
                foreach (var save in savesToDelete)
                {
                    FSQL.Delete<db_Save>().Where(s => s.SaveID == save.SaveID).ExecuteAffrows();
                    deleteMCount++;
                }
            }

            Console.WriteLine("DEBUG: SaveAuto" + savesauto.Count + " SaveManual" + savesmanual.Count + " DeleteAuto" + deleteACount + " DeleteManual" + deleteMCount);
        }

        /// <summary>
        /// 显示用户存档分布
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="gameName">游戏名称</param>
        public static void DisplayUserSaveDistribution(int userId, string gameName)
        {
            // 获取该用户的所有存档
            var saves = FSQL.Select<db_Save>()
                            .Where(s => s.Uid == userId && s.GameName == gameName)
                            .OrderByDescending(s => s.SaveTime)
                            .ToList();

            // 分类存档
            var recent24Hours = saves.Where(s => s.SaveTime >= DateTime.Now.AddHours(-24)).ToList();
            var recentWeek = saves.Where(s => s.SaveTime >= DateTime.Now.AddDays(-7)).ToList();
            var recentMonth = saves.Where(s => s.SaveTime >= DateTime.Now.AddDays(-30)).ToList();
            var olderSaves = saves.Where(s => s.SaveTime < DateTime.Now.AddDays(-30)).ToList();

            // 生成字符图形
            Console.WriteLine($"存档分布 for User ID: {userId}, Game: {gameName}");
            Console.WriteLine("\n");

            // 最近24小时 (每3小时为一格)
            Console.Write("最近24小时: ");
            for (int i = 0; i < 8; i++)
            {
                int count = recent24Hours.Count(s => s.SaveTime >= DateTime.Now.AddHours(-3 * (i + 1)));
                Console.Write(GetCharacterForCount(count));
            }
            Console.WriteLine();

            // 最近一周 (每天为一格)
            Console.Write("最近一周: ");
            for (int i = 0; i < 7; i++)
            {
                int count = recentWeek.Count(s => s.SaveTime >= DateTime.Now.AddDays(-1 * (i + 1)));
                Console.Write(GetCharacterForCount(count));
            }
            Console.WriteLine();

            // 最近一个月 (每周一格)
            Console.Write("最近一个月: ");
            for (int i = 0; i < 4; i++)
            {
                int count = recentMonth.Count(s => s.SaveTime >= DateTime.Now.AddDays(-7 * (i + 1)));
                Console.Write(GetCharacterForCount(count));
            }
            Console.WriteLine();

            // 超过一个月 (每半个月一格)
            Console.Write("超过一个月: ");
            int halfMonthCount = 0;
            int b = 2;
            while (true)
            {
                int count = olderSaves.Count(s => s.SaveTime >= DateTime.Now.AddDays(-15 * (++b)));
                if (count == 0) break; // 如果没有存档，停止循环
                Console.Write(GetCharacterForCount(count));
                halfMonthCount++;
            }
            if (halfMonthCount == 0) Console.Write("_"); // 如果没有存档，显示'_'
            Console.WriteLine("\n");
        }

        // 辅助方法，根据数量返回字符
        private static char GetCharacterForCount(int count)
        {
            if (count == 0)
                return '_';
            else if (count < 3)
                return '-';
            else if (count < 6)
                return '=';
            else if (count < 10)
                return '^';
            else
                return '|';
        }

    }
}
