using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Windows.Forms;

namespace CheckersGame
{
    public class Board
    {
        public Piece[,] Squares { get; set; }
        public const int Size = 8;
        private AI ai = new AI();

        public Board()
        {
            Squares = new Piece[Size, Size];
            InitializeBoard();
        }

        public Board(Board other)
        {
            Squares = new Piece[Size, Size];
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    Squares[x, y] = new Piece(other.Squares[x, y].Color, other.Squares[x, y].Type);
                }
            }
        }

        private void InitializeBoard()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (y < 3 && (x + y) % 2 != 0)
                    {
                        Squares[x, y] = new Piece(PieceColor.Red, PieceType.Man);
                    }
                    else if (y > 4 && (x + y) % 2 != 0)
                    {
                        Squares[x, y] = new Piece(PieceColor.Black, PieceType.Man);
                    }
                    else
                    {
                        Squares[x, y] = new Piece(PieceColor.None, PieceType.None);
                    }
                }
            }
        }

        public bool IsValidMove(Move move, PieceColor currentTurn)
        {
            // Check if the target position is within bounds
            if (move.EndX < 0 || move.EndX >= Board.Size || move.EndY < 0 || move.EndY >= Board.Size)
            {
                return false;
            }

            Piece piece = Squares[move.StartX, move.StartY];
            Piece targetPiece = Squares[move.EndX, move.EndY];

            if (piece.Color != currentTurn || targetPiece.Color != PieceColor.None)
            {
                return false;
            }

            int dx = move.EndX - move.StartX;
            int dy = move.EndY - move.StartY;

            // Check for forward movement for man pieces
            if (piece.Type == PieceType.Man)
            {
                if (piece.Color == PieceColor.Red && dy <= 0 && !HasCapture(piece.Color, move.StartX, move.StartY))
                {
                    return false;
                }
                else if (piece.Color == PieceColor.Black && dy > 0 && !HasCapture(piece.Color, move.StartX, move.StartY))
                {
                    return false;
                }

                // Check for normal move 
                if (Math.Abs(dx) == 1 && Math.Abs(dy) == 1)
                {
                    // Ensure no normal moves are allowed if captures are available
                    if (HasAvailableCaptures(currentTurn))
                    {
                        return false;
                    }
                    return true;
                }

                // Check for capturing move 
                if (Math.Abs(dx) == 2 && Math.Abs(dy) == 2)
                {
                    int midX = (move.StartX + move.EndX) / 2;
                    int midY = (move.StartY + move.EndY) / 2;

                    Piece midPiece = Squares[midX, midY];
                    if (midPiece.Color != PieceColor.None && midPiece.Color != piece.Color)
                    {
                        return true;
                    }
                }
            }

            // King piece moves
            else if (piece.Type == PieceType.King)
            {
                if (Math.Abs(dx) == Math.Abs(dy))
                {
                    int steps = Math.Abs(dx);
                    int stepX = dx / steps;
                    int stepY = dy / steps;
                    int opponentCount = 0;
                    
                    for (int i = 1; i < steps; i++)
                    {
                        int checkX = move.StartX + stepX * i;
                        int checkY = move.StartY + stepY * i;
                        Piece checkPiece = Squares[checkX, checkY];

                        if (checkPiece.Color == piece.Color)
                        {
                            return false;
                        }
                        if (checkPiece.Color != PieceColor.None && checkPiece.Color != piece.Color)
                        {
                            opponentCount++;
                            if (opponentCount > 1)
                            {
                                return false;
                            }
                        }
                    }

                    if (opponentCount == 1)
                    {
                        
                        int captureX = move.EndX;
                        int captureY = move.EndY;
                        if (captureX >= 0 && captureX < Board.Size && captureY >= 0 && captureY < Board.Size && Squares[captureX, captureY].Color == PieceColor.None)
                        {
                            return true;
                        }
                        return false;
                    }
                    if (opponentCount == 0)
                    {
                        if (HasAvailableCaptures(currentTurn)) { return false; }
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ApplyMove(Move move)
        {
            bool flag = true;
            Piece piece = Squares[move.StartX, move.StartY];
            Squares[move.StartX, move.StartY] = new Piece(PieceColor.None, PieceType.Man);
            Squares[move.EndX, move.EndY] = piece;

            int dx = move.EndX - move.StartX;
            int dy = move.EndY - move.StartY;

            // Capture for regular piece
            if (piece.Type == PieceType.Man && Math.Abs(dx) == 2 && Math.Abs(dy) == 2)
            {
                int midX = (move.StartX + move.EndX) / 2;
                int midY = (move.StartY + move.EndY) / 2;
                Squares[midX, midY] = new Piece(PieceColor.None, PieceType.Man);

                if (HasCapture(piece.Color, move.EndX, move.EndY))
                {
                    flag = false; // Wait for the player to make the additional capture
                }

            }

            // Capture for King piece
            if (piece.Type == PieceType.King && Math.Abs(dx) == Math.Abs(dy))
            {
                int steps = Math.Abs(dx);
                int stepX = dx / steps;
                int stepY = dy / steps;
                for (int i = 1; i < steps; i++)
                {
                    int checkX = move.StartX + stepX * i;
                    int checkY = move.StartY + stepY * i;
                    Piece checkPiece = Squares[checkX, checkY];

                    if (checkPiece.Color != PieceColor.None && checkPiece.Color != piece.Color)
                    {
                        Squares[checkX, checkY] = new Piece(PieceColor.None, PieceType.Man);
                        if (HasCapture(piece.Color, move.EndX, move.EndY))
                        {
                            flag = false; // Wait for the player to make the additional capture
                        }
                        break;
                    }
                }
            }

            if (piece.Color == PieceColor.Red && move.EndY == 7)
            {
                Squares[move.EndX, move.EndY] = new Piece(PieceColor.Red, PieceType.King);
            }
            if (piece.Color == PieceColor.Black && move.EndY == 0)
            {
                Squares[move.EndX, move.EndY] = new Piece(PieceColor.Black, PieceType.King);
            }

            return flag;
        }

        public bool HasCapture(PieceColor color, int x, int y)
        {
            Piece piece = Squares[x, y];
            if (piece.Type == PieceType.Man)
            {
                int[,] directions = new int[,] { { 2, 2 }, { 2, -2 }, { -2, 2 }, { -2, -2 } };

                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    int dx = directions[i, 0];
                    int dy = directions[i, 1];
                    int newX = x + dx;
                    int newY = y + dy;

                    if (IsInsideBoard(newX, newY) && IsValidCaptureMove(new Move(x, y, newX, newY), color))
                    {
                        return true;
                    }
                }

                return false;
            }
            else if (piece.Type == PieceType.King)
            {
                int[,] directions = new int[,] { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };

                for (int i = 0; i < 4; i++)
                {
                    int stepX = directions[i, 0];
                    int stepY = directions[i, 1];
                    int newX = x;
                    int newY = y;

                    while (IsInsideBoard(newX + stepX, newY + stepY))
                    {
                        newX += stepX;
                        newY += stepY;

                        if (IsInsideBoard(newX + stepX, newY + stepY) && IsValidCaptureMove(new Move(x, y, newX, newY), color))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public bool IsValidCaptureMove(Move move, PieceColor color)
        {
            Piece piece = Squares[move.StartX, move.StartY];
            Piece targetPiece = Squares[move.EndX, move.EndY];

            if (piece.Color != color || targetPiece.Color != PieceColor.None)
            {
                return false;
            }

            int dx = move.EndX - move.StartX;
            int dy = move.EndY - move.StartY;

            if (piece.Type == PieceType.Man)
            {
                if (Math.Abs(dx) == 2 && Math.Abs(dy) == 2)
                {
                    int midX = (move.StartX + move.EndX) / 2;
                    int midY = (move.StartY + move.EndY) / 2;

                    Piece midPiece = Squares[midX, midY];
                    if (midPiece.Color != PieceColor.None && midPiece.Color != piece.Color)
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (piece.Type == PieceType.King)
            {
                if (Math.Abs(dx) == Math.Abs(dy))
                {
                    int stepX = dx / Math.Abs(dx);
                    int stepY = dy / Math.Abs(dy);
                    int midX = move.StartX + stepX;
                    int midY = move.StartY + stepY;
                    int opponentCount = 0;

                    while (midX != move.EndX && midY != move.EndY)
                    {
                        Piece midPiece = Squares[midX, midY];
                        if (midPiece.Color == color)
                        {
                            return false;
                        }
                        if (midPiece.Color != PieceColor.None && midPiece.Color != color)
                        {
                            opponentCount++;
                            if (opponentCount > 1)
                            {
                                return false;
                            }
                        }

                        midX += stepX;
                        midY += stepY;
                    }

                    return opponentCount == 1;
                }
                return false;
            }

            return false;
        }

        public bool IsInsideBoard(int x, int y)
        {
            return x >= 0 && x < Size && y >= 0 && y < Size;
        }

        public bool HasAvailableCaptures(PieceColor color)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (Squares[x, y].Color == color && HasCapture(color, x, y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Move GetBestMove(PieceColor color, Board board, string algorithm)
        {
            Random rnd = new Random();
            if (algorithm == "Alpha-Beta")
            {
                return ai.GetBestMoveAlphaBeta(board, color);
            }
            else if (algorithm == "Combined")
            {
                return ai.GetBestMoveCombined(board, color, rnd.Next(1,7));
            }
            else
            {
                return ai.GetBestMoveMinimax(board, color);
            }
        }

        public bool IsGameOver(Board board)
        {
            return !ai.GetAllPossibleMoves(board, PieceColor.Red).Any() || !ai.GetAllPossibleMoves(board, PieceColor.Black).Any();
        }

        public PieceColor GetWinner()
        {
            int redPieces = CountPieces(PieceColor.Red);
            int blackPieces = CountPieces(PieceColor.Black);

            if (redPieces == 0)
            {
                return PieceColor.Black;
            }
            else if (blackPieces == 0)
            {
                return PieceColor.Red;
            }
            else
            {
                return PieceColor.None; // In case of a draw
            }
        }

        private int CountPieces(PieceColor color)
        {
            int count = 0;
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (Squares[x, y].Color == color)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void Reset()
        {
            Squares = new Piece[Size, Size];

            // Place pieces in initial positions
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if ((x + y) % 2 != 0 && y < 3)
                    {
                        Squares[x, y] = new Piece(PieceColor.Red, PieceType.Man);
                    }
                    else if ((x + y) % 2 != 0 && y > 4)
                    {
                        Squares[x, y] = new Piece(PieceColor.Black, PieceType.Man);
                    }
                    else
                    {
                        Squares[x, y] = new Piece(PieceColor.None, PieceType.None);
                    }
                }
            }
            


        }
    }
}