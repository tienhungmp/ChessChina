using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiangqi.Model;

namespace Xiangqi.Gamescreen
{
    public partial class Leaderboard : Form
    {
        public Leaderboard()
        {
            InitializeComponent();
            DisplayPlayers();
        }


        private void DisplayPlayers()
        {
            DatabaseHelper dbHelper = new DatabaseHelper();
            List<Player> players = dbHelper.GetAllPlayers();

            // Giả sử bạn có một DataGridView hoặc ListBox để hiển thị thông tin người chơi
            foreach (var player in players)
            {
                // Thêm vào DataGridView (ví dụ DataGridView1)
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.Add(new DataGridViewTextBoxCell { Value = player.Username });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = player.Name });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = player.Score });
                dataGridView1.Rows.Add(row);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
