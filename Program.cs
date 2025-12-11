using GameManager.Services;
using GameManager.ConsoleApp;

namespace GameManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // log.txt will be placed inside "Data" folder.
            Logger logger = new Logger("Data/log.txt");

            // players.json will also be stored in "Data" folder.
            PlayerRepository repo = new PlayerRepository(logger, "Data/players.json");

            // Load existing data (if file exists).
            repo.LoadFromFile();

            ConsoleMenu menu = new ConsoleMenu(repo);

            logger.Info("Application started.");

            menu.Show();

            // Save data when the user exits the menu.
            repo.SaveToFile();
            logger.Info("Application closed.");
        }
    }
}
