using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.FEEditor;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Core;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model.Cells;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.Pages;
using RC_FE_Design___Analysis_and_synthesis.ProjectTree;
using RC_FE_Design___Analysis_and_synthesis.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    /// <summary>
    /// ViewModel страницы анализа
    /// </summary>
    public class AnalysisViewModel : ViewModelBase, IPageViewModel
    {
        public AnalysisViewModel()
        {

        }

        public AnalysisViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;

            InitializeCommands();
        }

        #region Глобальные переменные

        /// <summary>
        /// Объект для вывода диалогов
        /// </summary>
        private IDialogCoordinator _dialogCoordinator;

        #endregion

        #region Свойства

        private Project _project;

        public Project Project
        {
            get { return _project; }
            set { _project = value; }
        }

        private Visibility canvasVisibility = Visibility.Collapsed;

        public Visibility CanvasVisibility
        {
            get 
            { 
                return canvasVisibility; 
            }
            set 
            { 
                canvasVisibility = value;
                RaisePropertyChanged(nameof(CanvasVisibility));
            }
        }

        /// <summary>
        /// Ссылка на страницу
        /// </summary>
        private AnalysisPage _Page { get; set; }

        #endregion

        #region Команды

        /// <summary>
        /// Команда для перемещения на главную страницу 
        /// </summary>
        public ICommand GoToMainPageCommand { get; set; }

        private ICommand newStructureCommand;
        /// <summary>
        /// Команда для создания новой структуры
        /// </summary>
        public ICommand NewStructureCommand
        {
            get
            {
                return newStructureCommand;
            }
            set
            {
                newStructureCommand = value;
                RaisePropertyChanged(nameof(NewStructureCommand));
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            NewStructureCommand = new RelayCommand(CreateNewStructure);
        }

        // Метод для создания новой структуры
        private void CreateNewStructure() 
        {
            try
            {
                // создать проект
                
                // создать окно
                var window = new NewStructureWindow();
                // создать vm для окна создания новой структуры
                var newStructureWindowViewModel = new NewStructureWindowViewModel(window);
                // вывести окно ввода параметров структуры
                window.DataContext = newStructureWindowViewModel;
                var dialogResult = window.ShowDialog();

                // если не было подтверждения выйти
                if (dialogResult.HasValue == false)
                {
                    return;
                }
                else 
                {
                    if (dialogResult.Value == false)
                    {
                        return;
                    }
                }

                // показать область проектирования, если она скрыта
                if (CanvasVisibility != Visibility.Visible)
                {
                    CanvasVisibility = Visibility.Visible;
                }

                // извлечь число ячеек по горизонтали структуры
                newStructureWindowViewModel.CurrentStructure.StructureProperties.TryGetValue("HorizontalCellsCount", out var horizontalStructureDimension);
                var horizontalStructureDimensionValue = (int)horizontalStructureDimension.Value;
                // извлечь число ячеек по вертикали структуры
                newStructureWindowViewModel.CurrentStructure.StructureProperties.TryGetValue("VerticalCellsCount", out var verticalStructureDimension);
                var verticalStructureDimensionValue = (int)verticalStructureDimension.Value;
                // новая структура
                var newStructure = newStructureWindowViewModel.CurrentStructure;

                for (int r = 0; r < verticalStructureDimensionValue; r++)
                {
                    var row = new List<StructureCellBase>();

                    for (int c = 0; c < horizontalStructureDimensionValue; c++)
                    {
                        row.Add(new StructureCellBase());
                    }

                    newStructure.StructureCells.Add(row);
                }

                Insert.StructureLayer(_Page.FEControl.FECanvas, new PointEx(0, 0), newStructure);

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Метод для установки ссылки на страницу
        /// </summary>
        /// <param name="page"></param>
        public void SetPage(Page page)
        {
            _Page = (AnalysisPage)page;
        }

        #endregion
    }
}
