# KQL - KUSTO QUERY LANGUAGE

1. Create the Application Insights in the Azure
2. Copy the connection strings from the Overview blade.
3. Update the localsettings.json file

```bash
#Find all logs specific to a OrderId
traces
| where customDimensions["OrderId"] == "PASTE_YOUR_GUID_HERE"
| project timestamp, message, severityLevel


requests
| order by timestamp desc

# 
traces
| where timestamp > ago(30m)
| where customDimensions.UserType != "VIP"
| where severityLevel >= 3 // 3 is Error
| project timestamp, message, customDimensions.OrderId

```

Level 2: Performance & Metrics (Aggregations)Goal: Understand how the function behaves over time.Find the 95th percentile duration (P95):In .NET terms: "95% of my users experience a latency of $x$ or less."

```bash
requests
| where name == "ProcessOrder"
| summarize P95_Duration = percentile(duration, 95), Avg_Duration = avg(duration) by bin(timestamp, 1h)
| render timechart
```
![alt text](image.png)

- Count total revenue by User Type (VIP vs Standard): This demonstrates extracting values from customDimensions.
```bash

```

- List last 30 Failure requests
```bash
requests
| where success == False
| sort  by timestamp desc
| take 30
```


- Find AverageDuration, MaxDuaration and TotalCalls 

```bash
traces
| where customDimensions.prop__functionName == "Functions.ProcessOrder"
| extend Duration = toint(customDimensions.prop__executionDuration)
| summarize 
    AvgDuration = avg(Duration), 
    MaxDuration = max(Duration), 
    TotalCalls = count() 
    by tostring(customDimensions.prop__status)
```

![alt text](image-1.png)

```bash
traces
| where customDimensions.prop__functionName == "Functions.ProcessOrder"
| project 
    timestamp, 
    Status = tostring(customDimensions.prop__status), 
    DurationMs = toint(customDimensions.prop__executionDuration),
    InvocationId = tostring(customDimensions.prop__invocationId)
```
![alt text](image-2.png)

## Business Value Query: 
- Instead of looking at logs, lets look at money. Since we used TrackMetric, we don't have to parse strings.

```bash
customMetrics
| where name == "PaymentAmount"
| summarize 
    TotalVolume = sum(value), 
    AverageTransaction = avg(value), 
    TransactionCount = count() 
    by bin(timestamp, 1h)
| render columnchart
```
![alt text](image-3.png)

## The Funnel Analysis.
- Did users who started a payment actually finish it? We use customEvents to track the "Step" flow.

```bash 
customEvents
| where name in ("PaymentInitiated", "FraudAlertTriggered")
| summarize 
    Started = countif(name == "PaymentInitiated"), 
    FraudFlags = countif(name == "FraudAlertTriggered") 
    by bin(timestamp, 1h)
| extend FraudRate = (todouble(FraudFlags) / Started) * 100
```

## Dependency Bottlenect Search
- We manually tracked the "ExternalCardProcessor". Lets see if that specific 3rd party API is slowing us down.

```bash
dependencies
| where target == "ExternalCardProcessor"
| summarize 
    AvgDuration = avg(duration), 
    SuccessRate = (countif(success == true) * 100.0 / count()),
    P95 = percentile(duration, 95)
    by bin(timestamp, 15m)
| render timechart
```
## Corelation Master
- If a "FraudAlertTriggered" event occurs, find every single log and dependency that happened during he specific execution

```bash
let fraudInvocations = customEvents 
    | where name == "FraudAlertTriggered" 
    | project operation_Id;
union requests, traces, dependencies, exceptions
| where operation_Id in (fraudInvocations)
| sort by timestamp asc
| project timestamp, itemType, name, message, duration
```
![alt text](image-4.png)