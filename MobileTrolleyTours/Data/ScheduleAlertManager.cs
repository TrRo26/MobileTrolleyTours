using System;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using MobileTrolleyTours.Models;

namespace MobileTrolleyTours.Data
{
	public class ScheduleAlertManager
	{
        private readonly string PK_ALERT_BANNER = "AlertBanner";
        private readonly string PK_ALERT_ITEM = "AlertItem";

        public ScheduleAlertManager()
		{
            var storageConfig = new AzureStorageConfig();
            TableStorageManager.Initialize(storageConfig.TableTourScheduleChanges);
        }

        public List<ScheduleAlertDetails> GetScheduleAlertsByPartitionKey(string partitionKey)
        {
            var alerts = new List<ScheduleAlertDetails>();
            var entities = TableStorageManager.GetEntitiesByPartitionKey(partitionKey);

            foreach (TableEntity entity in entities)
            {
                entity.TryGetValue("StartDate", out object startDate);
                entity.TryGetValue("EndDate", out object endDate);
                entity.TryGetValue("Description", out object description);
                entity.TryGetValue("ApplyDate", out object applyDate);
                entity.TryGetValue("RevokeDate", out object revokeDate);
                entity.TryGetValue("Status", out object status);

                var alert = new ScheduleAlertDetails
                {
                    StartDate = (DateTimeOffset)startDate,
                    EndDate = endDate != null ? (DateTimeOffset)endDate : null,
                    Description = description != null ? (string)description : null,
                    ApplyDate = applyDate != null ? (DateTimeOffset)applyDate: null,
                    RevokeDate = revokeDate != null ? (DateTimeOffset)revokeDate: null,
                    Status = (ScheduleAlertStatus)status
                };

                alerts.Add(alert);
            }

            return alerts;
        }

        public string AddScheduleAlertItem(ScheduleAlertDetails alertDetails)
        {
            var rowKey = Guid.NewGuid().ToString();

            try
            {
                TableEntity entity = new TableEntity(PK_ALERT_ITEM, rowKey)
                {
                    { nameof(alertDetails.StartDate), alertDetails.StartDate },
                    { nameof(alertDetails.EndDate), alertDetails.EndDate },
                    { nameof(alertDetails.Description), alertDetails.Description },
                    { nameof(alertDetails.ApplyDate), alertDetails.ApplyDate },
                    { nameof(alertDetails.RevokeDate), alertDetails.RevokeDate },
                    { nameof(alertDetails.Status), (int)alertDetails.Status },
                };

                TableStorageManager.AddEntity(entity);
            }
            catch (Exception)
            {
                return Guid.Empty.ToString();
            }

            return rowKey;
        }
    }
}
