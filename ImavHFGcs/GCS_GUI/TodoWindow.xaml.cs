using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using HighFlyers.Various;

namespace HighFlyers
{
    /// <summary>
    /// Interaction logic for TodoWindow.xaml
    /// </summary>
    public partial class TodoWindow
    {
        private readonly IEnumerable<ToDoElement> todoList;

        public TodoWindow(IEnumerable<ToDoElement> todoList)
        {
            InitializeComponent();
            KeyUp += TodoWindow_PreviewKeyUp;

            this.todoList = todoList;
            ReloadView();
        }

        void TodoWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelTakeOff();
            }
        }

        private void CancelTakeOff()
        {
            DialogResult = false;
            Close();
        }

        private void ReloadView()
        {
            mainGrid.Children.Clear();
            GenerateColumns();
            
            int i = 0;
            bool isOK = true;
            
            foreach (var element in todoList)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
                var lbl = MakeLabel(element.Description);
                Control doitElement;
                if (element.CheckMethod())
                    doitElement = MakeLabel("YES!");
                else
                {
                    isOK = false;
                    ToDoElement element1 = element;
                    if (element1.FixmeMethod != null)
                    {
                        doitElement = new Button { Content = "No, do it for me!", Background = new SolidColorBrush(Colors.Red) };
                        ToDoElement element2 = element;
                        (doitElement as Button).Click += (sender, args) => DoFixme(element2);
                    }
                    else
                    {
                        doitElement = new Button { Content = "Refresh status", Background = new SolidColorBrush(Colors.Red) };
                        (doitElement as Button).Click += (sender, args) => ReloadView();
                    }
                }
                InsertElementToGrid(doitElement, i, 1);
                InsertElementToGrid(lbl, i++, 0);
            }

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
            var refreshButton = new Button {Content = "Refresh"};
            refreshButton.Click += (sender, args) => ReloadView();
            InsertElementToGrid(refreshButton, i, 0);
            
            var doneButton = new Button { Content = isOK? "Everythink works fine\nTAKE OFF!" : "I know what I'm doing\nTAKE OFF!" };
            if (!isOK)
                doneButton.Background = new SolidColorBrush(Colors.Red);
            doneButton.Click += (sender, args) =>
                {
                    DialogResult = true;
                    Close();
                };
            InsertElementToGrid(doneButton, i, 1);
        }

        private void GenerateColumns()
        {
            for (int i = 0; i < 2; i++)
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(150, GridUnitType.Pixel)});
        }

        private Label MakeLabel(string content)
        {
            return new Label
            {
                Margin = new Thickness(10, 10, 10, 10),
                Content = content,
                Foreground = new SolidColorBrush(Colors.White)
            };
        }

        private void InsertElementToGrid(UIElement element, int row, int column)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            mainGrid.Children.Add(element);
        }

        private void DoFixme(ToDoElement element)
        {
            element.FixmeMethod();
            if (element.CheckMethod())
                ReloadView();
            else
            {
                var timer = new DispatcherTimer
                    {
                        Interval = new TimeSpan(0, 0, 0, 1),
                    };
                timer.Tick += (sender, args) => ReloadView();
                timer.Start();
            }
        }
    }
}
