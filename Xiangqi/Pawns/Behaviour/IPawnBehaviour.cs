using System.Collections.Generic;

namespace Xiangqi.Pawns.Behaviour
{
    public interface IPawnBehaviour
    {
        bool CheckMovement(int fromX, int fromY, int toX, int toY, IPawn[,] gameBoardPositions);

        bool[,] GetPossibleMovements(int fromX, int fromY, IPawn[,] gameBoardPositions);

        List<(int X, int Y)> GetPossibleMovementsByVector(int fromX, int fromY, IPawn[,] gameBoardPositions);
    }
}
