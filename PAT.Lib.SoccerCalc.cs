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

        public static int NUMBER_OF_DIMENSIONS = 2;

        public const int NUMBER_OF_TEAMS = 2;

        public const int NUMBER_OF_PLAYERS_PER_TEAM = 10;

        public static int TOTAL_NUMBER_OF_PLAYERS = NUMBER_OF_TEAMS * NUMBER_OF_PLAYERS_PER_TEAM;

        public const int NUMBER_OF_ACTIONS = 7;
        public static int DODGE = 0;
        public static int TACKLE = 1;
        public static int DRIBBLE = 2;
        public static int SHOOT = 3;
        public static int OUT_OF_BOUNDS = 4;
        public static int SCORE = 5;
        public static int PASS = 6;
        
        // public static int[,,] originalPlayerPosition = new int[2, 10, 2] { { { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 }, { 3, 3 } },
        //     { { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 }, { 3, 5 } } };

        public static int[] originalPlayerPosition = new int[40] { 3, 3, 1, 3, 2, 3, 4, 3, 5, 3, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2, 3, 5, 1, 5, 2, 5, 4, 5, 5, 5, 1, 6, 2, 6, 3, 6, 4, 6, 5, 6 };
        // private static int tacklePlayer = -1;

        ///////////////////
        // Probabilities //
        ///////////////////

        public static int[,,] playerProbabilities = new int[NUMBER_OF_TEAMS, NUMBER_OF_PLAYERS_PER_TEAM, NUMBER_OF_ACTIONS] {
            { // Team 0
                { // Player 0 - striker
                    10, // DodgeProb
                    1, // TackleProb
                    10, // DribbleProb
                    6, // ShootProb
                    1, // OutOfBoundsProb
                    5, // ScoreProb
                    1 // PassProb
                },
                { // Player 1 - striker
                    10, // DodgeProb
                    1, // TackleProb
                    10, // DribbleProb
                    6, // ShootProb
                    1, // OutOfBoundsProb
                    5, // ScoreProb
                    1 // PassProb
                },
                { // Player 2 - striker
                    10, // DodgeProb
                    1, // TackleProb
                    10, // DribbleProb
                    6, // ShootProb
                    1, // OutOfBoundsProb
                    5, // ScoreProb
                    1 // PassProb
                },
                { // Player 3 - midfielder
                    5, // DodgeProb
                    5, // TackleProb
                    8, // DribbleProb
                    4, // ShootProb
                    2, // OutOfBoundsProb
                    2, // ScoreProb
                    2 // PassProb
                },
                { // Player 4 - midfielder
                    5, // DodgeProb
                    5, // TackleProb
                    8, // DribbleProb
                    4, // ShootProb
                    2, // OutOfBoundsProb
                    2, // ScoreProb
                    2 // PassProb
                },
                { // Player 5 - midfielder
                    5, // DodgeProb
                    5, // TackleProb
                    8, // DribbleProb
                    4, // ShootProb
                    2, // OutOfBoundsProb
                    2, // ScoreProb
                    2 // PassProb
                },
                { // Player 6 - defender
                    1, // DodgeProb
                    10, // TackleProb
                    6, // DribbleProb
                    2, // ShootProb
                    3, // OutOfBoundsProb
                    1, // ScoreProb
                    3 // PassProb
                },
                { // Player 7 - defender
                    1, // DodgeProb
                    10, // TackleProb
                    6, // DribbleProb
                    2, // ShootProb
                    3, // OutOfBoundsProb
                    1, // ScoreProb
                    3 // PassProb
                },
                { // Player 8 - defender
                    1, // DodgeProb
                    10, // TackleProb
                    6, // DribbleProb
                    2, // ShootProb
                    3, // OutOfBoundsProb
                    1, // ScoreProb
                    3 // PassProb
                },
                { // Player 9 - defender
                    1, // DodgeProb
                    10, // TackleProb
                    6, // DribbleProb
                    2, // ShootProb
                    3, // OutOfBoundsProb
                    1, // ScoreProb
                    3 // PassProb
                }
            },
            { // Team 1
                { // Player 0 - striker
                    10, // DodgeProb
                    1, // TackleProb
                    10, // DribbleProb
                    6, // ShootProb
                    1, // OutOfBoundsProb
                    5, // ScoreProb
                    1 // PassProb
                },
                { // Player 1 - striker
                    10, // DodgeProb
                    1, // TackleProb
                    10, // DribbleProb
                    6, // ShootProb
                    1, // OutOfBoundsProb
                    5, // ScoreProb
                    1 // PassProb
                },
                { // Player 2 - striker
                    10, // DodgeProb
                    1, // TackleProb
                    10, // DribbleProb
                    6, // ShootProb
                    1, // OutOfBoundsProb
                    5, // ScoreProb
                    1 // PassProb
                },
                { // Player 3 - midfielder
                    5, // DodgeProb
                    5, // TackleProb
                    8, // DribbleProb
                    4, // ShootProb
                    2, // OutOfBoundsProb
                    2, // ScoreProb
                    2 // PassProb
                },
                { // Player 4 - midfielder
                    5, // DodgeProb
                    5, // TackleProb
                    8, // DribbleProb
                    4, // ShootProb
                    2, // OutOfBoundsProb
                    2, // ScoreProb
                    2 // PassProb
                },
                { // Player 5 - midfielder
                    5, // DodgeProb
                    5, // TackleProb
                    8, // DribbleProb
                    4, // ShootProb
                    2, // OutOfBoundsProb
                    2, // ScoreProb
                    2 // PassProb
                },
                { // Player 6 - defender
                    1, // DodgeProb
                    10, // TackleProb
                    6, // DribbleProb
                    2, // ShootProb
                    3, // OutOfBoundsProb
                    1, // ScoreProb
                    3 // PassProb
                },
                { // Player 7 - defender
                    1, // DodgeProb
                    10, // TackleProb
                    6, // DribbleProb
                    2, // ShootProb
                    3, // OutOfBoundsProb
                    1, // ScoreProb
                    3 // PassProb
                },
                { // Player 8 - defender
                    1, // DodgeProb
                    10, // TackleProb
                    6, // DribbleProb
                    2, // ShootProb
                    3, // OutOfBoundsProb
                    1, // ScoreProb
                    3 // PassProb
                },
                { // Player 9 - defender
                    1, // DodgeProb
                    10, // TackleProb
                    6, // DribbleProb
                    2, // ShootProb
                    3, // OutOfBoundsProb
                    1, // ScoreProb
                    3 // PassProb
                }
            }
        };

        // Set functions
        public static int[] setPlayerPosition(int[] playerPositions, int team, int player, int row, int col)
        {
            int position = team * TOTAL_NUMBER_OF_PLAYERS + player * 2;
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
                    } else {
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

        public static int getDodgeProb(int team, int player)
        {
            return playerProbabilities[team, player, DODGE];
        }

        public static int getTackleProb(int[] playerPositions, int[] ballPosition, int possession)
        {
            // If the ball does not belong to either team, tackling makes no sense
            if (possession == -1) {
                return 0;
            }

            // We are only interested in the defending team
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

                int ratio = playerProbabilities[team, player, TACKLE];
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
            // If the ball does not belong to either team, tackling makes no sense
            if (possession == -1) {
                return 0;
            }

            // We are only interested in the defending team
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

                int ratio = playerProbabilities[team, player, TACKLE];
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

        public static int getDribbleProb(int team, int player)
        {
            return playerProbabilities[team, player, DRIBBLE];
        }

        public static int getShootProb(int team, int player, int[] ballPosition) {
            
            int target = team == 0 ? 8 : 0;
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

            int shootProb = playerProbabilities[team, player, SHOOT] / row_diff - (col_diff - 1);
            if (shootProb < 0) {
                return shootProb * -1;
            } else {
                return shootProb;
            }
        }

        public static int getScoreProb(int team, int player) 
        {
            return playerProbabilities[team, player, SCORE];
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

        public static int getPassProb(int team, int player) 
        {
            return playerProbabilities[team, player, PASS];
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

        public static int getOutOfBoundProb(int team, int player) {
            return playerProbabilities[team, player, OUT_OF_BOUNDS];
        }

        public static int[] setOutOfBoundThenThrowIn(int[] ballPosition) {
            int currBallRow = ballPosition[0];
            if (currBallRow < 3) {
                ballPosition[0] = 1; // skip throw in action
            } else if (currBallRow > 3) {
                ballPosition[0] = 5; 
            } else {
                int chanceToGoUpOrDown = rand.Next(0,1);
                if (chanceToGoUpOrDown == 1) {
                     ballPosition[0] = 1;
                } else {
                    ballPosition[0] = 5; 
                }
            }
            return ballPosition;
        }
    }
}
