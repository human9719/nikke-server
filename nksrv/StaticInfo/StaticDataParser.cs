﻿using nksrv.Utils;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using Swan.Parsers;
using Newtonsoft.Json;

namespace nksrv.StaticInfo
{
    /// <summary>
    /// "Static data" which is what the game calls it, contains data such as map info, characters, quests, rewards and a lot more.
    /// </summary>
    public class StaticDataParser
    {
        // Extracted from staticinfo api call
        public const string StaticDataUrl = "https://cloud.nikke-kr.com/prdenv/121-c5e64b1a1b/staticdata/data/qa-240620-05b-p1/307748/StaticData.pack";
        public const string Version = "data/qa-240620-05b-p1/307748";
        public const int Size = 11575712;
        public static byte[] Sha256Sum = Convert.FromBase64String("PBcDa3PoHR2MJQ+4Xc3/FUSgkqx2gY25MBJ0ih9FMsM=");
        public static byte[] Salt1 = Convert.FromBase64String("WqyrQ8MGtzwHN3AGPkqVKyjdfWZjBJXw9K7nGblv/SA=");
        public static byte[] Salt2 = Convert.FromBase64String("6Gf2jEvAX2mt5OWIxIU5uDdbjKtIc+VgTjKKSLuYnsI=");

        // These fields were extracted from the game.
        public static byte[] PresharedKey = [0xCB, 0xC2, 0x1C, 0x6F, 0xF3, 0xF5, 0x07, 0xF5, 0x05, 0xBA, 0xCA, 0xD4, 0x98, 0x28, 0x84, 0x1F, 0xF0, 0xD1, 0x38, 0xC7, 0x61, 0xDF, 0xD6, 0xE6, 0x64, 0x9A, 0x85, 0x13, 0x3E, 0x1A, 0x6A, 0x0C, 0x68, 0x0E, 0x2B, 0xC4, 0xDF, 0x72, 0xF8, 0xC6, 0x55, 0xE4, 0x7B, 0x14, 0x36, 0x18, 0x3B, 0xA7, 0xD1, 0x20, 0x81, 0x22, 0xD1, 0xA9, 0x18, 0x84, 0x65, 0x13, 0x0B, 0xED, 0xA3, 0x00, 0xE5, 0xD9];
        public static RSAParameters RSAParameters = new RSAParameters()
        {
            Exponent = [0x01, 0x00, 0x01],
            Modulus = [0x89, 0xD6, 0x66, 0x00, 0x7D, 0xFC, 0x7D, 0xCE, 0x83, 0xA6, 0x62, 0xE3, 0x1A, 0x5E, 0x9A, 0x53, 0xC7, 0x8A, 0x27, 0xF3, 0x67, 0xC1, 0xF3, 0xD4, 0x37, 0xFE, 0x50, 0x6D, 0x38, 0x45, 0xDF, 0x7E, 0x73, 0x5C, 0xF4, 0x9D, 0x40, 0x4C, 0x8C, 0x63, 0x21, 0x97, 0xDF, 0x46, 0xFF, 0xB2, 0x0D, 0x0E, 0xDB, 0xB2, 0x72, 0xB4, 0xA8, 0x42, 0xCD, 0xEE, 0x48, 0x06, 0x74, 0x4F, 0xE9, 0x56, 0x6E, 0x9A, 0xB1, 0x60, 0x18, 0xBC, 0x86, 0x0B, 0xB6, 0x32, 0xA7, 0x51, 0x00, 0x85, 0x7B, 0xC8, 0x72, 0xCE, 0x53, 0x71, 0x3F, 0x64, 0xC2, 0x25, 0x58, 0xEF, 0xB0, 0xC9, 0x1D, 0xE3, 0xB3, 0x8E, 0xFC, 0x55, 0xCF, 0x8B, 0x02, 0xA5, 0xC8, 0x1E, 0xA7, 0x0E, 0x26, 0x59, 0xA8, 0x33, 0xA5, 0xF1, 0x11, 0xDB, 0xCB, 0xD3, 0xA7, 0x1F, 0xB1, 0xC6, 0x10, 0x39, 0xC8, 0x31, 0x1D, 0x60, 0xDB, 0x0D, 0xA4, 0x13, 0x4B, 0x2B, 0x0E, 0xF3, 0x6F, 0x69, 0xCB, 0xA8, 0x62, 0x03, 0x69, 0xE6, 0x95, 0x6B, 0x8D, 0x11, 0xF6, 0xAF, 0xD9, 0xC2, 0x27, 0x3A, 0x32, 0x12, 0x05, 0xC3, 0xB1, 0xE2, 0x81, 0x4B, 0x40, 0xF8, 0x8B, 0x8D, 0xBA, 0x1F, 0x55, 0x60, 0x2C, 0x09, 0xC6, 0xED, 0x73, 0x96, 0x32, 0xAF, 0x5F, 0xEE, 0x8F, 0xEB, 0x5B, 0x93, 0xCF, 0x73, 0x13, 0x15, 0x6B, 0x92, 0x7B, 0x27, 0x0A, 0x13, 0xF0, 0x03, 0x4D, 0x6F, 0x5E, 0x40, 0x7B, 0x9B, 0xD5, 0xCE, 0xFC, 0x04, 0x97, 0x7E, 0xAA, 0xA3, 0x53, 0x2A, 0xCF, 0xD2, 0xD5, 0xCF, 0x52, 0xB2, 0x40, 0x61, 0x28, 0xB1, 0xA6, 0xF6, 0x78, 0xFB, 0x69, 0x9A, 0x85, 0xD6, 0xB9, 0x13, 0x14, 0x6D, 0xC4, 0x25, 0x36, 0x17, 0xDB, 0x54, 0x0C, 0xD8, 0x77, 0x80, 0x9A, 0x00, 0x62, 0x83, 0xDD, 0xB0, 0x06, 0x64, 0xD0, 0x81, 0x5B, 0x0D, 0x23, 0x9E, 0x88, 0xBD],
            DP = null
        };

        // Fields
        public static StaticDataParser Instance;
        private ZipFile MainZip;
        private MemoryStream ZipStream;
        private JArray questDataRecords;
        private JArray stageDataRecords;

        public StaticDataParser(string filePath)
        {
            if (!File.Exists(filePath)) throw new ArgumentException("Static data file must exist", nameof(filePath));
            DecryptStaticDataAndLoadZip(filePath);
            if (MainZip == null) throw new Exception("failed to read zip file");
        }
        #region Decryption
        private void DecryptStaticDataAndLoadZip(string file)
        {
            using var fileStream = File.Open(file, FileMode.Open, FileAccess.Read);

            var keyDecryptor = new Rfc2898DeriveBytes(PresharedKey, Salt2, 10000, HashAlgorithmName.SHA256);
            var key2 = keyDecryptor.GetBytes(32);

            byte[] decryptionKey = key2[0..16];
            byte[] iv = key2[16..32];
            var aes = Aes.Create();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Key = decryptionKey;
            aes.IV = iv;
            var transform = aes.CreateDecryptor();

            // Decryption layer 1
            using CryptoStream stream = new CryptoStream(fileStream, transform, CryptoStreamMode.Read);

            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);

            var bytes = ms.ToArray();

            // Decryption of layer 2
            var zip = new ZipFile(ms, false);

            var signEntry = zip.GetEntry("sign");
            if (signEntry == null) throw new Exception("Sign entry not found in decrypted static data pack");
            var dataEntry = zip.GetEntry("data");
            if (dataEntry == null) throw new Exception("Data entry not found in decrypted static data pack");

            var signStream = zip.GetInputStream(signEntry);
            var dataStream = zip.GetInputStream(dataEntry);

            using MemoryStream signMs = new MemoryStream();
            signStream.CopyTo(signMs);

            using MemoryStream dataMs = new MemoryStream();
            dataStream.CopyTo(dataMs);
            dataMs.Position = 0;

            var rsa = RSA.Create(RSAParameters);
            if (!rsa.VerifyData(dataMs, signMs.ToArray(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                throw new Exception("failed to decrypt static data (round 2)");

            dataMs.Position = 0;

            // Decryption of layer 3
            var keyDecryptor2 = new Rfc2898DeriveBytes(PresharedKey, Salt1, 10000, HashAlgorithmName.SHA256);
            var key3 = keyDecryptor2.GetBytes(32);

            byte[] decryptionKey2 = key3[0..16];
            byte[] iv2 = key3[16..32];

            ZipStream = new MemoryStream();
            AesCtrTransform(decryptionKey2, iv2, dataMs, ZipStream);
            MainZip = new ZipFile(ZipStream, false);
        }

        public static void AesCtrTransform(
    byte[] key, byte[] salt, Stream inputStream, Stream outputStream)
        {
            SymmetricAlgorithm aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            int blockSize = aes.BlockSize / 8;

            if (salt.Length != blockSize)
            {
                throw new ArgumentException(
                    "Salt size must be same as block size " +
                    $"(actual: {salt.Length}, expected: {blockSize})");
            }

            var counter = (byte[])salt.Clone();

            var xorMask = new Queue<byte>();

            var zeroIv = new byte[blockSize];
            ICryptoTransform counterEncryptor = aes.CreateEncryptor(key, zeroIv);

            int b;
            while ((b = inputStream.ReadByte()) != -1)
            {
                if (xorMask.Count == 0)
                {
                    var counterModeBlock = new byte[blockSize];

                    counterEncryptor.TransformBlock(
                        counter, 0, counter.Length, counterModeBlock, 0);

                    for (var i2 = counter.Length - 1; i2 >= 0; i2--)
                    {
                        if (++counter[i2] != 0)
                        {
                            break;
                        }
                    }

                    foreach (var b2 in counterModeBlock)
                    {
                        xorMask.Enqueue(b2);
                    }
                }

                var mask = xorMask.Dequeue();
                outputStream.WriteByte((byte)(((byte)b) ^ mask));
            }
        }
        public static async Task Load()
        {
            string targetFile = Program.GetCachePathForPath(StaticDataUrl.Replace("https://cloud.nikke-kr.com", ""));
            var targetDir = Path.GetDirectoryName(targetFile);

            Directory.CreateDirectory(targetDir);

            if (!File.Exists(targetFile))
            {
                // TODO: Ip might change
                var requestUri = new Uri("https://43.132.66.200/" + StaticDataUrl.Replace("https://cloud.nikke-kr.com", ""));
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.TryAddWithoutValidation("host", "cloud.nikke-kr.com");

                Logger.Info("Downloading static game data from server. Please wait.");
                using var response = await Program.AssetDownloader.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using var fss = new FileStream(targetFile, FileMode.CreateNew);
                    await response.Content.CopyToAsync(fss);

                    fss.Close();
                }
                else
                {
                    throw new Exception("Failed to download static game data");
                }
            }


            Instance = new(targetFile);
        }
        #endregion
        public async Task Parse()
        {
            var mainQuestData = MainZip.GetEntry("MainQuestTable.json");
            var campaignStageData = MainZip.GetEntry("CampaignStageTable.json");

            if (mainQuestData == null) throw new Exception("MainQuestTable.json does not exist in static data");
            if (campaignStageData == null) throw new Exception("CampaignStageTable.json does not exist in static data");

            using StreamReader mainQuestReader = new StreamReader(MainZip.GetInputStream(mainQuestData));
            var mainQuestDataString = await mainQuestReader.ReadToEndAsync();

            using StreamReader campaignStageDataReader = new StreamReader(MainZip.GetInputStream(mainQuestData));
            var campaignStageDataString = await campaignStageDataReader.ReadToEndAsync();

            var questdata = JObject.Parse(mainQuestDataString);
            var stagedata = JObject.Parse(campaignStageDataString);

            questDataRecords = (JArray?)questdata["records"];
            stageDataRecords = (JArray?)stagedata["records"];
            if (questDataRecords == null) throw new Exception("MainQuestTable.json does not contain records array");
            if (stageDataRecords == null) throw new Exception("CampaignStageTable.json does not contain records array");
        }

        public MainQuestCompletionData? GetMainQuestForStageClearCondition(int stage)
        {
            foreach (JObject item in questDataRecords)
            {
                var id = item["condition_id"];
                if (id == null) throw new Exception("expected condition_id field in quest data");

                int value = id.ToObject<int>();
                if (value == stage)
                {
                    MainQuestCompletionData? data = JsonConvert.DeserializeObject<MainQuestCompletionData>(item.ToString());
                    if (data == null) throw new Exception("failed to deserialize main quest data item");
                    return data;
                }
            }

            return null;
        }
        public MainQuestCompletionData? GetMainQuestByTableId(int tid)
        {
            foreach (JObject item in questDataRecords)
            {
                var id = item["id"];
                if (id == null) throw new Exception("expected condition_id field in quest data");

                int value = id.ToObject<int>();
                if (value == tid)
                {
                    MainQuestCompletionData? data = JsonConvert.DeserializeObject<MainQuestCompletionData>(item.ToString());
                    if (data == null) throw new Exception("failed to deserialize main quest data item");
                    return data;
                }
            }

            return null;
        }
    }
}