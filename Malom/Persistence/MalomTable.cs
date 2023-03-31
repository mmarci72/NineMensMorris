using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Persistence;

public enum Values
{
    Player1,
    Player2,
    Empty
}

public class MalomTable
{
    private readonly int _totalNumberOfPieces;
    private int _currentNumberOfPieces;
    private int _player1NumberOfPieces;
    private int _player2NumberOfPieces;
    private int _gameStepCount;
    private Values _currentPlayer;

    private readonly Values[] _fieldOuterValues = new Values[8];
    private readonly Values[] _fieldMiddleValues = new Values[8];
    private readonly Values[] _fieldInnerValues = new Values[8];

    public Values CurrentPlayer
    {
        get => _currentPlayer;
        set => _currentPlayer = value;
    }


    public bool IsFilled
    {
        get
        {
            return !(_fieldOuterValues.Any(x => x == Values.Empty) ||
                     _fieldMiddleValues.Any(x => x == Values.Empty) ||
                     _fieldMiddleValues.Any(x => x == Values.Empty));
        }
    }

    public Values[] FieldOuterValues => _fieldOuterValues;

    public Values[] FieldMiddleValues => _fieldMiddleValues;

    public Values[] FieldInnerValues => _fieldInnerValues;

    public int CurrentNumberOfPieces
    {
        get { return _currentNumberOfPieces; }
        set { _currentNumberOfPieces = value; }
    }

    public int Player1NumberOfPieces
    {
        get { return _player1NumberOfPieces; }
        set { _player1NumberOfPieces = value; }
    }

    public int Player2NumberOfPieces
    {
        get { return _player2NumberOfPieces; }
        set { _player2NumberOfPieces = value; }
    }

    public int GameStepCount
    {
        get { return _gameStepCount; }
        set { _gameStepCount = value; }
    }

    public int TotalNumberOfPieces => _totalNumberOfPieces;
    public Values this[int x] => GetValue(x);

    public MalomTable() : this(18, Values.Player1)
    {
    }

    public MalomTable(int totalNumberOfPieces, Values startingPlayer)
    {
        if (totalNumberOfPieces < 0 || totalNumberOfPieces > 24 || totalNumberOfPieces % 2 != 0)
            throw new ArgumentOutOfRangeException(nameof(totalNumberOfPieces), "The total number of pieces is invalid");

        if (startingPlayer == Values.Empty)
            throw new ArgumentOutOfRangeException(nameof(startingPlayer), "Empty can't be a starting player");
        _totalNumberOfPieces = totalNumberOfPieces;
        _currentNumberOfPieces = TotalNumberOfPieces;
        _player2NumberOfPieces = _player1NumberOfPieces = _currentNumberOfPieces / 2;
        _gameStepCount = 0;
        _currentPlayer = startingPlayer;
        Array.Fill(_fieldOuterValues, Values.Empty);
        Array.Fill(_fieldInnerValues, Values.Empty);
        Array.Fill(_fieldMiddleValues, Values.Empty);
    }

    public Values GetValue(int x)
    {
        if (x < 0 || x >= 24)
            throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");

        if (x <= 7) return _fieldOuterValues[x];
        if (x <= 15) return _fieldMiddleValues[x - 8];

        return _fieldInnerValues[x - 16];
    }

    public void SetValue(int x, Values value)
    {
        if (x < 0 || x >= 24)
            throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
        if (x <= 7)
        {
            if (_fieldOuterValues[x] != Values.Empty) return;
            _fieldOuterValues[x] = value;
        }
        else if (x <= 15)
        {
            x -= 8;
            if (_fieldMiddleValues[x] != Values.Empty) return;
            _fieldMiddleValues[x] = value;
        }
        else
        {
            x -= 16;
            if (_fieldInnerValues[x] != Values.Empty) return;
            _fieldInnerValues[x] = value;
        }
    }

    public void SetEmpty(int x)
    {
        if (x < 0 || x >= 24)
            throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");

        if (x < 0 || x >= 24)
            throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");

        if (x <= 7) _fieldOuterValues[x] = Values.Empty;
        else if (x <= 15) _fieldMiddleValues[x - 8] = Values.Empty;
        else _fieldInnerValues[x - 16] = Values.Empty;
    }

    public void StepValue(int x)
    {
        if (GetValue(x) != Values.Empty)
            throw new ArgumentOutOfRangeException(nameof(x),
                "The field you're trying to step onto already has a piece");

        if (_currentPlayer == Values.Player1)
        {
            SetValue(x, Values.Player1);
            _currentPlayer = Values.Player2;
        }

        else if (_currentPlayer == Values.Player2)
        {
            SetValue(x, Values.Player2);
            _currentPlayer = Values.Player1;
        }
    }

    public void ChangePlayer()
    {
        if (_currentPlayer == Values.Player1)
        {
            _currentPlayer = Values.Player2;
        }

        else if (_currentPlayer == Values.Player2)
        {
            _currentPlayer = Values.Player1;
        }
    }
}