﻿using FluentValidation;
using WebApi.Dtos;
using WebApi.Models;

namespace WebApi.Validations;

public class CreatePostValidator :AbstractValidator<CreatePostRequest>
{
    public CreatePostValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Post cannot be null");

        When(x => x != null, () =>
        {
            RuleFor(p => p.Title)
                        .NotNull().WithMessage("Title is required")
                        .NotEmpty().WithMessage("Title is required")
                        .MaximumLength(100);

            RuleFor(p => p.Content)
                    .NotNull().WithMessage("Content is required")
                    .NotEmpty().WithMessage("Content is required");
        });
    }
}
