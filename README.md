TacticalSpaceCheeseRacer
========================

A C# text-based board game in which up to 4 players traverse a board trying to reach the far reaches of the cheese galaxy!

Game Concept
============

On a board (just pretend it's space please), players can roll a dice to move their rockets which they control onto different space squares. Some squares, namely: 8, 15, 19, 28, 33, 45, 55, 59 are `infused' with `cheese power' which the ships can use to perform special actions.

Rocket Movements
----------------
Each player awaits their turn to roll their dice. They will start off at square 0 and then move onto the board. On each player's turn, they should progress their rocket by the amout shown on the dice when rolled. Any square can contain any number of rockets.

Each game ends when a player's rocket has moved to or over the 64th square (the square at the very end of the board). The player to reach the end of the board is declared the ultimate ruler of the cheese galaxy!

Tactical Gameplay
-----------------
Once a player has moved their rocket along the board, they can choose whether or not to roll the `tactics dice'. If they land on a cheese square, they are forced to roll the tactics dice. The tactics dice has six possible operations depending on what the player rolls:

+ If they rolled a 1 the player's rocket has it's engines exploded and are sent back to square zero.
+ Rolling a 2 causes all the engines of all the players on the same square as the current exploded and sent back to the start.
+ Rolling a 3 is the same as rolling a 2 but instead the current player is not sent back to the start.
+ Rolling a 4 causes the current player to move six squares forwards. If they pass the last square, they win the game.
+ Rolling a 5 forces the player to move to any other payers' position.
+ Rolling a 6 forces the player to swap positions with any other player.

If a player lands on a cheese chedar square due to rolling a tactical dice of 4, 5 or 6, they are not allowed to roll another tactical dice.

How to Use
==========
Game Initialisation
-------------------
At the start of the game, the users will be prompted to enter the number of players that are going to play. This must be in the range 2--4 as that is the amount of players that can play the game. Then, the players need to insert each name for all of the players.

Gameplay
--------
The first roll after player initialisation is always rolled automagically by the cheese Faries. It will tell you what you have rolled and will again, automagically move you forward that many squares.

Afterwards, the user will be able to choose whether or not they would like to roll the tactical dice. If they decide to, they will be prompted to hit enter to roll the dice. If they get power 5 or 6, they will have to choose a player number to swap or move to. If they do not decide to roll the tactics dice, the program will move onto the next player. The program will also print a list of all the players' current positions.

Once a player has won the game by moving to or beyond the last square, they will be notified that they have won and will be asked if they want to play again. If they want to play again, they will be asked if they want to use the same number of players and use the same names if they want.
