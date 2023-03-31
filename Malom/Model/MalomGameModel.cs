using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Malom.Persistence;

namespace Malom.Model;

public class MalomGameModel
{
    private IMalomDataAccess _dataAccess;
    private MalomTable _table;
    private int _gameState = 0;




    public MalomTable Table => _table;


    public event EventHandler<MalomEventArgs>? GameAdvanced;

    public event EventHandler<MalomEventArgs>? GameOver;

    public event EventHandler<MalomEventArgs> OnMill;

    public event EventHandler<MoveEventArgs>? Move;



    public MalomGameModel(IMalomDataAccess dataAccess)
    {
        _table = new MalomTable();
        _dataAccess = dataAccess;
    }

    public int GameState => _gameState;
    public void NewGame()
    {
        _table = new MalomTable();
        Table.CurrentNumberOfPieces = Table.TotalNumberOfPieces;
        Table.Player2NumberOfPieces = Table.Player1NumberOfPieces = Table.CurrentNumberOfPieces / 2;
        _gameState = 0;
    }

    public bool IsGameOver()
    {
        return _table.IsFilled || Table.Player1NumberOfPieces < 3 || Table.Player2NumberOfPieces < 3;
    }

    public bool CanMove()
    {
        int[] adjacentTiles;
        for (int i = 0; i < Table.FieldInnerValues.Length; i++)
        {
            
            adjacentTiles = GetAdjacentTiles(i+16);
            if (Table.FieldInnerValues[i] == Table.CurrentPlayer)
            {
                foreach (var adjTile in adjacentTiles)
                {
                    if (Table.GetValue(adjTile) != Values.Player1 && Table.GetValue(adjTile) != Values.Player2)
                    {
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i < Table.FieldMiddleValues.Length; i++)
        {
            
            adjacentTiles = GetAdjacentTiles(i+8);
            if (Table.FieldMiddleValues[i] == Table.CurrentPlayer)
            {
                foreach (var adjTile in adjacentTiles)
                {
                    if (Table.GetValue(adjTile) != Values.Player1 && Table.GetValue(adjTile) != Values.Player2)
                    {
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i < Table.FieldOuterValues.Length; i++)
        {
            
            adjacentTiles = GetAdjacentTiles(i);
            if (Table.FieldOuterValues[i] == Table.CurrentPlayer)
            {
                foreach (var adjTile in adjacentTiles)
                {
                    if (Table.GetValue(adjTile) != Values.Player1 && Table.GetValue(adjTile) != Values.Player2)
                    {
                        return true;
                    }
                }
            }
        }
        

        return false;
    }
    
    public async Task LoadGameAsync(String path)
    {
        if (_dataAccess == null)
        {
            throw new InvalidOperationException("No data access is provided.");
        }

        _table = await _dataAccess.LoadAsync(path);
        if (Table.CurrentNumberOfPieces == 0) _gameState = 1;
        OnGameAdvanced();

    }

    public async Task SaveGameAsync(String path)
    {
        if (_dataAccess == null)
            throw new InvalidOperationException("No data access is provided.");

        await _dataAccess.SaveAsync(path, _table);
    }

    public void Skip()
    {
        Table.ChangePlayer();
        OnGameAdvanced();
    }
    public void Step(int x)
    {
        if (IsGameOver()) return;
        if (_gameState == 0)
        {
            _table.StepValue(x);
            Table.GameStepCount++;
            Table.CurrentNumberOfPieces--;
            if (Table.CurrentNumberOfPieces == 0) _gameState = 1;
        }


        if (CheckMill(x)) OnMill?.Invoke(this, new MalomEventArgs(_table.CurrentPlayer, Values.Empty, _gameState));

        else OnGameAdvanced();



    }

    public bool CheckMill(int x)
    {
        var isMill = false;
        

            if (_table.GetValue(x) == Values.Empty) return false;
            if (x % 2 == 0)
            {
                if (x == 0 || x == 16 || x == 8)
                    isMill = _table.GetValue(x) == _table.GetValue(x + 6) &&
                             _table.GetValue(x) == _table.GetValue(x + 7);
                else
                    isMill = _table.GetValue(x) == _table.GetValue(x - 1) &&
                             _table.GetValue(x) == _table.GetValue(x - 2);
                if ((x == 22 || x == 6 || x == 14) && !isMill)
                    isMill = _table.GetValue(x) == _table.GetValue(x + 1) &&
                             _table.GetValue(x) == _table.GetValue(x - 6);
                else if (!isMill)
                    isMill = _table.GetValue(x) == _table.GetValue(x + 1) &&
                             _table.GetValue(x) == _table.GetValue(x + 2);
                return isMill;
            }

            if (x < 8)
            {
                isMill = _table.GetValue(x) == _table.GetValue(x + 8) && _table.GetValue(x) == _table.GetValue(x + 16);
            }
            else if (x < 16)
            {
                isMill = _table.GetValue(x) == _table.GetValue(x + 8) && _table.GetValue(x) == _table.GetValue(x - 8);
            }
            else if (_table.GetValue(x) == _table.GetValue(x - 8) &&
                     _table.GetValue(x) == _table.GetValue(x - 16))
            {
                return true;
            }

            if ((x == 7 || x == 23 || x == 15) && !isMill)
                return _table.GetValue(x) == _table.GetValue(x - 7) && _table.GetValue(x) == _table.GetValue(x - 1);

            return (_table.GetValue(x) == _table.GetValue(x + 1) &&
                    _table.GetValue(x) == _table.GetValue(x - 1)) || isMill;
        

    }

    public int[] GetAdjacentTiles(int x)
    {
        if (x % 2 == 0)
        {
            int[] adjacentTilesEven = new int[2];
            if (x == 0 || x == 16 || x == 8)
                adjacentTilesEven[0] = x + 7;
            else
                adjacentTilesEven[0] = x - 1;

            adjacentTilesEven[1] = x + 1;
            return adjacentTilesEven;
        }

        if (x < 16 && x >= 8)
        {
            int[] adjacentTilesMiddle = new int[4];
            adjacentTilesMiddle[0] = x + 8;
            adjacentTilesMiddle[1] = x - 8;
            if (x == 15)
                adjacentTilesMiddle[2] = x - 7;
            else
                adjacentTilesMiddle[2] = x + 1;
            adjacentTilesMiddle[3] = x - 1;
            return adjacentTilesMiddle;

        }
        int[] adjacentTiles = new int[3];
        if (x < 8)
            adjacentTiles[0] = x + 8;
        else
            adjacentTiles[0] = x - 8;


        if (x == 7 || x == 23)
            adjacentTiles[1] = x - 7;
        else
            adjacentTiles[1] = x + 1;
        adjacentTiles[2] = x - 1;
        return adjacentTiles;
    }

    public bool Mill(int x)
    {
        if (_table.GetValue(x) != _table.CurrentPlayer || CheckMill(x))
        {
            //_table.CurrentPlayer = _table.CurrentPlayer == Values.Player1 ? Values.Player2 : Values.Player1;
            return false;
        }

        if (_table.CurrentPlayer == Values.Player1)
        {
            Table.Player1NumberOfPieces--;
        }
        else Table.Player2NumberOfPieces--;

        _table.SetEmpty(x);
        if (IsGameOver())
        {
            OnGameOver(_table.CurrentPlayer == Values.Player1 ? Values.Player2 : Values.Player1);
        }
        else OnGameAdvanced();
        return true;
    }

    public void MovePiece(int x, int y)
    {
        if(Table.GetValue(x) != Table.CurrentPlayer)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "The piece you clicked on is not yours");
        }
        if(Table.GetValue(y) != Values.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(y), "The move is not valid");
        }

        int[] adjacentTiles = GetAdjacentTiles(x);
        if (!adjacentTiles.Any(x => x == y))
        {
            throw new ArgumentOutOfRangeException(nameof(y), "The tile you clicked is not adjacent");
        }

        Table.SetEmpty(x);
        Table.SetValue(y, (Table.CurrentPlayer == Values.Player1 ? Values.Player1 : Values.Player2));
        Move?.Invoke(this, new MoveEventArgs(Table.CurrentPlayer, x, y));
        Table.ChangePlayer();
        if(CheckMill(y)) OnMill?.Invoke(this, new MalomEventArgs(_table.CurrentPlayer, Values.Empty, _gameState));
        else
        {
            OnGameAdvanced();
        }
    }

    private void OnGameAdvanced()
    {
        GameAdvanced?.Invoke(this, new MalomEventArgs(_table.CurrentPlayer, Values.Empty, _gameState));
    }

    private void OnGameOver(Values winner)
    {
        MalomEventArgs e;
        if (winner == Values.Player1)
            e = new MalomEventArgs(_table.CurrentPlayer, Values.Player1, _gameState);
        else if (winner == Values.Player2)
            e = new MalomEventArgs(_table.CurrentPlayer, Values.Player2, _gameState);
        else
            e = new MalomEventArgs(_table.CurrentPlayer, Values.Empty, _gameState);
        GameOver?.Invoke(this, e);
    }
}