using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.Model
{
    public class Patient
    {
        public Patient()
        {

        }

        public Patient(Guid id)
        {
            Id = id;
            IsActive = true;
            IsAnimal = false;
            IsDeceased = false;
            PatientIdentifier = new PatientIdentifier(Id);
            PatientContact = new PatientContact(Id);
        }

        [Required]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string FamilyName { get; set; }

        [StringLength(50)]
        public string GivenName { get; set; }

        public bool IsActive { get; set; }
        
        [StringLength(20)]
        public string HomePhone { get; set; }

        [StringLength(20)]
        public string MobilePhone { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        public string PatientGender { get; set; }

        public DateTime? Birthdate { get; set; }
        
        public bool IsDeceased { get; set; }
        
        [StringLength(50)]
        public string StreetAddress { get; set; }
       
        [StringLength(50)]
        public string City { get; set; }
        
        [StringLength(30)]
        public string State { get; set; }
        
        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        public string PatientMaritalStatus { get; set; }

        public bool IsAnimal { get; set; }
        
        [StringLength(30)]
        public string AnimalSpecies { get; set; }
        
        [StringLength(30)]
        public string AnimalBreed { get; set; }
        
        public string AnimalGenderStatus { get; set; }
        
        public PatientContact PatientContact { get; set; }

        public PatientIdentifier PatientIdentifier { get; set; }
    }
}
