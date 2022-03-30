﻿using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities.Actions
{
    public class Action
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Time { get; set; } = DateTime.Now;
        public string Info { get; set; }
        public bool Canceled { get; set; }
    }
}