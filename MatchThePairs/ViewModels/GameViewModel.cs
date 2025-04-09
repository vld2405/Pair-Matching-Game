using MatchThePairs.Models;
using MatchThePairs.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MatchThePairs.ViewModels
{
    class GameViewModel : ViewModelBase
    {
        #region table size

        private int _tableSizeX;
        public int TableSizeX
        {
            get => _tableSizeX;
            set
            {
                _tableSizeX = value;
                OnPropertyChanged(nameof(TableSizeX));
            }
        }

        private int _tableSizeY;
        public int TableSizeY
        {
            get => _tableSizeY;
            set
            {
                _tableSizeY = value;
                OnPropertyChanged(nameof(TableSizeY));
            }
        }

        private string _tableSizeString;
        public string TableSizeString
        {
            get => _tableSizeString;
            set
            {
                _tableSizeString = value;
                OnPropertyChanged(nameof(TableSizeString));
            }
        }

        #endregion

        #region user

        private User _user;
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        #endregion

        #region images

        private List<List<string>> Images;

        private List<string> _chosenImages { get; set; }

        public IEnumerable<string> ChosenImages => _chosenImages;

        private string _categoryName;
        public string CategoryName
        {
            get
            {
                return _categoryName;
            }
            set
            {
                _categoryName = value;
                OnPropertyChanged(nameof(CategoryName));
            }
        }

        #endregion

        #region game state
        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));

                // Start or stop the timer based on game state, but only if the timer is initialized
                if (_isPlaying && _gameTimer != null)
                    StartTimer();
                else if (!_isPlaying && _gameTimer != null)
                    StopTimer();
            }
        }

        private Card _firstCard;
        private Card _secondCard;
        private bool _canFlip = true;
        private DispatcherTimer _flipBackTimer;
        private int _matchesFound;
        private int _totalPairs;

        #endregion

        #region timer

        private int _timerCount;
        public int TimerCount
        {
            get
            {
                return _timerCount;
            }
            set
            {
                _timerCount = value;
                OnPropertyChanged(nameof(TimerCount));
            }
        }
        private DispatcherTimer _gameTimer;
        private TimeSpan _remainingTime;
        private bool _isTimeoutMessageShown = false;

        public TimeSpan RemainingTime
        {
            get => _remainingTime;
            set
            {
                _remainingTime = value;
                OnPropertyChanged(nameof(RemainingTime));
                OnPropertyChanged(nameof(TimerDisplay));
            }
        }

        public string TimerDisplay => $"{RemainingTime.Minutes:00}:{RemainingTime.Seconds:00}";
        #endregion

        #region commands
        public ICommand CommandAbout { get; set; }
        public ICommand CommandStats { get; set; }
        public ICommand CommandExit { get; set; }
        public ICommand CommandStandardMode { get; set; }
        public ICommand CommandCustomMode { get; set; }
        public ICommand CommandCategory1 { get; set; }
        public ICommand CommandCategory2 { get; set; }
        public ICommand CommandCategory3 { get; set; }
        public ICommand CommandOneMinute { get; set; }
        public ICommand CommandTwoMinutes { get; set; }
        public ICommand CommandFiveMinutes { get; set; }
        public ICommand CommandNewGame { get; set; }
        public ICommand CommandLoadGame { get; set; }
        public ICommand CommandSaveGame { get; set; }
        public ICommand CommandFlipCard { get; set; }

        #endregion

        public ObservableCollection<Card> Cards { get; private set; }

        public GameViewModel(Models.User selectedUser)
        {
            TableSizeX = 4;
            TableSizeY = 4;
            CategoryName = "";
            TableSizeString = "4x4";
            User = selectedUser;
            Cards = new ObservableCollection<Card>();
            IsPlaying = false;
            TimerCount = 2;
            RemainingTime = TimeSpan.FromMinutes(TimerCount);

            _flipBackTimer = new DispatcherTimer();
            _flipBackTimer.Interval = TimeSpan.FromSeconds(1);
            _flipBackTimer.Tick += FlipBackTimer_Tick;

            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += GameTimer_Tick;

            Images = new List<List<string>>
            { 
                // aj1
                new List<string>
                {
                    //"/images/aj1/1.webp",
                    //"/images/aj1/2.jpg",
                    //"/images/aj1/3.webp",
                    //"/images/aj1/4.webp",
                    //"/images/aj1/5.webp",
                    //"/images/aj1/6.webp",
                    //"/images/aj1/7.webp",
                    //"/images/aj1/8.webp",
                    //"/images/aj1/9.jpeg",
                    //"/images/aj1/10.webp"
                    "/images/aj1/slots1.jpg",
                    "/images/aj1/slots2.jpg",
                    "/images/aj1/slots3.jpg",
                    "/images/aj1/slots4.jpg",
                    "/images/aj1/slots5.jpg",
                    "/images/aj1/slots6.jpg",
                    "/images/aj1/slots7.jpg",
                    "/images/aj1/slots8.jpg",
                    "/images/aj1/slots9.jpg",
                    "/images/aj1/slots10.jpg",
                    "/images/aj1/slots11.jpg",
                    "/images/aj1/slots12.jpg",
                    "/images/aj1/slots13.jpg",
                    "/images/aj1/slots14.jpg",
                    "/images/aj1/slots15.jpg",
                },
                // albums
                new List<string>
                {
                    "/images/albums/2014hills.jfif",
                    "/images/albums/2093.jfif",
                    "/images/albums/damn.png",
                    "/images/albums/dbr.jpg",
                    "/images/albums/hav.jpg",
                    "/images/albums/liveloveasap.jpg",
                    "/images/albums/llasap.jfif",
                    "/images/albums/offseason.jpg",
                    "/images/albums/mbdtf.jfif",
                    "/images/albums/sihal.jfif",
                    "/images/albums/testing.png",
                    "/images/albums/utopia.jpeg"
                },
                // memes
                new List<string>
                {
                    "/images/memes/batman.jpg",
                    "/images/memes/ben_zeciu.png",
                    "/images/memes/danutzu.webp",
                    "/images/memes/gadagdigi.jpg",
                    "/images/memes/i_guess_well_never_know.jpg",
                    "/images/memes/irafiel.jpg",
                    "/images/memes/nbnweed.jpg",
                    "/images/memes/sorrymotherdog.png",
                    "/images/memes/xslayder.webp",
                    "/images/memes/sigma.jfif",
                    "/images/memes/dodge.webp",
                    "/images/memes/lebron.jfif",
                    "/images/memes/shrek.jfif",
                    "/images/memes/troll.png",
                    "/images/memes/squidward.png"
                }
            };

            MessageBox.Show(
                "Welcome to Match The Pairs!\n\n" +
                "To start playing:\n" +
                "1. Choose a category from the File menu, or\n" +
                "2. Click 'New Game' to select a category\n\n" +
                "You can also set a custom grid size from the Options menu.",
                "Game Instructions",
                MessageBoxButton.OK);

            CommandAbout = new RelayCommand(OpenAboutInfoWindow);
            CommandStats = new RelayCommand(OpenStatsWindow);
            CommandExit = new RelayCommand(ReturnToMainMenuWindow);
            CommandStandardMode = new RelayCommand(SetStandardMode);
            CommandCustomMode = new RelayCommand(SetCustomMode);
            CommandCategory1 = new RelayCommand(() => { SelectFirstCategory(); InitializeGame(); });
            CommandCategory2 = new RelayCommand(() => { SelectSecondCategory(); InitializeGame(); });
            CommandCategory3 = new RelayCommand(() => { SelectThirdCategory(); InitializeGame(); });
            CommandOneMinute = new RelayCommand(SetOneMinuteTimer);
            CommandTwoMinutes = new RelayCommand(SetTwoMinuteTimer);
            CommandFiveMinutes = new RelayCommand(SetFiveMinuteTimer);
            CommandNewGame = new RelayCommand(InitiateNewGame);
            CommandLoadGame = new RelayCommand(LoadGame);
            CommandSaveGame = new RelayCommand(SaveGame);
            CommandFlipCard = new RelayCommand<Card>(FlipCard);
        }

        private void SaveGame()
        {
            if (!IsPlaying)
            {
                MessageBox.Show("There is no active game to save.", "Save Game",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var gameState = new GameState
                {
                    TableSizeX = TableSizeX,
                    TableSizeY = TableSizeY,
                    CategoryName = CategoryName,
                    UserName = User.Name,
                    TimerCount = TimerCount,
                    RemainingTime = RemainingTime,
                    MatchesFound = _matchesFound,
                    TotalPairs = _totalPairs
                };

                foreach (var card in Cards)
                {
                    gameState.Cards.Add(new CardState
                    {
                        ImagePath = card.ImagePath,
                        IsFlipped = false,
                        IsMatched = card.IsMatched,
                        Id = card.Id
                    });
                }

                string saveDirectory = "../../../saves";
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }
                string fileName = $"{saveDirectory}/game_save_{User.Name}.json";

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(gameState, options);
                File.WriteAllText(fileName, jsonString);

                MessageBox.Show($"Game saved successfully to:\n{fileName}", "Save Game",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex.Message}", "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGame()
        {
            try
            {
                string saveDirectory = "../../../saves";
                if (!Directory.Exists(saveDirectory))
                {
                    MessageBox.Show("No saved games found.", "Load Game",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string saveFilePath = $"{saveDirectory}/game_save_{User.Name}.json";

                if (!File.Exists(saveFilePath))
                {
                    MessageBox.Show($"No saved games found for {User.Name}.", "Load Game",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Load the most recent save from {Path.GetFileName(saveFilePath)}?",
                    "Load Game",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    LoadGameFromFile(saveFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}", "Load Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGameFromFile(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions();
                var gameState = JsonSerializer.Deserialize<GameState>(jsonString, options);

                if (gameState == null)
                {
                    throw new Exception("Failed to deserialize game state.");
                }

                if (gameState.UserName != User.Name)
                {
                    MessageBox.Show($"This save belongs to {gameState.UserName}, not {User.Name}.",
                        "User Mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsPlaying = false;
                StopTimer();

                TableSizeX = gameState.TableSizeX;
                TableSizeY = gameState.TableSizeY;
                TableSizeString = $"{TableSizeX}x{TableSizeY}";
                CategoryName = gameState.CategoryName;
                TimerCount = gameState.TimerCount;
                RemainingTime = gameState.RemainingTime;
                _matchesFound = gameState.MatchesFound;
                _totalPairs = gameState.TotalPairs;

                switch (CategoryName)
                {
                    case "Jordan 1s":
                        SelectFirstCategory();
                        break;
                    case "Albums":
                        SelectSecondCategory();
                        break;
                    case "Memes":
                        SelectThirdCategory();
                        break;
                    default:
                        MessageBox.Show($"Unknown category: {CategoryName}", "Load Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                Cards.Clear();
                _firstCard = null;
                _secondCard = null;
                _canFlip = true;

                foreach (var savedCard in gameState.Cards)
                {
                    var card = new Card(savedCard.ImagePath, savedCard.Id, CommandFlipCard);
                    card.IsFlipped = savedCard.IsFlipped;
                    card.IsMatched = savedCard.IsMatched;
                    Cards.Add(card);
                }

                IsPlaying = true;
                MessageBox.Show($"Game loaded successfully!\nTime remaining: {TimerDisplay}",
                    "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}", "Load Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetOneMinuteTimer()
        {
            TimerCount = 1;
            if (IsPlaying)
            {
                var result = MessageBox.Show(
                    "Do you want to update the timer for the current game?",
                    "Update Timer",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    StopTimer();
                    RemainingTime = TimeSpan.FromMinutes(TimerCount);
                    StartTimer();
                }
            }
            else
            {
                RemainingTime = TimeSpan.FromMinutes(TimerCount);
            }
        }
        
        private void SetTwoMinuteTimer()
        {
            TimerCount = 2;
            if (IsPlaying)
            {
                var result = MessageBox.Show(
                    "Do you want to update the timer for the current game?",
                    "Update Timer",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    StopTimer();
                    RemainingTime = TimeSpan.FromMinutes(TimerCount);
                    StartTimer();
                }
            }
            else
            {
                RemainingTime = TimeSpan.FromMinutes(TimerCount);
            }
        }
        
        private void SetFiveMinuteTimer()
        {
            TimerCount = 5;
            if (IsPlaying)
            {
                var result = MessageBox.Show(
                    "Do you want to update the timer for the current game?",
                    "Update Timer",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    StopTimer();
                    RemainingTime = TimeSpan.FromMinutes(TimerCount);
                    StartTimer();
                }
            }
            else
            {
                RemainingTime = TimeSpan.FromMinutes(TimerCount);
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                RemainingTime = RemainingTime.Subtract(TimeSpan.FromSeconds(1));

                if (RemainingTime.TotalSeconds <= 0)
                {
                    StopTimer();

                    if (!_isTimeoutMessageShown)
                    {
                        _isTimeoutMessageShown = true;

                        IsPlaying = false;

                        MessageBox.Show("Time's up! Game over.",
                            "Time Expired",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        User.GamesPlayed++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Timer tick error: {ex.Message}");
                StopTimer();
            }
        }

        private void StartTimer()
        {
            try
            {
                if (_gameTimer == null)
                {
                    _gameTimer = new DispatcherTimer();
                    _gameTimer.Interval = TimeSpan.FromSeconds(1);
                    _gameTimer.Tick += GameTimer_Tick;
                }

                _isTimeoutMessageShown = false;

                RemainingTime = TimeSpan.FromMinutes(TimerCount);

                _gameTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting timer: {ex.Message}", "Timer Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopTimer()
        {
            try
            {
                if (_gameTimer != null && _gameTimer.IsEnabled)
                {
                    _gameTimer.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping timer: {ex.Message}", "Timer Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitiateNewGame()
        {
            InitializeGame();
        }

        private void OpenAboutInfoWindow()
        {
            AboutWindowView aboutWindowView = new AboutWindowView();
            Window gameWindow = Application.Current.MainWindow;
            aboutWindowView.Owner = gameWindow;
            aboutWindowView.ShowDialog();
        }

        private void OpenStatsWindow()
        {
            StatsView statsWindowView = new StatsView();
            Window gameWindow = Application.Current.MainWindow;
            statsWindowView.Owner = gameWindow;
            statsWindowView.ShowDialog();
        }

        private void ReturnToMainMenuWindow()
        {
            MainWindow mainWindow = new MainWindow();
            Window gameWindow = Application.Current.MainWindow;
            mainWindow.Show();
            Application.Current.MainWindow = mainWindow;
            gameWindow.Close();
        }

        public static List<T> ShuffleList<T>(List<T> list)
        {
            Random rng = new Random();
            return list.OrderBy(item => rng.Next()).ToList();
        }

        private void SetStandardMode()
        {
            TableSizeX = 4;
            TableSizeY = 4;
            TableSizeString = $"{TableSizeX}x{TableSizeY}";

            if (IsPlaying)
            {
                var result = MessageBox.Show(
                    "Do you want to restart the game with the new size?",
                    "Restart Game",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    InitializeGame();
                }
            }
        }

        private void SetCustomMode()
        {
            CustomSizeView customSizeView = new CustomSizeView();
            Window gameWindow = Application.Current.MainWindow;
            customSizeView.Owner = gameWindow;

            if (customSizeView.ShowDialog() == true)
            {
                TableSizeX = customSizeView.CustomWidth;
                TableSizeY = customSizeView.CustomHeight;
                TableSizeString = $"{TableSizeX}x{TableSizeY}";
                MessageBox.Show($"Custom size set to {TableSizeX}x{TableSizeY}", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                if (IsPlaying)
                {
                    var result = MessageBox.Show(
                        "Do you want to restart the game with the new size?",
                        "Restart Game",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        InitializeGame();
                    }
                }
            }
        }

        private void SelectFirstCategory()
        {
            _chosenImages = Images[0];
            _chosenImages = ShuffleList(_chosenImages);
            CategoryName = "Slots";
        }

        private void SelectSecondCategory()
        {
            _chosenImages = Images[1];
            _chosenImages = ShuffleList(_chosenImages);
            CategoryName = "Albums";
        }

        private void SelectThirdCategory()
        {
            _chosenImages = Images[2];
            _chosenImages = ShuffleList(_chosenImages);
            CategoryName = "Memes";
        }

        private void InitializeGame()
        {
            if (string.IsNullOrEmpty(CategoryName) || _chosenImages == null)
            {
                MessageBox.Show("Please select a category first.", "Category Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrEmpty(TableSizeString))
            {
                TableSizeString = $"{TableSizeX}x{TableSizeY}";
            }

            _matchesFound = 0;
            _firstCard = null;
            _secondCard = null;
            _canFlip = true;
            Cards.Clear();

            RemainingTime = TimeSpan.FromMinutes(TimerCount);

            int totalCells = TableSizeX * TableSizeY;
            _totalPairs = totalCells / 2;

            List<string> imagesToUse = new List<string>();

            _chosenImages = ShuffleList(_chosenImages);

            for (int i = 0; i < _totalPairs; i++)
            {
                int imageIndex = i % _chosenImages.Count;
                imagesToUse.Add(_chosenImages[imageIndex]);
            }

            var cardPairs = new List<Card>();
            for (int i = 0; i < _totalPairs; i++)
            {
                cardPairs.Add(new Card(imagesToUse[i], i, CommandFlipCard));
                cardPairs.Add(new Card(imagesToUse[i], i, CommandFlipCard));
            }

            var shuffledCards = ShuffleList(cardPairs);

            foreach (var card in shuffledCards)
            {
                Cards.Add(card);
            }

            IsPlaying = true;
        }

        private void FlipCard(Card card)
        {
            if (!_canFlip || card.IsFlipped || card.IsMatched)
                return;

            card.IsFlipped = true;

            if (_firstCard == null)
            {
                _firstCard = card;
            }
            else
            {
                _secondCard = card;
                _canFlip = false;

                if (_firstCard.Id == _secondCard.Id)
                {
                    _firstCard.IsMatched = true;
                    _secondCard.IsMatched = true;
                    _matchesFound++;

                    _firstCard = null;
                    _secondCard = null;
                    _canFlip = true;

                    if (_matchesFound == _totalPairs)
                    {
                        GameComplete();
                    }
                }
                else
                {
                    _flipBackTimer.Start();
                }
            }
        }

        private void FlipBackTimer_Tick(object sender, EventArgs e)
        {
            _flipBackTimer.Stop();

            _firstCard.IsFlipped = false;
            _secondCard.IsFlipped = false;

            _firstCard = null;
            _secondCard = null;
            _canFlip = true;
        }

        private void GameComplete()
        {
            IsPlaying = false;

            StopTimer();

            int timeBonus = (int)RemainingTime.TotalSeconds * 5;

            MessageBox.Show($"Congratulations! You've matched all pairs!\nTime remaining: {TimerDisplay}", "Game Complete",
                MessageBoxButton.OK, MessageBoxImage.Information);

            User.GamesPlayed++;
            User.GamesWon++;

            int baseScore = TableSizeX * TableSizeY * 10;
            int score = baseScore + timeBonus;

            if (score > User.TimeScore)
            {
                User.TimeScore = score;
                MessageBox.Show($"New high score: {score}!", "High Score",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            MainWindowViewModel.SharedUserList.SaveUsers();
        }
    }
}