using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GenerateReport(DateTime startDate, DateTime endDate, string groupBy)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var start = DateOnly.FromDateTime(startDate);
            var end = DateOnly.FromDateTime(endDate);

            var customer = await _context.Customers
                .Include(c => c.Prescriptions)
                    .ThenInclude(p => p.PrescriptionLines)
                        .ThenInclude(l => l.Medicine)
                .Include(c => c.Prescriptions)
                    .ThenInclude(p => p.Doctor)
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.Email == email);

            if (customer == null)
                return Unauthorized();

            var fullName = $"{customer.CustomerNavigation.FirstName} {customer.CustomerNavigation.LastName}";

            // Prescriptions in date range
            var prescriptions = customer.Prescriptions
                .Where(p => p.PrescriptionLines.Any(l => l.Date >= start && l.Date <= end))
                .ToList();

            // Orders in date range
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customer.CustomerId
                            && o.DatePlaced >= start && o.DatePlaced <= end)
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Medicine)
                .ToListAsync();

            var report = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // Header
                    page.Header().Row(row =>
                    {
                        row.ConstantItem(100).Image("wwwroot/images/logo_2-removebg-preview.png");
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Ibhayi Pharmacy").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                            col.Item().Text("Customer Report").FontSize(16).SemiBold();
                            col.Item().Text($"Date Range: {start:yyyy-MM-dd} – {end:yyyy-MM-dd}").FontSize(12);
                            col.Item().Text($"Customer: {fullName}").FontSize(12).Italic().FontColor(Colors.Grey.Darken2);
                        });
                    });

                    // Content
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // ===== Prescriptions =====
                        col.Item().PaddingBottom(5).Text("Dispensed Prescriptions").FontSize(18).Bold().Underline();

                        double prescriptionsGrandTotal = 0;

                        if (!prescriptions.Any())
                        {
                            col.Item().Text("No prescriptions found for the selected date range.")
                                .FontColor(Colors.Grey.Darken1);
                        }
                        else
                        {
                            foreach (var prescription in prescriptions)
                            {
                                string doctorName = $"{prescription.Doctor?.Name ?? "N/A"} {prescription.Doctor?.Surname ?? ""}".Trim();

                                col.Item().PaddingTop(6).Text($"DOCTOR: {doctorName} | Date: {prescription.Date:yyyy-MM-dd}")
                                    .FontSize(14).Bold();

                                double prescriptionSubtotal = 0;

                                col.Item().Padding(5).Background(Colors.Grey.Lighten4).Column(presBox =>
                                {
                                    presBox.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3); // Medication
                                            columns.RelativeColumn();  // Quantity
                                            columns.RelativeColumn();  // Repeats
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Blue.Lighten4).Border(1).Text("Medication").Bold();
                                            header.Cell().Background(Colors.Blue.Lighten4).Border(1).Text("Qty").Bold();
                                            header.Cell().Background(Colors.Blue.Lighten4).Border(1).Text("Repeats").Bold();
                                        });

                                        foreach (var line in prescription.PrescriptionLines.Where(l => l.Date >= start && l.Date <= end))
                                        {
                                            double lineTotal = line.Medicine != null ? line.Medicine.SalesPrice * line.Quantity : 0;
                                            prescriptionSubtotal += lineTotal;

                                            table.Cell().Border(1).Text(line.Medicine?.MedicineName ?? "N/A");
                                            table.Cell().Border(1).Text(line.Quantity.ToString());
                                            table.Cell().Border(1).Text(line.Repeats.ToString());
                                        }
                                    });
                                });

                                // Sub-total for this prescription
                                col.Item().PaddingTop(2).AlignRight().Text($"Sub-total: R{prescriptionSubtotal:0.00}").Bold();
                                prescriptionsGrandTotal += prescriptionSubtotal;

                                col.Item().PaddingBottom(8);
                            }

                            // Grand total for all prescriptions
                            col.Item().PaddingTop(5).AlignRight()
                                .Text($"GRAND TOTAL: R{prescriptionsGrandTotal:0.00}")
                                .FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                        }

                        // ===== Orders =====
                        col.Item().PaddingTop(12).PaddingBottom(5).Text("Orders").FontSize(18).Bold().Underline();

                        if (!orders.Any())
                        {
                            col.Item().Text("No orders found for the selected date range.")
                                .FontColor(Colors.Grey.Darken1);
                        }
                        else
                        {
                            foreach (var order in orders)
                            {
                                var orderDateStr = order.DatePlaced.ToString();

                                col.Item().PaddingTop(6)
                                    .Text($"Order ID: {order.OrderId}  |  Date: {orderDateStr}  |  Status: {order.Status ?? "N/A"}")
                                    .FontSize(13).SemiBold();

                                col.Item().Padding(5).Background(Colors.Grey.Lighten4).Column(orderBox =>
                                {
                                    orderBox.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3); // Medicine
                                            columns.RelativeColumn();   // Quantity
                                            columns.RelativeColumn();   // Price
                                            columns.RelativeColumn();   // Line Total
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Blue.Lighten4).Border(1).Text("Medicine").Bold();
                                            header.Cell().Background(Colors.Blue.Lighten4).Border(1).Text("Quantity").Bold();
                                            header.Cell().Background(Colors.Blue.Lighten4).Border(1).Text("Price").Bold();
                                            header.Cell().Background(Colors.Blue.Lighten4).Border(1).Text("Line Total").Bold();
                                        });

                                        foreach (var line in order.OrderLines)
                                        {
                                            table.Cell().Border(1).Text(line.Medicine?.MedicineName ?? "N/A");
                                            table.Cell().Border(1).Text(line.Quantity.ToString());
                                            table.Cell().Border(1).Text($"R{line.Price:0.00}");
                                            table.Cell().Border(1).Text($"R{line.LineTotal:0.00}");
                                        }

                                        table.Footer(footer =>
                                        {
                                            footer.Cell().ColumnSpan(4).AlignRight()
                                                  .Text($"Order Total: R{order.TotalDue:0.00}  |  VAT: R{order.Vat:0.00}")
                                                  .Bold();
                                        });
                                    });
                                });

                                col.Item().PaddingBottom(10);
                            }
                        }
                    });

                    // Footer (page numbering)
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
