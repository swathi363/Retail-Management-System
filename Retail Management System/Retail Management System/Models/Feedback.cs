﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Retail_Management_System.Models
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackId { get; set; }
        [Required]
        public string ProductId { get; set; }
        [Required]
        public string FeedBack { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }
        public virtual Product product { get; set; }
        public virtual User user { get; set; }
    }
}