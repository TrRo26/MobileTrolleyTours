using System;
using Azure;
using Azure.Data.Tables;
using MobileTrolleyTours.Models.Enums;

namespace MobileTrolleyTours.Models
{
	public class ScheduleChangeData
    {
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? ApplyDate { get; set; }
        public DateTimeOffset? RevokeDate { get; set; }
        public ScheduleAlertStatus Status { get; set; }
    }
}
