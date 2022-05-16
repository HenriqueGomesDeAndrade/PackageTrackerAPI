using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PackageTrackerAPI.Entities;
using PackageTrackerAPI.Models;
using PackageTrackerAPI.Persistence;
using PackageTrackerAPI.Persistence.Repository;

namespace PackageTrackerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageRepository _repository;
        public PackagesController(IPackageRepository repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// Consulta de todos os pacotes
        /// </summary>
        /// <returns>OTodos os pacotes</returns>
        /// <response code="200">Retorna todos os pacotes, sem exibir as atualizações</response>
        [HttpGet]
        public IActionResult GetAll()
        {
            var packages = _repository.GetAll();
            return Ok(packages);
        }



        /// <summary>
        /// Cadastro de um Pacote
        /// </summary>
        /// <remarks>
        /// {
        ///     "title" : "Pacote de Jogos"  ,
        ///     "weight": 1.8
        /// }
        /// </remarks>
        /// <param name="model">Dados do pacote</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="201">Cadastro realizado com sucesso</response>
        /// <response code="400">Dados estão inválidos</response>
        [HttpPost]
        public IActionResult Post(AddPackageInputModel model)
        {
            var package = new Package(model.Title, model.Weight);
            _repository.Add(package);

            return CreatedAtAction("GetByCode", new { code = package.Code }, package);
        }

        /// <summary>
        /// Consulta um pacote filtrando pelo código.
        /// </summary>
        /// <returns>Um pacote</returns>
        /// <response code="200">Retorna um pacote de acordo com o código filtrado</response>
        /// <response code="404">O pacote com o código fornecido não foi encontrado</response>
        [HttpGet("{code}")]
        public IActionResult GetByCode(string code)
        {
            var package = _repository.GetByCode(code);

            if (package == null)
            {
                return NotFound();
            }
            return Ok(package);
        }

        /// <summary>
        /// Adiciona uma atualização de um pacote, de acordo com o código passado.
        /// </summary>
        /// <remarks>
        /// {
        ///     "status": "Entregue á transportadora",
        ///     "delivered": false
        /// }
        /// </remarks>
        /// <param name="model">Atualizações do pacote</param>
        /// <returns>Atualização adicionada</returns>
        /// <response code="201">Cadastro realizado com sucesso</response>
        /// <response code="400">Dados estão inválidos ou o pacote já foi entregue, portanto não pode ter novas atualizações</response>
    [HttpPost("{code}/update")]
        public IActionResult PostUpdate(string code, AddPackageUpdateInputModel model)
        {
            try
            {
                var package = _repository.GetByCode(code);

                if (package == null)
                {
                    return NotFound();
                }

                package.AddUpdate(model.Status, model.Delivered);

                _repository.Update(package);

                return CreatedAtAction("GetByCode", new { code = package.Code }, package);

            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
