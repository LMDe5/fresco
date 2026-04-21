using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace battleLogs_backgroundSimulation
{
    internal class GameEvents
    {
        private Random random;
        private GameLogs gameLogs;
        private GameStatus status;
        private CancellationTokenSource cancelledToken;
        private Task tickTask;
        private GameCasts casts;

        public GameEvents(GameLogs gameLogs,GameStatus status, GameCasts casts, Random random)
        {
            this.gameLogs = gameLogs;
            this.status = status;
            this.casts = casts;
            this.random = random;
        }


        public void RunEventAsync()
        {
            cancelledToken = new CancellationTokenSource();
            tickTask = EventLoopAsync(cancelledToken.Token);
        }

        public async Task StopAsync()
        {
            cancelledToken.Cancel();
            if (tickTask != null)
            {
                await tickTask;
            }
            cancelledToken.Dispose();
        }

        private async Task EventLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    int delaySeconds = random.Next(2, 6);
                    await Task.Delay(delaySeconds * 1000, token);

                    RandomEvent();
                }
            }
            catch
            {
            }
        }

        public void Calm()
        {
            string kind = "CALM";

            gameLogs.EventLog(kind, 0, status.PlayerHp);
            Console.WriteLine($"Тишина. Ничего не происходит");
        }
        public void Bandage()
        {
            string kind = "BANDAGE";
            int heal = random.Next(1, 5);
            status.PlayerHp += heal;

            gameLogs.EventLog(kind, heal, status.PlayerHp);
            Console.WriteLine($"Игрок нашел бинт! Игрок получил исцеление {heal}. Текущее здоровье игрока: {status.PlayerHp}");
        }
        public void enemyRage()
        {
            string kind = "RAGE";
            int damage = random.Next(2, 7);
            status.PlayerHp -= damage;

            gameLogs.EventLog(kind, -damage, status.PlayerHp);
            Console.WriteLine($"Ярость врага! Игрок получил урон {damage}. Текущее здоровье игрока: {status.PlayerHp}");

            casts.CastingStopAsync();
        }
        public void RandomEvent()
        {
            int randomValue = random.Next(1, 4);
            switch(randomValue)
            {
                case 1:
                    enemyRage();
                    break;
                case 2:
                    Bandage();
                    break;
                case 3:
                    Calm();
                    break;
            }

        }
    }
}
