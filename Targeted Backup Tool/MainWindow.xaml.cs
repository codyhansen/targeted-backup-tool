using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;


namespace Targeted_Backup_Tool
{
 
	public partial class MainWindow : Window
	{

        private FolderBrowserDialog _dir;
        private string _targetPath;

        public MainWindow()
        {
            _dir = new FolderBrowserDialog();
			InitializeComponent();
        }

        private void SelectTarget(object sender, RoutedEventArgs e)
        {
			DialogResult result = _dir.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _targetPath = _dir.SelectedPath;
                lblTargetPath.Content = _targetPath;
            }
        }

        private void AddPath(object sender, RoutedEventArgs e)
        {
            DialogResult result = _dir.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                lstPaths.Items.Add(_dir.SelectedPath);
            }

        }
        private void DeletePath(object sender, RoutedEventArgs e)
        {
            lstPaths.Items.Remove(lstPaths.SelectedItem);
        }

        private void CreateFiles(string path, string targetDir)
        {
            // Create path directory in target
            string created_dir = targetDir + '\\' + path.Split('\\').Last();
            Directory.CreateDirectory("\\\\?\\" + created_dir);
            // Copy files inside directory
            foreach (string file in Directory.GetFiles(path))
            {
                string new_path = created_dir + "\\" + Path.GetFileName(file);
                if (File.Exists(new_path))
                {
                    File.Delete(new_path);
                }

                string long_file = "\\\\?\\" + file;
                new_path = "\\\\?\\" + new_path;
                File.Copy(long_file, new_path);
            }
            // Check for subdirectories
            string[] sub_dir = Directory.GetDirectories(path);
            if (sub_dir.Length != 0 || sub_dir != null)
            {
                // Recursively call method on each directory
                foreach (string dir in sub_dir)
                {
                    CreateFiles(dir, created_dir);
                }
            }
            
        }

        private void Run(object sender, RoutedEventArgs e)
        {
            List<string> paths = lstPaths.Items.Cast<String>().ToList();

            if (_targetPath == "" || paths.Count == 0)
            {
                lblResult.Foreground = Brushes.Red;
                lblResult.Content = "Please enter a target and paths.";
            } else
            {
                lblResult.Content = "";

                foreach (string path in paths)
                {
                    CreateFiles(path, _targetPath);
                }

                lblResult.Foreground = Brushes.Green;
                lblResult.Content = "Backup completed successfully!";
            }
            
        }
    }
}
