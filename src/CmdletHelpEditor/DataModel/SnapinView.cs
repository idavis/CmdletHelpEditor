#region Using Directives

using System;

#endregion

namespace CmdletHelpEditor.DataModel
{
    public class SnapinView
    {
        public String Name { set; get; }

        public String PsVersion { set; get; }

        public String PsDescription { set; get; }

        public String PsModule { set; get; }
    }
}
