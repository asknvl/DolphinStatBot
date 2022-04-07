

using RestSharp;

class Program
{
    public static void Main(string[] args)
    {
        //var client = new RestClient("https://188.225.43.87/2/#/adsmanager/new/users");        
        //var request = new RestRequest(Method.GET);
        //request.AddHeader("Authorization", "1-578000f643ac0dd4f72579dd758ebd8e");
        //request.AddHeader("Content-Type", "application/json");
        //var response = client.Execute(request);
        //Console.WriteLine(response.Content);

        var client = new RestClient("http://188.225.43.87/new/users/");
        client.Timeout = -1;
        var request = new RestRequest(Method.GET);
        request.AddHeader("Authorization", "1-578000f643ac0dd4f72579dd758ebd8e");
        request.AddHeader("Content-Type", "application/json");
       // var body = @"";
       // request.AddParameter("application/json", body, ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);
        Console.WriteLine(response.Content);

    }
}