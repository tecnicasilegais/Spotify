using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ShellProgressBar;
using SpotifyAPI.Web;

namespace Spotify
{
    class Program
    {
        private static SpotifyClient _spotify = null;
        public static async Task Main()
        {
            _spotify = Spotify.GetSpotifyClient;
            await GetArtistGenre("C:\\Users\\eduar\\Desktop\\Ouvidas - Artistas.csv");
        }

        public static async Task GetArtistGenre(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            Console.Write("Coluna do artista(zero-based):");
            int artistIndex = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Pesquisando gênero musical dos artistas");

            using (var progress = new ProgressBar(lines.Length, "", new ProgressBarOptions { ProgressCharacter = '─' }))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    progress.Tick($"Artista {i + 1} de {lines.Length}");
                    var data = lines[i].Split(',');
                    string appendix = lines[i][^1] == ',' ? "" : ",";

                    var requestData = new SearchRequest
                    (SearchRequest.Types.Artist, data[artistIndex])
                    { Limit = 1 };
                    SearchResponse search = await _spotify.Search.Item(requestData);

                    if (search.Artists.Items.Count <= 0 ||
                        search.Artists.Items[0].Genres.Count <= 0)
                    {
                        lines[i] += $"{appendix}desconhecido";
                        continue;
                    }

                    var genres = search.Artists.Items[0].Genres;
                    string genre = "";
                    foreach (string s in genres)
                    {
                        genre = s switch
                        {
                            var a when a.Contains("emo") => "emo",
                            var a when a.Contains("jazz") => "jazz",
                            var a when a.Contains("lo-fi") => "lo-fi",
                            _ => genres[0]
                        };
                    }

                    lines[i] += $"{appendix}{genre}";
                }
            }
            Console.WriteLine("Gravando arquivo...");
            File.WriteAllLines(filePath, lines);
            Console.WriteLine("Arquivo salvo com sucesso.");
        }
    }
}
