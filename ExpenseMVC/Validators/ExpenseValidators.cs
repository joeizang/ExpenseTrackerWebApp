using ExpenseMVC.ViewModels.ExpenseVM;
using FluentValidation;

namespace ExpenseMVC.Validators;

public class CreateExpenseInputModelValidator : AbstractValidator<CreateExpenseInputModel>
{
    public CreateExpenseInputModelValidator()
    {
        RuleFor(x => x.ExpenseName)
            .NotEmpty()
            .WithMessage("Expense Name is required")
            .MaximumLength(50)
            .WithMessage("Expense Name must not exceed 50 characters");

        RuleFor(x => x.ExpenseAmount)
            .NotEmpty()
            .WithMessage("Expense Amount is required")
            .GreaterThan(0)
            .WithMessage("Expense Amount must be greater than 0");
        
        RuleFor(x => x.ExpenseDescription)
            .NotEmpty()
            .WithMessage("Expense Description is required")
            .MaximumLength(500)
            .WithMessage("Expense Description must not exceed 100 characters");

        RuleFor(x => x.ExpenseDate)
            .NotEmpty()
            .WithMessage("Expense Date is required")
            .LessThan(DateTime.Now)
            .WithMessage("Expense Date must be in the past");

        RuleFor(x => x.ExpenseOwnerId)
            .NotEmpty()
            .WithMessage("Expense Owner is required");
    }
}
