using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace Spotify
{
    public static class Spotify
    {
        private static SpotifyClient _clientInstance;
        private static readonly object Padlock = new object();

        private static async Task<SpotifyClient> AccessSpotify()
        {
            SpotifyClientConfig config = SpotifyClientConfig.CreateDefault();

            var request = new ClientCredentialsRequest(
                Environment.GetEnvironmentVariable("Token_Spotify_ClientID"),
                Environment.GetEnvironmentVariable("Token_Spotify_ClientSecret")
            );
            CredentialsTokenResponse response = await new OAuthClient(config).RequestToken(request);

            return new SpotifyClient(config.WithToken(response.AccessToken));
        }

        public static SpotifyClient GetSpotifyClient
        {
            get
            {
                lock (Padlock)
                {
                    return _clientInstance ??= AccessSpotify().Result;
                }
            }
        }
    }
}
