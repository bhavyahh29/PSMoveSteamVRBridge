﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public class ConfigBase
    {
        protected string ConfigFileBase;

        public ConfigBase(string fnamebase)
        {
            ConfigFileBase = fnamebase;
        }

        private bool bIsDirty = false;
        public bool IsDirty
        {
            get { return bIsDirty; }
            set { bIsDirty = value; }
        }

        public void Save()
        {
            Save(FetchOrCreateConfigPath());
        }

        public void Save(string path)
        {
            if (IsDirty) {
                using (StreamWriter writer = File.CreateText(FetchOrCreateConfigPath())) {
                    JsonObject pt = new JsonObject();
                    WriteToJSON(pt);
                    pt.Save(writer);
                    IsDirty = false;
                }
            }
        }

        public bool Load()
        {
            return Load(FetchOrCreateConfigPath());
        }

        public bool Load(string path)
        {
            bool bLoadOk = false;

            using (StreamReader reader = File.OpenText(FetchOrCreateConfigPath())) {
                JsonValue pt = JsonValue.Load(reader);
                bLoadOk = ReadFromJSON(pt);
                if (bLoadOk) {
                    IsDirty = false;
                }
            }

            return bLoadOk;
        }

        public virtual void WriteToJSON(JsonValue pt)
        {
        }

        public virtual bool ReadFromJSON(JsonValue pt)
        {
            return true;
        }

        string FetchOrCreateConfigPath()
        {
            string HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string ConfigPath = Path.Combine(HomeDir, "PSMoveSteamVRBridge");

            if (!Directory.Exists(ConfigPath)) {
                try {
                    Directory.CreateDirectory(ConfigPath);
                }
                catch (IOException) {
                    Trace.TraceError(string.Format("Config::getConfigPath() - Failed to create config directory: {0}", ConfigPath));
                }
            }

            return Path.Combine(ConfigPath, ConfigFileBase + ".json");
        }
    }
}