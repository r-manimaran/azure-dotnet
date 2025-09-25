using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Functions;

public static class LeaveApprovalActivities
{
    [Function(nameof(ProcessLeaveApproval))]
    public static bool ProcessLeaveApproval([ActivityTrigger] object request, FunctionContext executionContext)
    {
       
        return true;
    }

    [Function(nameof(EscalateLeaveRequest))]
    public static bool EscalateLeaveRequest([ActivityTrigger] object request, FunctionContext executionContext)
    {

        return true;
    }

    [Function(nameof(HandleOrchestrationError))]
    public static bool HandleOrchestrationError([ActivityTrigger] object request, FunctionContext executionContext)
    {

        return true;
    }



}
