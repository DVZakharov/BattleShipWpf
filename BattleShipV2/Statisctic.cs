using System.ComponentModel.DataAnnotations;

namespace BattleShipV2 {
    public class Statistic {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public string winOrLose;
        private int steps;
        private string time;
        private int destroyedOurShips;
        private int destroyedEnemyShips;

        public int Steps { get => steps; set => steps = value; }
        public string Time { get => time; set => time = value; }
        public int DestoyedEnemyShips { get => destroyedEnemyShips; set => destroyedEnemyShips = value; }
        public int DestoyedOurShips { get => destroyedOurShips; set => destroyedOurShips = value; }
        public string WinOrLose { get => winOrLose; set => winOrLose = value; }

        public Statistic() { }
        public Statistic(int UserId, string winOrLose, int steps, string time, int destroyedOurShips, int destroyedEnemyShips) {
            this.UserId = UserId;
            this.steps = steps;
            this.time = time;
            this.destroyedOurShips = destroyedOurShips;
            this.destroyedEnemyShips = destroyedEnemyShips;
            this.winOrLose = winOrLose;
        }
    }
}
