namespace BattleShipV2 {
    public enum ShipDirection { Horisont, Vertical, None }
    public class ShipVM : ViewModelBase {
        int rank = 1;
        (int x, int y) pos;
        ShipDirection direction = ShipDirection.Horisont;

        public ShipVM() { }

        public ShipVM(MapVM.Ship ship) {
            pos = (ship.X, ship.Y);
            Rank = ship.Rank;
            Direction = ship.Direction;
        }

        public ShipVM(int x, int y, int rank, ShipDirection direction) {
            pos = (x, y);
            Rank = rank;
            Direction = direction;
        }

        public ShipDirection Direction { get => direction; set => Set(ref direction, value, "Angle"); }

        public int Rank { get => rank; set => Set(ref rank, value, "RankView"); }
        public int RankView => Rank * App.CellSize - 5;
        public int Angle => direction == ShipDirection.Horisont && direction != ShipDirection.None ? 0 : 90;

        public (int, int) Pos {
            get => pos;
            set => Set(ref pos, value, "X", "Y");
        }
        public int X => pos.x * App.CellSize + 3;
        public int Y => pos.y * App.CellSize + 3;
    }
}