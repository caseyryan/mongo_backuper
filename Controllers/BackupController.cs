using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Controllers.Dto;
using Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MongoBackuper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        // GET api/backup
        [HttpGet]
        public ActionResult<string> Get() {
            // можно использовать для проверки состояния сервиса
            return "Service OK!";
        }

        // POST api/backup/now
        [HttpPost("Now")]
        public async Task<string> Now([FromBody] BackupDto dto)
        {   
            StorageType storageType = dto.StorageType.ToEnum<StorageType>();
            

            return new Dictionary<string, object>() {
                { "result", "failure"},
                { "reason", $"Не удалось сделать резервную копию в хранилище {storageType}" }
            }.ToJson();
        }

        // POST api/backup/list
        [HttpPost("List")]
        public async Task<string> List([FromBody] BackupDto dto)
        {   
            StorageType storageType = dto.StorageType.ToEnum<StorageType>();
            

            return new Dictionary<string, object>() {
                { "result", "failure"},
                { "reason", $"Не удалось вывести список архивов для хранилища {storageType}" }
            }.ToJson();
        }

        /// удаляет локальную копию базы с указанным именем файла
        // DELETE api/backup/bkp_12_06_2019
        [HttpDelete("{name}")]
        public void Delete(string name)
        {

        }
    }
}
