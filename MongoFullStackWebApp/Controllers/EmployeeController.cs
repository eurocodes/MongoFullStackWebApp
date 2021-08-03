using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoFullStackWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MongoFullStackWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            var dbList = dbClient.GetDatabase("testdb").GetCollection<Employee>("Employee").AsQueryable();
            return new JsonResult(dbList);
        }

        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            int lastdeptId = dbClient.GetDatabase("testdb").GetCollection<Employee>("Employee").AsQueryable().Count();
            emp.EmployeeId = lastdeptId + 1;

            dbClient.GetDatabase("testdb").GetCollection<Employee>("Employee").InsertOne(emp);

            return new JsonResult("Added susseccfully");
        }

        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            var filter = Builders<Employee >.Filter.Eq("EmployeeId", emp.EmployeeId);
            var update = Builders<Employee>.Update.Set("EmployeeName", emp.EmployeeName)
                .Set("Department", emp.Department)
                .Set("DateOfJoining", emp.DateOfJoining)
                .Set("PhotoFileName", emp.PhotoFileName);

            dbClient.GetDatabase("testdb").GetCollection<Employee>("Employee").UpdateOne(filter, update);
            return new JsonResult("Updated susseccfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            var filter = Builders<Employee>.Filter.Eq("Employee", id);

            dbClient.GetDatabase("testdb").GetCollection<Employee>("Department").DeleteOne(filter);
            return new JsonResult("Deleted susseccfully");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using(var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(filename);
            }
            catch (Exception)
            {

                return new JsonResult("anonymous.png");
            }
        }
    }
}
