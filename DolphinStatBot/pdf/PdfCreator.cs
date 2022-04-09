using DolphinStatBot.Stat;
using DolphinStatBot.Users;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinStatBot.pdf
{
    public class PdfCreator
    {

        public string GetPdf(List<User> users, Directory<string, Statistics> statistics)
        {
            string res = "";

            GlobalFontSettings.FontResolver = new FontResolver();

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Roboto", 20, XFontStyle.Italic);

            gfx.DrawString("Hello World!", font, XBrushes.Black, new XRect(20, 20, page.Width, page.Height), XStringFormats.Center);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "PDFS");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            document.Save(Path.Combine(path, $"{name}.pdf"));

            return res;
        }

    }
}

public class FontResolver : IFontResolver
{
    string IFontResolver.DefaultFontName => throw new NotImplementedException();

    public byte[] GetFont(string faceName)
    {
        using (var ms = new MemoryStream())
        {
            using (var fs = File.Open(faceName, FileMode.Open))
            {
                fs.CopyTo(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }
    }
    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Roboto", StringComparison.CurrentCultureIgnoreCase))
        {
            if (isBold && isItalic)
            {
                return new FontResolverInfo("pdf/fonts/Roboto-BoldItalic.ttf");
            } else if (isBold)
            {
                return new FontResolverInfo("pdf/fonts/Roboto-Bold.ttf");
            } else if (isItalic)
            {
                return new FontResolverInfo("pdf/fonts/Roboto-Italic.ttf");
            } else
            {
                return new FontResolverInfo("pdf/fonts/Roboto-Regular.ttf");
            }
        }
        return null;
    }
}

