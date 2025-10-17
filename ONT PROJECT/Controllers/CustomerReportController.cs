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

            // Collect all relevant prescription lines in date range, keeping Prescription object for grouping
            var prescriptionLines = customer.Prescriptions
                .SelectMany(p => p.PrescriptionLines, (p, line) => new { Prescription = p, Line = line })
                .Where(x => x.Line.Date >= DateOnly.FromDateTime(startDate)
                         && x.Line.Date <= DateOnly.FromDateTime(endDate))
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
                            col.Item().Text($"Dispensed Prescriptions by {groupBy}").FontSize(16).SemiBold();
                            col.Item().Text($"Date Range: {startDate:yyyy-MM-dd} – {endDate:yyyy-MM-dd}").FontSize(12);
                            col.Item().Text($"Customer: {fullName}").FontSize(12).Italic().FontColor(Colors.Grey.Darken2);
                        });
                    });

                    // CONTENT
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        if (!prescriptionLines.Any())
                        {
                            col.Item().AlignCenter().Text("No prescriptions found for the selected date range.");
                            return;
                        }

                        if (groupBy == "Doctor")
                        {
                            var groupedByDoctor = prescriptionLines
                                .GroupBy(x => x.Prescription.Doctor)
                                .ToList();

                            int grandTotal = 0;

                            foreach (var doctorGroup in groupedByDoctor)
                            {
                                var doctor = doctorGroup.Key;
                                string doctorName = $"{doctor?.Name ?? "N/A"} {doctor?.Surname ?? ""}".Trim();

                                col.Item().Container().PaddingBottom(5)
                                    .Text($"DOCTOR: {doctorName}")
                                    .FontSize(14).Bold();

                                // Group by prescription under this doctor
                                var linesByPrescription = doctorGroup
                                    .GroupBy(x => x.Prescription)
                                    .ToList();

                                int subtotalForDoctor = 0;

                                foreach (var presGroup in linesByPrescription)
                                {
                                    var prescription = presGroup.Key;

                                    col.Item().Container().PaddingBottom(2)
                                        .Text($"Prescription Date: {prescription.Date:yyyy-MM-dd}")
                                        .FontSize(12).SemiBold();

                                    col.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();    // Date
                                            columns.RelativeColumn(3);   // Medication
                                            columns.RelativeColumn();    // Qty
                                            columns.RelativeColumn();    // Repeats
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Date").Bold();
                                            header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Medication").Bold();
                                            header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Qty").Bold();
                                            header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Repeats").Bold();
                                        });

                                        foreach (var item in presGroup)
                                        {
                                            table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Date.ToString("yyyy-MM-dd"));
                                            table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Medicine?.MedicineName ?? "N/A");
                                            table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Quantity.ToString());
                                            table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Repeats.ToString());
                                        }

                                        int subTotalPres = presGroup.Sum(x => x.Line.Quantity);
                                        subtotalForDoctor += subTotalPres;

                                        table.Footer(footer =>
                                        {
                                            footer.Cell().ColumnSpan(4)
                                                  .AlignRight()
                                                  .Padding(5)
                                                  .Text($"Sub-total (Prescription): {subTotalPres}")
                                                  .Bold();
                                        });
                                    });

                                    // Add space after prescription table
                                    col.Item().Container().PaddingBottom(10).Text("");
                                }

                                col.Item().Container().PaddingBottom(10)
                                    .Text($"Sub-total for Doctor: {subtotalForDoctor}")
                                    .FontSize(12).Bold();

                                grandTotal += subtotalForDoctor;
                            }

                            col.Item().Container().PaddingTop(10)
                                .AlignRight()
                                .Text($"GRAND TOTAL: {grandTotal}")
                                .FontSize(14).Bold();
                        }
                        else if (groupBy == "Medication")
                        {
                            var groupedByMedication = prescriptionLines
                                .GroupBy(x => x.Line.Medicine)
                                .ToList();

                            int grandTotal = 0;

                            foreach (var medGroup in groupedByMedication)
                            {
                                var med = medGroup.Key;
                                string medName = med?.MedicineName ?? "N/A";

                                col.Item().Container().PaddingBottom(5)
                                    .Text($"MEDICATION: {medName}")
                                    .FontSize(14).Bold();

                                // Group by prescription under this medication
                                var linesByPrescription = medGroup
                                    .GroupBy(x => x.Prescription)
                                    .ToList();

                                int subtotalForMedication = 0;

                                foreach (var presGroup in linesByPrescription)
                                {
                                    var prescription = presGroup.Key;

                                    col.Item().Container().PaddingBottom(2)
                                        .Text($"Prescription Date: {prescription.Date:yyyy-MM-dd}")
                                        .FontSize(12).SemiBold();

                                    col.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();    // Date
                                            columns.RelativeColumn(3);   // Doctor
                                            columns.RelativeColumn();    // Qty
                                            columns.RelativeColumn();    // Repeats
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Date").Bold();
                                            header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Doctor").Bold();
                                            header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Qty").Bold();
                                            header.Cell().Background(Colors.Grey.Lighten2).Border(1).BorderColor(Colors.Black).Text("Repeats").Bold();
                                        });

                                        foreach (var item in presGroup)
                                        {
                                            var doctor = item.Prescription.Doctor;
                                            string doctorName = $"{doctor?.Name ?? "N/A"} {doctor?.Surname ?? ""}".Trim();

                                            table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Date.ToString("yyyy-MM-dd"));
                                            table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(doctorName);
                                            table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Quantity.ToString());
                                            table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Text(item.Line.Repeats.ToString());
                                        }

                                        int subTotalPres = presGroup.Sum(x => x.Line.Quantity);
                                        subtotalForMedication += subTotalPres;

                                        table.Footer(footer =>
                                        {
                                            footer.Cell().ColumnSpan(4)
                                                  .AlignRight()
                                                  .Padding(5)
                                                  .Text($"Sub-total (Prescription): {subTotalPres}")
                                                  .Bold();
                                        });
                                    });

                                    col.Item().Container().PaddingBottom(10).Text("");
                                }

                                col.Item().Container().PaddingBottom(10)
                                    .Text($"Sub-total for Medication: {subtotalForMedication}")
                                    .FontSize(12).Bold();

                                grandTotal += subtotalForMedication;
                            }

                            col.Item().Container().PaddingTop(10)
                                .AlignRight()
                                .Text($"GRAND TOTAL: {grandTotal}")
                                .FontSize(14).Bold();
                        }
                    });

                    // FOOTER with page numbering
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
