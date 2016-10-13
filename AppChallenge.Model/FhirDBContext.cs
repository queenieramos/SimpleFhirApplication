using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.Model
{
    public class FhirDBContext : DbContext
    {

        public FhirDBContext() : base("FhirSampleDB")
        {
        }

        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientContact> PatientContacts { get; set; }
        public virtual DbSet<PatientIdentifier> PatientIdentifiers { get; set; }
    }
}
