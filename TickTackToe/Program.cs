using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace TickTackToe
{
    public class Board
    {
        public int[] position = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int[] moves = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public void DrawBoard()
        {
            string status = "";

            for (int i = 0; i < position.Length; i++)
            {
                if (position[i] == 0)
                {
                    status += " ";
                }
                else if (position[i] == 1)
                {
                    status += "X";
                }
                else
                {
                    status += "O";
                }
            }

            Console.WriteLine("\n" + status[0] + "|" + status[1] + "|" + status[2]);
            Console.WriteLine("-----");
            Console.WriteLine(status[3] + "|" + status[4] + "|" + status[5]);
            Console.WriteLine("-----");
            Console.WriteLine(status[6] + "|" + status[7] + "|" + status[8]);
        }
    }

    public class BoardIndex
    {
        public int boardIndex;
        public int moveIndex;
    }

    class Program
    {
        static void Main(string[] args)
        {
            int[] score = { 0, 0, 0 };

            List<Board> boards = new List<Board>();

            List<BoardIndex> boardIndices = new List<BoardIndex>();

            Board gameBoard = new Board();

            int gameWinner = 0;

            bool stopPlaying = false;

            bool rnd = false;

            Console.WriteLine("Place Your Piece With By Pressing 1-9, Starting In the Top Left Corner Of The 3x3 Grid.\n");

            while (!stopPlaying)
            {
                Setup();

                gameBoard.DrawBoard();

                Game();

                if (!rnd)
                {
                    Console.WriteLine("\nScore: \nPlayer: " + score[0] + "\nAI: " + score[1] + "\nDraws: " + score[2]);
                    Console.WriteLine("\nDo you want to play again? Y/N ");
                    if (Console.ReadLine().ToLower() == "n")
                    {
                        stopPlaying = true;
                    }
                }
            }



            void Setup()
            {
                gameBoard = new Board();

                gameWinner = 0;

                boardIndices = new List<BoardIndex>();
            }



            void Game()
            {
                while (gameWinner == 0)
                {
                    int playerMove = -1;

                    while (playerMove < 0)
                    {
                        playerMove = Play();

                        if (playerMove >= 1 && playerMove <= 9 && gameBoard.position[playerMove - 1] == 0)
                        {
                            gameBoard.position[playerMove - 1] = 1;
                        }
                        else
                        {
                            Console.WriteLine("Invalid Move!");
                            playerMove = -1;
                        }
                    }

                    gameBoard.DrawBoard();
                    Console.WriteLine("Player Placed In Cell " + playerMove + "\n");

                    if ((gameWinner = CheckWinner(gameBoard)) == 0)
                    {
                        if (!CheckDraw(gameBoard))
                        {
                            int AIMove = AIPlay();

                            gameBoard.position[AIMove] = 2;

                            gameBoard.DrawBoard();

                            Console.WriteLine("AI Placed In Cell " + (AIMove + 1) + "\n");

                            gameWinner = CheckWinner(gameBoard);
                        }
                        else
                        {
                            gameWinner = 3;
                        }
                    }
                }
                Endgame();
            }



            void Endgame()
            {
                gameBoard.DrawBoard();

                int moveModifier = 0;

                switch (gameWinner)
                {
                    case 1:
                        moveModifier = -1;
                        score[0]++;
                        Console.WriteLine("Player Wins!");
                        break;
                    case 2:
                        moveModifier = 3;
                        score[1]++;
                        Console.WriteLine("AI Wins!");
                        break;
                    case 3:
                        Console.WriteLine("Draw!");
                        score[2]++;
                        moveModifier = 1;
                        break;
                }

                for (int i = 0; i < boardIndices.Count; i++)
                {
                    boards[boardIndices[i].boardIndex].moves[boardIndices[i].moveIndex] += moveModifier;
                    Console.WriteLine("\nModified board " + boardIndices[i].boardIndex);
                    Console.WriteLine("Move " + boardIndices[i].moveIndex + " Now Has The Weight Of " + boards[boardIndices[i].boardIndex].moves[boardIndices[i].moveIndex]);
                }
            }



            int Play()
            {
                int pos = 0;

                if (rnd)
                {
                    Random rand = new Random();
                    pos = rand.Next(1, 9);
                }
                else
                {
                    Console.WriteLine("Make your move!\n");
                    int.TryParse(Console.ReadLine(), out pos);
                }

                return pos;
            }



            int AIPlay()
            {
                int index = -1;

                BoardIndex boardIndex = new BoardIndex();

                for (int i = 0; i < boards.Count; i++)
                {
                    if (boards[i] == gameBoard)
                    {
                        index = i;
                    }
                }

                if (index >= 0)
                {
                    boardIndex.boardIndex = index;
                    index = GetRandomMove(boards[index]);
                    boardIndex.moveIndex = index;
                }
                else
                {
                    Board tempBoard = GenerateBoard(gameBoard);

                    index = GetRandomMove(tempBoard);

                    boardIndex.moveIndex = index;

                    boards.Add(tempBoard);

                    boardIndex.boardIndex = boards.Count - 1;
                }

                boardIndices.Add(boardIndex);

                return index;
            }



            Board GenerateBoard(Board refBoard)
            {
                Board board = new Board();

                board.position = refBoard.position;

                for (int i = 0; i < board.moves.Length; i++)
                {
                    if (board.position[i] < 1)
                    {
                        board.moves[i] = 2;
                    }
                    else
                    {
                        board.moves[i] = 0;
                    }
                }

                return board;
            }



            int GetRandomMove(Board board)
            {
                List<int> randList = new List<int>();

                for (int i = 0; i < board.moves.Length; i++)
                {
                    for (int o = 0; o < board.moves[i]; o++)
                    {
                        randList.Add(i);
                    }
                }
                
                Random random = new Random();

                int ret = randList[random.Next(0, randList.Count - 1)];

                return ret;
            }



            bool CheckDraw(Board board)
            {
                bool draw = true;

                for (int i = 0; i < board.position.Length; i++)
                {
                    if (board.position[i] < 1)
                    {
                        draw = false;
                    }
                }

                return draw;
            }

            int CheckWinner(Board board)
            {
                int winNum = 0;

                int[] pos = board.position;



                if (pos[0] > 0)
                {
                    int i = pos[0];

                    if (pos[1] == i)
                    {
                        if (pos[2] == i)
                        {
                            winNum = i;
                        }
                    }

                    if (pos[3] == i)
                    {
                        if (pos[6] == i)
                        {
                            winNum = i;
                        }
                    }

                    if (pos[4] == i)
                    {
                        if (pos[8] == i)
                        {
                            winNum = i;
                        }
                    }
                }



                if (pos[1] > 0)
                {
                    int x = pos[1];

                    if (pos[4] == x)
                    {
                        if (pos[7] == x)
                        {
                            winNum = x;
                        }
                    }
                }



                if (pos[2] > 0)
                {
                    int x = pos[2];

                    if (pos[5] == x)
                    {
                        if (pos[8] == x)
                        {
                            winNum = x;
                        }
                    }

                    if (pos[4] == x)
                    {
                        if (pos[6] == x)
                        {
                            winNum = x;
                        }
                    }
                }



                if (pos[3] > 0)
                {
                    int x = pos[3];

                    if (pos[4] == x)
                    {
                        if (pos[5] == x)
                        {
                            winNum = x;
                        }
                    }
                }



                if (pos[6] > 0)
                {
                    int x = pos[6];

                    if (pos[7] == x)
                    {
                        if (pos[8] == x)
                        {
                            winNum = x;
                        }
                    }
                }



                return winNum;
            }
        }
    }
}
