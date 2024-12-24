using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Xiangqi.Model
{
    public class DatabaseHelper
    {
        public bool AuthenticateUser(string username, string password)
        {
            using (var context = new ChessDbContext())
            {
                var user = context.Users
                                  .FirstOrDefault(u => u.Username == username && u.Password == password);
                return user != null;
            }
        }

        public int GetUserId(string username)
        {
            using (var context = new ChessDbContext())
            {
                var user = context.Users
                                  .FirstOrDefault(u => u.Username == username);
                return user?.UserId ?? 0;
            }
        }

        public int GetPlayerScore(int userId)
        {
            using (var context = new ChessDbContext())
            {
                var user = context.Users
                                  .FirstOrDefault(u => u.UserId == userId);
                return user?.Score ?? 0;
            }
        }

        public void UpdatePlayerScore(int userId, int newScore)
        {
            using (var context = new ChessDbContext())
            {
                var user = context.Users
                                  .FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    user.Score = newScore;
                    context.SaveChanges();
                }
            }
        }

        public void AddNewUser(string username, string password, string name)
        {
            using (var context = new ChessDbContext())
            {
                var user = new User
                {
                    Username = username,
                    Password = password,
                    Name = name,
                    Score = 0
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public bool RegisterUser(string username, string password, string name)
        {
            if (IsUsernameTaken(username))
            {
                return false;
            }

            using (var context = new ChessDbContext())
            {
                var user = new User
                {
                    Username = username,
                    Password = password,
                    Name = name,
                    Score = 0
                };

                context.Users.Add(user);
                context.SaveChanges();
                return true;
            }
        }

        public bool IsUsernameTaken(string username)
        {
            using (var context = new ChessDbContext())
            {
                return context.Users.Any(u => u.Username == username);
            }
        }

        public List<Player> GetAllPlayers()
        {
            using (var context = new ChessDbContext())
            {
                return context.Users
                              .Select(u => new Player
                              {
                                  Username = u.Username,
                                  Name = u.Name,
                                  Score = u.Score
                              })
                              .ToList();
            }
        }
    }
}
