using MatchThePairs.ViewModels;
using System.Windows;

namespace MatchThePairs.Views
{
    /// <summary>
    /// Interaction logic for CustomSizeView.xaml
    /// </summary>
    public partial class CustomSizeView : Window
    {
        public int CustomWidth
        {
            get
            {
                var viewModel = DataContext as CustomSizeViewModel;
                return viewModel?.CustomWidth ?? 4;
            }
        }

        public int CustomHeight
        {
            get
            {
                var viewModel = DataContext as CustomSizeViewModel;
                return viewModel?.CustomHeight ?? 4;
            }
        }

        public CustomSizeView()
        {
            InitializeComponent();
            DataContext = new CustomSizeViewModel();
        }
    }
}