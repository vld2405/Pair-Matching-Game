using MatchThePairs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MatchThePairs.ViewModels
{
    class NewUserViewModel : ViewModelBase
    {
        private bool _canAddUser;
        public bool CanAddUser
        {
            get => _canAddUser;
            set
            {
                _canAddUser = value;
                OnPropertyChanged(nameof(CanAddUser));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                CanAddUser = !string.IsNullOrWhiteSpace(_name);
                OnPropertyChanged(nameof(Name));
            }
        }
        private int _imageIndex;
        private string _selectedImage;

        public string SelectedImage
        {
            get => _selectedImage;
            set
            {
                _selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }

        public List<string> ImagePaths { get; set; }
        public ICommand CommandAddUser { get; set; }
        public ICommand CommandPrevImage { get; set; }
        public ICommand CommandNextImage { get; set; }

        public NewUserViewModel()
        {
            _imageIndex = 0;
            _name = string.Empty;
            CanAddUser = false;
            ImagePaths = new List<string>
            {
                "/images/avatars/batman.jpg",
                "/images/avatars/cptamerica.webp",
                "/images/avatars/deadpool.jpg",
                "/images/avatars/spiderman.webp",
                "/images/avatars/wonderwoman.jpg",
                "/images/bahoi/bahoi.jpg",
                "/images/bahoi/bahoi_gheghe.jfif",
                "/images/bahoi/baloi.webp",
                "/images/bahoi/puiu_spartan.png",
                "/images/bahoi/ronaldo_bahoi.jpg",
            };
            SelectedImage = ImagePaths[_imageIndex];

            CommandAddUser = new RelayCommand(AddNewUser, () => CanAddUser);
            CommandPrevImage = new RelayCommand(PrevImage);
            CommandNextImage = new RelayCommand(NextImage);
        }

        private void AddNewUser()
        {
            User newUser = new User(Name, SelectedImage);
            MainWindowViewModel.SharedUserList.AddUser(newUser);

            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void PrevImage()
        {
            _imageIndex = (_imageIndex - 1 + ImagePaths.Count) % ImagePaths.Count;
            SelectedImage = ImagePaths[_imageIndex];
        }

        private void NextImage()
        {
            _imageIndex = (_imageIndex + 1) % ImagePaths.Count;
            SelectedImage = ImagePaths[_imageIndex];
        }
    }
}