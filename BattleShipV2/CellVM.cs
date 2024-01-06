using System.Windows;
using System;

namespace BattleShipV2 {
    public class CellVM : ViewModelBase {
        public int x, y;
        public char name;

        static Random rnd = new Random();
        public int Angle { get; } = rnd.Next(-5, 5);
        public int AngleX { get; } = rnd.Next(-5, 5);
        public int AngleY { get; } = rnd.Next(-5, 5);

        public float ScaleX { get; } = 1 + rnd.Next(-15, 3) / 100.0f;
        public float ScaleY { get; } = 1 + rnd.Next(-15, 3) / 100.0f;
        public float ShiftX { get; } = 1 + rnd.Next(-20, 20) / 10.0f;
        public float ShiftY { get; } = 1 + rnd.Next(-20, 20) / 10.0f;

        bool ship, shot;

        public CellVM(char state = '*') => ship = state == 'X';
        public CellVM(int x, int y, char name) {
            this.x = x; this.y = y;
            this.name = name;
        }
        public Visibility Miss => shot && !ship ? Visibility.Visible : Visibility.Collapsed;
        public Visibility Shot => shot && ship ? Visibility.Visible : Visibility.Collapsed;
        public void ToShot() {
            shot = true;
            Notify("Miss", "Shot");
        }

        public void ClearShot() {
            shot = false;
            Notify("Miss", "Shot");
        }

        public void DelShip() => ship = false;
        public void ToShip() => ship = true;
        public bool IsShip() { return ship; }
    }
}