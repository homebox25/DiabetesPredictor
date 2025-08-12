using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DiabetesPredictorRazor.Pages
{
    public class UploadModel : PageModel
    {
        private readonly string _connectionString;

        public UploadModel(IConfiguration configuration)
        {
            // Pull connection string directly from appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [BindProperty]
        public IFormFile UploadFile { get; set; }

        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (UploadFile == null || UploadFile.Length == 0)
            {
                Message = "Please select a file.";
                return Page();
            }

            var extension = Path.GetExtension(UploadFile.FileName).ToLower();

            try
            {
                if (extension == ".xlsx")
                {
                    await ProcessExcelFile(UploadFile);
                }
                else if (extension == ".csv")
                {
                    await ProcessCsvFile(UploadFile);
                }
                else
                {
                    Message = "Only .xlsx or .csv files are allowed.";
                    return Page();
                }

                Message = "File uploaded and processed successfully!";
            }
            catch (Exception ex)
            {
                Message = $"Error processing file: {ex.Message}";
            }

            return Page();
        }

        private async Task ProcessExcelFile(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        foreach (var row in worksheet.RowsUsed().Skip(1)) // skip header row
                        {
                            using (var cmd = new SqlCommand("InsertPatient", connection))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@PatientId", int.Parse(row.Cell(1).GetString()));
                                cmd.Parameters.AddWithValue("@Name", row.Cell(2).GetString());
                                cmd.Parameters.AddWithValue("@Age", int.Parse(row.Cell(3).GetString()));
                                cmd.Parameters.AddWithValue("@Gender", row.Cell(4).GetString());
                                cmd.Parameters.AddWithValue("@BMI", float.Parse(row.Cell(5).GetString()));
                                cmd.Parameters.AddWithValue("@Glucose", float.Parse(row.Cell(6).GetString()));
                                cmd.Parameters.AddWithValue("@Insulin", int.Parse(row.Cell(7).GetString()));
                                cmd.Parameters.AddWithValue("@BloodPressure", int.Parse(row.Cell(8).GetString()));
                                cmd.Parameters.AddWithValue("@DiabetesPedigree", float.Parse(row.Cell(9).GetString()));
                                cmd.Parameters.AddWithValue("@PhysicalHours", int.Parse(row.Cell(10).GetString()));

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
        }

        private async Task ProcessCsvFile(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string headerLine = await reader.ReadLineAsync(); // skip header

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        var values = line.Split(',');

                        using (var cmd = new SqlCommand("InsertPatient", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@PatientId", int.Parse(values[0]));
                            cmd.Parameters.AddWithValue("@Name", values[1]);
                            cmd.Parameters.AddWithValue("@Age", int.Parse(values[2]));
                            cmd.Parameters.AddWithValue("@Gender", values[3]);
                            cmd.Parameters.AddWithValue("@BMI", float.Parse(values[4]));
                            cmd.Parameters.AddWithValue("@Glucose", float.Parse(values[5]));
                            cmd.Parameters.AddWithValue("@Insulin", int.Parse(values[6]));
                            cmd.Parameters.AddWithValue("@BloodPressure", int.Parse(values[7]));
                            cmd.Parameters.AddWithValue("@DiabetesPedigree", float.Parse(values[8]));
                            cmd.Parameters.AddWithValue("@PhysicalHours", int.Parse(values[9]));

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }
    }
}
