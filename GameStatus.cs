using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace battleLogs_backgroundSimulation
{
    internal class GameStatus
    {
        private int playerHp;
        private int enemyHp;
        private int tick;

        public GameStatus(int playerHp, int enemyHp, int tick)
        {
            this.playerHp = playerHp;
            this.enemyHp = enemyHp;
            this.tick = tick;
        }


        public const int MaxPlayerHp = 100;
        public const int DefaultPlayerHp = 100;
        public const int DefaultEnemyHp = 300;
        public int PlayerHp
        {
            get
            {
                return playerHp;
            }
            set
            {
                if (value < 0)
                {
                    playerHp = 0;
                }
                else if (value > MaxPlayerHp)
                {
                    playerHp = MaxPlayerHp;
                }
                else
                {
                    playerHp = value;
                }
            }
        }
        public int EnemyHp
        {
            get
            {
                return enemyHp;
            }
            set
            {
                if (value < 0)
                {
                    enemyHp = 0;
                }
                else
                {
                    enemyHp = value;
                }
            }
        }
        public int Tick { get { return tick; } }
        public void IncrementTick()
        {
            tick++;
        }
        public bool IsPlayerAlive()
        {
            return (playerHp > 0);
        }
        public bool IsEnemyAlive()
        {
            return (enemyHp > 0);
        }
    }
}
