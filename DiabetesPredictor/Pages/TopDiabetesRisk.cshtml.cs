using Dapper; 
using DiabetesPredictor.Pages.Data;
using DiabetesPredictor.Pages.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
//using Microsoft.EntityFrameworkCore;

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


            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetTop5AtRiskPatients", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TopPatients.Add(new Patient
                        {
                            PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            Glucose = reader.GetDouble(reader.GetOrdinal("Glucose")),
                            BMI = reader.GetDouble(reader.GetOrdinal("BMI"))
                        });
                    }
                }
            }
        }
    }
}
