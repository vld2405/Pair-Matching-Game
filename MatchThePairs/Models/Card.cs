using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows;

namespace MatchThePairs.Models
{
    public class Card : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static string CardBackImagePath { get; } = "/images/card_back.png";

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            private set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
                OnPropertyChanged(nameof(DisplayImage));
            }
        }

        private bool _isFlipped;
        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                _isFlipped = value;
                OnPropertyChanged(nameof(IsFlipped));
                OnPropertyChanged(nameof(DisplayImage));
            }
        }

        private bool _isMatched;
        public bool IsMatched
        {
            get => _isMatched;
            set
            {
                _isMatched = value;
                OnPropertyChanged(nameof(IsMatched));
                OnPropertyChanged(nameof(DisplayImage));
            }
        }

        public int Id { get; private set; }

        public ICommand FlipCommand { get; private set; }

        public string DisplayImage => IsFlipped || IsMatched ? ImagePath : CardBackImagePath;

        public Card(string imagePath, int id, ICommand flipCommand)
        {
            ImagePath = imagePath;
            Id = id;
            FlipCommand = flipCommand;
            IsFlipped = false;
            IsMatched = false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}