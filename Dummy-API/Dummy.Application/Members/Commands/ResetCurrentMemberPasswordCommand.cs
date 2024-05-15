using Dummy.Domain.Constants;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Extensions;
using Dummy.Infrastructure.Helpers;
using Dummy.Infrastructure.Services.Auth;
using Dummy.Infrastructure.Services.EmailService;
using Dummy.Infrastructure.Services.JobService;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public class ResetCurrentMemberPasswordCommand : IRequestWithBaseResponse
    {
        public String Email { get; init; }
    }

    // Command validation
    public class ForgotPasswordMemberCommandValidator : AbstractValidator<ResetCurrentMemberPasswordCommand>
    {
        private readonly IStringLocalizer<ForgotPasswordMemberCommandValidator> _localizer;

        public ForgotPasswordMemberCommandValidator(IStringLocalizer<ForgotPasswordMemberCommandValidator> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Email).EmailAddress()
                                 .OverridePropertyName(_localizer.Translate("email"))
                                 .WithMessage(_localizer.Translate("failure.invalid"))
                                 .NotEmpty()
                                 .OverridePropertyName(_localizer.Translate("email"))
                                 .WithMessage(_localizer.Translate("failure.cant_be_empty"));
        }
    }

    internal class ForgotPasswordMemberCommandHandler : IRequestWithBaseResponseHandler<ResetCurrentMemberPasswordCommand>
    {
        private readonly IJobService _jobService;
        private readonly IAuthService _authService;
        private readonly MainDBContext _mainDBContext;
        private readonly IStringLocalizer<ForgotPasswordMemberCommandHandler> _localizer;

        public ForgotPasswordMemberCommandHandler(MainDBContext mainDBContext, IJobService jobService,
        IAuthService authService, IStringLocalizer<ForgotPasswordMemberCommandHandler> localizer)
        {
            _localizer = localizer;
            _jobService = jobService;
            _authService = authService;
            _mainDBContext = mainDBContext;
        }
        public async Task<BaseResponseDTO> Handle(ResetCurrentMemberPasswordCommand request, CancellationToken cancellationToken)
        {
            var existingMember = await _mainDBContext.Members.Include(x => x.Locations)
                                                             .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);


            if (existingMember is null)
            {
                throw new RestfulAPIException(HttpStatusCode.NotFound, _localizer.Translate("failure.not_exists", new List<String> { "user" }));
            }

            var newPassword = StringHelper.GenerateTemporaryPassword();
            existingMember.Password = _authService.HashPassword(newPassword);

            // [Background Job]: Send email notification with temporary password           
            await _jobService.ExecuteNow<ForgotPasswordJob>(
                $"forgotPassword-{existingMember.Id}-{DateTime.UtcNow.ToString("yyyy-mm-dd HH:mm:ss")}",
                new Dictionary<string, object> {
                    { ForgotPasswordJob.RecieverEmail, existingMember.Email },
                    { ForgotPasswordJob.TemporaryPassword, newPassword },
                },
                cancellationToken
            );

            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO
            {
                Code = HttpStatusCode.OK,
                Message = _localizer.Translate("successful.email_sent")
            };
        }
    }

    internal class ForgotPasswordJob : IJob
    {
        public const string RecieverEmail = "recieverEmail";
        public const string TemporaryPassword = "temporaryPassword";


        private readonly ILogger<ForgotPasswordJob> _logger;
        private readonly IEmailNotificationService _emailNotificationService;

        public ForgotPasswordJob(ILogger<ForgotPasswordJob> logger,
                                 IEmailNotificationService emailNotificationService)
        {
            _logger = logger;
            _emailNotificationService = emailNotificationService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation($"Start to execute job named {context.JobDetail.Key.Name} group {context.JobDetail.Key.Group}");

                var data = context.MergedJobDataMap;
                var recieverEmailAddress = data.GetString(RecieverEmail);
                var temporaryPassword = data.GetString(TemporaryPassword);

                await _emailNotificationService.SendEmailAsync(recieverEmailAddress, EmailEvent.ForgotPassword, null, new List<string> { temporaryPassword });

                _logger.LogInformation($"Finish execute job named {context.JobDetail.Key.Name} group {context.JobDetail.Key.Group}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
