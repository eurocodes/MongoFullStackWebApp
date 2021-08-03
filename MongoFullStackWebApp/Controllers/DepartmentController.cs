using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoFullStackWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoFullStackWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            var dbList = dbClient.GetDatabase("testdb").GetCollection<Department>("Department").AsQueryable();
            return new JsonResult(dbList);
        }

        [HttpPost]
        public JsonResult Post(Department dept)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            int lastdeptId = dbClient.GetDatabase("testdb").GetCollection<Department>("Department").AsQueryable().Count();
            dept.DepartmentId = lastdeptId + 1;

            dbClient.GetDatabase("testdb").GetCollection<Department>("Department").InsertOne(dept);

            return new JsonResult("Added susseccfully");
        }

        [HttpPut]
        public JsonResult Put(Department dept)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            var filter = Builders<Department>.Filter.Eq("DepartmentId", dept.DepartmentId);
            var update = Builders<Department>.Update.Set("DepartmentName", dept.DepartmentName);

            dbClient.GetDatabase("testdb").GetCollection<Department>("Department").UpdateOne(filter, update);
            return new JsonResult("Updated susseccfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            var filter = Builders<Department>.Filter.Eq("DepartmentId", id);

            dbClient.GetDatabase("testdb").GetCollection<Department>("Department").DeleteOne(filter);
            return new JsonResult("Deleted susseccfully");
        }
    }
}
