using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Functions;

public static class ExpenseActivities
{

    [Function(nameof(AutoApproveExpense))]
    public static bool AutoApproveExpense([ActivityTrigger] object expenseRequest, FunctionContext context)
    {
        return true;
    }

    [Function(nameof(ProcessExpenseApproval))]
    public static bool ProcessExpenseApproval([ActivityTrigger] object expenseDetail, FunctionContext context)
    {
        return true;
    }

    [Function(nameof(EscalateExpenseRequest))]
    public static bool EscalateExpenseRequest([ActivityTrigger] object expenseRequest, FunctionContext context)
    {
        return true;
    }
}
