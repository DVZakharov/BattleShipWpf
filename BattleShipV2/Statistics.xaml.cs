using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BattleShipV2 {
    public partial class Statistics : Window {
        int userId;

        public Statistics(int userId) {
            InitializeComponent();
            this.userId = userId;

            List<Stat> statView = new List<Stat>();
            AppContext Db = new AppContext();
            List<Statistic> allStat = Db.Statistics.ToList();

            foreach (Statistic stat in allStat)
                if (stat.UserId == userId)
                    statView.Add(new Stat(stat));

            StatDG.ItemsSource = statView;
        }

        struct Stat {
            public string WinOrLose { get; set; }
            public int Steps { get; set; }
            public string Time { get; set; }
            public int DestroyedOurShips { get; set; }
            public int DestroyedEnemyShips { get; set; }

            public Stat(Statistic s) {
                WinOrLose = s.winOrLose;
                Steps = s.Steps;
                Time = s.Time;
                DestroyedOurShips = s.DestoyedOurShips;
                DestroyedEnemyShips = s.DestoyedEnemyShips;
            }

        }

    }
}
