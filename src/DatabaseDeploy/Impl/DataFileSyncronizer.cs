using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DatabaseDeploy.Impl
{
    public class DataFileSyncronizer
    {
        public bool RecreateDb = Convert.ToBoolean(ConfigurationManager.AppSettings["RecreateDB"]);
        private readonly DataExecutor _dataExecutor;

        private const string SchemaFolder = "Schema";
        private const string DataFolder = "Data";
        private const string ModificationsFolder = "Modifications";

        public DataFileSyncronizer()
        {
            _dataExecutor = new DataExecutor();
        }

        public void StartSync()
        {
            Console.ForegroundColor = ConsoleColor.White;
            if (!_dataExecutor.DatabaseExists() || RecreateDb)
            {
                CreateDatabase();
            }
            else
            {
                _dataExecutor.SetDatabaseBlueJay();

                if (!_dataExecutor.TrackingTableExists())
                {
                    Logger.LogInfo("Database is missing Tracking Information for Incremental Upgrade.");

                    while (true)
                    {
                        Console.WriteLine("Enter (Y) to wipe out & create new schema, OR (N) to exit.");
                        var readKey = Console.ReadKey();
                        Console.WriteLine("\n");

                        if (readKey.Key == ConsoleKey.Y)
                        {
                            CreateDatabase();
                            break;
                        }
                        if (readKey.Key == ConsoleKey.N)
                        {
                            break;
                        }

                        Console.WriteLine("Invalid Option\n\n\n");
                    }
                }
                else
                {
                    Logger.LogInfo("Database Found!");
                    StartSync(ModificationsFolder, _dataExecutor.GetLastExecutedScriptfortheFolder(ModificationsFolder));
                }
            }
        }

        private void CreateDatabase()
        {
            Logger.LogInfo("Creating Database!");

            bool success = StartSync(SchemaFolder);
            if (!success) return;

            _dataExecutor.SetDatabaseBlueJay();

            StartSync(DataFolder);
            StartSync(ModificationsFolder);
        }

        private bool StartSync(string folderName, string filePoint = "")
        {
            var directoryInfo = new DirectoryInfo(folderName);
            if(directoryInfo.Exists == false)
            {
                Logger.LogInfo("Does not exists - " + folderName);
                return false;
            }

            var filesToCheck = directoryInfo.GetFiles("*.sql").OrderBy(x => x.Name).ToArray();

            if (!string.IsNullOrEmpty(filePoint) && filesToCheck.Any(x => x.Name.ToLower().Equals(filePoint.ToLower())))
            {
                filesToCheck = filesToCheck.SkipWhile(x => x.Name != filePoint).ToArray();
                filesToCheck = filesToCheck.Except(new[] { filesToCheck.First() }).ToArray();
            }

            Logger.LogInfo("Starting Folder " + folderName);

            string lastFileExecuted = "";
            bool success = true;
            foreach (FileInfo fileInfo in filesToCheck)
            {
                Logger.LogInfo("Executing " + fileInfo.Name);
                try
                {
                    _dataExecutor.ExecuteFile(fileInfo.FullName);
                }
                catch (Exception ex)
                {
                    success = false;
                    Logger.LogError("EXCEPTION: While executing script. Please check the log for details.", ex);
                    break;
                }

                lastFileExecuted = fileInfo.Name;
            }

            if (!string.IsNullOrEmpty(lastFileExecuted))
                _dataExecutor.UpdateTablewithLastPoint(folderName, lastFileExecuted);

            return success;
        }

    }
}
