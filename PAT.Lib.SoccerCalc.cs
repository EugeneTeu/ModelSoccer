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

        private const int NUMBER_OF_DIMENSIONS = 2;

        private const int MIN_ROW = 1;
        private const int MAX_ROW = 5;
        private const int MIN_COL = 1;
        private const int MAX_COL = 7;

        private const int NUMBER_OF_TEAMS = 2;

        private const int NUMBER_OF_PLAYERS_PER_TEAM = 10;

        private const int TOTAL_NUMBER_OF_PLAYERS = NUMBER_OF_TEAMS * NUMBER_OF_PLAYERS_PER_TEAM;

        private const int PRECISION_FACTOR = 1000000;

        private const int NUMBER_OF_OFFENSIVE_ACTIONS = 3;
        private const int NUMBER_OF_OFFENSIVE_STATS = 3;
        private const int SHOTS = 0;
        private const int GOALS = 1;
        private const int SHOTS_ON_TARGET = 2;

        private const int NUMBER_OF_PROBABILITIES = 2;

        private const int DODGE = 0;
        private const int TACKLE = 1;

        // 4-3-3
        private static int[] FORMATION_4_3_3 = new int[TOTAL_NUMBER_OF_PLAYERS * NUMBER_OF_DIMENSIONS] {
            // Team 0
            2, 4, // Player 0 - left wing striker
            3, 4, // Player 1 - centre striker
            4, 4, // Player 2 - right wing striker
            2, 3, // Player 3 - left wing midfielder
            3, 3, // Player 4 - centre midfielder
            4, 3, // Player 5 - right wing midfielder
            2, 2, // Player 6 - left wing defender
            3, 2, // Player 7 - centre defender
            3, 2, // Player 8 - centre defender
            4, 2, // Player 9 - right wing defender
            // Team 1
            4, 4, // Player 0 - left wing striker
            3, 4, // Player 1 - centre striker
            2, 4, // Player 2 - right wing striker
            4, 5, // Player 3 - left wing midfielder
            3, 5, // Player 4 - centre midfielder
            2, 5, // Player 5 - right wing midfielder
            4, 6, // Player 6 - left wing defender
            3, 6, // Player 7 - centre defender
            3, 6, // Player 8 - centre defender
            2, 6  // Player 9 - right wing defender
        };

		// 4-2-2
        private static int[] FORMATION_4_4_2 = new int[TOTAL_NUMBER_OF_PLAYERS * NUMBER_OF_DIMENSIONS] {
            // Team 0
            3, 4, // Player 0 - centre striker
            3, 4, // Player 1 - centre striker
            2, 3, // Player 2 - left wing midfielder
            3, 3, // Player 3 - centre midfielder
            3, 3, // Player 4 - centre midfielder
            4, 3, // Player 5 - right wing midfielder
            2, 2, // Player 6 - left wing defender
            3, 2, // Player 7 - centre defender
            3, 2, // Player 8 - centre defender
            4, 2, // Player 9 - right wing defender
            // Team 1
            3, 4, // Player 0 - centre striker
            3, 4, // Player 1 - centre striker
            4, 5, // Player 2 - left wing midfielder
            3, 5, // Player 3 - centre midfielder
            3, 5, // Player 4 - centre midfielder
            2, 5, // Player 5 - right wing midfielder
            4, 6, // Player 6 - left wing defender
            3, 6, // Player 7 - centre defender
            3, 6, // Player 8 - centre defender
            2, 6  // Player 9 - right wing defender
        };

        ///////////////////
        // Probabilities //
        ///////////////////

        private static int[,,] OFFENSIVE_STATS = new int[NUMBER_OF_TEAMS, NUMBER_OF_PLAYERS_PER_TEAM, NUMBER_OF_OFFENSIVE_STATS] {
            { // Team 0 - Manchester City
                { // Player 0 - Sterling
                    602, // Shots
                    95,  // Goals
                    246, // Shots on target
                },
                { // Player 1 - Jesus
                    267,
                    49,
                    132
                },
                { // Player 2 - Mahrez
                    477,
                    66,
                    205
                },
                { // Player 3 - Gündogan
                    172,
                    27,
                    54
                },
                { // Player 4 - Rodrigo
                    51,
                    4,
                    16
                },
                { // Player 5 - Silva
                    166,
                    21,
                    64
                },
                { // Player 6 - Cancelo
                    10,
                    1,
                    1
                },
                { // Player 7 - Dias
                    10,
                    1,
                    2
                },
                { // Player 8 - Stones
                    50,
                    5,
                    12
                },
                { // Player 9 - Walker
                    80,
                    8,
                    17
                }
            },
            { // Team 1 - Manchester United
                { // Player 0 - Rashford
                    366,
                    54,
                    163
                },
                { // Player 1 - Fernandes
                    139,
                    24,
                    61
                },
                { // Player 2 - Greenwood
                    93,
                    13,
                    37
                },
                { // Player 3 - Pogba
                    323,
                    28,
                    118
                },
                { // Player 4 - Fred
                    81,
                    2,
                    18
                },
                { // Player 5 - McTominay
                    66,
                    10,
                    24
                },
                { // Player 6 - Shaw
                    20,
                    2,
                    3
                },
                { // Player 7 - Maguire
                    100,
                    10,
                    20
                },
                { // Player 8 - Lindelöf
                    30,
                    3,
                    7
                },
                { // Player 9 - W-Bissaka
                    20,
                    2,
                    5
                }
            }
        };

        private static int[,,] DEFENSIVE_STATS = new int[NUMBER_OF_TEAMS, NUMBER_OF_PLAYERS_PER_TEAM, NUMBER_OF_OFFENSIVE_STATS] {
            { // Team 0 - Manchester City
                { // Player 0 - Sterling
                    50
                },
                { // Player 1 - Jesus
                    50
                },
                { // Player 2 - Mahrez
                    50
                },
                { // Player 3 - Gündogan
                    63
                },
                { // Player 4 - Rodrigo
                    60
                },
                { // Player 5 - Silva
                    63
                },
                { // Player 6 - Cancelo
                    58
                },
                { // Player 7 - Dias
                    62
                },
                { // Player 8 - Stones
                    73
                },
                { // Player 9 - Walker
                    76
                }
            },
            { // Team 1 - Manchester United
                { // Player 0 - Rashford
                    50
                },
                { // Player 1 - Fernandes
                    50
                },
                { // Player 2 - Greenwood
                    50
                },
                { // Player 3 - Pogba
                    68
                },
                { // Player 4 - Fred
                    52
                },
                { // Player 5 - McTominay
                    57
                },
                { // Player 6 - Shaw
                    72
                },
                { // Player 7 - Maguire
                    67
                },
                { // Player 8 - Lindelöf
                    59
                },
                { // Player 9 - W-Bissaka
                    64
                }
            }
        };


        public static int[,,] PLAYER_PROBABILITIES = new int[NUMBER_OF_TEAMS, NUMBER_OF_PLAYERS_PER_TEAM, NUMBER_OF_PROBABILITIES] {
            { // Team 0
                { // Player 0
                    10, // DodgeProb
                    1, // TackleProb
                },
                { // Player 1
                    10, // DodgeProb
                    1, // TackleProb
                },
                { // Player 2
                    10, // DodgeProb
                    1, // TackleProb
                },
                { // Player 3
                    5, // DodgeProb
                    5, // TackleProb
                },
                { // Player 4
                    5, // DodgeProb
                    5, // TackleProb
                },
                { // Player 5
                    5, // DodgeProb
                    5, // TackleProb
                },
                { // Player 6
                    1, // DodgeProb
                    10, // TackleProb
                },
                { // Player 7
                    1, // DodgeProb
                    10, // TackleProb
                },
                { // Player 8
                    1, // DodgeProb
                    10, // TackleProb
                },
                { // Player 9
                    1, // DodgeProb
                    10, // TackleProb
                }
            },
            { // Team 1
                { // Player 0
                    10, // DodgeProb
                    1, // TackleProb
                },
                { // Player 1
                    10, // DodgeProb
                    1, // TackleProb
                },
                { // Player 2
                    10, // DodgeProb
                    1, // TackleProb
                },
                { // Player 3
                    5, // DodgeProb
                    5, // TackleProb
                },
                { // Player 4
                    5, // DodgeProb
                    5, // TackleProb
                },
                { // Player 5
                    5, // DodgeProb
                    5, // TackleProb
                },
                { // Player 6
                    1, // DodgeProb
                    10, // TackleProb
                },
                { // Player 7
                    1, // DodgeProb
                    10, // TackleProb
                },
                { // Player 8
                    1, // DodgeProb
                    10, // TackleProb
                },
                { // Player 9
                    1, // DodgeProb
                    10, // TackleProb
                }
            }
        };

        ////////////////////////
        //  Helper functions  //
        ////////////////////////
        private static int getPlayerRowIdx(int team, int player) 
        {
            return (team * NUMBER_OF_PLAYERS_PER_TEAM + player) * NUMBER_OF_DIMENSIONS;
        }

        private static int getPlayerColIdx(int team, int player) 
        {
            return getPlayerRowIdx(team, player) + 1;
        }

        ////////////////////////
        // Movement functions //
        ////////////////////////
        
        public static int[] move(int[] playerPositions, int possession, int[] ballPosition, int ballPlayer)
        {
            if (possession != -1)
            {
                // If either team has the ball, move defensively or offensively
                return strategicMove(playerPositions, possession, ballPlayer);
            }
            else
            {
                // Otherwise, both teams chase after the ball
                return runToBall(playerPositions, ballPosition);
            }
        }

        private static void moveTowardsOpponentGoal(int[] playerPositions, int team, int ballPlayer) {
            for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player++) {
                // Player who carries the ball does not move
                if (player == ballPlayer)
                {
                    continue;
                }

                int playerRowIdx = getPlayerRowIdx(team, player);
                int playerColIdx = getPlayerColIdx(team, player);

                // Handle horizontal movement first
                if (team == 0) 
                {
                    // Team 0 attacks to the right
                    playerPositions[playerColIdx] = Math.Min(playerPositions[playerColIdx] + 1, MAX_COL);
                }

                if (team == 1) 
                {
                    // Team 1 attacks to the left
                    playerPositions[playerColIdx] = Math.Max(playerPositions[playerColIdx] - 1, MIN_COL);
                }

                // 33% to move diagonally
                /* NOTE: This introduces variance into the verification probabilities. */
                bool moveDiagonally = rand.Next(0, 3) == 0;

                if (!moveDiagonally) 
                {
                    continue;
                }

                // Handle vertical movement
                if (playerPositions[playerRowIdx] < 3)
                {
                    playerPositions[playerRowIdx] = Math.Max(playerPositions[playerRowIdx] - 1, MIN_ROW);
                }
                else
                {
                    playerPositions[playerRowIdx] = Math.Min(playerPositions[playerRowIdx] + 1, MAX_ROW);
                }
            }
        }

        private static void moveTowardsOwnGoal(int[] playerPositions, int team) {
            for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player++) {
                int playerRowIdx = getPlayerRowIdx(team, player);
                int playerColIdx = getPlayerColIdx(team, player);

                // Handle horizontal movement first
                if (team == 0)
                {
                    // Team 0 defends to the left
                    playerPositions[playerColIdx] = Math.Max(playerPositions[playerColIdx] - 1, MIN_COL);
                }

                if (team == 1)
                {
                    // Team 1 defends to the right
                    playerPositions[playerColIdx] = Math.Min(playerPositions[playerColIdx] + 1, MAX_COL);
                }

                // 33% to move diagonally
                /* NOTE: This introduces variance into the verification probabilities. */
                bool moveDiagonally = rand.Next(0, 3) == 0;

                if (!moveDiagonally) 
                {
                    continue;
                }

                // Handle vertical movement
                if (playerPositions[playerRowIdx] < 3)
                {
                    playerPositions[playerRowIdx] = Math.Max(playerPositions[playerRowIdx] - 1, MIN_ROW);
                }
                else
                {
                    playerPositions[playerRowIdx] = Math.Min(playerPositions[playerRowIdx] + 1, MAX_ROW);
                }
            }
        }

        public static int[] strategicMove(int[] playerPositions, int possession, int ballPlayer)
        {
            if (possession == 0) {
                moveTowardsOwnGoal(playerPositions, 1);
                moveTowardsOpponentGoal(playerPositions, 0, ballPlayer);
            } else {
                moveTowardsOwnGoal(playerPositions, 0);
                moveTowardsOpponentGoal(playerPositions, 1, ballPlayer);
            }

            return playerPositions;
        }

        public static int[] runToBall(int[] playerPositions, int[] ballPosition)
        {
            for (int team = 0; team < NUMBER_OF_TEAMS; team++)
            {
                for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player++)
                {
                    int playerRowIdx = getPlayerRowIdx(team, player);
                    int playerColIdx = getPlayerColIdx(team, player);
                    int playerRow = playerPositions[playerRowIdx];
                    int playerCol = playerPositions[playerColIdx];
                    int ballRow = ballPosition[0];
                    int ballCol = ballPosition[1];
                    if (playerRow < ballRow)
                    {
                        playerPositions[playerRowIdx]++;
                    }
                    else if (playerRow > ballRow)
                    {
                        playerPositions[playerRowIdx]--;
                    }
                    if (playerCol < ballCol)
                    {
                        playerPositions[playerColIdx]++;
                    }
                    else if (playerCol > ballCol)
                    {
                        playerPositions[playerColIdx]--;
                    }
                }
            }

            return playerPositions;
        }

        public static int[] dribble(int[] playerPositions, int possession, int ballPlayer)
        {
            int playerRowIdx = getPlayerRowIdx(possession, ballPlayer);
            int playerColIdx = getPlayerColIdx(possession, ballPlayer);
            int playerRow = playerPositions[playerRowIdx];
            int playerCol = playerPositions[playerColIdx];

            if (playerRow < 3) {
                playerPositions[playerRowIdx]++;
            } else if (playerRow > 3) {
                playerPositions[playerRowIdx]--;
            }

            if (possession == 0) {
                playerPositions[playerColIdx] = Math.Min(playerCol + 1, MAX_COL);
            } else {
                playerPositions[playerColIdx] = Math.Max(playerCol - 1, MIN_COL);
            }

            return playerPositions;
        }

        ///////////////////////////
        // Probability functions //
        ///////////////////////////

        public static int[] getOffenseProbabilities(int possession, int[] ballPosition, int[] playerPositions) {
            // Return an array of relative probabilities for [SHOOT, PASS, DRIBBLE]
            double shootProb = getShootProb(possession, ballPosition);

            double remainingProb = 1 - shootProb;

            double passProb = getPassProb(possession, ballPosition, playerPositions) * remainingProb;

            remainingProb -= passProb;

            double dribbleProb = remainingProb;

            return new int[NUMBER_OF_OFFENSIVE_ACTIONS] {
                Convert.ToInt32(shootProb * PRECISION_FACTOR),
                Convert.ToInt32(passProb * PRECISION_FACTOR),
                Convert.ToInt32(dribbleProb * PRECISION_FACTOR)
            };
        }

        public static int getDodgeProb(int team, int player)
        {
            return PLAYER_PROBABILITIES[team, player, DODGE];
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

                int ratio = PLAYER_PROBABILITIES[team, player, TACKLE];
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

                int ratio = PLAYER_PROBABILITIES[team, player, TACKLE];
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

        private static double getShootProb(int team, int[] ballPosition) {
            // Shooting probability is dependent on the distance of the player from the opponent goal

            // Determine goalPosition
            int goalRow = 3;
            int goalCol = team == 0 ? 8 : 0;

            int ballRow = ballPosition[0];
            int ballCol = ballPosition[1];

            int horizontalDistance = Math.Abs(ballCol - goalCol);
            int verticalDistance = Math.Abs(ballRow - goalRow);

            double distance = Math.Sqrt(horizontalDistance * horizontalDistance + verticalDistance * verticalDistance);

            /* 
                Use formula of p = e^(1 - distance) to find probability
                This gives a probability of 1 if distance = 1, i.e. right in front of goal
                This gives a probability of 0.368 if distance = 2
                This gives a probability of 0.135 if distance = 3
                Sounds reasonable
            */
            double distanceProbability = Math.Exp(1.0 - distance);

            return distanceProbability;
        }

        public static int getScoreProb(int team, int player) 
        {
            return Convert.ToInt32((double)OFFENSIVE_STATS[team, player, GOALS] / (double)OFFENSIVE_STATS[team, player, SHOTS] * PRECISION_FACTOR);
        }

        public static int getOutOfBoundsProb(int team, int player) {
            return Convert.ToInt32((double)(OFFENSIVE_STATS[team, player, SHOTS] - OFFENSIVE_STATS[team, player, SHOTS_ON_TARGET]) / (double)OFFENSIVE_STATS[team, player, SHOTS] * PRECISION_FACTOR);
        }

        public static int getMissProb(int team, int player) {
            return Convert.ToInt32((double)(OFFENSIVE_STATS[team, player, SHOTS_ON_TARGET] - OFFENSIVE_STATS[team, player, GOALS]) / (double)OFFENSIVE_STATS[team, player, SHOTS] * PRECISION_FACTOR);
        }

        public static double getPassProb(int possession, int[] ballPosition, int[] playerPositions) 
        {
            // Pass probability is dependent on the number of opposing players at the same position - the more players, the higher
            int opposingTeam = (possession + 1) % 2;

            int count = 0;

            for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player++) {
                int playerRowIdx = getPlayerRowIdx(opposingTeam, player);
                int playerColIdx = getPlayerColIdx(opposingTeam, player);

                int playerRow = playerPositions[playerRowIdx];
                int playerCol = playerPositions[playerColIdx];

                if (playerRow == ballPosition[0] && playerCol == ballPosition[1]) {
                    count++;
                }
            }

            /* 
                Use formula of p = 1 / (1 + e^(3 - 3 * count)) to find probability
                This gives a probability of 0.047 if count = 0
                This gives a probability of 0.5 if count = 1
                This gives a probability of 0.953 if count = 2
                This gives a probability of 0.998 if count = 3
                Sounds reasonable
            */
            return 1.0 / (1.0 + Math.Exp(3.0 - 3.0 * count));
        }

        public static int[] setPassTarget(int possession,  int[] ballPosition) {
            // We randomly pass to a position in front or beside

            int ballCol = ballPosition[1];

            if (possession == 0) 
            {
                // We want to pass to the right
                int resultantCol = rand.Next(ballCol, MAX_COL);
                int resultantRow = rand.Next(MIN_ROW, MAX_ROW + 1);
                return new int[2] { resultantRow, resultantCol };
            } 
            else 
            {
                int resultantCol = rand.Next(MIN_COL + 1, ballCol + 1);
                int resultantRow = rand.Next(MIN_ROW, MAX_ROW + 1);
                return new int[2] { resultantRow, resultantCol };
            }
        }

        ////////////////////////////
        // Non-movement functions //
        ////////////////////////////

        public static int[] getBall(int[] playerPositions, int team, int possession, int[] ballPosition)
        {
            for (int player = 0; player < NUMBER_OF_PLAYERS_PER_TEAM; player++)
            {
                int playerRowIdx = getPlayerRowIdx(team, player);
                int playerColIdx = getPlayerColIdx(team, player);
                if (ballPosition[0] == playerPositions[playerRowIdx] && ballPosition[1] == playerPositions[playerColIdx])
                {
                    return new int[2]{ team, player };
                }
            }
            return new int[2]{ -1, -1 };
        }

        public static int[] setOutOfBoundThenThrowIn(int[] ballPosition) {
            int ballRow = ballPosition[0];

            if (ballRow < 3) {
                ballPosition[0] = MIN_ROW; // skip throw in action
            } else if (ballRow > 3) {
                ballPosition[0] = MAX_ROW;
            } else {
                bool goUp = rand.Next(0,1) == 0;
                if (goUp) {
                    ballPosition[0] = MIN_ROW;
                } else {
                    ballPosition[0] = MAX_ROW; 
                }
            }
            return ballPosition;
        }

        public static int[] getFormation(int code) {
            if (code == 442) {
                return FORMATION_4_4_2;
            }

            return FORMATION_4_3_3;
        }
    }
}
