using System;

namespace BattleShipV2 {
    internal class EnemyBot {
        enum Direction { Horisont, Vertical }
        enum AttackDirection { Up, Down, Left, Right }

        private int[] countDestrShips;
        private Direction? dirAttackShip;
        private AttackDirection? dirAttackToShip;
        private bool?[,] attackedCells;
        private bool? lastShotBool;
        private Random rnd = new Random();
        private (int x, int y) lastShotXY = (100, 100);
        private Direction? dirAttack;
        private (int x, int y) lastShotForLine;
        private bool attackShip;
        private (int, int) lastShotOnShip = (100, 100);

        public EnemyBot() {
            countDestrShips = new int[5];
            attackedCells = new bool?[10, 10];
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                    attackedCells[x, y] = null;
            dirAttackShip = null;
            dirAttackToShip = null;
            lastShotBool = null;
            lastShotXY = (100, 100);
            lastShotForLine = (100, 100);
            lastShotOnShip = (100, 100);
        }

        private (int x, int y) StartLine() {
            int i = 0;
            dirAttack = rnd.Next(0, 2) == 1 ? Direction.Vertical : Direction.Horisont;
            for (; i < 9; i++)
                if (CanShotForLine(dirAttack, i))
                    break;
            do
                lastShotXY = dirAttack == Direction.Horisont ? (i, rnd.Next(0, 10)) : (rnd.Next(0, 10), i);
            while (attackedCells[lastShotXY.x, lastShotXY.y] != null);
            lastShotForLine = lastShotXY;
            return (lastShotXY.x, lastShotXY.y);
        }

        private bool CanShotForLine(Direction? dir, int layer) {
            if (dir == Direction.Horisont)
                for (int i = 0; i < 10; i++)
                    if (attackedCells[layer, i] == null)
                        return true;
            if (dir == Direction.Vertical)
                for (int i = 0; i < 10; i++)
                    if (attackedCells[i, layer] == null)
                        return true;
            return false;
        }

        public (int x, int y) GetShot() {
            if (attackShip) {
                lastShotXY = AttackShip();
                return lastShotXY;
            } else return AttackLine();
        }

        private (int, int) AttackLine() {
            if (dirAttack != null) {
                if (dirAttack == Direction.Horisont) {
                    for (int i = lastShotForLine.x; i < 10; i++)
                        if (attackedCells[i, lastShotForLine.y] == null) {
                            lastShotXY = (i, lastShotForLine.y);
                            lastShotForLine = lastShotXY;
                            return lastShotForLine;
                        }
                } else {
                    for (int i = lastShotForLine.y; i < 10; i++)
                        if (attackedCells[lastShotForLine.x, i] == null) {
                            lastShotXY = (lastShotForLine.x, i);
                            lastShotForLine = lastShotXY;
                            return lastShotForLine;
                        }
                }
            }
            return StartLine();
        }

        private (int, int) AttackShip() {
            (int lastX, int lastY) = lastShotOnShip;
            if (dirAttackShip == null) {
                if (lastX != 9 && attackedCells[lastX + 1, lastY] == null)
                    return (lastX + 1, lastY);
                if (lastX != 0 && attackedCells[lastX - 1, lastY] == null)
                    return (lastX - 1, lastY);
                if (lastY != 0 && attackedCells[lastX, lastY - 1] == null)
                    return (lastX, lastY - 1);
                if (lastY != 9 && attackedCells[lastX, lastY + 1] == null)
                    return (lastX, lastY + 1);
            } else {
                if (dirAttackShip == Direction.Horisont) {
                    if (dirAttackToShip == AttackDirection.Right) {
                        if (lastX != 9) {
                            for (int x = lastX; x < 10; x++)
                                if (attackedCells[x, lastY] == null && (x != 0 && attackedCells[x - 1, lastY] == true)) {
                                    lastShotXY = (lastX + 1, lastY);
                                    return lastShotXY;
                                }
                            dirAttackToShip = AttackDirection.Left;
                        }
                    }
                    if (dirAttackToShip == AttackDirection.Left) {
                        for (int x = lastX; x > -1; x--)
                            if (attackedCells[x, lastY] == null) {
                                lastShotXY = (x, lastY);
                                return lastShotXY;
                            }
                    }
                } else {
                    if (dirAttackToShip == AttackDirection.Down) {
                        if (lastY != 9) {
                            for (int y = lastY; y < 10; y++)
                                if (attackedCells[lastX, y] == null && (y != 0 && attackedCells[lastX, y - 1] == true)) {
                                    lastShotXY = (lastX, lastY + 1);
                                    return lastShotXY;
                                }
                            dirAttackToShip = AttackDirection.Up;
                        }
                    }
                    if (dirAttackToShip == AttackDirection.Up) {
                        for (int y = lastY; y > -1; y--)
                            if (attackedCells[lastX, lastY - y] == null) {
                                lastShotXY = (lastX, lastY - y);
                                return lastShotXY;
                            }
                    }
                }
            }
            return AttackLine();
        }

        public void IsShot(bool shot) {
            attackedCells[lastShotXY.x, lastShotXY.y] = shot;
            if (shot && dirAttackShip == null) {
                (int x, int y) = lastShotOnShip.Item1 == 100 ? lastShotXY : lastShotOnShip;
                if ((y != 0 && attackedCells[x, y - 1] == true) || (y != 9 && attackedCells[x, y + 1] == true))
                    dirAttackShip = Direction.Vertical;
                if ((x != 0 && attackedCells[x - 1, y] == true) || (x != 9 && attackedCells[x + 1, y] == true))
                    dirAttackShip = Direction.Horisont;
                dirAttackToShip = dirAttackShip == Direction.Horisont ? AttackDirection.Right : AttackDirection.Up;
                dirAttackToShip = dirAttackShip == null ? null : dirAttackToShip;
            }
            if (lastShotBool == true && !shot && attackShip)
                if (dirAttackShip == Direction.Horisont)
                    if (dirAttackToShip == AttackDirection.Right)
                        dirAttackToShip = AttackDirection.Left;
                    else dirAttackToShip = AttackDirection.Right;
                else {
                    if (dirAttackToShip == AttackDirection.Up)
                        dirAttackToShip = AttackDirection.Down;
                    else dirAttackToShip = AttackDirection.Up;
                }
            lastShotBool = shot;
            if (lastShotBool == true) {
                attackShip = true;
                lastShotOnShip = lastShotXY;
            }
        }

        public void ShipIsDead(bool shipDead, int attackedShipRank = 0) {
            if (shipDead) {
                countDestrShips[attackedShipRank] += 1;

                (int lastX, int lastY) = lastShotXY;
                int X, Y, rangeX, rangeY, shipX = lastX, shipY = lastY;

                if (dirAttackShip == Direction.Horisont) {
                    if (lastX == 0)
                        shipX = lastX;
                    else
                        for (int x = lastX; x > 0; x--)
                            if (attackedCells[x, lastY] == true)
                                shipX = x;
                            else break;
                } else {
                    if (lastY == 0)
                        shipY = lastY;
                    else
                        for (int y = lastY; y > 0; y--)
                            if (attackedCells[lastX, y] == true)
                                shipY = y;
                            else break;
                }

                X = shipX != 0 ? shipX - 1 : shipX;
                Y = shipY != 0 ? shipY - 1 : shipY;

                if (dirAttackShip == Direction.Horisont) {
                    rangeX = X + attackedShipRank + 2; rangeY = shipY == 0 ? Y + 2 : Y + 3;
                } else {
                    rangeX = shipX == 0 ? X + 2 : X + 3; rangeY = Y + attackedShipRank + 2;
                }

                for (int x = X; x < rangeX && x < 10; x++)
                    for (int y = Y; y < rangeY && y < 10; y++)
                        attackedCells[x, y] = false;

                if (dirAttackShip == Direction.Horisont)
                    for (int x = shipX; x < shipX + attackedShipRank; x++)
                        attackedCells[x, shipY] = true;
                else
                    for (int y = shipY; y < shipY + attackedShipRank; y++)
                        attackedCells[shipX, y] = true;

                attackShip = false;
                lastShotOnShip = (100, 100);
                dirAttackShip = null;
                dirAttackToShip = null;
            }
        }
    }
}