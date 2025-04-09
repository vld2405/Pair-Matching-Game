using MatchThePairs.Views;
using MatchThePairs.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Threading.Channels;
using System.Windows.Controls;
using System.IO;

namespace MatchThePairs.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public static UserList SharedUserList { get; private set; }
        private UserList _userList => SharedUserList;
        public IEnumerable<User> Users => _userList.Users;
        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public bool CanManageUser => SelectedUser != null;

        public ICommand CommandNewUser { get; set; }
        public ICommand CommandCancelButton { get; set; }
        public ICommand CommandDeleteUser { get; set; }
        public ICommand CommandPrevUser { get; set; }
        public ICommand CommandNextUser { get; set; }
        public ICommand CommandOpenPlayGameWindow { get; set; }

        public MainWindowViewModel()
        {
            SharedUserList = new UserList();
            CommandNewUser = new RelayCommand(OpenAddUserWindow);
            CommandCancelButton = new RelayCommand(ExitApplication);
            CommandDeleteUser = new RelayCommand(DeleteUser, () => CanManageUser);
            CommandPrevUser = new RelayCommand(PrevUser);
            CommandNextUser = new RelayCommand(NextUser);
            CommandOpenPlayGameWindow = new RelayCommand(OpenPlayGameWindow, () => CanManageUser);
        }

        private void OpenAddUserWindow()
        {
            NewUserView newUserView = new NewUserView();
            Window mainWindow = Application.Current.MainWindow;
            newUserView.Owner = mainWindow;
            newUserView.ShowDialog();
        }

        private void ExitApplication()
        {
            // Save all user data before exiting
            SharedUserList.SaveUsers();
            Application.Current.Shutdown();
        }

        private void DeleteSave()
        {
            string saveDirectory = "../../../saves";
            if (!Directory.Exists(saveDirectory))
            {
                return;
            }
            string saveFilePath = $"{saveDirectory}/game_save_{SelectedUser.Name}.json";
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
        }

        private void DeleteUser()
        {
            DeleteSave();
            SharedUserList.RemoveUser(SelectedUser);
        }

        private void PrevUser()
        {
            SelectedUser = SharedUserList.PrevUser(SelectedUser);
        }

        private void NextUser()
        {
            SelectedUser = SharedUserList.NextUser(SelectedUser);
        }

        private void OpenPlayGameWindow()
        {
            GameView gameView = new GameView(SelectedUser);
            gameView.Show();
            Window mainWindow = Application.Current.MainWindow;
            mainWindow.Close();
            Application.Current.MainWindow = gameView;
        }
    }
}
