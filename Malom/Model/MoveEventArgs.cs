using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malom.Persistence;

namespace Malom.Model
{
    public class MoveEventArgs : EventArgs
    {
        private Values _currentPlayer;

        private int _fromMoveIndex;

        private int _toMoveIndex;
        public Values CurrentPlayer => _currentPlayer;

        public int FromMoveIndex => _fromMoveIndex;

        public int ToMoveIndex => _toMoveIndex;      

        public MoveEventArgs(Values currentPlayer, int fromMoveIndex, int toMoveIndex) {
            _currentPlayer = currentPlayer;
            _toMoveIndex = toMoveIndex;
            _fromMoveIndex = fromMoveIndex;
        }
    }
}
