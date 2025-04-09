using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MatchThePairs.Models
{
    class UserList
    {
        public ObservableCollection<User> Users { get; set; }
        private const string USER_DATA_FILE = "../../../saves/users.json";

        public UserList()
        {
            Users = new ObservableCollection<User>();
            LoadUsers();
        }
        public UserList(ObservableCollection<User> users)
        {
            Users = users;
        }

        public void AddUser(User user)
        {
            foreach(User existingUser in Users)
                if(user.Name == existingUser.Name)
                {
                    MessageBox.Show("User already exists!", "Error");
                    return;
                }

            Users.Add(user);
            SaveUsers();
        }

        public void RemoveUser(User user)
        {
            Users.Remove(user);
            SaveUsers();
        }

        public User PrevUser(User currUser)
        {
            if (Users.Count == 0)
                return null;

            int indexOfCurr = Users.IndexOf(currUser);
            int usersLen = Users.Count;
            return Users[(indexOfCurr - 1 + usersLen) % usersLen];
        }

        public User NextUser(User currUser)
        {
            if (Users.Count == 0)
                return null;

            int indexOfCurr = Users.IndexOf(currUser);
            int usersLen = Users.Count;
            return Users[(indexOfCurr + 1) % usersLen];
        }

        public void SaveUsers()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(Users, options);
                File.WriteAllText(USER_DATA_FILE, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user data: {ex.Message}", "Error");
            }
        }

        public void LoadUsers()
        {
            try
            {
                if (File.Exists(USER_DATA_FILE))
                {
                    string jsonString = File.ReadAllText(USER_DATA_FILE);

                    Users.Clear();

                    var loadedUsers = JsonSerializer.Deserialize<List<User>>(jsonString);
                    if (loadedUsers != null)
                    {
                        foreach (var user in loadedUsers)
                        {
                            Users.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Error");
            }
        }
    }
}
