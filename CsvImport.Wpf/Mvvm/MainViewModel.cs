using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CsvImport.Database;
using CsvImport.Model;
using Microsoft.Win32;

namespace CsvImport.Wpf.Mvvm
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private readonly IDbManager _dbManager;
        private readonly Importer _importer;
        private bool _connected;

        public MainViewModel(IDbManager dbManager)
        {
            _dbManager = dbManager;

            _importer = new Importer(dbManager);

            InitCommands();
        }

        // TODO: Edit in UI.
        public int PageSize { get; set; } = 50;

        public async Task Connect()
        {
            Loading(status: "Подключение к базе данных...");
            try
            {
                await _dbManager.Init();
                _connected = true;
            }
            catch (ImporterException ex)
            {
                ShowError(ex);
            }
            Loading(false);

            await LoadRecords();
        }

        public async Task StartEditItem()
        {
            if (SelectedItem == null)
                return;

            await LoadRecord();
        }

        private void InitCommands()
        {
            LoadFiles = new Command(async _ => await ImportFile(), false);
            SavePeople = new Command(async _ => await SaveRecord());
            CancelPeople = new Command(async _ => await ResetRecord());
            Prev = new Command(async _ =>
            {
                _page--;
                await LoadRecords();
            }, false);
            Next = new Command(async _ =>
            {
                _page++;
                await LoadRecords();
            }, false);
        }

        private async Task ImportFile()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                Loading(status: "Загрузка файлов...");
                var imported = await _importer.ImportAsync(dialog.FileName);
                await LoadRecords();
                Loading(false, doneStatus: $"Импортировано {imported} записей");
            }
        }

        private async Task SaveRecord()
        {
            try
            {
                await _dbManager.Update(People);
            }
            catch (ImporterException ex)
            {
                ShowError(ex);
            }
            await LoadRecords();
        }

        private Task ResetRecord()
        {
            People = null;
            IsEditing = false;
            return Task.FromResult((object) null);
        }

        private async Task LoadRecords()
        {
            if (!_connected)
                return;

            Loading();

            try
            {
                Result = await _dbManager.Load(_page, PageSize);
                UpdatePaging();
                Loading(false);
            }
            catch (ImporterException ex)
            {
                ShowError(ex);
                _connected = false;
                Loading(false, doneStatus: ex.Message);
            }

            await ResetRecord();
        }

        private async Task LoadRecord()
        {
            try
            {
                People = await _dbManager.Load(SelectedItem.Id);
                IsEditing = true;
            }
            catch (ImporterException ex)
            {
                ShowError(ex);
                IsEditing = false;
            }
        }

        private void UpdatePaging()
        {
            if (!_connected)
                return;

            Prev.Enabled = Result.Page > 1;
            Next.Enabled = Result.Page < Result.Pages;
            PageText = $"Страница {Result?.Page} из {Result?.Pages}. Всего записей: {Result?.Count}";
        }

        private void Loading(bool loading = true, string status = "Загрузка...", string doneStatus = "")
        {
            LoadFiles.Enabled = !loading && _connected;
            IsLoading = loading;
            Status = loading ? status : doneStatus;
        }

        private static void ShowError(ImporterException ex)
        {
            MessageBox.Show(ex.Message, "Ошибка");
        }

        #region ViewModel Properties

        public Command LoadFiles { get; private set; }

        public Command SavePeople { get; private set; }

        public Command CancelPeople { get; private set; }

        public Command Prev { get; private set; }

        public Command Next { get; private set; }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<People> Peoples
        {
            get { return _peoples; }
            set
            {
                _peoples = value;
                OnPropertyChanged();
            }
        }

        public People SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public People People
        {
            get { return _people; }
            set
            {
                _people = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        public PagedResult<People> Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        public string PageText
        {
            get { return _pageText; }
            set
            {
                _pageText = value;
                OnPropertyChanged();
            }
        }

        private bool _isEditing;
        private bool _isLoading;
        private int _page;
        private string _pageText;
        private People _people;
        private IEnumerable<People> _peoples;
        private PagedResult<People> _result;
        private People _selectedItem;
        private string _status;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}