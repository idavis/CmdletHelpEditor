#region Using Directives

using System;
using System.ComponentModel;

#endregion

namespace CmdletHelpEditor.DataModel
{
    public class ModuleObject : INotifyPropertyChanged
    {
        private String _Descrition;
        private String _ModuleType;
        private String _Name;
        private String _Version;

        public String Name
        {
            set
            {
                if ( _Name != value )
                {
                    _Name = value;
                    OnPropertyChanged( "Name" );
                }
            }
            get { return _Name; }
        }

        public String Version
        {
            set
            {
                if ( _Version != value )
                {
                    _Version = value;
                    OnPropertyChanged( "Version" );
                }
            }
            get { return _Version; }
        }

        public String Descrition
        {
            set
            {
                if ( _Descrition != value )
                {
                    _Descrition = value;
                    OnPropertyChanged( "Descrition" );
                }
            }
            get { return _Descrition; }
        }

        public String ModuleType
        {
            set
            {
                if ( _ModuleType != value )
                {
                    _ModuleType = value;
                    OnPropertyChanged( "ModuleType" );
                }
            }
            get { return _ModuleType; }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }
    }
}
