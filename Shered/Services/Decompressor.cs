using HtmlAgilityPack;
using System.Net;

namespace Shered.Services
{
    public class Decompressor
    {
        public HtmlWeb web = new HtmlWeb();

        public void Decompress()
        {
            web.PreRequest += request =>
            {
                request.AutomaticDecompression = DecompressionMethods.All;
                return true;
            };

        }
    }
}
