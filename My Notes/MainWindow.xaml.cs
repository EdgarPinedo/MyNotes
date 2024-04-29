using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace My_Notes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = this;
            noteTitles = new ObservableCollection<string>();
            LoadFileNames();
            InitializeComponent();
        }

        private ObservableCollection<string> noteTitles;
        public ObservableCollection<string> NoteTitles
        {
            get { return noteTitles; }
            set { noteTitles = value; }
        }

        private string currentNote = string.Empty;
        private bool newNote = false;

        readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Notes");

        private void LoadFileNames()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                noteTitles.Add(file.Split(Path.DirectorySeparatorChar).Last());
            }
        }

        private void Save(object sender, RoutedEventArgs args)
        {
            string src = path + @"\" + (string)notesList.SelectedItem;
            string name = title.Text;
            string dest = path + @"\" + name;

            if(newNote == true)
            {
                noteTitles.Add(name);
                notesList.SelectedItem = name;
                newNote = false;
            }

            if (name != currentNote && newNote == false)
            {
                File.Move(src, dest);
                var index = noteTitles.IndexOf(currentNote);
                noteTitles[index] = name;
                currentNote = name;
                title.Text = currentNote;
            }
            SaveXaml(dest);
        }

        private void ChangeCurrentNote(object sender, RoutedEventArgs args)
        {
            dockpanel.Visibility = Visibility.Visible;
            LoadXaml(path + @"\" + (string)notesList.SelectedItem);
            textBox.IsUndoEnabled = false;
            textBox.IsUndoEnabled = true;
            currentNote = (string)notesList.SelectedItem;
            title.Text = currentNote;
        }

        private void CanSave(object sender, RoutedEventArgs args)
        {
            if(ValidateTitle(title.Text))
            {
                saveBtn.IsEnabled = true;
            }
            else
            {
                saveBtn.IsEnabled = false;
            }
        }

        private static bool ValidateTitle(string title)
        {
            string[] chars = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|"};

            if (string.IsNullOrEmpty(title))
            {
                return false;
            }

            foreach(var _char in chars)
            {
                if (title.Contains(_char))
                {
                    return false;
                }
            }
            return true;
        }

        private void Delete(object sender, RoutedEventArgs args)
        {
            DeleteXaml(path + @"\" + (string)notesList.SelectedItem);
        }

        void SaveXaml(string fileName)
        {
            TextRange range;
            FileStream fStream;
            range = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            fStream = new FileStream(fileName, FileMode.Create);
            range.Save(fStream, DataFormats.XamlPackage);
            fStream.Close();
            notesList.SelectedItem = currentNote;
        }

        void LoadXaml(string fileName)
        {
            TextRange range;
            FileStream fStream;
            if (File.Exists(fileName))
            {
                range = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
                fStream = new FileStream(fileName, FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.XamlPackage);
                fStream.Close();
            }
        }

        void DeleteXaml(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            noteTitles.Remove(currentNote);
            notesList.SelectedItem = null;
            dockpanel.Visibility = Visibility.Hidden;
        }

        private void AddNote(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            textBox.Document.Blocks.Clear();
            textBox.IsUndoEnabled = false;
            textBox.IsUndoEnabled = true;
            dockpanel.Visibility = Visibility.Visible;
            notesList.SelectedItem = null;
            newNote = true;
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DragWindow(object sender, RoutedEventArgs e)
        {
            DragMove();
        }
    }
}
