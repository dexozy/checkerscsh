using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CheckersGame
{
    public class AI
    {
        #region
        public Move GetBestMoveCombined(Board board, PieceColor color, int depth)
        {
            if (depth <= 2) // Use Minimax for shallow depths
            {
                return GetBestMoveMinimax(board, color);
            }
            else // Use Alpha-Beta for deeper searches
            {
                return GetBestMoveAlphaBeta(board, color);
            }
        }

        public Move GetBestMoveMinimax(Board board, PieceColor color)
        {
            int depth = 5;
            int bestValue = int.MinValue;
            Move bestMove = null;
            List<Move> possibleMoves = GetAllPossibleMoves(board, color);

            foreach (var move in possibleMoves)
            {
                Board newBoard = new Board(board);
                newBoard.ApplyMove(move);
                int moveValue = Minimax(newBoard, depth - 1, false, color);

                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = move;
                }
            }


            return bestMove;
        }

        private int Minimax(Board board, int depth, bool maximizingPlayer, PieceColor color)
        {
            if (depth == 0 || IsGameOver(board))
            {
                return EvaluateBoard(board, color);
            }

            List<Move> possibleMoves = GetAllPossibleMoves(board, maximizingPlayer ? color : OpponentColor(color));

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (var move in possibleMoves)
                {
                    Board newBoard = new Board(board);
                    newBoard.ApplyMove(move);
                    int eval = Minimax(newBoard, depth - 1, false, color);
                    maxEval = Math.Max(maxEval, eval);
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var move in possibleMoves)
                {
                    Board newBoard = new Board(board);
                    newBoard.ApplyMove(move);
                    int eval = Minimax(newBoard, depth - 1, true, color);
                    minEval = Math.Min(minEval, eval);
                }
                return minEval;
            }
        }

        private int EvaluateBoard(Board board, PieceColor color)
        {
            int score = 0;

            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    Piece piece = board.Squares[x, y];
                    if (piece.Color == color)
                    {
                        score += piece.Type == PieceType.Man ? 10 : 30;
                    }
                    else if (piece.Color == OpponentColor(color))
                    {
                        score -= piece.Type == PieceType.Man ? 10 : 30;
                    }
                }
            }

            return score;
        }
        #endregion

        public Move GetBestMoveAlphaBeta(Board board, PieceColor color)
        {
            int depth = 5;
            int bestValue = int.MinValue;
            Move bestMove = null;
            List<Move> possibleMoves = GetAllPossibleMoves(board, color);

            foreach (var move in possibleMoves)
            {
                Board newBoard = new Board(board);
                newBoard.ApplyMove(move);
                int moveValue = AlphaBeta(newBoard, depth - 1, int.MinValue, int.MaxValue, false, color);

                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int AlphaBeta(Board board, int depth, int alpha, int beta, bool maximizingPlayer, PieceColor color)
        {
            if (depth == 0 || IsGameOver(board))
            {
                return EvaluateBoard(board, color);
            }

            List<Move> possibleMoves = GetAllPossibleMoves(board, maximizingPlayer ? color : OpponentColor(color));

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (var move in possibleMoves)
                {
                    Board newBoard = new Board(board);
                    newBoard.ApplyMove(move);
                    int eval = AlphaBeta(newBoard, depth - 1, alpha, beta, false, color);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var move in possibleMoves)
                {
                    Board newBoard = new Board(board);
                    newBoard.ApplyMove(move);
                    int eval = AlphaBeta(newBoard, depth - 1, alpha, beta, true, color);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return minEval;
            }
        }

        private PieceColor OpponentColor(PieceColor color)
        {
            return color == PieceColor.Red ? PieceColor.Black : PieceColor.Red;
        }

        public List<Move> GetAllPossibleMoves(Board board, PieceColor color)
        {
            List<Move> allMoves = new List<Move>();
            bool hasCapture = false;

            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    Piece piece = board.Squares[x, y];
                    if (piece.Color == color)
                    {
                        List<Move> pieceMoves = GetValidMoves(board, x, y, piece.Color);
                        foreach (Move move in pieceMoves)
                        {
                            if (board.IsValidMove(move, color))
                            {
                                if (board.IsValidCaptureMove(move, color))
                                {
                                    hasCapture = true;
                                    allMoves.Add(move);
                                }
                                else if (!hasCapture)
                                {
                                    allMoves.Add(move);
                                }
                            }
                        }
                    }
                }
            }

            // If there are any capture moves, only return those
            if (hasCapture)
            {
                allMoves.RemoveAll(move => !board.IsValidCaptureMove(move, color));
            }

            return allMoves;
        }

        public List<Move> GetPieceValidMoves(Board board, int x, int y)
        {
            List<Move> moves = new List<Move>();
            Piece piece = board.Squares[x, y];

            if (piece.Type == PieceType.Man)
            {
                int direction = (piece.Color == PieceColor.Red) ? 1 : -1;
                int[,] directions = new int[,] { { 1, direction }, { -1, direction }, { 2, 2 * direction }, { -2, 2 * direction } };

                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    int dx = directions[i, 0];
                    int dy = directions[i, 1];
                    int newX = x + dx;
                    int newY = y + dy;
                    if (board.IsInsideBoard(newX, newY))
                    {
                        moves.Add(new Move(x, y, newX, newY));
                    }
                }
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

                    while (board.IsInsideBoard(newX + stepX, newY + stepY))
                    {
                        newX += stepX;
                        newY += stepY;
                        moves.Add(new Move(x, y, newX, newY));
                        if (board.Squares[newX, newY].Color == piece.Color)
                        {
                            break;
                        }
                    }
                }
            }

            return moves;
        }

        public List<Move> GetValidMoves(Board board, int startX, int startY, PieceColor color)
        {
            List<Move> moves = new List<Move>();
            Piece piece = board.Squares[startX, startY];
            if (piece.Type == PieceType.Man)
            {
                int[] dx = { 1, -1, -1, 1 };
                int[] dy = { 1, -1, 1, -1 };

                for (int i = 0; i < dx.Length; i++)
                {
                    int endX = startX + dx[i];
                    int endY = startY + dy[i];

                    // Check if the target position is within bounds
                    if (endX >= 0 && endX < Board.Size && endY >= 0 && endY < Board.Size)
                    {
                        Move move = new Move(startX, startY, endX, endY);
                        if (board.IsValidMove(move, color))
                        {
                            moves.Add(move);
                        }
                    }
                }
                int[] dx1 = { 2, -2, -2, 2 };
                int[] dy1 = { 2, -2, 2, -2 };

                for (int i = 0; i < dx1.Length; i++)
                {
                    int endX = startX + dx1[i];
                    int endY = startY + dy1[i];

                    if (Math.Abs(endX - startX) == 2 && Math.Abs(endY - startY) == 2)
                    {
                        if (endX >= 0 && endX < Board.Size && endY >= 0 && endY < Board.Size)
                        {
                            int midX = (startX + endX) / 2;
                            int midY = (startY + endY) / 2;
                            Piece midPiece = board.Squares[midX, midY];

                            if (midPiece.Color != PieceColor.None && midPiece.Color != piece.Color)
                            {
                                Move move = new Move(startX, startY, endX, endY);
                                if (board.IsValidMove(move, color))
                                {
                                    moves.Add(move);
                                }
                            }
                        }
                    }
                }
            }
            bool CaptureIs = false;
            if (piece.Type == PieceType.King)
            {
                int[] longDx = { 1, -1 };
                int[] longDy = { 1, -1 };

                foreach (int dxDir in longDx)
                {
                    foreach (int dyDir in longDy)
                    {
                        int x = startX + dxDir;
                        int y = startY + dyDir;
                        
                        while (x >= 0 && x < Board.Size && y >= 0 && y < Board.Size)
                        {
                            Move move = new Move(startX, startY, x, y);
                            if (board.IsValidMove(move, color))
                            {
                                if (board.IsValidCaptureMove(move, color))
                                {
                                    CaptureIs = true;
                                }
                                moves.Add(move);
                            }
                            x += dxDir;
                            y += dyDir;
                        }
                    }
                }
            }

            if (CaptureIs)
            {
                moves.RemoveAll(move => !board.IsValidCaptureMove(move, color));
            }


            return moves;
        }   

        private bool IsGameOver(Board board)
        {
            bool hasRed = false;
            bool hasBlack = false;

            for (int y = 0; y < Board.Size; y++)
            {
                for (int x = 0; x < Board.Size; x++)
                {
                    if (board.Squares[x, y].Color == PieceColor.Red)
                    {
                        hasRed = true;
                    }
                    if (board.Squares[x, y].Color == PieceColor.Black)
                    {
                        hasBlack = true;
                    }
                }
            }

            return !hasRed || !hasBlack;
        }
    }
}