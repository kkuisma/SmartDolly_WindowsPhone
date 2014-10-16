using BluetoothConnectionManager;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DollyProtocol;

namespace SmartDolly
{
    public class Dolly : ViewModelBase
    {
        public static class Property
        {
            public const char State             = 'A';
            public const char Ping              = 'B';
            public const char Interval          = 'C';
            public const char TotalTime         = 'D';
            public const char ElapsedTime       = 'E';
            public const char TotalShots        = 'F';
            public const char ShotsTaken        = 'G';
            public const char MaxDistance       = 'H';
            public const char TotalDistance     = 'I';
            public const char CurrentPosition   = 'J';
            public const char MotionPerCycle    = 'K';
            public const char Limit1            = 'L';
            public const char Limit2            = 'M';
            public const char Measuring         = 'N';
            public const char Homing            = 'O';
            public const char BatVoltage        = 'P';
        }

        public Dolly()
        {
        }

        private int _interval;
        private TimeSpan _totalTime;
        private TimeSpan _elapsedTime;
        private int _totalShots;
        private int _shotsTaken;
        private int _maxDistance;
        private int _totalDistance;
        private int _currentPosition;
        private int _motionPerCycle;
        private bool _state;
        private bool _limit1;
        private bool _limit2;
        private bool _measuring;
        private bool _homing;
        private double _batVoltage;

        public override char objectId() { return TargetObject.Dolly; }

        public int Interval
        {
            get { return _interval; }
            set
            {
                if (value < 10) value = 10;
                SetProperty(ref _interval, value); SetPropertyRemote(Property.Interval, _interval);
                if (IntervalLock)
                {
                    if (!TotalShotsLock)
                    {
                        TotalShots = Convert.ToInt32(TotalTime.TotalMilliseconds) / Interval;
                    }
                    if (!MotionPerCycleLock)
                    {
                        MotionPerCycle = MaxDistance / TotalShots;
                    }
                    if (!TotalTimeLock)
                    {
                        TotalTime = TimeSpan.FromMilliseconds(TotalShots * Interval);
                    }
                }
            }
        }

        public bool State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); SetPropertyRemote(Property.State, Convert.ToInt32(_state));
                if (_state)
                {
                    App.Speak("Dolly started.");
                }
                else 
                {
                    App.Speak("Dolly stopped.");
                }
            }
        }

        public bool Limit1
        {
            get { return _limit1; }
            set { SetProperty(ref _limit1, value); }
        }

        public bool Limit2
        {
            get { return _limit2; }
            set { SetProperty(ref _limit2, value); }
        }

        public bool Measuring
        {
            get { return _measuring; }
            set { SetProperty(ref _measuring, value); SetPropertyRemote(Property.Measuring, Convert.ToInt32(_measuring)); }
        }

        public bool Homing
        {
            get { return _homing; }
            set { SetProperty(ref _homing, value); SetPropertyRemote(Property.Homing, Convert.ToInt32(_homing)); }
        }

        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set { SetProperty(ref _totalTime, value); SetPropertyRemote(Property.TotalTime, Convert.ToInt32(_totalTime.TotalSeconds)); }
        }

        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
            set { SetProperty(ref _elapsedTime, value); }
        }

        public int TotalShots
        {
            get { return _totalShots; }
            set
            {
                if (value < 1) value = 1;
                SetProperty(ref _totalShots, value); SetPropertyRemote(Property.TotalShots, _totalShots);
                if (TotalShotsLock)
                {
                    if (!IntervalLock)
                    {
                        Interval = Convert.ToInt32(TotalTime.TotalMilliseconds) / TotalShots;
                    }
                    if (!TotalTimeLock)
                    {
                        TotalTime = TimeSpan.FromMilliseconds(TotalShots * Interval);
                    }
                    if(!MotionPerCycleLock)
                    {
                        MotionPerCycle = MaxDistance / TotalShots;
                    }
                }
            }
        }

        public int ShotsTaken
        {
            get { return _shotsTaken; }
            set { SetProperty(ref _shotsTaken, value); }
        }

        public int MaxDistance
        {
            get { return _maxDistance; }
            set { SetProperty(ref _maxDistance, value); SetPropertyRemote(Property.MaxDistance, _maxDistance); }
        }

        public int TotalDistance
        {
            get { return _totalDistance; }
            set { SetProperty(ref _totalDistance, value); SetPropertyRemote(Property.TotalDistance, _totalDistance); }
        }

        public int CurrentPosition
        {
            get { return _currentPosition; }
            set { SetProperty(ref _currentPosition, value); }
        }

        public int MotionPerCycle
        {
            get { return _motionPerCycle; }
            set
            {
                if (value < 1) value = 1;
                SetProperty(ref _motionPerCycle, value); SetPropertyRemote(Property.MotionPerCycle, _motionPerCycle);
                if (MotionPerCycleLock)
                {
                    if (!TotalShotsLock)
                    {
                        TotalShots = MaxDistance / MotionPerCycle;
                    }
                    if (!IntervalLock)
                    {
                        Interval = Convert.ToInt32(TotalTime.TotalMilliseconds) / TotalShots;
                    }
                    if (!TotalTimeLock)
                    {
                        TotalTime = TimeSpan.FromMilliseconds(TotalShots * Interval);
                    }
                }
            }
        }

        public double BatVoltage
        {
            get { return _batVoltage; }
            set { SetProperty(ref _batVoltage, value); }
        }

        public bool IntervalLock { get; set; }
        public bool TotalTimeLock { get; set; }
        public bool TotalShotsLock { get; set; }
        public bool MotionPerCycleLock { get; set; }
        public bool MaxDistanceLock { get; set; }

        public void getInitValues()
        {
            GetPropertyRemote(Property.Interval);
            GetPropertyRemote(Property.TotalTime);
            GetPropertyRemote(Property.ElapsedTime);
            GetPropertyRemote(Property.TotalShots);
            GetPropertyRemote(Property.ShotsTaken);
            GetPropertyRemote(Property.MaxDistance);
            GetPropertyRemote(Property.TotalDistance);
            GetPropertyRemote(Property.CurrentPosition);
            GetPropertyRemote(Property.MotionPerCycle);
            GetPropertyRemote(Property.Limit1);
            GetPropertyRemote(Property.Limit2);
            GetPropertyRemote(Property.Measuring);
            GetPropertyRemote(Property.Homing);
            GetPropertyRemote(Property.BatVoltage);
        }

        public event EventHandler<EventArgs> PingReceivedEventHandler;

        public void handleMessage(string msg)
        {
            string[] messageArray = msg.Split(Protocol.ValueSeparator);

            char property = msg[0];
            char command = msg[1];

            if (property == Property.Ping)
            {
                if (PingReceivedEventHandler != null) {
                    PingReceivedEventHandler(this, null);
                }
                return;
            }

            if (command == Command.GetResponse || command == Command.Set)
            {
                newValueFromDevice = true;
                Int32 newValue = Convert.ToInt32(messageArray[1]);

                switch(property)
                {
                case Property.State:
                    State = (newValue == 0) ? false : true;
                    break;
                case Property.Interval:
                    Interval = newValue;
                    break;
                case Property.TotalShots:
                    TotalShots = newValue;
                    break;
                case Property.TotalTime:
                    TotalTime = TimeSpan.FromSeconds(newValue);
                    break;
                case Property.ElapsedTime:
                    ElapsedTime = TimeSpan.FromSeconds(newValue);
                    break;
                case Property.MaxDistance:
                    MaxDistance = newValue;
                    break;
                case Property.TotalDistance:
                    TotalDistance = newValue;
                    break;
                case Property.MotionPerCycle:
                    MotionPerCycle = newValue;
                    break;
                case Property.ShotsTaken:
                    ShotsTaken = newValue;
                    break;
                case Property.CurrentPosition:
                    CurrentPosition = newValue;
                    break;
                case Property.BatVoltage:
                    BatVoltage = Convert.ToDouble(newValue)/1000.0;
                    break;
                case Property.Limit1:
                    Limit1 = (newValue == 0) ? false : true;
                    break;
                case Property.Limit2:
                    Limit2= (newValue == 0) ? false : true;
                    break;
                case Property.Measuring:
                    Measuring = (newValue == 0) ? false : true;
                    Debug.WriteLine("Should NEVER be here !!!");
                    break;
                case Property.Homing:
                    Homing = (newValue == 0) ? false : true;
                    Debug.WriteLine("Should NEVER be here !!!");
                    break;

                }
                newValueFromDevice = false;
            }
        }
    }
}
