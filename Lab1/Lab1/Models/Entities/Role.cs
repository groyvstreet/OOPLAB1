﻿using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class Role
    {
        [Key] public string? Id { get; set; }
        public string? Name { get; set; }
        public List<User> Users { get; set; } = new List<User>();
    }
}
