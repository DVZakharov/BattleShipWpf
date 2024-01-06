using BattleShipV2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BattleShipTests {
    [TestClass]
    public class BattleShipTests {

        [TestMethod]
        public void Auth_User() {
            BattleShipV2.AppContext Db = new BattleShipV2.AppContext();
            string login = "admin";
            string password = "789987";
            User authUser = Db.Users.Where(user => user.Login == login && user.Password == password).FirstOrDefault();

            Assert.AreNotEqual(authUser, null);
        }

        [TestMethod]
        public void Reg_User() {
            BattleShipV2.AppContext Db = new BattleShipV2.AppContext();
            Random rnd = new Random();            
            string email = "testemail@test.test";
            string login, password;
            login = "admintest" + rnd.Next(1000).ToString();
            password = rnd.Next(99999, 10000001).ToString();            
            var userReg = new User(login, email, password);            

            Db.Users.Add(userReg);
            Db.SaveChanges();

            User authUser = Db.Users.Where(user => user.Login == login && user.Password == password).FirstOrDefault();
            Assert.AreNotEqual(authUser, null);
        }

        [TestMethod]
        public void AttackMap_MapAndXY_MissOrShot() {
            Random rnd = new Random();
            MapVM map = new MapVM('O');
            map.FillMap();
            
            int x = rnd.Next(0,10);
            int y = rnd.Next(0, 10);

            map[x, y].ToShot();

            Assert.AreEqual(map[x, y].Miss == Visibility.Visible || map[x, y].Miss == Visibility.Collapsed, true);
        }

        [TestMethod]
        public void ViewStatistic_User_StatWindow() {
            BattleShipV2.AppContext Db = new BattleShipV2.AppContext();
            List<Statistic> allStat = Db.Statistics.ToList();
            Assert.IsNotNull(allStat);
        }
    }
}
