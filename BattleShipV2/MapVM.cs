using static BattleShipV2.MapVM;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace BattleShipV2 {
    public class MapVM : ViewModelBase {
        static Random rnd = new Random();
        CellVM[,] map = new CellVM[10, 10];

        public ObservableCollection<ShipVM> ShipsView { get; } = new ObservableCollection<ShipVM>();
        public ObservableCollection<ShipVM> Ships { get; } = new ObservableCollection<ShipVM>();

        public CellVM this[int x, int y] => map[y, x];

        public IReadOnlyCollection<IReadOnlyCollection<CellVM>> Map {
            get {
                var viewMap = new List<List<CellVM>>();
                for (int y = 0; y < 10; y++) {
                    viewMap.Add(new List<CellVM>());
                    for (int x = 0; x < 10; x++)
                        viewMap[y].Add(this[x, y]);

                }
                return viewMap;
            }
        }

        public void SetShips(List<ShipVM> ships) {
            foreach (var ship in ships) {
                ShipsView.Add(ship);
                Ships.Add(ship);
                var (x, y) = ship.Pos;
                var rank = ship.Rank;
                if (ship.Direction == ShipDirection.Horisont)
                    for (int xx = x; xx < x + rank; xx++)
                        this[xx, y].ToShip();
                else
                    for (int yy = y; yy < y + rank; yy++)
                        this[x, yy].ToShip();
            }
        }

        public MapVM(char name) {
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                    map[x, y] = new CellVM(y, x, name);
        }

        private static List<Ship> GetRandomMap(List<Ship> ships, params int[] navy) { //пересмотреть код (возможно улучшить)
            var p = navy.Length - 1;
            bool[,] m = new bool[10, 10];

            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                    m[x, y] = false;

            int X, Y, rangeX, rangeY;

            while (p > 0) {
                while (navy[p] > 0) {
                    var ship = new Ship(0, 0, p, ShipDirection.Horisont);
                    navy[p]--;
                    bool inst = false;
                    int k = 0;
                    while (k < 10) {
                        k++;
                        ship.Direction = rnd.Next(2) == 0 ? ShipDirection.Horisont : ShipDirection.Vertical;
                        ship.X = ship.Direction == ShipDirection.Horisont ? rnd.Next(11 - p) : rnd.Next(10);
                        ship.Y = ship.Direction == ShipDirection.Horisont ? rnd.Next(10) : rnd.Next(11 - p);
                        if (ship.Rank == 1) ship.Direction = ShipDirection.None;

                        if (!CanPlaceShip(m, ship)) continue; // проверка                       

                        X = ship.X != 0 ? ship.X - 1 : ship.X;
                        Y = ship.Y != 0 ? ship.Y - 1 : ship.Y;

                        if (ship.Direction == ShipDirection.Horisont) {
                            rangeX = X + ship.Rank + 2; rangeY = Y + 3;
                        } else {
                            rangeX = X + 3; rangeY = Y + ship.Rank + 2;
                        }

                        for (int x = X; x < rangeX && x < 10; x++)
                            for (int y = Y; y < rangeY && y < 10; y++)
                                m[x, y] = true;
                        inst = true;
                        break;
                    }
                    if (inst) ships.Add(ship);
                    if (!inst) navy[p]++;
                }
                p--;
            }
            return ships;
        }

        public static bool CanPlaceShip(bool[,] m, Ship ship) {
            if (m[ship.X, ship.Y]) return false;
            if (ship.Direction == ShipDirection.Horisont) {
                for (int x = ship.X; x < ship.X + ship.Rank; x++)
                    if (x > 9 || m[x, ship.Y])
                        return false;
            } else {
                for (int y = ship.Y; y < ship.Y + ship.Rank; y++)
                    if (y > 9 || m[ship.X, y])
                        return false;
            }
            return true;
        }

        public static bool CanPlaceShip(bool[,] m, ShipVM ship) => CanPlaceShip(m, new Ship(ship.Pos.Item1, ship.Pos.Item2, ship.Rank, ship.Direction));

        public void FillMap(params int[] navy) {
            List<Ship> ships = null;
            while (ships == null)
                ships = GetRandomMap(new List<Ship>(), navy);
            foreach (var ship in ships) {
                if (ship.Direction == ShipDirection.Horisont)
                    for (int x = ship.X; x < ship.X + ship.Rank; x++)
                        this[x, ship.Y].ToShip();
                else
                    for (int y = ship.Y; y < ship.Y + ship.Rank; y++)
                        this[ship.X, y].ToShip();
            }
            if (map[0, 0].name == 'O') ShipsView.Clear();
            Ships.Clear();
            foreach (var ship in ships) {
                if (map[0, 0].name == 'O')
                    ShipsView.Add(new ShipVM(ship));
                Ships.Add(new ShipVM(ship));
            }
        }

        public void DeleteShip(ShipVM ship) {
            var temp = new List<ShipVM> { };
            foreach (var t in ShipsView)
                if (!(t.Pos == ship.Pos && t.Direction == ship.Direction))
                    temp.Add(t);
            Ships.Clear();
            ShipsView.Clear();
            foreach (var s in temp) {
                ShipsView.Add(s);
                Ships.Add(s);
            }
            Notify("ShipsView");
            if (ship.Direction == ShipDirection.None) this[ship.Pos.Item1, ship.Pos.Item2].DelShip();
            if (ship.Direction == ShipDirection.Horisont)
                for (int x = ship.Pos.Item1; x < ship.Pos.Item1 + ship.Rank; x++)
                    this[x, ship.Pos.Item2].DelShip();

            if (ship.Direction == ShipDirection.Vertical)
                for (int y = ship.Pos.Item2; y < ship.Pos.Item2 + ship.Rank; y++)
                    this[ship.Pos.Item1, y].DelShip();
        }

        public struct Ship {
            public int X, Y, Rank;
            public ShipDirection Direction;

            public Ship(int x, int y, int rank, ShipDirection direction) {
                X = x; Y = y; Rank = rank; Direction = direction;
            }
        }
    }
}