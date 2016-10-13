using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.Model
{
    public class PatientContact
    {
        public PatientContact()
        {

        }

        public PatientContact(Guid patientId)
        {
            this.Id = Guid.NewGuid();
            this.PatientId = patientId;
        }

        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        public string Relationship { get; set; }

        [StringLength(50)]
        public string FamilyName { get; set; }

        [StringLength(50)]
        public string GivenName { get; set; }

        [StringLength(20)]
        public string HomePhone { get; set; }

        [StringLength(20)]
        public string MobilePhone { get; set; }

        [Required]
        public Patient Patient { get; set; }
    }
}
