using System;

namespace TicTacToeCLI
{
    enum GameMode
    {
        TwoPlayer = 1,
        VsComputer = 2
    }

    class Program
    {
        static char[] board;
        static char currentPlayer;
        static GameMode gameMode;

        static void Main(string[] args)
        {
            Console.Title = "Tic-Tac-Toe (CLI)";
            bool playAgain = true;

            while (playAgain)
            {
                gameMode = ChooseGameMode();
                InitializeGame();
                PlayGame();

                Console.Write("\nPlay again? (y/n): ");
                string? input = Console.ReadLine();
                playAgain = !string.IsNullOrEmpty(input) && char.ToLowerInvariant(input[0]) == 'y';
            }
        }

        static GameMode ChooseGameMode()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Tic-Tac-Toe (CLI)");
                Console.WriteLine("-----------------\n");
                Console.WriteLine("Choose game mode:");
                Console.WriteLine("1) 2-Player (Human vs Human)");
                Console.WriteLine("2) AI Mode (Human vs Computer)");
                Console.Write("\nEnter choice (1 or 2): ");

                string? input = Console.ReadLine();
                if (int.TryParse(input, out int choice) &&
                    (choice == (int)GameMode.TwoPlayer || choice == (int)GameMode.VsComputer))
                {
                    return (GameMode)choice;
                }

                Console.WriteLine("Invalid choice. Press any key to try again...");
                Console.ReadKey(true);
            }
        }

        static void InitializeGame()
        {
            board = new char[9];
            for (int i = 0; i < 9; i++)
            {
                board[i] = (char)('1' + i); // '1'..'9'
            }

            currentPlayer = 'X'; // Human is X (and always first in AI mode)
        }

        static void PlayGame()
        {
            bool gameOver = false;

            while (!gameOver)
            {
                Console.Clear();
                Console.WriteLine("Tic-Tac-Toe (CLI)");
                Console.WriteLine("-----------------\n");
                Console.WriteLine(gameMode == GameMode.TwoPlayer
                    ? "Mode: 2-Player (Human vs Human)\n"
                    : "Mode: AI Mode (Human (X) vs Computer (O))\n");

                DrawBoard();

                int moveIndex;
                if (gameMode == GameMode.VsComputer && currentPlayer == 'O')
                {
                    Console.WriteLine("\nComputer (O) is thinking...");
                    moveIndex = GetComputerMove();
                }
                else
                {
                    Console.WriteLine($"\nPlayer {currentPlayer}'s turn.");
                    moveIndex = GetPlayerMove();
                }

                board[moveIndex] = currentPlayer;

                if (IsWinner(currentPlayer))
                {
                    Console.Clear();
                    Console.WriteLine("Tic-Tac-Toe (CLI)");
                    Console.WriteLine("-----------------\n");
                    Console.WriteLine(gameMode == GameMode.TwoPlayer
                        ? "Mode: 2-Player (Human vs Human)\n"
                        : "Mode: AI Mode (Human (X) vs Computer (O))\n");
                    DrawBoard();

                    if (gameMode == GameMode.VsComputer && currentPlayer == 'O')
                        Console.WriteLine("\nComputer (O) wins!");
                    else
                        Console.WriteLine($"\nPlayer {currentPlayer} wins!");

                    gameOver = true;
                }
                else if (IsDraw())
                {
                    Console.Clear();
                    Console.WriteLine("Tic-Tac-Toe (CLI)");
                    Console.WriteLine("-----------------\n");
                    Console.WriteLine(gameMode == GameMode.TwoPlayer
                        ? "Mode: 2-Player (Human vs Human)\n"
                        : "Mode: AI Mode (Human (X) vs Computer (O))\n");
                    DrawBoard();
                    Console.WriteLine("\nIt's a draw!");
                    gameOver = true;
                }
                else
                {
                    SwitchPlayer();
                }
            }
        }

        static void DrawBoard()
        {
            Console.WriteLine(" {0} | {1} | {2} ", board[0], board[1], board[2]);
            Console.WriteLine("---+---+---");
            Console.WriteLine(" {0} | {1} | {2} ", board[3], board[4], board[5]);
            Console.WriteLine("---+---+---");
            Console.WriteLine(" {0} | {1} | {2} ", board[6], board[7], board[8]);
        }

        static int GetPlayerMove()
        {
            while (true)
            {
                Console.Write("Enter a position (1-9): ");
                string? input = Console.ReadLine();

                if (!int.TryParse(input, out int pos))
                {
                    Console.WriteLine("Invalid input. Please enter a number 1-9.");
                    continue;
                }

                if (pos < 1 || pos > 9)
                {
                    Console.WriteLine("Position out of range. Choose 1-9.");
                    continue;
                }

                int index = pos - 1;
                if (board[index] == 'X' || board[index] == 'O')
                {
                    Console.WriteLine("That position is already taken. Choose another.");
                    continue;
                }

                return index;
            }
        }

        // Simple AI: win if possible, block, then take center, corner, side
        static int GetComputerMove()
        {
            // 1. If computer can win this move, take it.
            int move = FindWinningMove('O');
            if (move != -1) return move;

            // 2. If human can win next move, block it.
            move = FindWinningMove('X');
            if (move != -1) return move;

            // 3. Take center if available.
            if (board[4] != 'X' && board[4] != 'O')
                return 4;

            // 4. Take a corner if available.
            int[] corners = { 0, 2, 6, 8 };
            foreach (int c in corners)
            {
                if (board[c] != 'X' && board[c] != 'O')
                    return c;
            }

            // 5. Take any side.
            int[] sides = { 1, 3, 5, 7 };
            foreach (int s in sides)
            {
                if (board[s] != 'X' && board[s] != 'O')
                    return s;
            }

            // Fallback (shouldn't happen): first available
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] != 'X' && board[i] != 'O')
                    return i;
            }

            return 0; // safety
        }

        static int FindWinningMove(char player)
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == 'X' || board[i] == 'O')
                    continue;

                char backup = board[i];
                board[i] = player;
                bool wins = IsWinner(player);
                board[i] = backup;

                if (wins)
                    return i;
            }
            return -1;
        }

        static bool IsWinner(char player)
        {
            int[,] winningCombinations = new int[,]
            {
                {0, 1, 2}, // rows
                {3, 4, 5},
                {6, 7, 8},
                {0, 3, 6}, // columns
                {1, 4, 7},
                {2, 5, 8},
                {0, 4, 8}, // diagonals
                {2, 4, 6}
            };

            for (int i = 0; i < winningCombinations.GetLength(0); i++)
            {
                int a = winningCombinations[i, 0];
                int b = winningCombinations[i, 1];
                int c = winningCombinations[i, 2];

                if (board[a] == player && board[b] == player && board[c] == player)
                {
                    return true;
                }
            }

            return false;
        }

        static bool IsDraw()
        {
            foreach (char c in board)
            {
                if (c != 'X' && c != 'O')
                    return false;
            }
            return true;
        }

        static void SwitchPlayer()
        {
            currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
        }
    }
}
