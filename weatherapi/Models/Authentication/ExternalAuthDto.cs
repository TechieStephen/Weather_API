namespace weatherapi.Models.Authentication
{
    public class ExternalAuthDto
    {
        //provider e.g (google, facebook)
        public string Provider { get; set; }

        // this is the client ID gotton from the provider
        public string Token { get; set; }
    }
}
