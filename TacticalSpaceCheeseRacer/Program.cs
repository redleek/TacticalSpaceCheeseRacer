using System;
/*
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
*/

namespace TacticalSpaceCheeseRacer
{
    class Program
    {
        #region Constant and Top-hierarchical Data
        /// <summary>
        /// A data structure for multiple players of a game.
        /// </summary>
        struct Player
        {
            public string name;
            // Denotes position of player on the board.
            public int position;
            // Denotes whether or not the player has already rolled the tactical dice on their turn.
            public bool tact_roll_used;
        };

        // Define constants we will want to use throughout the program.
        const int MAX_PLAYERS = 4;
        const int MIN_PLAYERS = 2;

        // Initialise an array to store each player.
        static Player[] players;

        // Allow each method to find out the number of players.
        static int no_of_players;

        // Initialise the 'won' game state to false so the main loop is enabled.
        static bool gamewon = false;

        // Represents the status of debug mode
# if DEBUG
        static bool debugmode = false;
# endif

        // Create an instance of random number so we don't get the same values in a short space of time.
        static Random randomnum = new Random();
        #endregion

        #region Input Functions
        /// <summary>
        /// Reads in the input for a yes/no question.
        /// </summary>
        /// <param name="prompt">A prompt to ask the user.</param>
        /// <returns>Returns true for "yes" and false for "no".</returns>
        static bool ReadYN(string prompt)
        {
            do
            {
                Console.Write(prompt);
                string text2check = Console.ReadLine().ToLower();

                if (text2check == "y" || text2check == "yes")
                {
                    return true;
                }
                else if (text2check == "n" || text2check == "no")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Invalid input, please enter yes or no [Y/n]");
                }
            }
            while (true);
        }

        /// <summary>
        /// Reads in the input for a question that requires an integer within a given range.
        /// </summary>
        /// <param name="prompt">A prompt to ask the user.</param>
        /// <param name="max">The maximum value the user is allowed to enter.</param>
        /// <param name="min">The minimum value the user is allowed to enter.</param>
        /// <returns></returns>
        static int ReadNumber(string prompt, int max, int min)
        {
            int result = 0;
            do
            {
                Console.Write(prompt);
                // Try and get integer from the user, if it is larger than the maximum that a 32-bit integer can hold, tell the user and get them to re-enter. This also works when the user has entered characters.
                try
                {
                    result = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Warning: number entered too large to handle or text entered, enter a numerical value between {0} and {1}", min, max);
                    continue;
                }

                if ((result > max) || (result < min))
                {
                    Console.WriteLine("Please enter a numerical value in the range {0} to {1}.", min, max);
                }
                else
                {
                    return result;
                }
            }
            while (true);
        }
        #endregion

        #region Game Setup Functions
        /// <summary>
        /// Parse command line arguments when the program is run.
        /// </summary>
        /// <param name="args">Array that contains the arguments.</param>
        static void ParseCLA(string[] args)
        {
            try
            {
                if (args == null)
                {
                    // Place holder so the else doesn't execute if args is null.
                    return;
                }
            }
            finally
            {
# if DEBUG
                try
                {
                    // Enable the testing variables.
                    if (args[0] == "--test-enable" || args[0] == "-t")
                    {
                        // Allow automatic use of the testing input variables.
                        debugmode = true;
                    }
                }
                catch
                {
                    // Leave blank to continue.
                }
# endif
                try
                {
                    if (args[0] == "--help" || args[0] == "-h")
                    {
                        // Print out the command line help.
                    }
                }
                catch
                {
                    // Leave blank to continue.
                }
            }
            return;
        }

        // A method to set up the players and resources of the game. Options available for re-initialising variables.
        /// <summary>
        /// Set up the players and resources of the game. Also allows reinitialisation/reset of current players in-game.
        /// </summary>
        /// <param name="reinitialise">Set to reinitialise/reset the current players.</param>
        /// <param name="use_old_names">Set to keep the currently stored player names.</param>
        static void SetupGame(bool reinitialise, bool use_old_names)
        {
            if (reinitialise)
            {
                bool player_nums_same = ReadYN("Use same number of players? [Y/n]: ");
                if (player_nums_same)
                {
                    Console.WriteLine("WARNING: Incomplete method parameter, still in development.");
                    return;
                }
            }
            else
            {
                no_of_players = ReadNumber("Please enter the number of players: ", MAX_PLAYERS, MIN_PLAYERS);
                //Console.WriteLine("The number of players is: {0}", no_of_players);
                players = new Player[no_of_players];

                // Counter used for asking for a specific player name.
                int player_number_count = 1;
                // Read in the players' names.
                for (int i = 0; i < no_of_players; i++)
                {
                    Console.Write("Please enter the name of player {0}: ", player_number_count);
                    players[i].name = Console.ReadLine();
                    player_number_count++;
                }

                // Check that the names are being stored correctly.
# if DEBUG
                Console.WriteLine("Stored players:");
                for (int i = 0; i < no_of_players; i++)
                {
                    Console.WriteLine(players[i].name);
                }
# endif

            }
        }
        #endregion

        #region Gameplay Functions
        /// <summary>
        /// Generates a psuedo random number for a dice roll.
        /// </summary>
        /// <returns>Returns a random number.</returns>
        static int RandDiceRoll()
        {
            return randomnum.Next(1, 7);
        }

        /// <summary>
        /// Execute procedures required for a player's turn including a dice roll and a tactical dice roll.
        /// </summary>
        /// <param name="player">The specified player who's turn it is.</param>
        static void PlayerTurn(int playerno)
        {
            // Rolls the dice by getting value from user (debug mode) or randomly generated.
#if DEBUG
            int roll = ReadNumber("Please enter a value for the dice roll: ", 65535, -65535);
# else
            int roll = RandDiceRoll();
#endif
            players[playerno].position += roll;
            Console.WriteLine("{0}'s new position on the board is {1}.", players[playerno].name, players[playerno].position);
        }
        #endregion


        static void Main(string[] args)
        {
            // Parse the command line arguments given to the program.
            ParseCLA(args);

# if DEBUG
            if (debugmode)
            {
                Console.WriteLine("Entering in Debug mode...");
            }
# endif

            // Set up the game for the first time. Reinitialise and use of old names will never need to be set on the first set up.
            SetupGame(false, false);

            // Enter main game loop while the game has not been won.
            do
            {
                for (int playercount = 0; playercount < no_of_players; playercount++)
                {
                    PlayerTurn(playercount);
                    // Check if the current player has won.
                    if (players[playercount].position >= 64)
                    {
                        gamewon = true;
                        Console.WriteLine("{0} has won the game!", players[playercount].name);
                        break;
                    }
                    else
                    {
                        // Do tactical stuff later.
                    }
                }
            } while (!gamewon);

            /*  Testing functions
            SetupGame(false, false);
            players[0].name = "Freddie Mercury";
            players[0].position = 0;
            players[0].tact_roll_used = false;
            Console.WriteLine("{0} has a position of {1}", players[0].name, players[0].position);
            */

            Console.ReadLine();
            return;
        }
    }
}