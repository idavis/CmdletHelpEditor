#region Using Directives

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.DataModel;

#endregion

namespace CmdletHelpEditor
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Used to create the XML MAML output file.
        public static WriteXMLHelpFileHelperClass helpWriter = new WriteXMLHelpFileHelperClass();

        // This is PsSnapin node in the tree view. 
        public static TreeViewItem PsSnapinNode = new TreeViewItem();

        // This denotes that the tree view contains at least one obsolete info
        // we use this prperty to warn the user that saving the help file will cause him 
        // to lose the obsolete help content.
        public static Boolean ObsoleteInfo = false;


        // This property holds the PsSnapin name which is loaded 
        public static String PsSnapinName = "";

        // This property holds the name of te assembly of the PsSnapin
        // This is needed to create the default help file name.
        public static String PsSnapinModuleName = "";

        // this is a flag to denote that we opened an existing help file.
        public static Boolean OldHelpFileExist = false;

        // Path where the help file was loaded from
        public static String HelpFilePath = "";

        // This is the sped project name. We use this across the app to
        // determine whether we are working offline or online.
        public static String ProjectName = "";

        // Clipboard object used in the copy and paste operations.
        public static ClipBoardObject ClipBoardRecord = new ClipBoardObject();


        //Define zthe global CmdletDesciption record which will contain all the Cmdlets.
        // this record contains all the data from Spec, help and code.
        public static Collection<cmdletDescription> CmdletsHelps = new Collection<cmdletDescription>();

        // This holds the PsObject results from the get-command.
        public static Collection<PSObject> results = new Collection<PSObject>();

        public static ModuleObject selectedModule = new ModuleObject();

        public static NavigationControl NavControl = new NavigationControl();
        public static MainGrid MainGrid1 = new MainGrid();
        public static HeaderControl HeaderControl1 = new HeaderControl(); //height 300
        public static EmptyParameterControl EmptyParameterControl1 = new EmptyParameterControl(); //Not sure it is used.
        public static DescriptionControl DescriptionControl1 = new DescriptionControl(); //Margin="282.24,133,8,8"
        public static ParametersControl ParametersControl1 = new ParametersControl(); //Margin="282.24,137,8,8"
        public static ExamplesControl ExamplesControl1 = new ExamplesControl(); //Margin="293,148,8,8"
        public static RelatedLinks RelatedLinks1 = new RelatedLinks(); //"282.24,137,8,8"


        public MainWindow()
        {
            InitializeComponent();

            HeaderPannel.Children.Add( HeaderControl1 );
            EditControls.Children.Add( MainGrid1 );
            AddNavigationContol( NavControl );
            AddDescriptionControl( DescriptionControl1 );
            EmptyParameterControl1.Visibility = Visibility.Hidden;
            AddParametersControl( ParametersControl1 );
            AddExamplesControl( ExamplesControl1 );
            AddExamplesControl( RelatedLinks1 );
            AddEmptyParameterControl( EmptyParameterControl1 );
        }

        private void AddNavigationContol( NavigationControl navcontrol )
        {
            var NavMargin = new Thickness(); //"0,0,0,0";
            navcontrol.Width = 278.24;
            navcontrol.Height = 900;
            navcontrol.HorizontalAlignment = HorizontalAlignment.Left;
            NavMargin.Left = 0;
            NavMargin.Top = 0;
            NavMargin.Right = 0;
            NavMargin.Bottom = 0;
            NavControl.Margin = NavMargin;
            NavControl.Visibility = Visibility.Hidden;
            NavControl.Visibility = Visibility.Collapsed;
            EditControls.Children.Add( NavControl );
        }

        private void AddDescriptionControl( DescriptionControl descriptionControl )
        {
            //Margin="282.24,133,8,8"
            var Margin = new Thickness();
            Margin.Top = 30;
            Margin.Left = 10;
            Margin.Right = 8;
            Margin.Bottom = 8;
            descriptionControl.HorizontalAlignment = HorizontalAlignment.Left;
            descriptionControl.Margin = Margin;
            descriptionControl.Visibility = Visibility.Collapsed;
            descriptionControl.Width = 900;
            descriptionControl.Height = 900;
            //DescriptionControl.Visibility = Visibility.Collapsed;
            EditControls.Children.Add( descriptionControl );
        }

        private void AddParametersControl( ParametersControl paramControl )
        {
            //Margin="282.24,133,8,8"
            var Margin = new Thickness();
            Margin.Top = 30;
            Margin.Left = 10;
            Margin.Right = 8;
            Margin.Bottom = 8;
            paramControl.HorizontalAlignment = HorizontalAlignment.Left;
            paramControl.Margin = Margin;
            paramControl.Visibility = Visibility.Collapsed;
            paramControl.Width = 900;
            paramControl.Height = 900;
            //DescriptionControl.Visibility = Visibility.Collapsed;
            EditControls.Children.Add( paramControl );
        }

        private void AddExamplesControl( ExamplesControl examplesControl )
        {
            //Margin="282.24,133,8,8"
            var Margin = new Thickness();
            Margin.Top = 30;
            Margin.Left = 10;
            Margin.Right = 8;
            Margin.Bottom = 8;
            examplesControl.HorizontalAlignment = HorizontalAlignment.Left;
            examplesControl.Margin = Margin;
            examplesControl.Visibility = Visibility.Collapsed;
            examplesControl.Width = 900;
            examplesControl.Height = 900;
            EditControls.Children.Add( examplesControl );
        }

        private void AddExamplesControl( RelatedLinks relatedLinks )
        {
            //Margin="282.24,133,8,8"
            var Margin = new Thickness();
            Margin.Top = 30;
            Margin.Left = 10;
            Margin.Right = 8;
            Margin.Bottom = 8;
            relatedLinks.HorizontalAlignment = HorizontalAlignment.Left;
            relatedLinks.Margin = Margin;
            relatedLinks.Visibility = Visibility.Collapsed;
            relatedLinks.Width = 900;
            relatedLinks.Height = 900;
            EditControls.Children.Add( relatedLinks );
        }

        private void AddEmptyParameterControl( EmptyParameterControl emptyParameterControl )
        {
            //Margin="282.24,133,8,8"
            var Margin = new Thickness();
            Margin.Top = 30;
            Margin.Left = 10;
            Margin.Right = 8;
            Margin.Bottom = 8;
            emptyParameterControl.HorizontalAlignment = HorizontalAlignment.Left;
            emptyParameterControl.Margin = Margin;
            emptyParameterControl.Visibility = Visibility.Collapsed;
            emptyParameterControl.Width = 900;
            emptyParameterControl.Height = 900;
            EditControls.Children.Add( emptyParameterControl );
        }

        /// <summary>
        ///   Resets the examples page.
        /// </summary>
        public static void ResetExamplesPage()
        {
            ExamplesControl1.ExampleCommandTextBox.Text = "";
            ExamplesControl1.ExampleDescriptionTextBox.Text = "";
            ExamplesControl1.ExampleNameTextBox.Text = "";
            ExamplesControl1.ExampleOutputTextBox.Text = "";
            ExamplesControl1.ExampleID.Text = "";
            ExamplesControl1.OldExampleDescTextBox.Text = "";
        }

        /// <summary>
        ///   Resets the related Link page.
        /// </summary>
        public static void ResetLinksPage()
        {
            RelatedLinks1.RelatedLinkTextBox.Text = "";
            RelatedLinks1.LinkIDTextBox.Text = "";
            RelatedLinks1.OldRelatedLinkTextBox.Text = "";
        }


        /// This routine resets the description page.
        /// </summary>
        public static void resetDescriptionPage()
        {
            DescriptionControl1.ShortDescriptionTextBox.Text = "";
            DescriptionControl1.DetailedDescriptionTextBox.Text = "";
            DescriptionControl1.InputTypeDescTextBox.Text = "";
            DescriptionControl1.InputTypeTextBox.Text = "";
            DescriptionControl1.OutputTypeDescTextBox.Text = "";
            DescriptionControl1.OutpuTypeTextBox.Text = "";
            DescriptionControl1.NotesDescriptionTextBox.Text = "";
            DescriptionControl1.OldShortDescTextBox.Text = "";
            DescriptionControl1.OldDetailedDescriptionTextBox.Text = "";
            DescriptionControl1.OldNotesTextBox.Text = "";
            DescriptionControl1.OldOutputTypeDescTextBox.Text = "";
            DescriptionControl1.OldInputTypeTextBox.Text = "";
            DescriptionControl1.OldInputTypeDescTextBox.Text = "";
            DescriptionControl1.OldTypeTextBox.Text = "";
        }


        /// <summary>
        ///   This routine resets the parameter page.
        /// </summary>
        public static void resetParameterPage()
        {
            ParametersControl1.ParameterDescTextBox.Text = "";
            ParametersControl1.ParameterNameTextBox.Text = "";
            ParametersControl1.ValueFromRemainingCheckBox.IsChecked = false;
            ParametersControl1.VFPBPN_CheckBox.IsChecked = false;
            ParametersControl1.VFP_CheckBox.IsChecked = false;
            ParametersControl1.MandatoryCheckBox.IsChecked = false;
            ParametersControl1.DynamicCheckBox.IsChecked = false;
            ParametersControl1.PositionalCheckBox.IsChecked = false;
            ParametersControl1.PositionTextBox.Text = "";
            ParametersControl1.AliasesList.Items.Clear();
            ParametersControl1.AttributesList.Items.Clear();
            ParametersControl1.DefaultValueTextBox.Text = "";
            ParametersControl1.GlobbingCheckBox.IsChecked = false;
            ParametersControl1.OldParamDefaultValueTextBox.Text = "";
            ParametersControl1.OldParameterDescTextBox.Text = "";
            ParametersControl1.OldGlobbingCheckBox.IsChecked = false;
            ParametersControl1.ParameterTypeTextBox.Text = "";
        }

        public static void Exit()
        {
            Application current = Application.Current;
            current.Shutdown();
        }
    }
}
