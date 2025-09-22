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
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenerateReport(DateTime startDate, DateTime endDate, string groupBy)
        {
            // Get logged-in customer's email
            var email = User.Identity.Name; // assumes email is username
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

            // Filter prescriptions by date range
            var prescriptions = customer.Prescriptions
                .Where(p => p.Date >= DateOnly.FromDateTime(startDate) &&
                            p.Date <= DateOnly.FromDateTime(endDate))
                .ToList();

            var report = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // Header
                    page.Header().AlignCenter().Column(col =>
                    {
                        col.Item().Text("CUSTOMER REPORT").SemiBold().FontSize(16).FontColor(Colors.Blue.Darken1);
                        col.Item().Text($"Date Range: {startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}")
                            .FontSize(12)
                            .FontColor(Colors.Black);
                    });

                    // Content
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        if (!prescriptions.Any())
                        {
                            col.Item().AlignCenter().Text("No prescriptions found for the selected date range.");
                        }
                        else if (groupBy == "Doctor")
                        {
                            var groupedByDoctor = prescriptions.GroupBy(p => p.Doctor).ToList();
                            foreach (var doctorGroup in groupedByDoctor)
                            {
                                var doctor = doctorGroup.Key;
                                col.Item().Text($"DOCTOR: {doctor?.Name ?? "N/A"} {doctor?.Surname ?? ""}")
                                    .Bold().FontSize(12);

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
                                        header.Cell().Element(CellStyle).Text("Date");
                                        header.Cell().Element(CellStyle).Text("Medication");
                                        header.Cell().Element(CellStyle).Text("Qty");
                                        header.Cell().Element(CellStyle).Text("Repeats");
                                    });

                                    foreach (var prescription in doctorGroup)
                                    {
                                        foreach (var line in prescription.PrescriptionLines)
                                        {
                                            table.Cell().Element(CellStyle).Text(prescription.Date.ToString("yyyy-MM-dd"));
                                            table.Cell().Element(CellStyle).Text(line.Medicine?.MedicineName ?? "N/A");
                                            table.Cell().Element(CellStyle).Text(line.Quantity.ToString());
                                            table.Cell().Element(CellStyle).Text(line.Repeats.ToString());
                                        }
                                    }

                                    table.Footer(footer =>
                                    {
                                        footer.Cell().ColumnSpan(4)
                                            .AlignRight()
                                            .Text($"Sub-total: {doctorGroup.Sum(d => d.PrescriptionLines.Sum(l => l.Quantity))}");
                                    });
                                });

                                col.Item().Text(""); // spacing
                            }

                            col.Item().Text($"GRAND TOTAL: {prescriptions.Sum(p => p.PrescriptionLines.Sum(l => l.Quantity))}")
                                .Bold().FontSize(12);
                        }
                        else if (groupBy == "Medication")
                        {
                            var groupedByMedication = prescriptions
                                .SelectMany(p => p.PrescriptionLines)
                                .GroupBy(l => l.Medicine)
                                .ToList();

                            foreach (var medGroup in groupedByMedication)
                            {
                                var med = medGroup.Key;
                                col.Item().Text($"MEDICATION: {med?.MedicineName ?? "N/A"}")
                                    .Bold().FontSize(12);

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
                                        header.Cell().Element(CellStyle).Text("Date");
                                        header.Cell().Element(CellStyle).Text("Doctor");
                                        header.Cell().Element(CellStyle).Text("Qty");
                                        header.Cell().Element(CellStyle).Text("Repeats");
                                    });

                                    foreach (var line in medGroup)
                                    {
                                        table.Cell().Element(CellStyle).Text(line.Prescription.Date.ToString("yyyy-MM-dd"));
                                        table.Cell().Element(CellStyle).Text($"{line.Prescription.Doctor?.Name ?? "N/A"} {line.Prescription.Doctor?.Surname ?? ""}");
                                        table.Cell().Element(CellStyle).Text(line.Quantity.ToString());
                                        table.Cell().Element(CellStyle).Text(line.Repeats.ToString());
                                    }

                                    table.Footer(footer =>
                                    {
                                        footer.Cell().ColumnSpan(4)
                                            .AlignRight()
                                            .Text($"Sub-total: {medGroup.Sum(l => l.Quantity)}");
                                    });
                                });

                                col.Item().Text(""); // spacing
                            }

                            col.Item().Text($"GRAND TOTAL: {prescriptions.Sum(p => p.PrescriptionLines.Sum(l => l.Quantity))}")
                                .Bold().FontSize(12);
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            });

            var pdfBytes = report.GeneratePdf();
            return File(pdfBytes, "application/pdf", "CustomerReport.pdf");
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
        }
    }
}
