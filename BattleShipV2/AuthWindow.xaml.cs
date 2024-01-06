using System;
using System.Linq;
using System.Windows;

namespace BattleShipV2 {
    public partial class AuthWindow : Window {
        public AuthWindow() {
            InitializeComponent();
        }

        private void Button_AuthClick(object sender, RoutedEventArgs e) {
            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password.Trim();

            User authUser = null;
            using (AppContext Db = new AppContext()) {
                authUser = Db.Users.Where(user => user.Login == login && user.Password == password).FirstOrDefault();
            }

            if (authUser != null) {
                GameWindow window = new GameWindow(authUser.id);
                window.Show();
                Close();
            } else MessageBox.Show("Такого пользователя не существует");
        }

        private void Button_Widow_RegClick(object sender, RoutedEventArgs e) {
            RegWindow window = new RegWindow();
            window.Show();
            Close();
        }
    }
}
