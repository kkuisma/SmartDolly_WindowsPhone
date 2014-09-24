using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDolly
{
    class CameraModel : INotifyPropertyChanged
    {
        private int _exposureTime;      // milliseconds
        private int _postExposureTime;  // milliseconds
        private int _focusTime;         // milliseconds
        private int _exposureMode;      // BULB or CAMERA
        private int _focalLength;       // mm

        public int ExposureTime
        {
            get { return _exposureTime; }
            set { 
                if(value != _exposureTime)
                {
                    _exposureTime = value;
                    NotifyPropertyChanged("ExposureTime");
                }
            }
        }

        public int PostExposureTime
        {
            get { return _postExposureTime; }
            set
            {
                if (value != _postExposureTime)
                {
                    _postExposureTime = value;
                    NotifyPropertyChanged("PostExposureTime");
                }
            }
        }
        public int FocusTime
        {
            get { return _focusTime; }
            set
            {
                if (value != _focusTime)
                {
                    _focusTime = value;
                    NotifyPropertyChanged("FocusTime");
                }
            }
        }

        public int ExposureMode
        {
            get { return _exposureMode; }
            set
            {
                if (value != _exposureMode)
                {
                    _exposureMode = value;
                    NotifyPropertyChanged("ExposureMode");
                }
            }
        }

        public int FocalLength
        {
            get { return _focalLength; }
            set
            {
                if (value != _focalLength)
                {
                    _focalLength = value;
                    NotifyPropertyChanged("FocalLength");
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
