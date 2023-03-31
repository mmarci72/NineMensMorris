using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Malom.Persistence;

namespace Malom.WPF.ViewModel
{
    public class MalomField : ViewModelBase
    {
        private String _bgColor = "Black";

        private double loc = 0;

        public double Loc
        {
            get { return loc; }
            set
            {
                loc = value;
                OnPropertyChanged();
            }
        }
        public String BgColor 
        {
            get { return _bgColor; }
            set
            {
                if (_bgColor != value)
                {
                    _bgColor = value; 
                    OnPropertyChanged();
                }
            } 
        }

        private bool isEnabled = true;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value; 
                OnPropertyChanged();
            }
        }



        public double X { get; set; }
        public double Y { get; set; }

        public int Number { get; set; }

        public DelegateCommand? StepCommand { get; set; }
    }
}
