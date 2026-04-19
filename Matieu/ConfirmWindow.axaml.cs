using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Matieu
{
    public partial class ConfirmWindow : Window
    {
        public ConfirmWindow() => InitializeComponent();

        private void BtnYes_Click(object? sender, RoutedEventArgs e) => Close(true);
        private void BtnNo_Click(object? sender, RoutedEventArgs e) => Close(false);
    }
}