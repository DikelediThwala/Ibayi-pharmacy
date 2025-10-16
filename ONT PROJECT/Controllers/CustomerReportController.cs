using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

namespace ONT_PROJECT.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerReportController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenerateReport(DateTime startDate, DateTime endDate, string groupBy)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var customer = _context.Customers
                .Include(c => c.Prescriptions)
                    .ThenInclude(p => p.PrescriptionLines)
                        .ThenInclude(l => l.Medicine)
                .Include(c => c.Prescriptions)
                    .ThenInclude(p => p.Doctor)
                .Include(c => c.CustomerNavigation)
                .FirstOrDefault(c => c.CustomerNavigation.Email == email);

            if (customer == null)
                return Unauthorized();

            var fullName = $"{customer.CustomerNavigation.FirstName} {customer.CustomerNavigation.LastName}";

            var prescriptionLines = customer.Prescriptions
                .SelectMany(p => p.PrescriptionLines, (p, line) => new { Prescription = p, Line = line })
                .Where(x => x.Line.Date >= DateOnly.FromDateTime(startDate) &&
                            x.Line.Date <= DateOnly.FromDateTime(endDate))
                .ToList();

            var report = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // HEADER
                    page.Header().Row(row =>
                    {
                        row.ConstantItem(100).Image("wwwroot/images/logo_2-removebg-preview.png");
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Ibhayi Pharmacy").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                            col.Item().Text($"Customer Report - {DateTime.Now:dd/MM/yyyy}").FontSize(16).SemiBold();
                            col.Item().Text($"Customer: {fullName}").FontSize(12).Italic().FontColor(Colors.Grey.Darken2);
                        });
                    });

                    // CONTENT
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Container().PaddingBottom(10)
                           .Text($"Prescriptions from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}")
                           .FontSize(18).Bold().FontColor(Colors.Black).Underline();

                        if (!prescriptionLines.Any())
                        {
                            col.Item().AlignCenter().Text("No prescriptions found for the selected date range.");
                        }
                        else if (groupBy == "Doctor")
                        {
                            var groupedByDoctor = prescriptionLines
                                .GroupBy(x => x.Prescription.Doctor)
                                .ToList();

                            foreach (var doctorGroup in groupedByDoctor)
                            {
                                var doctor = doctorGroup.Key;

                                col.Item().Text($"DOCTOR: {doctor?.Name ?? "N/A"} {doctor?.Surname ?? ""}")
                                    .FontSize(14).Bold().FontColor(Colors.Black).Underline();

                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(); // Date
                                        columns.RelativeColumn(); // Medication
                                        columns.RelativeColumn(); // Qty
                                        columns.RelativeColumn(); // Repeats
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Date").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Medication").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Qty").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Repeats").Bold();
                                    });

                                    foreach (var item in doctorGroup)
                                    {
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Date.ToString("yyyy-MM-dd"));
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Medicine?.MedicineName ?? "N/A");
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Quantity.ToString());
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Repeats.ToString());
                                    }

                                    table.Footer(footer =>
                                    {
                                        footer.Cell().ColumnSpan(4).Background(Colors.Grey.Lighten2).AlignRight()
                                            .Text($"Sub-total: {doctorGroup.Sum(x => x.Line.Quantity)}").Bold();
                                    });
                                });

                                col.Item().PaddingBottom(10);
                            }

                            col.Item().Text($"GRAND TOTAL: {prescriptionLines.Sum(p => p.Line.Quantity)}")
                                .Bold().FontSize(12).AlignRight();
                        }
                        else if (groupBy == "Medication")
                        {
                            var groupedByMedication = prescriptionLines
                                .GroupBy(x => x.Line.Medicine)
                                .ToList();

                            foreach (var medGroup in groupedByMedication)
                            {
                                var med = medGroup.Key;

                                col.Item().Text($"MEDICATION: {med?.MedicineName ?? "N/A"}")
                                    .FontSize(14).Bold().FontColor(Colors.Black).Underline();

                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(); // Date
                                        columns.RelativeColumn(); // Doctor
                                        columns.RelativeColumn(); // Qty
                                        columns.RelativeColumn(); // Repeats
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Date").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Doctor").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Qty").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Repeats").Bold();
                                    });

                                    foreach (var item in medGroup)
                                    {
                                        var doctor = item.Prescription.Doctor;
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Date.ToString("yyyy-MM-dd"));
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text($"{doctor?.Name ?? "N/A"} {doctor?.Surname ?? ""}");
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Quantity.ToString());
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Repeats.ToString());
                                    }

                                    table.Footer(footer =>
                                    {
                                        footer.Cell().ColumnSpan(4).Background(Colors.Grey.Lighten2).AlignRight()
                                            .Text($"Sub-total: {medGroup.Sum(x => x.Line.Quantity)}").Bold();
                                    });
                                });

                                col.Item().PaddingBottom(10);
                            }

                            col.Item().Text($"GRAND TOTAL: {prescriptionLines.Sum(p => p.Line.Quantity)}")
                                .Bold().FontSize(12).AlignRight();
                        }
                    });

                    // FOOTER
                    page.Footer().AlignCenter().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                    });
                });
            });

            var pdfBytes = report.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"CustomerReport_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
