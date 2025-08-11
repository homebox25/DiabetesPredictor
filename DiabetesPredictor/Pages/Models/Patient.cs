
using System.ComponentModel.DataAnnotations.Schema;

namespace DiabetesPredictor.Pages.Models
{
    [Table("Patients")]
    public class Patient
    {
        public int PatientId { get; set; }
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public char Gender { get; set; }
        public double BMI { get; set; }
        public double Glucose { get; set; }
        public int Insulin { get; set; }
        public int BloodPressure { get; set; }
        public double DiabetesPedigree { get; set; }
        public int PhysicalHours { get; set;}

        //public static implicit operator Patient(Patient v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
