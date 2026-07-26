using LOCPS.Enums;
using System.ComponentModel.DataAnnotations;

namespace LOCPS.DTOs;

public class RegisterAdminRequest
{
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterCustomerRequest
{
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}

public class AssignRoleRequest
{
    [Required]
    public int RoleId { get; set; }
}

public class CreateProductRequest
{
    [Required]
    public string ProductName { get; set; } = string.Empty;
    public string? ProductDescription { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int MaxTenureMonths { get; set; }
    public decimal ProcessingFee { get; set; }
    public int CreatedByUserId { get; set; }
}

public class UpdateStatusRequest
{
    [Required]
    public ApplicationStatus Status { get; set; }

    [Required]
    public int ActorUserId { get; set; }
}

public class KycDecisionRequest
{
    [Required]
    public int VerifiedByUserId { get; set; }

    public string Remarks { get; set; } = string.Empty;
}

public class CreditDecisionRequest
{
    [Required]
    public int UserId { get; set; }

    public string? Comments { get; set; }
}

public class CalculateCreditRequest
{
    [Required]
    public int EvaluatedByUserId { get; set; }
}

public class ApproveLoanRequest
{
    [Required]
    public int ApplicationId { get; set; }

    [Required]
    public int ApproverUserId { get; set; }

    [Required]
    public decimal ApprovedAmount { get; set; }

    [Required]
    public int TenureMonths { get; set; }

    [Required]
    public decimal InterestRate { get; set; }

    public string? Comments { get; set; }
}

public class RejectLoanRequest
{
    [Required]
    public int ApplicationId { get; set; }

    [Required]
    public int ApproverUserId { get; set; }

    [Required]
    public string Reason { get; set; } = string.Empty;

    public string? Comments { get; set; }
}

public class SendBackLoanRequest
{
    [Required]
    public int ApplicationId { get; set; }

    [Required]
    public int ApproverUserId { get; set; }

    [Required]
    public string Remarks { get; set; } = string.Empty;
}
