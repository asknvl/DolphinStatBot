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
        XImage image;
        string sign;
        public PdfCreator(string sign)
        {
            //GlobalFontSettings.FontResolver = new FontResolver();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            image = XImage.FromFile("pdf/Images/xtime_trim.png"); image = XImage.FromFile("pdf/Images/xtime_trim.png");
            this.sign = sign;
        }

        public MemoryStream GetPdf(string date, List<User> users, Dictionary<uint, Statistics> userstat, Dictionary<string,Dictionary<uint, Statistics>> tagstat )
        {
            string res = "";

            var document = new PdfDocument();
            var page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A5;

            var gfx = XGraphics.FromPdfPage(page);

            XStringFormat formatHeader = new XStringFormat();
            formatHeader.LineAlignment = XLineAlignment.Near;
            formatHeader.Alignment = XStringAlignment.Near;
            var fontHeader = new XFont("Consolas", 14, XFontStyle.Bold);

            XStringFormat format = new XStringFormat();
            format.LineAlignment = XLineAlignment.Near;
            format.Alignment = XStringAlignment.Near;
            var font = new XFont("Consolas", 14, XFontStyle.Regular);
            var fontSmal = new XFont("Consolas", 12, XFontStyle.Regular);
            var fontMicro = new XFont("Consolas", 10, XFontStyle.Regular);

            int marginLeft = 30;
            int marginTop = 100;
            int columnDistance = 100;
            int col = 0;

            gfx.DrawImage(image, 10, 10, 100, 56);

            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            gfx.DrawString($"Отчет за:".PadLeft(12, ' '), fontSmal, XBrushes.Black, new XRect(195, 7, page.Width, page.Height), format);
            gfx.DrawString($"{date}", fontSmal, XBrushes.Black, new XRect(280, 7, page.Width, page.Height), format);
            gfx.DrawString($"Сформирован:".PadLeft(12, ' '), fontSmal, XBrushes.Black, new XRect(195, 27, page.Width, page.Height), format);
            gfx.DrawString($"{dateTime}", fontSmal, XBrushes.Black, new XRect(280, 27, page.Width, page.Height), format);
            gfx.DrawString("Валюта:".PadLeft(12, ' '), fontSmal, XBrushes.Black, new XRect(195, 47, page.Width, page.Height), format);
            gfx.DrawString("USD", fontSmal, XBrushes.Black, new XRect(280, 47, page.Width, page.Height), format);

            gfx.DrawString("Имя", fontHeader, XBrushes.Black, new XRect(marginLeft + col, marginTop, page.Width, page.Height), format);
            gfx.DrawString("Расход".PadLeft(8, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), marginTop, page.Width, page.Height), format);
            gfx.DrawString("Лиды".PadLeft(8, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), marginTop, page.Width, page.Height), format);
            gfx.DrawString("CPA".PadLeft(8, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), marginTop, page.Width, page.Height), format);

            int i = 0;
            int top = 0;

            foreach (var item in userstat)
            {
                if (item.Key == 0xFF)
                    continue;
                var user = users.FirstOrDefault(o => o.id == item.Key);
                string userName = !user.display_name.Equals("") ? user.display_name : user.login;

                col = 0;
                top = 10 + marginTop + (i + 1) * 20;
                gfx.DrawString(userName, font, XBrushes.Black, new XRect(marginLeft + col, top, page.Width, page.Height), format);
                gfx.DrawString($"{item.Value.spend:0.00}".PadLeft(8, ' '), font, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
                gfx.DrawString($"{item.Value.results}".PadLeft(8, ' '), font, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
                gfx.DrawString($"{item.Value.cpa:0.00}".PadLeft(8, ' '), font, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
                i++;

            }

            XPen lineRed = new XPen(XColors.Black, 1);
            top += 30;
            gfx.DrawLine(lineRed, 20, top , page.Width - 20, top);

            var total = userstat.FirstOrDefault(o => o.Key == 0xFF).Value;

            top += 10;
            col = 0;
            gfx.DrawString("Итог:".PadLeft(5, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + col, top, page.Width, page.Height), format);
            gfx.DrawString($"{total.spend:0.00}".PadLeft(8, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
            gfx.DrawString($"{total.results}".PadLeft(8, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
            gfx.DrawString($"{total.cpa:0.00}".PadLeft(8, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);

            top += 10;
            foreach (var item in tagstat)
            {
                top += 20;
                col = 0;

                var totalAcc = item.Value.FirstOrDefault(o => o.Key == 0xFF).Value;
                gfx.DrawString($"{item.Key}:".PadLeft(5, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + col, top, page.Width, page.Height), format);
                gfx.DrawString($"{totalAcc.spend:0.00}".PadLeft(8, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
                gfx.DrawString($"{totalAcc.results}".PadLeft(8, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
                gfx.DrawString($"{totalAcc.cpa:0.00}".PadLeft(8, ' '), fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
            }

            gfx.DrawString(sign, fontMicro, XBrushes.Gray, new XRect(page.Width - 130, page.Height - 20, page.Width, page.Height), format);

            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            return stream;

            //return res;
        }

        public string GetAndSavePdf(List<User> users, Dictionary<uint, Statistics> statistics)
        {
            string res = "";

            var document = new PdfDocument();
            var page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A5;

            var gfx = XGraphics.FromPdfPage(page);

            XStringFormat formatHeader = new XStringFormat();
            formatHeader.LineAlignment = XLineAlignment.Near;
            formatHeader.Alignment = XStringAlignment.Near;
            var fontHeader = new XFont("Roboto", 14, XFontStyle.Bold);

            XStringFormat format = new XStringFormat();
            format.LineAlignment = XLineAlignment.Near;
            format.Alignment = XStringAlignment.Near;
            var font = new XFont("Roboto", 14, XFontStyle.Regular);

            int marginLeft = 25;
            int marginTop = 100;
            int columnDistance = 100;
            int col = 0;

            //XImage image = XImage.FromFile("pdf/Images/xtime_trim.png");
            gfx.DrawImage(image, 10, 10, 100, 56);

            string dateTime = DateTime.Now.ToString("dd.MM.yyyy-HH:mm:ss");
            gfx.DrawString(dateTime, font, XBrushes.Black, new XRect(270, 7, page.Width, page.Height), format);
            gfx.DrawString("Валюта: USD", font, XBrushes.Black, new XRect(270, 27, page.Width, page.Height), format);

            gfx.DrawString("Имя", fontHeader, XBrushes.Black, new XRect(marginLeft + col, marginTop, page.Width, page.Height), format);
            gfx.DrawString("Расход", fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), marginTop, page.Width, page.Height), format);
            gfx.DrawString("Лиды", fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), marginTop, page.Width, page.Height), format);
            gfx.DrawString("Лиды, CPA", fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), marginTop, page.Width, page.Height), format);

            int i = 0;
            int top = 0;

            foreach (var item in statistics)
            {
                if (item.Key == 0xFF)
                    continue;
                var user = users.FirstOrDefault(o => o.id == item.Key);
                string userName = !user.display_name.Equals("") ? user.display_name : user.login;

                col = 0;
                top = 10 + marginTop + (i + 1) * 20;
                gfx.DrawString(userName, font, XBrushes.Black, new XRect(marginLeft + col, top, page.Width, page.Height), format);
                gfx.DrawString($"{item.Value.spend}", font, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
                gfx.DrawString($"{item.Value.results}", font, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
                gfx.DrawString($"{item.Value.cpa}", font, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
                i++;

            }

            XPen lineRed = new XPen(XColors.Black, 1);
            top += 30;
            gfx.DrawLine(lineRed, 20, top, page.Width - 20, top);

            var total = statistics.FirstOrDefault(o => o.Key == 0xFF).Value;

            top += 10;
            col = 0;
            gfx.DrawString("Итог:", fontHeader, XBrushes.Black, new XRect(marginLeft + col, top, page.Width, page.Height), format);
            gfx.DrawString($"{total.spend}", fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
            gfx.DrawString($"{total.results}", fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);
            gfx.DrawString($"{total.cpa}", fontHeader, XBrushes.Black, new XRect(marginLeft + (col += columnDistance), top, page.Width, page.Height), format);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "PDFS");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            res = Path.Combine(path, $"stat_{dateTime.Replace(":", " ")}.pdf");
            document.Save(res);
            
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

