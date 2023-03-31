using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malom.Model;
using Malom.Persistence;
using Microsoft.Win32;

namespace Malom.WPF.ViewModel
{
    class MalomViewModel : ViewModelBase
    {

        private MalomGameModel model;
        private bool isMill;
        private bool isMove;
        private int moveIndex;
        private bool skipTurn;
        private string text;

        private bool saveGameEnabled = true;

        public bool SaveGameEnabled
        {
            get { return saveGameEnabled; }
            set
            {
                saveGameEnabled = value; 
                OnPropertyChanged();
            }
        }


        public String Path { get; set; }


        public Values CurrentPlayer
        {
            get { return model.Table.CurrentPlayer; }
        }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }


        public bool SkipTurn
        {
            get => skipTurn;
            set
            {
                skipTurn = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand NewGameCommand { get; private set; }

        public DelegateCommand LoadGameCommand { get; private set; }

        public DelegateCommand SaveGameCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }
        //public DelegateCommand MillCommand { get; private set; }
        public DelegateCommand SkipCommand { get; private set; }


        public ObservableCollection<MalomField> Fields { get; set; }
        //public ObservableCollection<MalomField> InnerFields { get; set; }
        //public ObservableCollection<MalomField> MiddleFields { get; set; }


        public event EventHandler? NewGame;

        public event EventHandler? LoadGame;

        public event EventHandler? SaveGame;

        public event EventHandler? ExitGame;

        public event EventHandler<ExceptionEventArgs> WrongStep; 




        public MalomViewModel(ref MalomGameModel model)
        {
            isMill = false;
            isMove = false;
            moveIndex = 0;
            skipTurn = false;
            text = "Put a piece onto the table";
            this.model = model;
            this.model.GameAdvanced += Model_GameAdvanced;
           // this.model.GameOver += Model_GameOver;
            this.model.OnMill += OnMill;
            this.model.GameOver += Model_GameOver;
            //this.model.GameCreated += new EventHandler<MalomEventArgs>(Model_GameCreated);

           
            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());
            SkipCommand = new DelegateCommand(param => OnSkipGame());
            //MillCommand = new DelegateCommand(param => OnMill());

            
            Fields = new ObservableCollection<MalomField>();
            for (int i = 0; i < 24; i++) 
            { 
                Fields.Add(new MalomField
                    {
                        BgColor = "Black",
                        Number = i, 
                        StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                    });
            }
            SetLocation();
            
            RefreshTable();
        }

        private void Model_GameOver(object? sender, MalomEventArgs e)
        {
            foreach (var field in Fields)
            {
                field.IsEnabled = false;
                SaveGameEnabled = false;
            }
        }


        private void SetLocation()
        {
           
            Fields[0].X = 135;
            Fields[0].Y = -5;

            Fields[1].X = 390;
            Fields[1].Y = -5;

            Fields[2].X = 630;
            Fields[2].Y = -5;

            Fields[3].X = 630;
            Fields[3].Y = 230;

            Fields[4].X = 630;
            Fields[4].Y = 485;

            Fields[5].X = 390;
            Fields[5].Y = 485;

            Fields[6].X = 135;
            Fields[6].Y = 485;

            Fields[7].X = 135;
            Fields[7].Y = 230;

            Fields[8].X = 195;
            Fields[8].Y = 55;

            Fields[9].X = 390;
            Fields[9].Y = 55;

            Fields[10].X = 570;
            Fields[10].Y = 55;

            Fields[11].X = 570;
            Fields[11].Y = 230;

            Fields[12].X = 570;
            Fields[12].Y = 425;

            Fields[13].X = 390;
            Fields[13].Y = 425;

            Fields[14].X = 195;
            Fields[14].Y = 425;

            Fields[15].X = 195;
            Fields[15].Y = 230;

            Fields[16].X = 250;
            Fields[16].Y = 110;

            Fields[17].X = 390;
            Fields[17].Y = 110;

            Fields[18].X = 515;
            Fields[18].Y = 110;

            Fields[19].X = 515;
            Fields[19].Y = 230;

            Fields[20].X = 515;
            Fields[20].Y = 370;

            Fields[21].X = 390;
            Fields[21].Y = 370;

            Fields[22].X = 250;
            Fields[22].Y = 370;

            Fields[23].X = 250;
            Fields[23].Y = 230;

        }

        private void StepGame(int index)
        {
            if (isMill)
            {
                RemovePiece(index);
                ChangeText();
                return;
            }

            try
            {
                if (model.GameState == 0)
                {
                    MalomField field = Fields[index];
                    model.Step(field.Number);

                    field.BgColor = model.Table[field.Number] == Values.Player1 ? "Red" : "Blue";
                    OnPropertyChanged(nameof(CurrentPlayer));
                }
                else
                {
                    if (!isMove)
                    {
                        moveIndex = index;
                        isMove = true;
                    }
                    else
                    {
                        isMove = false;
                        model.MovePiece(moveIndex, index);
                        RefreshTable();
                    }

                    if (!model.CanMove())
                    {
                        SkipTurn = true;
                    }
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                WrongStep?.Invoke(this, new ExceptionEventArgs(e.Message));
            }
             
        }

        private void RemovePiece(int index)
        {
            if (!isMill) return;

            try
            {
                if (!model.Mill(index))
                {
                    WrongStep?.Invoke(this, new ExceptionEventArgs("Wrong step!"));
                    return;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
               WrongStep?.Invoke(this, new ExceptionEventArgs(ex.Message));
            }

            Fields[index].BgColor = "Black";
            isMill = false;

        }

        private void RefreshTable()
        {
            foreach (MalomField field in Fields)
            {
                field.BgColor = model.Table[field.Number] == Values.Player1 ? 
                    "Red" : model.Table[field.Number] == Values.Empty ? "Black" : "Blue";
            }

            OnPropertyChanged(nameof(CurrentPlayer));
            
        }

        private void ChangeText()
        {
            if (model.GameState == 0)
            {
                Text = "Put a piece onto the table";

            }
            else
            {
                Text = "Move a piece";
            }
        }

        private void Model_GameAdvanced(object? sender, MalomEventArgs e)
        {
            OnPropertyChanged(nameof(CurrentPlayer));
            ChangeText();

        }

        private void OnMill(object? sender, MalomEventArgs e)
        {
            isMill = true;
            Text = "You have a mill!";
        }

        //private void Model_GameOver(object? sender, MalomEventArgs e)
        //{
        //    //foreach (MalomField field in Fields)
        //    //{
        //    //    field.IsLocked = true;
        //    //}
        //}

        private void OnNewGame()
        {
            isMill = false;
            isMove = false;
            moveIndex = 0;
            NewGame?.Invoke(this, EventArgs.Empty);
            SkipTurn = false;
            SaveGameEnabled = true;
            RefreshTable();
            foreach (var field in Fields)
            {
                field.IsEnabled = true; 
            }
        }

        private async void OnLoadGame()
        {
            isMill = false;
            isMove = false;
            moveIndex = 0;
            saveGameEnabled = true;

            LoadGame?.Invoke(this, EventArgs.Empty);
            await model.LoadGameAsync(Path);

            
            RefreshTable();
            if (!model.CanMove())
            {
                SkipTurn = true;
            }
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }
        private void OnSkipGame()
        {
            model.Skip();
            SkipTurn = false;
        }
        
    }
}
