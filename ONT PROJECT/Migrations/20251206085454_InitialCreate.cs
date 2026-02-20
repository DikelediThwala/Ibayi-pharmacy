//using System;
//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace ONT_PROJECT.Migrations
//{
//    /// <inheritdoc />
//    public partial class InitialCreate : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.CreateTable(
//                name: "ActiveIngredient",
//                columns: table => new
//                {
//                    ActiveIngredientID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    Ingredients = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_ActiveIngredient", x => x.ActiveIngredientID);
//                });

//            migrationBuilder.CreateTable(
//                name: "ActivityLogs",
//                columns: table => new
//                {
//                    ActivityLogId = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    ActivityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    DatePerformed = table.Column<DateTime>(type: "datetime2", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_ActivityLogs", x => x.ActivityLogId);
//                });

//            migrationBuilder.CreateTable(
//                name: "Doctor",
//                columns: table => new
//                {
//                    DoctorID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    PracticeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    ContactNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Doctor", x => x.DoctorID);
//                });

//            migrationBuilder.CreateTable(
//                name: "DosageForm",
//                columns: table => new
//                {
//                    FormID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    FormName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_DosageForm", x => x.FormID);
//                });

//            migrationBuilder.CreateTable(
//                name: "Supplier",
//                columns: table => new
//                {
//                    SupplierID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    ContactSurname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active")
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Supplier", x => x.SupplierID);
//                });

//            migrationBuilder.CreateTable(
//                name: "tblUser",
//                columns: table => new
//                {
//                    UserID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    IDNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    PhoneNumber = table.Column<string>(name: "Phone Number", type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Title = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
//                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
//                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    ResetToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
//                    TokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    ProfilePicture = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_User", x => x.UserID);
//                });

//            migrationBuilder.CreateTable(
//                name: "Medicine",
//                columns: table => new
//                {
//                    MedicineID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    MedicineName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Schedule = table.Column<int>(type: "int", nullable: false),
//                    SalesPrice = table.Column<double>(type: "float", nullable: false),
//                    SupplierID = table.Column<int>(type: "int", nullable: false),
//                    ReorderLevel = table.Column<int>(type: "int", nullable: true),
//                    Quantity = table.Column<int>(type: "int", nullable: true),
//                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
//                    FormID = table.Column<int>(type: "int", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Medicine", x => x.MedicineID);
//                    table.ForeignKey(
//                        name: "FK_Medicine_DosageForm",
//                        column: x => x.FormID,
//                        principalTable: "DosageForm",
//                        principalColumn: "FormID");
//                    table.ForeignKey(
//                        name: "FK_Medicine_Supplier_SupplierID",
//                        column: x => x.SupplierID,
//                        principalTable: "Supplier",
//                        principalColumn: "SupplierID",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Customer",
//                columns: table => new
//                {
//                    CustomerID = table.Column<int>(type: "int", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Customer", x => x.CustomerID);
//                    table.ForeignKey(
//                        name: "FK_Customer_TblUser",
//                        column: x => x.CustomerID,
//                        principalTable: "tblUser",
//                        principalColumn: "UserID",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Pharmacist",
//                columns: table => new
//                {
//                    PharmacistID = table.Column<int>(type: "int", nullable: false),
//                    HealthCounsilRegNo = table.Column<string>(type: "nvarchar(max)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Pharmacist", x => x.PharmacistID);
//                    table.ForeignKey(
//                        name: "FK_Pharmacist_TblUser",
//                        column: x => x.PharmacistID,
//                        principalTable: "tblUser",
//                        principalColumn: "UserID",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "PharmacyManager",
//                columns: table => new
//                {
//                    PharmacyManagerID = table.Column<int>(type: "int", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_PharmacyManager", x => x.PharmacyManagerID);
//                    table.ForeignKey(
//                        name: "FK_PharmacyManager_TblUser",
//                        column: x => x.PharmacyManagerID,
//                        principalTable: "tblUser",
//                        principalColumn: "UserID",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "MedIngredient",
//                columns: table => new
//                {
//                    MedIngredientID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    MedicineID = table.Column<int>(type: "int", nullable: false),
//                    ActiveIngredientID = table.Column<int>(type: "int", nullable: false),
//                    Strength = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_MedIngredient", x => x.MedIngredientID);
//                    table.ForeignKey(
//                        name: "FK_MedIngredient_ActiveIngredient",
//                        column: x => x.ActiveIngredientID,
//                        principalTable: "ActiveIngredient",
//                        principalColumn: "ActiveIngredientID");
//                    table.ForeignKey(
//                        name: "FK_MedIngredient_Medicine",
//                        column: x => x.MedicineID,
//                        principalTable: "Medicine",
//                        principalColumn: "MedicineID");
//                });

//            migrationBuilder.CreateTable(
//                name: "CustomerAllergy",
//                columns: table => new
//                {
//                    CustomerAllergyID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    CustomerID = table.Column<int>(type: "int", nullable: false),
//                    ActiveIngredientID = table.Column<int>(type: "int", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_CustomerAllergy", x => x.CustomerAllergyID);
//                    table.ForeignKey(
//                        name: "FK_CustomerAllergy_ActiveIngredient",
//                        column: x => x.ActiveIngredientID,
//                        principalTable: "ActiveIngredient",
//                        principalColumn: "ActiveIngredientID");
//                    table.ForeignKey(
//                        name: "FK_CustomerAllergy_Customer",
//                        column: x => x.CustomerID,
//                        principalTable: "Customer",
//                        principalColumn: "CustomerID");
//                });

//            migrationBuilder.CreateTable(
//                name: "UnprocessedPrescription",
//                columns: table => new
//                {
//                    UnprocessedPrescriptionID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    CustomerID = table.Column<int>(type: "int", nullable: false),
//                    Date = table.Column<DateOnly>(type: "date", nullable: false),
//                    PrescriptionPhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
//                    Dispense = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
//                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_UnprocessedPrescription", x => x.UnprocessedPrescriptionID);
//                    table.ForeignKey(
//                        name: "FK_UnprocessedPrescription_Customer",
//                        column: x => x.CustomerID,
//                        principalTable: "Customer",
//                        principalColumn: "CustomerID");
//                });

//            migrationBuilder.CreateTable(
//                name: "Order",
//                columns: table => new
//                {
//                    OrderID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    CustomerID = table.Column<int>(type: "int", nullable: false),
//                    PharmacistID = table.Column<int>(type: "int", nullable: true),
//                    Status = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
//                    TotalDue = table.Column<double>(type: "float", nullable: false),
//                    VAT = table.Column<double>(type: "float", nullable: false),
//                    SupplierID = table.Column<int>(type: "int", nullable: true),
//                    DatePlaced = table.Column<DateOnly>(type: "date", nullable: false),
//                    DateRecieved = table.Column<DateOnly>(type: "date", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Order", x => x.OrderID);
//                    table.ForeignKey(
//                        name: "FK_Order_Customer",
//                        column: x => x.CustomerID,
//                        principalTable: "Customer",
//                        principalColumn: "CustomerID");
//                    table.ForeignKey(
//                        name: "FK_Order_Pharmacist",
//                        column: x => x.PharmacistID,
//                        principalTable: "Pharmacist",
//                        principalColumn: "PharmacistID");
//                });

//            migrationBuilder.CreateTable(
//                name: "Pharmacy",
//                columns: table => new
//                {
//                    PharmacyID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    HealthCounsilRegistrationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    ContactNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
//                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    WebsiteUrl = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
//                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
//                    VATRate = table.Column<double>(type: "float", nullable: false),
//                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
//                    PharmacistID = table.Column<int>(type: "int", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Pharmacy", x => x.PharmacyID);
//                    table.ForeignKey(
//                        name: "FK_Pharmacy_Pharmacist",
//                        column: x => x.PharmacistID,
//                        principalTable: "Pharmacist",
//                        principalColumn: "PharmacistID");
//                });

//            migrationBuilder.CreateTable(
//                name: "Prescription",
//                columns: table => new
//                {
//                    PrescriptionID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    Date = table.Column<DateOnly>(type: "date", nullable: false),
//                    CustomerID = table.Column<int>(type: "int", nullable: false),
//                    PharmacistID = table.Column<int>(type: "int", nullable: false),
//                    PrescriptionPhoto = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: true),
//                    DoctorID = table.Column<int>(type: "int", nullable: false),
//                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Prescription", x => x.PrescriptionID);
//                    table.ForeignKey(
//                        name: "FK_Prescription_Customer",
//                        column: x => x.CustomerID,
//                        principalTable: "Customer",
//                        principalColumn: "CustomerID");
//                    table.ForeignKey(
//                        name: "FK_Prescription_Doctor",
//                        column: x => x.DoctorID,
//                        principalTable: "Doctor",
//                        principalColumn: "DoctorID");
//                    table.ForeignKey(
//                        name: "FK_Prescription_Pharmacist",
//                        column: x => x.PharmacistID,
//                        principalTable: "Pharmacist",
//                        principalColumn: "PharmacistID");
//                });

//            migrationBuilder.CreateTable(
//                name: "B_Order",
//                columns: table => new
//                {
//                    B_OrderID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    DatePlaced = table.Column<DateOnly>(type: "date", nullable: false),
//                    DateRecieved = table.Column<DateOnly>(type: "date", nullable: true),
//                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
//                    PharmacyManagerID = table.Column<int>(type: "int", nullable: true),
//                    SupplierID = table.Column<int>(type: "int", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_B_Order", x => x.B_OrderID);
//                    table.ForeignKey(
//                        name: "FK_B_Order_PharmacyManager",
//                        column: x => x.PharmacyManagerID,
//                        principalTable: "PharmacyManager",
//                        principalColumn: "PharmacyManagerID");
//                    table.ForeignKey(
//                        name: "FK_B_Order_Supplier",
//                        column: x => x.SupplierID,
//                        principalTable: "Supplier",
//                        principalColumn: "SupplierID");
//                });

//            migrationBuilder.CreateTable(
//                name: "OrderLine",
//                columns: table => new
//                {
//                    OrderLineID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    OrderID = table.Column<int>(type: "int", nullable: false),
//                    MedicineID = table.Column<int>(type: "int", nullable: false),
//                    Quantity = table.Column<int>(type: "int", nullable: false),
//                    LineID = table.Column<int>(type: "int", nullable: true),
//                    Price = table.Column<double>(type: "float", nullable: false),
//                    LineTotal = table.Column<double>(type: "float", nullable: false),
//                    Status = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_OrderLine", x => x.OrderLineID);
//                    table.ForeignKey(
//                        name: "FK_OrderLine_Medicine",
//                        column: x => x.MedicineID,
//                        principalTable: "Medicine",
//                        principalColumn: "MedicineID");
//                    table.ForeignKey(
//                        name: "FK_OrderLine_Order",
//                        column: x => x.OrderID,
//                        principalTable: "Order",
//                        principalColumn: "OrderID");
//                });

//            migrationBuilder.CreateTable(
//                name: "PrescriptionLine",
//                columns: table => new
//                {
//                    PrescriptionLineID = table.Column<int>(type: "int", nullable: false),
//                    PrescriptionID = table.Column<int>(type: "int", nullable: false),
//                    MedicineID = table.Column<int>(type: "int", nullable: false),
//                    Instructions = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    Date = table.Column<DateOnly>(type: "date", nullable: false),
//                    RepeatsLeft = table.Column<int>(type: "int", nullable: true),
//                    Repeats = table.Column<int>(type: "int", nullable: true),
//                    Quantity = table.Column<int>(type: "int", nullable: false),
//                    UnprocessedPrescriptionID = table.Column<int>(type: "int", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_PrescriptionLine", x => x.PrescriptionLineID);
//                    table.ForeignKey(
//                        name: "FK_PrescriptionLine_Medicine",
//                        column: x => x.MedicineID,
//                        principalTable: "Medicine",
//                        principalColumn: "MedicineID");
//                    table.ForeignKey(
//                        name: "FK_PrescriptionLine_Prescription",
//                        column: x => x.PrescriptionID,
//                        principalTable: "Prescription",
//                        principalColumn: "PrescriptionID");
//                });

//            migrationBuilder.CreateTable(
//                name: "B_OrderLine",
//                columns: table => new
//                {
//                    B_OrderLineID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    B_OrderID = table.Column<int>(type: "int", nullable: false),
//                    MedicineID = table.Column<int>(type: "int", nullable: false),
//                    Quantity = table.Column<int>(type: "int", nullable: false),
//                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_B_OrderLine", x => x.B_OrderLineID);
//                    table.ForeignKey(
//                        name: "FK_B_OrderLine_B_Order",
//                        column: x => x.B_OrderID,
//                        principalTable: "B_Order",
//                        principalColumn: "B_OrderID");
//                    table.ForeignKey(
//                        name: "FK_B_OrderLine_Medicine",
//                        column: x => x.MedicineID,
//                        principalTable: "Medicine",
//                        principalColumn: "MedicineID");
//                });

//            migrationBuilder.CreateTable(
//                name: "RepeatRequest",
//                columns: table => new
//                {
//                    RepeatRequestId = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    OrderLineId = table.Column<int>(type: "int", nullable: false),
//                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
//                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_RepeatRequest", x => x.RepeatRequestId);
//                    table.ForeignKey(
//                        name: "FK_RepeatRequest_OrderLine_OrderLineId",
//                        column: x => x.OrderLineId,
//                        principalTable: "OrderLine",
//                        principalColumn: "OrderLineID",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "RepeatHistory",
//                columns: table => new
//                {
//                    RepeatHistoryID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    PrescriptionLineID = table.Column<int>(type: "int", nullable: false),
//                    RepeatsDecremented = table.Column<int>(type: "int", nullable: false),
//                    DateUsed = table.Column<DateTime>(type: "datetime", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_RepeatHistory", x => x.RepeatHistoryID);
//                    table.ForeignKey(
//                        name: "FK_RepeatHistory_PrescriptionLine",
//                        column: x => x.PrescriptionLineID,
//                        principalTable: "PrescriptionLine",
//                        principalColumn: "PrescriptionLineID",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateIndex(
//                name: "IX_B_Order_PharmacyManagerID",
//                table: "B_Order",
//                column: "PharmacyManagerID");

//            migrationBuilder.CreateIndex(
//                name: "IX_B_Order_SupplierID",
//                table: "B_Order",
//                column: "SupplierID");

//            migrationBuilder.CreateIndex(
//                name: "IX_B_OrderLine_B_OrderID",
//                table: "B_OrderLine",
//                column: "B_OrderID");

//            migrationBuilder.CreateIndex(
//                name: "IX_B_OrderLine_MedicineID",
//                table: "B_OrderLine",
//                column: "MedicineID");

//            migrationBuilder.CreateIndex(
//                name: "IX_CustomerAllergy_ActiveIngredientID",
//                table: "CustomerAllergy",
//                column: "ActiveIngredientID");

//            migrationBuilder.CreateIndex(
//                name: "IX_CustomerAllergy_CustomerID",
//                table: "CustomerAllergy",
//                column: "CustomerID");

//            migrationBuilder.CreateIndex(
//                name: "IX_Medicine_FormID",
//                table: "Medicine",
//                column: "FormID");

//            migrationBuilder.CreateIndex(
//                name: "IX_Medicine_SupplierID",
//                table: "Medicine",
//                column: "SupplierID");

//            migrationBuilder.CreateIndex(
//                name: "IX_MedIngredient_ActiveIngredientID",
//                table: "MedIngredient",
//                column: "ActiveIngredientID");

//            migrationBuilder.CreateIndex(
//                name: "IX_MedIngredient_MedicineID",
//                table: "MedIngredient",
//                column: "MedicineID");

//            migrationBuilder.CreateIndex(
//                name: "IX_Order_CustomerID",
//                table: "Order",
//                column: "CustomerID");

//            migrationBuilder.CreateIndex(
//                name: "IX_Order_PharmacistID",
//                table: "Order",
//                column: "PharmacistID");

//            migrationBuilder.CreateIndex(
//                name: "IX_OrderLine_MedicineID",
//                table: "OrderLine",
//                column: "MedicineID");

//            migrationBuilder.CreateIndex(
//                name: "IX_OrderLine_OrderID",
//                table: "OrderLine",
//                column: "OrderID");

//            migrationBuilder.CreateIndex(
//                name: "IX_Pharmacy_PharmacistID",
//                table: "Pharmacy",
//                column: "PharmacistID");

//            migrationBuilder.CreateIndex(
//                name: "IX_Prescription_CustomerID",
//                table: "Prescription",
//                column: "CustomerID");

//            migrationBuilder.CreateIndex(
//                name: "IX_Prescription_DoctorID",
//                table: "Prescription",
//                column: "DoctorID");

//            migrationBuilder.CreateIndex(
//                name: "IX_Prescription_PharmacistID",
//                table: "Prescription",
//                column: "PharmacistID");

//            migrationBuilder.CreateIndex(
//                name: "IX_PrescriptionLine_MedicineID",
//                table: "PrescriptionLine",
//                column: "MedicineID");

//            migrationBuilder.CreateIndex(
//                name: "IX_PrescriptionLine_PrescriptionID",
//                table: "PrescriptionLine",
//                column: "PrescriptionID");

//            migrationBuilder.CreateIndex(
//                name: "IX_RepeatHistory_PrescriptionLineID",
//                table: "RepeatHistory",
//                column: "PrescriptionLineID");

//            migrationBuilder.CreateIndex(
//                name: "IX_RepeatRequest_OrderLineId",
//                table: "RepeatRequest",
//                column: "OrderLineId");

//            migrationBuilder.CreateIndex(
//                name: "IX_UnprocessedPrescription_CustomerID",
//                table: "UnprocessedPrescription",
//                column: "CustomerID");
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "ActivityLogs");

//            migrationBuilder.DropTable(
//                name: "B_OrderLine");

//            migrationBuilder.DropTable(
//                name: "CustomerAllergy");

//            migrationBuilder.DropTable(
//                name: "MedIngredient");

//            migrationBuilder.DropTable(
//                name: "Pharmacy");

//            migrationBuilder.DropTable(
//                name: "RepeatHistory");

//            migrationBuilder.DropTable(
//                name: "RepeatRequest");

//            migrationBuilder.DropTable(
//                name: "UnprocessedPrescription");

//            migrationBuilder.DropTable(
//                name: "B_Order");

//            migrationBuilder.DropTable(
//                name: "ActiveIngredient");

//            migrationBuilder.DropTable(
//                name: "PrescriptionLine");

//            migrationBuilder.DropTable(
//                name: "OrderLine");

//            migrationBuilder.DropTable(
//                name: "PharmacyManager");

//            migrationBuilder.DropTable(
//                name: "Prescription");

//            migrationBuilder.DropTable(
//                name: "Medicine");

//            migrationBuilder.DropTable(
//                name: "Order");

//            migrationBuilder.DropTable(
//                name: "Doctor");

//            migrationBuilder.DropTable(
//                name: "DosageForm");

//            migrationBuilder.DropTable(
//                name: "Supplier");

//            migrationBuilder.DropTable(
//                name: "Customer");

//            migrationBuilder.DropTable(
//                name: "Pharmacist");

//            migrationBuilder.DropTable(
//                name: "tblUser");
//        }
//    }
//}
