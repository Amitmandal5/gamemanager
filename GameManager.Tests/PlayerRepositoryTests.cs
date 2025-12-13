using GameManager.Services;

namespace GameManager.Tests
{
    public class PlayerRepositoryTests
    {
        private readonly string testFile;
        private readonly string logFile;

        public PlayerRepositoryTests()
        {
            string basePath = Path.Combine(Path.GetTempPath(), "GameManagerTests");
            Directory.CreateDirectory(basePath);

            testFile = Path.Combine(basePath, "players_test.json");
            logFile = Path.Combine(basePath, "log_test.txt");

            if (File.Exists(testFile))
                File.Delete(testFile);

            if (File.Exists(logFile))
                File.Delete(logFile);
        }

        private PlayerRepository CreateRepo()
        {
            var logger = new Logger(logFile);
            var repo = new PlayerRepository(logger, testFile);
            repo.LoadFromFile();
            return repo;
        }

        [Fact]
        public void AddPlayer_ShouldAddPlayerSuccessfully()
        {
            var repo = CreateRepo();

            repo.AddPlayer("testuser", 5, 200, "Red Team", 4.5);

            var players = repo.GetAllPlayers();
            Assert.Single(players);
        }

        [Fact]
        public void Search_ShouldReturnMatchingPlayer()
        {
            var repo = CreateRepo();
            repo.AddPlayer("alex", 3, 150, "Blue Team", 3.8);

            var result = repo.Search("alex");

            Assert.Single(result);
            Assert.Equal("alex", result[0].Username);
        }

        [Fact]
        public void UpdatePlayer_ShouldUpdatePlayerDetails()
        {
            var repo = CreateRepo();
            var player = repo.AddPlayer("john", 2, 100, "Alpha", 3.0);

            bool updated = repo.UpdatePlayer(
                player.Id,
                6,
                400,
                "Beta",
                4.9
            );

            var updatedPlayer = repo.GetById(player.Id);

            Assert.True(updated);
            Assert.NotNull(updatedPlayer);
            Assert.Equal(6, updatedPlayer!.HoursPlayed);
            Assert.Equal(400, updatedPlayer.HighScore);
            Assert.Equal("Beta", updatedPlayer.Team);
            Assert.Equal(4.9, updatedPlayer.Rating);
        }

        [Fact]
        public void SaveToFile_ShouldCreateJsonFile()
        {
            var repo = CreateRepo();
            repo.AddPlayer("saveuser", 1, 50, "SaveTeam", 2.5);

            repo.SaveToFile();

            Assert.True(File.Exists(testFile));
        }

        [Fact]
        public void LoadFromFile_ShouldLoadSavedPlayers()
        {
            var repo = CreateRepo();
            repo.AddPlayer("loaduser", 4, 250, "LoadTeam", 4.0);
            repo.SaveToFile();

            var newRepo = CreateRepo();
            var players = newRepo.GetAllPlayers();

            Assert.Single(players);
            Assert.Equal("loaduser", players[0].Username);
        }
    }
}
