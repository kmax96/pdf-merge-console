using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Text;



    public class ItextSharpHelper
    {
        public static bool MergePDFs(List<string> fileNames, string targetPdf)
        {
            return MergePDFs(fileNames.ToArray(), targetPdf);
        }

        public static bool MergePDFs(string[] fileNames, string targetPdf)
        {
            bool merged = true;
            using (FileStream stream = new FileStream(targetPdf, FileMode.Create))
            {
                Document document = new Document();
                PdfCopy pdf = new PdfCopy(document, stream);
                PdfReader reader = null;
                try
                {
                    document.Open();
                    foreach (string file in fileNames)
                    {
                        reader = new PdfReader(file);
                        pdf.AddDocument(reader);
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch (Exception)
                {
                    merged = false;
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                finally
                {
                    if (document != null)
                    {
                        document.Close();
                        document.Dispose();
                    }
                }
                pdf.Close();
                pdf.Dispose();

            }
            return merged;
        }

        public static string ReadPdfContent(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);

                }
                pdfReader.Close();
            }
            return text.ToString();
        }

        public static void AddTitleToPDF(string inputFile, string outputFile, string title)
        {
            Dictionary<string, string> attr = new Dictionary<string, string>();
            attr.Add("Title", title);
            AddAttrToPDF(inputFile, outputFile, attr);
            attr.Clear();
            attr = null;
        }

        public static void AddKeyWordsToPDF(string inputFile, string outputFile, string Keywords)
        {
            Dictionary<string, string> attr = new Dictionary<string, string>();
            attr.Add("Keywords", Keywords);
            AddAttrToPDF(inputFile, outputFile, attr);
            attr.Clear();
            attr = null;
        }

        public static void AddAttrToPDF(string inputFile, string outputFile, Dictionary<string, string> attr)
        {
           

            if (attr.Count > 0)
            {

                PdfReader reader = new PdfReader(inputFile);
                using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (PdfStamper stamper = new PdfStamper(reader, fs))
                    {
                        Dictionary<String, String> info = reader.Info;

                        foreach (KeyValuePair<string, string> kvp in attr)
                        {

                            if (kvp.Key == "Keywords")
                            {
                                info[kvp.Key] = kvp.Value;
                            }
                            else if (!info.ContainsKey(kvp.Key))
                            {
                                info.Add(kvp.Key, kvp.Value);
                            }
                            else
                            {
                                info[kvp.Key] += " " + kvp.Value;
                            }
                        }
                        stamper.MoreInfo = info;
                        stamper.Close();
                    }
                }
                reader.Close();
            }

        }

        public static void AddWaterMark(string toFile, string fromFile, string waterMarkText)
        {


            // Creating watermark on a separate layer
            // Creating iTextSharp.text.pdf.PdfReader object to read the Existing PDF Document
            PdfReader reader1 = new PdfReader(fromFile);
            using (FileStream fs = new FileStream(toFile, FileMode.Create, FileAccess.Write, FileShare.None))
            // Creating iTextSharp.text.pdf.PdfStamper object to write Data from iTextSharp.text.pdf.PdfReader object to FileStream object
            using (PdfStamper stamper = new PdfStamper(reader1, fs))
            {
                // Getting total number of pages of the Existing Document
                int pageCount = reader1.NumberOfPages;

                // Create New Layer for Watermark
                PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                // Loop through each Page
                for (int i = 1; i <= pageCount; i++)
                {
                    // Getting the Page Size
                    Rectangle rect = reader1.GetPageSize(i);

                    // Get the ContentByte object
                    PdfContentByte cb = stamper.GetOverContent(i);

                    // Tell the cb that the next commands should be "bound" to this new layer
                    cb.BeginLayer(layer);


                    //cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 50);

                    cb.SetFontAndSize(BaseFont.CreateFont(
                      BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 60);

                    PdfGState gState = new PdfGState();
                    gState.FillOpacity = 0.25f;
                    cb.SetGState(gState);

                    cb.SetColorFill(BaseColor.RED);
                    cb.BeginText();
                    cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, waterMarkText, rect.Width / 2, rect.Height/2, 45f);
                    cb.EndText();


                    // Close the layer
                    cb.EndLayer();
                }

                stamper.FormFlattening = true;


            }
            reader1.Close();
            reader1.Dispose();


        }


 

    }
 