using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExperimentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SampleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResultCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SubmittedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ReviewComments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ArchivedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ArchivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Approvals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequiredApprovalCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewStartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Experiments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SampleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExperimentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ArchivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    Sha256Checksum = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UploadedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    QuarantineReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UploadedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AvailableAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    QuarantinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RegisteredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RetiredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CasNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChemicalFormula = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MolecularWeight = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HazardClassification = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SafetyDataSheetReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DefaultUnit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MinimumStockLevel = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Laboratories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Protocols",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProtocolCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActivatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeprecatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ArchivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protocols", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampleRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SampleName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SampleDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequestedSampleType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    SampleSource = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AssignedReviewerUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedSampleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewComments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewStartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FulfilledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Samples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RegisteredByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RegisteredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ArchivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samples", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeprecatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDefinitionVersion = table.Column<int>(type: "int", nullable: false),
                    WorkflowName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResultEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    NumericValue = table.Column<decimal>(type: "decimal(28,10)", precision: 28, scale: 10, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Uncertainty = table.Column<decimal>(type: "decimal(28,10)", precision: 28, scale: 10, nullable: true),
                    Method = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultEntries_AnalysisResults_ResultId",
                        column: x => x.ResultId,
                        principalTable: "AnalysisResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalDecisionRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalDecisionRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalDecisionRecords_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalReviewers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    HasActed = table.Column<bool>(type: "bit", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalReviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalReviewers_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceType = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LinkedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileReferences_FileAssets_FileId",
                        column: x => x.FileId,
                        principalTable: "FileAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstrumentMaintenances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ScheduledByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CompletionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentMaintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstrumentMaintenances_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LotNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InitialQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MinimumStockLevel = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    ManufactureDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StorageConditions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CertificateOfAnalysisNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryLots_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryMembers_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProtocolSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProtocolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Instruction = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    TargetValue = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    TargetUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MinimumValue = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    MaximumValue = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtocolSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProtocolSteps_Protocols_ProtocolId",
                        column: x => x.ProtocolId,
                        principalTable: "Protocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStepDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    AllowReassignment = table.Column<bool>(type: "bit", nullable: false),
                    DueAfterHours = table.Column<int>(type: "int", nullable: true),
                    RequiredRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStepDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStepDefinitions_WorkflowDefinitions_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowStepDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CompletedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CompletionComments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowTasks_WorkflowInstances_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    BalanceAfterMovement = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PerformedByUserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ExperimentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryMovements_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryMovements_InventoryLots_LotId",
                        column: x => x.LotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_ExperimentId",
                table: "AnalysisResults",
                column: "ExperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_InstrumentId",
                table: "AnalysisResults",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_LaboratoryId",
                table: "AnalysisResults",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_LaboratoryId_ResultCode",
                table: "AnalysisResults",
                columns: new[] { "LaboratoryId", "ResultCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_SampleId",
                table: "AnalysisResults",
                column: "SampleId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_Status",
                table: "AnalysisResults",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDecisionRecords_ApprovalId",
                table: "ApprovalDecisionRecords",
                column: "ApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDecisionRecords_ReviewerUserId",
                table: "ApprovalDecisionRecords",
                column: "ReviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalReviewers_ApprovalId_Sequence",
                table: "ApprovalReviewers",
                columns: new[] { "ApprovalId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalReviewers_ApprovalId_UserId",
                table: "ApprovalReviewers",
                columns: new[] { "ApprovalId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_RequestedByUserId",
                table: "Approvals",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_Status",
                table: "Approvals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_SubjectType_SubjectId",
                table: "Approvals",
                columns: new[] { "SubjectType", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_AssignedToUserId",
                table: "Experiments",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_LaboratoryId",
                table: "Experiments",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_LaboratoryId_ExperimentCode",
                table: "Experiments",
                columns: new[] { "LaboratoryId", "ExperimentCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_SampleId",
                table: "Experiments",
                column: "SampleId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAssets_Category",
                table: "FileAssets",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_FileAssets_Sha256Checksum",
                table: "FileAssets",
                column: "Sha256Checksum");

            migrationBuilder.CreateIndex(
                name: "IX_FileAssets_Status",
                table: "FileAssets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FileAssets_StorageKey",
                table: "FileAssets",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileAssets_UploadedByUserId",
                table: "FileAssets",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FileReferences_FileId",
                table: "FileReferences",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_FileReferences_FileId_ReferenceType_ReferenceId",
                table: "FileReferences",
                columns: new[] { "FileId", "ReferenceType", "ReferenceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileReferences_ReferenceType_ReferenceId",
                table: "FileReferences",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentMaintenances_InstrumentId",
                table: "InstrumentMaintenances",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentMaintenances_ScheduledAt",
                table: "InstrumentMaintenances",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_LaboratoryId",
                table: "Instruments",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_LaboratoryId_InstrumentCode",
                table: "Instruments",
                columns: new[] { "LaboratoryId", "InstrumentCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Status",
                table: "Instruments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_LaboratoryId_ItemCode",
                table: "InventoryItems",
                columns: new[] { "LaboratoryId", "ItemCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_LaboratoryId_Status_Type",
                table: "InventoryItems",
                columns: new[] { "LaboratoryId", "Status", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_ExpiryDate",
                table: "InventoryLots",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_InventoryItemId_LotNumber",
                table: "InventoryLots",
                columns: new[] { "InventoryItemId", "LotNumber" },
                unique: true,
                filter: "[InventoryItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_Status",
                table: "InventoryLots",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_ExperimentId",
                table: "InventoryMovements",
                column: "ExperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_InventoryItemId",
                table: "InventoryMovements",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_LotId",
                table: "InventoryMovements",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_OccurredAt",
                table: "InventoryMovements",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratories_Code",
                table: "Laboratories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryMembers_LaboratoryId_UserId",
                table: "LaboratoryMembers",
                columns: new[] { "LaboratoryId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryMembers_UserId",
                table: "LaboratoryMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_LaboratoryId",
                table: "Protocols",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_LaboratoryId_ProtocolCode_Version",
                table: "Protocols",
                columns: new[] { "LaboratoryId", "ProtocolCode", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_Status",
                table: "Protocols",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolSteps_ProtocolId_Sequence",
                table: "ProtocolSteps",
                columns: new[] { "ProtocolId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultEntries_ResultId",
                table: "ResultEntries",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleRequests_LaboratoryId",
                table: "SampleRequests",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleRequests_RequestCode",
                table: "SampleRequests",
                column: "RequestCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleRequests_RequestedByUserId",
                table: "SampleRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleRequests_Status",
                table: "SampleRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SampleRequests_Status_CreatedAt",
                table: "SampleRequests",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Samples_AssignedToUserId",
                table: "Samples",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_LaboratoryId",
                table: "Samples",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_LaboratoryId_SampleCode",
                table: "Samples",
                columns: new[] { "LaboratoryId", "SampleCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Samples_Status",
                table: "Samples",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_Code_Version",
                table: "WorkflowDefinitions",
                columns: new[] { "Code", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_Status",
                table: "WorkflowDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_ReferenceType_ReferenceId",
                table: "WorkflowInstances",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_Status",
                table: "WorkflowInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_WorkflowDefinitionId",
                table: "WorkflowInstances",
                column: "WorkflowDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStepDefinitions_WorkflowDefinitionId_Code",
                table: "WorkflowStepDefinitions",
                columns: new[] { "WorkflowDefinitionId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStepDefinitions_WorkflowDefinitionId_Sequence",
                table: "WorkflowStepDefinitions",
                columns: new[] { "WorkflowDefinitionId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_AssignedToUserId",
                table: "WorkflowTasks",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_DueAt",
                table: "WorkflowTasks",
                column: "DueAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_Status",
                table: "WorkflowTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_WorkflowInstanceId_Sequence",
                table: "WorkflowTasks",
                columns: new[] { "WorkflowInstanceId", "Sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalDecisionRecords");

            migrationBuilder.DropTable(
                name: "ApprovalReviewers");

            migrationBuilder.DropTable(
                name: "Experiments");

            migrationBuilder.DropTable(
                name: "FileReferences");

            migrationBuilder.DropTable(
                name: "InstrumentMaintenances");

            migrationBuilder.DropTable(
                name: "InventoryMovements");

            migrationBuilder.DropTable(
                name: "LaboratoryMembers");

            migrationBuilder.DropTable(
                name: "ProtocolSteps");

            migrationBuilder.DropTable(
                name: "ResultEntries");

            migrationBuilder.DropTable(
                name: "SampleRequests");

            migrationBuilder.DropTable(
                name: "Samples");

            migrationBuilder.DropTable(
                name: "WorkflowStepDefinitions");

            migrationBuilder.DropTable(
                name: "WorkflowTasks");

            migrationBuilder.DropTable(
                name: "Approvals");

            migrationBuilder.DropTable(
                name: "FileAssets");

            migrationBuilder.DropTable(
                name: "Instruments");

            migrationBuilder.DropTable(
                name: "InventoryLots");

            migrationBuilder.DropTable(
                name: "Laboratories");

            migrationBuilder.DropTable(
                name: "Protocols");

            migrationBuilder.DropTable(
                name: "AnalysisResults");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "InventoryItems");
        }
    }
}
