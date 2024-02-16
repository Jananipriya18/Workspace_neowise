using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


[TestFixture]
public class Tests
{
    private HttpClient _httpClient;
    private string _generatedToken;
    [SetUp]
    public void Setup()
    {
         _httpClient = new HttpClient();
        // _httpClient.BaseAddress = new Uri("https://localhost:7266"); 
        // _httpClient.BaseAddress = new Uri("https://api.example.com/"); 
        _httpClient.BaseAddress = new Uri("https://8080-bfdeeddcedfabcfacbdcbaeadbebabcdebdca.premiumproject.examly.io/"); 


    }

    // [Test]
    // public void Test1()
    // {
    //     Assert.Pass();
    // }

    
    [Test, Order(1)]
    public async Task Backend_TestRegisterUser()
    {
        string uniqueId = Guid.NewGuid().ToString();

        // Generate a unique userName based on a timestamp
        string uniqueUsername = $"abcd_{uniqueId}";
        string uniqueEmail = $"abcd{uniqueId}@gmail.com";

        string requestBody = $"{{\"Username\": \"{uniqueUsername}\", \"Password\": \"abc@123A\", \"Email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\", \"Role\": \"customer\"}}";
        HttpResponseMessage response = await _httpClient.PostAsync("/api/register", new StringContent(requestBody, Encoding.UTF8, "application/json"));

        Console.WriteLine(response.StatusCode);
        string responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseString);

        // Assuming you get a 200 OK status for successful registration
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test, Order(2)]
    public async Task Backend_TestLoginUser()
    {
        string uniqueId = Guid.NewGuid().ToString();

        string uniqueusername = $"abcd_{uniqueId}";
        string uniquepassword = $"abcdA{uniqueId}@123";
        string uniqueEmail = $"abcd{uniqueId}@gmail.com";
        string uniquerole = "customer";
        string requestBody = $"{{\"Username\": \"{uniqueusername}\", \"Password\": \"{uniquepassword}\", \"Email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\",\"Role\" : \"{uniquerole}\" }}";
        HttpResponseMessage response = await _httpClient.PostAsync("/api/register", new StringContent(requestBody, Encoding.UTF8, "application/json"));
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        string requestBody1 = $"{{\"email\": \"{uniqueEmail}\",\"password\": \"{uniquepassword}\"}}";
        HttpResponseMessage response1 = await _httpClient.PostAsync("/api/login", new StringContent(requestBody1, Encoding.UTF8, "application/json"));
        Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);
        string responseBody = await response1.Content.ReadAsStringAsync();
    }

    [Test, Order(3)]
    public async Task Backend_TestRegisterAdmin()
    {
        string uniqueId = Guid.NewGuid().ToString();
        string uniqueUsername = $"abcd_{uniqueId}";
        string uniqueEmail = $"abcd{uniqueId}@gmail.com";

        string requestBody = $"{{\"Username\": \"{uniqueUsername}\", \"Password\": \"abc@123A\", \"Email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\", \"Role\": \"admin\"}}";
        
        HttpResponseMessage response = await _httpClient.PostAsync("/api/register", new StringContent(requestBody, Encoding.UTF8, "application/json"));

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        string responseBody = await response.Content.ReadAsStringAsync();
        // Add your assertions based on the response if needed
    }

    [Test, Order(4)]
    public async Task Backend_TestLoginAdmin()
    {
        string uniqueId = Guid.NewGuid().ToString();

        string uniqueusername = $"abcd_{uniqueId}";
        string uniquepassword = $"abcdA{uniqueId}@123";
        string uniqueEmail = $"abcd{uniqueId}@gmail.com";
        string uniquerole = "admin";
        string requestBody = $"{{\"Username\": \"{uniqueusername}\", \"Password\": \"{uniquepassword}\", \"Email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\",\"Role\" : \"{uniquerole}\" }}";
        HttpResponseMessage response = await _httpClient.PostAsync("/api/register", new StringContent(requestBody, Encoding.UTF8, "application/json"));
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        string requestBody1 = $"{{\"email\": \"{uniqueEmail}\",\"password\": \"{uniquepassword}\"}}";
        HttpResponseMessage response1 = await _httpClient.PostAsync("/api/login", new StringContent(requestBody1, Encoding.UTF8, "application/json"));
        Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);
        string responseBody = await response1.Content.ReadAsStringAsync();
    }

    [Test, Order(5)]
    public async Task Backend_TestAddGiftAdmin()
    {
       string uniqueId = Guid.NewGuid().ToString();

       // Use a dynamic and unique userName for admin (appending timestamp)
       string uniqueusername = $"abcd_{uniqueId}";
        string uniqueEmail = $"abcd{uniqueId}@gmail.com";


       // Assume you have a valid admin registration method, adjust the request body accordingly
       string adminRegistrationRequestBody = $"{{\"password\": \"abc@123A\", \"userName\": \"{uniqueusername}\",\"role\": \"admin\",\"email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\"}}";
       HttpResponseMessage registrationResponse = await _httpClient.PostAsync("api/register", new StringContent(adminRegistrationRequestBody, Encoding.UTF8, "application/json"));

       Assert.AreEqual(HttpStatusCode.OK, registrationResponse.StatusCode);

       // Now, perform the login for the admin user
       string adminLoginRequestBody = $"{{\"email\": \"{uniqueEmail}\",\"password\": \"abc@123A\"}}";
       HttpResponseMessage loginResponse = await _httpClient.PostAsync("api/login", new StringContent(adminLoginRequestBody, Encoding.UTF8, "application/json"));

       Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
       string responseBody = await loginResponse.Content.ReadAsStringAsync();

       dynamic responseMap = JsonConvert.DeserializeObject(responseBody);

       string token = responseMap.token;

       Assert.IsNotNull(token);

       string uniquetitle = Guid.NewGuid().ToString();

       // Use a dynamic and unique userName for admin (appending timestamp)
       string uniqueGiftTitle = $"giftTitle_{uniquetitle}";

       string giftJson = $"{{\"GiftType\":\"{uniqueGiftTitle}\",\"GiftImageUrl\":\"test\",\"GiftDetails\":\"test\",\"GiftPrice\":10,\"Quantity\":1}}";
       _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
       HttpResponseMessage response = await _httpClient.PostAsync("/api/gift",
           new StringContent(giftJson, Encoding.UTF8, "application/json"));

       Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test, Order(6)]
    public async Task Backend_TestGetAllGiftsForAdminAndCustomer()
    {
       string uniqueId = Guid.NewGuid().ToString();

       // Use a dynamic and unique userName for admin (appending timestamp)
       string uniqueusername = $"admin_{uniqueId}";
       string uniqueEmail = $"abcd{uniqueId}@gmail.com";


       // Assume you have a valid admin registration method, adjust the request body accordingly
       string adminRegistrationRequestBody =  $"{{\"password\": \"abc@123A\", \"userName\": \"{uniqueusername}\",\"role\": \"admin\",\"email\": \"{uniqueEmail}\", \"MobileNumber\": \"1234567890\"}}";
       HttpResponseMessage registrationResponse = await _httpClient.PostAsync("api/register", new StringContent(adminRegistrationRequestBody, Encoding.UTF8, "application/json"));

       Assert.AreEqual(HttpStatusCode.OK, registrationResponse.StatusCode);

       // Now, perform the login for the admin user
       string adminLoginRequestBody = $"{{\"email\": \"{uniqueEmail}\",\"password\": \"abc@123A\"}}";
       HttpResponseMessage loginResponse = await _httpClient.PostAsync("api/login", new StringContent(adminLoginRequestBody, Encoding.UTF8, "application/json"));

       Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
       string responseBody = await loginResponse.Content.ReadAsStringAsync();

       dynamic responseMap = JsonConvert.DeserializeObject(responseBody);

       string token = responseMap.token;

       Assert.IsNotNull(token);


       Console.WriteLine("admin111" + token);
       _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

       HttpResponseMessage response = await _httpClient.GetAsync("/api/gift");

       Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

[Test, Order(7)]
public async Task Backend_TestGetCartByCustomerId()
{
    // Assuming customerId is the ID of an existing customer with a known cart
    long customerId = 1; // Replace with the actual customer ID

    // Assuming authentication and token retrieval logic here...

    // Set up authorization header with the obtained token
    _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + YOUR_ACCESS_TOKEN);

    // Perform the GET request to retrieve the cart by customer ID
    HttpResponseMessage response = await _httpClient.GetAsync($"/api/customer/{customerId}");

    // Assert the expected status code
    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

    // If needed, assert or process the response content as required
    string responseBody = await response.Content.ReadAsStringAsync();
    // Add assertions based on the expected response content or structure

    // Optionally, deserialize the response content if it's JSON and perform further assertions
    // Example:
    // var cart = JsonConvert.DeserializeObject<Cart>(responseBody);
    // Assert.IsNotNull(cart);
    // Add more assertions based on the properties of the Cart object
}



    [TearDown]
    public void TearDown()
    {
        // Cleanup or additional teardown logic if needed.
        _httpClient.Dispose();
    }
}