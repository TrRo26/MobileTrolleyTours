using System;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using MobileTrolleyTours.Models;
using MobileTrolleyTours.Models.Enums;

namespace MobileTrolleyTours.Data
{
	public static class ScheduleAlertService
	{
        static ScheduleAlertService()
		{
            var storageConfig = new AzureStorageConfig();
            TableStorageService.Initialize(storageConfig.TableTourScheduleChanges);
        }

        #region Public Methods

        public static ScheduleChangeData GetAlertBoxHeader()
        {
            var alertBoxHeader = GetScheduleChangeData(PartitionKeys.AlertBoxHeader, ScheduleAlertStatus.Active);

            return alertBoxHeader.FirstOrDefault();
        }

        public static ScheduleChangeData GetAlertBoxSubHeader()
        {
            var alertBoxSubHeader = GetScheduleChangeData(PartitionKeys.AlertBoxSubHeader, ScheduleAlertStatus.Active);

            return alertBoxSubHeader.FirstOrDefault();
        }

        public static List<ScheduleChangeData> GetActiveAlerts()
        {
            var alerts = GetScheduleChangeData(PartitionKeys.AlertBoxItem, ScheduleAlertStatus.Active);

            return alerts;
        }

        public static ScheduleChangeData GetAlertBoxFooter()
        {
            var alertBoxFooter = GetScheduleChangeData(PartitionKeys.AlertBoxFooter, ScheduleAlertStatus.Active);

            return alertBoxFooter.FirstOrDefault();
        }

        public static IOrderedEnumerable<ScheduleChangeData> GetAllAlerts()
        {
            var alerts = GetScheduleChangeData(PartitionKeys.AlertBoxItem);

            var sortedAlerts = alerts.OrderByDescending(d => d.StartDate);

            return sortedAlerts;
        }

        public static string AddAlert(ScheduleChangeData changeData)
        {
            var alertKey = AddScheduleChangeData(PartitionKeys.AlertBoxItem, changeData);

            return alertKey;
        }

        #endregion

        #region Private Methods

        private static List<ScheduleChangeData> GetScheduleChangeData(PartitionKeys partitionKey, ScheduleAlertStatus? alertStatus = null)
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

                if ((alertStatus == null || alert.Status == alertStatus) && alert.Status != ScheduleAlertStatus.Deleted)
                {
                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        private static string AddScheduleChangeData(PartitionKeys partitionKey, ScheduleChangeData changeData)
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

                Console.WriteLine($"Table entity with partition key {partitionKey.ToString()} " +
                                  $"and row key {rowKey} was added successfully.");
            }
            catch (Exception)
            {
                return Guid.Empty.ToString();
            }

            return rowKey;
        }

        #endregion
    }
}
