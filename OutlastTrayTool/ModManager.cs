using Newtonsoft.Json.Linq;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlastTrayTool
{
    public class ModManager
    {
        private Config _userConfig;
        private string _gameModFolderPath;
        private string _toolModFolderPath;
        private FileSystemWatcher? _downloadWatcher;
        private Action? _refreshUi;
        public List<string> enabledMods;
        public List<string> disabledMods;
        public ModManager(Config userConfig) { 
            _userConfig = userConfig;
            // was getting issues passing through _userConfig.LoadConfig()["gamePath"] into Path.Combine, not sure why
            // so I create a temp string here
            string initialGamePath = _userConfig.LoadConfig()["gamePath"];
            _gameModFolderPath = Path.Combine(initialGamePath, "OPP/Content/Paks");
            _toolModFolderPath = _userConfig.modFolderPath;
        }

        public void SetRefreshUiAction(Action refreshUi)
        {
            _refreshUi = refreshUi;
        }

        private void RefreshUi()
        {
            _refreshUi?.Invoke();
        }

        private bool IsFileInModMap(JObject modMap, string fileName)
        {
            foreach (JProperty modEntry in modMap.Properties())
            {
                if (modEntry.Value is not JObject modObj)
                    continue;

                if (modObj["files"] is not JArray files)
                    continue;

                foreach (JToken file in files)
                {
                    if (file.ToString() == fileName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public void RefreshMods()
        {

            if (!Directory.Exists(_userConfig.modFolderPath))
            {
                Directory.CreateDirectory(_userConfig.modFolderPath);
            }

            enabledMods = Directory.GetFiles(_gameModFolderPath, "*_P.pak")
                .Select(Path.GetFileName)
                .Where(fileName => fileName != null)
                .ToList();

            disabledMods = Directory.GetFiles(_userConfig.modFolderPath, "*_P.pak")
                .Select(Path.GetFileName)
                .Where(fileName => fileName != null)
                .ToList();

            JObject config = JObject.Parse(File.ReadAllText(_userConfig.configPath));

            if (config["modMap"] == null || config["modMap"] is not JObject modMap)
            {
                modMap = new JObject();
                config["modMap"] = modMap;
            }

            if (modMap["undefined"] == null || modMap["undefined"] is not JObject undefinedObj)
            {
                undefinedObj = new JObject
                {
                    ["name"] = "Undefined",
                    ["files"] = new JArray()
                };

                modMap["undefined"] = undefinedObj;
            }

            if (undefinedObj["files"] == null || undefinedObj["files"] is not JArray undefinedFiles)
            {
                undefinedFiles = new JArray();
                undefinedObj["files"] = undefinedFiles;
            }

            List<string> allDetectedMods = new List<string>();
            allDetectedMods.AddRange(enabledMods);
            allDetectedMods.AddRange(disabledMods);

            foreach (string fileName in allDetectedMods)
            {
                if (!IsFileInModMap(modMap, fileName))
                {
                    undefinedFiles.Add(fileName);
                }
            }

            _userConfig.ChangeProperty("modMap", modMap);
        }

        public void RegisterMod(string modId, string modName, string fileName, string modVersion)
        {

            dynamic config = _userConfig.LoadConfig();
            JObject modMap = config.modMap;

            if (modMap[modId] == null)
            {
                modMap[modId] = new JObject();
                modMap[modId]["name"] = modName;
                modMap[modId]["version"] = modVersion;
                modMap[modId]["update"] = false;
                modMap[modId]["files"] = new JArray();
            }

            JArray files = (JArray)modMap[modId]["files"];
            files.Add(fileName);
            _userConfig.ChangeProperty("modMap", modMap);
        }
        public void AddMod(string fileName)
        {
            AddModInternal(fileName, "undefined", "Undefined", "Undefined");
        }

        public async Task AddMod(string fileName, string modId)
        {
            dynamic json = await ModManagerAPI.GetModName(Convert.ToInt32(modId));
            string modName = json.data.mod.name;
            dynamic json2 = await ModManagerAPI.GetModVersion(Convert.ToInt32(modId));
            string modVersion = json2.data.mod.version;

            Debug.WriteLine(modName);

            AddModInternal(fileName, modId, modName, modVersion);
        }

        private void AddModInternal(string fileName, string modId, string modName, string modVersion)
        {
            string fileType = Path.GetExtension(fileName).ToLower();

            switch (fileType)
            {
                case ".pak":
                    {
                        string pakName = Path.GetFileName(fileName);
                        string outputPath = Path.Combine(_gameModFolderPath, pakName);

                        File.Copy(fileName, outputPath, true);

                        RegisterMod(modId, modName, pakName, modVersion);
                        RefreshMods();
                        RefreshUi();
                        Debug.WriteLine($"Installed pak: {pakName}");
                        return;
                    }

                case ".zip":
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(fileName))
                        {
                            foreach (ZipArchiveEntry file in archive.Entries)
                            {
                                if (Path.GetExtension(file.FullName).ToLower() == ".pak")
                                {
                                    string pakName = Path.GetFileName(file.FullName);
                                    string outputPath = Path.Combine(_gameModFolderPath, pakName);

                                    file.ExtractToFile(outputPath, true);

                                    RegisterMod(modId, modName, pakName, modVersion);
                                    RefreshMods();
                                    RefreshUi();
                                    Debug.WriteLine($"Extracted pak from zip: {pakName}");
                                }
                            }
                        }

                        return;
                    }

                case ".rar":
                case ".7z":
                    {
                        using (var archive = ArchiveFactory.OpenArchive(fileName))
                        {
                            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                            {
                                if (Path.GetExtension(entry.Key).ToLower() == ".pak")
                                {
                                    string pakName = Path.GetFileName(entry.Key);
                                    string outputPath = Path.Combine(_gameModFolderPath, pakName);

                                    entry.WriteToFile(outputPath, new SharpCompress.Common.ExtractionOptions
                                    {
                                        ExtractFullPath = false,
                                        Overwrite = true
                                    });

                                    RegisterMod(modId, modName, pakName, modVersion);
                                    RefreshMods();
                                    RefreshUi();
                                    Debug.WriteLine($"Extracted pak from archive: {pakName}");
                                }
                            }
                        }

                        return;
                    }

                default:
                    MessageBox.Show("Not a .pak, .zip, .rar, or .7z file.");
                    return;
            }
        }

        public void EnableMod(string fileName)
        {
            File.Move(Path.Combine(_toolModFolderPath, fileName), Path.Combine(_gameModFolderPath, Path.GetFileName(fileName)));
        }

        public void DisableMod(string fileName)
        {
            File.Move(Path.Combine(_gameModFolderPath, fileName), Path.Combine(_toolModFolderPath, Path.GetFileName(fileName)));

        }


        public void StartDownloadWatcher()
        {
            string downloadsPath = _userConfig.LoadConfig()["downloadsPath"].ToString();
            Debug.WriteLine(downloadsPath);
            _downloadWatcher = new FileSystemWatcher(downloadsPath);

            _downloadWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            _downloadWatcher.Changed += OnChanged;
            _downloadWatcher.Created += OnCreated;
            _downloadWatcher.Deleted += OnDeleted;
            _downloadWatcher.Renamed += OnRenamed;
            _downloadWatcher.Error += OnError;

            _downloadWatcher.Filters.Add("*.pak");
            _downloadWatcher.Filters.Add("*.rar");
            _downloadWatcher.Filters.Add("*.zip");

            _downloadWatcher.EnableRaisingEvents = true;

            Debug.WriteLine("Watcher started. Monitoring for .pak, .rar, and .zip files.");
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            try
            {
                string zoneIdentifierPath = $"{e.FullPath}:Zone.Identifier";


                string[] text = File.ReadAllLines(zoneIdentifierPath);

                foreach (string line in text)
                {
                    if (line.Contains("/5376/"))
                    {
                        int start = line.IndexOf("5376/") + 5;
                        string modId = line[start..].Split('/')[0];

                        _ = AddMod(e.FullPath, modId);
                        return;
                    }
                }
            }
            catch (Exception ex) when (
                ex is FileNotFoundException ||
                ex is DirectoryNotFoundException ||
                ex is IOException ||
                ex is UnauthorizedAccessException
            )
            {
                Debug.WriteLine($"Could not read download metadata for {e.FullPath}: {ex.Message}");
            }
        }


        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine($"Created: {e.FullPath}");
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Debug.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Debug.WriteLine("Renamed:");
            Debug.WriteLine($"  Old: {e.OldFullPath}");
            Debug.WriteLine($"  New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            Debug.WriteLine(e.GetException());


    }
}
