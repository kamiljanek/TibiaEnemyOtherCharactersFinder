using HtmlAgilityPack;
using System.Net;

namespace TibiaCharacterFinderAPI.Entities
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
