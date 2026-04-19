using Newtonsoft.Json;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Image = System.Drawing.Image;
using ISImage = SixLabors.ImageSharp.Image;

namespace OutlastTrayTool
{
    public class ModManagerAPI
    {


        public ModManagerAPI()
        {
            

        }

        public static async Task<dynamic> GetModPageAsync() //int page
        {
            string apiUrl = "https://api.nexusmods.com/v2/graphql";
            string outlastDomain = "theoutlasttrials";

            using HttpClient client = new HttpClient();

            var queryString = @"query GetModsForGame($domain: String!, $offset: Int!, $count: Int!) { 
                                    mods(
                                        filter: { gameDomainName: [{ value: $domain }] }
                                        offset: $offset,
                                        count: $count
                                        ) { 
                                        nodes { modId uid name summary description uploader { name } thumbnailUrl version updatedAt downloads endorsements } 
                                        nodesCount 
                                        totalCount 
                                    } 
                                }";
            var requestPayload = new
            {
                query = queryString,
                variables = new
                {
                    domain = outlastDomain,
                    offset = 0,
                    count = 20
                }
            };

            var response = await client.PostAsJsonAsync(apiUrl, requestPayload);
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(result)!;
        }

        public static async Task<dynamic> GetModName(int modId)
        {
            int gameId = 5376;
            string apiUrl = "https://api.nexusmods.com/v2/graphql";

            using HttpClient client = new HttpClient();

            var queryString = @"query mod($modId: ID!, $gameId: ID!) {
                                  mod(modId: $modId, gameId: $gameId) {
                                    name
                                  }
                                }";
            var requestPayload = new
            {
                query = queryString,
                variables = new
                {
                    modId,
                    gameId
                }
            };

            var response = await client.PostAsJsonAsync(apiUrl, requestPayload);
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(result)!;
        }

        public static async Task<dynamic> GetModFilesAsync(int modId)
        {
            int gameId = 5376;
            string apiUrl = "https://api.nexusmods.com/v2/graphql";

            using HttpClient client = new HttpClient();

            var queryString = @"query GetModFiles($modId: ID!, $gameId: ID!) {
                                  modFiles(modId: $modId, gameId: $gameId) {
                                    fileId
                                    name
                                    version
                                    category
                                    uri
                                  }
                                }";
            var requestPayload = new
            {
                query = queryString,
                variables = new
                {
                    modId,
                    gameId
                }
            };

            var response = await client.PostAsJsonAsync(apiUrl, requestPayload);
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(result)!;
        }



        public static async Task LoadImageAsync(string url, PictureBox pictureBox)
        {
            using HttpClient client = new HttpClient();
            using Stream stream = await client.GetStreamAsync(url);
            using ISImage image = await ISImage.LoadAsync(stream);
            var ms = new MemoryStream();
            await image.SaveAsBmpAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            pictureBox.Image = Image.FromStream(ms);

        }
    }
    
        
    }




