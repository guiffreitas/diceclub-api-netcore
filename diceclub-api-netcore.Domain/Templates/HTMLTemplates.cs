using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.Templates
{
    public class HTMLTemplates
    {
        public static string SourcePath => Path.GetDirectoryName(Environment.CurrentDirectory)!;
        public static string EmailConfirmationPath => SourcePath + "\\diceclub-api-netcore.Domain\\Templates\\EmailConfirmation.html";
    }
}
