using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Net;

namespace EnemyCharsFinder
{
    public class Program
    {
        static void Main(string[] args)
        {
            Decompressor decompressor = new Decompressor();
            decompressor.Decompress();
            decompressor.Servers(Link._mainUrl);

            Link link = new Link();
            link.SetLinks(decompressor.ServersList);


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var item in link.Links)
            {
                decompressor.Names(item);
            }

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

        }

    }

}
