using System.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BattleShipV2 {
    public partial class GameWindow : Window {
        static string Chars = "ABCDEFGHIJ";

        bool isEdit = false;
        bool manualMode = false;
        BattleshipVM battleship = new BattleshipVM();

        int userId;
        AppContext Db = new AppContext();

        public GameWindow(int UserId) {
            userId = UserId;

            DataContext = battleship;
            InitializeComponent();

            int[] marginsForChars = new int[10] { 47, 15, 15, 15, 15, 20, 15, 13, 18, 22 };

            for (int i = 1; i < 11; i++) {
                TextBlock tb = new TextBlock() { Text = $"{Chars[i - 1]}", Margin = new Thickness(left: marginsForChars[i - 1], 0, 0, 0) }; CharsEnemy.Children.Add(tb);
                TextBlock tb2 = new TextBlock() { Text = $"{Chars[i - 1]}", Margin = new Thickness(left: marginsForChars[i - 1], 0, 0, 0) }; CharsOur.Children.Add(tb2);

                TextBlock tb3 = new TextBlock() { Text = $"{i}", Margin = new Thickness(0, top: -2, 0, 0), HorizontalAlignment = HorizontalAlignment.Right }; NumEnemy.Children.Add(tb3);
                TextBlock tb4 = new TextBlock() { Text = $"{i}", Margin = new Thickness(0, top: -2, 0, 0), HorizontalAlignment = HorizontalAlignment.Right }; NumOur.Children.Add(tb4);
            }
        }

        private void ActivateWinBorder() {
            if (battleship.IsWin() != 0) {
                StartStop_btn.Content = "Старт";
                battleship.Stop();

                Edit_btn.IsEnabled = true;
                if (battleship.IsWin() == 2)
                    WinBrd.Visibility = Visibility.Visible;
                if (battleship.IsWin() == 1)
                    LoseBrd.Visibility = Visibility.Visible;

                Db.Statistics.Add(new Statistic(userId, battleship.IsWin() == 2 ? "Win" : "Lose", battleship.stepsInt, battleship.Time, battleship.ourShipsKilled.Sum(), battleship.enemyShipsKilled.Sum())); ;
                Db.SaveChanges();
            }
        }

        private void Cell_MouseDown(object sender, MouseButtonEventArgs e) {
            var cellVM = (sender as Border).DataContext as CellVM;

            if (battleship.isStart && !isEdit && cellVM.name != 'O') {
                if (cellVM.Shot == Visibility.Visible || cellVM.Miss == Visibility.Visible)
                    return;

                cellVM.ToShot();
                battleship.UpdateSteps();
                if (cellVM.Shot == Visibility.Visible) {
                    battleship.Destroyed(cellVM);
                    ActivateWinBorder();
                    return;
                }

                battleship.ShotToOurMap();
                ActivateWinBorder();
                return;
            }
            if (!battleship.isStart && isEdit && manualMode && cellVM.name != 'E') {
                if ((battleship.OurMap.Ships.Count < 10 && e.LeftButton == MouseButtonState.Pressed) || (e.RightButton == MouseButtonState.Pressed && battleship.OurMap.Ships.Count > 0) || e.MiddleButton == MouseButtonState.Pressed)
                    battleship.EditOurMap(sender, e);
                if (battleship.OurMap.Ships.Count == 10)
                    Edit_btn.IsEnabled = true;
            }
        }

        private void Exit_Buttno_Click(object sender, RoutedEventArgs e) {
            AuthWindow auth = new AuthWindow();
            auth.Show();
            Close();
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e) {
            if (battleship.isStart) {
                StartStop_btn.Content = "Старт";
                battleship.Stop();
                SelectBot.IsEnabled = true;
                Edit_btn.IsEnabled = true;
                return;
            }
            if (!battleship.isStart && battleship.OurMap.ShipsView.Count != 0) {
                StartStop_btn.Content = "Стоп";
                battleship.Start();
                SelectBot.IsEnabled = false;
                Edit_btn.IsEnabled = false;
                EditMenu_SP.Visibility = Visibility.Hidden;
                return;
            }
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e) {
            if ((Button)sender == Edit_btn) {
                if (isEdit)
                    if (battleship.OurMap.Ships.Count == 10) {
                        isEdit = false;
                        manualMode = false;
                        Edit_btn.Content = "Изменить карту";
                        EditMenu_SP.Visibility = Visibility.Hidden;
                        StartStop_btn.IsEnabled = true;
                        return;
                    }
                if (!isEdit) {
                    isEdit = true;
                    battleship.ClearOurMap();
                    EditMenu_SP.Visibility = Visibility.Visible;
                    StartStop_btn.IsEnabled = false;
                    Edit_btn.Content = "Готово";
                    return;
                }
            }
            if (isEdit && (Button)sender == RandomPos_btn) {
                battleship.DeleteOurMap();
                battleship.OurMap.FillMap(0, 4, 3, 2, 1);
                Edit_btn.IsEnabled = true;
                return;
            }
            if (isEdit && (Button)sender == ManualMode_btn) {
                if (!manualMode) {
                    manualMode = true;
                    ManualMode_btn.Content = "Готово";
                    ManualMode_SP.Visibility = Visibility.Visible;
                    battleship.CreateFalseMap();

                    if (battleship.OurMap.Ships.Count != 10)
                        Edit_btn.IsEnabled = false;
                    else Edit_btn.IsEnabled = true;

                    return;
                }
                if (manualMode) {
                    manualMode = false;
                    ManualMode_btn.Content = "Ручной режим";
                    ManualMode_SP.Visibility = Visibility.Hidden;

                    if (battleship.OurMap.Ships.Count != 10)
                        Edit_btn.IsEnabled = false;
                    else Edit_btn.IsEnabled = true;

                    return;
                }
            }
            if (isEdit && (Button)sender == ClearMap_btn) {
                battleship.DeleteOurMap();
                battleship.CreateFalseMap();
                Edit_btn.IsEnabled = false;
                battleship.DelNavy();
            }
        }

        private void EditShipRank_btn(object sender, RoutedEventArgs e) => battleship.EditSelectedShipRank((Button)sender);

        private void MouseDown_WinLoseBorder(object sender, MouseButtonEventArgs e) {
            var brd = (Border)sender;
            brd.Visibility = Visibility.Hidden;
        }

        Statistics statistic = null;

        private void Statistics_btn(object sender, RoutedEventArgs e) {
            statistic = new Statistics(userId);
            statistic.ShowDialog();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e) {
            battleship.ChangeBot();
        }
    }
}
