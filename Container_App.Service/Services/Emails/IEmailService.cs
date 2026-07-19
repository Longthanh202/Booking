using Container_App.Core.Model.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Emails
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest request);
    }
}
