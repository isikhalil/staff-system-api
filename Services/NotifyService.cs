using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Web.Data;
using WishaLink.Models;

namespace WishaLink.Services
{
    public interface INotifyService
    {
        Task NotifyStaffs();
    }

    public class NotifyService : INotifyService, IHostedService, IDisposable
    {
        private Timer mTimer;
        private IServiceProvider mServices;
        private DateTime mLastNotifyTime = DateTime.MinValue;

        public NotifyService(
            IServiceProvider services
        )
        {
            mServices = services;
        }

        public async Task NotifyStaffs()
        {
            using (var scope = mServices.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<StaffDbContext>();

                    var allStafs = dbContext.Staffs
                       .Include(x => x.StaffTitles).ThenInclude(x => x.Title).ThenInclude(x => x.Department)
                       .ToList();

                    var accountingDepartmentId = dbContext.Departments.First(x => x.Name.ToLower() == "muhasebe").Id;

                    var accountingStaffIds = new List<long>();
                    var accountingStaffEmails = new List<string>();

                    foreach (var allStaff in allStafs)
                    {
                        if (!allStafs.Any(x => x.Id == allStaff.Id))
                        {
                            var currentStaffTitle = allStaff.StaffTitles.OrderByDescending(x => x.StartDate).FirstOrDefault();
                            if (currentStaffTitle != null && 
                                currentStaffTitle.Title != null && 
                                currentStaffTitle.Title.Department != null &&
                                currentStaffTitle.Title.DepartmentId == accountingDepartmentId
                            ) {
                                accountingStaffIds.Add(allStaff.Id);

                                if (!string.IsNullOrEmpty(allStaff.Email))
                                {
                                    accountingStaffEmails.Add(allStaff.Email);
                                }
                            }
                        }
                    }

                    if (accountingStaffEmails.Count > 0)
                    {
                        await SendEmailAsync(accountingStaffEmails);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public Task SendEmailAsync(List<string> emails)
        {
            var client = new SmtpClient("mail.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("test@gmail.com", "Test123!");

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("test@gmail.com")
            };

            foreach (var email in emails)
            {
                mailMessage.To.Add(email);
            }

            mailMessage.Body = "Muhasebe departmanına iletilecek mesaj içeriği buraya gelecek";
            mailMessage.Subject = "Merhaba Muhasebeciler!";
            client.Send(mailMessage);
            return Task.FromResult(0);
        }

        private async void Check(object state)
        {
            if (mLastNotifyTime.Date != DateTime.Today && DateTime.Now.Hour > 8)
            {
                await NotifyStaffs();
                mLastNotifyTime = DateTime.Now;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mTimer = new Timer(Check, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            mTimer?.Dispose();
        }
    }
}
