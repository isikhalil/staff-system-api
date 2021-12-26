using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Data
{
    public static class DataSeeder
    {
        public static async void Seed(ServiceProvider serviceProvider)
        {
            await InitDepartments(serviceProvider);
            await InitTitles(serviceProvider);
            await InitProvinces(serviceProvider);
            await InitStaffs(serviceProvider);
        }

        private static async Task InitProvinces(ServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<StaffDbContext>();
            if (!dbContext.Provinces.Any())
            {
                var provinces = new List<Province>
                {
                    new Province { Name = "Lefkoşa" },
                    new Province { Name = "Girne" },
                    new Province { Name = "Gazi Mağuza" },
                    new Province { Name = "Güzelyurt" },
                    new Province { Name = "İskele" },
                    new Province { Name = "Lefke" }
                };

                dbContext.Provinces.AddRange(provinces);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task InitDepartments(ServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<StaffDbContext>();
            if (!dbContext.Departments.Any())
            {
                var departments = new List<Department>
                {
                    new Department { Name = "IK" },
                    new Department { Name = "IT" },
                    new Department { Name = "Satış" },
                    new Department { Name = "Finans" },
                    new Department { Name = "Satınalma" }
                };

                dbContext.Departments.AddRange(departments);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task InitTitles(ServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<StaffDbContext>();
            if (!dbContext.Titles.Any())
            {
                var departments = dbContext.Departments.ToList();

                var titles = new List<Title>
                {
                    new Title { Name = "IK Direktörü", Department = departments.FirstOrDefault(x => x.Name == "IK") },
                    new Title { Name = "IK Uzmanı", Department = departments.FirstOrDefault(x => x.Name == "IK") },
                    new Title { Name = "IT Direktörü", Department = departments.FirstOrDefault(x => x.Name == "IT") },
                    new Title { Name = "IT Uzmanı", Department = departments.FirstOrDefault(x => x.Name == "IT") },
                    new Title { Name = "Satış Direktörü", Department = departments.FirstOrDefault(x => x.Name == "Satış") },
                    new Title { Name = "Satış Uzmanı", Department = departments.FirstOrDefault(x => x.Name == "Satış") },
                    new Title { Name = "Finans Direktörü", Department = departments.FirstOrDefault(x => x.Name == "Finans") },
                    new Title { Name = "Muhasebe Uzmanı", Department = departments.FirstOrDefault(x => x.Name == "Finans") },
                    new Title { Name = "Satınalma Direktörü", Department = departments.FirstOrDefault(x => x.Name == "Satınalma") },
                    new Title { Name = "Satınalma Uzmanı", Department = departments.FirstOrDefault(x => x.Name == "Satınalma") },
                    new Title { Name = "Yazılım Direktörü", Department = departments.FirstOrDefault(x => x.Name == "IT") },
                    new Title { Name = "Senior Developer", Department = departments.FirstOrDefault(x => x.Name == "IT") },
                    new Title { Name = "Junior Developer", Department = departments.FirstOrDefault(x => x.Name == "IT") },
                };

                dbContext.Titles.AddRange(titles);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task InitStaffs(ServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<StaffDbContext>();
            if (!dbContext.Staffs.Any())
            {
                var titles = dbContext.Titles.ToList();
                var provinces = dbContext.Provinces.ToList();

                var staffs = new List<Staff>
                {
                    new Staff
                    {
                        FirstName = "John",
                        LastName = "Doe1",
                        Email = "john.doe1@gmail.com",
                        BirthDate = DateTime.Now.AddYears(-30).AddDays(-100),
                        Province = provinces.FirstOrDefault(x => x.Name == "Lefkoşa"),
                        StaffTitles = new List<StaffTitle>
                        {
                            new StaffTitle
                            {
                                Title = titles.FirstOrDefault(x => x.Name == "IK Direktörü"),
                                StartDate = DateTime.Now.AddDays(-5000)
                            }
                        }
                    },
                    new Staff
                    {
                        FirstName = "John",
                        LastName = "Doe2",
                        Email = "john.doe2@gmail.com",
                        BirthDate = DateTime.Now.AddYears(-30).AddDays(-100),
                        Province = provinces.FirstOrDefault(x => x.Name == "Lefkoşa"),
                        StaffTitles = new List<StaffTitle>
                        {
                            new StaffTitle
                            {
                                Title = titles.FirstOrDefault(x => x.Name == "IK Uzmanı"),
                                StartDate = DateTime.Now.AddDays(-6000)
                            }
                        }
                    },
                    new Staff
                    {
                        FirstName = "John",
                        LastName = "Doe3",
                        Email = "john.doe3@gmail.com",
                        BirthDate = DateTime.Now.AddYears(-30).AddDays(-100),
                        Province = provinces.FirstOrDefault(x => x.Name == "Lefkoşa"),
                        StaffTitles = new List<StaffTitle>
                        {
                            new StaffTitle
                            {
                                Title = titles.FirstOrDefault(x => x.Name == "Junior Developer"),
                                StartDate = DateTime.Now.AddDays(-4000)
                            },
                            new StaffTitle
                            {
                                Title = titles.FirstOrDefault(x => x.Name == "Senior Developer"),
                                StartDate = DateTime.Now.AddDays(-3000)
                            }
                        }
                    },
                };

                dbContext.Staffs.AddRange(staffs);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
