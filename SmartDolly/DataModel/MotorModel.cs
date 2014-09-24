using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDolly
{
    class MotorModel : INotifyPropertyChanged
    {
        private int         _stepsPerUnit;  // steps
        private int         _speed;         // mm/sec
        private TimeSpan    _ramp;          // sec
        private TimeSpan    _postTime;      // sec
        private int         _direction;     // TODO: Chech the type! LEFT/RIGHT or FORWARD/REVERSED??
        private bool _disableBetweenMoves;

        public int StepsPerUnit
        {
            get { return _stepsPerUnit; }
            set { 
                if(value != _stepsPerUnit)
                {
                    _stepsPerUnit = value;
                    NotifyPropertyChanged("StepsPerUnit");
                }
            }
        }

        public int Speed
        {
            get { return _speed; }
            set
            {
                if (value != _speed)
                {
                    _speed = value;
                    NotifyPropertyChanged("Speed");
                }
            }
        }

        public TimeSpan Ramp
        {
            get { return _ramp; }
            set
            {
                if (value != _ramp)
                {
                    _ramp = value;
                    NotifyPropertyChanged("Ramp");
                }
            }
        }

        public TimeSpan PostTime
        {
            get { return _postTime; }
            set
            {
                if (value != _postTime)
                {
                    _postTime = value;
                    NotifyPropertyChanged("PostTime");
                }
            }
        }

        public int Direction
        {
            get { return _direction; }
            set
            {
                if (value != _direction)
                {
                    _direction = value;
                    NotifyPropertyChanged("Direction");
                }
            }
        }

        public bool DisableBetweenMoves
        {
            get { return _disableBetweenMoves; }
            set
            {
                if (value != _disableBetweenMoves)
                {
                    _disableBetweenMoves = value;
                    NotifyPropertyChanged("DisableBetweenMoves");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
