using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projekt_api.Data;
using projekt_api.Models;

// namespace projekt_api.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class DeviceController : ControllerBase
//     {
//         private readonly ApiContext _context;

//         public DeviceController(ApiContext context)
//         {
//             _context = context;
//         }

        

//         [HttpPost]
//         public ActionResult<Devices> CreateEdit(Devices devices)
//         {
//             if (devices.Id == 0)
//             {
//                 _context.Devices.Add(devices);
//             }
//             else
//             {
//                 var deviceInDb = _context.Devices.Find(devices.Id);
//                 if (deviceInDb == null)
//                 {
//                     return new JsonResult("Device not found");
//                 }
//                 else
//                 {
//                     deviceInDb.Name = devices.Name;
//                     deviceInDb.Owner = devices.Owner;
//                     deviceInDb.Status = devices.Status;
//                     deviceInDb.IsCar = devices.IsCar;
//                 }
//             }
//             _context.SaveChanges();
//             return Ok(devices);
//         }

//         //GET
//         [HttpGet("{id}")]
//         public ActionResult<Devices> Get(int id)
//         {
//             var device = _context.Devices.Find(id);
//             if (device == null)
//             {
//                 return new JsonResult("Device not found");
//             }
//             else
//             {
//                 return Ok(device);
//             }
//         }

//         //GETALL
//         [HttpGet]
//         public ActionResult<Devices> GetAll()
//         {
//             var result = _context.Devices.ToList();
//             return Ok(result);
//         }

//     }
// }

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly TableStorageService _service;

    public DevicesController(TableStorageService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var devices = _service.GetDevices();
        return Ok(devices);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] DeviceEntity device)
    {
        device.RowKey = Guid.NewGuid().ToString();
        await _service.AddDeviceAsync(device);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteDeviceAsync(id);
        return NoContent();
    }
}
