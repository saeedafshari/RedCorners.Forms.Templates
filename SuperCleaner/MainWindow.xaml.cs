using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;

namespace SuperCleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class ListItem
        {
            public bool IsSelected { get; set; } = true;
            public string Path { get; set; }
            public bool IsDirectory { get; set; }
        }

        string[] Folders = new string[]
        {
            @"E:\Projects\RedCorners.Forms"
        };

        public MainWindow()
        {
            InitializeComponent();
            new[]
            {
                chkSubdirectories,
                chkBin,
                chkObj,
                chkVs,
                chkUser,
                chkPackages
            }.ToList().ForEach(x =>
            {
                x.Checked += ChkChecked;
                x.Unchecked += ChkChecked;
            });
        }

        private void ChkChecked(object sender, RoutedEventArgs e)
        {
            ReloadList();
        }

        public void LoadFolders(string[] folders)
        {
            Folders = folders;
            ReloadList();
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete.IsEnabled = false;
            lst.IsEnabled = false;
            panelchk.Visibility = Visibility.Collapsed;

            if (lst.ItemsSource is ObservableCollection<ListItem> files)
            {
                while (files.Count > 0)
                {
                    var file = files[0];
                    if (file.IsSelected)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                if (file.IsDirectory) Directory.Delete(file.Path, true);
                                else File.Delete(file.Path);
                            }
                            catch (Exception ex)
                            {

                            }
                        });
                    }
                    files.RemoveAt(0);
                }
            }

            Application.Current.Dispatcher.Invoke(() => Close());
        }

        void ReloadList()
        {
            List<ListItem> files = new List<ListItem>();
            var option = chkSubdirectories.IsChecked.GetValueOrDefault() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var folder in Folders)
            {
                foreach (var item in Directory.EnumerateDirectories(folder, "*.*", option))
                {
                    DirectoryInfo di = new DirectoryInfo(item);
                    if (di.Name == "bin" && chkBin.IsChecked.GetValueOrDefault())
                        files.Add(new ListItem { Path = item, IsDirectory = true });
                    if (di.Name == "obj" && chkObj.IsChecked.GetValueOrDefault())
                        files.Add(new ListItem { Path = item, IsDirectory = true });
                    if (di.Name == ".vs" && chkVs.IsChecked.GetValueOrDefault())
                        files.Add(new ListItem { Path = item, IsDirectory = true });
                    if (di.Name == "packages" && chkPackages.IsChecked.GetValueOrDefault())
                        files.Add(new ListItem { Path = item, IsDirectory = true });
                }
                if (chkUser.IsChecked.GetValueOrDefault())
                {
                    foreach (var item in Directory.EnumerateFiles(folder, "*.user", option))
                        files.Add(new ListItem { Path = item });
                }
            }
            lst.ItemsSource = new ObservableCollection<ListItem>(files);
        }
    }
}
