using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        string inputPath = @"D:\KHANHNGUYEN\251111_MORAY-COLUMN TRANSITION DETAILS SHEET 3.1.pdf";
        string outputPath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(inputPath)!,
            System.IO.Path.GetFileNameWithoutExtension(inputPath) + "_API.pdf"
        );

        try
        {
            Console.WriteLine("📄 Đang đọc PDF...");
            Console.WriteLine($"Input: {inputPath}");

            if (!File.Exists(inputPath))
            {
                Console.WriteLine("❌ File không tồn tại!");
                return;
            }

            ProcessPdfWithBlueFilter(inputPath, outputPath);

            Console.WriteLine($"✅ Hoàn thành!");
            Console.WriteLine($"Output: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
        }

        Console.WriteLine("\nPress Enter to exit...");
        Console.ReadLine();
    }

    static void ProcessPdfWithBlueFilter(string inputPath, string outputPath)
    {
        using var reader = new PdfReader(inputPath);
        reader.SetUnethicalReading(true);

        using var writer = new PdfWriter(outputPath);
        using var pdfDoc = new PdfDocument(reader, writer);

        Console.WriteLine($"📊 Số trang: {pdfDoc.GetNumberOfPages()}");

        for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
        {
            Console.WriteLine($"  ⏳ Đang xử lý trang {pageNum}...");

            try
            {
                var page = pdfDoc.GetPage(pageNum);
                FilterBlueContentFromPage(page);
                Console.WriteLine($"  ✓ Trang {pageNum} xong");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠ Lỗi trang {pageNum}: {ex.Message}");
            }
        }
    }

    static void FilterBlueContentFromPage(PdfPage page)
    {
        var pageDict = page.GetPdfObject();
        var contents = pageDict.Get(PdfName.Contents);

        if (contents == null)
        {
            Console.WriteLine("    ℹ Không có content stream");
            return;
        }

        var streams = GetContentStreams(contents);
        int modifiedCount = 0;

        foreach (var stream in streams)
        {
            try
            {
                byte[] contentBytes = stream.GetBytes();
                string content = Encoding.GetEncoding("ISO-8859-1").GetString(contentBytes);
                string originalContent = content;

                // Xử lý và lọc bỏ các đối tượng màu xanh dương
                content = FilterBlueObjects(content);

                if (content != originalContent)
                {
                    modifiedCount++;
                    byte[] newBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(content);
                    stream.SetData(newBytes);
                    stream.SetModified();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ⚠ Stream error: {ex.Message}");
            }
        }

        // Xử lý XObjects (Form XObjects có thể chứa graphics)
        ProcessXObjects(page);

        if (modifiedCount > 0)
        {
            Console.WriteLine($"    ✓ Đã sửa {modifiedCount} content stream(s)");
        }
        else
        {
            Console.WriteLine($"    ℹ Không phát hiện đối tượng xanh dương");
        }
    }

    static List<PdfStream> GetContentStreams(PdfObject contents)
    {
        var streams = new List<PdfStream>();

        if (contents.IsStream())
        {
            streams.Add((PdfStream)contents);
        }
        else if (contents.IsArray())
        {
            var array = (PdfArray)contents;
            for (int i = 0; i < array.Size(); i++)
            {
                var obj = array.Get(i);
                if (obj?.IsIndirectReference() == true)
                {
                    var direct = ((PdfIndirectReference)obj).GetRefersTo();
                    if (direct?.IsStream() == true)
                        streams.Add((PdfStream)direct);
                }
                else if (obj?.IsStream() == true)
                {
                    streams.Add((PdfStream)obj);
                }
            }
        }

        return streams;
    }

    static string FilterBlueObjects(string content)
    {
        // Pattern để tìm và thay thế màu xanh dương RGB
        // Xanh dương thuần: R=0, G=0, B=1 hoặc các biến thể gần đó

        // RGB patterns - rg (non-stroking/fill), RG (stroking)
        // Xanh dương thuần: 0 0 1
        content = ReplaceBlueRgbPattern(content, "rg");
        content = ReplaceBlueRgbPattern(content, "RG");

        // CMYK patterns - k (non-stroking), K (stroking)
        // Xanh dương CMYK: C=1, M=0.5-1, Y=0, K=0
        content = ReplaceBlueCmykPattern(content, "k");
        content = ReplaceBlueCmykPattern(content, "K");

        // Grayscale patterns - g (non-stroking), G (stroking) 
        // Không cần xử lý vì grayscale không có xanh

        return content;
    }

    static string ReplaceBlueRgbPattern(string content, string op)
    {
        // Pattern: số số số rg/RG
        // Tìm các pattern có R thấp, G thấp, B cao (xanh dương)
        string pattern = @"([\d.]+)\s+([\d.]+)\s+([\d.]+)\s+" + op + @"(?=\s|$)";

        return Regex.Replace(content, pattern, match =>
        {
            if (double.TryParse(match.Groups[1].Value, out double r) &&
                double.TryParse(match.Groups[2].Value, out double g) &&
                double.TryParse(match.Groups[3].Value, out double b))
            {
                // Kiểm tra xem có phải màu xanh dương không
                // Xanh dương: B > 0.5 và B > R và B > G
                if (IsBlueColor(r, g, b))
                {
                    // Thay bằng màu trắng (ẩn đối tượng)
                    return $"1 1 1 {op}";
                }
            }
            return match.Value;
        });
    }

    static string ReplaceBlueCmykPattern(string content, string op)
    {
        // Pattern: số số số số k/K (C M Y K)
        string pattern = @"([\d.]+)\s+([\d.]+)\s+([\d.]+)\s+([\d.]+)\s+" + op + @"(?=\s|$)";

        return Regex.Replace(content, pattern, match =>
        {
            if (double.TryParse(match.Groups[1].Value, out double c) &&
                double.TryParse(match.Groups[2].Value, out double m) &&
                double.TryParse(match.Groups[3].Value, out double y) &&
                double.TryParse(match.Groups[4].Value, out double k))
            {
                // CMYK Blue: Cyan cao, Magenta trung bình-cao, Yellow thấp
                // Ví dụ: 1 1 0 0 = xanh dương thuần
                //        1 0.5 0 0 = xanh dương nhạt
                if (IsBlueCmyk(c, m, y, k))
                {
                    // Thay bằng màu trắng CMYK
                    return $"0 0 0 0 {op}";
                }
            }
            return match.Value;
        });
    }

    static bool IsBlueColor(double r, double g, double b)
    {
        // Định nghĩa màu xanh dương:
        // - Blue component cao (> 0.5)
        // - Blue > Red và Blue > Green
        // - Red và Green tương đối thấp

        // Xanh dương thuần túy
        if (b >= 0.8 && r <= 0.3 && g <= 0.3)
            return true;

        // Xanh dương trung bình
        if (b >= 0.6 && b > r && b > g && r <= 0.4 && g <= 0.5)
            return true;

        // Xanh navy/tối
        if (b >= 0.4 && r <= 0.2 && g <= 0.2 && b > r * 2 && b > g * 2)
            return true;

        // Xanh cyan (xanh lơ)
        if (b >= 0.7 && g >= 0.7 && r <= 0.3)
            return true;

        return false;
    }

    static bool IsBlueCmyk(double c, double m, double y, double k)
    {
        // CMYK Blue: Cyan cao, có thể có Magenta, Yellow thấp
        // Cyan = 1, Magenta = 0-1, Yellow = 0, K = 0 => Blue

        if (c >= 0.7 && y <= 0.3 && k <= 0.3)
        {
            // Xanh thuần với magenta
            if (m >= 0.5)
                return true;

            // Cyan thuần (xanh lơ)
            if (c >= 0.9 && m <= 0.3)
                return true;
        }

        return false;
    }

    static void ProcessXObjects(PdfPage page)
    {
        try
        {
            var resources = page.GetResources();
            if (resources == null) return;

            var xObjects = resources.GetResource(PdfName.XObject);
            if (xObjects == null) return;

            foreach (var name in xObjects.KeySet())
            {
                try
                {
                    var xObj = xObjects.GetAsStream(name);
                    if (xObj == null) continue;

                    var subtype = xObj.GetAsName(PdfName.Subtype);
                    if (PdfName.Form.Equals(subtype))
                    {
                        // Form XObject - xử lý nội dung bên trong
                        ProcessFormXObject(xObj);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"    ⚠ XObject error ({name}): {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    ⚠ XObjects processing error: {ex.Message}");
        }
    }

    static void ProcessFormXObject(PdfStream formXObj)
    {
        try
        {
            byte[] contentBytes = formXObj.GetBytes();
            string content = Encoding.GetEncoding("ISO-8859-1").GetString(contentBytes);
            string originalContent = content;

            content = FilterBlueObjects(content);

            if (content != originalContent)
            {
                byte[] newBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(content);
                formXObj.SetData(newBytes);
                formXObj.SetModified();
                Console.WriteLine($"    ✓ Đã sửa Form XObject");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    ⚠ Form XObject error: {ex.Message}");
        }
    }
}


