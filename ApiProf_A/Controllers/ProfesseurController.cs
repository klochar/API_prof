using ApiProf_A.Models;
using ApiProf_A.Services;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System.Configuration;
using System.Data;
using Dapper;

namespace ApiProf_A.Controllers
{
    [Route("api/[controller]")] //  /api/professeur
    [ApiController]
    public class ProfesseurController : Controller
    {
        private readonly ExcelService _excelService;
        private readonly IConfiguration _configuration;//faire la connexion MYSQL et ensuite integrer dans windows server
        private static List<Professeur> _professeurs = new List<Professeur>();
        public ProfesseurController(ExcelService excelService, IConfiguration configuration)
        {
            //injecte service
            _excelService = excelService;
            _configuration = configuration;
        }

        [HttpPost("upload")] ///api/professeur/upload
        public IActionResult Upload([FromBody] List<Professeur> professeurs)
        {
            if (professeurs == null)
            {
                return BadRequest("Professeurs list is null");
            }

            _professeurs.AddRange(professeurs);

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO prof (idProf,prenomProf, nomProf, departement) VALUES (@IdProf,@PrenomProf, @NomProf, @Departement)";
                    connection.Execute(insertQuery, professeurs);

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur s'est produite: {ex.Message}");
            }
        }


        [HttpGet("export")] ///api/professeur/export
        public IActionResult Export()
        {
            var fileContents = _excelService.GenerateExcel(_professeurs);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Professeurs.xlsx");
        }

        [HttpGet("professeurs")]
        public IActionResult GetProfesseurs()
        {
            List<Professeur> professeurs = new List<Professeur>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT idprof, prenomProf, nomProf, departement FROM prof";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Professeur professeur = new Professeur
                            {
                                idProf = reader.GetInt32("idprof").ToString(),
                                prenomProf = reader.GetString("prenomProf"),
                                nomProf = reader.GetString("nomProf"),
                                departement = reader.GetString("departement")
                            };
                            professeurs.Add(professeur);
                        }
                    }
                }
            }

            return Ok(professeurs);
        }

        [HttpDelete("/api/professeur/{id}")]
        public async Task<IActionResult> DeleteProfBaseDonne_(string id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new MySqlCommand("DELETE FROM PROF WHERE idProf = @idProf", connection);
                    command.Parameters.AddWithValue("@idProf", id);
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return Ok($"Le professeur avec l'ID {id} a été supprimé.");
                    }
                    else
                    {
                        return NotFound($"Le professeur avec l'ID {id} n'a pas été trouvé.");//ne devra jamais etre, car on le fait avec le bouton liee a ID
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des professeurs: {ex.Message}");
            }
        }

        [HttpGet("/api/professeur/liste")]
        public async Task<IActionResult> GetProfesseurs_()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new MySqlCommand("SELECT * FROM prof", connection);
                    var reader = await command.ExecuteReaderAsync();

                    var professeurs = new List<Professeur>();

                    while (await reader.ReadAsync())
                    {
                        Professeur professeur = new Professeur
                        {
                            prenomProf = reader.GetString("prenomProf"),
                            nomProf = reader.GetString("nomProf"),
                            idProf = reader.GetInt32("idprof").ToString(),
                            departement = reader.GetString("departement"),
                        };
                        professeurs.Add(professeur);
                    }

                    return Ok(professeurs);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des professeurs: {ex.Message}");
            }
        }

        [HttpDelete("deleteTableau")]//delete tableau Local et non Base de donne
        public IActionResult SupprimerTousLesProfesseurs()
        {
            try
            {
                _professeurs = new List<Professeur>();
                return Ok("tableau vide");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression des professeurs: {ex.Message}");
            }
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }

        /*        private readonly ActiveDirectoryService _adService;
        */
        /*        private readonly List<Professeur> _professeurs;
        */
        /*public ProfesseurController(ActiveDirectoryService adService)
        {
            _adService = adService;
            _professeurs = new List<Professeur>();
        }*/

        /* public ProfesseurController()
         {
             _professeurs = new List<Professeur>();
         }*/



        /*
                [HttpPost("ajouter")]
                public IActionResult AjouterProfesseur([FromBody] Professeur professeur)
                {
                    _professeurs.Add(professeur);
                    return Ok(new { message = "Professeur ajouté avec succès !" });
                }*/

        /*[HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            var professors = new List<Professeur>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var professor = new Professeur
                        {
                            prenomProf = worksheet.Cells[row, 1].Text,
                            nomProf = worksheet.Cells[row, 2].Text,
                            idProf = worksheet.Cells[row, 3].Text,
                            departement = worksheet.Cells[row, 4].Text,
                        };
                        professors.Add(professor);
                        //_adService.AddProfessor(professor);
                    }
                }
            }

            return Ok(professors);
        }*/
    }
}
