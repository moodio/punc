using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Punc.Data;
using Punc.Utils;
using String = System.String;

namespace Punc
{
    public class RefereeCodesService : IRefereeCodesService
    {
        private readonly IRefereeCodesRepository _refereeCodesRepo;
        private readonly IEmailService _emailService;

        public RefereeCodesService(IRefereeCodesRepository refereeCodesRepository,
            IEmailService emailService)
        {
            _refereeCodesRepo = refereeCodesRepository;
            _emailService = emailService;
        }

        public async Task<string> CreateRefereeCodeAsync(Guid timerId)
        {
            var code = Guid.NewGuid().ToString("N");

            //store the confirmation code
            if (await _refereeCodesRepo.CreateRefereeCodeAsync(code, timerId))
            {
                return code;
            }
            else
            {
                throw new Exception("General server error. Unable to create confirmation code");
            }

        }

        public async Task<Guid?> GetRefereeTimerIdAsync(string refereeCode)
        {
            if (String.IsNullOrWhiteSpace(refereeCode))
                return null;

            return await _refereeCodesRepo.GetRefereeCodeTimerId(refereeCode);
        }

    }
}
