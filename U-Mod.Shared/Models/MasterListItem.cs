using System;
using System.Collections.Generic;
using System.Linq;
using U_Mod.Shared.Enums;

namespace U_Mod.Shared.Models
{
    public enum ExtractLocation
    {
        Base,
        Data,
        DataTextures,
        DataMeshes,
        DataObsePlugins,
        DataMenus,
        DataFosePlugins,
    }

    public enum ZipFileType
    {
        Unknown,
        _7z,
        Zip,
        Rar,
        Exe
    }

    public class GameItem : ListItemBase
    {
        #region Public Properties

        public GamesEnum Game { get; set; }

        public string GameName { get; set; }

        public string GameVersion { get; set; } = "1";

        public List<Mod> Mods { get; set; } = new List<Mod>();

        #endregion Public Properties

        #region Public Constructors

        public GameItem()
        {
        }

        #endregion Public Constructors

        //Do not remove. Json deserializer requries parameterless constructor
    }

    public abstract class ListItemBase
    {
        #region Public Properties

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ParentId { get; set; }

        #endregion Public Properties
    }

    public class MasterList
    {
        #region Public Properties

        public List<GameItem> Games { get; set; } = new List<GameItem>();

        #endregion Public Properties

        #region Public Constructors

        public MasterList()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        //Do not remove. Json deserializer requries parameterless constructor
        public void RemoveItemWithId(Guid id)
        {
            foreach (var g in Games)
            {
                if (g.Id == id)
                {
                    Games.Remove(g);
                    return;
                }
                foreach (var m in g.Mods)
                {
                    if (m.Id == id)
                    {
                        g.Mods.Remove(m);
                        return;
                    }
                    foreach (var f in m.Files)
                    {
                        if (f.Id == id)
                        {
                            m.Files.Remove(f);
                            return;
                        }
                        foreach (var c in f.Content)
                        {
                            if (c.Id == id)
                            {
                                f.Content.Remove(c);
                                return;
                            }
                        }
                    }
                }
            }
        }

        #endregion Public Methods
    }

    public class Mod : ListItemBase
    {
        #region Public Properties

        public string CreatedBy { get; set; } = "";
        public string CreatorUrl { get; set; } = "";

        public List<ModZipFile> Files { get; set; } = new List<ModZipFile>();

        public int InstallOrder { get; set; } = 0;

        public InstallProfileEnum InstallProfile =>
            (IsSteam ? InstallProfileEnum.Steam : 0)
             | (IsNonSteam ? InstallProfileEnum.NonSteam : 0)
             | (IsAllDlcOnly ? InstallProfileEnum.AllDlc : 0)
             | (IsNotAllDlcOnly ? InstallProfileEnum.NoDlc : 0)
             | (NoSteamTags ? InstallProfileEnum.NoSteamTag : 0)
             | (NoDlcTags ? InstallProfileEnum.NoDlcTag : 0)
             | (NoTags ? InstallProfileEnum.NoData : 0);

        public bool IsAllDlcOnly { get; set; }

        public bool IsEssential { get; set; }

        public bool IsFullInstallOnly { get; set; }

        public bool IsNonSteam { get; set; }

        public bool IsNotAllDlcOnly { get; set; }

        public bool IsSteam { get; set; }

        public string ModName { get; set; }

        public int Version { get; set; } = 1;

        #endregion Public Properties

        #region Private Properties

        private bool NoDlcTags => !IsAllDlcOnly && !IsNotAllDlcOnly;

        private bool NoSteamTags => !IsSteam && !IsNonSteam;

        private bool NoTags => !IsEssential
                               && !IsSteam
                               && !IsNonSteam
                               && !IsFullInstallOnly
                               && !IsAllDlcOnly
                               && !IsNotAllDlcOnly;

        #endregion Private Properties

        #region Public Constructors

        public Mod()
        {
        } //Do not remove. Json deserializer requries parameterless constructor

        public Mod(Guid parentId)
        {
            ParentId = parentId;
        }

        #endregion Public Constructors

        public bool IsNotEssential => !IsEssential;
    }

    public class ModVersionInfo
    {
        #region Public Properties

        public Guid Id { get; set; }
        public int Version { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public ModVersionInfo()
        {
        }//Do not remove. Json deserializer requries parameterless constructor

        public ModVersionInfo(Mod m)
        {
            Id = m.Id;
            Version = m.Version;
        }

        #endregion Public Constructors
    }

    public class ModZipContent : ListItemBase
    {
        #region Public Properties

        public string FileName { get; set; }

        public int InstallOrder { get; set; } = 0;

        #endregion Public Properties

        #region Public Constructors

        public ModZipContent()
        {
        } //Do not remove. Json deserializer requries parameterless constructor

        public ModZipContent(Guid parentId)
        {
            ParentId = parentId;
        }

        #endregion Public Constructors
    }

    public class ModZipFile : ListItemBase
    {
        #region Public Properties

        public string AutoDownloadUrl
        {
            get
            {
                if (string.IsNullOrEmpty(ManualDownloadUrl))
                    return "";

                // Manual download is like so:
                // "https://www.nexusmods.com/oblivion/mods/13834?tab=files&file_id=1000019812",
                //  which maps to these variables like so:
                // "https://www.nexusmods.com/{game}/mods/{mod_id}?tab=files&file_id={id}",

                var splits = ManualDownloadUrl.Split('/');
                string game = splits[3];
                string modId = splits[5].Split('?').First();
                string fileId = splits[5].Split('=').Last();

                return $"https://api.nexusmods.com/v1/games/{game}/mods/{modId}/files/{fileId}/download_link.json";
            }
        }

        public List<ModZipContent> Content { get; set; } = new List<ModZipContent>();
        public string DirectDownloadUrl { get; set; }
        public ExtractLocation ExtractLocation { get; set; }
        public string ExtractLocationString(string gameName)
        {
            return ExtractLocation switch
            {
                ExtractLocation.Base => $"{gameName}",
                ExtractLocation.Data => $"{gameName}/Data",
                ExtractLocation.DataTextures => $"{gameName}/Data/Textures",
                ExtractLocation.DataMeshes => $"{gameName}/Data/Meshes",
                ExtractLocation.DataObsePlugins => $"{gameName}/Data/OBSE/Plugins",
                ExtractLocation.DataFosePlugins => $"{gameName}/Data/FOSE/Plugins",
                ExtractLocation.DataMenus => $"{gameName}/Data/Menu",
                _ => ""
            };
        }
        public string FileName { get; set; }
        public int InstallOrder { get; set; } = 0;
        public string ManualDownloadUrl { get; set; }
        public double SizeinKb { get; set; } = 0d;
        public ZipFileType ZipFileType { get; set; } = ZipFileType.Unknown;

        #endregion Public Properties

        #region Public Constructors

        public ModZipFile()
        {
        } //Do not remove. Json deserializer requries parameterless constructor

        public ModZipFile(Guid parentId)
        {
            ParentId = parentId;
        }

        #endregion Public Constructors
    }
}