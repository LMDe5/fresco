using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace battleLogs_backgroundSimulation
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            GameLogs gameLogs = new GameLogs();
            GameSaving gameSavings = new GameSaving();
            Random random = new Random();

            await Game(gameLogs, gameSavings, random);

        }

        static void EndGame(GameStatus status, GameLogs logs, GameLogic logic,
            GameEvents events, GameSaving saving, GameLogic.Winner winner)
        {
            if (winner.ToString() == "player")
            {
                Console.WriteLine("Бой закончен. Победил: Противник");
            }
            else
            {
                Console.WriteLine("Бой закончен. Победил: Игрок");
            }

            ShowHistory(logs);

            logic.StopAsync();
            events.StopAsync();

            logs.ClearLog();
            saving.Delete();

            Environment.Exit(0);
        }
        static async Task Game(GameLogs gameLogs, GameSaving saving, Random random)
        {
            GameStatus gameStatus;

            bool logExists = File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "battleLog.txt"));
            bool saveExists = File.Exists(saving.path);
            if (!logExists || !saveExists)
            {
                gameLogs.ClearLog();
                saving.Delete();
                gameStatus = new GameStatus(GameStatus.DefaultPlayerHp, GameStatus.DefaultEnemyHp, 0);
                saving.Save(gameStatus);
                Console.WriteLine("Файлы сохранения созданы. Начинаем новую игру!");
            }
            else
            {
                Console.WriteLine("1 — Начать заново (очистить лог и начать новую игру)");
                Console.WriteLine("2 — Продолжить");
                Console.Write("Ваш выбор:");
                int choice = IntInput(1, 2);
                if (choice == 1)
                {
                    gameLogs.ClearLog();
                    saving.Delete();
                    gameStatus = new GameStatus(GameStatus.DefaultPlayerHp, GameStatus.DefaultEnemyHp, 0);
                    saving.Save(gameStatus);
                    Console.WriteLine("Hачата новая игра!");
                }
                else
                {
                    gameStatus = saving.Load();
                    Console.WriteLine("Игра продолжена!");
                    ShowHistory(gameLogs);
                }
            }
            GameCasts gameCasts = new GameCasts(gameStatus, gameLogs);
            GameEvents events = new GameEvents(gameLogs, gameStatus, gameCasts, random);
            GameLogic logic = new GameLogic(gameStatus, gameLogs, gameCasts, random);

            logic.RunGameClockAsync();
            events.RunEventAsync();
            logic.RunCheckingGameOver();

            logic.OnGameOver = (winner) => EndGame(gameStatus, gameLogs, logic, events, saving, winner);

            bool gameContinues = true;
            while (true)
            {
                if (!gameContinues)
                {
                    Console.WriteLine("Игра завершена!");
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("Доступные команды: 'attack', 'cast', heal', 'stats', 'logs', 'exit'");
                Console.WriteLine("Вводите команду и нажимайте 'Enter'");

                string input = Console.ReadLine().Trim().ToLower();
                switch (input)
                {
                    case "attack":
                        logic.Attack();
                        break;
                    case "cast":
                        logic.Cast();
                        break;
                    case "heal":
                        logic.Heal();
                        break;
                    case "stats":
                        logic.ShowStats();
                        break;
                    case "logs":
                        ShowLogs(gameLogs);
                        break;
                    case "exit":
                        gameContinues = false;
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда. Доступны: attack, heal, stats, exit");
                        break;
                }
            }

            saving.Save(gameStatus);
            await logic.StopAsync();
            await events.StopAsync();
            
        }

        static void ShowLogs(GameLogs logs)
        {
            string[] lines = logs.ReadAllLines();
            int totalLines = lines.Count();

            if (totalLines == 0)
            {
                Console.WriteLine("лог пуст");
                return;
            }
            if (totalLines <= 10)
            {
                foreach(var e in lines)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                int startIndex = totalLines-10;
                for (int i = startIndex; i < totalLines; i++)
                {
                    Console.WriteLine(lines[i]);
                }
            }

        } 
        static void ShowHistory(GameLogs logs)
        {
            string[] lines = logs.ReadAllLines();
            int attack = 0; int heal = 0; int evnt = 0; int castCancel = 0; int castSuccess = 0;
            foreach (string line in lines)
            {
                if (line.Contains("| ATTACK |"))
                {
                    attack++;
                }
                else if (line.Contains("| HEAL |"))
                {
                    heal++;
                }
                else if (line.Contains("| EVENT |"))
                {
                    evnt++;
                }
                else if (line.Contains("| CASTCANCEL |"))
                {
                    castCancel++;
                }
                else if (line.Contains("| CASTSUCCESS |"))
                {
                    castSuccess++;
                }
            }
            Console.WriteLine($"История: ATTACK={attack}, HEAL={heal}, EVENT={evnt}, CAST_CANCELLED={castCancel}, CAST_SUCCESS={castSuccess}");
        }
        static int IntInput(int a, int b)
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int ans) && ans >= a && ans <= b)
                {
                    return ans;
                }
                else
                {
                    Console.WriteLine("Неверный формат ввода!");
                }
            }
        }

        // тут я просто для себя я решил чуть порешать, чтобы понятнее было :)
        //
        //static async Task WaitForEscapeAsync(CancellationTokenSource cancelledToken)
        //{
        //    while (true)
        //    {
        //        if (Console.ReadLine().ToLower() == "esc") 
        //        {
        //            cancelledToken.Cancel();
        //            return;
        //        }
        //        await Task.Delay(100);
        //    }
        //}
        //static async Task DownloadPadAsync(CancellationToken cancelledToken)
        //{
        //    Console.WriteLine("Начинается загрузка... Нажмите Esc для отмены");

        //    for (int i = 0; i < 100; i += 10)
        //    {
        //        if (cancelledToken.IsCancellationRequested)
        //        {
        //            Console.WriteLine("Загрузка отменена");
        //            return;
        //        }

        //        await Task.Delay(500);
        //        Console.WriteLine($"{i}%");
        //    }

        //    Console.WriteLine("Загрузка завершена!");
        //}
    }
}
