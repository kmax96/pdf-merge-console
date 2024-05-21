using System;
using System.Collections.Generic;
namespace pdf_merge_console
{
    class Program
    {
        static void Main(string[] args)
        {
            //  mergePdf();
            AddWaterMark();
        }


        private static void mergePdf()
        {
            List<string> files = new List<string>() {
                @"d:\Download\e.pdf",@"d:\Download\r.pdf" };
            string targetFilePath = @"d:\Download\re.pdf";
            ItextSharpHelper.MergePDFs(files, targetFilePath);


            Console.WriteLine("finished");
        }


        private static void AddWaterMark()
        {
            string from = @"d:\Download\re.pdf";
            string to = @"d:\Download\re2.pdf";
            ItextSharpHelper.AddWaterMark(to, from, "this is watermark");
            Console.WriteLine("Added Watermark");
        }
    }
}
