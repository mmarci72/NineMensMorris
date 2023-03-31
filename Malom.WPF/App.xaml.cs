using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Malom.Model;
using Malom.Persistence;
using Malom.WPF.ViewModel;
using Microsoft.Win32;

namespace Malom.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    
    public partial class App : Application
    {
        private MalomGameModel gameModel;
        private MalomViewModel viewModel;
        private MainWindow view;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            
            gameModel = new MalomGameModel(new MalomFileDataAccess());
            gameModel.GameOver += Model_GameOver;
            gameModel.NewGame();

            
            viewModel = new MalomViewModel(ref gameModel);
            viewModel.NewGame += ViewModel_NewGame;
            viewModel.ExitGame += ViewModel_ExitGame;
            viewModel.LoadGame += ViewModel_LoadGame;
            viewModel.SaveGame += ViewModel_SaveGame;
            viewModel.WrongStep += ViewModel_WrongStep;

           
            view = new MainWindow();
            view.DataContext = viewModel;
            view.Closing += View_Closing;
            view.Show();
        }

        private void ViewModel_WrongStep(object? sender, ExceptionEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            gameModel.NewGame();
          
        }

        private void ViewModel_LoadGame(object? sender, System.EventArgs e)
        {


            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Load Malom table";
                openFileDialog.Filter = "Malom table|*.stl";
                if (openFileDialog.ShowDialog() == true)
                {
                    viewModel.Path = openFileDialog.FileName;

                    //viewModel = new MalomViewModel(ref gameModel);
                }

                
            }
            catch (MalomDataException)
            {
                MessageBox.Show("Error loading file!", "Malom", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {


            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save Malom Table";
                saveFileDialog.Filter = "Malom table|*.stl";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        // játéktábla mentése
                        await gameModel.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (MalomDataException)
                    {
                        MessageBox.Show("Error saving game!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error saving file!", "Malom", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewModel_ExitGame(object? sender, System.EventArgs e)
        {
            view.Close(); 
        }

        private void Model_GameOver(object? sender, MalomEventArgs e)
        {

            if (e.Winner == Values.Player1 || e.Winner == Values.Player2)
            {
                MessageBox.Show($"The winner is {e.Winner}!",
                    "Game over",
                    MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("It's a draw!",
                    "Game over",
                    MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
            }
        }

        private void View_Closing(object? sender, CancelEventArgs e)
        {


            if (MessageBox.Show("Are you sure you want to exit?", "Malom", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást
                
            }
        }
    }
}
