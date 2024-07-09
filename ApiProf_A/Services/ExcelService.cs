using ApiProf_A.Models;
using OfficeOpenXml;

namespace ApiProf_A.Services
{
    public class ExcelService
    {
        public byte[] GenerateExcel(List<Professeur> professeurs)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Professeurs");

                // Add header
                worksheet.Cells[1, 1].Value = "Prénom";
                worksheet.Cells[1, 2].Value = "Nom";
                worksheet.Cells[1, 3].Value = "Identifiant";
                worksheet.Cells[1, 4].Value = "Département";

                // Add data
                for (int i = 0; i < professeurs.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = professeurs[i].prenomProf;
                    worksheet.Cells[i + 2, 2].Value = professeurs[i].nomProf;
                    worksheet.Cells[i + 2, 3].Value = professeurs[i].idProf;
                    worksheet.Cells[i + 2, 4].Value = professeurs[i].departement;
                }

                return package.GetAsByteArray();
            }
        }
    }
}
