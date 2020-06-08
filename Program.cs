using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace Spotify
{
    class Program
    {
        public static async Task Main()
        {
            var config = SpotifyClientConfig.CreateDefault();

            var request = new ClientCredentialsRequest(
                Environment.GetEnvironmentVariable("Token_Spotify_ClientID"),
                Environment.GetEnvironmentVariable("Token_Spotify_ClientSecret")
                );
            var response = await new OAuthClient(config).RequestToken(request);

            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));

            var playlist = await spotify.Playlists.Get("0nOc0WO1l1c5vTs6LQ3TIo");

            foreach (var item in playlist.Tracks.Items)
            {
                if (item.Track is FullTrack track)
                {
                    var tempArtists = new StringBuilder();
                    foreach (var artist in track.Artists.Where(artist => artist.Name != null))
                    {
                        tempArtists.Append(artist.Name);
                        tempArtists.Append(", ");
                    }
                    if (tempArtists.Length >= 2) { tempArtists.Length -= 2; }

                    Console.WriteLine($"{tempArtists} - {track.Name}");
                }
            }

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}
