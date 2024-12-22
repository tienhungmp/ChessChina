using System.Collections.Generic;

namespace Xiangqi.Pawns.Behaviour
{
    public class GeneralBehaviour : IPawnBehaviour
    {
        public bool CheckMovement(int fromX, int fromY, int toX, int toY, IPawn[,] gameBoardPositions)
        {
            PlayerSide currentSide = gameBoardPositions[fromX, fromY].GetPlayerSide();
            int rowMin = currentSide == PlayerSide.BLACK ? 0 : 7;
            int rowMax = currentSide == PlayerSide.BLACK ? 2 : 9;

            //Check if movement is inside palace.
            if (toY <= 5 && toY >= 3 && toX >= rowMin && toX <= rowMax)
            {
                // Check top.
                if ((fromX - 1) == toX && fromY == toY)
                {
                    return true;
                }

                // Check left.
                if (fromX == toX && (fromY - 1) == toY)
                {
                    return true;
                }

                // Check bottom.
                if ((fromX + 1) == toX && fromY == toY)
                {
                    return true;
                }

                // Check right.
                if (fromX == toX && (fromY + 1) == toY)
                {
                    return true;
                }
            }

            return false;
        }

        public bool[,] GetPossibleMovements(int fromX, int fromY, IPawn[,] gameBoardPositions)
        {
            PlayerSide currentSide = gameBoardPositions[fromX, fromY].GetPlayerSide();
            int rowMin = currentSide == PlayerSide.BLACK ? 0 : 7;
            int rowMax = currentSide == PlayerSide.BLACK ? 2 : 9;

            bool[,] possiblePositionsOnGameBoard = new bool[10, 9];

            int possibleX;
            int possibleY;

            // Check top.
            possibleX = (fromX - 1);
            possibleY = fromY;

            //Check if movement is inside palace.
            if (possibleY <= 5 && possibleY >= 3 && possibleX >= rowMin && possibleX <= rowMax)
            {
                if (gameBoardPositions[possibleX, possibleY].GetPlayerSide() == PlayerSide.EMPTY || gameBoardPositions[possibleX, possibleY].GetPlayerSide() != currentSide)
                {
                    possiblePositionsOnGameBoard[possibleX, possibleY] = true;
                }
            }

            // Check left.
            possibleX = fromX;
            possibleY = (fromY - 1);

            //Check if movement is inside palace.
            if (possibleY <= 5 && possibleY >= 3 && possibleX >= rowMin && possibleX <= rowMax)
            {
                if (gameBoardPositions[possibleX, possibleY].GetPlayerSide() == PlayerSide.EMPTY || gameBoardPositions[possibleX, possibleY].GetPlayerSide() != currentSide)
                {
                    possiblePositionsOnGameBoard[possibleX, possibleY] = true;
                }
            }

            // Check bottom.
            possibleX = (fromX + 1);
            possibleY = fromY;

            //Check if movement is inside palace.
            if (possibleY <= 5 && possibleY >= 3 && possibleX >= rowMin && possibleX <= rowMax)
            {
                if (gameBoardPositions[possibleX, possibleY].GetPlayerSide() == PlayerSide.EMPTY || gameBoardPositions[possibleX, possibleY].GetPlayerSide() != currentSide)
                {
                    possiblePositionsOnGameBoard[possibleX, possibleY] = true;
                }
            }

            // Check right.
            possibleX = fromX;
            possibleY = (fromY + 1);

            //Check if movement is inside palace.
            if (possibleY <= 5 && possibleY >= 3 && possibleX >= rowMin && possibleX <= rowMax)
            {
                if (gameBoardPositions[possibleX, possibleY].GetPlayerSide() == PlayerSide.EMPTY || gameBoardPositions[possibleX, possibleY].GetPlayerSide() != currentSide)
                {
                    possiblePositionsOnGameBoard[possibleX, possibleY] = true;
                }
            }

            return possiblePositionsOnGameBoard;
        }

        public List<(int X, int Y)> GetPossibleMovementsByVector(int fromX, int fromY, IPawn[,] gameBoardPositions)
        {
            List<(int X, int Y)> possiblePositions = new List<(int X, int Y)>();

            PlayerSide currentSide = gameBoardPositions[fromX, fromY].GetPlayerSide();
            int rowMin = currentSide == PlayerSide.BLACK ? 0 : 7;
            int rowMax = currentSide == PlayerSide.BLACK ? 2 : 9;

            int possibleX;
            int possibleY;

            // Check top.
            possibleX = (fromX - 1);
            possibleY = fromY;

            // Check if movement is inside palace.
            if (possibleY <= 5 && possibleY >= 3 && possibleX >= rowMin && possibleX <= rowMax)
            {
                if (gameBoardPositions[possibleX, possibleY].GetPlayerSide() == PlayerSide.EMPTY || gameBoardPositions[possibleX, possibleY].GetPlayerSide() != currentSide)
                {
                    possiblePositions.Add((possibleX, possibleY));
                }
            }

            // Check left.
            possibleX = fromX;
            possibleY = (fromY - 1);

            // Check if movement is inside palace.
            if (possibleY <= 5 && possibleY >= 3 && possibleX >= rowMin && possibleX <= rowMax)
            {
                if (gameBoardPositions[possibleX, possibleY].GetPlayerSide() == PlayerSide.EMPTY || gameBoardPositions[possibleX, possibleY].GetPlayerSide() != currentSide)
                {
                    possiblePositions.Add((possibleX, possibleY));
                }
            }

            // Check bottom.
            possibleX = (fromX + 1);
            possibleY = fromY;

            // Check if movement is inside palace.
            if (possibleY <= 5 && possibleY >= 3 && possibleX >= rowMin && possibleX <= rowMax)
            {
                if (gameBoardPositions[possibleX, possibleY].GetPlayerSide() == PlayerSide.EMPTY || gameBoardPositions[possibleX, possibleY].GetPlayerSide() != currentSide)
                {
                    possiblePositions.Add((possibleX, possibleY));
                }
            }

            // Check right.
            possibleX = fromX;
            possibleY = (fromY + 1);

            // Check if movement is inside palace.
            if (possibleY <= 5 && possibleY >= 3 && possibleX >= rowMin && possibleX <= rowMax)
            {
                if (gameBoardPositions[possibleX, possibleY].GetPlayerSide() == PlayerSide.EMPTY || gameBoardPositions[possibleX, possibleY].GetPlayerSide() != currentSide)
                {
                    possiblePositions.Add((possibleX, possibleY));
                }
            }

            return possiblePositions;
        }
    }
}
