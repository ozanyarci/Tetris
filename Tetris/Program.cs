using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleTetris
{
    class Program
    {
        static int boardWidth = 10;
        static int boardHeight = 20;
        static int[,] board = new int[boardHeight, boardWidth];

        static List<int[,]> tetrominoes = new List<int[,]>
        {
            new int[,] { { 1, 1, 1, 1 } }, // I
            new int[,] { { 2, 2, 0 }, { 0, 2, 2 } }, // Z
            new int[,] { { 0, 3, 3 }, { 3, 3, 0 } }, // S
            new int[,] { { 4, 0, 0 }, { 4, 4, 4 } }, // J
            new int[,] { { 0, 0, 5 }, { 5, 5, 5 } }, // L
            new int[,] { { 6, 6 }, { 6, 6 } }, // O
            new int[,] { { 7, 7, 7 }, { 0, 7, 0 } }  // T
        };

        static int currentPieceIndex;
        static int[,] currentPiece;
        static int currentX, currentY;

        static void Main(string[] args)
        {
            Random rand = new Random();
            currentPieceIndex = rand.Next(tetrominoes.Count);
            currentPiece = tetrominoes[currentPieceIndex];
            currentX = boardWidth / 2 - currentPiece.GetLength(1) / 2;
            currentY = 0;

            Task.Run(() => {
                while (true)
                {
                    Thread.Sleep(1000);
                    if (!MovePiece(1, 0))
                    {
                        PlacePiece();
                        CheckForLines();
                        currentPieceIndex = rand.Next(tetrominoes.Count);
                        currentPiece = tetrominoes[currentPieceIndex];
                        currentX = boardWidth / 2 - currentPiece.GetLength(1) / 2;
                        currentY = 0;

                        if (!MovePiece(0, 0))
                        {
                            Console.Clear();
                            Console.SetCursorPosition(0, 5);
                            Console.WriteLine("Game Over!");
                            break;
                        }
                    }
                    Render();
                }
            });

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.LeftArrow:
                            MovePiece(0, -1);
                            break;
                        case ConsoleKey.RightArrow:
                            MovePiece(0, 1);
                            break;
                        case ConsoleKey.DownArrow:
                            MovePiece(1, 0);
                            break;
                        case ConsoleKey.UpArrow:
                            RotatePiece();
                            break;
                    }
                    Render();
                }
            }
        }

        static void Render()
        {
            Console.Clear();
            for (int i = 0; i < boardHeight; i++)
            {
                for (int j = 0; j < boardWidth; j++)
                {
                    Console.Write(board[i, j] > 0 ? "#" : " ");
                }
                Console.WriteLine();
            }

            for (int i = 0; i < currentPiece.GetLength(0); i++)
            {
                for (int j = 0; j < currentPiece.GetLength(1); j++)
                {
                    if (currentPiece[i, j] > 0)
                    {
                        Console.SetCursorPosition(currentX + j, currentY + i);
                        Console.Write("#");
                    }
                }
            }
        }

        static bool MovePiece(int dx, int dy)
        {
            if (CanMove(currentPiece, currentX + dy, currentY + dx))
            {
                currentY += dx;
                currentX += dy;
                return true;
            }
            return false;
        }

        static void RotatePiece()
        {
            int[,] rotatedPiece = RotateMatrix(currentPiece);
            if (CanMove(rotatedPiece, currentX, currentY))
            {
                currentPiece = rotatedPiece;
            }
        }

        static int[,] RotateMatrix(int[,] matrix)
        {
            int[,] ret = new int[matrix.GetLength(1), matrix.GetLength(0)];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    ret[j, matrix.GetLength(0) - i - 1] = matrix[i, j];
                }
            }
            return ret;
        }

        static bool CanMove(int[,] piece, int newX, int newY)
        {
            for (int i = 0; i < piece.GetLength(0); i++)
            {
                for (int j = 0; j < piece.GetLength(1); j++)
                {
                    if (piece[i, j] > 0)
                    {
                        int boardX = newX + j;
                        int boardY = newY + i;
                        if (boardX < 0 || boardX >= boardWidth || boardY >= boardHeight || (boardY >= 0 && board[boardY, boardX] > 0))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        static void PlacePiece()
        {
            for (int i = 0; i < currentPiece.GetLength(0); i++)
            {
                for (int j = 0; j < currentPiece.GetLength(1); j++)
                {
                    if (currentPiece[i, j] > 0)
                    {
                        int boardX = currentX + j;
                        int boardY = currentY + i;
                        if (boardY < boardHeight && boardX < boardWidth)
                        {
                            board[boardY, boardX] = currentPiece[i, j];
                        }
                    }
                }
            }
        }

        static void CheckForLines()
        {
            for (int i = 0; i < boardHeight; i++)
            {
                bool lineComplete = true;
                for (int j = 0; j < boardWidth; j++)
                {
                    if (board[i, j] == 0)
                    {
                        lineComplete = false;
                        break;
                    }
                }
                if (lineComplete)
                {
                    for (int k = i; k > 0; k--)
                    {
                        for (int j = 0; j < boardWidth; j++)
                        {
                            board[k, j] = board[k - 1, j];
                        }
                    }
                    for (int j = 0; j < boardWidth; j++)
                    {
                        board[0, j] = 0;
                    }
                }
            }
        }
    }
}