#region Using Directives

using System.Windows;

#endregion

namespace CmdletHelpEditor
{
    /// <summary>
    ///   Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_OK_Button_Click( object sender, RoutedEventArgs e )
        {
            Close();
        }
    }
}
