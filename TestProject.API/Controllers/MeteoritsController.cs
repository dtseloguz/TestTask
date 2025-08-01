using Microsoft.AspNetCore.Mvc;
using TestProject.Application.Mappers;
using TestProject.Common.Models;
using TestProject.Core.Interfaces;

namespace TestProject.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeteoritsController : ControllerBase
    {
        private readonly IMeteoriteRepository _meteoriteRepository;

        public MeteoritsController(
            IMeteoriteRepository meteoriteRepository)
        {
            _meteoriteRepository = meteoriteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMeteorites(
            [FromQuery] YearRange? year,
            [FromQuery] string? recClass,
            [FromQuery] string? name,
            [FromQuery] MeteoriteByYearSort sort,
            CancellationToken cancellationToken)
        {
            var result = await _meteoriteRepository.SearchMeteorites(
                year?.From,
                year?.To,
                recClass,
                name,
                sort,
                cancellationToken);
            
            if (result.IsSuccess) 
            {
                return Ok(MeteoriteMapper.mapToMeteoritesByYearDTO(result.Value));
            }
            else
            {
                return Problem(
                        title: "error",
                        detail: result.Error.Message,
                        statusCode: result.Error.Code);
            }
        }

        [HttpGet("YearsList")]
        public async Task<IActionResult> GetYearsList(CancellationToken cancellationToken)
        {
            var result = await _meteoriteRepository.GetYearsList(cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return Problem(
                        title: "error",
                        detail: result.Error.Message,
                        statusCode: result.Error.Code);
            }
        }

        [HttpGet("RecClassList")]
        public async Task<IActionResult> GetRecClassList(CancellationToken cancellationToken)
        {
            var result = await _meteoriteRepository.GetRecClassList(cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return Problem(
                        title: "error",
                        detail: result.Error.Message,
                        statusCode: result.Error.Code);
            }
        }
    }
}