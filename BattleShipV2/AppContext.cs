using System.Data.Entity;
using System.Data.SQLite;

namespace BattleShipV2 {
    public class AppContext : DbContext {
        public AppContext() : base(new SQLiteConnection() {
            ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "G:\\Курсач\\BattleShipV2\\BattleShipV2\\DataBase.db", ForeignKeys = true }.ConnectionString
        }, true) {
            DbConfiguration.SetConfiguration(new SQLiteConfiguration());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

    }
}
