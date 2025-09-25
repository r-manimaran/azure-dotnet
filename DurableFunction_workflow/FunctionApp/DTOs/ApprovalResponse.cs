using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.DTOs;

public class ApprovalResponse
{
    public string InstanceId { get; set; }
    public ApprovalStatus Status { get; set; }
    public ApprovalFor ApprovalFor { get; set; }
}

public enum ApprovalStatus
{
    Approved,
    Rejected,
    Pending
}
public enum ApprovalFor
{
    Leave,
    TimeOff,
    Expenses
}
