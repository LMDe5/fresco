using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace battleLogs_backgroundSimulation
{
    public class GameLogs
    {
        private readonly string path;

        public GameLogs()
        {
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "battleLog.txt");
        }

        public void AttackLog(int playerHp, int enemyHp, int damage)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string line = $"{timestamp} | ATTACK | dmg={damage} | enemyHp={enemyHp} | playerHp={playerHp}";
            File.AppendAllText(path, line + Environment.NewLine);
        } 

        public void HealLog(int playerHp, int healedHp)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string line = $"{timestamp} | HEAL | value={healedHp} | playerHp={playerHp}";
            File.AppendAllText(path, line + Environment.NewLine);
        }
        public void EventLog(string kind, int playerHp, int hpDelta)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string line = $"{timestamp} | EVENT | kind={kind} | HpDelta={hpDelta} | playerHp={playerHp}";
            File.AppendAllText(path, line + Environment.NewLine);
        }

        public void CastStartedLog(int tick)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string line = $"{timestamp} | CAST_START | durationMs={tick}";
            File.WriteAllText(path, line + Environment.NewLine);
        }

        public void CancelledCastLog(string kind)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string line = $"{timestamp} | CAST_CANCELLED | kind={kind}";
            File.AppendAllText(path, line + Environment.NewLine);
        }

        public void SuccessCastLog(int damage, int enemyHp)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string line = $"{timestamp} | CAST_SUCCESS | bonusDmg={damage} | enemyHp={enemyHp}";
            File.WriteAllText(path, line + Environment.NewLine);
        }

        public void ClearLog()
        {
            File.WriteAllText(path, "");
        }

        public string[] ReadAllLines()
        {
            return File.ReadAllLines(path);
        }
    }
}

