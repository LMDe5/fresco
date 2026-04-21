using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace battleLogs_backgroundSimulation
{
    internal class GameCasts
    {
        private GameStatus status;
        private GameLogs gameLogs;
        private CancellationTokenSource cancelledToken;
        private bool isCasting = false;

        public bool IsCasting { get { return isCasting; } }

        public GameCasts(GameStatus status, GameLogs gameLogs)
        {
            this.status = status;
            this.gameLogs = gameLogs;
        }
        public void CastingStopAsync()
        {
            if (isCasting && cancelledToken != null)
            {
                cancelledToken.Cancel();
            } 
        }
        //public async Task WaitForStopCastAsync()
        //{
        //    cancelledToken = new CancellationTokenSource();
        //    while (!cancelledToken.IsCancellationRequested || isCasting)
        //    {
        //        if (Console.ReadLine() == "stop".Trim().ToLower())
        //        {
        //            cancelledToken.Cancel();
        //            break;
        //        }
        //        await Task.Delay(100);
        //    }
        //}
        public async Task StartCastingAsync()
        {
            isCasting = true;
            gameLogs.CastStartedLog(status.Tick);
            cancelledToken = new CancellationTokenSource();
            //WaitForStopCastAsync();

            Console.WriteLine("Начинается каст заклинания (3 секунды)...");

            for (int i = 0; i < 30; i++)
            {
                if (cancelledToken.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Каст отменён яростью врага!");
                    gameLogs.CancelledCastLog("rage"); 
                    isCasting = false;
                    cancelledToken.Dispose();
                    cancelledToken = null;
                    return;
                }
                await Task.Delay(100);
            }

            int damage = 25;
            status.EnemyHp -= damage;
            gameLogs.SuccessCastLog(damage, status.EnemyHp);
            Console.WriteLine($"Каст завершен! Враг получает урон в размере {damage} ед. Текущее здоровье противника: {status.EnemyHp}");

            isCasting = false;
            cancelledToken.Dispose();
            cancelledToken = null;
        }
    }
}
