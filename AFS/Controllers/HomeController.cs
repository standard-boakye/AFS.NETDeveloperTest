using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFS.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using AFS.Interface;

namespace AFS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Contents> _contentRepo;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IRepository<Contents> contentRepo)
        {
            _logger = logger;
            _configuration = configuration;
            _contentRepo = contentRepo;
        }

        public async Task<IActionResult> IndexAsync()
        {
            try
            {
                List<Contents> contents = new List<Contents>();

                contents = await _contentRepo.GetAll();
                return View(contents);
            }
            catch (Exception ex)
            {
                return new JsonResult(View(ex.InnerException.Message));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public async Task<IActionResult> TranslateText([FromBody]UserText _text)
        {
            string logEntry;

            try
            {
                if (!ModelState.IsValid)
                    return View();
                
                Response funResponse = new Response();
                string apiUrl = $"{_configuration["BaseAddress:BaseUrl"]}/translate/leetspeak.json";

                using (var client = new HttpClient())
                {
                    var _content = new StringContent(JsonConvert.SerializeObject(_text), Encoding.UTF8, "application/json");

                     logEntry = $"[API CALL] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {apiUrl}\nRequest Data: {_text.text}\n";
                    _logger.LogInformation(logEntry);

                    HttpResponseMessage msgResponse = await client.PostAsync(apiUrl, _content);
                    
                    if (msgResponse.IsSuccessStatusCode)
                    {
                        var responseDetails = await msgResponse.Content.ReadAsStringAsync();
                        funResponse = JsonConvert.DeserializeObject<Response>(responseDetails);
                        await _contentRepo.Insert(funResponse.contents);

                        logEntry = $"[API RESPONSE] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - Status Code: {msgResponse.StatusCode}\nResponse Content: {msgResponse.Content.ReadAsStringAsync().Result}\n";
                        _logger.LogInformation(logEntry);
                    }
                    else
                    {
                        ViewBag.StatusCode = msgResponse.StatusCode;
                    }

                    return new JsonResult(funResponse);
                }
            }
            catch (Exception ex)
            {
                 logEntry = $"[EXCEPTION] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {ex.ToString()}\n";
                _logger.LogError(logEntry);
                return new JsonResult(View(ex.InnerException.Message));
            }
        }

    }
}
