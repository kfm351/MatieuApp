using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Matieu.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Matieu
{
    public partial class ServiceEditWindow : Window
    {
        private string? _selectedImagePath;
        public Service CurrentService { get; set; }
        public Bitmap? PreviewImage { get; set; }

        // Конструктор для добавления
        public ServiceEditWindow()
        {
            InitializeComponent();
            CurrentService = new Service { Price = 0, Category = "Custom" };
            DataContext = this;
            LoadCollections();
        }

        // Конструктор для редактирования
        public ServiceEditWindow(Service service)
        {
            InitializeComponent();
            CurrentService = service;

            if (!string.IsNullOrEmpty(service.ImagePath) && File.Exists(service.ImagePath))
            {
                try { PreviewImage = new Bitmap(service.ImagePath); } catch { }
            }

            DataContext = this;
            LoadCollections();

            // Устанавливаем значения после загрузки коллекций
            cbCategory.SelectedIndex = service.Category == "Cosplay" ? 1 : 0;

            if (service.CollectionId != null)
            {
                var collections = cbCollection.ItemsSource as List<Collection>;
                cbCollection.SelectedItem = collections?.FirstOrDefault(c => c.Id == service.CollectionId);
            }
        }

        private void LoadCollections()
        {
            using (var db = new AppDbContext())
            {
                cbCollection.ItemsSource = db.Collections.ToList();
            }
        }

        private async void BtnSelectPhoto_Click(object? sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter { Name = "Изображения", Extensions = { "jpg", "jpeg", "png" } });

            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                _selectedImagePath = result[0];
                try
                {
                    PreviewImage = new Bitmap(_selectedImagePath);
                    imgPreview.Source = PreviewImage;
                    txtFileName.Text = Path.GetFileName(_selectedImagePath);
                }
                catch { }
            }
        }

        private void BtnSave_Click(object? sender, RoutedEventArgs e)
        {
            // Проверка на lblError (теперь он есть в XAML)
            if (string.IsNullOrWhiteSpace(txtName.Text) || !decimal.TryParse(txtPrice.Text, out decimal price))
            {
                lblError.Text = "Проверьте название и цену!";
                return;
            }

            using (var db = new AppDbContext())
            {
                CurrentService.Name = txtName.Text;
                CurrentService.Description = txtDesc.Text;
                CurrentService.Price = price;

                // Исправлено получение категории
                if (cbCategory.SelectedItem is ComboBoxItem item)
                    CurrentService.Category = item.Content?.ToString() ?? "Custom";

                // Исправлено получение коллекции
                if (cbCollection.SelectedItem is Collection col)
                    CurrentService.CollectionId = col.Id;
                else
                    CurrentService.CollectionId = null;

                CurrentService.UpdatedAt = DateTime.UtcNow;

                if (_selectedImagePath != null)
                {
                    string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                    if (!Directory.Exists(assetsPath)) Directory.CreateDirectory(assetsPath);
                    string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(_selectedImagePath);
                    string destPath = Path.Combine(assetsPath, newFileName);
                    File.Copy(_selectedImagePath, destPath, true);
                    CurrentService.ImagePath = destPath;
                }

                if (CurrentService.Id == 0) db.Services.Add(CurrentService);
                else
                {
                    db.Services.Attach(CurrentService);
                    db.Entry(CurrentService).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            this.Close(true);
        }

        private void BtnCancel_Click(object? sender, RoutedEventArgs e) => this.Close(false);
    }
}