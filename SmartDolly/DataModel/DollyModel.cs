using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDolly
{
    class DollyModel : INotifyPropertyChanged
    {
        private TimeSpan    _interval;
        private TimeSpan    _totalTime;
        private TimeSpan    _elapsedTime;
        private Int32       _totalShots;
        private Int32       _shotsTaken;
        private int         _maxDistance;
        private int         _motionDistance;
        private int         _currentPosition;
        private int         _motionPerCycle;
        
        public TimeSpan Interval
        {
            get { return _interval; }
            set { 
                if(value != this._interval)
                {
                    _interval = value;
                    NotifyPropertyChanged("Interval");
                }
            }
        }

        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set
            {
                if (value != _totalTime)
                {
                    _totalTime = value;
                    NotifyPropertyChanged("TotalTime");
                }
            }
        }

        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
            set { _elapsedTime = value; }
        }

        public Int32 TotalShots
        {
            get { return _totalShots; }
            set {
                if (value != this._totalShots)
                {
                    _totalShots = value;
                    NotifyPropertyChanged("TotalShots");
                }
            }
        }

        public Int32 ShotsTaken
        {
            get { return _shotsTaken; }
            set { 
                if(value != _shotsTaken)
                {
                    _shotsTaken = value;
                    NotifyPropertyChanged("ShotsTaken");
                }
            }
        }

        public int MaxDistance
        {
            get { return _maxDistance; }
            set { 
                if(value != _maxDistance)
                {
                    _maxDistance = value;
                    NotifyPropertyChanged("MaxDistance");
                }
            }
        }

        public int MotionDistance
        {
            get { return _motionDistance; }
            set {
                if(value != _motionDistance)
                {
                    _motionDistance = value;
                    NotifyPropertyChanged("MotionDistance");
                }
            }
        }

        public int CurrentPosition
        {
            get { return _currentPosition; }
            set { 
                if(value != _currentPosition)
                {
                    _currentPosition = value;
                    NotifyPropertyChanged("CurrentPosition");
                }
            }
        }

        public int MotionPerCycle
        {
            get { return _motionPerCycle; }
            set { 
                if(value != _motionPerCycle)
                {
                    _motionPerCycle = value;
                    NotifyPropertyChanged("MotionPerCycle");
                }
            }
        }        
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
