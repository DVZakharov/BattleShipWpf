using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;

namespace BattleShipV2 {
    class BattleshipVM : ViewModelBase {
        Random rnd = new Random();
        EnemyBot bot;

        public bool isStart = false;

        DateTime startTime;
        DispatcherTimer timer;
        string time = "00:00";
        public string Time {
            get => time;
            private set => Set(ref time, value);
        }

        public int stepsInt = 0;
        string stepsString = "Шаги: 0";
        public string Steps {
            get => stepsString;
            private set => Set(ref stepsString, value);
        }

        MapVM ourMap = new MapVM('O');
        MapVM enemyMap = new MapVM('E');
        public MapVM OurMap { get => ourMap; private set => Set(ref ourMap, value); }
        public MapVM EnemyMap { get => enemyMap; private set => Set(ref enemyMap, value); }

        private char SelectedBot = 'm';

        public BattleshipVM() {
            timer = new DispatcherTimer() {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            OurMap = new MapVM('O');
            EnemyMap = new MapVM('E');
            EnemyBot bot = new EnemyBot();
        }

        public void ChangeBot() {
            SelectedBot = SelectedBot == 'l' ? 'm' : 'l';
        }

        internal void ShotToOurMap() {
            if (SelectedBot == 'l') {
                while (true) {
                    var X = rnd.Next(10);
                    var Y = rnd.Next(10);

                    if (OurMap[X, Y].Miss == Visibility.Visible || OurMap[X, Y].Shot == Visibility.Visible)
                        continue;
                    else {
                        OurMap[X, Y].ToShot();
                        if (OurMap[X, Y].Shot == Visibility.Visible) {
                            Destroyed(OurMap[X, Y]);
                            continue;
                        }
                        break;
                    }
                }
            } else {
                int count;
                while (true) {
                    (int X, int Y) = bot.GetShot();
                    OurMap[X, Y].ToShot();
                    if (OurMap[X, Y].Shot == Visibility.Visible) {
                        bot.IsShot(true);
                        count = SumShipArr(ourShipsKilled);
                        Destroyed(OurMap[X, Y]);
                        if (IsWin() != 0) break;
                        if (count < SumShipArr(ourShipsKilled))
                            bot.ShipIsDead(true, SumShipArr(ourShipsKilled) - count);
                        else bot.ShipIsDead(false);
                        continue;
                    }
                    bot.IsShot(false);
                    break;
                }
            }
        }

        private static int SumShipArr(int[] arrShips) {
            int ret = 0;
            for (int i = 0; i < 5; i++)
                ret += arrShips[i] * i;
            return ret;
        }

        public void DeleteOurMap() => OurMap = new MapVM('O');

        public void ClearOurMap() {
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                    OurMap[x, y].ClearShot();
        }

        public void UpdateSteps() {
            stepsInt += 1;
            Steps = stepsString.Substring(0, 5);
            if (stepsInt == 10 || stepsInt == 100) 
                Steps += ' ';                        
            Steps += stepsInt;
        }

        private void Timer_Tick(object sender, EventArgs e) => Time = (DateTime.Now - startTime).ToString(@"mm\:ss");

        public void Start() {
            startTime = DateTime.Now;
            timer.Start();

            stepsInt = 0;
            stepsString = $"Шаги: {stepsInt}";
            Notify("Steps");

            isStart = true;

            EnemyMap = new MapVM('E');
            EnemyMap.FillMap(0, 4, 3, 2, 1);
            ClearOurMap();

            ourShipsKilled = new int[5];
            enemyShipsKilled = new int[5];
            bot = new EnemyBot();
        }

        public void Stop() {
            isStart = false;
            timer.Stop();
        }

        ShipDirection shipSetDirection = ShipDirection.Horisont;
        int rankSetShip = 4;
        public int SelectedRank { get => rankSetShip; set => Set(ref rankSetShip, value); }
        int[] placedShips = new int[5];
        bool[,] map;

        public void CreateFalseMap() {
            bool thereAreShips = OurMap.Ships.Count > 0;
            map = new bool[10, 10];
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++) {
                    if (!thereAreShips)
                        map[x, y] = false;
                    else map[x, y] = OurMap[x, y].IsShip();
                }
            if (thereAreShips) {
                bool[,] temp = new bool[10, 10];
                Array.Copy(map, temp, 100);
                for (int x = 0; x < 10; x++)
                    for (int y = 0; y < 10; y++) {
                        if (temp[x, y]) {
                            if (x != 0) {
                                map[x - 1, y] = true;
                                if (y != 0)
                                    map[x - 1, y - 1] = true;
                                if (y != 9)
                                    map[x - 1, y + 1] = true;
                            }
                            if (x != 9) {
                                map[x + 1, y] = true;
                                if (y != 0)
                                    map[x + 1, y - 1] = true;
                                if (y != 9)
                                    map[x + 1, y + 1] = true;
                            }
                            if (y != 9)
                                map[x, y + 1] = true;
                            if (y != 0)
                                map[x, y - 1] = true;

                        }
                    }
            }
        }

        public void DelNavy() => placedShips = new int[5];

        public void EditSelectedShipRank(Button btn) {
            if (btn.Name == "PlusRank")
                if (rankSetShip < 4)
                    rankSetShip += 1;

            if (btn.Name == "MinusRank")
                if (rankSetShip > 1)
                    rankSetShip -= 1;
            Notify("SelectedRank");
        }

        internal void EditOurMap(object sender, MouseButtonEventArgs e) {
            var cell = (sender as Border).DataContext as CellVM;

            int xCell = cell.x; int yCell = cell.y;

            if (e.MiddleButton == MouseButtonState.Pressed)
                RotateSetShip();

            if (e.LeftButton == MouseButtonState.Pressed)
                SetShip(xCell, yCell);

            if (e.RightButton == MouseButtonState.Pressed)
                DeleteShip(xCell, yCell);
        }

        private void RotateSetShip() {
            if (shipSetDirection == ShipDirection.Horisont)
                shipSetDirection = ShipDirection.Vertical;
            else shipSetDirection = ShipDirection.Horisont;
        }

        private void SetShip(int x, int y) {
            var ship = new ShipVM(x, y, rankSetShip, shipSetDirection);
            if (rankSetShip == 1) ship.Direction = ShipDirection.None;

            int maxPlaceShipOnThisRank = 4;
            if (rankSetShip == 4) maxPlaceShipOnThisRank = 1;
            if (rankSetShip == 3) maxPlaceShipOnThisRank = 2;
            if (rankSetShip == 2) maxPlaceShipOnThisRank = 3;

            if (MapVM.CanPlaceShip(map, ship) && placedShips[rankSetShip] < maxPlaceShipOnThisRank) {
                placedShips[rankSetShip] += 1;
                OurMap.SetShips(new List<ShipVM> { ship });
                CreateFalseMap();
            }
        }

        private static ShipVM ShipDefinition(int x, int y, MapVM map) {
            ShipVM ship = null;

            if ((x == 9 || (x != 9 && !map[x + 1, y].IsShip())) &&
                (x == 0 || (x != 0 && !map[x - 1, y].IsShip())) &&
                (y == 9 || (y != 9 && !map[x, y + 1].IsShip())) &&
                (y == 0 || (y != 0 && !map[x, y - 1].IsShip())))
                ship = new ShipVM(x, y, 1, ShipDirection.None);

            if (ship == null)
                if ((x < 9 && map[x + 1, y].IsShip()) || (x > 0 && map[x - 1, y].IsShip())) {
                    ship = new ShipVM {
                        Direction = ShipDirection.Horisont,
                        Pos = (x, y)
                    };
                    for (int i = x; i < 10; i++)
                        if (map[i, y].IsShip())
                            ship.Pos = (i, y);
                        else break;
                    if (map[ship.Pos.Item1 - 1, ship.Pos.Item2].IsShip()) {
                        int i = ship.Pos.Item1;
                        for (; i >= 0; i--)
                            if (map[i, ship.Pos.Item2].IsShip())
                                ship.Rank = ship.Pos.Item1 - i + 1;
                            else break;
                        ship.Pos = (i + 1, y);
                    }
                }
            if (ship == null)
                if ((y < 9 && map[x, y + 1].IsShip()) || (y > 0 && map[x, y - 1].IsShip())) {
                    ship = new ShipVM {
                        Direction = ShipDirection.Vertical,
                        Pos = (x, y)
                    };
                    for (int i = y; i < 10; i++)
                        if (map[x, i].IsShip())
                            ship.Pos = (x, i);
                        else break;
                    if (map[ship.Pos.Item1, ship.Pos.Item2 - 1].IsShip()) {
                        int i = ship.Pos.Item2;
                        for (; i >= 0; i--)
                            if (map[ship.Pos.Item1, i].IsShip())
                                ship.Rank = ship.Pos.Item2 - i + 1;
                            else break;
                        ship.Pos = (x, i + 1);
                    }
                }
            return ship;
        }

        private void DeleteShip(int x, int y) {
            if (!OurMap[x, y].IsShip())
                return;

            var ship = ShipDefinition(x, y, OurMap);

            OurMap.DeleteShip(ship);
            placedShips[ship.Rank] -= 1;
            CreateFalseMap();
        }

        public int[] ourShipsKilled = new int[5];
        public int[] enemyShipsKilled = new int[5];

        internal void Destroyed(CellVM cellVM) {
            int x = cellVM.x;
            int y = cellVM.y;

            var shotMap = cellVM.name == 'O' ? OurMap : EnemyMap;

            var ship = ShipDefinition(x, y, shotMap);

            if (ship.Rank == 1)
                if (shotMap == OurMap)
                    ourShipsKilled[ship.Rank] += 1;
                else {
                    EnemyMap.ShipsView.Add(ship);
                    Notify("ShipView");
                    enemyShipsKilled[ship.Rank] += 1;
                }
            else {
                if (ship.Direction == ShipDirection.Horisont) {
                    for (int X = ship.Pos.Item1; X < ship.Pos.Item1 + ship.Rank; X++)
                        if (!(shotMap[X, y].IsShip() && shotMap[X, y].Shot == Visibility.Visible))
                            return;
                } else {
                    for (int Y = ship.Pos.Item2; Y < ship.Pos.Item2 + ship.Rank; Y++)
                        if (!(shotMap[x, Y].IsShip() && shotMap[x, Y].Shot == Visibility.Visible))
                            return;
                }
                if (shotMap == OurMap) ourShipsKilled[ship.Rank] += 1;
                else {
                    EnemyMap.ShipsView.Add(ship);
                    Notify("ShipView");
                    enemyShipsKilled[ship.Rank] += 1;
                }
            }
        }

        public int IsWin() {
            if (ourShipsKilled.Sum() == 10) return 1;
            if (enemyShipsKilled.Sum() == 10) return 2;
            return 0;
        }
    }
}

