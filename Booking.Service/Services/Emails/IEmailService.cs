using Booking.Core.Model.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Emails
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest request);
    }
}
