using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PackageTrackerAPI.Emails;
using PackageTrackerAPI.Entities;
using PackageTrackerAPI.Models;
using PackageTrackerAPI.Persistence;
using PackageTrackerAPI.Persistence.Repository;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PackageTrackerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageRepository _repository;
        private readonly IEmailDependency _emailDependency;
        public PackagesController(IPackageRepository repository, IEmailDependency emailDependency)
        {
            _repository = repository;
            _emailDependency = emailDependency;
        }


        /// <summary>
        /// Consulta de todos os pacotes
        /// </summary>
        /// <returns>Todos os pacotes</returns>
        /// <response code="200">Retorna todos os pacotes, sem exibir as atualizações</response>
        /// <response code="500">Ocorreu algum erro</response>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var packages = _repository.GetAll();
                return Ok(packages);

            }
            catch (Exception e )
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error retrieving data from the database - {e.Message}");
            }
        }

        /// <summary>
        /// Cadastro de um Pacote. As informações de "senderName" e "senderEmail" devem estar preenchidas para que as atualizações sejam enviadas por email.
        /// </summary>
        /// <remarks>
        /// {
        ///     "title" : "Pacote de Jogos"  ,
        ///     "weight": 1.8,
        ///     "senderName": "Henrique",
        ///     "senderEmail": "hgomes.andrade@gmail.com"
        /// }
        /// </remarks>
        /// <param name="model">Dados do pacote</param>
        /// <returns>Pacote recém-criado</returns>
        /// <response code="201">Cadastro realizado com sucesso</response>
        /// <response code="400">Dados estão inválidos</response>
        /// <response code="500">Ocorreu algum erro</response>
        [HttpPost]
        public async Task<IActionResult> Post(AddPackageInputModel model)
        {
            try
            {
                var package = new Package(model.Title, model.Weight, model.SenderName, model.SenderEmail);
                _repository.Add(package);

                if (package.SenderEmail != null && package.SenderName != null)
                {
                    var message = _emailDependency
                        .CreateMessage(
                        $"Hi {package.SenderName}, Your Package was dispatched!",
                        $"Your package '{package.Title}' with code {package.Code} was dispatched");
                    _emailDependency.AddSenderToMessage(message, package);
                    await _emailDependency.SendMessage(message);
                }
                return CreatedAtAction("GetByCode", new { code = package.Code }, package);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error retrieving data from the database - {e.Message}");
            }
        }

        /// <summary>
        /// Consulta um pacote filtrando pelo código.
        /// </summary>
        /// <returns>Um pacote de acordo com o código passado</returns>
        /// <param name="code">Código do pacote</param>
        /// <response code="200">Retorna um pacote de acordo com o código filtrado</response>
        /// <response code="404">O pacote com o código fornecido não foi encontrado</response>
        /// <response code="500">Ocorreu algum erro</response>
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
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error retrieving data from the database - {e.Message}");
            }
            
        }

        /// <summary>
        /// Altera informações do pacote como o Título e o Peso. Se o Pacote já tiver alguma atualização, esses campos não podem ser alterados.
        /// As informações de "senderName" e "senderEmail" devem estar preenchidas para que as atualizações sejam enviadas por email.
        /// </summary>
        /// <remarks>
        /// {
        ///     "title": "Pacote De Cartas",
        ///     "weight": 3.4,
        ///     "senderName": "Henrique",
        ///     "senderEmail": "hgomes.andrade@gmail.com"
        /// }
        /// </remarks>
        /// <param name="code">Código do pacote</param>
        /// <param name="model">Dados do pacote</param>
        /// <returns>Atualização de pacote realizada</returns>
        /// <response code="200">Pacote atualizado</response>
        /// <response code="400">O pacote não foi encontrado ou ele já teve alguma atualização, e por isso não pode mais ser alterado</response>
        /// <response code="500">Ocorreu algum erro</response>
        [HttpPut("{code}")]
        public async Task<IActionResult> PutAsync(string code, AddPackageInputModel model)
        {
            try
            {
                var package = _repository.GetByCode(code);

                if (package == null)
                {
                    return NotFound();
                }

                package.UpdatePackage(model.Title, model.Weight, model.SenderName, model.SenderEmail);

                if (package.SenderEmail != null && package.SenderName != null)
                {
                    var message = _emailDependency
                        .CreateMessage(
                        "Your Package was updated",
                        $@"Your package '{package.Title}' with code {package.Code} was updated, his actual data is: 
                               Title: {package.Title}
                               Weight: {package.Weight}
                               SenderEmail: {package.SenderEmail}
                               SenderName: {package.SenderName}");
                    _emailDependency.AddSenderToMessage(message, package);
                    await _emailDependency.SendMessage(message);
                }
                _repository.Update(package);

                return Ok(package);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error updating data - {e.Message}");
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
        /// <param name="code">Código do pacote</param>
        /// <param name="model">Atualizações do pacote</param>
        /// <returns>Atualização adicionada</returns>
        /// <response code="201">Cadastro realizado com sucesso</response>
        /// <response code="400">Dados estão inválidos ou o pacote já foi entregue, portanto não pode ter novas atualizações</response>
        /// <response code="500">Ocorreu algum erro</response>
        [HttpPost("{code}/Updates")]
        public async Task<IActionResult> PostUpdateAsync(string code, AddPackageUpdateInputModel model)
        {
            try
            {
                var package = _repository.GetByCode(code);

                if (package == null)
                {
                    return NotFound();
                }

                var packageUpdate = package.AddUpdate(model.Status, model.Delivered);

                if (package.SenderEmail != null && package.SenderName != null)
                {

                    if (package.Delivered)
                    {
                        var message = _emailDependency
                        .CreateMessage(
                        "Your Package was delivered",
                         $"Your package '{package.Title}' with code {package.Code} was delivered!!");
                        _emailDependency.AddSenderToMessage(message, package);
                        await _emailDependency.SendMessage(message);
                    }
                    else
                    {
                        var message = _emailDependency
                        .CreateMessage(
                        "Your Package has a new Update",
                         $"Your package '{package.Title}' with code {package.Code} has a new update: {packageUpdate.Status}");
                        _emailDependency.AddSenderToMessage(message, package);
                        await _emailDependency.SendMessage(message);
                    }
                }

                _repository.Update(package);

                return CreatedAtAction("GetByCode", new { code = package.Code }, package);

            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error retrieving data from the database - {e.Message}");
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
        /// <param name="code">Código do pacote</param>
        /// <param name="updateId">Id de uma atualização do pacote</param>
        /// <param name="model">Dados do pacote</param>
        /// <returns>Atualização de pacote adicionada</returns>
        /// <response code="200">Pacote atualizado</response>
        /// <response code="400">O pacote ou a atualização não foram encontrados</response>
        /// <response code="500">Ocorreu algum erro</response>
        [HttpPut("{code}/Updates/{updateId}")]
        public async Task<IActionResult> PutUpdatesAsync(string code, int updateId, AddPackageUpdateInputModel model)
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

                if (package.SenderEmail != null && package.SenderName != null)
                {
                    if (package.Delivered)
                    {
                        var message = _emailDependency
                        .CreateMessage(
                        "Your Package was delivered",
                        $"Your package '{package.Title}' with code {package.Code} was delivered!!");
                        _emailDependency.AddSenderToMessage(message, package);
                        await _emailDependency.SendMessage(message);
                    } 
                }
                _repository.Update(package);

                return Ok(package);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error updating data - {e.Message}");
            }

        }

        /// <summary>
        /// Apaga um pacote e suas atualizações
        /// </summary>
        /// <param name="code">Código do pacote</param>
        /// <returns>Pacote apagado</returns>
        /// <response code="204">Pacote Removido</response>
        /// <response code="400">O pacote não foi encontrado</response>
        /// <response code="500">Ocorreu algum erro</response>
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
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error deleting data - {e.Message}");
            }
        }
    }
}
