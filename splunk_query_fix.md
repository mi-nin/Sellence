# Splunk Query Fix for Multiselect Variable

## Issue
The current query has a problem with the `where` clause when using a multiselect variable `planning_status`. When multiple values are selected, the variable contains comma-separated values, but the `like()` function doesn't handle this properly.

## Current Problematic Line
```splunk
| where "${planning_status}" = "*" OR like(planningStatus, "${planning_status}")
```

## Fixed Query
```splunk
index="${env:value}" kubernetes_namespace="optimizer-v2-${env:text}"
("begin consuming GlobalPlanningCommand" OR "publishing ParcelPlanState")
| eval temp_event=case(
    like(message, "%begin consuming GlobalPlanningCommand%"), "start",
    like(message, "%publishing ParcelPlanState%"), "end",
    true(), null()
)
| rex field=message "deliveryOfficePostcode6=(?<deliveryOfficePostcode6>\d+)"
| rex field=message "deliveryOfficeName=(?<deliveryOfficeName>[^,]+)"
| rex field=message "deliveryProcess=(?<deliveryProcess>\w+)"
| rex field=message "deliveryTeamId=(?<deliveryTeamId>[^,]+)"
| rex field=message "planDate=(?<planDate>\d{4}-\d{2}-\d{2})"
| rex field=message "parcelPlanStatus=(?<planComputationStatus>\w+)"
| eval 
    deliveryTeamId = if(deliveryTeamId="<null>", "0", deliveryTeamId),
    startedAt=if(temp_event="start", _time, null()),
    publishedAt=if(temp_event="end", _time, null()),
    planningStatus = case(
        parcelPlanStatus="PLAN_SELECTED", "SUCCESS",
        parcelPlanStatus="ERROR", "FAILURE"
    )
| eval group_id = deliveryOfficePostcode6 . "-" . planDate . "-" . deliveryTeamId . "-" . deliveryProcess
| stats 
    values(deliveryOfficePostcode6) as deliveryOfficePostcode6
    values(deliveryOfficeName) as deliveryOfficeName
    values(deliveryProcess) as deliveryProcess
    values(deliveryTeamId) as deliveryTeamId
    values(planDate) as planDate
    min(startedAt) as startedAt
    max(publishedAt) as publishedAt
    values(planningStatus) as planningStatus
by group_id
| eval 
    startedAt = strftime(startedAt, "%H:%M:%S"),
    publishedAt = if(isnull(publishedAt), "", strftime(publishedAt, "%H:%M:%S")),
    planningStatus = if(isnull(planningStatus), "NOT AVAILABLE", planningStatus)
| where "${planning_status}" = "*" OR match(planningStatus, "${planning_status}")
| table deliveryOfficePostcode6 deliveryOfficeName deliveryProcess deliveryTeamId planDate startedAt publishedAt planningStatus
| sort by planDate, startedAt
```

## Alternative Solutions

### Option 1: Use `match()` function (Recommended)
Replace the `like()` with `match()`:
```splunk
| where "${planning_status}" = "*" OR match(planningStatus, "${planning_status}")
```

### Option 2: Use `in()` function
```splunk
| where "${planning_status}" = "*" OR in(planningStatus, "${planning_status}")
```

### Option 3: Use `searchmatch()` function
```splunk
| where "${planning_status}" = "*" OR searchmatch("planningStatus=${planning_status}")
```

## Why This Fixes the Issue

1. **`match()` function**: Handles comma-separated values in multiselect variables properly. It treats each comma-separated value as a separate pattern to match against.

2. **`in()` function**: Directly checks if the field value is in the list of comma-separated values.

3. **`searchmatch()` function**: Creates a search expression that works with multiselect variables.

## Testing
To test if it's working:
1. Select "All" in the planning_status variable - should show all records
2. Select a single status (e.g., "SUCCESS") - should show only SUCCESS records
3. Select multiple statuses (e.g., "SUCCESS" and "FAILURE") - should show records matching either status

The `match()` function is generally the most reliable for this use case as it properly handles the comma-separated format that Grafana uses for multiselect variables.