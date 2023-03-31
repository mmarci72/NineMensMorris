using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.WPF.ViewModel
{
    internal class ExceptionEventArgs : EventArgs
    {
        private String message;

        public String Message
        {
            get => message;

        }
        public ExceptionEventArgs(String message)
        {
            this.message = message;
        }
    }
}
