﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.FEEditor;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model.Cells;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Tools;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.Pages;
using RC_FE_Design___Analysis_and_synthesis.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RC_FE_Design___Analysis_and_synthesis.IO;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Views;
using RC_FE_Design___Analysis_and_synthesis.MathModel;
using System.Threading.Tasks;
using RC_FE_Design___Analysis_and_synthesis.StructureSchemeSynthesis;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    /// <summary>
    /// ViewModel страницы анализа и синтеза
    /// </summary>
    class StructureDesigningPageViewModel : ViewModelBase, IPageViewModel
    {
        public StructureDesigningPageViewModel()
        {
            InitializeCommands();

            SchemeSynthesizer.OnDoWork += OnDoWork;
            SchemeSynthesizer.OnStateChange += OnProgressStateChange;
            StructureCreator.OnDoWork += OnDoWork;
            StructureCreator.OnStateChange += OnProgressStateChange;

            EditorTools = new Dictionary<string, Tool>()
            {
                {
                    "Numerate",
                    new Tool()
                    {
                        Name = "Нумерация КП",
                        ImageURI = "pack://application:,,,/Resources/button0.png",
                        Type = ToolType.ContactNumerator
                    }
                },

                {
                    "Cut", new Tool()
                    {
                        Name = "Разрез",
                        ImageURI = "pack://application:,,,/Resources/button1.png",
                        Type = ToolType.CutCellDisposer
                    }
                },

                {
                    "RC",
                    new Tool()
                    {
                        Name = "RC-ячейка",
                        ImageURI = "pack://application:,,,/Resources/button2.png",
                        Type = ToolType.RCCellDisposer
                    }
                },

                {
                    "R", new Tool()
                    {
                        Name = "R-ячейка",
                        ImageURI = "pack://application:,,,/Resources/button3.png",
                        Type = ToolType.RCellDisposer
                    }
                },

                {
                    "Contact",
                    new Tool()
                    {
                        Name = "Контактная площадка",
                        ImageURI = "pack://application:,,,/Resources/button4.png",
                        Type = ToolType.ContactCellDisposer
                    }
                },

                {
                    "Forbid", new Tool()
                    {
                        Name = "Запрет КП",
                        ImageURI = "pack://application:,,,/Resources/button5.png",
                        Type = ToolType.ForbidContactDisposer
                    }
                },

                {
                    "Shunt",
                    new Tool()
                    {
                        Name = "Шунт",
                        ImageURI = "pack://application:,,,/Resources/button6.png",
                        Type = ToolType.ShuntCellDisposer
                    }
                },
            };

            foreach (var tool in EditorTools.Values)
            {
                tool.PropertyChanged += Tool_PropertyChanged;
            }

            StructureCellBase.ApplyToolDelegate = (StructureCellBase cell) =>
            {
                if (SelectedTool != null && SelectedTool.IsChecked == true)
                {
                    if (!CheckToolApplyPossibility(SelectedTool, cell))
                    {
                        return;
                    }

                    switch (SelectedTool.Type)
                    {
                        case ToolType.None:
                            cell.CellType = CellType.None;
                            break;
                        case ToolType.ContactNumerator:
                            
                            break;
                        case ToolType.CutCellDisposer:
                            cell.CellType = CellType.Cut;
                            break;
                        case ToolType.ContactCellDisposer:
                            cell.CellType = CellType.Contact;
                            break;
                        case ToolType.ForbidContactDisposer:
                            cell.CellType = CellType.Forbid;
                            break;
                        case ToolType.RCCellDisposer:
                            cell.CellType = CellType.RC;
                            break;
                        case ToolType.RCellDisposer:
                            cell.CellType = CellType.R;
                            break;
                        case ToolType.ShuntCellDisposer:
                            cell.CellType = CellType.Shunt;
                            break;
                        default:
                            break;
                    }
                }
            };

            HomePageVisibility = Visibility.Visible;
        }

        public StructureDesigningPageViewModel(IDialogCoordinator dialogCoordinator) : this()
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
        /// <summary>
        /// Выбранный элемент дерева проекта
        /// </summary>
        public object SelectedProjectTreeItem
        {
            get { return selectedProjectTreeItem; }
            set 
            {
                InvalidateAllVisualViewers();
                SetChosenProjectTreeItemEnvironment(value);

                selectedProjectTreeItem = value;
                RaisePropertyChanged(nameof(SelectedProjectTreeItem));
            }
        }

        private Project selectedProject = null;
        /// <summary>
        /// Выбранный проект
        /// </summary>
        public Project SelectedProject
        {
            get { return selectedProject; }
            set 
            { 
                selectedProject = value;
                RaisePropertyChanged(nameof(SelectedProject));
            }
        }

        private RCStructure selectedStructure = null;
        /// <summary>
        /// Выбранная структура
        /// </summary>
        public RCStructure SelectedStructure
        {
            get { return selectedStructure; }
            set
            {
                selectedStructure = value;
                RaisePropertyChanged(nameof(SelectedStructure));
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
                _Page.structureEditorControl.FEControl.Editor = value.Editor;
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
            get
            {
                if (selectedTool != null && selectedTool.IsChecked)
                {
                    return selectedTool;
                }
                else
                {
                    return null;    
                }
            }
            set 
            {
                selectedTool = value;
                RaisePropertyChanged(nameof(SelectedTool));
            }
        }

        private StructureSchemeSynthesisParameters currentStructureSchemeSynthesisParameters;

        public StructureSchemeSynthesisParameters CurrentStructureSchemeSynthesisParameters
        {
            get { return currentStructureSchemeSynthesisParameters; }
            set 
            { 
                currentStructureSchemeSynthesisParameters = value;
                RaisePropertyChanged(nameof(CurrentStructureSchemeSynthesisParameters));
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

        private double progressBarValue;
        /// <summary>
        /// Значение строки прогресса
        /// </summary>
        public double ProgressBarValue
        {
            get { return progressBarValue; }
            set 
            { 
                progressBarValue = value;
                RaisePropertyChanged(nameof(ProgressBarValue));
            }
        }

        private string progressState = string.Empty;
        /// <summary>
        /// Статус прогресса
        /// </summary>
        public string ProgressState
        {
            get { return progressState; }
            set
            {
                progressState = value;
                RaisePropertyChanged(nameof(ProgressState));
            }
        }

        private Visibility structureEeditorVisibility = Visibility.Hidden;
        /// <summary>
        /// Видимость редактора структуры
        /// </summary>
        public Visibility StructureEditorVisibility
        {
            get
            {
                return structureEeditorVisibility;
            }
            set
            {
                structureEeditorVisibility = value;
                RaisePropertyChanged(nameof(StructureEditorVisibility));
            }
        }

        private Visibility schemeEditorVisibility = Visibility.Hidden;
        /// <summary>
        /// Видимость редактора схем
        /// </summary>
        public Visibility SchemeEditorVisibility
        {
            get
            {
                return schemeEditorVisibility;
            }
            set
            {
                schemeEditorVisibility = value;
                RaisePropertyChanged(nameof(SchemeEditorVisibility));
            }
        }

        private Visibility homePageVisibility = Visibility.Hidden;
        /// <summary>
        /// Видимость домашней страницы
        /// </summary>
        public Visibility HomePageVisibility
        {
            get
            {
                return homePageVisibility;
            }
            set
            {
                homePageVisibility = value;
                RaisePropertyChanged(nameof(HomePageVisibility));
            }
        }

        private Visibility plotVisibility = Visibility.Hidden;
        /// <summary>
        /// Видимость графика
        /// </summary>
        public Visibility PlotVisibility
        {
            get
            {
                return plotVisibility;
            }
            set
            {
                plotVisibility = value;
                RaisePropertyChanged(nameof(PlotVisibility));
            }
        }

        private bool isProjectTreeEnabled = true;
        /// <summary>
        /// Разрешено ли манипулирование с деревом проекта 
        /// </summary>
        public bool IsProjectTreeEnabled
        {
            get { return isProjectTreeEnabled; }
            set 
            { 
                isProjectTreeEnabled = value;
                RaisePropertyChanged(nameof(IsProjectTreeEnabled));
            }
        }


        /// <summary>
        /// Ссылка на страницу
        /// </summary>
        private StructureDesigningPage _Page { get; set; }

        #endregion

        #region Команды

        private ICommand newProjectCommand;
        /// <summary>
        /// Команда для создания нового проекта
        /// </summary>
        public ICommand NewProjectCommand
        {
            get
            {
                return newProjectCommand;
            }
            set
            {
                newProjectCommand = value;
                RaisePropertyChanged(nameof(NewProjectCommand));
            }
        }

        private ICommand loadProjectCommand;
        /// <summary>
        /// Команда для загрузки проекта
        /// </summary>
        public ICommand LoadProjectCommand
        {
            get
            {
                return loadProjectCommand;
            }
            set
            {
                loadProjectCommand = value;
                RaisePropertyChanged(nameof(LoadProjectCommand));
            }
        }

        private ICommand saveProjectCommand;
        /// <summary>
        /// Команда для сохранения проекта
        /// </summary>
        public ICommand SaveProjectCommand
        {
            get
            {
                return saveProjectCommand;
            }
            set
            {
                saveProjectCommand = value;
                RaisePropertyChanged(nameof(SaveProjectCommand));
            }
        }

        private ICommand schemeSynthesisCommand;
        /// <summary>
        /// Команда для синтеза схемы
        /// </summary>
        public ICommand SchemeSynthesisCommand
        {
            get
            {
                return schemeSynthesisCommand;
            }
            set
            {
                schemeSynthesisCommand = value;
                RaisePropertyChanged(nameof(SchemeSynthesisCommand));
            }
        }

        private ICommand сreateStructureCommand;
        /// <summary>
        /// Команда для создания конструкции элемента
        /// </summary>
        public ICommand CreateStructureCommand
        {
            get
            {
                return сreateStructureCommand;
            }
            set
            {
                сreateStructureCommand = value;
                RaisePropertyChanged(nameof(CreateStructureCommand));
            }
        }

        private ICommand choiceOfSchemeCommand;
        /// <summary>
        /// Команда для выбора схемы
        /// </summary>
        public ICommand ChoiceOfSchemeCommand
        {
            get
            {
                return choiceOfSchemeCommand;
            }
            set
            {
                choiceOfSchemeCommand = value;
                RaisePropertyChanged(nameof(ChoiceOfSchemeCommand));
            }
        }

        /// <summary>
        /// Команда для перемещения на главную страницу 
        /// </summary>
        public ICommand GoToMainPageCommand { get; set; }

        #endregion

        #region Обработчики событий

        /// <summary>
        /// Обработчик выполнения части работы
        /// </summary>
        /// <param name="value">Процент выполнения</param>
        private void OnDoWork(double value)
        {
            ProgressBarValue = value;
        }

        /// <summary>
        /// Обработчик изменения статуса выполнения процесса
        /// </summary>
        /// <param name="state">Статус выполнения</param>
        private void OnProgressStateChange(string state)
        {
            ProgressState = state;
        }

        #endregion

        #region Методы

        private void InvalidateAllVisualViewers() 
        {
            HomePageVisibility = Visibility.Hidden;
            SchemeEditorVisibility = Visibility.Hidden;
            StructureEditorVisibility = Visibility.Hidden;
            PlotVisibility = Visibility.Hidden;
        }

        private void SetChosenProjectTreeItemEnvironment(object value) 
        {
            if (value is Project project)
            {

            }
            else if (value is FElementScheme scheme)
            {
                SchemeEditorVisibility = Visibility.Visible;
            }
            else if (value is PRPlot plot)
            {
                PlotVisibility = Visibility.Visible;
                _Page.plotControl.plotView.Model = plot.Model;
            }
            else if (value is RCStructure structure)
            {
                StructureEditorVisibility = Visibility.Visible;
            }
            else if (value is Layer layer)
            {
                StructureEditorVisibility = Visibility.Visible;
                _Page.structureEditorControl.FEControl.Editor = layer.Editor;
            }
        }

        // метод для проверки возможности применения инструмента к ячейке
        private bool CheckToolApplyPossibility(Tool tool, StructureCellBase cell) 
        {
            var result = true;

            switch (tool.Type)
            {
                case ToolType.None:
                    break;
                case ToolType.ContactNumerator:
                    if (cell.CellType != CellType.Contact) result = false;
                    break;
                case ToolType.CutCellDisposer:
                    if (cell.CellType != CellType.R && cell.CellType != CellType.RC) result = false;
                    break;
                case ToolType.ContactCellDisposer:
                    if (cell.CellType != CellType.PlaceForContact && cell.CellType != CellType.Forbid && cell.CellType != CellType.Shunt) result = false;
                    break;
                case ToolType.ForbidContactDisposer:
                    if (cell.CellType != CellType.PlaceForContact && cell.CellType != CellType.Contact && cell.CellType != CellType.Shunt) result = false;
                    break;
                case ToolType.RCCellDisposer:
                    if (cell.CellType != CellType.R && cell.CellType != CellType.Cut) result = false;
                    break;
                case ToolType.RCellDisposer:
                    if (cell.CellType != CellType.RC && cell.CellType != CellType.Cut) result = false;
                    break;
                case ToolType.ShuntCellDisposer:
                    if (cell.CellType != CellType.PlaceForContact && cell.CellType != CellType.Forbid && cell.CellType != CellType.Contact) result = false;
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Обработчик изменения выбранного инструмента
        /// </summary>
        /// <param name="sender">Объект отправитель события</param>
        /// <param name="e">Объект параметров события</param>
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

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            NewProjectCommand = new RelayCommand(CreateNewProject);
            LoadProjectCommand = new RelayCommand(LoadProject);
            SaveProjectCommand = new RelayCommand(SaveProject);

            ChoiceOfSchemeCommand = new RelayCommand(ChoiceOfScheme, IsChoiceOfSchemePossible);

            SchemeSynthesisCommand = new RelayCommand(SchemeSynthesize, IsSchemeSynthesisPossible);
            CreateStructureCommand = new RelayCommand(CreateStructure, IsStructureCreatingPossible);
        }

        /// <summary>
        /// Метод для выбора схемы
        /// </summary>
        private void ChoiceOfScheme() 
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                if (project.Items.Where(x => x is FElementScheme).Count() == 1)
                    return;

                if (project == null)
                    return;

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    project.Items.Clear();
                    project.Items.Add(scheme);
                });
            }
        }


        // Метод для создания конструкции
        private void CreateStructure()
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
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

                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                CreateStructureAsync(project, scheme, newStructureWindowViewModel.CurrentStructure);
            }
        }

        /// <summary>
        /// Метод определяющий возможность создания конструкции схемы
        /// </summary>
        /// <returns>Разрешающий флаг</returns>
        private bool IsStructureCreatingPossible() 
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                if (scheme.IsLocked)
                {
                    return false;
                }

                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                if (project.Items.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Метод для определения возможности выбора схемы
        /// </summary>
        /// <returns>Разрешающий флаг</returns>
        private bool IsChoiceOfSchemePossible()
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                if (project == null)
                    return false;

                if (project.Items.Where(x => x is FElementScheme).Count() == 1)
                    return false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод определяющий возможность синтеза схемы
        /// </summary>
        /// <returns>Разрешающий флаг</returns>
        private bool IsSchemeSynthesisPossible() 
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                if (scheme.IsLocked)
                {
                    return false;
                }

                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                if (project.Items.Count == 1)
                {
                    return true;
                }              
            }

            return false;
        }

        /// <summary>
        /// Метод для запуска синтеза схемы
        /// </summary>
        private void SchemeSynthesize() 
        {
            if (SelectedProjectTreeItem is FElementScheme scheme)
            {
                // создать окно
                var window = new StructureSchemeSynthesisParametersWindow();
                // создать vm для окна 
                var structureSchemeSynthesisParametersViewModel = new StructureSchemeSynthesisParametersWindowViewModel(window);
                // вывести окно ввода параметров 
                window.DataContext = structureSchemeSynthesisParametersViewModel;
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

                var project = Projects.SingleOrDefault(x => x.Items.Contains(scheme));

                project.Items.Clear();

                StartSynthesisAsync(structureSchemeSynthesisParametersViewModel.StructureSchemeSynthesisParametersInstance, project, scheme);
            }
        }

        // Метод для сохранения проекта
        private void SaveProject() 
        {
            try
            {
                var project = SelectedProject;

                if (SelectedProject == null)
                {
                    _dialogCoordinator.ShowMessageAsync(this, "", "Не выбран проект для сохранения");
                    return;
                }
                else
                {
                    var dialog = new CommonSaveFileDialog();
                    ConfigureDialogForProjectSaving(ref dialog);
                    CommonFileDialogResult result = dialog.ShowDialog();

                    if (result == CommonFileDialogResult.Ok)
                    {
                        var savingProject = ProjectConverter.Convert(project);
                        ProjectSaver.SaveProject(savingProject, dialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Ошибка", "Проект не сохранён" + Environment.NewLine + ex.Message);
            }

            void ConfigureDialogForProjectSaving(ref CommonSaveFileDialog dialog) 
            {
                dialog.Title = "Выберите путь для сохранения проекта";
                dialog.InitialDirectory = Environment.CurrentDirectory;
                dialog.DefaultFileName = "Project1" + "." + "feproj";
                dialog.DefaultExtension = "feproj";
            }
        }

        // Метод для загрузки проекта
        private void LoadProject() 
        {
            try
            {
                var dialog = new CommonOpenFileDialog();
                ConfigureDialogForProjectOpening(ref dialog);
                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {               
                    var savingProject = ProjectSaver.LoadProject(dialog.FileName);
                    var project = ProjectConverter.ConvertBack(savingProject);

                    Projects.Add(project);

                    var _structures = project.Items.Where(x => x is RCStructureBase).Select(x => x as RCStructureBase).ToList();

                    foreach (var structure in _structures)
                    {
                        foreach (var layer in structure.StructureLayers)
                        {
                            var editor = new Editor() { Context = new Context() };
                            editor.Context.CurrentCanvas = _Page.structureEditorControl.FEControl.CreateFECanvas();

                            layer.Editor = editor;

                            Insert.ExistingStructureLayer(editor.Context.CurrentCanvas, layer, layer.CellsType);
                        }
                    }

                    // показать область проектирования, если она скрыта
                    if (StructureEditorVisibility != Visibility.Visible)
                    {
                        StructureEditorVisibility = Visibility.Visible;
                    }

                    //_Page.FEControl.Editor = project.Structures.First().StructureLayers.First().Editor;

                    _Page.structureEditorControl.FEControl.ZoomToFit();
                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Ошибка", "Проект не может быть открыт" + Environment.NewLine + ex.Message);
            }

            void ConfigureDialogForProjectOpening(ref CommonOpenFileDialog dialog)
            {
                dialog.Title = "Выберите проект для открытия";
                dialog.InitialDirectory = Environment.CurrentDirectory;
                dialog.DefaultExtension = "feproj";
            }
        }

        // Метод для создания нового проекта
        private void CreateNewProject()
        {
            try
            { 
                // создать окно
                var window = new StructureSchemeSynthesisParametersWindow();
                // создать vm для окна 
                var structureSchemeSynthesisParametersViewModel = new StructureSchemeSynthesisParametersWindowViewModel(window);
                // вывести окно ввода параметров 
                window.DataContext = structureSchemeSynthesisParametersViewModel;
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

                //////////////////////////////////////////////////
                if (HomePageVisibility == Visibility.Visible)
                {
                    HomePageVisibility = Visibility.Hidden;
                }

                // создать проект
                var project = new Project() { Name = "Проект №1" };

                Projects.Add(project);

                var schemePrototype = new FElementScheme(
                    structureSchemeSynthesisParametersViewModel.StructureSchemeSynthesisParametersInstance.FESections) { Name = "Схема", Elements = { new PRPlot() } };        

                StartSynthesisAsync(structureSchemeSynthesisParametersViewModel.StructureSchemeSynthesisParametersInstance, project, schemePrototype);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Метод для асинхронного создания конструкции элемента
        private async void CreateStructureAsync(Project currentProject, FElementScheme scheme, RCStructure _structure) 
        {
            await Task.Run(() =>
            {
                try
                {
                    scheme.IsLocked = true;

                    // создать конструкцию элемента
                    var structure = StructureCreator.Create(scheme, _structure);

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        currentProject.Items.Add(structure);
                        StructureCreator.InsertVisual(structure, _Page.structureEditorControl.FEControl);
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    _dialogCoordinator.ShowMessageAsync(this, "", "Ошибка создания конструкции" + Environment.NewLine + ex.Message);
                }
                finally
                {
                    scheme.IsLocked = false;
                }
            });
        }

        // Метод для запуска синтеза асинхронно
        private async void StartSynthesisAsync(StructureSchemeSynthesisParameters structureSchemeSynthesisParametersInstance, Project currentProject, FElementScheme scheme) 
        {
            await Task.Run(() =>
            {
                try
                {
                    //scheme.IsLocked

                    // синтезировать схему
                    var schemes = SchemeSynthesizer.Synthesize(structureSchemeSynthesisParametersInstance, scheme);

                    for (int i = 0; i < schemes.Count; i++)
                    {
                        var _scheme = schemes[i];
                        _scheme.Name = "Схема №" + (i+1);

                        var plot = _scheme.Elements.Where(x => x is PRPlot).SingleOrDefault() as PRPlot;

                        PRPlot.InitializatePhaseResponsePlot(_scheme.Model.PhaseResponsePoints, plot);

                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            currentProject.Items.Add(_scheme);
                        });               
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    _dialogCoordinator.ShowMessageAsync(this, "", "Ошибка синтеза" + Environment.NewLine + ex.Message);
                }
                finally 
                {
                    //scheme.IsLocked = false;
                }    
            });
        }

        private void CreateNewStructureProject()
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
                if (StructureEditorVisibility != Visibility.Visible)
                {
                    StructureEditorVisibility = Visibility.Visible;
                }

                // создать проект
                var project = new Project() { Name = "Проект_0" };

                SelectedProject = project;

                var newStructure = StructureCreator.InitializeStructure(newStructureWindowViewModel.CurrentStructure);

                project.Items.Add(newStructure);

                Projects.Add(project);

                // вставить слои
                foreach (var layer in newStructure.StructureLayers)
                {
                    var editor = new Editor() { Context = new Context() };
                    editor.Context.CurrentCanvas = _Page.structureEditorControl.FEControl.CreateFECanvas();

                    layer.Editor = editor;

                    Insert.StructureLayer(editor.Context.CurrentCanvas, layer, layer.CellsType);
                }

                _Page.structureEditorControl.FEControl.Editor = newStructure.StructureLayers.First().Editor;

                _Page.structureEditorControl.FEControl.ZoomToFit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Метод для установки ссылки на страницу
        /// </summary>
        /// <param name="page"></param>
        public void SetPage(Page page)
        {
            _Page = (StructureDesigningPage)page;
        }

        #endregion
    }
}
