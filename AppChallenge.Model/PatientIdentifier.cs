using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.Model
{
    public class PatientIdentifier
    {
        public PatientIdentifier()
        {

        }

        public PatientIdentifier(Guid patientId)
        {
            this.Id = Guid.NewGuid();
            this.PatientId = patientId;
        }

        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [StringLength(15)]
        public string IdentifierUseValue { get; set; }

        [StringLength(10)]
        public string IdentifierTypeCodeValue { get; set; }

        [StringLength(20)]
        public string IdentifierTypeTextValue { get; set; }

        [StringLength(100)]
        public string IdentifierSystemValue { get; set; }

        [StringLength(50)]
        public string IdentifierValue { get; set; }

        [StringLength(100)]
        public string AssignerDisplayValue { get; set; }

        [Required]
        public Patient Patient { get; set; }

        public IEnumerator<PatientIdentifier> GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
