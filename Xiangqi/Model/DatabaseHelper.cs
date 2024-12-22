using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiangqi.Model
{
    public class DatabaseHelper
    {
        private string connectionString = @"Server=DESKTOP-NC78VN5;Database=ChessChina;User Id=sa;Password=123;";

        // Hàm kết nối cơ sở dữ liệu
        private SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Hàm xác thực người dùng đăng nhập
        public bool AuthenticateUser(string username, string password)
        {
            bool isAuthenticated = false;

            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(1) FROM Users WHERE Username = @username AND Password = @password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = (int)cmd.ExecuteScalar();
                    isAuthenticated = count > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi kết nối cơ sở dữ liệu: " + ex.Message);
                }
            }

            return isAuthenticated;
        }

        // Hàm lấy UserId từ Username
        public int GetUserId(string username)
        {
            int userId = 0;

            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT UserId FROM Users WHERE Username = @username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userId = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi lấy UserId: " + ex.Message);
                }
            }

            return userId;
        }

        // Hàm lấy điểm số người chơi từ cơ sở dữ liệu
        public int GetPlayerScore(int userId)
        {
            int score = 0;

            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Score FROM Users WHERE UserId = @userId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        score = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi kết nối cơ sở dữ liệu: " + ex.Message);
                }
            }

            return score;
        }

        // Hàm cập nhật điểm số người chơi
        public void UpdatePlayerScore(int userId, int newScore)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Users SET Score = @score WHERE UserId = @userId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@score", newScore);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi cập nhật cơ sở dữ liệu: " + ex.Message);
                }
            }
        }

        // Hàm thêm người chơi mới vào bảng Users
        public void AddNewUser(string username, string password, string name)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO Users (Username, Password, Name, Score) VALUES (@username, @password, @name, 0)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@name", name);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi thêm người chơi: " + ex.Message);
                }
            }
        }

        public bool RegisterUser(string username, string password, string name)
        {
            if (IsUsernameTaken(username))
            {
                return false;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Users (Username, Password, Name, Score) VALUES (@Username, @Password, @Name, 0)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Name", name);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool IsUsernameTaken(string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Nếu có người dùng trùng tên, trả về true
            }
        }

        public List<Player> GetAllPlayers()
        {
            List<Player> players = new List<Player>();

            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Username, Name, Score FROM Users";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string username = reader["Username"].ToString();
                            string name = reader["Name"].ToString();
                            int score = Convert.ToInt32(reader["Score"]);

                            players.Add(new Player(username, name, score));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi lấy danh sách người chơi: " + ex.Message);
                }
            }

            return players;
        }
    }
}
