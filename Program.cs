using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ShellProgressBar;
using SpotifyAPI.Web;

namespace Spotify
{
    internal static class Program
    {
        private static SpotifyClient _spotify;
        public static async Task Main()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.WriteLine("Pesquisa de gênero musical de artistas");
            Console.Write("Informe o caminho do csv sem cabeçalho:");
            string filePath = Console.ReadLine();

            _spotify = Spotify.GetSpotifyClient;
            await GetArtistGenre(filePath);
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
                    progress.Tick($"Artista {i + 1} de {lines.Length + 1}");
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
                    string genre = SimplifySearchedGenre(genres) ?? genres[0];


                    lines[i] += $"{appendix}{genre}";
                }
            }
            Console.WriteLine("Gravando arquivo...");
            File.WriteAllLines(filePath, lines);
            Console.WriteLine("Arquivo salvo com sucesso.");
        }

        private static string SimplifySearchedGenre(IEnumerable<string> genres)
        {
            foreach (string s in genres)
            {
                switch (s)
                {
                    case var a when a.Contains("emo"):
                        return "emo";
                    case var a when a.Contains("jazz"):
                        return "jazz";
                    case var a when a.Contains("lo-fi"):
                        return "lo-fi";
                }
            }

            return null;
        }
    }
}
