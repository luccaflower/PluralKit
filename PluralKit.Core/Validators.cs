using System;

using FluentValidation;

using NodaTime;

namespace PluralKit.Core
{
    public static class BasicValidators
    {
        public static bool BeValidUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                return uri.IsAbsoluteUri;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        public static bool BeValidTimeZone(string tz) =>
            DateTimeZoneProviders.Tzdb.GetZoneOrNull(tz) != null;
    }

    public class SystemValidator: AbstractValidator<PKSystem>
    {
        public SystemValidator()
        {
            RuleFor(s => s.Hid)
                .NotNull()
                .Length(5)
                .Matches("^[a-z]{5}$");

            RuleFor(s => s.Name)
                .MaximumLength(Limits.MaxSystemNameLength)
                .WithName("System name");

            RuleFor(s => s.Description)
                .MaximumLength(Limits.MaxDescriptionLength)
                .WithName("System description");

            RuleFor(s => s.Tag)
                .MaximumLength(Limits.MaxSystemTagLength)
                .WithName("System tag");

            RuleFor(s => s.AvatarUrl)
                .Must(BasicValidators.BeValidUrl)
                .WithName("System avatar URL")
                .When(s => s.AvatarUrl != null);

            RuleFor(s => s.Created)
                .NotNull();

            RuleFor(s => s.UiTz)
                .NotNull()
                .WithName("System time zone")
                .Must(BasicValidators.BeValidTimeZone)
                .WithName("System time zone");
        }
    }

    public class MemberValidator: AbstractValidator<PKMember>
    {
        public MemberValidator()
        {
            RuleFor(m => m.Hid)
                .NotNull()
                .Length(5)
                .Matches("^[a-z]{5}$");

            RuleFor(m => m.System)
                .GreaterThan(0);

            RuleFor(m => m.Color)
                .Matches("^[0-9a-fA-F]{6}$");
            
            RuleFor(m => m.Name)
                .MaximumLength(Limits.MaxMemberNameLength)
                .WithName("Member name");

            RuleFor(m => m.DisplayName)
                .MaximumLength(32)
                .WithName("Member display name"); // TODO: can we check system tag here too?
            
            // TODO: research whether we can actually like,
            // do the proper size/dimensions check right in here
            // might be too slow?
            RuleFor(m => m.AvatarUrl)
                .Must(BasicValidators.BeValidUrl)
                .WithName("Member avatar URL")
                .When(m => m.AvatarUrl != null);

            // arbitrary sanity checks
            RuleFor(m => m.Pronouns).MaximumLength(Limits.MaxPronounsLength).WithName("Member pronouns");
            RuleFor(m => m.Description).MaximumLength(Limits.MaxDescriptionLength).WithName("Member description");
            RuleFor(m => m.Prefix).MaximumLength(64).WithName("Member proxy prefix");
            RuleFor(m => m.Suffix).MaximumLength(64).WithName("Member proxy suffix");

            RuleFor(m => m.Created).NotNull();
        }
    }
}