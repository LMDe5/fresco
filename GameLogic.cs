using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace battleLogs_backgroundSimulation
{
    internal class GameLogic
    {
        private GameStatus status;
        private GameLogs gameLogs;
        private GameCasts casts;
        private Random random;

        private CancellationTokenSource canceledToken;
        private Task tickTask;

        private CancellationTokenSource cancelledTokenGameOver;
        private Task gameOverTickTask;

        public Action OnGameOver;

        public GameLogic(GameStatus status, GameLogs gameLogs, GameCasts casts, Random random)
        {
            this.status = status;
            this.gameLogs = gameLogs;
            this.casts = casts;
            this.random = random;
        }

        public void RunCheckingGameOver()
        {
            cancelledTokenGameOver = new CancellationTokenSource();
            gameOverTickTask = GOTickLoopAsync(cancelledTokenGameOver);
        }

        private async Task GOTickLoopAsync(CancellationTokenSource cancelledToken)
        {
            try
            {
                while (!cancelledToken.IsCancellationRequested)
                {
                    await Task.Delay(500, cancelledToken.Token);

                    if (!status.IsPlayerAlive() || !status.IsEnemyAlive())
                    {
                        OnGameOver.Invoke();
                        return;
                    }
                }
            }
            catch 
            {
            }
        }

        public void RunGameClockAsync()
        {
            canceledToken = new CancellationTokenSource();
            tickTask = TickLoopAsync(canceledToken.Token);
        }

        public async Task StopAsync()
        {
            canceledToken.Cancel();
            if (tickTask != null)
            {
                await tickTask;
            }
            canceledToken.Dispose();

            cancelledTokenGameOver.Cancel();
            if (gameOverTickTask != null)
            {
                await gameOverTickTask;
            }
            cancelledTokenGameOver.Dispose();
        }

        private async Task TickLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);  
                    status.IncrementTick();                  
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void Attack()
        {
            int damage = random.Next(8, 16);
            status.EnemyHp -= damage;

            gameLogs.AttackLog(status.PlayerHp, status.EnemyHp, damage);
            Console.WriteLine($"нанесено урона: {damage}. Текущее здоровье врага: {status.EnemyHp}");
        }

        public void Cast()
        {
            casts.StartCastingAsync();
        }

        public void Heal()
        {
            int heal = random.Next(5, 13);
            status.PlayerHp += heal;

            gameLogs.HealLog(status.PlayerHp, heal);
            Console.WriteLine($"игрок восполняет {heal} здоровья. Текущее: {status.PlayerHp}");
        }

        public void ShowStats()
        {
            Console.WriteLine($"PlayerHp={status.PlayerHp} | EnemyHp={status.EnemyHp} | Tick={status.Tick}");
        }

    }
}
