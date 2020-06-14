using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.FEEditor;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Controls;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Core;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model.Cells;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Tools;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.Pages;
using RC_FE_Design___Analysis_and_synthesis.ProjectTree;
using RC_FE_Design___Analysis_and_synthesis.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    /// <summary>
    /// ViewModel страницы анализа
    /// </summary>
    public class AnalysisViewModel : ViewModelBase, IPageViewModel
    {
        public AnalysisViewModel()
        {
            InitializeCommands();

            EditorTools = new Dictionary<string, Tool>()
            {
                {
                    "Numerate",
                    new Tool()
                    {
                        Name = "Нумерация КП",
                        ImageURI = "pack://application:,,,/Resources/button0.png"
                    }
                },

                {
                    "Cut", new Tool()
                    {
                        Name = "Разрез",
                        ImageURI = "pack://application:,,,/Resources/button1.png"
                    }
                },

                {
                    "RC",
                    new Tool()
                    {
                        Name = "RC-ячейка",
                        ImageURI = "pack://application:,,,/Resources/button2.png"
                    }
                },

                {
                    "R", new Tool()
                    {
                        Name = "R-ячейка",
                        ImageURI = "pack://application:,,,/Resources/button3.png"
                    }
                },

                {
                    "Contact",
                    new Tool()
                    {
                        Name = "Контактная площадка",
                        ImageURI = "pack://application:,,,/Resources/button4.png"
                    }
                },

                {
                    "Forbid", new Tool()
                    {
                        Name = "Запрет КП",
                        ImageURI = "pack://application:,,,/Resources/button5.png"
                    }
                },

                {
                    "Shunt",
                    new Tool()
                    {
                        Name = "Шунт",
                        ImageURI = "pack://application:,,,/Resources/button6.png"
                    }
                },
            };

            foreach (var tool in EditorTools.Values)
            {
                tool.PropertyChanged += Tool_PropertyChanged;
            }
        }

        private void Tool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender as Tool != null)
            {
                var tool = (Tool)sender;

                if (e.PropertyName == nameof(tool.IsChecked))
                {
                    if (tool.IsChecked == true)
                    {
                        SelectedTool = tool;
                    }
                }
            }
        }

        public AnalysisViewModel(IDialogCoordinator dialogCoordinator) : this()
        {
            _dialogCoordinator = dialogCoordinator;
        }

        #region Глобальные переменные

        /// <summary>
        /// Объект для вывода диалогов
        /// </summary>
        private IDialogCoordinator _dialogCoordinator;

        #endregion

        #region Свойства

        private object selectedProjectTreeItem;

        public object SelectedProjectTreeItem
        {
            get { return selectedProjectTreeItem; }
            set 
            {
                if (value is Project project)
                {

                }
                else if (value is RCStructure structure)
                {

                }
                else if (value is Layer layer)
                {
                    SelectedLayer = layer;
                }

                selectedProjectTreeItem = value;
                RaisePropertyChanged(nameof(SelectedProjectTreeItem));
            }
        }

        private Layer selectedLayer;
        /// <summary>
        /// Выбранный слой структуры
        /// </summary>
        public Layer SelectedLayer
        {
            get { return selectedLayer; }
            set 
            {
                _Page.FEControl.Editor = value.Editor;
                selectedLayer = value;
                RaisePropertyChanged(nameof(SelectedLayer));
            }
        }

        private Tool selectedTool = null;
        /// <summary>
        /// Выбранный инструмент
        /// </summary>
        public Tool SelectedTool 
        { 
            get => selectedTool;
            set 
            {
                selectedTool = value;
                RaisePropertyChanged(nameof(SelectedTool));
            }
        }

        /// <summary>
        /// Словарь инструментов для редактирования структуры
        /// </summary>
        public Dictionary<string, Tool> EditorTools { get; set; }

        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();
        /// <summary>
        /// Список проектов
        /// </summary>
        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set 
            { 
                _projects = value;
                RaisePropertyChanged(nameof(Projects));
            }
        }

        private Visibility editorVisibility = Visibility.Hidden;

        public Visibility EditorVisibility
        {
            get
            {
                return editorVisibility;
            }
            set
            {
                editorVisibility = value;
                RaisePropertyChanged(nameof(EditorVisibility));
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
                if (EditorVisibility != Visibility.Visible)
                {
                    EditorVisibility = Visibility.Visible;
                }

                // создать проект
                var project = new Project() { Name = "Проект_0" };             

                var newStructure = InitializeStructure(newStructureWindowViewModel.CurrentStructure);

                project.Structures.Add(newStructure);

                Projects.Add(project);

                // вставить слои
                foreach (var layer in newStructure.StructureLayers)
                {
                    var editor = new Editor() { Context = new Context() };
                    editor.Context.CurrentCanvas = _Page.FEControl.CreateFECanvas();

                    layer.Editor = editor;

                    _Page.FEControl.Editor = editor;

                    Insert.StructureLayer(_Page.FEControl.Editor.Context.CurrentCanvas, layer, layer.CellsType);
                }

                _Page.FEControl.Editor = newStructure.StructureLayers.First().Editor;

                _Page.FEControl.ZoomToFit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Метод для инициализации структуры
        private RCStructure InitializeStructure(RCStructure structure)
        {
            // извлечь число ячеек по горизонтали структуры
            structure.StructureProperties.TryGetValue("HorizontalCellsCount", out var horizontalStructureDimension);
            var horizontalStructureDimensionValue = (int)horizontalStructureDimension.Value + 2;// +2 добавляется для учёта контактных площадок
                                                                                                // извлечь число ячеек по вертикали структуры
            structure.StructureProperties.TryGetValue("VerticalCellsCount", out var verticalStructureDimension);
            var verticalStructureDimensionValue = (int)verticalStructureDimension.Value + 2;// +2 добавляется для учёта контактных площадок
            // новая структура
            var newStructure = structure;

            foreach (var layer in newStructure.StructureLayers)
            {
                for (int r = 0; r < verticalStructureDimensionValue; r++)
                {
                    var row = new ObservableCollection<StructureCellBase>();

                    for (int c = 0; c < horizontalStructureDimensionValue; c++)
                    {
                        row.Add(new StructureCellBase());
                    }

                    layer.StructureCells.Add(row);
                }
            }

            return newStructure;
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
