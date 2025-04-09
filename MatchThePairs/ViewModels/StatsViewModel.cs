using MatchThePairs.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchThePairs.ViewModels
{
    class StatsViewModel
    {
        private UserList _userList => MainWindowViewModel.SharedUserList;
        public IEnumerable<User> Users => _userList.Users.OrderByDescending(u => u.GamesWon);
    }
}
