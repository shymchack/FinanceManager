using FinanceManager.Types.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinanceManager.API.Serialization.Types
{
    public class MoneyOperationViewModel
    {
        public int OperationSettingID { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Description ")]
        public string Description { get; set; }
        [Display(Name = "Initial Amount")]
        public decimal InitialAmount { get; set; }
        [Display(Name = "Is Real")]
        public bool IsReal { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Validity Begin Date")]
        public DateTime ValidityBeginDate { get; set; }
        [Display(Name = "Validity End Date")]
        public DateTime ValidityEndDate { get; set; }
        [Display(Name = "Next Operation Execution Date")]
        public DateTime NextOperationExecutionDate { get; } //TODO: Decide when to set this prop (not sure if BL)
        [Display(Name = "Repetition Unit Quantity")]
        public short RepetitionUnitQuantity { get; set; } //TODO: Rename
        [Display(Name = "Repetition Unit")]
        public PeriodUnit RepetitionUnit { get; set; } //TODO: Rename
        public int AccountID { get; set; }
    }
}