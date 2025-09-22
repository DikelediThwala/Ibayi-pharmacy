using Microsoft.AspNetCore.Authorization;   // For [Authorize]
using Microsoft.AspNetCore.Mvc;              // For Controller, IActionResult
using Microsoft.EntityFrameworkCore;         // For Include(), ThenInclude(), EF queries
using ONT_PROJECT.Models;                     // Your DbContext and models
using QuestPDF.Fluent;                        // For Document.Create()
using QuestPDF.Helpers;                       // For Colors
using QuestPDF.Infrastructure;                // For IContainer
using System;                                 // For DateTime, DateOnly
using System.Linq;                            // For LINQ queries (Where, GroupBy, SelectMany, etc.)


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
            // Get logged-in customer's email
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

            // Filter prescriptions by PrescriptionLine date
            var prescriptionLines = customer.Prescriptions
                .SelectMany(p => p.PrescriptionLines, (p, line) => new { Prescription = p, Line = line })
                .Where(x => x.Line.Date >= DateOnly.FromDateTime(startDate) &&
                            x.Line.Date <= DateOnly.FromDateTime(endDate))
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
                                        header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Date");
                                        header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Medication");
                                        header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Qty");
                                        header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Repeats");
                                    });

                                    foreach (var item in doctorGroup)
                                    {
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                            .Text(item.Line.Date.ToString("yyyy-MM-dd"));
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                            .Text(item.Line.Medicine?.MedicineName ?? "N/A");
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                            .Text(item.Line.Quantity.ToString());
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                            .Text(item.Line.Repeats.ToString());
                                    }

                                    table.Footer(footer =>
                                    {
                                        footer.Cell().ColumnSpan(4)
                                            .AlignRight()
                                            .Text($"Sub-total: {doctorGroup.Sum(d => d.Line.Quantity)}");
                                    });
                                });

                                col.Item().Text(""); // spacing
                            }

                            col.Item().Text($"GRAND TOTAL: {prescriptionLines.Sum(p => p.Line.Quantity)}")
                                .Bold().FontSize(12);
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
                                        header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Date");
                                        header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Doctor");
                                        header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Qty");
                                        header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Repeats");
                                    });

                                    foreach (var item in medGroup)
                                    {
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                            .Text(item.Line.Date.ToString("yyyy-MM-dd"));
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                            .Text($"{item.Prescription.Doctor?.Name ?? "N/A"} {item.Prescription.Doctor?.Surname ?? ""}");
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                            .Text(item.Line.Quantity.ToString());
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                            .Text(item.Line.Repeats.ToString());
                                    }

                                    table.Footer(footer =>
                                    {
                                        footer.Cell().ColumnSpan(4)
                                            .AlignRight()
                                            .Text($"Sub-total: {medGroup.Sum(l => l.Line.Quantity)}");
                                    });
                                });

                                col.Item().Text(""); // spacing
                            }

                            col.Item().Text($"GRAND TOTAL: {prescriptionLines.Sum(p => p.Line.Quantity)}")
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
    }
}
