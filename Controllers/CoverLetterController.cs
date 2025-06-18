using Microsoft.AspNetCore.Mvc;
using RizeUp.DTOs;
using RizeUp.Interfaces;

namespace RizeUp.Controllers
{
    public class CoverLetterController : Controller
    {
        private readonly IAiLetterService _service;
        public CoverLetterController(IAiLetterService service)
        {
            _service = service;
        }
        public IActionResult CoverLetter()
        {
            return View(new CoverLetterRequestDto());
        }

        public async Task<IActionResult> CreateCoverLetter(CoverLetterRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View("CoverLetter",dto);
            }else
            {
                var c = await _service.GenerateAsync(dto);
                if(c != null)
                {
                    return View("CoverLetterResult", c);
                }else
                {
                    return View("CoverLetter", dto);
                }
            }
        }

        public IActionResult CoverLetterResult (CoverLetterResponseDto dto)
        {
            return View(dto);
        }



        public IActionResult ThankYouLetter()
        {
            return View();
        }
    }
}
