﻿namespace Lab1.Models.Entities.Actions
{
    public class SalaryApprovingByOperatorAction : Action
    {
        public string ClientId { get; set; }
        public string ClientEmail { get; set; }
        public string CompanyName { get; set; }
    }
}
