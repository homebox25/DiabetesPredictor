using ClosedXML.Excel;
using DiabetesPredictor.Pages.Data;
using DiabetesPredictor.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiabetesPredictorRazor.Pages
{
    public class UploadModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UploadModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IFormFile UploadFile { get; set; }

        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync(IFormFile UploadFile)
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
                    // Process Excel file
                    using (var stream = new MemoryStream())
                    {
                        await UploadFile.CopyToAsync(stream);
                        using (var workbook = new XLWorkbook(stream))
                        {
                            var worksheet = workbook.Worksheet(1);
                            foreach (var row in worksheet.RowsUsed().Skip(1))
                            {
                                var patientName = row.Cell(1).GetString();
                                var riskScore = row.Cell(2).GetDouble();
                            }
                        }
                    }
                }
                else if (extension == ".csv")
                {
                    // Process CSV file
                    using (var reader = new StreamReader(UploadFile.OpenReadStream()))
                    {
                        string headerLine = await reader.ReadLineAsync(); // skip header
                        while (!reader.EndOfStream)
                        {
                            var line = await reader.ReadLineAsync();
                            var values = line.Split(',');

                            var patient = new Patient
                            {
                                PatientId = int.Parse(values[0]),
                                Name = values[1],
                                Age = int.Parse(values[2]),
                                Gender = char.Parse(values[3]),
                                BMI = float.Parse(values[4]),
                                Glucose = float.Parse(values[5]),
                                Insulin = int.Parse(values[6]),
                                BloodPressure = int.Parse(values[7]),
                                DiabetesPedigree = float.Parse(values[8]),
                                PhysicalHours = int.Parse(values[9])
                            };


                            // Save to DB or process
                            _context.Patients.Add(patient);
                            _context.SaveChanges();
                        }
                    }
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

    }
}
