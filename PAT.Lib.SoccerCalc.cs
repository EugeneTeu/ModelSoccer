using System;
using System.Collections.Generic;
using System.Text;
//the namespace must be PAT.Lib, the class and method names can be arbitrary
namespace PAT.Lib
{
    /// <summary>
    /// The math library that can be used in your model.
    /// all methods should be declared as public static.
    /// 
    /// The parameters must be of type "int", or "int array"
    /// The number of parameters can be 0 or many
    /// 
    /// The return type can be bool, int or int[] only.
    /// 
    /// The method name will be used directly in your model.
    /// e.g. call(max, 10, 2), call(dominate, 3, 2), call(amax, [1,3,5]),
    /// 
    /// Note: method names are case sensetive
    /// </summary>
    public class SoccerCalc
    {
        // for book keeping
        private static Random rand = new Random();

        private static int possession = -1;

        // Ball info
        private static int ballPlayer = -1;

        private static int[] ballPosition = new int[2] { 3, 4 };

        // Player positions
        private static int[,,] playerPosition = new int[2, 10, 2] { { { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 } },
            { { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 } } };

        private static int[,] goaliePosition = new int[2, 2] { { 3, 1 }, { 3, 7 } };

        private static int tacklePlayer = -1;

        ///////////////////
        // Probabilities //
        ///////////////////
        
        // Tackle stuff
        private static int[,] playerDodgeTackleProb = new int[10, 2] { { 10, 1 }, { 10, 1 }, { 10, 1 }, { 5, 5 }, { 5, 5 }, { 5, 5 }, { 5, 5 }, { 1, 10 }, { 1, 10 }, { 1, 10 } };

        // Dribble stuff
        private static int[] playerDribbleProb = new int[10] { 10, 10, 10, 9, 9, 9, 9, 9, 8, 8};

        // Shoot stuff
        // private static int[] playerShootProb = new int[10] { 5, 5, 5, 4, 4, 4, 4, 3, 3, 3 };

        // Pass stuff
        // private static int[,] playerPassOrKeepProb = new int[10, 2] { { 10, 1 }, { 9, 2 }, { 8, 3 }, { 7, 4 }, { 6, 5 }, { 5, 6 }, { 4, 7 }, { 3, 8 }, { 2, 9 }, { 1, 10 } };

        // Set functions
        public static int setPlayerPosition(int team, int player, int row, int col)
        {
            if (1 <= row && row <= 5 && 1 <= col && col <= 7 && 0 <= team && team <= 1 && 0 <= player && player <= 9)
            {
                playerPosition[team, player, 0] = row;
                playerPosition[team, player, 1] = col;
                return 1;
            }
            return 1;
        }

        public static int setGoaliePosition(int team, int row, int col)
        {
            if ((1 == col || col == 7) && (2 <= row && row <= 4))
            {
                goaliePosition[team, 0] = row;
                goaliePosition[team, 1] = col;
                return 1;
            }
            return 1;
        }

        public static int setBallPosition(int row, int col)
        {
            if ((0 <= row && row <= 6) && (0 <= col && col <= 8))
            {
                ballPosition[0] = row;
                ballPosition[1] = col;
                return 1;
            }
            return 1;
        }

        public static int setPossession(int pos)
        {
            if (pos == -1 || pos == 1 || pos == 0)
            {
                possession = pos;
                return -1;
            }
            return 1;
        }

        public static int setBallPlayer(int player)
        {
            if (0 <= player && player <= 10) // ball can be with the goalie too
            {
                ballPlayer = player;
                return -1;
            }
            return 1;
        }

        ////////////////////////
        // Movement functions //
        ////////////////////////
        
        public static int move()
        {
            if (possession != -1)
            {
                strategicMove();
            }
            else
            {
                runToBall();
            }
            moveGoalie();
            return 1;
        }

        public static int dribble()
        {
            int playerRow = playerPosition[possession, ballPlayer, 0];
            if (playerRow < 3)
            {
                playerPosition[possession, ballPlayer, 0] += 1;
            }
            else if (playerRow > 3)
            {
                playerPosition[possession, ballPlayer, 0] -= 1;
            }

            int multiplier = 1;
            if (possession == 0)
            {
                multiplier = -1;
            }
            playerPosition[possession, ballPlayer, 1] -= (1 * multiplier);
            if (playerPosition[possession, ballPlayer, 1] < 1)
            {
                playerPosition[possession, ballPlayer, 1] = 1;
            }
            else if (playerPosition[possession, ballPlayer, 1] > 7)
            {
                playerPosition[possession, ballPlayer, 1] = 7;
            }

            ballPosition[0] = playerPosition[possession, ballPlayer, 0];
            ballPosition[1] = playerPosition[possession, ballPlayer, 1];

            return 1;
        }

        private static void strategicMove()
        {
            for (int team = 0; team < playerPosition.GetLength(0); team += 1)
            {
                for (int player = 0; player < playerPosition.GetLength(1); player += 1)
                {
                    if (possession == team && player == ballPlayer)
                    {
                        continue;
                    }
                    int playerRow = playerPosition[team, player, 0];
                    if (playerRow < 3)
                    {
                        playerPosition[team, player, 0] += 1;
                    }
                    else if (playerRow > 3)
                    {
                        playerPosition[team, player, 0] -= 1;
                    }

                    int multiplier = 1;
                    if (possession == 0)
                    {
                        multiplier = -1;
                    }

                    int chance = rand.Next(0, 3);
                    // 33 % to move diagonally towards goal
                    if (chance == 2)
                    {
                        playerPosition[team, player, 1] -= (1 * multiplier);
                        if (playerPosition[team, player, 1] < 1)
                        {
                            playerPosition[team, player, 1] = 1;
                        }
                        else if (playerPosition[team, player, 1] > 7)
                        {
                            playerPosition[team, player, 1] = 7;
                        }
                    }
                }
            }
        }

        private static void runToBall()
        {
            for (int team = 0; team < playerPosition.GetLength(0); team += 1)
            {
                for (int player = 0; player < playerPosition.GetLength(1); player += 1)
                {
                    int playerRow = playerPosition[team, player, 0];
                    int playerCol = playerPosition[team, player, 1];
                    int ballRow = ballPosition[0];
                    int ballCol = ballPosition[1];
                    if (playerRow < ballRow)
                    {
                        playerPosition[team, player, 0] += 1;
                    }
                    else if (playerRow > ballRow)
                    {
                        playerPosition[team, player, 0] -= 1;
                    }
                    if (playerCol < ballCol)
                    {
                        playerPosition[team, player, 1] += 1;
                    }
                    else if (playerCol > ballCol)
                    {
                        playerPosition[team, player, 1] -= 1;
                    }
                }
            }
        }

        // to complete
        private static void moveGoalie()
        { 
        
        }

        ///////////////////////////
        // Probability functions //
        ///////////////////////////
        
        private static int getPlayerTackleDodge(int player, int action)
        {
            return playerDodgeTackleProb[player, action];
        }

        // public static double getTackleProb()
        // {
        //     int team = 0;
        //     if (possession == 0)
        //     {
        //         team = 1;
        //     }
        //     int tackler = -1;
        //     double ratio = 0;
        //     for (int player = 0; player < playerPosition.GetLength(1); player += 1)
        //     {
        //         if (playerPosition[team, player, 0] != ballPosition[0] || playerPosition[team, player, 1] != ballPosition[1])
        //         {
        //             continue;
        //         }
        //         double calced = (double)getPlayerTackleDodge(player, 1) / (double)getPlayerTackleDodge(ballPlayer, 0);
        //         if (calced > ratio)
        //         {
        //             ratio = calced;
        //             tackler = player;
        //         }
        //     }
        //     if (tackler == -1)
        //     {
        //         return 0;
        //     }
        //     tacklePlayer = tackler;
        //     return ratio;
        // }

        public static int updateTackle()
        {
            if (possession == 0)
            {
                possession = 1;
            }
            else
            {
                possession = 0;
            }
            ballPlayer = tacklePlayer;
            return 1;
        }

        public static int getDribbleProb()
        {
            return playerDribbleProb[ballPlayer];
        }

        // public static double getShootProb()
        // {
        //     double[] target;
        //     if (possession == 0)
        //     {
        //         target = { 3, 8 };
        //     }
        //     else
        //     {
        //         target = { 3, 0 };
        //     }
        //     double row_diff = target[0] - ballPosition[0];
        //     double col_diff = target[1] - ballPosition[1];
        //     double distance = Math.Sqrt(row_diff * row_diff + col_diff * col_diff);
        //     if (distance > 3)
        //     {
        //         return 0;
        //     }
        //     return playerShootProb[ballPlayer] / distance;
        // }

        // //todo
        // public static double getShootSuccessProb()
        // { 
        
        // }

        // //todo
        // public static double getPassProb()
        // { 
        
        // }

        // //todo
        // public static double getPassSuccessProb()
        // { 
        
        // }

        ////////////////////////////
        // Non-movement functions //
        ////////////////////////////

        // can change to non deterministic if needed
        public static int getBallDeterministic(int team)
        {
            if (possession == -1)
            {
                int ballRow = ballPosition[0];
                int ballCol = ballPosition[1];
                for (int player = 0; player < playerPosition.GetLength(1); player += 1)
                {
                    if (ballRow == playerPosition[team, player, 0] && ballCol == playerPosition[team, player, 1])
                    {
                        possession = team;
                        ballPlayer = player;
                        return 1;
                    }
                }
            }
            return 1;
        }
    }
}
