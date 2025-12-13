using GameManager.Services;
using GameManager.ConsoleApp;

namespace GameManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Base folder where the .exe is running from
            string baseDir = AppContext.BaseDirectory;

            // Put both log and json inside a "Data" folder next to the exe
            string dataDir = Path.Combine(baseDir, "Data");
            Directory.CreateDirectory(dataDir);

            string logPath = Path.Combine(dataDir, "log.txt");
            string jsonPath = Path.Combine(dataDir, "players.json");

            // Create logger and repository using these absolute paths
            Logger logger = new Logger(logPath);
            PlayerRepository repo = new PlayerRepository(logger, jsonPath);

            // Load any existing players from file
            repo.LoadFromFile();

            ConsoleMenu menu = new ConsoleMenu(repo);
            logger.Info("Application started.");

            // Run main menu loop
            menu.Show();

            // Extra safety save when closing
            repo.SaveToFile();
            logger.Info("Application closed.");
        }
    }
}
