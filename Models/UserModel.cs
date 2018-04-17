using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank_Accounts.Models
{
    public abstract class BaseEntity {
        [Key]
        public int UserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class User : BaseEntity
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [Display(Name = "Current Balance: ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C2}")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "You do not have enough funds to make the Withdrawal!")]
        public double Balance { get; set; } = 0.00;

        public List<Transaction> Transactions { get; set; }
 
        public User()
        {
            Transactions = new List<Transaction>();
        }
    }
}