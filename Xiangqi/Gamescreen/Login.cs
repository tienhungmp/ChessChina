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
    public partial class Login : Form
    {

        private DatabaseHelper dbHelper;
        public static int UserId { get; set; } // Biến tĩnh để lưu trữ thông tin người dùng
        public Login()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (dbHelper.AuthenticateUser(username, password))
            {
                // Lấy UserId của người dùng đã đăng nhập
                UserId = dbHelper.GetUserId(username);

                new GameScreen().ShowDialog();
                //lblWelcome.Text = $"Chào mừng {username}!";
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!");
            }
        }

        public void OnPlayerWin()
        {
            if (UserId == 0) // Kiểm tra xem userId đã được thiết lập chưa
            {
                MessageBox.Show("Chưa đăng nhập!");
                return;
            }

            // Lấy điểm số hiện tại của người chơi
            int currentScore = dbHelper.GetPlayerScore(UserId);
            currentScore++;

            // Cập nhật điểm số mới vào cơ sở dữ liệu
            dbHelper.UpdatePlayerScore(UserId, currentScore);

            MessageBox.Show($"Bạn thắng! Điểm số hiện tại: {currentScore}");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            new BtnRegister().ShowDialog();
        }
    }
}
