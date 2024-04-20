using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class HomeController : Controller
{
    // GET
    [HttpGet("/")]
    public string Index()
    {
        return  "api is working...";
    }
    
    [HttpGet("/login/{data}")]
    public IActionResult Login(string data)
    {
        if (data == "is-21fiot-22-035")
        {
            return Ok(new { Name = "Danylo", Surname = "Honcharov", Year=2, Group = "IS-21" });
        }
        return NotFound("User with this login not found");
    }

    [HttpGet("/api/crypto/{name}")]
    public  async Task<IActionResult> GetCryptoInfo(string name)
    {
        string text;
        using (var request = new HttpRequestMessage(HttpMethod.Get,$"http://api.coincap.io/v2/assets/{name}" ))
        {
            var client = new HttpClient();
            var response = client.Send(request);
            if (response.StatusCode is HttpStatusCode.NotFound)
                return NotFound("Wrong name");
            text = await response.Content.ReadAsStringAsync();
        }

        var cryptoData = JsonNode.Parse(text);
        var html = @$"
        <table>
            <thead>
                <tr>
                    <th>Ідентифікатор</th>
                    <th>Рейтинг</th>
                    <th>Символічне позначення</th>
                    <th>Назва</th>
                    <th>Доступна кількість для торгівлі</th>
                    <th>Обіг</th>
                    <th>Капіталізація в долларах</th>
                    <th>Ціна в долларах</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>{cryptoData["data"]["id"]}</td>
                    <td>{cryptoData["data"]["rank"]}</td>
                    <td>{cryptoData["data"]["symbol"]}</td>
                    <td>{cryptoData["data"]["name"]}</td>
                    <td>{cryptoData["data"]["supply"]}</td>
                    <td>{cryptoData["data"]["maxSupply"]}</td>
                    <td>{cryptoData["data"]["marketCapUsd"]}</td>
                    <td>{cryptoData["data"]["priceUsd"]}</td>
                </tr>
            </tbody>
        </table>";
        return Content(html,"text/html",Encoding.UTF8);
    }
}