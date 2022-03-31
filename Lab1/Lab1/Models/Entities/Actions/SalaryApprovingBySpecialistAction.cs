﻿namespace Lab1.Models.Entities.Actions
{
    public class SalaryApprovingBySpecialistAction : Action
    {
        public string ClientId { get; set; }
        public string ClientEmail { get; set; }
        public string CompanyName { get; set; }
        public bool ApprovedByOperator { get; set; }
    }
}
