using System;

namespace NetForge.VsExtension
{
    // Mirrors VSCommandTable.vsct. The toolkit's PackageIds source generator is disabled in this package
    // configuration, so we author the command-set GUID + ids by hand (they must match the .vsct exactly).
    internal static class PackageGuids
    {
        public const string CmdSetString = "6f7a1b2c-3d4e-5f60-7182-93a4b5c6d7e8";
        public static readonly Guid CmdSet = new Guid(CmdSetString);
    }

    internal static class PackageIds
    {
        public const int NewProject = 0x0100;
        public const int WhatsInPro = 0x0101;
        public const int OpenConfigurator = 0x0102;
        public const int OpenDemo = 0x0103;
        public const int OpenDocs = 0x0104;
    }
}
