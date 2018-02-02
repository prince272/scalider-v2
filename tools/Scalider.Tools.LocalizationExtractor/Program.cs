using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

namespace Scalider
{
    internal static class Program
    {

        public static void Main(string[] args)
        {
            var projectRoot = new DirectoryInfo("./");
            var files = new[] {"./Program.cs", "./test/test.cshtml"};
            
            CSharpSyntaxTree.ParseText("");
            VisualBasicSyntaxTree.ParseText("");

            var project = RazorProject.Create("./");
            var o = project.EnumerateItems("./").ToArray();
            var engine =
                new RazorTemplateEngine(RazorEngine.CreateDesignTime(), project);
            
            var csdoc = engine.GenerateCode("./test/test.cshtml");
            var parsed = CSharpSyntaxTree.ParseText(csdoc.GeneratedCode);

            var sl = parsed.GetRoot().GetLocation().GetLineSpan().StartLinePosition;
            var t = csdoc.SourceMappings[0].GeneratedSpan.LineIndex == sl.Line;
        }

    }
}