using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Windows.Forms;


namespace HelpEditorOS
{
    /// <summary>
    /// Interaction logic for HeaderControl.xaml
    /// </summary>
    public partial class HeaderControl : System.Windows.Controls.UserControl
    {
            //// Used to create the XML MAML output file.
            //WriteXMLHelpFileHelperClass helpWriter = new WriteXMLHelpFileHelperClass();

        public HeaderControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This is the back button navigation routine.
        /// handles the Re-Load PsSnapin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ForwardNavigationButton_Click(object sender, RoutedEventArgs e)
        {

            DialogResult result = System.Windows.Forms.MessageBox.Show("All unsaved help text will be lost!\nSave help file before returning to PsSnapin page?", "All unsaved help text will be lost!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                if (MainWindow.HelpFilePath == "")
                {
                    SaveHelpFileAs_Click(sender, e);
                }
                else
                {
                    SaveHelpFile(MainWindow.HelpFilePath);
                }
            }

            MainWindow.NavControl.CmdletTreeView.Items.Clear();
            MainWindow.MainGrid1.errorLable.Visibility = Visibility.Hidden;
            MainWindow.DescriptionControl1.Visibility = Visibility.Hidden;
            MainWindow.resetDescriptionPage();
            MainWindow.ResetExamplesPage();
            MainWindow.ResetLinksPage();
            MainWindow.resetParameterPage();
            MainWindow.NavControl.NavigationGrid.Visibility = Visibility.Hidden;
            MainWindow.ParametersControl1.Visibility = Visibility.Hidden;
            MainWindow.ExamplesControl1.Visibility = Visibility.Hidden;
            MainWindow.RelatedLinks1.Visibility = Visibility.Hidden;
            MainWindow.EmptyParameterControl1.Visibility = Visibility.Hidden;
            MainWindow.HeaderControl1.StartOverButton.Visibility = Visibility.Hidden;
            MainWindow.MainGrid1.Visibility = Visibility.Visible;
            //this.CmdletTreeView.Visibility = Visibility.Hidden;
            // this.NavigationSplitter.Visibility = Visibility.Hidden;
            MainWindow.CmdletsHelps = new Collection<cmdletDescription>();
            MainWindow.HeaderControl1.___Button__Save_File.Visibility = Visibility.Hidden;
            MainWindow.HeaderControl1.___Button__SaveHelpFileAs.Visibility = Visibility.Hidden;

            MainWindow.HeaderControl1.StartOverButton.Width = 0;



            //CmdletsHelps = new Collection<cmdletDescription>();
            MainWindow.MainGrid1.errorLable.Content = "";
            MainWindow.HelpFilePath = "";


        }

        /// <summary>
        /// The actual Save file routine.
        /// It calls the XML writer routine.
        /// </summary>
        /// <param name="FilePath"></param>
        public void SaveHelpFile(String FilePath)
        {
            Boolean save = true;
            if (MainWindow.ObsoleteInfo)
            {
                MessageBoxResult Result = System.Windows.MessageBox.Show("This action will make you loose all Obsolete Help info Marked in Bold red in the Tree View. Are you sure you want to proceed?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (Result == MessageBoxResult.No)
                {
                    save = false;
                }
            }

            // If we are sure we want to save the file then we do the actual save action.
            if (save)
            {
                try
                {
                    XmlWriter writer = null;
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = ("    ");
                    settings.NewLineHandling = NewLineHandling.None;

                    settings.ConformanceLevel = ConformanceLevel.Document;

                    writer = XmlWriter.Create(FilePath, settings);


                    writer.WriteStartDocument();

                    writer.WriteRaw("\r\n\r\n<helpItems xmlns=\"http://msh\" schema=\"maml\">\r\n\r\n");
                    foreach (cmdletDescription CmdletHelp in MainWindow.CmdletsHelps)
                    {
                        // Code that creates the MAML content for one cmdlet at a time.
                        SaveHelpFileBody(writer, CmdletHelp);
                    }
                    writer.WriteRaw("</helpItems>\r\n");

                    writer.Flush();
                    writer.Close();
                    System.Windows.Forms.MessageBox.Show(MainWindow.HelpFilePath + "\n\nFile saved.", "File saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Failed to save the help file");
                }
            }


        }

        /// <summary>
        /// This routine is used to create the one Cmdlet help MAML
        /// at a time.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="CmdletHelp"></param>
        public void SaveHelpFileBody(XmlWriter writer, cmdletDescription CmdletHelp)
        {
            try
            {

                writer.WriteRaw("<command:command xmlns:maml=\"http://schemas.microsoft.com/maml/2004/10\" xmlns:command=\"http://schemas.microsoft.com/maml/dev/command/2004/10\" xmlns:dev=\"http://schemas.microsoft.com/maml/dev/2004/10\">\r\n");
                writer = MainWindow.helpWriter.writeCmdletDetails(writer, CmdletHelp);

                //Add Syntax section <command:syntax>
                writer.WriteRaw("	<command:syntax>");

                Collection<parameterDecription> ParamDescription = CmdletHelp.ParameterDecription;
                Collection<parameterSet> HelpParameterSets = CmdletHelp.ParameterSets;
                //Iterate through the Syntax section and add Syntax items.
                //foreach (parameterset in paramDescription.ParameterSets[0].Parameters[0].Name
                if (HelpParameterSets != null)
                {
                    foreach (parameterSet HelpParameterSet in HelpParameterSets)
                    {
                        writer.WriteRaw("		<command:syntaxItem>\r\n");
                        //Cdmlet Name.
                        writer.WriteRaw("			<maml:name>");
                        writer.WriteRaw(CmdletHelp.CmdletName);
                        writer.WriteRaw("</maml:name>\r\n");
                        // writer.WriteRaw("		<!--New Syntax Item-->\r\n");
                        if (HelpParameterSet.Parameters != null)
                        {
                            foreach (parametersetParameter HelpParameter in HelpParameterSet.Parameters)
                            {
                                foreach (parameterDecription param in ParamDescription)
                                {
                                    if (param.Name.ToLower() == HelpParameter.Name.ToLower())
                                    {
                                        writer = MainWindow.helpWriter.createSyntaxItem(writer, param, CmdletHelp);
                                        break;
                                    }
                                }
                            }
                        }
                        //End <command:syntaxItem>
                        writer.WriteRaw("		</command:syntaxItem>\r\n");
                    }
                }
                else
                {
                    writer.WriteRaw("		<command:syntaxItem>\r\n");
                    writer.WriteRaw("		</command:syntaxItem>\r\n");

                }


                writer.WriteRaw("	</command:syntax>\r\n");
                //writer.WriteRaw("	</command:syntax>\r\n");

                //Add Parameters section <command:parameters>
                writer.WriteRaw("	<command:parameters>\r\n");
                //writer.WriteComment("Parameters section");

                //Iterate through the parameters section and add parameters Items.


                foreach (parameterDecription parameter in ParamDescription)
                {
                    writer = MainWindow.helpWriter.createParameters(writer, parameter);
                }


                writer.WriteRaw("	</command:parameters>\r\n");


                //Input Object Section
                writer = MainWindow.helpWriter.createInputSection(writer, CmdletHelp);

                //Output Object Section
                writer = MainWindow.helpWriter.createOutputSection(writer, CmdletHelp);


                // Error Section (Static section not used)
                //<command:terminatingErrors />
                //<command:nonTerminatingErrors />
                writer.WriteRaw("	<command:terminatingErrors>\r\n");
                //writer.WriteComment("Terminating errors section");
                writer.WriteRaw("	</command:terminatingErrors>\r\n");
                writer.WriteRaw("	<command:nonTerminatingErrors>\r\n");
                //writer.WriteComment("Non terminating errors section");
                writer.WriteRaw("	</command:nonTerminatingErrors>\r\n");


                //AlertSet  <!-- Notes section  -->
                writer = MainWindow.helpWriter.createAlertSetSection(writer, CmdletHelp);

                //Examples section.
                //Examples header goes here <command:examples>
                writer.WriteRaw("	<command:examples>\r\n");

                //Iterate through all the examples here
                if (CmdletHelp.Examples != null)
                {
                    foreach (example ExampleRecord in CmdletHelp.Examples)
                    {
                        writer = MainWindow.helpWriter.createExampleItemSection(writer, ExampleRecord);
                        //End examples section

                    }
                }
                writer.WriteRaw("	</command:examples>\r\n");


                //Links section
                // <maml:relatedLinks>
                writer.WriteRaw("	<maml:relatedLinks>\r\n");
                //Iterate through the links

                if (CmdletHelp.RelatedLinks != null)
                {
                    foreach (relatedlink RelatedLinkRecord in CmdletHelp.RelatedLinks)
                    {
                        writer = MainWindow.helpWriter.createLinksSection(writer, RelatedLinkRecord);

                    }
                }
                writer.WriteRaw("	</maml:relatedLinks>\r\n");

                //Write the end node for the starting <command:command> node.
                //writer.WriteRaw();
                writer.WriteRaw("</command:command>\r\n");
                //  }

                //Write the help file.

                //  }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error writing the XML file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //FailedToWrite = true;
                if (writer != null)
                {
                    writer.Close();
                }

            }
        }

        /// <summary>
        /// This routine loads a PsSnapin and prompts for a
        /// Help file to load.
        /// We do not do any validation to ensure the help file 
        /// matches the snapin and we do not report errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BrowseHelpFile_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.MainGrid1.PsSnapinList.SelectedItems.Count > 0 && MainWindow.MainGrid1.PsSnapinList.Items.Count > 0)
            {
                try
                {
                    if (MainWindow.CmdletsHelps.Count > 0)
                    {
                        DialogResult result = System.Windows.Forms.MessageBox.Show("This action deletes unsaved help file text.\n\nContinue?", "Open Help File", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            // Configure open file dialog box
                            OpenFileDialog dlg = new OpenFileDialog();
                            ModuleObject myview = new ModuleObject();
                            myview = (ModuleObject)MainWindow.MainGrid1.PsSnapinList.SelectedItem;
                            MainWindow.PsSnapinName = myview.Name;
                            MainWindow.PsSnapinModuleName = myview.Name;
                            dlg.FileName = MainWindow.PsSnapinModuleName + "-Help"; // Default file name
                            dlg.DefaultExt = ".xml"; // Default file extension
                            dlg.Filter = "PowerShell Help Xml files (.xml)|*.xml"; // Filter files by extension

                            // Show open file dialog box
                            Nullable<DialogResult> FODresult = dlg.ShowDialog();

                            // Process open file dialog box results
                            if (FODresult.Value == System.Windows.Forms.DialogResult.OK)
                            {
                                // Open document
                                MainWindow.HelpFilePath = dlg.FileName;
                                MainWindow.OldHelpFileExist = true;

                                LoadPsNapin();
                            }
                        }
                    }
                    else
                    {

                        // Configure open file dialog box
                        OpenFileDialog dlg = new OpenFileDialog();
                        ModuleObject myview = new ModuleObject();
                        myview = (ModuleObject)MainWindow.MainGrid1.PsSnapinList.SelectedItem;
                        MainWindow.PsSnapinName = myview.Name;
                        MainWindow.PsSnapinModuleName = myview.Name;
                        dlg.FileName = MainWindow.PsSnapinModuleName + "-Help"; // Default file name
                        dlg.DefaultExt = ".xml"; // Default file extension
                        dlg.Filter = "PowerShell Help Xml files (.xml)|*.xml"; // Filter files by extension

                        // Show open file dialog box
                        Nullable<DialogResult> FODresult = dlg.ShowDialog();

                        // Process open file dialog box results
                        if (FODresult.Value == System.Windows.Forms.DialogResult.OK)
                        {
                            // Open document
                            MainWindow.HelpFilePath = dlg.FileName;
                            MainWindow.OldHelpFileExist = true;

                            LoadPsNapin();
                        }

                    }

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error loading the help file.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Select a PsSnapin before opening a help file.", "PsSnapin Not Selected", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// This routine is used to cal Save Help file routine
        /// If this is not defined yet then call Save As instead.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void saveHelpInfoToTextFile_Click(object sender, RoutedEventArgs args)
        {
            if (MainWindow.CmdletsHelps.Count > 0)
            {
                if (MainWindow.HelpFilePath != "")
                {
                    SaveHelpFile(MainWindow.HelpFilePath);
                }
                else
                {
                    SaveHelpFileAs_Click(sender, args);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Cannot save the help file because a snapin is not loaded.", "Application Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// This routine saves the Help file into a new name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveHelpFileAs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MainWindow.CmdletsHelps.Count > 0)
            {
                try
                {
                    Boolean save = true;


                    if (save)
                    {

                        // Configure open file dialog box
                        SaveFileDialog dlg = new SaveFileDialog();
                        //OpenFileDialog dlg = new OpenFileDialog();
                        //ModuleObject SelectedSnapin = new ModuleObject();
                        //SelectedSnapin = (ModuleObject)MainWindow.MainGrid1.PsSnapinList.SelectedItem;
                        ////String PsSnapinName = SelectedSnapin[0].Name;

                        dlg.FileName = MainWindow.PsSnapinModuleName + "-Help"; // Default file name
                        dlg.DefaultExt = ".xml"; // Default file extension
                        dlg.Filter = "PowerShell Help Xml files (.xml)|*.xml"; // Filter files by extension

                        // Show open file dialog box
                        Nullable<DialogResult> result = dlg.ShowDialog();

                        // Process open file dialog box results
                        if (result.Value == System.Windows.Forms.DialogResult.OK)
                        {
                            // Open document
                            MainWindow.HelpFilePath = dlg.FileName;
                            MainWindow.OldHelpFileExist = true;
                            SaveHelpFile(dlg.FileName);
                            // LoadPsNapinAndPopulateCmdlet(this, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error loading the help file.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Select a PsSnapin before opening a help file.", "PsSnapin Not Selected", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// About dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void About_Click(object sender, RoutedEventArgs e)
        {
            About AboutDlg = new About();
            AboutDlg.Show();

        }

        /// <summary>
        /// TODO: add code to invoke the help CHM file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HelpLink_Click(object sender, RoutedEventArgs e)
        {
            //Load the help file from the current directory
            try
            {
                AppDomain MyAppDomain = AppDomain.CurrentDomain;
                String Mydir = MyAppDomain.BaseDirectory;
                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                processStartInfo.FileName = Mydir + "\\CmdletHelpEditor.doc";
                processStartInfo.UseShellExecute = true;

                System.Diagnostics.Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to load the help file. Please reinstall this aplication.\n" + ex.Message, "Failed to load the help contents", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Exit and terminate the application.
        /// We do not do any checing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Exit();
        }

        /// <summary>
        /// This routine Loads a PsSnapin without a help file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadPsNapinWithoutHelpFile(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CmdletsHelps.Count > 0)
            {
                DialogResult result = System.Windows.Forms.MessageBox.Show("When you load a snap-in, you lose any unsaved help file text.\n\nContinue?", "Select New PsSnapin", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    MainWindow.HelpFilePath = "";
                    MainWindow.OldHelpFileExist = false;
                    LoadPsNapin();
                }
            }
            else
            {
                MainWindow.HelpFilePath = "";
                MainWindow.OldHelpFileExist = false;
                LoadPsNapin();
            }

        }

        public static void LoadPsNapin()
        {
            String PsSnapinModuleName;
            String PsSnapinName;
            // Make sure the correct PsSNapin is selected before executing this routine.            
            if ( HelpEditorOS.MainWindow.selectedModule == null/*(this.MainGrid1.PsSnapinList.Items.Count == 0) || (this.MainGrid1.PsSnapinList.SelectedItems.Count == 0)*/)
            {
                System.Windows.MessageBox.Show("Select a PsSnapin before opening a help file.", "PsSnapin Not Selected", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
            else
            {

                ModuleObject myview = new ModuleObject();
                myview = (ModuleObject)HelpEditorOS.MainWindow.selectedModule;
                //myview = (List<SnapinView>)MainGrid.PsSnapinList.SelectedItem;
                //Name of the Assembly
                PsSnapinModuleName = myview.Name;
                //Name of the Snapin
                PsSnapinName = myview.Name;
                //Load the proper UI grid and make the rest not visible.
                MainWindow.NavControl.CmdletTreeView.Items.Clear();
                MainWindow.PsSnapinNode = new TreeViewItem();
                MainWindow.CmdletsHelps = new Collection<cmdletDescription>();
                MainWindow.HeaderControl1.___Button__Save_File.Visibility = Visibility.Visible;
                MainWindow.HeaderControl1.___Button__SaveHelpFileAs.Visibility = Visibility.Visible;
                MainWindow.DescriptionControl1.Visibility = Visibility.Hidden;
                MainWindow.NavControl.NavigationGrid.Visibility = Visibility.Visible;
                MainWindow.ParametersControl1.Visibility = Visibility.Hidden;
                MainWindow.MainGrid1.Visibility = Visibility.Hidden;
                MainWindow.EmptyParameterControl1.Visibility = Visibility.Hidden;
                MainWindow.ExamplesControl1.Visibility = Visibility.Hidden;
                MainWindow.RelatedLinks1.Visibility = Visibility.Hidden;
                MainWindow.HeaderControl1.StartOverButton.Visibility = Visibility.Visible;
                //this.NavigationSplitter.Visibility = Visibility.Visible;
                //Not sure I still need this...
                MainWindow.HeaderControl1.StartOverButton.Width = 136;


                try
                {
                    //initialize the XMLHelper class
                    // this is needed to write the MAML file.
                    XMLReaderHelper xmlHelper = new XMLReaderHelper();

                    //Create a default RunspaceConfiguration 
                    RunspaceConfiguration config = RunspaceConfiguration.Create();

                    //Add GetProcPSSnapIn01 to config.
                    PSSnapInException warning;
                    //Add non PowerShell code PS Snapins to the list.
                    //PowerShell's ones are loaded later.

                    // Add the AspenCmdlets snapin
                    //config.AddPSSnapIn("AspenCmdletManagement", out warning);



                    // Create a runspace. 
                    // (Note that no PSHost instance is supplied in the CreateRunspace call
                    // so the default PSHost implementation is used.)
                    Runspace myRunSpace = RunspaceFactory.CreateRunspace(config);
                    myRunSpace.Open();

                    // Create a pipeline with get-comand -pssnapin the seleceted PsSnapin

                    //this.PsSnapinNameLabel.Content = "Loaded PsSnapin: " + PsSnapinName;
                    String MyInvokationScript = null;
                    if (myview.ModuleType.ToString().ToLower() == "manifest")
                    {
                        MyInvokationScript = "import-module -name " + PsSnapinName + "; " + "get-command -Module " + PsSnapinName;
                    }
                    else
                    {
                        // If this is a PowerShell Snapin, do not add it. We already have it.
                        if (PsSnapinName != "Microsoft.PowerShell.Core" &&
                            PsSnapinName != "Microsoft.PowerShell.Host" &&
                            PsSnapinName != "Microsoft.PowerShell.Management" &&
                            PsSnapinName != "Microsoft.PowerShell.Security" &&
                            PsSnapinName != "Microsoft.PowerShell.Utility" &&
                            PsSnapinName != "Microsoft.WSMan.Management")
                        // PsSnapinName != "AspenCmdletManagement")
                        {
                            config.AddPSSnapIn(PsSnapinName, out warning);

                            if (warning != null)
                            {
                                // A warning is not expected, but if one is detected
                                // write the warning and return.
                                System.Console.Write(warning.Message);
                                return;
                            }
                        }
                        MyInvokationScript = "get-command -pssnapin " + PsSnapinName;
                    }

                    Pipeline pipeLine = myRunSpace.CreatePipeline(MyInvokationScript);

                    //PsCmdlets is the collection of all the CmdletInfo objects returned by GetCommand.
                    MainWindow.results = pipeLine.Invoke();
                    if (MainWindow.results.Count == 0)
                    {
                        return;
                    }

                    // Iterate through all the Cmdlets and populate the cmdletDescription help object (CmdletsHelps).

                    // Give the header of the tree view the name of the Snapin.
                    MainWindow.PsSnapinNode.Header = PsSnapinName;

                    // Objects read from the Spec data base.
                    Collection<PSObject> PSSpecParameterInfo = new Collection<PSObject>();
                    Collection<PSObject> PSSpecParameterSetEntries = new Collection<PSObject>();
                    Collection<PSObject> PSSpecCmdletInfo = new Collection<PSObject>();

                    //if we are in the online mode get the cmdlet names for the selected project.
                    //if (ProjectName != null && ProjectName != "")
                    //{
                    //    MyInvokationScript = "get-cmdlet -ProjectName \"" + ProjectName + "\"";
                    //    pipeLine = myRunSpace.CreatePipeline(MyInvokationScript);
                    //    PSSpecCmdletInfo = pipeLine.Invoke();

                    //    // Initialize a SpecCmdlet info record.
                    //    SpecCmdlet = new Microsoft.Aspen.CmdletManagement.AspenCmdlet();

                    //    // Get all the parameters in the project. This improves the db access performance.
                    //    MyInvokationScript = "Get-CmdletParameter -ProjectName \"" + ProjectName + "\"";
                    //    pipeLine = myRunSpace.CreatePipeline(MyInvokationScript);
                    //    PSSpecParameterInfo = pipeLine.Invoke();

                    //    // Get all the parameter set entries at once. This improves the db access performance.
                    //    MyInvokationScript = "Get-CmdletParameterSetEntry -ProjectName \"" + ProjectName + "\"";
                    //    pipeLine = myRunSpace.CreatePipeline(MyInvokationScript);
                    //    PSSpecParameterSetEntries = pipeLine.Invoke();
                    //}
                    //else
                    //{
                    // If this is offline, change the lable text note Original info rather then spec info
                    MainWindow.DescriptionControl1.OldInputTypeDescLable.Content = "Original Input Type Description";
                    MainWindow.DescriptionControl1.OldInputTypeLable.Content = "Original Input Type";
                    MainWindow.DescriptionControl1.ShortDescriptionLable_Copy1.Content = "Original Short Description";
                    MainWindow.DescriptionControl1.DetailedDescriptionLable_Copy.Content = "Original Detailed Description";
                    MainWindow.DescriptionControl1.OldInputTypeDescLable.Content = "Original Input Type Description";
                    MainWindow.DescriptionControl1.OldOutputTypeLable.Content = "Original Return Type";
                    MainWindow.DescriptionControl1.OldOutputTypeDescLable.Content = "Original Return Type Description";
                    MainWindow.DescriptionControl1.OldNotesDescriptionLable.Content = "Original Notes";
                    MainWindow.ExamplesControl1.ExampleDescriptionLabel_Copy.Content = "Original Example Description";
                    MainWindow.RelatedLinks1.OldRelatedLinkLabel.Content = "Original Related Link";
                    MainWindow.ParametersControl1.OldParameterDescLable.Content = "Original Parameter Description";
                    MainWindow.ParametersControl1.OldGlobbingCheckBox.Content = "Original Globbing";
                    MainWindow.ParametersControl1.DefaultValueLable_Copy.Content = "Original Default Value";
                    //}

                    // iterate through the cmdlets info from code. 
                    // foreach cmdlet in code update the tree.
                    // when you are done. do the remaining ones in help only or in spec only.
                    foreach (PSObject psCmdletInfo in MainWindow.results)
                    {
                        //Get the CmdletHelp data from get-Command
                        cmdletDescription CmdletHelp = new cmdletDescription();

                        // this contain all the parameters in the cmdlet.
                        Collection<parameterDecription> parameterDescriptions = new Collection<parameterDecription>();

                        // CmdleHelp holds all info on the psCmdldetInfo in the foreach loop.
                        CmdletHelp.CmdletName = (String)psCmdletInfo.Members["Name"].Value;
                        if (psCmdletInfo.Members["Verb"] == null)
                        {
                            String[] verbNoum = CmdletHelp.CmdletName.Split('-');
                            CmdletHelp.Verb = verbNoum[0];
                            CmdletHelp.Noun = verbNoum[1];
                        }
                        else
                        {
                            CmdletHelp.Verb = (String)psCmdletInfo.Members["Verb"].Value;
                            CmdletHelp.Noun = (String)psCmdletInfo.Members["Noun"].Value;
                        }

                        // SpecCmdlet = new Microsoft.Aspen.CmdletManagement.AspenCmdlet();

                        // If we are in the online mode, get the spec info for the code cmdlet we are working against.
                        //if (ProjectName != null && ProjectName != "")
                        //{
                        //    foreach (PSObject PSSpecCmdlet in PSSpecCmdletInfo)
                        //    {
                        //        //Fine the matching spec cmdlet record.
                        //        String SpecCmdletName = (String)PSSpecCmdlet.Members["Name"].Value;
                        //        if (SpecCmdletName == CmdletHelp.CmdletName)
                        //        {
                        //            //This is the cmdlet we want
                        //            SpecCmdlet = (Microsoft.Aspen.CmdletManagement.AspenCmdlet)PSSpecCmdlet.ImmediateBaseObject;
                        //        }
                        //    }
                        //}

                        // find the proper Spec Parameters and the spec parameter entries.
                        // I cannot initialize these collection in the if statement.
                        //Collection<Microsoft.Aspen.CmdletManagement.AspenCmdletParameter> SpecParameters = new Collection<Microsoft.Aspen.CmdletManagement.AspenCmdletParameter>();
                        //Collection<Microsoft.Aspen.CmdletManagement.AspenCmdletParameterSetEntry> SpecParameterSetEntries = new Collection<Microsoft.Aspen.CmdletManagement.AspenCmdletParameterSetEntry>();

                        //// If this is an online mode...
                        //if (ProjectName != null && ProjectName != "")
                        //{
                        //    //Retrieve the parameter info for the specific cmdlet
                        //    foreach (PSObject PSSpecParamInfo in PSSpecParameterInfo)
                        //    {
                        //        // Get the Spec Parameter and make sure it belongs to our cmdlet.
                        //        Microsoft.Aspen.CmdletManagement.AspenCmdletParameter SpecParameter = (Microsoft.Aspen.CmdletManagement.AspenCmdletParameter) PSSpecParamInfo.ImmediateBaseObject;
                        //        if (SpecParameter.CmdletName == SpecCmdlet.Name)
                        //        {
                        //            SpecParameters.Add(SpecParameter);
                        //        }
                        //    }

                        //    // Do the same for Parameter Set entries.
                        //    foreach (PSObject PSSpecParameterSetEntry in PSSpecParameterSetEntries)
                        //    {
                        //        Microsoft.Aspen.CmdletManagement.AspenCmdletParameterSetEntry SpecParameterSetEntry = (Microsoft.Aspen.CmdletManagement.AspenCmdletParameterSetEntry)PSSpecParameterSetEntry.ImmediateBaseObject;
                        //        if (SpecParameterSetEntry.CmdletName == SpecCmdlet.Name)
                        //        {
                        //            SpecParameterSetEntries.Add(SpecParameterSetEntry);
                        //        }
                        //    }

                        //}

                        // Set the spec info in the Old record. In the online mode Old mean spec
                        // In the offline mode, Old means Original (coming from the help file)
                        //CmdletHelp.OldShortDescription = SpecCmdlet.ShortDescription;
                        //CmdletHelp.OldLongDescription = SpecCmdlet.LongDescription;
                        //CmdletHelp.OldOutputType = SpecCmdlet.OutputObject;
                        //CmdletHelp.OldOutputDesc = SpecCmdlet.OutputObjectDescription;
                        //CmdletHelp.OldNote = SpecCmdlet.Notes;
                        //ParameterSets;
                        ReadOnlyCollection<CommandParameterSetInfo> ParameterSets = null;
                        try
                        {
                            ParameterSets = (ReadOnlyCollection<CommandParameterSetInfo>)psCmdletInfo.Members["ParameterSets"].Value;
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(CmdletHelp.CmdletName + ": " + ex.Message, CmdletHelp.CmdletName + " failed to load the parametersets!");
                            if (CmdletHelp.ParameterSets == null)
                            {
                                Collection<parameterSet> _ParameterSets = new Collection<parameterSet>();
                                CmdletHelp.ParameterSets = _ParameterSets;
                            }
                            //continue;
                        }
                        if (ParameterSets != null)
                        {

                            //Iterate through all the ParameterSets and get the parameter records.
                            foreach (CommandParameterSetInfo ParameterSet in ParameterSets)
                            {
                                parameterSet helpParameterSet = new parameterSet();
                                Collection<parametersetParameter> parameterSetParameters = new Collection<parametersetParameter>();

                                helpParameterSet.Name = ParameterSet.Name;

                                //Get the parameters in each parameter set and disregard the common ones.
                                //Also remove duplicates within the parameter sets.
                                ReadOnlyCollection<CommandParameterInfo> Parameters = ParameterSet.Parameters;
                                Collection<parametersetParameter> testParameters = new Collection<parametersetParameter>();
                                foreach (CommandParameterInfo Parameter in Parameters)
                                {
                                    parametersetParameter ParametersetParameter = new parametersetParameter();
                                    parameterDecription ParameterDescriptionInstance = new parameterDecription();

                                    String ParameterName = Parameter.Name;
                                    string ParameterNameLower = ParameterName.ToLower();
                                    //Skip the Ubiquiteous parameters.
                                    if (ParameterNameLower != "verbose" && ParameterNameLower != "debug"
                                        && ParameterNameLower != "erroraction" && ParameterNameLower != "errorvariable"
                                        && ParameterNameLower != "outvariable" && ParameterNameLower != "outbuffer"
                                        && ParameterNameLower != "warningvariable" && ParameterNameLower != "warningaction"
                                        )
                                    {
                                        parametersetParameter parSetParameter = new parametersetParameter();

                                        parSetParameter.Name = ParameterName;
                                        //Microsoft.Aspen.CmdletManagement.AspenCmdletParameter SpecParam = new Microsoft.Aspen.CmdletManagement.AspenCmdletParameter();
                                        //Microsoft.Aspen.CmdletManagement.AspenCmdletParameterSetEntry SpecParamEntry = new Microsoft.Aspen.CmdletManagement.AspenCmdletParameterSetEntry();

                                        ////find the matching parameter from the spec data base.
                                        //// We do not care about exact match. This is the 90% case if not all the metadata
                                        //// are identical, we do not care. we take the last hit.
                                        //foreach ( Microsoft.Aspen.CmdletManagement.AspenCmdletParameter specparam in SpecParameters)
                                        //{
                                        //    if (specparam.Name == ParameterName)
                                        //    {
                                        //        //This is the one we want.
                                        //        SpecParam = specparam;
                                        //    }
                                        //}

                                        // Do the same for the paramter set entry. Not all the metadata are available in the 
                                        // paramerter entry. I need to find the matching parameter set entry as well.
                                        // Here I also do a last match as well.
                                        //foreach (Microsoft.Aspen.CmdletManagement.AspenCmdletParameterSetEntry parmentry in SpecParameterSetEntries)
                                        //{
                                        //    if (parmentry.ParameterName == ParameterName)
                                        //    {
                                        //        SpecParamEntry = parmentry;
                                        //    }

                                        //}

                                        testParameters.Add(parSetParameter);
                                        //Start building the Help ParameterSet Object.
                                        Boolean ParameterExist = false;
                                        ParameterDescriptionInstance.MismatchInfo = false;
                                        ParametersetParameter.Name = ParameterName;
                                        ParameterDescriptionInstance.Name = ParameterName;
                                        ParameterDescriptionInstance.VFRA = Parameter.ValueFromRemainingArguments;
                                        //if (SpecParamEntry.ValueFromRemainingArguments != null)
                                        //{
                                        //    ParameterDescriptionInstance.SpecVFRA = (bool)SpecParamEntry.ValueFromRemainingArguments;
                                        //}

                                        if (ParameterDescriptionInstance.VFRA != ParameterDescriptionInstance.SpecVFRA)
                                        {
                                            ParameterDescriptionInstance.MismatchInfo = true;
                                        }

                                        ParameterDescriptionInstance.VFP = Parameter.ValueFromPipeline;
                                        //if (SpecParamEntry.ValueFromPipeline != null)
                                        //{
                                        //    ParameterDescriptionInstance.SpecVFP = (bool)SpecParamEntry.ValueFromPipeline;
                                        //}

                                        if (ParameterDescriptionInstance.VFP != ParameterDescriptionInstance.SpecVFP)
                                        {
                                            ParameterDescriptionInstance.MismatchInfo = true;
                                        }

                                        ParameterDescriptionInstance.VFPBPN = Parameter.ValueFromPipelineByPropertyName;
                                        //if (SpecParamEntry.ValueFromPipelineByPropertyName != null)
                                        //{
                                        //    ParameterDescriptionInstance.SpecVFPBPN = (bool)SpecParamEntry.ValueFromPipelineByPropertyName;
                                        //}

                                        if (ParameterDescriptionInstance.VFPBPN != ParameterDescriptionInstance.SpecVFPBPN)
                                        {
                                            ParameterDescriptionInstance.MismatchInfo = true;
                                        }

                                        ParameterDescriptionInstance.isMandatory = Parameter.IsMandatory;
                                        //if (SpecParamEntry.Mandatory != null)
                                        //{
                                        //    ParameterDescriptionInstance.SpecisMandatory = (bool)SpecParamEntry.Mandatory;
                                        //}

                                        if (ParameterDescriptionInstance.isMandatory != ParameterDescriptionInstance.SpecisMandatory)
                                        {
                                            ParameterDescriptionInstance.MismatchInfo = true;
                                        }

                                        ParameterDescriptionInstance.isDynamic = Parameter.IsDynamic;
                                        //if (SpecParam.Dynamic != null)
                                        //{
                                        //    ParameterDescriptionInstance.SpecisDynamic = (bool)SpecParam.Dynamic;
                                        //}

                                        if (ParameterDescriptionInstance.isDynamic != ParameterDescriptionInstance.SpecisDynamic)
                                        {
                                            ParameterDescriptionInstance.MismatchInfo = true;
                                        }

                                        String[] test = (String[])Parameter.ParameterType.ToString().Split('.');
                                        int length = test.Length;
                                        ParameterDescriptionInstance.ParameterType = test[length - 1];
                                        length = ParameterDescriptionInstance.ParameterType.Length;
                                        if (ParameterDescriptionInstance.ParameterType[length - 1] == ']' && ParameterDescriptionInstance.ParameterType[length - 2] != '[')
                                        {
                                            ParameterDescriptionInstance.ParameterType = ParameterDescriptionInstance.ParameterType.TrimEnd(']');
                                        }
                                        //ParameterDescriptionInstance.ParameterType = Parameter.ParameterType.Name;

                                        //if (SpecParam.Type != null)
                                        //{
                                        //    test = (String[])SpecParam.Type.ToString().Split('.');
                                        //    length = test.Length;
                                        //    ParameterDescriptionInstance.SpecParameterType = test[length - 1];
                                        //}

                                        if (ParameterDescriptionInstance.ParameterType != ParameterDescriptionInstance.SpecParameterType)
                                        {
                                            ParameterDescriptionInstance.MismatchInfo = true;
                                        }


                                        //If the parameter is not positioned call it named else give it the 
                                        //position and convert it to String
                                        //if (SpecParamEntry.Position != null)
                                        //{
                                        //    ParameterDescriptionInstance.SpecPosition = (int)SpecParamEntry.Position;
                                        //}
                                        if (Parameter.Position < 0)
                                        {
                                            ParameterDescriptionInstance.Position = "named";
                                        }
                                        else
                                        {
                                            // for some reason in the Help documentation the writers like posinal value to start 
                                            // wit one. This is why we increment the numbet by one. Any code/MAML comparer tool need to 
                                            // make a note of this.
                                            ParameterDescriptionInstance.Position = (String)(Parameter.Position + 1).ToString();
                                        }

                                        //ParameterDescriptionInstance.OldDescription = SpecParam.Description;

                                        //if (SpecParam.AllowGlobbing != null)
                                        //{
                                        //    ParameterDescriptionInstance.OldGlobbing = (bool)SpecParam.AllowGlobbing;
                                        //}

                                        if (ParameterDescriptionInstance.Globbing != ParameterDescriptionInstance.OldGlobbing)
                                        {
                                            ParameterDescriptionInstance.MismatchInfo = true;
                                        }

                                        //Collect the Attributes on the parameters and build them.
                                        // these are not consumed anywhere except as an FYI to the writer.
                                        // We do not store this data in the MAML.
                                        Collection<parameterAttribute> Attributes = new Collection<parameterAttribute>();
                                        foreach (Attribute ParameterAttribute in Parameter.Attributes)
                                        {
                                            try
                                            {
                                                parameterAttribute paramAttribute = new parameterAttribute();
                                                paramAttribute.Attribute = ParameterAttribute.ToString();
                                                Attributes.Add(paramAttribute);
                                            }
                                            catch (Exception ex)
                                            {
                                                System.Windows.MessageBox.Show(ex.Message, "Error loading the parameter attributes.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                                            }
                                        }
                                        ParameterDescriptionInstance.Attributes = Attributes;

                                        //Collect the Aliases on the parameter and build the Aliases record.
                                        // Again, this is not used in MAML. Just as an FYI to the writer.
                                        Collection<parameterAlias> Aliases = new Collection<parameterAlias>();
                                        foreach (String ParameterAlias in Parameter.Aliases)
                                        {
                                            parameterAlias paramAlias = new parameterAlias();
                                            paramAlias.Alias = ParameterAlias;
                                            Aliases.Add(paramAlias);
                                        }
                                        ParameterDescriptionInstance.Aliases = Aliases;

                                        //only add non duplicate parameters to the Parm description record 
                                        //for this cmdlet.
                                        foreach (parameterDecription param in parameterDescriptions)
                                        {
                                            if (param.Name == ParameterDescriptionInstance.Name)
                                            {
                                                ParameterExist = true;
                                            }
                                        }
                                        if (!ParameterExist)
                                        {
                                            parameterDescriptions.Add(ParameterDescriptionInstance);

                                        }

                                    }


                                }
                                if (testParameters != null)
                                {
                                    helpParameterSet.Parameters = testParameters;
                                }

                                // Initialize the record if there were no parameters in this cmdlet.
                                if (CmdletHelp.ParameterSets == null)
                                {
                                    Collection<parameterSet> _ParameterSets = new Collection<parameterSet>();
                                    CmdletHelp.ParameterSets = _ParameterSets;
                                }
                                CmdletHelp.ParameterSets.Add(helpParameterSet);// = tempParSet;
                            }

                        }

                        //Update the global CmdletHelp record with info from code
                        //before we read the UA info from the help file if one exists.
                        //We only rely on code for the Cmdlet Help metadata.
                        CmdletHelp.ParameterDecription = parameterDescriptions;

                        //if a help file is loaded, then read its contents and add it to
                        //the CmdletHelp record.
                        if (MainWindow.OldHelpFileExist)
                        {
                            CmdletHelp = XMLReaderHelper.GetExistingHelpInfo(CmdletHelp, CmdletHelp.CmdletName, MainWindow.HelpFilePath);
                        }

                        //Start building the Cmdlet navigation tree
                        TreeViewItem Node = new TreeViewItem();
                        // The header is the display text for this node.
                        Node.Header = CmdletHelp.CmdletName;

                        // Check if detailed description and Short description have text, then consider
                        // this record is complete. This is based on the speced behavior.
                        // Make the text green and not bold.
                        if ((CmdletHelp.LongDescription != null || CmdletHelp.LongDescription != "") &&
                            (CmdletHelp.ShortDescription != null || CmdletHelp.ShortDescription != ""))
                        {
                            Node.Foreground = Brushes.Green;
                        }

                        // This boolean flag is used to annotate that there is a mismatch in the parameter metadata
                        // between spec and code in the online mode.
                        Boolean paramMistmatch = false;

                        TreeViewItem ParameterNode = new TreeViewItem();
                        TreeViewItem ExamplesNode = new TreeViewItem();
                        TreeViewItem LinksNode = new TreeViewItem();

                        // Add the Parameters record to the tree.
                        // Start with the assumption that the parameter is complete.
                        ParameterNode.Foreground = Brushes.Green;
                        foreach (parameterDecription nodeparameterDesc in CmdletHelp.ParameterDecription)
                        {
                            TreeViewItem paramItem = new TreeViewItem();
                            paramItem.Header = (nodeparameterDesc.Name);
                            paramItem.DataContext = nodeparameterDesc;

                            // If the parameter item under the Parameters node has description then 
                            // mark it in green otherwise mark it in black to indicate the contents are not complete.
                            if ((nodeparameterDesc.NewDescription == null || nodeparameterDesc.NewDescription == ""))
                            {
                                paramItem.Foreground = Brushes.Black;
                                ParameterNode.Foreground = Brushes.Black;
                            }
                            else
                            {
                                paramItem.Foreground = Brushes.Green;
                            }

                            // if this is a parameter that only exist in the help MAML file and not
                            // in code, then mark it in bold red
                            if (nodeparameterDesc.HelpOnlyParameter)
                            {
                                paramItem.Foreground = Brushes.Red;
                                paramItem.FontWeight = FontWeights.Bold;
                                // Set this ObsoleteInfo flag to true to indicate that 
                                // we do have onsolete info in the tree.
                                // We use this flag later to warn the user that saving the MAML file
                                // will cause them to loose this info.
                                MainWindow.ObsoleteInfo = true;
                                Node.Foreground = Brushes.Red;
                            }

                            // If we are in the online mode we check if there were mismatch info reported and we mark them red.
                            if (MainWindow.ProjectName != null && MainWindow.ProjectName != "")
                            {
                                if (nodeparameterDesc.MismatchInfo)
                                {
                                    if ((String)(paramItem.Header).ToString().ToLower() != "whatif" && (String)(paramItem.Header).ToString().ToLower() != "confirm")
                                    {
                                        paramItem.Foreground = Brushes.Red;
                                        paramItem.FontWeight = FontWeights.Normal;
                                        // This falg is set to mark the parent Parameters node red as well.
                                        paramMistmatch = true;
                                    }
                                }
                            }
                            ParameterNode.Items.Add(paramItem);


                        }

                        ParameterNode.Header = "Parameters";
                        ParameterNode.DataContext = CmdletHelp.ParameterDecription;


                        // Mark the Parameters node red in cas of Mismatch
                        if (paramMistmatch)
                        {
                            ParameterNode.Foreground = Brushes.Red;
                            ParameterNode.FontWeight = FontWeights.Normal;
                        }

                        ExamplesNode.Header = "Examples";
                        ExamplesNode.DataContext = new Collection<example>();
                        // We use this to keep track of the spec example especially when the user is 
                        // trying to create a new Help example, he should have access to the spec examples.
                        //ExamplesNode.Resources.Add("SpecExample", SpecCmdlet.SpecExamples);

                        // Do this if we do have example contents for this cmdlet
                        if (CmdletHelp.Examples != null)
                        {
                            if (CmdletHelp.Examples.Count > 0)
                            {
                                // if we have contents then the mother examples node need to be green
                                ExamplesNode.Foreground = Brushes.Green;
                                ExamplesNode.DataContext = CmdletHelp.Examples;


                                foreach (example examp in CmdletHelp.Examples)
                                {
                                    // add the spec example to every example record in the online mode.
                                    // this is redundant. we could use the parent Spec example record and add it.
                                    // this code was here before the Examples.Resources section was intrduced above.
                                    // I will leave it for now.
                                    //if (ProjectName != null && ProjectName != "")
                                    //{
                                    //    examp.OldExampleDescription = SpecCmdlet.SpecExamples;
                                    //}

                                    TreeViewItem exmpNode = new TreeViewItem();
                                    exmpNode.DataContext = examp;
                                    // if we do have text in the cmd and description section, then
                                    // mark that example record green otherwise black
                                    if (examp.ExampleCmd != null && examp.ExampleCmd != "" &&
                                        examp.ExampleDescription != null && examp.ExampleDescription != "")
                                    {
                                        exmpNode.Foreground = Brushes.Green;
                                    }
                                    else
                                    {
                                        exmpNode.Foreground = Brushes.Black;
                                        ExamplesNode.Foreground = Brushes.Black;
                                    }
                                    exmpNode.Header = examp.ExampleName;
                                    ExamplesNode.Items.Add(exmpNode);
                                }
                            }
                            else // if we have no examples mark the Examples node Black.
                            {
                                ExamplesNode.Foreground = Brushes.Black;
                            }

                        }
                        else
                        {
                            ExamplesNode.Foreground = Brushes.Black;
                        }

                        // We do the same thing in Related Links as we did with examples.
                        // very similar logic
                        LinksNode.Header = "Related Links";
                        LinksNode.DataContext = new Collection<relatedlink>();
                        // LinksNode.Resources.Add("SpecLinks", SpecCmdlet.RelatedTo);
                        if (CmdletHelp.RelatedLinks != null)
                        {
                            LinksNode.Foreground = Brushes.Black;
                            if (CmdletHelp.RelatedLinks.Count > 0)
                            {
                                LinksNode.Foreground = Brushes.Green;
                                LinksNode.DataContext = CmdletHelp.RelatedLinks;
                                foreach (relatedlink Link in CmdletHelp.RelatedLinks)
                                {
                                    //if (ProjectName != null && ProjectName != "")
                                    //{
                                    //    Link.OldLinkText = SpecCmdlet.RelatedTo;
                                    //}
                                    TreeViewItem linkNode = new TreeViewItem();
                                    linkNode.DataContext = Link;
                                    linkNode.Header = Link.LinkText;
                                    linkNode.Foreground = Brushes.Green;
                                    LinksNode.Items.Add(linkNode);
                                }
                            }
                            else
                            {
                                LinksNode.Foreground = Brushes.Black;
                            }
                        }
                        else
                        {
                            LinksNode.Foreground = Brushes.Black;
                        }

                        // Add the sub nodes to the cmdlet record node.
                        Node.Items.Add(ParameterNode);
                        Node.Items.Add(ExamplesNode);
                        Node.Items.Add(LinksNode);

                        // If there is no mismatch anywher mark the cmdlet green otherwise
                        // Red if errors or black if incomplete.
                        if (ExamplesNode.Foreground != Brushes.Green ||
                            LinksNode.Foreground != Brushes.Green ||
                            ParameterNode.Foreground != Brushes.Green ||
                            Node.Foreground != Brushes.Green)
                        {
                            if (paramMistmatch)
                            {
                                Node.Foreground = Brushes.Red;
                            }
                            else
                            {
                                Node.Foreground = Brushes.Black;
                            }
                        }

                        // Fill the cmdlet node with the data structure 
                        // we built above.
                        Node.Header = CmdletHelp.CmdletName;
                        Node.DataContext = CmdletHelp;


                        MainWindow.PsSnapinNode.Items.Add(Node);
                        MainWindow.CmdletsHelps.Add(CmdletHelp);


                    }

                    // If we loaded an existing help file, load all the cmdlets which are only in the 
                    // help document and not in code. These are the obsolete help content. We need thi in the event a 
                    // cmdlet was renamed.
                    if (MainWindow.HelpFilePath != "")
                    {
                        XMLReaderHelper.GetHelpInfoNotInCode(MainWindow.CmdletsHelps, MainWindow.HelpFilePath);
                    }

                    MainWindow.NavControl.Visibility = System.Windows.Visibility.Visible;
                    MainWindow.DescriptionControl1.Visibility = System.Windows.Visibility.Visible;                    
                    MainWindow.MainGrid1.Visibility = System.Windows.Visibility.Collapsed;

                    //Make sure the first Cmdlet in the list is expaneded.
                     MainWindow.NavControl.CmdletTreeView.Items.Add(MainWindow.PsSnapinNode);
                    TreeViewItem firstTreeViewItem = (TreeViewItem)MainWindow.NavControl.CmdletTreeView.Items[0];
                    firstTreeViewItem.IsExpanded = true;
                    firstTreeViewItem = (TreeViewItem)firstTreeViewItem.Items[0];
                    firstTreeViewItem.IsSelected = true;
                    firstTreeViewItem.IsExpanded = true;
                    //Close the Runspace handle.
                    myRunSpace.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Cmdlet Help Editor: Serious Error. Restart the program.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }
    }
}
