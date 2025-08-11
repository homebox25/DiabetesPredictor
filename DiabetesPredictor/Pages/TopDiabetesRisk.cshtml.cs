using Dapper; 
using DiabetesPredictor.Pages.Data;
using DiabetesPredictor.Pages.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DiabetesPredictorRazor.Pages
{
    public class TopDiabetesRiskModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString = "Server=np:\\\\.\\pipe\\LOCALDB#EDC036FF\\tsql\\query;Database=Patients;Trusted_Connection=True;";

        public List<Patient> HighRiskPatients { get; set; }


        public TopDiabetesRiskModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Patient> TopPatients { get; set; } = new List<Patient>();

        public void OnGet()
        {

            TopPatients = _context.Patients
            .FromSqlRaw("EXEC GetTop5AtRiskPatients")
            .ToList();

            // Ensure it's never null
            if (TopPatients == null)
                TopPatients = new List<Patient>();
        }
    }
}
