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
        public static Random rand = new Random();

        public const int NUMBER_OF_TEAMS = 2;

        public const int NUMBER_OF_PLAYERS_PER_TEAM = 10;

        public const int TOTAL_NUMBER_OF_PLAYERS = NUMBER_OF_TEAMS * NUMBER_OF_PLAYERS_PER_TEAM;
        
        // public static int[,,] originalPlayerPosition = new int[2, 10, 2] { { { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 } },
        //     { { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 } } };

        public static int[] originalPlayerPosition = new int[40] { 3, 3, 1, 3, 2, 3, 4, 3, 5, 3, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2, 3, 5, 1, 5, 2, 5, 4, 5, 5, 5, 1, 6, 2, 6, 3, 6, 4, 6, 5, 6 };
        // private static int tacklePlayer = -1;

        ///////////////////
        // Probabilities //
        ///////////////////
        
        // Tackle stuff
        public static int[,] playerDodgeTackleProb = new int[10, 2] { { 10, 1 }, { 10, 1 }, { 10, 1 }, { 5, 5 }, { 5, 5 }, { 5, 5 }, { 5, 5 }, { 1, 10 }, { 1, 10 }, { 1, 10 } };

        // Dribble stuff
        public static int[] playerDribbleProb = new int[10] { 10, 10, 10, 9, 9, 9, 9, 9, 8, 8};

        // Shoot stuff
        public static int[] playerShootProb = new int[10] { 6, 6, 6, 4, 4, 4, 4, 3, 3, 3 };

        public static int[] playerScoreProb = new int[10] { 4, 3, 3, 2, 2, 2, 2, 1, 1, 1};

        // Pass stuff
        public static int[] playerPassProb = new int[10] { 1, 1, 1, 2, 2, 2, 2, 3, 3, 3 };

        // Set functions
        public static int[] setPlayerPosition(int[] playerPositions, int team, int player, int row, int col)
        {
            int position = team * 20 + player;
            playerPositions[position] = row;
            playerPositions[position + 1] = col;
            return playerPositions;
        }

        ////////////////////////
        // Movement functions //
        ////////////////////////
        
        public static int[] move(int[] playerPositions, int possession, int[] ballPosition, int ballPlayer)
        {
             if (possession == 7) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("Paaaaaaa " + possession + "  " );
            }
            if (possession != -1)
            {
                return strategicMove(playerPositions, possession, ballPlayer);
            }
            else
            {
                return runToBall(playerPositions, ballPosition);
            }
        }

        public static int[] strategicMove(int[] playerPositions, int possession, int ballPlayer)
        {
             if (possession == 7) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("movvvvv" + possession + "  ");
            }
            for (int team = 0; team < NUMBER_OF_TEAMS; team += 1)
            {
                for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player += 1)
                {   
                    // Player who carries the ball does not move
                    if (possession == team && player == ballPlayer)
                    {
                        continue;
                    }
                    int playerOffsetIdxRow = team * TOTAL_NUMBER_OF_PLAYERS + player * 2;
                    int playerOffsetIdxCol = playerOffsetIdxRow + 1;
                    int chance = rand.Next(0, 3);
                    // TODO: Check deterministic or probabilistic
                    
                    // 33 % to move diagonally towards goal
                    if (chance == 2)
                    {
                        int row = playerPositions[playerOffsetIdxRow];
                        if (row < 3)
                        {
                            playerPositions[playerOffsetIdxRow] += 1;
                        }
                        else if (row > 3)
                        {
                            playerPositions[playerOffsetIdxRow] -= 1;
                        }
                    }

                    // Determine direction to attack
                    int direction = possession == 0 ? 1 : -1;
                    playerPositions[playerOffsetIdxCol] += direction;
                    // account for bounds
                    if (playerPositions[playerOffsetIdxCol] < 1)
                    {
                        playerPositions[playerOffsetIdxCol] = 1;
                    }
                    else if (playerPositions[playerOffsetIdxCol] > 7)
                    {
                        playerPositions[playerOffsetIdxCol] = 7;
                    }
                }
            }

            return playerPositions;
        }

        public static int[] runToBall(int[] playerPositions, int[] ballPosition)
        {
            for (int team = 0; team < NUMBER_OF_TEAMS; team += 1)
            {
                for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player += 1)
                {
                    int playerOffsetIdxRow = team * TOTAL_NUMBER_OF_PLAYERS + player * 2;
                    int playerOffsetIdxCol = playerOffsetIdxRow + 1;

                    int playerRow = playerPositions[playerOffsetIdxRow];
                    int playerCol = playerPositions[playerOffsetIdxCol];
                    int ballRow = ballPosition[0];
                    int ballCol = ballPosition[1];
                    if (playerRow < ballRow)
                    {
                        playerPositions[playerOffsetIdxRow] += 1;
                    }
                    else if (playerRow > ballRow)
                    {
                        playerPositions[playerOffsetIdxRow] -= 1;
                    }
                    if (playerCol < ballCol)
                    {
                        playerPositions[playerOffsetIdxCol] += 1;
                    }
                    else if (playerCol > ballCol)
                    {
                        playerPositions[playerOffsetIdxCol] -= 1;
                    }
                }
            }
            return playerPositions;
        }

        public static int[] dribble(int[] playerPositions, int possession, int ballPlayer)
        {
            int playerOffsetIdxRow = (possession * TOTAL_NUMBER_OF_PLAYERS) + (ballPlayer * 2);
            int playerOffsetIdxCol = playerOffsetIdxRow + 1;
            if (possession == 7) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("Possession > 1 " + possession + "  ");
            }
            if (ballPlayer < 0) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("BallPlayer < 0");
            }

            if (playerOffsetIdxRow < 0) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("Access an empty stack!");
            }
            if (playerOffsetIdxRow > 39) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("Access an empty stack! 2 playerOffsetIdxRow: " + playerOffsetIdxRow);
            }

            int row = playerPositions[playerOffsetIdxRow];
            if (row < 3)
            {
                playerPositions[playerOffsetIdxRow] += 1;
            }
            else if (row > 3)
            {
                playerPositions[playerOffsetIdxRow] -= 1;
            }

            // Determine direction to attack
            int direction = possession == 0 ? 1 : -1;
            playerPositions[playerOffsetIdxCol] += direction;
            // account for bounds
            if (playerPositions[playerOffsetIdxCol] < 1)
            {
                playerPositions[playerOffsetIdxCol] = 1;
            }
            else if (playerPositions[playerOffsetIdxCol] > 7)
            {
                playerPositions[playerOffsetIdxCol] = 7;
            }
            return playerPositions;
        }

        public static int[] getDribbleBall(int[] playerPositions, int ballPlayer, int possession)
        {
             if (possession == 7) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("getDba " + possession + "  ");
            }
            int[] positionBall = new int[2];
            int playerOffsetIdxRow = possession * TOTAL_NUMBER_OF_PLAYERS + ballPlayer * 2;
            int playerOffsetIdxCol = playerOffsetIdxRow + 1;
            positionBall[0] = playerPositions[playerOffsetIdxRow];
            positionBall[1] = playerPositions[playerOffsetIdxCol];
            return positionBall;
        }
        // to complete
        // public static void moveGoalie()
        // { 
        
        // }

        ///////////////////////////
        // Probability functions //
        ///////////////////////////
        
        // private static int getPlayerTackleDodge(int player, int action)
        // {
        //     return playerDodgeTackleProb[player, action];
        // }

        public static int getDodge(int ballPlayer)
        {
            return playerDodgeTackleProb[ballPlayer, 0];
        }

        public static int getTackle(int[] playerPositions, int[] ballPosition, int possession)
        {
             if (possession == 7) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("tavklrs" + possession + "  ");
            }
            if (possession == -1) {
                return 0;
            }

            int team = possession == 0 ? 1 : 0;
            int best = -1;
            for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player += 1)
            {
                int playerOffsetIdxRow = team * TOTAL_NUMBER_OF_PLAYERS + player * 2;
                int playerOffsetIdxCol = playerOffsetIdxRow + 1;
                if (playerPositions[playerOffsetIdxRow] != ballPosition[0] || playerPositions[playerOffsetIdxCol] != ballPosition[1])
                {
                    continue;
                }

                int ratio = playerDodgeTackleProb[player, 1];
                if (ratio > best)
                {
                    best = ratio;
                }
            }
            if (best == -1) {
                return 0;
            }
            return best;
        }

        public static int getTacklePlayer(int[] playerPositions, int[] ballPosition, int possession)
        {
             if (possession == 7) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("getetate > 1 " + possession + "  " );
            }
            if (possession == -1) {
                return 0;
            }

            int team = possession == 0 ? 1 : 0;
            int best = -1;
            int bestPlayer = -1;
            for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player += 1)
            {
                int playerOffsetIdxRow = team * TOTAL_NUMBER_OF_PLAYERS + player * 2;
                int playerOffsetIdxCol = playerOffsetIdxRow + 1;
                if (playerPositions[playerOffsetIdxRow] != ballPosition[0] || playerPositions[playerOffsetIdxCol] != ballPosition[1])
                {
                    continue;
                }

                int ratio = playerDodgeTackleProb[player, 1];
                if (ratio > best)
                {
                    best = ratio;
                    bestPlayer = player;
                }
            }
            if (best == -1) {
                return 0;
            }
            return bestPlayer;
        }


        // public static int updateTackle()
        // {
        //     if (possession == 0)
        //     {
        //         possession = 1;
        //     }
        //     else
        //     {
        //         possession = 0;
        //     }
        //     ballPlayer = tacklePlayer;
        //     return 1;
        // }

        public static int getDribbleProb(int ballPlayer)
        {
            return playerDribbleProb[ballPlayer];
        }

        public static int getShootProb(int ballPlayer, int[] ballPosition, int possession) {
             if (possession == 7) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("Possession >asdasdadadadaads 1 " + possession + "  ");
            }
            int target;
            target = possession == 0 ? 8 : 0;
            int col_diff = -1;
            if (target == 8)
            {
                col_diff = target - ballPosition[1];
            } 
            else 
            {
                col_diff = ballPosition[1] - target;
            }

            if (col_diff > 3) 
            {
                return 0;
            }

            int row_diff = -1;
            if (ballPosition[0] >= 3) 
            {
                row_diff = ballPosition[0] - 2;
            }
            else
            {
                row_diff = 4 - ballPosition[0];
            }
            int shootProb = playerShootProb[ballPlayer] / row_diff - (col_diff - 1);
            return shootProb;
        
        }

        public static int getScoreProb(int ballPlayer) 
        {
            return playerScoreProb[ballPlayer];
        }

        public static int[] resetPlayerPosition() 
        {
            return originalPlayerPosition;
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

        public static int getPassProb(int ballPlayer) 
        {
            return playerPassProb[ballPlayer];
        }

        public static int[] setPassTarget(int possession,  int[] ballPosition) {
            // int currRow = ballPosition[0];
            int currCol = ballPosition[1];

            if (possession == 0) {
                int finalCol = 7;
                int resultantCol = rand.Next(currCol, finalCol+1);
                int resultantRow = rand.Next(1,5+1);
                return new int[2] { resultantRow, resultantCol } ;
            } else if (possession == 1) {
                int resultantCol = rand.Next(0, currCol+1);
                int resultantRow = rand.Next(1,5+1);
                return new int[2] {resultantRow, resultantCol};
            } else {
               throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException(" possession is invalid");
            }
        }

        public static int getPassTarget(int ballPlayer, int possession, int[] playerPosition) {
            // int nearestPlayer = ballPlayer;

            // for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player++) {
            //     int playerRow = playerPosition[possession * TOTAL_NUMBER_OF_PLAYERS + player * 2];
            //     int playerCol = playerPosition[possession * TOTAL_NUMBER_OF_PLAYERS + player * 2 + 1];


            // }
            if (possession == 1) {

            } else if (possession == 1) {

            }

            return (ballPlayer + 1) % 10;
        }

        // public static int getPassSuccessProb(int ballPlayer)
        // { 
        //     return playerPassSuccessPassProb[ballPlayer, 0];
        // }

        // public static int getPassFailProb(int[] playerPositions, int ballPlayer, int possession)
        // {
        //     return 
        // }

        ////////////////////////////
        // Non-movement functions //
        ////////////////////////////

        //deterministic
        public static int getBall(int[] playerPositions, int team, int possession, int[] ballPosition)
        {
             if (possession == 7) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("gegetetetetetetetetetetetetn > 1 " + possession + "  " );
            }
            if (possession == -1)
            {
                for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player += 1)
                {
                    int playerOffsetIdxRow = team * TOTAL_NUMBER_OF_PLAYERS + player * 2;
                    int playerOffsetIdxCol = playerOffsetIdxRow + 1;
                    if (ballPosition[0] == playerPositions[playerOffsetIdxRow] && ballPosition[1] == playerPositions[playerOffsetIdxCol])
                    {
                        if (team > 1) {
                            throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("Ball is saved. 2");
                        }
                        return team;
                    }
                }
                return -1;
            }

            if (possession > 1) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("Ball is saved.");
            }

            return possession;
        }


        public static int getBallPlayer(int[] playerPositions, int possession, int[] ballPosition)
        {
             if (possession == 7) {
                throw new PAT.Common.Classes.Expressions.ExpressionClass.RuntimeException("Paw awa" + possession + "  " );
            }
            if (possession != -1)
            {
                for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player += 1)
                {
                    int playerOffsetIdxRow = possession * TOTAL_NUMBER_OF_PLAYERS + player * 2;
                    int playerOffsetIdxCol = playerOffsetIdxRow + 1;
                    if (ballPosition[0] == playerPositions[playerOffsetIdxRow] && ballPosition[1] == playerPositions[playerOffsetIdxCol])
                    {
                        return player;
                    }
                }
                return -1;
            }
            return -1;
        }
    }
}
