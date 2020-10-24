using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FractalElementDesigner.IO;
using FractalElementDesigner.MathModel;
using System.Threading.Tasks;
using FractalElementDesigner.StructureSchemeSynthesis;
using GalaSoft.MvvmLight.Threading;
using FractalElementDesigner.FEEditing.Controls;
using FractalElementDesigner.SchemeEditing;
using FractalElementDesigner.MathModel.Structure;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using FractalElementDesigner.FEEditing;
using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.FEEditing.Model.StructureElements;
using FractalElementDesigner.Navigating.Interfaces;
using FractalElementDesigner.Pages;
using FractalElementDesigner.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FractalElementDesigner.ViewModels
{
    /// <summary>
    /// ViewModel страницы анализа и синтеза
    /// </summary>
    class StructureDesigningPageViewModel : INotifyPropertyChanged, IPageViewModel
    {
        public StructureDesigningPageViewModel()
        {
            InitializeCommands();

            SchemeSynthesizer.OnDoWork += OnDoWork;
            SchemeSynthesizer.OnStateChange += OnProgressStateChange;
            StructureCreator.OnDoWork += OnDoWork;
            StructureCreator.OnStateChange += OnProgressStateChange;

            StructureSynthesizer.OnDoWork += OnDoWork;
            StructureSynthesizer.OnStateChange += OnProgressStateChange;

            IToolCreator toolCreator = new ToolCreator();

            EditorTools.Add("Numerate", toolCreator.CreateTool(ToolType.ContactNumerator));
            EditorTools.Add("Cut", toolCreator.CreateTool(ToolType.CutCellDisposer));
            EditorTools.Add("RC", toolCreator.CreateTool(ToolType.RCCellDisposer));
            EditorTools.Add("R", toolCreator.CreateTool(ToolType.RCellDisposer));
            EditorTools.Add("Contact", toolCreator.CreateTool(ToolType.ContactCellDisposer));
            EditorTools.Add("Forbid", toolCreator.CreateTool(ToolType.ForbidContactDisposer));
            EditorTools.Add("Shunt", toolCreator.CreateTool(ToolType.ShuntCellDisposer));

            foreach (var tool in EditorTools.Values)
            {
                tool.PropertyChanged += (object sender, PropertyChangedEventArgs e) => 
                {
                    if (sender as Tool != null)
                    {
                        var _tool = (Tool)sender;

                        if (e.PropertyName == nameof(_tool.IsChecked))
                        {
                            if (_tool.IsChecked == true)
                            {
                                SelectedTool = _tool;
                            }
                        }
                    }
                };
            }

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

        /// <summary>
        /// Ссылка на страницу
        /// </summary>
        private StructureDesigningPage _Page;

        #endregion

        #region Свойства

        /// <summary>
        /// Словарь инструментов для редактирования структуры
        /// </summary>
        public Dictionary<string, IEditingTool> EditorTools { get; set; } = new Dictionary<string, IEditingTool>();

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
                selectedProjectTreeItem = value;
                SetChosenProjectTreeItemEnvironment(value);
 
                RaisePropertyChanged(nameof(SelectedProjectTreeItem));
            }
        }

        private IEditingTool selectedTool = null;
        /// <summary>
        /// Выбранный инструмент
        /// </summary>
        public IEditingTool SelectedTool 
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
            get { return structureEeditorVisibility; }
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
            get { return schemeEditorVisibility; }
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
            get { return homePageVisibility; }
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
            get{ return plotVisibility; }
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

        #endregion

        #region Команды

        private ICommand testCommand;
        public ICommand TestCommand
        {
            get { return testCommand; }
            set
            {
                testCommand = value;
                RaisePropertyChanged(nameof(TestCommand));
            }
        }

        private ICommand newProjectCommand;
        /// <summary>
        /// Команда для создания нового проекта
        /// </summary>
        public ICommand NewProjectCommand
        {
            get { return newProjectCommand; }
            set
            {
                newProjectCommand = value;
                RaisePropertyChanged(nameof(NewProjectCommand));
            }
        }

        private ICommand cellApplyToolCommand;
        /// <summary>
        /// Команда для применения инструмента к ячейке элемента
        /// </summary>
        public ICommand CellApplyToolCommand
        {
            get { return cellApplyToolCommand; }
            set
            {
                cellApplyToolCommand = value;
                RaisePropertyChanged(nameof(CellApplyToolCommand));
            }
        }  

        private ICommand loadProjectCommand;
        /// <summary>
        /// Команда для загрузки проекта
        /// </summary>
        public ICommand LoadProjectCommand
        {
            get { return loadProjectCommand; }
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
            get { return saveProjectCommand; }
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
            get { return schemeSynthesisCommand; }
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
            get { return сreateStructureCommand; }
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
            get { return choiceOfSchemeCommand; }
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
                _Page.schemeEditorControl.SchemeControl.Editor = scheme.Editor;
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

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            NewProjectCommand = new RelayCommand(CreateNewProject);
            LoadProjectCommand = new RelayCommand(LoadProject);
            SaveProjectCommand = new RelayCommand(SaveProject);

            // удалить
            TestCommand = new RelayCommand(Test);
            // удалить

            ChoiceOfSchemeCommand = new RelayCommand(ChoiceOfScheme, IsChoiceOfSchemePossible);

            SchemeSynthesisCommand = new RelayCommand(SynthesizeScheme, IsSchemeSynthesisPossible);
            CreateStructureCommand = new RelayCommand(CreateStructure, IsStructureCreatingPossible);

            CellApplyToolCommand = new RelayCommand<Cell>(ApplyToolForElementCell);
        }

        // удалить
        private bool TestingBool = false;
        private void Test() 
        {
            RCWorkbenchLibrary.TestMeth();

            TestingBool = true;
            CreateNewProject();
        }
        // удалить

        /// <summary>
        /// Метод для применения инструмента к ячейке элемента
        /// </summary>
        /// <param name="cell">Объект ячейки</param>
        private void ApplyToolForElementCell(Cell cell) 
        {
            cell.ApplyTool(SelectedTool);
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

                if (project.Items.Where(x => x is FElementScheme).Count() > 1)
                {
                    return false;
                }
                else if (project.Items.Where(x => x is FElementScheme).Count() == 1)
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
        private void SynthesizeScheme() 
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

                StartSchemeSynthesisAsync(structureSchemeSynthesisParametersViewModel.StructureSchemeSynthesisParametersInstance, project, scheme);
            }
        }

        // Метод для сохранения проекта
        private void SaveProject() 
        {
            try
            {
                //var project = SelectedProject;

                //if (SelectedProject == null)
                //{
                //    _dialogCoordinator.ShowMessageAsync(this, "", "Не выбран проект для сохранения");
                //    return;
                //}
                //else
                //{
                //    var dialog = new CommonSaveFileDialog();
                //    ConfigureDialogForProjectSaving(ref dialog);
                //    CommonFileDialogResult result = dialog.ShowDialog();

                //    if (result == CommonFileDialogResult.Ok)
                //    {
                //        var savingProject = ProjectConverter.Convert(project);
                //        ProjectSaver.SaveProject(savingProject, dialog.FileName);
                //    }
                //}
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
                            var editor = new Editor() { Context = new FEEditing.Context() };
                            editor.Context.CurrentCanvas = _Page.structureEditorControl.FEControl.CreateFECanvas();

                            layer.Editor = editor;

                            FEEditing.Insert.ExistingStructureLayer(editor.Context.CurrentCanvas as FECanvas, structure, layer);
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

                StartSchemeSynthesisAsync(structureSchemeSynthesisParametersViewModel.StructureSchemeSynthesisParametersInstance, project, schemePrototype);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Метод для асинхронного создания конструкции элемента
        private async void CreateStructureAsync(Project currentProject, FElementScheme scheme, RCStructure _structure) 
        {
            //удалить

            // TODO организовать получение параметров синтеза СХЕМЫ

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

            // TODO организовать получение параметров синтеза

            //удалить



            await Task.Run(() =>
            {
                try
                {
                    scheme.IsLocked = true;

                    // создать конструкцию элемента
                    var structure = StructureCreator.Create(scheme, _structure);

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        StructureCreator.InsertVisual(structure, _Page.structureEditorControl.FEControl);
                        currentProject.Items.Add(structure);
                    });

                    // тест
                    // тест
                    
                    // TODO выполнить по выбору пользователя
                    StartStructureSynthesisAsync(structureSchemeSynthesisParametersViewModel.StructureSchemeSynthesisParametersInstance, currentProject, structure);
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

        // Метод для запуска синтеза конструкции асинхронно
        private async void StartStructureSynthesisAsync(StructureSchemeSynthesisParameters structureSchemeSynthesisParametersInstance, Project currentProject, RCStructureBase structure)
        {
            await Task.Run(() =>
            {
                try
                {
                    // синтезировать конструкцию
                    StructureSynthesizer.Synthesize(structureSchemeSynthesisParametersInstance, structure);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    _dialogCoordinator.ShowMessageAsync(this, "", "Ошибка синтеза" + Environment.NewLine + ex.Message);
                }
            });
        }


        // Метод для запуска синтеза схемы асинхронно
        private async void StartSchemeSynthesisAsync(StructureSchemeSynthesisParameters structureSchemeSynthesisParametersInstance, Project currentProject, FElementScheme scheme) 
        {
            await Task.Run(() =>
            {
                try
                {
                    // синтезировать схему
                    var schemes = SchemeSynthesizer.Synthesize(structureSchemeSynthesisParametersInstance, scheme);

                    for (int i = 0; i < schemes.Count; i++)
                    {
                        var _scheme = schemes[i];
                        _scheme.Name = "Схема №" + (i+1);

                        // Инициализировать график
                        var plot = _scheme.Elements.Where(x => x is PRPlot).SingleOrDefault() as PRPlot;
                        PRPlot.InitializatePhaseResponsePlot(_scheme.Model.PhaseResponsePoints, plot);

                        // Создать отображение схемы из полученной модели
                        CreateSchemeInEditor(_scheme);

                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            currentProject.Items.Add(_scheme);
                        });               
                    }

                    //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
                    //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
                    //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
                    if (TestingBool) 
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            SelectedProjectTreeItem = schemes[0];
                            ChoiceOfScheme();
                            CreateStructure();
                        });
                    }
                    //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
                    //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
                    //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    _dialogCoordinator.ShowMessageAsync(this, "", "Ошибка синтеза" + Environment.NewLine + ex.Message);
                } 
            });
        }

        // Метод для отображения схемы 
        private void CreateSchemeInEditor(FElementScheme scheme) 
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => 
            {
                var canvas = _Page.schemeEditorControl.SchemeControl.CreateSchemeCanvas();

                if (canvas == null)
                    return;

                var editor = _Page.schemeEditorControl.InitializeNewEditor(canvas);
                scheme.Editor = editor;

                SchemeVisualizator.InsertSections(scheme);
            });
     
        }

        /// <summary>
        /// Метод для установки ссылки на страницу
        /// </summary>
        /// <param name="page"></param>
        public void SetPage(Page page)
        {
            _Page = (StructureDesigningPage)page;

            //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
            //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
            //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
            if (TestingBool)
            {
                _Page.schemeEditorControl.SchemeControl.Loaded += (object sender, RoutedEventArgs e) =>
                {
                    // создать проект
                    var project = new Project() { Name = "Проект №1" };

                    Projects.Add(project);

                    var plot = new PRPlot();

                    var schemePrototype = new FElementScheme(new StructureSchemeSynthesisParameters().FESections) { Name = "Схема", Elements = { plot } };

                    schemePrototype.Model.PhaseResponsePoints = SchemePhaseResponseCalculatorByFrequencies.CalculatePhaseResponseInScheme(
                    1, 3, 50, schemePrototype.Model);

                    PRPlot.InitializatePhaseResponsePlot(schemePrototype.Model.PhaseResponsePoints, plot);

                    // Создать отображение схемы из полученной модели
                    CreateSchemeInEditor(schemePrototype);

                    project.Items.Add(schemePrototype);

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

                    CreateStructureAsync(project, schemePrototype, newStructureWindowViewModel.CurrentStructure);
                };
            }
            //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
            //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
            //для теста!!!!!!!!!!!!!!!!!!!!!!//удалить
        }

        #endregion

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для поднятия события изменения свойства
        /// </summary>
        /// <param name="propName">Имя свойства</param>
        protected virtual void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
