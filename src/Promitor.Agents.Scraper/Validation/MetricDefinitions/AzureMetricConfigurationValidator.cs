﻿using System.Collections.Generic;
using Promitor.Core.Scraping.Configuration.Model;
using Promitor.Core.Scraping.Configuration.Model.Metrics;
using ResourceType = Promitor.Core.Contracts.ResourceType;

namespace Promitor.Agents.Scraper.Validation.MetricDefinitions
{
    public class AzureMetricConfigurationValidator
    {
        private readonly MetricDefaults _metricDefaults;

        public AzureMetricConfigurationValidator(MetricDefaults metricDefaults)
        {
            _metricDefaults = metricDefaults;
        }

        public IEnumerable<string> Validate(MetricDefinition metrics)
        {
            if (metrics.ResourceType == ResourceType.LogAnalytics)
            {
                return ValidateLogAnalyticsConfiguration(metrics.LogAnalyticsConfiguration);
            }

            return ValidateAzureMetricConfiguration(metrics.AzureMetricConfiguration);
        }

        private IEnumerable<string> ValidateAzureMetricConfiguration(AzureMetricConfiguration azureMetricConfiguration)
        {
            var errorMessages = new List<string>();

            if (azureMetricConfiguration == null)
            {
                errorMessages.Add("Invalid azure metric configuration is configured");
                return errorMessages;
            }

            if (string.IsNullOrWhiteSpace(azureMetricConfiguration.MetricName))
            {
                errorMessages.Add("No metric name for Azure is configured");
            }

            // Validate limit, if configured
            if (azureMetricConfiguration.Limit != null)
            {
                if (azureMetricConfiguration.Limit > Promitor.Core.Defaults.MetricDefaults.Limit)
                {
                    errorMessages.Add($"Limit cannot be higher than {Promitor.Core.Defaults.MetricDefaults.Limit}");
                }

                if (azureMetricConfiguration.Limit <= 0)
                {
                    errorMessages.Add("Limit has to be at least 1");
                }
            }

            var metricAggregationValidator = new MetricAggregationValidator(_metricDefaults);
            var metricsAggregationErrorMessages = metricAggregationValidator.Validate(azureMetricConfiguration.Aggregation);
            errorMessages.AddRange(metricsAggregationErrorMessages);

            return errorMessages;
        }

        private IEnumerable<string> ValidateLogAnalyticsConfiguration(LogAnalyticsConfiguration logAnalyticsConfiguration)
        {
            var errorMessages = new List<string>();

            if (logAnalyticsConfiguration == null)
            {
                errorMessages.Add("Invalid Log Analytics is configured");
                return errorMessages;
            }

            if (string.IsNullOrWhiteSpace(logAnalyticsConfiguration.Query))
            {
                errorMessages.Add("No Query for Log Analytics is configured");
            }

            if (logAnalyticsConfiguration.LogAnalyticsAggregation?.Interval == null)
            {
                errorMessages.Add("No Log Analytics Interval is configured");
            }

            return errorMessages;
        }
    }
}