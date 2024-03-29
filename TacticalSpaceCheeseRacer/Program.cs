/*   Program.cs - Main program file.
*    Copyright (C) 2014  Connor Blakey <connorblakey96@gmail.com>
*
*    This program is free software; you can redistribute it and/or modify
*    it under the terms of the GNU General Public License as published by
*    the Free Software Foundation; either version 2 of the License, or
*    (at your option) any later version.
*
*    This program is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU General Public License for more details.
*
*    You should have received a copy of the GNU General Public License along
*    with this program; if not, write to the Free Software Foundation, Inc.,
*    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;

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
    const int BOARD_UP_BOUND = 64;
    const int BOARD_LWR_BOUND = 0;
    
    // Initialise an array to store each player.
    static Player[] players;
    
    // Allow each method to find out the number of players.
    static int no_of_players;
    
    // Initialise the 'won' game state to false so the main loop is enabled.
    static bool gamewon = false;
    
    // Locations of all the cheese squares.
    static int[] cheese_squares = new int[8] { 8, 15, 19, 28, 33, 45, 55, 59 };
    
    // Represents the status of debug mode
#if DEBUG
    static bool debugmode = false;
#endif
    
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
	  Print(prompt);
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
	      Print("Invalid input, please enter yes or no [Y/n]");
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
	  Print(prompt);
	  // Try and get integer from the user, if it is larger than the maximum that a 32-bit integer can hold, tell the user and get them to re-enter.
	  // This also works when the user has entered characters.
	  try
	    {
	      result = int.Parse(Console.ReadLine());
	    }
	  catch
	    {
	      Console.WriteLine("Warning: number entered too large to handle or text entered, enter a numerical value between {0} and {1}.", min, max);
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
    
    /// <summary>
    /// Reads in the names of the game players from the user(s).
    /// </summary>
    static void ReadNames()
    {
      int player_number_count = 1;
      for (int playercount = 0; playercount < no_of_players; playercount++)
	{
	  Console.Write("Please enter the name of player {0}: ", player_number_count);
	  players[playercount].name = Console.ReadLine();
	  player_number_count++;
	}
    }
    #endregion
    
    #region Game Setup Functions
    /// <summary>
    /// Parse command line arguments when the program is run.
    /// </summary>
    /// <param name="args">Array that contains the arguments.</param>
    static void ParseCLA(string[] args)
    {
	  for (int argument = 0; argument < args.Length; argument++)
	    {
#if DEBUG
	      Console.WriteLine("DEBUG ARGS: On argument number: {0}", argument);
#endif
	      switch (args[argument])
		{
#if DEBUG
		case "--test-enable":
		case "-t":
		  // Allow automatic use of the testing input variables.
		  debugmode = true;
		  break;
#endif
		case "--help":
		case "-h":
		  // Print out command line help.
		  System.IO.StreamReader reader;
		  try
		    {
		      reader = new System.IO.StreamReader(@"README.md");
		    }
		  catch
		    {
		      PrintLine("Help file `README.md' not found! If found, please place in the same directory as the program.");
		      Environment.Exit(0);
		    }
		  while (true)
		    {
		      string line = reader.ReadLine();
		      if (line == null)
			{
			  break;
			}
		      PrintLine(line);
		    }
		  reader.Close();
		  Environment.Exit(0);
		  break;
		default:
		  Console.WriteLine("ERROR: \"{0}\" is not a known argument.", args[argument]);
		  Environment.Exit(1);
		  break;
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
	  // Reset all player positions back to the start of the board.
	  for (int playercount = 0; playercount < no_of_players; playercount++)
	    {
	      players[playercount].position = BOARD_LWR_BOUND;
	    }
	  if (ReadYN("Use the same number of players?: "))
	    {
	      if (!use_old_names)
		{
		  ReadNames();
		}
	    }
	  else
	    {
	      no_of_players = ReadNumber("Please enter the new number of players: ", MAX_PLAYERS, MIN_PLAYERS);
	      players = new Player[no_of_players];
	      ReadNames();
	    }
	}
      else
	{
	  no_of_players = ReadNumber("Please enter the number of players: ", MAX_PLAYERS, MIN_PLAYERS);
#if DEBUG
	  Console.WriteLine("The number of players is: {0}", no_of_players);
#endif
	  players = new Player[no_of_players];
	  
	  // Read in the players' names.
	  ReadNames();
	  
	  // Check that the names are being stored correctly.
#if DEBUG
	  Console.WriteLine("Stored players:");
	  for (int playercount = 0; playercount < no_of_players; playercount++)
	    {
                    Console.WriteLine(players[playercount].name);
	    }
	  Console.WriteLine();
#endif
	  
	}
    }
    #endregion
    
    #region Visual Output Methods
    /// <summary>
    /// Prints the current position of the specified player on the board.
    /// </summary>
    /// <param name="playerno">The current player playing.</param>
    static void PrintPlayerPosition(int playerno)
    {
      Console.WriteLine("{0}'s new position on the board is {1}.", players[playerno].name, players[playerno].position);
    }
    
    /// <summary>
    /// Prints the current positions of all the players.
    /// </summary>
    static void PrintAllPositions()
    {
      Console.WriteLine("Players and their positions:");
      for (int playercount = 0; playercount < no_of_players; playercount++)
	{
	  Console.WriteLine("{0})  {1} \t\t- {2}", playercount + 1, players[playercount].name, players[playercount].position);
	}
    }
    
    /// <summary>
    /// Macro to clear the console to make it more visualy appealing and display
    /// all important information such as positions of all the players.
    /// </summary>
    static void ResetConsole()
    {
      Console.Clear();
      PrintAllPositions();
      Console.WriteLine();
    }
    
    /// <summary>
    /// Prints out the given string in a way that it appears to be typed by a human.
    /// </summary>
    /// <param name="Text">Text to be printed to stdout.</param>
    static void Print(string Text)
    {
      foreach (char character in Text)
	{
	  System.Threading.Thread.Sleep(randomnum.Next(
#if DEBUG
						       0, 0)
#else
						       0, 250)
#endif
					);
	  Console.Write(character);
	}
    }

    /// <summary>
    /// Same as Print but adds a newline character to the end of the string.
    /// </summary>
    /// <param name="Text">Text to be printed to stdout.</param>
    static void PrintLine(string Text)
    {
      Print(Text);
      Console.WriteLine();
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
      int roll = 0;
#if DEBUG
      if (debugmode)
	{
	  roll = ReadNumber("Please enter a value for the dice roll: ", 65535, -65535);
	}
      else
	{
	  roll = RandDiceRoll();
	}
#else
      roll = RandDiceRoll();
#endif
      Console.WriteLine("{0} rolled a {1}.", players[playerno].name, roll);
      players[playerno].position += roll;
      PrintPlayerPosition(playerno);
      
      // Check if the dice roll was 6, if so, allow current player to throw the tactics dice for another player.
      if (roll == 6)
	{
	  // Don't allow the player to roll the tactics dice for themselves.
	  do
	    {
	      PrintLine("Because you rolled a 6, you can throw the tactics dice for another player:");
	      PrintAllPositions();
	      int roll_choice = ReadNumber("Please enter a player number: ", no_of_players, 1) - 1;
	      if (roll_choice == playerno)
		{
		  PrintLine("You cannot roll for yourself.");
		  continue;
		}
	      TacticalRoll(roll_choice);
	      break;
	    } while (true);
	}
    }
    
    /// <summary>
    /// Check if the current player has won the game/at or over the board limit.
    /// </summary>
    /// <param name="playerno">The number of the current player.</param>
    /// <returns>Wether or not the player has won.</returns>
    static bool CheckWin(int playerno)
    {
      if (players[playerno].position >= BOARD_UP_BOUND)
	{
	  Console.WriteLine("{0} has won the game!", players[playerno].name);
	  return true;
	}
      else
	{
	  return false;
	}
    }
    
    #region Tactical Dice Methods
    /// <summary>
    /// Sends the player who rolled the tactics dice back to the start of the board.
    /// </summary>
    /// <param name="playerno">The number of the current player.</param>
    static void Power1(int playerno)
    {
      players[playerno].position = 0;
      Console.WriteLine("{0} has had their engines exploded and are now back to square 0.", players[playerno].name);
    }
    
    /// <summary>
    /// Sends all the rockets on the current square that the player who rolled the tactics dice is currently on.
    /// </summary>
    /// <param name="playerno">The number of the current player.</param>
    static void Power2(int playerno)
    {
      int current_player_position = players[playerno].position;
      for (int playercount = 0; playercount < no_of_players; playercount++)
	{
	  if (players[playercount].position == current_player_position)
	    {
	      players[playercount].position = 0;
	      Console.WriteLine("{0} has had his/her engines exploded and are now back to square 0.", players[playercount].name);
	    }
	}
    }
    
    /// <summary>
    /// Same as Power2 but the player who rolled does not become affected.
    /// </summary>
    /// <param name="playerno">The number if the current player.</param>
    static void Power3(int playerno)
    {
      int current_player_position = players[playerno].position;
      for (int playercount = 0; playercount < no_of_players; playercount++)
	{
	  if (players[playercount].position == current_player_position)
	    {
	      if (!(playercount == playerno))
		{
		  players[playercount].position = 0;
		  Console.WriteLine("{0} has had their engines exploded and are now back to square 0.", players[playercount].name);
		}
	    }
	}
    }
    
    /// <summary>
    /// Moves the player who rolled the tactical dice six squares forward.
    /// </summary>
    /// <param name="playerno">The number of the current player.</param>
    static void Power4(int playerno)
    {
      players[playerno].position += 6;
      Console.WriteLine("{0} has moved to square {1}.", players[playerno].name, players[playerno].position);
    }
    
    /// <summary>
    /// Make the player who rolled the tactics dice to change to someone else's position.
    /// </summary>
    /// <param name="playerno">The number of the current player.</param>
    static void Power5(int playerno)
    {
      // Use do while to be able to return back to top if user tries to move to their own position.
      do
	{
	  PrintAllPositions();
	  int newposition = ReadNumber("Please enter a player number to move to: ", no_of_players, 1) - 1;
	  if (newposition == playerno)
	    {
	      PrintLine("You cannot move to the position you are already on.");
	      continue;
	    }
	  players[playerno].position = players[newposition].position;
	  PrintPlayerPosition(playerno);
	  break;
	} while (true);
    }
    
    /// <summary>
    /// Make the player who rolled the tactics dice to swap positions with another player on the board.
    /// </summary>
    /// <param name="playerno">The number of the current player.</param>
    static void Power6(int playerno)
    {
      // Use do while to be able to return back to top if user tries to swap with themselves.
      do
	{
	  PrintAllPositions();
	  int playerno2swap = ReadNumber("Please enter a player number to swap with: ", no_of_players, 1) - 1;
	  if (playerno2swap == playerno)
	    {
	      PrintLine("You cannot swap position with yourself.");
	      continue;
	    }
	  int newposition = players[playerno2swap].position;
	  players[playerno2swap].position = players[playerno].position;
	  players[playerno].position = newposition;
	  PrintPlayerPosition(playerno2swap);
	  PrintPlayerPosition(playerno);
	  break;
	} while (true);
    }
    
    /// <summary>
    /// Manage and perform a Tactical roll for the player based on the dice roll they get.
    /// </summary>
    static void TacticalRoll(int playerno)
    {
      Print("Press enter to roll the tactical dice... ");
      Console.ReadLine();
      int roll = 0;
#if DEBUG
      if (debugmode)
	{
	  roll = ReadNumber("Please enter a value for the dice roll: ", 65535, -65535);
	}
      else
	{
	  roll = RandDiceRoll();
	}
#else
      roll = RandDiceRoll();
#endif
      Console.WriteLine("{0} rolled a {1}.", players[playerno].name, roll);
      
      // Decide which tactical roll to use.
      switch (roll)
	{
	case 1:
	  Power1(playerno);
	  break;
	case 2:
	  Power2(playerno);
	  break;
	case 3:
	  Power3(playerno);
	  break;
	case 4:
	  Power4(playerno);
	  break;
	case 5:
	  Power5(playerno);
	  break;
	case 6:
	  Power6(playerno);
	  break;
	}
    }
    #endregion
    #endregion
    
    static void Main(string[] args)
    {
      // Parse the command line arguments given to the program.
      ParseCLA(args);
      
#if DEBUG
      if (debugmode)
	{
	  PrintLine("Entering in Debug mode...");
	}
#endif
	
      // Set up the game for the first time. Reinitialise and use of old names will never need to be set on the first set up.
      SetupGame(false, false);
      
      // Loop so we can ask if the players want to play again.
      bool replay;
      do
	{
	  
	  // Enter main game loop while the game has not been won.
	  do
	    {
	      for (int playercount = 0; playercount < no_of_players; playercount++)
		{
		  PlayerTurn(playercount);
		  // Check if the current player (or player we have rolled the six power for) has won.
		  for (int playercount2 = 0; playercount2 < no_of_players; playercount2++)
		    {
		      if (CheckWin(playercount2))
			{
			  gamewon = true;
			  break;
			}
		    }
		  // Re-break as we can't double-break out of 2 for loops.
		  if (gamewon)
		    {
		      break;
		    }
		  
		  // Check if the player is on a cheese square
		  for (int item = 0; item < cheese_squares.Length; item++)
		    {
		      // Break out early if cheese square position is greater than player position.
		      if (cheese_squares[item] > players[playercount].position)
			{
			  break;
			}
		      
		      if (cheese_squares[item] == players[playercount].position)
			{
			  PrintLine("You have landed on a cheese square!");
			  TacticalRoll(playercount);
			  players[playercount].tact_roll_used = true;
			  break;
			}
		    }
		  
		  if (CheckWin(playercount))
		    {
		      gamewon = true;
		      break;
		    }
		  
		  // Ask if the player wants to roll the tactics dice.
		  if (!players[playercount].tact_roll_used)
		    {
		      if (ReadYN("Do you want to roll the tactics dice?: "))
			{
			  TacticalRoll(playercount);
			}
		    }
		  
		  if (CheckWin(playercount))
		    {
		      gamewon = true;
		      break;
		    }
		  
		  // Reset for end of turn.
		  if (players[playercount].tact_roll_used)
		    {
		      players[playercount].tact_roll_used = false;
		    }
		  
		  Print("Press enter to continue... ");
		  Console.ReadLine();
		  ResetConsole();
		}
	    } while (!gamewon);
	  
	  // Ask if they want to play again and if they want to keep the same names.
	  replay = ReadYN("Do you want to play again?: ");
	  if (replay)
	    {
	      gamewon = false;
	      SetupGame(true, ReadYN("Do you want to use the same names for players?: "));
	    }
	  
	} while (replay);
      
      return;
    }
  }
}
