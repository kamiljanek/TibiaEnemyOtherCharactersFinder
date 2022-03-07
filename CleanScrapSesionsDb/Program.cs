using System;
using System.Text;
using EnemyCharsFinder;
using EnemyCharsFinder.Data;
using EnemyCharsFinder.Models;

namespace CleanScrapSesionsDb
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cleanScrapSesion = new CleanScrapSesion();
            cleanScrapSesion.Run();
        }
    }
}
