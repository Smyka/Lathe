using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace OutlastTrayTool
{
    public class GameManager
    {
        private Config _userConfig;
        private string _ReshadeInstallFolderPath;
        public GameManager(Config userConfig) {
            _userConfig = userConfig;
            string initialGamePath = _userConfig.LoadConfig()["gamePath"];
            _ReshadeInstallFolderPath = Path.Combine(initialGamePath, "OPP/Binaries/Win64");
        }
        private string _GameIniConfigPath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "OPP/Saved/Config/WindowsClient/Game.ini");
        private string _EngineIniConfigPath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "OPP/Saved/Config/WindowsClient/Engine.ini");
        
        

        public void ChangeFOV(int userValue) { 
            if (userValue < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(userValue), "FOV must be greater than 0");
            }
            List<string> contents = File.ReadAllLines(_GameIniConfigPath).ToList();
            bool modified = false;
            for (int i = 0; i < contents.Count; i++)
            {

                if (contents[i].Contains("DefaultFOV="))
                {
                    contents[i] = $"DefaultFOV={userValue}.000000";
                    modified = true;
                }
                if (contents[i].Contains("AimingFOV="))
                {
                    contents[i] = $"AimingFOV={userValue}.000000";
                    modified = true;
                }
            }
            if (modified)
            {
                // change fov values
                File.WriteAllLines(_GameIniConfigPath, contents);
                _userConfig.ChangeProperty("FOV", userValue);
            }
            else
            {
                // if it can't find the settings in game.ini, create them
                contents.Add("[/script/opp.rbsettings_gameplay]");
                contents.Add($"DefaultFOV={userValue}.000000");
                contents.Add($"AimingFOV={userValue}.000000");
                File.WriteAllLines(_GameIniConfigPath, contents);
                _userConfig.ChangeProperty("FOV", userValue);
            }
            
        }

        public void ChangeScreenPercentage(int userValue)
        {
            if (userValue < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(userValue), "Screen percentage must be greater than 0");
            }
            List<string> contents = File.ReadAllLines(_EngineIniConfigPath).ToList();
            bool modified = false;
            for (int i = 0; i < contents.Count; i++)
            {

                if (contents[i].Contains("r.ScreenPercentage="))
                {
                    contents[i] = $"r.ScreenPercentage={userValue}";
                    modified = true;
                }
            }
            if (modified)
            {
                // change fov values
                File.WriteAllLines(_EngineIniConfigPath, contents);
                _userConfig.ChangeProperty("screenPercentage", userValue);
            }
            else
            {
                // if it can't find the settings in game.ini, create them
                contents.Add("[ConsoleVariables]");
                contents.Add($"r.ScreenPercentage={userValue}");
                File.WriteAllLines(_EngineIniConfigPath, contents);
                _userConfig.ChangeProperty("screenPercentage", userValue);
            }

        }

        public void ChangeFogToggle(string userValue)
        {
            string fogValue = "1";
            switch (userValue)
            {
                case "Disabled":
                    fogValue = "0";
                    break;
            }

                List<string> contents = File.ReadAllLines(_EngineIniConfigPath).ToList();
            bool modified = false;
            for (int i = 0; i < contents.Count; i++)
            {

                if (contents[i].Contains("r.Fog="))
                {
                    contents[i] = $"r.Fog={fogValue}";
                    modified = true;
                }
            }
            if (modified)
            {
                // change fov values
                File.WriteAllLines(_EngineIniConfigPath, contents);
                _userConfig.ChangeProperty("fog", userValue);
            }
            else
            {
                // if it can't find the settings in game.ini, create them
                contents.Add("[ConsoleVariables]");
                contents.Add($"r.Fog={fogValue}");
                File.WriteAllLines(_EngineIniConfigPath, contents);
                _userConfig.ChangeProperty("fog", userValue);
            }

        }

        public void InstallReshade()
        {
            string reshadeDll = Path.Combine(Environment.CurrentDirectory, "ReshadeAssets/dxgi.dll");
            string reshadeDllDest = Path.Combine(_ReshadeInstallFolderPath, "dxgi.dll");
            try { 
                File.Copy(reshadeDll, reshadeDllDest);
                MessageBox.Show("Reshade installed");
            }
            catch (IOException ex) {
                MessageBox.Show("Reshade already installed");
            }
        }

        public void UninstallReshade()
        {
            string reshadeDll = Path.Combine(Environment.CurrentDirectory, "ReshadeAssets/dxgi.dll");
            string reshadeDllDest = Path.Combine(_ReshadeInstallFolderPath, "dxgi.dll");
            try
            {
                File.Delete(reshadeDllDest);
                MessageBox.Show("Reshade uninstalled");
            }
            catch (IOException ex)
            {
                MessageBox.Show("Uninstall failed");
            }
        }



    }
}
