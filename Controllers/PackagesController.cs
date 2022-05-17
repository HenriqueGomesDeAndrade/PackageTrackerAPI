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
            try
            {
                var packages = _repository.GetAll();
                return Ok(packages);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
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
            try
            {
                var package = new Package(model.Title, model.Weight);
                _repository.Add(package);

                return CreatedAtAction("GetByCode", new { code = package.Code }, package);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
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
            try
            {
                var package = _repository.GetByCode(code);

                if (package == null)
                {
                    return NotFound();
                }
                return Ok(package);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
            
        }

        /// <summary>
        /// Altera informações do pacote como o Título e o Peso. Se o Pacote já tiver alguma atualização, esses campos não podem ser alterados.
        /// </summary>
        /// <remarks>
        /// {
        ///     "title": "Pacote De Cartas",
        ///     "weight": 3.4
        /// }
        /// </remarks>
        /// <param name="model">Atualizações do pacote</param>
        /// <returns>Atualização adicionada</returns>
        /// <response code="200">Pacote atualizado</response>
        /// <response code="400">O pacote não foi encontrado ou ele já teve alguma atualização, e por isso não pode mais ser alterado</response>
        [HttpPut("{code}")]
        public IActionResult Put(string code, AddPackageInputModel model)
        {
            try
            {
                var package = _repository.GetByCode(code);

                if (package == null)
                {
                    return NotFound();
                }

                package.UpdatePackage(model.Title, model.Weight);
                _repository.Update(package);

                return Ok(package);

            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }

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
        [HttpPost("{code}/Updates")]
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        /// <summary>
        /// Altera informação do status de uma atualização do pacote ou altera se o pacote já foi entregue ou não.
        /// </summary>
        /// <remarks>
        /// {
        ///     "status": "Coletado pela Transportadora",
        ///     "delivered": false
        /// }
        ///
        /// </remarks>
        /// <param name="model">Atualizações do pacote</param>
        /// <returns>Atualização adicionada</returns>
        /// <response code="200">Pacote atualizado</response>
        /// <response code="400">O pacote ou a atualização não foram encontrados</response>
        [HttpPut("{code}/Updates/{updateId}")]
        public IActionResult PutUpdates(string code, int updateId, AddPackageUpdateInputModel model)
        {
            try
            {
                var package = _repository.GetByCode(code);


                if (package == null)
                {
                    return NotFound();
                }

                var packageUpdate = _repository.GetByUpdateId(package, updateId);

                if (packageUpdate == null)
                {
                    return NotFound();
                }

                package.UpdatePackage(model.Delivered);
                packageUpdate.UpdatePackageUpdateStatus(model.Status);

                _repository.Update(package);

                return Ok(package);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }

        }


        /// <summary>
        /// Apaga um pacote e suas atualizações
        /// </summary>
        /// <param name="model">Deleção do pacote</param>
        /// <returns>Remoção concluída</returns>
        /// <response code="204">Pacote Removido</response>
        /// <response code="400">O pacote não foi encontrado</response>
        [HttpDelete("{code}")]
        public IActionResult Delete(string code)
        {
            try
            {
                var package = _repository.GetByCode(code);

                if (package == null)
                {
                    return NotFound();
                }

                _repository.Remove(package);

                return NoContent();

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data database");
            }
        }
    }
}
