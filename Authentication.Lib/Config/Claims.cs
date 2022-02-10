namespace Authentication.Lib.Config
{
    public class Claims
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
    }
}