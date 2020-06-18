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

            Console.WriteLine("Índice do artista:");
            int artistIndex = Convert.ToInt32(Console.ReadLine());

            using (var progress = new ProgressBar(lines.Length, "pesquisando gênero musical dos artistas"))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    progress.Tick($"Artist {i + 1} de {lines.Length}");
                    var data = lines[i].Split(',');
                    string appendix = lines[i][^1] == ',' ? "" : ",";

                    var requestData = new SearchRequest
                    (SearchRequest.Types.Artist, data[artistIndex])
                    { Limit = 1 };
                    SearchResponse search = await _spotify.Search.Item(requestData);

                    if (search.Artists.Items.Count <= 0 ||
                        search.Artists.Items[0].Genres.Count <= 0)
                    {
                        lines[i] += $"{appendix}Desconhecido";
                        continue;
                    }

                    var genres = search.Artists.Items[0].Genres;
                    int genreIndex = genres.FindIndex(IsSearchedGenre);
                    string genre = genreIndex >= 0 ? genres[genreIndex] : genres[0];
                    lines[i] += $"{appendix}{genre}";
                }
            }
            Console.WriteLine("Gravando arquivo...");
            File.WriteAllLines(filePath, lines);
            Console.WriteLine("Arquivo salvo com sucesso.");
        }

        private static bool IsSearchedGenre(string str)
        {
            return str.Contains("emo") || str.Contains("lo-fi") || str.Contains("jazz");
        }
    }
}
