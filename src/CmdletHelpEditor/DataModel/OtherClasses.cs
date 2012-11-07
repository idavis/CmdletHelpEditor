#region Using Directives

using System;
using System.Collections.ObjectModel;

#endregion

namespace CmdletHelpEditor.DataModel
{
    public class ClipBoardObject
    {
        public Object ClipBoardItem { set; get; }

        public String TypeName { set; get; }
    }

    /// <summary>
    ///   This is the Parameter Description Obejct used to hold
    ///   the temporary parameter help metadata.
    /// </summary>
    public class parameterDecription
    {
        private String _ParameterType = "";
        private String _SpecParameterType = "";

        /// <summary>
        ///   TODO// Need to remove this.
        /// </summary>
        public String CmdletName { set; get; }

        public Boolean HelpOnlyParameter { set; get; }

        public Boolean MismatchInfo { set; get; }

        /// <summary>
        ///   TODO// Need to remove this.
        /// </summary>
        public String DefaultValue { set; get; }


        /// <summary>
        ///   TODO// Need to remove this.
        /// </summary>
        public String OldDefaultValue { set; get; }


        /// <summary>
        ///   This is the new Parameter Description 
        ///   entered using this tool.
        /// </summary>
        public String NewDescription { set; get; }

        /// <summary>
        ///   This ist the Paramter Description entered
        ///   from the loaded help xml file.
        /// </summary>
        public String OldDescription { set; get; }

        /// <summary>
        ///   This is the Name of the parameter
        ///   used for identification.
        /// </summary>
        public String Name { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows wild card expansion
        /// </summary>
        public Boolean Globbing { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows wild card expansion
        /// </summary>
        public Boolean OldGlobbing { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean VFP { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean SpecVFP { set; get; }


        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean SpecVFPBPN { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean VFPBPN { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean VFRA { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean SpecVFRA { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean isMandatory { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean SpecisMandatory { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean isDynamic { set; get; }


        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Boolean SpecisDynamic { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public String ParameterType
        {
            set { _ParameterType = value; }
            get { return _ParameterType; }
        }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public String SpecParameterType
        {
            set { _SpecParameterType = value; }
            get { return _SpecParameterType; }
        }


        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public String Position { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public int SpecPosition { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Collection<parameterAlias> Aliases { set; get; }

        /// <summary>
        ///   This value holds a boolean switch 
        ///   describing whether the parameter allows VFP
        /// </summary>
        public Collection<parameterAttribute> Attributes { set; get; }


        /// <summary>
        ///   This is the ParameterID.
        /// </summary>
        public Int32 ParameterID { set; get; }
    }

    public class parameterAlias
    {
        public String Alias { set; get; }
    }

    public class parameterAttribute
    {
        public String Attribute { set; get; }
    }

    /// <summary>
    ///   This is the example record.
    /// </summary>
    public class example
    {
        public String ExampleDescription { set; get; }

        public String OldExampleDescription { set; get; }

        public String SpecExample { set; get; }


        public String ExampleCmd { set; get; }

        public String OldExampleCmd { set; get; }


        public String ExampleOutput { set; get; }

        public String OldExampleOutput { set; get; }


        public String ExampleName { set; get; }

        public String OldExampleName { set; get; }

        public Int32 ExampleID { set; get; }
    }

    /// <summary>
    ///   This is the related link item.
    /// </summary>
    public class relatedlink
    {
        public String LinkText { set; get; }

        public String OldLinkText { set; get; }


        public Int32 LinkID { set; get; }
    }

    /// <summary>
    ///   This is the ParameterSet Object which
    ///   contains parameter names.
    /// </summary>
    public class parameterSet
    {
        public String Name { set; get; }


        public Collection<parametersetParameter> Parameters { set; get; }
    }

    /// <summary>
    ///   This is the Parameteset parameter Name.
    /// </summary>
    public class parametersetParameter
    {
        public String Name { set; get; }
    }

    /// <summary>
    ///   This is the Parameter desciption object
    ///   created by this tool
    /// </summary>
    public class cmdletDescription
    {
        /// <summary>
        ///   This is the Cmdlet name that holds this parameter.
        /// </summary>
        public String CmdletName { set; get; }

        public String Verb { set; get; }

        public String Noun { set; get; }

        public String ShortDescription { set; get; }

        public String OldShortDescription { set; get; }

        public String LongDescription { set; get; }

        public String OldLongDescription { set; get; }

        public String InputType { set; get; }

        public String OldInputType { set; get; }


        public String InputDesc { set; get; }

        public String OldInputDesc { set; get; }

        public String OutputType { set; get; }

        public String OldOutputType { set; get; }

        public String OutputDesc { set; get; }

        public String OldOutputDesc { set; get; }


        public String Note { set; get; }

        public String OldNote { set; get; }


        public Collection<example> Examples { set; get; }


        public Collection<relatedlink> RelatedLinks { set; get; }


        public Collection<parameterDecription> ParameterDecription { set; get; }


        public Collection<parameterSet> ParameterSets { set; get; }

        #region Nested type: ClipBoardObject

        public class ClipBoardObject
        {
            public Object ClipBoardItem { set; get; }

            public String TypeName { set; get; }
        }

        #endregion
    }
}
