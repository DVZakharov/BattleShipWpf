using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BattleShipV2 {
    public partial class RegWindow : Window {
        AppContext Db;

        public RegWindow() {
            InitializeComponent();

            Db = new AppContext();
        }

        private void Button_RegClick(object sender, RoutedEventArgs e) {
            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password.Trim();
            string passwordRepeat = PbPasswordRepeat.Password.Trim();
            string email = TbEmail.Text.Trim();

            bool l, p, rp, em;


            if (login.Length < 5) {
                TbLogin.ToolTip = "Это поле введено не корректно!";
                TbLogin.Background = Brushes.Red;
                l = false; ;
            } else {
                TbLogin.ToolTip = "";
                TbLogin.Background = Brushes.Transparent;
                l = true;
            }
            if (password.Length < 5) {
                PbPassword.ToolTip = "Это поле введено не корректно!";
                PbPassword.Background = Brushes.Red;
                p = false;
            } else {
                PbPassword.ToolTip = "";
                PbPassword.Background = Brushes.Transparent;
                p = true;
            }
            if (passwordRepeat != password || !p) {
                PbPasswordRepeat.ToolTip = "Это поле введено не корректно!";
                PbPasswordRepeat.Background = Brushes.Red;
                rp = false;
            } else {
                PbPasswordRepeat.ToolTip = "";
                PbPasswordRepeat.Background = Brushes.Transparent;
                rp = true;
            }
            if (email.Length < 5 || !email.Contains('@') || !email.Contains('.')) {
                TbEmail.ToolTip = "Это поле введено не корректно!";
                TbEmail.Background = Brushes.Red;
                em = false;
            } else {
                TbEmail.ToolTip = "";
                TbEmail.Background = Brushes.Transparent;
                em = true;
            }
            if (l && p && rp && em) {
                var userReg = new User(login, email, password);
                var userFind = Db.Users.Where(user => user.Login == login).FirstOrDefault();
                if (userFind == null) {
                    Db.Users.Add(userReg);
                    Db.SaveChanges();
                    AuthWindow authW = new AuthWindow();
                    authW.Show();
                    Close();
                } else MessageBox.Show("Пользователь с таким именем уже существует");

            }
        }

        private void Button_Window_AuthClick(object sender, RoutedEventArgs e) {
            AuthWindow authW = new AuthWindow();
            authW.Show();
            Close();
        }
    }
}
