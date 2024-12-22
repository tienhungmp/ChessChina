﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiangqi.Gameboard;
using Xiangqi.Gamescreen;
using Xiangqi.Pawns;

namespace Xiangqi
{
    public enum PlayerSide { BLACK, RED, EMPTY}
    public enum GameState { PLAYING, CHECKMATED, BREAK}

    public class GameBoard
    {
        private Bitmap gameBoardImg;
        private IPawn[,] gameBoardPositions;
        private GameState gameState;

        private int screenWidthMiddle;
        private int screenHeightMiddle;

        private int pawnSizeWidth;
        private int pawnSizeHeight;

        private int spaceBetweenPawnsX;
        private int spaceBetweenPawnsY;

        private int imageType;

        private Rectangle selectedPawn;
        private int selectedCol;
        private int selectedRow;
        private PlayerSide previousPlayer;
        private bool[,] selectedPawnPossibleMovements;

        private IPawn blackGeneral;
        private int colBlackGeneral;
        private int rowBlackGeneral;

        private IPawn redGeneral;
        private int colRedGeneral;
        private int rowRedGeneral;

        private IPawn threateningPawn;
        private int colThreateningPawn;
        private int rowThreateningPawn;

        private bool surrender;

        public PlayerSide GetPreviousPlayer()
        {
            return previousPlayer;
        }

        public GameBoard()
        {
            gameBoardImg = new Bitmap(Xiangqi.Properties.Resources.smboard);
            gameBoardPositions = new IPawn[10, 9];

            pawnSizeWidth = PawnBitmapCollection.WIDTH;
            pawnSizeHeight = PawnBitmapCollection.HEIGHT;

            spaceBetweenPawnsX = 12;
            spaceBetweenPawnsY = 12;

            imageType = 0;

            selectedPawn = Rectangle.Empty;
            previousPlayer = PlayerSide.RED;
            surrender = false;

            PlacePawns();
            
            screenWidthMiddle = (GameScreen.width / 2) - (gameBoardImg.Width / 2);
            screenHeightMiddle = (GameScreen.height / 2) - (gameBoardImg.Height / 2);
        }

        public void Paint(PaintEventArgs e, Label l)
        {
            string currentSide = previousPlayer == PlayerSide.RED ? PlayerSide.BLACK.ToString() : PlayerSide.RED.ToString();
            l.Text = "Current side on set: " + currentSide;
            PaintBoardImg(e);
            PaintPawns(e);
        }

        private void PaintBoardImg(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawImage(gameBoardImg, new Rectangle(screenWidthMiddle, screenHeightMiddle, gameBoardImg.Width, gameBoardImg.Height));
        }

        // TODO: Mark the right threathening pawn. 
        // TODO: Mark possible movement points as red if blocked by threat.
        private void PaintPawns(PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            IPawn checkedGeneral = null;
            string winningPlayer = string.Empty;

            int locX = screenWidthMiddle - (pawnSizeWidth / 2);
            int locY = screenHeightMiddle - (pawnSizeHeight / 2);


            int marginX = pawnSizeWidth + spaceBetweenPawnsX;
            int marginY = pawnSizeHeight + spaceBetweenPawnsY;

            if (surrender == true)
            {
                surrender = false;
                DialogResult result = MessageBox.Show("Bạn thua bot", "Thông báo", MessageBoxButtons.OK);

                // Kiểm tra kết quả khi người dùng nhấn OK
                if (result == DialogResult.OK)
                {
                    // Thực hiện hàm khác khi nhấn OK
                    NewGame();
                }
            }

            // If black general is checked.
            if (GeneralIsChecked(blackGeneral))
            {
                checkedGeneral = blackGeneral;
                winningPlayer = "RED";
            }

            // If red general is checked.
            if (GeneralIsChecked(redGeneral))
            {
                checkedGeneral = redGeneral;
                winningPlayer = "BLACK";
            }

            // Mark the checked general and check if the general is checkmated.
            if (checkedGeneral != null)
            {
                g.DrawImage(PawnBitmapCollection.generalChecked, checkedGeneral.GetRec());

                if (GeneralIsCheckMated(checkedGeneral))
                {
                    gameState = GameState.CHECKMATED;
                    g.DrawString("Checkmate, " + winningPlayer + " has won!!!\n\nStart a new game!", new Font(FontFamily.GenericSansSerif, 16, FontStyle.Regular),
                    new SolidBrush(Color.Black), 0, 60);
                    if(winningPlayer == "BLACK")
                    {
                         new Login().OnPlayerWin();
                    } else
                    {
                        MessageBox.Show($"Bạn thua bot");
                    }
                }
            }

            // Draw every pawn on the board and updating its X and Y and marking the possible positions the pawn can move to.
            for (int row = 0; row < 10; row++)
            {
                for(int col = 0; col < 9; col++)
                {
                    if (gameBoardPositions[row, col] != null)
                    {
                        gameBoardPositions[row, col].Paint(g, (marginX * col) + locX, (marginY * row) + locY, imageType);
                    }

                    if (selectedPawnPossibleMovements != null && selectedPawnPossibleMovements[row, col]) 
                    {
                        g.DrawImage(PawnBitmapCollection.possibleMovementMarker, gameBoardPositions[row, col].GetRec());
                    }
                }
            }
       
            // Marking the selected pawn.
            if(!selectedPawn.IsEmpty) g.DrawImage(PawnBitmapCollection.pawnMarker, selectedPawn);

            // Marking the pawn that threatens the general if checked.
            if (threateningPawn != null) g.DrawImage(PawnBitmapCollection.threateningPawnMarker, threateningPawn.GetRec());
        }

        private void ResetBoard()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    gameBoardPositions[row, col] = null;
                }
            }

            PlacePawns();
        }

        private void PlacePawns()
        {
            gameState = GameState.PLAYING;

            blackGeneral = new General(PlayerSide.BLACK);
            redGeneral = new General(PlayerSide.RED);

            colBlackGeneral = 4;
            rowBlackGeneral = 0;

            colRedGeneral = 4;
            rowRedGeneral = 9;

            threateningPawn = null;
            rowThreateningPawn = -1;
            colThreateningPawn = -1;

            selectedPawn = Rectangle.Empty;
            selectedCol = -1;
            selectedRow = -1;
            selectedPawnPossibleMovements = null;

            // Black side.
            gameBoardPositions[0, 0] = new Rook(PlayerSide.BLACK);
            gameBoardPositions[0, 1] = new Knight(PlayerSide.BLACK);
            gameBoardPositions[0, 2] = new Elephant(PlayerSide.BLACK);
            gameBoardPositions[0, 3] = new Advisor(PlayerSide.BLACK);
            gameBoardPositions[0, 4] = blackGeneral;
            gameBoardPositions[0, 5] = new Advisor(PlayerSide.BLACK);
            gameBoardPositions[0, 6] = new Elephant(PlayerSide.BLACK);
            gameBoardPositions[0, 7] = new Knight(PlayerSide.BLACK);
            gameBoardPositions[0, 8] = new Rook(PlayerSide.BLACK);

            gameBoardPositions[2, 1] = new Cannon(PlayerSide.BLACK);
            gameBoardPositions[2, 7] = new Cannon(PlayerSide.BLACK);

            gameBoardPositions[3, 0] = new Soldier(PlayerSide.BLACK);
            gameBoardPositions[3, 2] = new Soldier(PlayerSide.BLACK);
            gameBoardPositions[3, 4] = new Soldier(PlayerSide.BLACK);
            gameBoardPositions[3, 6] = new Soldier(PlayerSide.BLACK);
            gameBoardPositions[3, 8] = new Soldier(PlayerSide.BLACK);

            // Red side.
            gameBoardPositions[9, 0] = new Rook(PlayerSide.RED);
            gameBoardPositions[9, 1] = new Knight(PlayerSide.RED);
            gameBoardPositions[9, 2] = new Elephant(PlayerSide.RED);
            gameBoardPositions[9, 3] = new Advisor(PlayerSide.RED);
            gameBoardPositions[9, 4] = redGeneral;
            gameBoardPositions[9, 5] = new Advisor(PlayerSide.RED);
            gameBoardPositions[9, 6] = new Elephant(PlayerSide.RED);
            gameBoardPositions[9, 7] = new Knight(PlayerSide.RED);
            gameBoardPositions[9, 8] = new Rook(PlayerSide.RED);

            gameBoardPositions[7, 1] = new Cannon(PlayerSide.RED);
            gameBoardPositions[7, 7] = new Cannon(PlayerSide.RED);

            gameBoardPositions[6, 0] = new Soldier(PlayerSide.RED);
            gameBoardPositions[6, 2] = new Soldier(PlayerSide.RED);
            gameBoardPositions[6, 4] = new Soldier(PlayerSide.RED);
            gameBoardPositions[6, 6] = new Soldier(PlayerSide.RED);
            gameBoardPositions[6, 8] = new Soldier(PlayerSide.RED);
            
            // Fill null cells with empty pawns.
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (gameBoardPositions[row, col] == null)
                    {
                        gameBoardPositions[row, col] = new EmptyPawn();
                    }
                }
            }
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (gameState == GameState.PLAYING)
            {
                if (e.Button == MouseButtons.Left)
                {
                    
                    LeftMouseClicked(e);
                    if (previousPlayer == PlayerSide.BLACK)
                    {
                        BotMakeMove();
                        Console.WriteLine("Bot Play");
                    }
                }

                if (e.Button == MouseButtons.Right)
                {
                    RightMouseClicked();
                }
            }
        }

        private void LeftMouseClicked(MouseEventArgs e)
        {
            if (!selectedPawn.IsEmpty) // Pawn already selected. Check if pawn can be moved.
            {
                bool pawnFound = false;

                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 10; col++)
                    {
                        // Check which cell is clicked by user and if the cell has an empty pawn or a pawn of the opposite player.
                        if (gameBoardPositions[col, row].GetRec().Contains(e.Location) && (gameBoardPositions[col, row].GetPlayerSide() == previousPlayer || gameBoardPositions[col, row].GetPlayerSide() == PlayerSide.EMPTY))
                        {
                            // Check if the selected cell is the same pawn that the user wants to move.
                            if(col == selectedCol && row == selectedRow)
                            {
                                pawnFound = true;
                                break;
                            }
                            else
                            {
                                //Let the pawn check if it is allowed to move to the new location.
                                if (gameBoardPositions[selectedCol, selectedRow].CheckMovement(selectedCol, selectedRow, col, row, gameBoardPositions))
                                {
                                    // Get true back if pawn can move to selected cell.
                                    previousPlayer = previousPlayer == PlayerSide.RED ? PlayerSide.BLACK : PlayerSide.RED;
                                    IPawn tempPawnStart = gameBoardPositions[selectedCol, selectedRow];
                                    IPawn tempPawnEnd = gameBoardPositions[col, row];

                                    if (gameBoardPositions[selectedCol, selectedRow].Equals(blackGeneral))
                                    {
                                        gameBoardPositions[col, row] = gameBoardPositions[selectedCol, selectedRow].Duplicate();
                                        blackGeneral = gameBoardPositions[col, row];
                                        colBlackGeneral = row;
                                        rowBlackGeneral = col;
                                        gameBoardPositions[selectedCol, selectedRow] = new EmptyPawn();

                                        // Check if generals do not face each other. If they face each other reset move.
                                        if (!CheckLineOfSightIsBlockedGenerals())
                                        {
                                            gameBoardPositions[selectedCol, selectedRow] = tempPawnStart;
                                            gameBoardPositions[col, row] = tempPawnEnd;

                                            blackGeneral = tempPawnStart;
                                            colBlackGeneral = selectedRow;
                                            rowBlackGeneral = selectedCol;

                                            previousPlayer = previousPlayer == PlayerSide.RED ? PlayerSide.BLACK : PlayerSide.RED;
                                            pawnFound = true;
                                            break;
                                        }

                                        if (GeneralIsChecked(blackGeneral))
                                        {
                                            gameBoardPositions[selectedCol, selectedRow] = tempPawnStart;
                                            gameBoardPositions[col, row] = tempPawnEnd;

                                            blackGeneral = tempPawnStart;
                                            colBlackGeneral = selectedRow;
                                            rowBlackGeneral = selectedCol;

                                            previousPlayer = previousPlayer == PlayerSide.RED ? PlayerSide.BLACK : PlayerSide.RED;
                                            pawnFound = true;
                                            break;
                                        }
                                        else
                                        {
                                            threateningPawn = null;
                                            rowThreateningPawn = -1;
                                            colThreateningPawn = -1;
                                        }
                                    }
                                    else if (gameBoardPositions[selectedCol, selectedRow].Equals(redGeneral))
                                    {
                                        gameBoardPositions[col, row] = gameBoardPositions[selectedCol, selectedRow].Duplicate();
                                        redGeneral = gameBoardPositions[col, row];
                                        colRedGeneral = row;
                                        rowRedGeneral = col;
                                        gameBoardPositions[selectedCol, selectedRow] = new EmptyPawn();

                                        // Check if generals do not face each other. If they face each other reset move.
                                        if (!CheckLineOfSightIsBlockedGenerals())
                                        {
                                            gameBoardPositions[selectedCol, selectedRow] = tempPawnStart;
                                            gameBoardPositions[col, row] = tempPawnEnd;

                                            redGeneral = tempPawnStart;
                                            colRedGeneral = selectedRow;
                                            rowRedGeneral = selectedCol;

                                            previousPlayer = previousPlayer == PlayerSide.RED ? PlayerSide.BLACK : PlayerSide.RED;
                                            pawnFound = true;
                                            break;
                                        }

                                        if (GeneralIsChecked(redGeneral))
                                        {
                                            gameBoardPositions[selectedCol, selectedRow] = tempPawnStart;
                                            gameBoardPositions[col, row] = tempPawnEnd;

                                            redGeneral = tempPawnStart;
                                            colRedGeneral = selectedRow;
                                            rowRedGeneral = selectedCol;

                                            previousPlayer = previousPlayer == PlayerSide.RED ? PlayerSide.BLACK : PlayerSide.RED;
                                            pawnFound = true;
                                            break;
                                        }
                                        else
                                        {
                                            threateningPawn = null;
                                            rowThreateningPawn = -1;
                                            colThreateningPawn = -1;
                                        }
                                    }
                                    else
                                    {
                                        gameBoardPositions[col, row] = gameBoardPositions[selectedCol, selectedRow].Duplicate();
                                        gameBoardPositions[selectedCol, selectedRow] = new EmptyPawn();

                                        // Check if generals do not face each other. If they face each other reset move.
                                        if (!CheckLineOfSightIsBlockedGenerals())
                                        {
                                            gameBoardPositions[selectedCol, selectedRow] = tempPawnStart;
                                            gameBoardPositions[col, row] = tempPawnEnd;

                                            previousPlayer = previousPlayer == PlayerSide.RED ? PlayerSide.BLACK : PlayerSide.RED;
                                            pawnFound = true;
                                            break;
                                        }

                                        if (previousPlayer == PlayerSide.BLACK)
                                        {
                                            if (GeneralIsChecked(blackGeneral))
                                            {
                                                gameBoardPositions[selectedCol, selectedRow] = tempPawnStart;
                                                gameBoardPositions[col, row] = tempPawnEnd;

                                                previousPlayer = previousPlayer == PlayerSide.RED ? PlayerSide.BLACK : PlayerSide.RED;
                                                pawnFound = true;
                                                break;
                                            }

                                            // Check if by moving this black pawn, it threatens the red general.
                                            if(GeneralIsChecked(redGeneral))
                                            {
                                                threateningPawn = gameBoardPositions[col, row];
                                                rowThreateningPawn = col;
                                                colThreateningPawn = row;
                                            }
                                            else
                                            {
                                                threateningPawn = null;
                                                rowThreateningPawn = -1;
                                                colThreateningPawn = -1;
                                            }
                                        }

                                        if (previousPlayer == PlayerSide.RED)
                                        {
                                            if (GeneralIsChecked(redGeneral))
                                            {
                                                gameBoardPositions[selectedCol, selectedRow] = tempPawnStart;
                                                gameBoardPositions[col, row] = tempPawnEnd;

                                                previousPlayer = previousPlayer == PlayerSide.RED ? PlayerSide.BLACK : PlayerSide.RED;
                                                pawnFound = true;
                                                break;
                                            }

                                            // Check if by moving this red pawn, it threatens the black general.
                                            if (GeneralIsChecked(blackGeneral))
                                            {
                                                threateningPawn = gameBoardPositions[col, row];
                                                rowThreateningPawn = col;
                                                colThreateningPawn = row;
                                            }
                                            else
                                            {
                                                threateningPawn = null;
                                                rowThreateningPawn = -1;
                                                colThreateningPawn = -1;
                                            }
                                        }
                                    }

                                    // Deselects current pawn so that the next player can select a pawn.
                                    selectedPawn = Rectangle.Empty;
                                    selectedCol = -1;
                                    selectedRow = -1;
                                    selectedPawnPossibleMovements = null;

                                    pawnFound = true;
                                    break;

                                }
                                else
                                {
                                    // Pawn is not allowed to move to the new location. Current pawn is still selected.
                                    pawnFound = true;
                                    break;
                                }                        
                            }
                        }
                    }

                    if (pawnFound)
                    {
                        break;
                    }
                }
            }
            else if (selectedPawn.IsEmpty && e != null) // No pawn selected, check if pawn can be selected.
            {
                bool pawnFound = false;

                for (int row = 0; row < 10; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        if (gameBoardPositions[row, col].GetPlayerSide() != PlayerSide.EMPTY && gameBoardPositions[row, col].GetPlayerSide() != previousPlayer && gameBoardPositions[row, col].GetRec().Contains(e.Location))
                        {
                            pawnFound = true;
                            selectedPawn = gameBoardPositions[row, col].GetRec();
                            selectedCol = row;
                            selectedRow = col;
                            selectedPawnPossibleMovements = gameBoardPositions[row, col].GetPossibleMovements(row, col, gameBoardPositions);

                            break;
                        }
                    }

                    if (pawnFound)
                    {
                        break;
                    }
                }
            }
        }

        private bool GeneralIsChecked(IPawn General)
        {
            PlayerSide player = General.GetPlayerSide();
            int rowGeneral = player == PlayerSide.BLACK ? rowBlackGeneral : rowRedGeneral;
            int colGeneral = player == PlayerSide.BLACK ? colBlackGeneral : colRedGeneral;

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (gameBoardPositions[row, col].GetPlayerSide() != PlayerSide.EMPTY && gameBoardPositions[row, col].GetPlayerSide() != player)
                    {
                        if (gameBoardPositions[row, col].CheckMovement(row, col, rowGeneral, colGeneral, gameBoardPositions))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool GeneralIsCheckMated(IPawn General)
        {
            PlayerSide player = General.GetPlayerSide();
            int rowGeneral = player == PlayerSide.BLACK ? rowBlackGeneral : rowRedGeneral;
            int colGeneral = player == PlayerSide.BLACK ? colBlackGeneral : colRedGeneral;

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (gameBoardPositions[row, col].GetPlayerSide() != PlayerSide.EMPTY && gameBoardPositions[row, col].GetPlayerSide() == player)
                    {
                        if (gameBoardPositions[row, col].CheckMovement(row, col, rowThreateningPawn, colThreateningPawn, gameBoardPositions))
                        {
                            return false;
                        }
                    }
                }
            }

            IPawn tempPawnStart;
            IPawn tempPawnEnd;
            bool generalCanEscape = false;

            // Move general up;
            if (General.CheckMovement(rowGeneral, colGeneral, (rowGeneral - 1), colGeneral, gameBoardPositions))
            {
                tempPawnStart = gameBoardPositions[rowGeneral, colGeneral];
                tempPawnEnd = gameBoardPositions[(rowGeneral - 1), colGeneral];

                gameBoardPositions[(rowGeneral - 1), colGeneral] = gameBoardPositions[rowGeneral, colGeneral].Duplicate();
                if (player == PlayerSide.BLACK)
                {
                    blackGeneral = gameBoardPositions[(rowGeneral - 1), colGeneral];
                    colBlackGeneral = colGeneral;
                    rowBlackGeneral = (rowGeneral - 1);
                }
                else
                {
                    redGeneral = gameBoardPositions[(rowGeneral - 1), colGeneral];
                    colRedGeneral = colGeneral;
                    rowRedGeneral = (rowGeneral - 1);
                }
           
                gameBoardPositions[rowGeneral, colGeneral] = new EmptyPawn();

                if (CheckLineOfSightIsBlockedGenerals() && !GeneralIsChecked(General))
                {
                    generalCanEscape = true;
                }

                gameBoardPositions[rowGeneral, colGeneral] = tempPawnStart;
                if (player == PlayerSide.BLACK)
                {
                    blackGeneral = tempPawnStart;
                    colBlackGeneral = colGeneral;
                    rowBlackGeneral = rowGeneral;
                }
                else
                {
                    redGeneral = tempPawnStart;
                    colRedGeneral = colGeneral;
                    rowRedGeneral = rowGeneral;
                }

                gameBoardPositions[(rowGeneral - 1), colGeneral] = tempPawnEnd;
            }

            // Move general down;
            if (General.CheckMovement(rowGeneral, colGeneral, (rowGeneral + 1), colGeneral, gameBoardPositions))
            {
                tempPawnStart = gameBoardPositions[rowGeneral, colGeneral];
                tempPawnEnd = gameBoardPositions[(rowGeneral + 1), colGeneral];

                gameBoardPositions[(rowGeneral + 1), colGeneral] = gameBoardPositions[rowGeneral, colGeneral].Duplicate();
                if (player == PlayerSide.BLACK)
                {
                    blackGeneral = gameBoardPositions[(rowGeneral + 1), colGeneral];
                    colBlackGeneral = colGeneral;
                    rowBlackGeneral = (rowGeneral + 1);
                }
                else
                {
                    redGeneral = gameBoardPositions[(rowGeneral + 1), colGeneral];
                    colRedGeneral = colGeneral;
                    rowRedGeneral = (rowGeneral + 1);
                }

                gameBoardPositions[rowGeneral, colGeneral] = new EmptyPawn();

                if (CheckLineOfSightIsBlockedGenerals() && !GeneralIsChecked(General))
                {
                    generalCanEscape = true;
                }

                gameBoardPositions[rowGeneral, colGeneral] = tempPawnStart;
                if (player == PlayerSide.BLACK)
                {
                    blackGeneral = tempPawnStart;
                    colBlackGeneral = colGeneral;
                    rowBlackGeneral = rowGeneral;
                }
                else
                {
                    redGeneral = tempPawnStart;
                    colRedGeneral = colGeneral;
                    rowRedGeneral = rowGeneral;
                }

                gameBoardPositions[(rowGeneral + 1), colGeneral] = tempPawnEnd;
            }

            // Move general right;
            if (General.CheckMovement(rowGeneral, colGeneral, rowGeneral, (colGeneral + 1), gameBoardPositions))
            {
                tempPawnStart = gameBoardPositions[rowGeneral, colGeneral];
                tempPawnEnd = gameBoardPositions[rowGeneral, (colGeneral + 1)];

                gameBoardPositions[rowGeneral, (colGeneral + 1)] = gameBoardPositions[rowGeneral, colGeneral].Duplicate();
                if (player == PlayerSide.BLACK)
                {
                    blackGeneral = gameBoardPositions[rowGeneral, (colGeneral + 1)];
                    colBlackGeneral = (colGeneral + 1);
                    rowBlackGeneral = rowGeneral;
                }
                else
                {
                    redGeneral = gameBoardPositions[rowGeneral, (colGeneral + 1)];
                    colRedGeneral = (colGeneral + 1);
                    rowRedGeneral = rowGeneral;
                }

                gameBoardPositions[rowGeneral, colGeneral] = new EmptyPawn();

                if (CheckLineOfSightIsBlockedGenerals() && !GeneralIsChecked(General))
                {
                    generalCanEscape = true;
                }

                gameBoardPositions[rowGeneral, colGeneral] = tempPawnStart;
                if (player == PlayerSide.BLACK)
                {
                    blackGeneral = tempPawnStart;
                    colBlackGeneral = colGeneral;
                    rowBlackGeneral = rowGeneral;
                }
                else
                {
                    redGeneral = tempPawnStart;
                    colRedGeneral = colGeneral;
                    rowRedGeneral = rowGeneral;
                }

                gameBoardPositions[rowGeneral, (colGeneral + 1)] = tempPawnEnd;
            }

            // Move general left;
            if (General.CheckMovement(rowGeneral, colGeneral, rowGeneral, (colGeneral - 1), gameBoardPositions))
            {
                tempPawnStart = gameBoardPositions[rowGeneral, colGeneral];
                tempPawnEnd = gameBoardPositions[rowGeneral, (colGeneral - 1)];

                gameBoardPositions[rowGeneral, (colGeneral - 1)] = gameBoardPositions[rowGeneral, colGeneral].Duplicate();
                if (player == PlayerSide.BLACK)
                {
                    blackGeneral = gameBoardPositions[rowGeneral, (colGeneral - 1)];
                    colBlackGeneral = (colGeneral - 1);
                    rowBlackGeneral = rowGeneral;
                }
                else
                {
                    redGeneral = gameBoardPositions[rowGeneral, (colGeneral - 1)];
                    colRedGeneral = (colGeneral - 1);
                    rowRedGeneral = rowGeneral;
                }

                gameBoardPositions[rowGeneral, colGeneral] = new EmptyPawn();

                if (CheckLineOfSightIsBlockedGenerals() && !GeneralIsChecked(General))
                {
                    generalCanEscape = true;
                }

                gameBoardPositions[rowGeneral, colGeneral] = tempPawnStart;
                if (player == PlayerSide.BLACK)
                {
                    blackGeneral = tempPawnStart;
                    colBlackGeneral = colGeneral;
                    rowBlackGeneral = rowGeneral;
                }
                else
                {
                    redGeneral = tempPawnStart;
                    colRedGeneral = colGeneral;
                    rowRedGeneral = rowGeneral;
                }

                gameBoardPositions[rowGeneral, (colGeneral - 1)] = tempPawnEnd;
            }

            if (generalCanEscape)
            {
                return false;
            }

            return true;
        }

        private bool CheckLineOfSightIsBlockedGenerals()
        {
            if(colBlackGeneral == colRedGeneral)
            {
                bool pawnPassed = false;
                for(int row = rowBlackGeneral + 1; row < rowRedGeneral; row++)
                {
                    if(gameBoardPositions[row, colBlackGeneral].GetPlayerSide() != PlayerSide.EMPTY)
                    {
                        pawnPassed = true;
                        break;
                    }
                }

                if(pawnPassed)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        private void RightMouseClicked()
        {
            selectedPawn = Rectangle.Empty;
            selectedCol = -1;
            selectedRow = -1;
            selectedPawnPossibleMovements = null;
        }

        public void NewGame()
        {
            ResetBoard();
            selectedPawn = Rectangle.Empty;
            selectedCol = -1;
            selectedRow = -1;
            previousPlayer = PlayerSide.RED;
        }

        public void ChangePawnType(int i)
        {
            imageType = i;
        }

        //public void BotMakeMove()
        //{
        //    if (gameState == GameState.PLAYING)
        //    {
        //        MinimaxAI ai = new MinimaxAI(PlayerSide.RED);
        //        var bestMove = ai.GetBestMove(gameBoardPositions);


        //        if (bestMove.fromRow != -1)
        //        {
        //            //IPawn movingPawn = gameBoardPositions[bestMove.fromRow, bestMove.fromCol];
        //            //gameBoardPositions[bestMove.toRow, bestMove.toCol] = movingPawn;
        //            //gameBoardPositions[bestMove.fromRow, bestMove.fromCol] = new EmptyPawn();
        //            //SimulateClick(bestMove.fromRow, bestMove.fromCol);
        //            LeftMouseClicked(SimulateClickEvent(bestMove.fromRow, bestMove.fromCol));
        //            LeftMouseClicked(SimulateClickEvent(bestMove.toRow, bestMove.toCol));
        //            Console.WriteLine(bestMove.fromRow + " " + bestMove.fromCol + " " + bestMove.toRow + " " + bestMove.toCol);
        //            //SimulateClick(bestMove.toRow, bestMove.toCol);

        //            //previousPlayer = PlayerSide.BLACK;
        //        }
        //    }
        //}

        public async void BotMakeMove()
        {
            if (gameState == GameState.PLAYING)
            {
                MinimaxAI ai = new MinimaxAI(PlayerSide.RED);
                var bestMove = ai.GetBestMove(gameBoardPositions);

                if (bestMove.fromRow != -1)
                {
                    // Thực hiện lần click đầu tiên
                    LeftMouseClicked(SimulateClickEvent(bestMove.fromRow, bestMove.fromCol));

                    // Đợi một khoảng thời gian ngắn (100ms) để chắc chắn rằng thao tác click đầu tiên đã được thực hiện
                    await Task.Delay(100);  // Tạm dừng 100ms

                    LeftMouseClicked(SimulateClickEvent(bestMove.toRow, bestMove.toCol));


                    Console.WriteLine($"{bestMove.fromRow} {bestMove.fromCol} {bestMove.toRow} {bestMove.toCol}");
                }
            }
        }

        public void SimulateClick(int row, int col, bool isLeftClick = true)
        {
            if (gameState != GameState.PLAYING)
                return;

            int locX = screenWidthMiddle - (pawnSizeWidth / 2) + col * (pawnSizeWidth + spaceBetweenPawnsX);
            int locY = screenHeightMiddle - (pawnSizeHeight / 2) + row * (pawnSizeHeight + spaceBetweenPawnsY);

            var mouseArgs = new MouseEventArgs(
                isLeftClick ? MouseButtons.Left : MouseButtons.Right,
                1,
                locX,
                locY,
                0
            );

            if (isLeftClick)
            {
                LeftMouseClicked(mouseArgs);
            }
            else
            {
                RightMouseClicked();
            }
        }

        public MouseEventArgs SimulateClickEvent(int row, int col, bool isLeftClick = true)
        {
            if (gameState != GameState.PLAYING)
                return null;

            int locX = screenWidthMiddle - (pawnSizeWidth / 2) + col * (pawnSizeWidth + spaceBetweenPawnsX);
            int locY = screenHeightMiddle - (pawnSizeHeight / 2) + row * (pawnSizeHeight + spaceBetweenPawnsY);

            var mouseArgs = new MouseEventArgs(
                isLeftClick ? MouseButtons.Left : MouseButtons.Right,
                1,
                locX,
                locY,
                0
            );

            return mouseArgs;
        }


        public void AutoSurrender()
        {
            surrender = true;
        }


    }
}
