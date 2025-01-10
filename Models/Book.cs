using System;
using System.ComponentModel.DataAnnotations;

namespace BookManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title can't be longer than 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100, ErrorMessage = "Author name can't be longer than 100 characters.")]
        public string Author { get; set; }

        [DataType(DataType.Date)]
        [CustomValidation(typeof(Book), nameof(ValidatePublicationDate))]
        public DateTime PublicationDate { get; set; } = DateTime.MinValue;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be between $0.01 and $1000.")]
        public decimal Price { get; set; }

        // Custom validation method for PublicationDate
        public static ValidationResult ValidatePublicationDate(DateTime publicationDate, ValidationContext context)
        {
            if (publicationDate > DateTime.Now)
            {
                return new ValidationResult("Publication date cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
