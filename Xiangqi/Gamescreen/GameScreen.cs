using System;
using System.Drawing;
using System.Windows.Forms;
using Xiangqi.Gamescreen;

namespace Xiangqi
{
    public partial class GameScreen : Form
    {
        public static int width = 1024;
        public static int height = 800;
        private GameBoard gameBoard;

        private Timer autoClickTimer;

        public GameScreen()
        {
            InitializeComponent();

            Width = width;
            Height = height;
            CenterToScreen();
            Text = "Xiangqi";
            FormBorderStyle = FormBorderStyle.FixedSingle;

            DoubleBuffered = true;

            gameBoard = new GameBoard();

            // Tạo Timer
            autoClickTimer = new Timer();
            autoClickTimer.Interval = 400; // 500ms = 0.5s
            autoClickTimer.Tick += AutoClickTimer_Tick;
            autoClickTimer.Start(); // Bắt đầu Timer
        }

        // Sự kiện của Timer mỗi khi nó "Tick"
        private void AutoClickTimer_Tick(object sender, EventArgs e)
        {
                MouseEventArgs fakeMouseEvent = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
                OnMouseClick(fakeMouseEvent); // Gọi OnMouseClick để xử lý sự kiện
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            gameBoard.Paint(e, label1);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            gameBoard.HandleMouseClick(e);
            Refresh();
        }

        private void ToolBarNewGameClicked(object sender, EventArgs e)
        {
            gameBoard.NewGame();
            Refresh();
        }

        private void ToolBarWesternPawnsClicked(object sender, EventArgs e)
        {
            gameBoard.ChangePawnType(1);
            Refresh();
        }

        private void ToolBarChinesePawnsClicked(object sender, EventArgs e)
        {
            gameBoard.ChangePawnType(0);
            Refresh();
        }

        private void ToolBarCloseGameClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void ToolBarAboutClicked(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void playWithBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Code cho play with bot nếu cần
        }

        private void Surrender_Click(object sender, EventArgs e)
        {
            gameBoard.AutoSurrender();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            new Leaderboard().ShowDialog();
        }
    }
}
