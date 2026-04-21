using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleLogs_backgroundSimulation
{
    internal class GameSaving
    {
        public readonly string path;
        
        public GameSaving() 
        {
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameSaveFile.txt");
        }

        public GameStatus Load()
        {
            if (!File.Exists(path))
            {
                return new GameStatus(GameStatus.DefaultPlayerHp, GameStatus.DefaultEnemyHp, 0);
            }

            string[] lines = File.ReadAllLines(path);
            int playerHp = int.Parse(lines[0]);
            int enemyHp = int.Parse(lines[1]);
            int tick = int.Parse(lines[2]);

            return new GameStatus(playerHp, enemyHp, tick);
        }


        public void Save(GameStatus status)
        {
            string[] lines = new string[3];
            lines[0] = status.PlayerHp.ToString();
            lines[1] = status.EnemyHp.ToString();
            lines[2] = status.Tick.ToString();
            File.WriteAllLines(path, lines);
        }

        public void Delete()
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

    }
}
