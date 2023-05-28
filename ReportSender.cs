using SoftwareCatalogDatabaseASP.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Quartz;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;

namespace SoftwareCatalogDatabaseASP
{
    public class ReportSender : IJob
    {

        string file_path_report_categories;
        string file_path_report_comment;
        string file_path_report_soft;
        private readonly SoftwareCatalogDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public ReportSender(SoftwareCatalogDBContext context, IWebHostEnvironment
       appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public void GetSoftwareReport()
        {
            string path = "/Reports/software_report_template.xlsx";
            string result = "/Reports/software_report.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                excelPackage.Workbook.Properties.Author = "Вертоградов И.А.";
                excelPackage.Workbook.Properties.Title = "Отчёт по программам";
                excelPackage.Workbook.Properties.Subject = "Программы";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Software"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Software> softwares = _context.Softwares.ToList();
                foreach (Software software in softwares)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = software.Id;
                    worksheet.Cells[startLine, 3].Value = software.Categories.Count;
                    worksheet.Cells[startLine, 4].Value = software.Screens.Count;
                    worksheet.Cells[startLine, 5].Value = software.Comments.Count;
                    worksheet.Cells[startLine, 6].Value = software.Name;
                    worksheet.Cells[startLine, 7].Value = software.Discription;
                    worksheet.Cells[startLine, 8].Value = software.Image;
                    worksheet.Cells[startLine, 9].Value = software.Link;
                    worksheet.Cells[startLine, 10].Value = software.SystemRequirements;
                    worksheet.Cells[startLine, 11].Value = software.LicensName;
                    worksheet.Cells[startLine, 12].Value = software.LicenseType;
                    worksheet.Cells[startLine, 13].Value = software.LicensePrice;
                    worksheet.Cells[startLine, 14].Value = software.LicenseDuration;
                    startLine++;
                }
                string file_path_report = _appEnvironment.WebRootPath + result;
                excelPackage.SaveAs(file_path_report);
            }
        }
        public void GetCommentsReport()
        {
            string path = "/Reports/comments_report_template.xlsx";
            string result = "/Reports/comments_report.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                excelPackage.Workbook.Properties.Author = "Вертоградов И.А.";
                excelPackage.Workbook.Properties.Title = "Отчёт по комментариям";
                excelPackage.Workbook.Properties.Subject = "Комментарии";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Comments"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Comments> comments = _context.Comments.ToList();
                foreach (Comments coment in comments)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = coment.Id;
                    worksheet.Cells[startLine, 3].Value = coment.Text;
                    worksheet.Cells[startLine, 4].Value = coment.Software;
                    startLine++;
                }
                string file_path_report = _appEnvironment.WebRootPath + result;
                excelPackage.SaveAs(file_path_report);
            }
        }
        public void GetCategoriesReport()
        {
            string path = "/Reports/categories_report_template.xlsx";
            string result = "/Reports/categories_report.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                excelPackage.Workbook.Properties.Author = "Вертоградов И.А.";
                excelPackage.Workbook.Properties.Title = "Отчёт по категориям";
                excelPackage.Workbook.Properties.Subject = "Категории";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Categories"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Categories> categories = _context.Categories.ToList();
                foreach (Categories category in categories.GroupBy(c => c.Name).Select(group => group.First()))
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = category.Id;
                    worksheet.Cells[startLine, 3].Value = category.Name;
                    startLine++;
                }
                string file_path_report = _appEnvironment.WebRootPath + result;
                excelPackage.SaveAs(file_path_report);
            }
        }
        delegate void SoftwareReportDelegate();
        public async Task Execute(IJobExecutionContext context)
        {
            file_path_report_soft = _appEnvironment.WebRootPath + "/Reports/software_report.xlsx";
            file_path_report_comment = _appEnvironment.WebRootPath + "/Reports/comments_report.xlsx";
            file_path_report_categories = _appEnvironment.WebRootPath + "/Reports/categories_report.xlsx";

            try
            {
                if (File.Exists(file_path_report_categories))
                    File.Delete(file_path_report_categories);
                if (File.Exists(file_path_report_comment))
                    File.Delete(file_path_report_comment);
                if (File.Exists(file_path_report_soft))
                    File.Delete(file_path_report_soft);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            GetSoftwareReport();
            GetCommentsReport();
            GetCategoriesReport();

            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("ilyant2222@mail.ru", "Система автоматической отчетности");
            // кому отправляем
            MailAddress to = new MailAddress("ilyant2000@gmail.com");
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Отчет о программах";
            // текст письма
            m.Body = "<h2>Системой был сформирован и отправлен отчет о добавленном в систему ПО</h2>";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            // логин и пароль
            smtp.Credentials = new NetworkCredential("ilyant2222@mail.ru",
            "AqPGDxWjY4X8zuXAzTQS"); //eEaJVhzFh1K0jewPp66Q
            smtp.EnableSsl = true;
            // вкладываем файл в письмо
            m.Attachments.Add(new Attachment(file_path_report_categories));
            m.Attachments.Add(new Attachment(file_path_report_comment));
            m.Attachments.Add(new Attachment(file_path_report_soft));
            // отправляем асинхронно
            smtp.DeliveryFormat = SmtpDeliveryFormat.SevenBit;
            smtp.Timeout = 500;
            await smtp.SendMailAsync(m);
            m.Dispose();
        }
    }

}
