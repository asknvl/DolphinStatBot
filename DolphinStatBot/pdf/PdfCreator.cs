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

        public string GetPdf(List<User> users, Dictionary<string, Statistics> statistics)
        {
            string res = "";

            GlobalFontSettings.FontResolver = new FontResolver();

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            XStringFormat formatHeader = new XStringFormat();
            formatHeader.LineAlignment = XLineAlignment.Near;
            formatHeader.Alignment = XStringAlignment.Near;
            var fontHeader = new XFont("Roboto", 12, XFontStyle.Bold);

            XStringFormat format = new XStringFormat();
            format.LineAlignment = XLineAlignment.Near;
            format.Alignment = XStringAlignment.Near;
            var font = new XFont("Roboto", 12, XFontStyle.Regular);

            int marginLeft = 20;
            int marginTop = 20;

            int columnDistance = 120;
            int col = 0;

            gfx.DrawString("Имя", fontHeader, XBrushes.Black, new XRect(marginLeft + col, marginTop, page.Width, page.Height), format);
            gfx.DrawString("Расход", fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), marginTop, page.Width, page.Height), format);
            gfx.DrawString("Лиды", fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), marginTop, page.Width, page.Height), format);
            gfx.DrawString("Лиды, CPA", fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), marginTop, page.Width, page.Height), format);

            //for (int i = 0; i < users.Count - 1; i++)
            int i = 0;
            foreach (var user in users)
            {
                if (user.id == )
                int top = marginTop + (i + 1) * 20;
                col = 0;

                //var user = users[i];
                string userName = !user.display_name.Equals("") ? user.display_name : user.login;

                string id = $"{user.id}";

                gfx.DrawString(userName, font, XBrushes.Black, new XRect(marginLeft + col, top, page.Width, page.Height), format);
                gfx.DrawString($"{statistics[id].spend}", font, XBrushes.Black, new XRect(marginLeft + (col+=columnDistance), top, page.Width, page.Height), format);
                gfx.DrawString($"{statistics[id].results}", font, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
                gfx.DrawString($"{statistics[id].cpa}", font, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);

                i++;
            }


            //gfx.DrawString("Hello World!", font, XBrushes.Black, new XRect(20, 20, page.Width, page.Height), XStringFormats.Center);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "PDFS");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = "test";            
            document.Save(Path.Combine(path, $"{fileName}.pdf"));

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

