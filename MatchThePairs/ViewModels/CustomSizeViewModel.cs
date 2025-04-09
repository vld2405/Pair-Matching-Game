using MatchThePairs.Models;
using MatchThePairs.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace MatchThePairs.ViewModels
{
    class CustomSizeViewModel : ViewModelBase
    {
        private string _width;
        public string Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private string _height;
        public string Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public int CustomWidth { get; private set; }
        public int CustomHeight { get; private set; }

        public ICommand CommandConfirm { get; private set; }
        public ICommand CommandCancel { get; private set; }

        public CustomSizeViewModel()
        {
            Width = "4";
            Height = "4";

            CommandConfirm = new RelayCommand(Confirm);
            CommandCancel = new RelayCommand(Cancel);
        }

        private void Confirm()
        {
            if (ValidateInput())
            {
                CustomWidth = int.Parse(Width);
                CustomHeight = int.Parse(Height);
                CloseDialogWithResult(true);
            }
        }

        private void Cancel()
        {
            CloseDialogWithResult(false);
        }

        private bool ValidateInput()
        {
            if (!int.TryParse(Width, out int width) || width < 2 || width > 10)
            {
                MessageBox.Show("Width must be a number between 2 and 10.", "Invalid Input",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(Height, out int height) || height < 2 || height > 10)
            {
                MessageBox.Show("Height must be a number between 2 and 10.", "Invalid Input",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if ((width * height) % 2 != 0)
            {
                MessageBox.Show("The total number of cells (width × height) must be even.",
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void CloseDialogWithResult(bool result)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    if (window is CustomSizeView customSizeView)
                    {
                        customSizeView.DialogResult = result;
                        customSizeView.Close();
                    }
                    break;
                }
            }
        }
    }
}