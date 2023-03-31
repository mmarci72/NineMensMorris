using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malom.Persistence;

namespace Malom.Model
{
    public class MalomEventArgs : EventArgs
    {
        private Values _currentPlayer;

        private Values _winner;

        //0 means that players have to place pieces, 1 means that they have to move them
        private int _gameState = 0;

        public Values CurrentPlayer => _currentPlayer;

        //Empty if it was a draw
        public Values Winner => _winner;
      
        public int GameState => _gameState;
        
        public MalomEventArgs(Values currentPlayer, Values winner, int gameState) {
            _currentPlayer = currentPlayer;
            _winner = winner;
            _gameState = gameState;
        }
    }
}
