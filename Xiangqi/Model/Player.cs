namespace Xiangqi.Model
{
    public class Player
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public Player()
        {

        }

        public Player(string username, string name, int score)
        {
            Username = username;
            Name = name;
            Score = score;
        }
    }
}
