using Avalonia.Controls;
using Avalonia.Interactivity;
using Matieu.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace Matieu
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        public async void BtnAuth_Click(object? sender, RoutedEventArgs e)
        {
            lblError.Text = ""; // Сброс ошибки

            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Введите логин и пароль"; // Стр. 17 ТЗ
                return;
            }

            using (var db = new AppDbContext())
            {
                var user = db.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Login == txtLogin.Text && u.Password == txtPassword.Text);

                if (user != null)
                {
                    new MainWindow(user).Show();
                    this.Close();
                }
                else
                {
                    lblError.Text = "Пользователь не найден"; // Стр. 17 ТЗ
                }
            }
        }
    }

    // Вспомогательный класс для простых уведомлений (так как в Avalonia нет стандартного MessageBox)
    public static class MsBox
    {
        public static async System.Threading.Tasks.Task Show(Window parent, string title, string text)
        {
            var dialog = new Window { Title = title, Width = 300, Height = 150, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            var okButton = new Button { Content = "ОК", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center };
            okButton.Click += (_, __) => dialog.Close();
            dialog.Content = new StackPanel
            {
                Children = {
                    new TextBlock { Text = text, Margin = new Avalonia.Thickness(20), TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    okButton
                }
            };
            await dialog.ShowDialog(parent);
        }
    }
}