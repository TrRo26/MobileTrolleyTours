using System;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using MobileTrolleyTours.Models;
using MobileTrolleyTours.Models.Enums;

namespace MobileTrolleyTours.Data
{
	public class ScheduleAlertService
	{
        public ScheduleAlertService()
		{
            var storageConfig = new AzureStorageConfig();
            TableStorageService.Initialize(storageConfig.TableTourScheduleChanges);
        }

        public List<ScheduleChangeData> GetScheduleChangeData(PartitionKeys partitionKey, ScheduleAlertStatus? alertStatus = null)
        {
            var alerts = new List<ScheduleChangeData>();
            var entities = TableStorageService.GetEntitiesByPartitionKey(partitionKey);

            foreach (TableEntity entity in entities)
            {
                entity.TryGetValue("StartDate", out object startDate);
                entity.TryGetValue("EndDate", out object endDate);
                entity.TryGetValue("Description", out object description);
                entity.TryGetValue("ApplyDate", out object applyDate);
                entity.TryGetValue("RevokeDate", out object revokeDate);
                entity.TryGetValue("Status", out object status);

                var alert = new ScheduleChangeData
                {
                    StartDate = startDate != null ? (DateTimeOffset)startDate : null,
                    EndDate = endDate != null ? (DateTimeOffset)endDate : null,
                    Description = description != null ? (string)description : null,
                    ApplyDate = applyDate != null ? (DateTimeOffset)applyDate: null,
                    RevokeDate = revokeDate != null ? (DateTimeOffset)revokeDate: null,
                    Status = (ScheduleAlertStatus)status
                };

                if (alertStatus == null || alert.Status == alertStatus)
                {
                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public string AddScheduleChangeData(PartitionKeys partitionKey, ScheduleChangeData changeData)
        {
            var rowKey = Guid.NewGuid().ToString();

            try
            {
                TableEntity entity = new TableEntity(partitionKey.ToString(), rowKey)
                {
                    { nameof(changeData.StartDate), changeData.StartDate },
                    { nameof(changeData.EndDate), changeData.EndDate },
                    { nameof(changeData.Description), changeData.Description },
                    { nameof(changeData.ApplyDate), changeData.ApplyDate },
                    { nameof(changeData.RevokeDate), changeData.RevokeDate },
                    { nameof(changeData.Status), (int)changeData.Status },
                };

                TableStorageService.AddEntity(entity);
            }
            catch (Exception)
            {
                return Guid.Empty.ToString();
            }

            return rowKey;
        }
    }
}
