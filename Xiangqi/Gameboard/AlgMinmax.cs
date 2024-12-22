//using System;
//using Xiangqi.Pawns;

//namespace Xiangqi.Gameboard
//{
//    public class MinimaxAI
//    {
//        private readonly int maxDepth;
//        private readonly PlayerSide botSide;

//        public MinimaxAI(PlayerSide botSide, int maxDepth = 3)
//        {
//            this.botSide = botSide;
//            this.maxDepth = maxDepth;
//        }

//        public (int fromRow, int fromCol, int toRow, int toCol) GetBestMove(IPawn[,] board)
//        {
//            int bestValue = int.MinValue;
//            (int fromRow, int fromCol, int toRow, int toCol) bestMove = (-1, -1, -1, -1);

//            for (int row = 0; row < 10; row++)
//            {
//                for (int col = 0; col < 9; col++)
//                {
//                    IPawn pawn = board[row, col];
//                    if (pawn != null && pawn.GetPossibleMovementsByVector(row, col, board) != null  && pawn.GetPlayerSide() == botSide)
//                    {
//                        foreach (var move in pawn.GetPossibleMovementsByVector(row, col, board))
//                        {
//                            IPawn targetPawn = board[move.row, move.col];
//                            // Bỏ qua nước đi nếu ô mục tiêu có quân cùng phe
//                            if (targetPawn != null && targetPawn.GetPlayerSide() == botSide)
//                            {
//                                continue;
//                            }

//                            // Giả lập nước đi
//                            IPawn originalPawn = board[move.row, move.col];
//                            board[move.row, move.col] = pawn;
//                            board[row, col] = new EmptyPawn();

//                            // Gọi Minimax
//                            int moveValue = Minimax(board, maxDepth - 1, false, botSide, int.MinValue, int.MaxValue);

//                            // Hoàn tác nước đi
//                            board[row, col] = pawn;
//                            board[move.row, move.col] = originalPawn;

//                            if (moveValue > bestValue)
//                            {
//                                bestValue = moveValue;
//                                bestMove = (row, col, move.row, move.col);
//                            }
//                        }
//                    }
//                }
//            }

//            return bestMove;
//        }

//        private int Minimax(IPawn[,] board, int depth, bool isMaximizingPlayer, PlayerSide currentPlayer, int alpha, int beta)
//        {
//            if (depth == 0 || IsGameOver(board, currentPlayer))
//            {
//                return EvaluateBoard(board, currentPlayer);
//            }

//            if (isMaximizingPlayer)
//            {
//                int maxEval = int.MinValue;
//                for (int row = 0; row < 10; row++)
//                {
//                    for (int col = 0; col < 9; col++)
//                    {
//                        IPawn pawn = board[row, col];
//                        if (pawn != null && pawn.GetPossibleMovementsByVector(row, col, board) != null && pawn.GetPlayerSide() == currentPlayer)
//                        {
//                            foreach (var move in pawn.GetPossibleMovementsByVector(row, col, board))
//                            {
//                                IPawn targetPawn = board[move.row, move.col];
//                                // Bỏ qua nước đi nếu ô mục tiêu có quân cùng phe
//                                if (targetPawn != null && targetPawn.GetPlayerSide() == currentPlayer)
//                                {
//                                    continue;
//                                }

//                                // Giả lập nước đi
//                                IPawn originalPawn = board[move.row, move.col];
//                                board[move.row, move.col] = pawn;
//                                board[row, col] = new EmptyPawn();

//                                // Gọi Minimax đệ quy
//                                int eval = Minimax(board, depth - 1, false, GetOpponentSide(currentPlayer), alpha, beta);

//                                // Hoàn tác nước đi
//                                board[row, col] = pawn;
//                                board[move.row, move.col] = originalPawn;

//                                maxEval = Math.Max(maxEval, eval);
//                                alpha = Math.Max(alpha, eval);
//                                if (beta <= alpha)
//                                {
//                                    break; // Alpha-beta pruning
//                                }
//                            }
//                        }
//                    }
//                }
//                return maxEval;
//            }
//            else
//            {
//                int minEval = int.MaxValue;
//                for (int row = 0; row < 10; row++)
//                {
//                    for (int col = 0; col < 9; col++)
//                    {
//                        IPawn pawn = board[row, col];
//                        if (pawn != null && pawn.GetPossibleMovementsByVector(row, col, board) !=null && pawn.GetPlayerSide() != currentPlayer)
//                        {
//                            foreach (var move in pawn.GetPossibleMovementsByVector(row, col, board))
//                            {
//                                IPawn targetPawn = board[move.row, move.col];
//                                // Bỏ qua nước đi nếu ô mục tiêu có quân cùng phe
//                                if (targetPawn != null && targetPawn.GetPlayerSide() != currentPlayer)
//                                {
//                                    continue;
//                                }

//                                // Giả lập nước đi
//                                IPawn originalPawn = board[move.row, move.col];
//                                board[move.row, move.col] = pawn;
//                                board[row, col] = new EmptyPawn();

//                                // Gọi Minimax đệ quy
//                                int eval = Minimax(board, depth - 1, true, GetOpponentSide(currentPlayer), alpha, beta);

//                                // Hoàn tác nước đi
//                                board[row, col] = pawn;
//                                board[move.row, move.col] = originalPawn;

//                                minEval = Math.Min(minEval, eval);
//                                beta = Math.Min(beta, eval);
//                                if (beta <= alpha)
//                                {
//                                    break; // Alpha-beta pruning
//                                }
//                            }
//                        }
//                    }
//                }
//                return minEval;
//            }
//        }

//        private int EvaluateBoard(IPawn[,] board, PlayerSide currentPlayer)
//        {
//            int score = 0;
//            for (int row = 0; row < 10; row++)
//            {
//                for (int col = 0; col < 9; col++)
//                {
//                    if (board[row, col] != null)
//                    {
//                        score += board[row, col].GetEvaluationScore(currentPlayer);
//                    }
//                }
//            }
//            return score;
//        }

//        private bool IsGameOver(IPawn[,] board, PlayerSide currentPlayer)
//        {
//            return false; // Logic kiểm tra kết thúc trò chơi
//        }

//        private PlayerSide GetOpponentSide(PlayerSide side)
//        {
//            return side == PlayerSide.BLACK ? PlayerSide.RED : PlayerSide.BLACK;
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using Xiangqi.Pawns;

namespace Xiangqi.Gameboard
{
    public class MinimaxAI
    {
        private readonly int maxDepth;
        private readonly PlayerSide botSide;

        public MinimaxAI(PlayerSide botSide, int maxDepth = 3)
        {
            this.botSide = botSide;
            this.maxDepth = maxDepth;
        }

        public (int fromRow, int fromCol, int toRow, int toCol) GetBestMove(IPawn[,] board)
        {
            int bestValue = int.MinValue;
            (int fromRow, int fromCol, int toRow, int toCol) bestMove = (-1, -1, -1, -1);

            var possibleMoves = GenerateMoves(board, botSide);

            foreach (var move in possibleMoves)
            {
                ExecuteMove(board, move);
                int moveValue = Minimax(board, maxDepth - 1, false, GetOpponentSide(botSide), int.MinValue, int.MaxValue);
                UndoMove(board, move);

                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = (move.FromRow, move.FromCol, move.ToRow, move.ToCol);
                }
            }

            return bestMove;
        }

        private int Minimax(IPawn[,] board, int depth, bool isMaximizingPlayer, PlayerSide currentPlayer, int alpha, int beta)
        {
            if (depth == 0 || IsGameOver(board, currentPlayer))
            {
                return EvaluateBoard(board, botSide);
            }

            var possibleMoves = GenerateMoves(board, currentPlayer);

            if (isMaximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (var move in possibleMoves)
                {
                    ExecuteMove(board, move);
                    int eval = Minimax(board, depth - 1, false, GetOpponentSide(currentPlayer), alpha, beta);
                    UndoMove(board, move);

                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break; // Alpha-beta pruning
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var move in possibleMoves)
                {
                    ExecuteMove(board, move);
                    int eval = Minimax(board, depth - 1, true, GetOpponentSide(currentPlayer), alpha, beta);
                    UndoMove(board, move);

                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break; // Alpha-beta pruning
                    }
                }
                return minEval;
            }
        }

        private int EvaluateBoard(IPawn[,] board, PlayerSide currentPlayer)
        {
            int score = 0;
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] != null)
                    {
                        int pieceScore = board[row, col].GetEvaluationScore(currentPlayer);

                        // Ưu tiên vị trí
                        if (board[row, col].GetPlayerSide() == currentPlayer)
                        {
                            pieceScore += GetPositionalValue(row, col, board[row, col]);
                        }

                        score += pieceScore;
                    }
                }
            }
            return score;
        }

        private int GetPositionalValue(int row, int col, IPawn pawn)
        {
            if (pawn is Rook) // Xe
            {
                return (row >= 4 && row <= 5) ? 10 : 5;
            }
            else if (pawn is Cannon) // Pháo
            {
                return 8;
            }
            else if (pawn is Soldier) // Tốt
            {
                return (row > 4) ? 5 : 2;
            }
            return 0;
        }

        private bool IsGameOver(IPawn[,] board, PlayerSide currentPlayer)
        {
            // Kiểm tra điều kiện kết thúc game (ví dụ: tướng bị bắt)
            return false;
        }

        private PlayerSide GetOpponentSide(PlayerSide side)
        {
            return side == PlayerSide.BLACK ? PlayerSide.RED : PlayerSide.BLACK;
        }

        private List<Move> GenerateMoves(IPawn[,] board, PlayerSide currentPlayer)
        {
            var moves = new List<Move>();
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    IPawn pawn = board[row, col];
                    if (pawn != null && pawn.GetPlayerSide() == currentPlayer)
                    {
                        var possibleMoves = pawn.GetPossibleMovementsByVector(row, col, board);
                        if (possibleMoves != null)
                        {
                            foreach (var move in possibleMoves)
                            {
                                moves.Add(new Move(row, col, move.row, move.col, board[move.row, move.col]));
                            }
                        }
                    }
                }
            }
            return moves.OrderByDescending(m => EvaluateMove(m, board)).ToList();
        }

        private int EvaluateMove(Move move, IPawn[,] board)
        {
            IPawn target = move.CapturedPawn;
            return target != null ? target.GetEvaluationScore(botSide) : 0;
        }

        private void ExecuteMove(IPawn[,] board, Move move)
        {
            board[move.ToRow, move.ToCol] = board[move.FromRow, move.FromCol];
            board[move.FromRow, move.FromCol] = new EmptyPawn();
        }

        private void UndoMove(IPawn[,] board, Move move)
        {
            board[move.FromRow, move.FromCol] = board[move.ToRow, move.ToCol];
            board[move.ToRow, move.ToCol] = move.CapturedPawn;
        }

        private class Move
        {
            public int FromRow { get; }
            public int FromCol { get; }
            public int ToRow { get; }
            public int ToCol { get; }
            public IPawn CapturedPawn { get; }

            public Move(int fromRow, int fromCol, int toRow, int toCol, IPawn capturedPawn = null)
            {
                FromRow = fromRow;
                FromCol = fromCol;
                ToRow = toRow;
                ToCol = toCol;
                CapturedPawn = capturedPawn;
            }
        }
    }
}
