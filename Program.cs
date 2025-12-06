using GameManager.Services;
using GameManager.ConsoleApp;

namespace GameManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PlayerRepository repo = new PlayerRepository();
            ConsoleMenu menu = new ConsoleMenu(repo);

            menu.Show();
        }
    }
}
