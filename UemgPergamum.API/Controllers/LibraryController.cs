using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UemgPergamumAPI.Domain.Enum;
using UemgPergamumAPI.Domain.Services;

namespace UemgPergamumAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class LibraryController : ControllerBase
    {
        private readonly IWebScraperService _webScraperService;

        public LibraryController(IWebScraperService webScraperService)
        {
            _webScraperService = webScraperService;
        }

        /// <summary>
        /// Procura por livros.
        /// </summary>
        /// <param name="termoPesquisa">Termo para ser pesquisado</param>
        /// /// <param name="unidadePesquisa">Parâmetro opcional para pesquisa por unidade nas bibliotecas da UEMG</param>
        /// <returns>Returna os livros encontrados a partir do termo de pesquisa informado.</returns>
        /// <response code="200">Returna os livros encontrados a partir do termo de pesquisa informado.</response>
        [HttpGet]
        [Route("/livros/{termoPesquisa}")]
        public string GetBookSearch(string termoPesquisa, [FromQuery] string unidadePesquisa)
        {
            var result = _webScraperService.WebScrapingUemgPergamum(termoPesquisa, SearchType.books, unidadePesquisa);
            return Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        /// <summary>
        /// Procura por artigos.
        /// </summary>
        /// <param name="termoPesquisa">Termo para ser pesquisado</param>
        /// <param name="unidadePesquisa">Parâmetro opcional para pesquisa por unidade nas bibliotecas da UEMG</param>
        /// <returns>Returna os artigos encontrados a partir do termo de pesquisa informado.</returns>
        /// <response code="200">Returna os artigos encontrados a partir do termo de pesquisa informado.</response>
        [HttpGet]
        [Route("/artigos/{termoPesquisa}")]
        public string GetArticleSearch(string termoPesquisa, [FromQuery] string unidadePesquisa)
        {
            var result = _webScraperService.WebScrapingUemgPergamum(termoPesquisa, SearchType.article, unidadePesquisa);
            return Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented);
        }


        /// <summary>
        /// Procura por Monografias e Trabalhos.
        /// </summary>
        /// <param name="termoPesquisa">Termo para ser pesquisado</param>
        /// <param name="unidadePesquisa">Parâmetro opcional para pesquisa por unidade nas bibliotecas da UEMG</param>
        /// <returns>Returna os Monografias e Trabalhos encontrados a partir do termo de pesquisa informado.</returns>
        /// <response code="200">Returna os monografias e trabalhos encontrados a partir do termo de pesquisa informado.</response>
        [HttpGet]
        [Route("/monografias/{termoPesquisa}")]
        public string GetMonographsSearch(string termoPesquisa, [FromQuery] string unidadePesquisa)
        {
            var result = _webScraperService.WebScrapingUemgPergamum(termoPesquisa, SearchType.monographs, unidadePesquisa);
            return Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        /// <summary>
        /// Procura geral.
        /// </summary>
        /// <param name="termoPesquisa">Termo para ser pesquisado</param>
        /// <param name="unidadePesquisa">Parâmetro opcional para pesquisa por unidade nas bibliotecas da UEMG</param>
        /// <returns>Returna os todos os tipos de obra encontrados a partir do termo de pesquisa informado.</returns>
        /// <response code="200">Returna os todos os tipos de obra encontrados a partir do termo de pesquisa informado.</response>
        [HttpGet]
        [Route("/geral/{termoPesquisa}")]
        public string GetSearch(string termoPesquisa, [FromQuery] string unidadePesquisa)
        {
            var result = _webScraperService.WebScrapingUemgPergamum(termoPesquisa, SearchType.general, unidadePesquisa);
            return Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented);
        }
    }
}
