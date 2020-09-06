using System;
using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models
{
    public class Contact
    {
        public int Contactid { get; set; }

        [Required, MinLength(2), MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MinLength(2), MaxLength(100)]
        public string LastName { get; set; }

        [MinLength(2), MaxLength(100)]
        public string City { get; set; }

        [MinLength(2), MaxLength(100)]
        public string State { get; set; }

        [Required, MinLength(6), MaxLength(7)]
        public string Zip { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}