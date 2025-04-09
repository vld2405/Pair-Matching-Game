using System.ComponentModel;

namespace MatchThePairs.Models
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        private int _gamesPlayed;
        public int GamesPlayed
        {
            get => _gamesPlayed;
            set
            {
                _gamesPlayed = value;
                OnPropertyChanged(nameof(GamesPlayed));
            }
        }
        private int _gamesWon;
        public int GamesWon
        {
            get => _gamesWon;
            set
            {
                _gamesWon = value;
                OnPropertyChanged(nameof(GamesWon));
            }
        }

        private int _timeScore;
        public int TimeScore
        {
            get => _timeScore;
            set
            {
                _timeScore = value;
                OnPropertyChanged(nameof(TimeScore));
            }
        }

        public User(string name, string imagePath)
        {
            Name = name;
            ImagePath = imagePath;
            GamesPlayed = 0;
            GamesWon = 0;
            TimeScore = 0;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}