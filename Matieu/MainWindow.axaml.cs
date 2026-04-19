using Avalonia.Controls;
using Avalonia.Input; // Обязательно для KeyEventArgs
using Avalonia.Interactivity; // Обязательно для RoutedEventArgs
using Matieu.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Matieu
{
    public partial class MainWindow : Window
    {
        private int _currentPage = 1;
        private const int _pageSize = 3;
        private int _totalItems = 0;
        private User? _currentUser;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(User user) : this()
        {
            _currentUser = user;
            LoadCollections();
            LoadData();
        }

        public void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var query = db.Services.Include(s => s.Collection).AsQueryable();

                // Фильтр категории
                string category = rbCustom.IsChecked == true ? "Custom" : "Cosplay";
                query = query.Where(s => s.Category == category);

                // Фильтр поиска
                if (txtSearch != null && !string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string search = txtSearch.Text.ToLower();
                    query = query.Where(s => s.Name.ToLower().Contains(search));
                }

                // Фильтр коллекции
                if (cbCollections?.SelectedItem is Collection col && col.Id != 0)
                {
                    query = query.Where(s => s.CollectionId == col.Id);
                }

                _totalItems = query.Count();

                var list = query
                    .OrderBy(s => s.Id)
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                // Подгружаем картинки
                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(item.ImagePath) && System.IO.File.Exists(item.ImagePath))
                    {
                        try { item.PreviewBitmap = new Avalonia.Media.Imaging.Bitmap(item.ImagePath); } catch { }
                    }
                }

                lstServices.ItemsSource = list;
                UpdatePaginationText();
            }
        }

        private void UpdatePaginationText()
        {
            int start = (_currentPage - 1) * _pageSize + 1;
            int end = _currentPage * _pageSize;
            if (end > _totalItems) end = _totalItems;
            if (_totalItems == 0) start = 0;

            if (txtPagination != null)
                txtPagination.Text = $"{start}-{end} из {_totalItems}";
        }

        // --- ОБРАБОТЧИКИ СОБЫТИЙ С ПРАВИЛЬНЫМИ ТИПАМИ ---

        // Для RadioButton (IsCheckedChanged)
        public void OnFilterChanged(object? sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true)
            {
                _currentPage = 1;
                LoadData();
            }
        }

        // Для ComboBox (SelectionChanged)
        public void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1;
            LoadData();
        }

        // Для TextBox (KeyUp)
        public void OnSearchKeyUp(object? sender, KeyEventArgs e)
        {
            _currentPage = 1;
            LoadData();
        }

        // Остальные кнопки
        private void BtnPrev_Click(object? sender, RoutedEventArgs e) { if (_currentPage > 1) { _currentPage--; LoadData(); } }
        private void BtnNext_Click(object? sender, RoutedEventArgs e) { if (_currentPage * _pageSize < _totalItems) { _currentPage++; LoadData(); } }
        private void BtnExit_Click(object? sender, RoutedEventArgs e) => Close();
        private void BtnMinimize_Click(object? sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
        private void BtnClose_Click(object? sender, RoutedEventArgs e) => Close();

        private void BtnAddService_Click(object? sender, RoutedEventArgs e)
        {
            var win = new ServiceEditWindow();
            win.Closed += (s, ev) => LoadData();
            win.ShowDialog(this);
        }

        private void BtnEditService_Click(object? sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Service service)
            {
                var win = new ServiceEditWindow(service);
                win.Closed += (s, ev) => LoadData();
                win.ShowDialog(this);
            }
        }

        private void LoadCollections()
        {
            using (var db = new AppDbContext())
            {
                var collections = db.Collections.ToList();
                collections.Insert(0, new Collection { Id = 0, Name = "Все коллекции" });
                cbCollections.ItemsSource = collections;
                cbCollections.SelectedIndex = 0;
            }
        }
    }
}