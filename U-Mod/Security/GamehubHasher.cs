using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using U_Mod.Helpers;
using U_Mod.Static;
using Ionic.Zip;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace U_Mod.Security
{
    public class U_ModHasher
    {
        private HashAlgorithm Sha { get; } = SHA256.Create();
        /// <summary>
        /// A random unguessable string appended to start of machineId hash, so any malicious users who somehow get as far as creating
        /// their own SHA256 from a machine ID will also need to know this!
        /// </summary>
        private readonly string MachineIdAppendage = "sdnsfds89fdsf0uhjds0";
        private string KeyFilePath { get; set; }
        private string Key { get; set; } 
        public static string ExpiryDateHash => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{DateTime.Now.Date:dd-MM-yyyy}:{SystemHelper.GetMachineId()}"));
        public string EncodedKey => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{SystemHelper.GetMachineId()}:{this.Key}"));
        public byte[] MachineIdHash => Sha.ComputeHash(Encoding.UTF8.GetBytes($"{MachineIdAppendage}{SystemHelper.GetMachineId()}"));
        /// <summary>
        /// Compares a given machineIdHash with the hash generated from the user's computer
        /// </summary>
        /// <param name="machineID"></param>
        /// <returns></returns>
        public bool CheckMachineId(byte[] machineIdHash)
        {
            return machineIdHash.SequenceEqual(MachineIdHash);
        }

        public U_ModHasher(string keyFilePath)
        {
            this.KeyFilePath = keyFilePath;
        }

        public U_ModHasher() { }
        public bool CheckSoftwareHash(string hash)
        {
            try
            {
                if (string.IsNullOrEmpty(hash))
                    return false;

                string decodedHash = Encoding.UTF8.GetString(Convert.FromBase64String(hash));

                var hashData = decodedHash.Split(':');

                if (hashData.Length != 2)
                    return false;

                if (!DateTime.TryParseExact(hashData[0], "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime expiryDate))
                    return false;

                if (expiryDate > DateTime.Now.AddDays(30))
                    return false;

                if (hashData[1] != SystemHelper.GetMachineId())
                    return false;

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool CheckDataKey(string dataKey)
        {
            if (string.IsNullOrEmpty(dataKey))
                return false;

            string decodedKey = Encoding.UTF8.GetString(Convert.FromBase64String(dataKey));
            
            //key of the form "GUID:key"

            if (!decodedKey.Contains(':'))
                return false;

            var data = decodedKey.Split(':');

            if (Helpers.SystemHelper.GetMachineId() != data[0])
            {
                
                GeneralHelpers.ShowMessageBox("STOLEN KEY.");
                return false;
            }

            if (HashOk(data[1]))
            {
                this.Key = data[1];
                return true;
            }

            return false;
        }

        public bool HashOk(string base64Hash)
        {
            return this.AllowedHashes().Any(h => h.SequenceEqual(Convert.FromBase64String(base64Hash)));
        }

        private List<byte[]> AllowedHashes()
        {
            var dateToday = DateTime.Now;

            return new List<byte[]>
            {
                Sha.ComputeHash(Encoding.UTF8.GetBytes($"U_Mod{dateToday.AddDays(- 1).DayOfYear}")),
                Sha.ComputeHash(Encoding.UTF8.GetBytes($"U_Mod{dateToday.DayOfYear}")),
                Sha.ComputeHash(Encoding.UTF8.GetBytes($"U_Mod{dateToday.AddDays(1).DayOfYear}")),
            };
        }

        public enum U_ModHasherResult
        {
            Unknown,
            Error,
            KeyFileNotFound,
            ZipFileNotFound,
            Ok,
            OutofDate,
            KeyEmpty
        }

        public bool TryCheckHash(out U_ModHasherResult res, out string message)
        {
            message = "";
            res = U_ModHasherResult.Unknown;

            string temp = "";
            try
            {
                if (File.Exists(KeyFilePath))
                {

                    using ZipFile archive = new ZipFile(KeyFilePath)
                    {
                        Password = Constants.AmgKeyPassword,
                        Encryption = EncryptionAlgorithm.WinZipAes256,
                        StatusMessageTextWriter = Console.Out
                    };

                    temp = Path.Combine(Static.Constants.AppDataFolder, "AMGkey");
                    archive.ExtractAll(temp, ExtractExistingFileAction.OverwriteSilently);
                    string filePath = Path.Combine(temp, "AMGkey.txt");

                    if (File.Exists(filePath))
                    {
                        this.Key = File.ReadAllText(filePath);

                        if (string.IsNullOrEmpty(this.Key))
                        {
                            res = U_ModHasherResult.KeyEmpty;
                            message = "No key found!";
                            return false;
                        }

                        if (this.HashOk(this.Key))
                        {
                            res = U_ModHasherResult.Ok;
                            return true;
                        }
                        else
                        {
                            res = U_ModHasherResult.OutofDate;
                            message = "Activation key expired!";
                            return false;
                        }


                    }

                    res = U_ModHasherResult.KeyFileNotFound;
                    message = "Key file not found!";
                    return false;
                }

                message = "Zip file not found!";
                res = U_ModHasherResult.ZipFileNotFound;
                return false;
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("TryCheckHash", e);
                message = AmgShared.Helpers.StringHelpers.ErrorMessage(e);
                res =  U_ModHasherResult.Error;
                return false;
            }
            finally
            {
                if (Directory.Exists(temp))
                {
                    Directory.Delete(temp, true);
                }

                if (File.Exists(this.KeyFilePath))
                {
                    File.Delete(this.KeyFilePath);
                }
            }

        }
    }
}
