using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EnglishPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BadgeType = table.Column<int>(type: "int", nullable: false),
                    PointThreshold = table.Column<int>(type: "int", nullable: true),
                    BonusPoints = table.Column<int>(type: "int", nullable: false),
                    CriteriaType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriteriaValue = table.Column<int>(type: "int", nullable: true),
                    CriteriaSkill = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SenderPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    SchoolType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    TargetGradeId = table.Column<int>(type: "int", nullable: true),
                    TargetUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PriceEGP = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    Features = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GradeId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsGuest = table.Column<bool>(type: "bit", nullable: false),
                    FacebookId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StudentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PinHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BookingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    PreferredDates = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingRequests_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DragDropQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    SkillCategory = table.Column<int>(type: "int", nullable: false),
                    ContentTopic = table.Column<int>(type: "int", nullable: true),
                    GameTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NumberOfZones = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    TimeLimit = table.Column<int>(type: "int", nullable: true),
                    PointsPerCorrectItem = table.Column<int>(type: "int", nullable: false),
                    ShowImmediateFeedback = table.Column<bool>(type: "bit", nullable: false),
                    UITheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DragDropQuestions_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlipCardQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    SkillCategory = table.Column<int>(type: "int", nullable: false),
                    ContentTopic = table.Column<int>(type: "int", nullable: true),
                    GameTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GameMode = table.Column<int>(type: "int", nullable: false),
                    NumberOfPairs = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    TimerMode = table.Column<int>(type: "int", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "int", nullable: true),
                    PointsPerMatch = table.Column<int>(type: "int", nullable: false),
                    MovePenalty = table.Column<int>(type: "int", nullable: false),
                    ShowHints = table.Column<bool>(type: "bit", nullable: false),
                    MaxHints = table.Column<int>(type: "int", nullable: false),
                    UITheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CardBackDesign = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomCardBackUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnableAudio = table.Column<bool>(type: "bit", nullable: false),
                    EnableExplanations = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlipCardQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlipCardQuestions_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuestSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuestSessions_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchingGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    SkillCategory = table.Column<int>(type: "int", nullable: false),
                    ContentTopic = table.Column<int>(type: "int", nullable: true),
                    NumberOfPairs = table.Column<int>(type: "int", nullable: false),
                    MatchingMode = table.Column<int>(type: "int", nullable: false),
                    UITheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShowConnectingLines = table.Column<bool>(type: "bit", nullable: false),
                    EnableAudio = table.Column<bool>(type: "bit", nullable: false),
                    EnableHints = table.Column<bool>(type: "bit", nullable: false),
                    MaxHints = table.Column<int>(type: "int", nullable: false),
                    TimerMode = table.Column<int>(type: "int", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "int", nullable: true),
                    PointsPerMatch = table.Column<int>(type: "int", nullable: false),
                    WrongMatchPenalty = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchingGames_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    InstructionAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    SkillCategory = table.Column<int>(type: "int", nullable: false),
                    ContentTopic = table.Column<int>(type: "int", nullable: true),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    TestType = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PassageText = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: true),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    HintText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Points = table.Column<int>(type: "int", nullable: false),
                    EstimatedTimeMinutes = table.Column<int>(type: "int", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitNumber = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WheelQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    SkillCategory = table.Column<int>(type: "int", nullable: false),
                    ContentTopic = table.Column<int>(type: "int", nullable: true),
                    QuestionText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    WrongAnswers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    PointsValue = table.Column<int>(type: "int", nullable: false),
                    TimeLimit = table.Column<int>(type: "int", nullable: false),
                    HintText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryTag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WheelQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WheelQuestions_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WheelSpinSegments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SegmentType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    SkillCategory = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WheelSpinSegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WheelSpinSegments_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaderboardEntries",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardEntries", x => new { x.UserId, x.GradeId });
                    table.ForeignKey(
                        name: "FK_LeaderboardEntries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaderboardEntries_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParentChildren",
                columns: table => new
                {
                    ParentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChildId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LinkedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentChildren", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_ParentChildren_AspNetUsers_ChildId",
                        column: x => x.ChildId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentChildren_AspNetUsers_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentProgress",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    TotalTestsTaken = table.Column<int>(type: "int", nullable: false),
                    TotalGamesPlayed = table.Column<int>(type: "int", nullable: false),
                    AverageScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false),
                    LongestStreak = table.Column<int>(type: "int", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProgress", x => new { x.UserId, x.GradeId });
                    table.ForeignKey(
                        name: "FK_StudentProgress_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentProgress_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBadges",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BadgeId = table.Column<int>(type: "int", nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBadges", x => new { x.UserId, x.BadgeId });
                    table.ForeignKey(
                        name: "FK_UserBadges_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBadges_Badges_BadgeId",
                        column: x => x.BadgeId,
                        principalTable: "Badges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_SubscriptionPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WheelGameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GuestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    SkillCategory = table.Column<int>(type: "int", nullable: true),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    QuestionsAnswered = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    WrongAnswers = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    HintsUsed = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    SessionData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WheelGameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WheelGameSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WheelGameSessions_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DragDropGameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GuestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DragDropQuestionId = table.Column<int>(type: "int", nullable: false),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    CorrectPlacements = table.Column<int>(type: "int", nullable: false),
                    WrongPlacements = table.Column<int>(type: "int", nullable: false),
                    HintsUsed = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropGameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DragDropGameSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DragDropGameSessions_DragDropQuestions_DragDropQuestionId",
                        column: x => x.DragDropQuestionId,
                        principalTable: "DragDropQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DragDropZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DragDropQuestionId = table.Column<int>(type: "int", nullable: false),
                    ZoneLabel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ZoneColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ZoneOrder = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DragDropZones_DragDropQuestions_DragDropQuestionId",
                        column: x => x.DragDropQuestionId,
                        principalTable: "DragDropQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlipCardGameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GuestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FlipCardQuestionId = table.Column<int>(type: "int", nullable: false),
                    TotalPairs = table.Column<int>(type: "int", nullable: false),
                    MatchesFound = table.Column<int>(type: "int", nullable: false),
                    TotalFlips = table.Column<int>(type: "int", nullable: false),
                    WrongFlips = table.Column<int>(type: "int", nullable: false),
                    HintsUsed = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlipCardGameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlipCardGameSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FlipCardGameSessions_FlipCardQuestions_FlipCardQuestionId",
                        column: x => x.FlipCardQuestionId,
                        principalTable: "FlipCardQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlipCardPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlipCardQuestionId = table.Column<int>(type: "int", nullable: false),
                    Card1Type = table.Column<int>(type: "int", nullable: false),
                    Card1Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card1ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card1AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card2Type = table.Column<int>(type: "int", nullable: false),
                    Card2Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card2ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card2AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PairOrder = table.Column<int>(type: "int", nullable: false),
                    DifficultyWeight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlipCardPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlipCardPairs_FlipCardQuestions_FlipCardQuestionId",
                        column: x => x.FlipCardQuestionId,
                        principalTable: "FlipCardQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchingGamePairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchingGameId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    QuestionImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuestionAudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AnswerImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AnswerAudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AnswerType = table.Column<int>(type: "int", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PairOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingGamePairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchingGamePairs_MatchingGames_MatchingGameId",
                        column: x => x.MatchingGameId,
                        principalTable: "MatchingGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchingGameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GuestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MatchingGameId = table.Column<int>(type: "int", nullable: false),
                    TotalPairs = table.Column<int>(type: "int", nullable: false),
                    CorrectMatches = table.Column<int>(type: "int", nullable: false),
                    WrongAttempts = table.Column<int>(type: "int", nullable: false),
                    HintsUsed = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingGameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchingGameSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MatchingGameSessions_MatchingGames_MatchingGameId",
                        column: x => x.MatchingGameId,
                        principalTable: "MatchingGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchingPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    LeftText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RightText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LeftImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RightImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PairIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchingPairs_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Points = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LessonNumber = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WheelQuestionAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SegmentLanded = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SelectedAnswer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    TimeTakenSeconds = table.Column<int>(type: "int", nullable: false),
                    HintUsed = table.Column<bool>(type: "bit", nullable: false),
                    AttemptTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WheelQuestionAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WheelQuestionAttempts_WheelGameSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "WheelGameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WheelQuestionAttempts_WheelQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "WheelQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DragDropItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DragDropQuestionId = table.Column<int>(type: "int", nullable: false),
                    ItemText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ItemAudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrectZoneId = table.Column<int>(type: "int", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ItemOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DragDropItems_DragDropQuestions_DragDropQuestionId",
                        column: x => x.DragDropQuestionId,
                        principalTable: "DragDropQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DragDropItems_DragDropZones_CorrectZoneId",
                        column: x => x.CorrectZoneId,
                        principalTable: "DragDropZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlipCardAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    Card1PairId = table.Column<int>(type: "int", nullable: false),
                    Card2PairId = table.Column<int>(type: "int", nullable: false),
                    Card1Position = table.Column<int>(type: "int", nullable: false),
                    Card2Position = table.Column<int>(type: "int", nullable: false),
                    IsMatch = table.Column<bool>(type: "bit", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    AttemptTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeSpentMs = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlipCardAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlipCardAttempts_FlipCardGameSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "FlipCardGameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchingAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    QuestionPairId = table.Column<int>(type: "int", nullable: false),
                    SelectedAnswerPairId = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    AttemptTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeSpentMs = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchingAttempts_MatchingGameSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "MatchingGameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    LessonId = table.Column<int>(type: "int", nullable: true),
                    TestType = table.Column<int>(type: "int", nullable: false),
                    SkillCategory = table.Column<int>(type: "int", nullable: true),
                    IsTimedTest = table.Column<bool>(type: "bit", nullable: false),
                    TimeLimitMinutes = table.Column<int>(type: "int", nullable: true),
                    PassingScore = table.Column<int>(type: "int", nullable: false),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    ShuffleQuestions = table.Column<bool>(type: "bit", nullable: false),
                    ShuffleOptions = table.Column<bool>(type: "bit", nullable: false),
                    ShowCorrectAnswers = table.Column<bool>(type: "bit", nullable: false),
                    ShowExplanations = table.Column<bool>(type: "bit", nullable: false),
                    AllowRetake = table.Column<bool>(type: "bit", nullable: false),
                    MaxRetakeCount = table.Column<int>(type: "int", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AvailableTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tests_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tests_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DragDropAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    PlacedInZoneId = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    AttemptTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeSpentMs = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DragDropAttempts_DragDropGameSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "DragDropGameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DragDropAttempts_DragDropItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "DragDropItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DragDropAttempts_DragDropZones_PlacedInZoneId",
                        column: x => x.PlacedInZoneId,
                        principalTable: "DragDropZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GuestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MaxPossibleScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    WrongAnswers = table.Column<int>(type: "int", nullable: false),
                    SkippedAnswers = table.Column<int>(type: "int", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttempts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TestAttempts_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestQuestions_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttemptAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttemptId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SelectedOptionIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    PointsEarned = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_TestAttempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "TestAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "Id", "BadgeType", "BonusPoints", "CreatedAt", "CreatedBy", "CriteriaSkill", "CriteriaType", "CriteriaValue", "DeletedAt", "DescriptionAr", "DescriptionEn", "Icon", "IconUrl", "IsActive", "IsDeleted", "NameAr", "NameEn", "PointThreshold", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 1, 10, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5193), null, null, null, null, null, "أكمل أول اختبار", "Complete your first test", "🎯", null, true, false, "الخطوة الأولى", "First Steps", null, null, null },
                    { 2, 2, 50, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5204), null, null, null, null, null, "احصل على 100% في أي اختبار", "Get 100% on any test", "⭐", null, true, false, "العلامة الكاملة", "Perfect Score", null, null, null },
                    { 3, 3, 25, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5205), null, null, "Streak", 7, null, "نشاط 7 أيام متتالية", "7 consecutive days of activity", "🔥", null, true, false, "سلسلة 7 أيام", "7 Day Streak", null, null, null },
                    { 4, 4, 100, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5207), null, null, "Streak", 30, null, "نشاط 30 يوم متتالي", "30 consecutive days of activity", "💪", null, true, false, "سلسلة 30 يوم", "30 Day Streak", null, null, null },
                    { 5, 5, 50, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5209), null, null, null, null, null, "احتل المركز الثالث أو أعلى", "Reach top 3 on grade leaderboard", "🥉", null, true, false, "ضمن أفضل 3", "Top 3", null, null, null },
                    { 6, 6, 100, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5210), null, null, null, null, null, "احتل المركز الأول", "Reach #1 on grade leaderboard", "🏆", null, true, false, "المركز الأول", "Champion", null, null, null },
                    { 7, 7, 40, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5211), null, null, null, null, null, "أكمل جميع أنواع الألعاب", "Complete all 4 game types", "🎮", null, true, false, "محترف الألعاب", "Game Master", null, null, null },
                    { 8, 8, 30, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5213), null, null, null, null, null, "أنهِ اختباراً في أقل من 30% من الوقت مع 80%+", "Complete a timed test in <30% time with 80%+", "⚡", null, true, false, "سريع البرق", "Speed Runner", null, null, null },
                    { 9, 9, 35, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5214), null, null, "UnitCount", 5, null, "أكمل اختبارات من 5 وحدات مختلفة", "Complete tests from 5 different units", "🧭", null, true, false, "المستكشف", "Explorer", null, null, null },
                    { 10, 10, 75, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(5215), null, null, null, null, null, "أكمل اختباراً في كل وحدة", "Complete at least one test in every unit", "📚", null, true, false, "متفوق الوحدات", "All Units", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DisplayOrder", "IsActive", "IsDeleted", "Level", "NameAr", "NameEn", "SchoolType", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(4928), null, null, 1, true, false, 1, "الصف الأول الابتدائي", "Grade 1 Primary", "Primary", null, null },
                    { 2, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(4939), null, null, 2, true, false, 2, "الصف الثاني الابتدائي", "Grade 2 Primary", "Primary", null, null },
                    { 3, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(4941), null, null, 3, true, false, 3, "الصف الثالث الابتدائي", "Grade 3 Primary", "Primary", null, null },
                    { 4, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(4942), null, null, 4, true, false, 4, "الصف الرابع الابتدائي", "Grade 4 Primary", "Primary", null, null },
                    { 5, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(4943), null, null, 5, true, false, 5, "الصف الخامس الابتدائي", "Grade 5 Primary", "Primary", null, null },
                    { 6, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(4978), null, null, 6, true, false, 6, "الصف السادس الابتدائي", "Grade 6 Primary", "Primary", null, null },
                    { 7, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(4979), null, null, 7, true, false, 7, "الصف الأول الإعدادي", "Grade 1 Preparatory", "Preparatory", null, null },
                    { 8, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(4980), null, null, 8, true, false, 8, "الصف الثاني الإعدادي", "Grade 2 Preparatory", "Preparatory", null, null },
                    { 9, new DateTime(2026, 4, 16, 6, 4, 53, 804, DateTimeKind.Utc).AddTicks(4981), null, null, 9, true, false, 9, "الصف الثالث الإعدادي", "Grade 3 Preparatory", "Preparatory", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GradeId",
                table: "AspNetUsers",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StudentCode",
                table: "AspNetUsers",
                column: "StudentCode",
                unique: true,
                filter: "[StudentCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_AttemptId",
                table: "AttemptAnswers",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_QuestionId",
                table: "AttemptAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_GradeId",
                table: "BookingRequests",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropAttempts_ItemId",
                table: "DragDropAttempts",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropAttempts_PlacedInZoneId",
                table: "DragDropAttempts",
                column: "PlacedInZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropAttempts_SessionId",
                table: "DragDropAttempts",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropGameSessions_DragDropQuestionId",
                table: "DragDropGameSessions",
                column: "DragDropQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropGameSessions_UserId",
                table: "DragDropGameSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropItems_CorrectZoneId",
                table: "DragDropItems",
                column: "CorrectZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropItems_DragDropQuestionId",
                table: "DragDropItems",
                column: "DragDropQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropQuestions_GradeId",
                table: "DragDropQuestions",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropZones_DragDropQuestionId",
                table: "DragDropZones",
                column: "DragDropQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardAttempts_SessionId",
                table: "FlipCardAttempts",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardGameSessions_FlipCardQuestionId",
                table: "FlipCardGameSessions",
                column: "FlipCardQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardGameSessions_UserId",
                table: "FlipCardGameSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardPairs_FlipCardQuestionId",
                table: "FlipCardPairs",
                column: "FlipCardQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardQuestions_GradeId",
                table: "FlipCardQuestions",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_Level",
                table: "Grades",
                column: "Level",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuestSessions_GradeId",
                table: "GuestSessions",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntries_GradeId",
                table: "LeaderboardEntries",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_UnitId",
                table: "Lessons",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingAttempts_SessionId",
                table: "MatchingAttempts",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingGamePairs_MatchingGameId",
                table: "MatchingGamePairs",
                column: "MatchingGameId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingGames_GradeId",
                table: "MatchingGames",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingGameSessions_MatchingGameId",
                table: "MatchingGameSessions",
                column: "MatchingGameId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingGameSessions_UserId",
                table: "MatchingGameSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingPairs_QuestionId",
                table: "MatchingPairs",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentChildren_ChildId",
                table: "ParentChildren",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_GradeId",
                table: "Questions",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProgress_GradeId",
                table: "StudentProgress",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProgress_UserId",
                table: "StudentProgress",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubQuestions_QuestionId",
                table: "SubQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_TestId",
                table: "TestAttempts",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_UserId",
                table: "TestAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestions_QuestionId",
                table: "TestQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestions_TestId_QuestionId",
                table: "TestQuestions",
                columns: new[] { "TestId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_GradeId",
                table: "Tests",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_LessonId",
                table: "Tests",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_UnitId",
                table: "Tests",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_GradeId",
                table: "Units",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_BadgeId",
                table: "UserBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_NotificationId",
                table: "UserNotifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId",
                table: "UserNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_PlanId",
                table: "UserSubscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WheelGameSessions_GradeId",
                table: "WheelGameSessions",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_WheelGameSessions_UserId",
                table: "WheelGameSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WheelQuestionAttempts_QuestionId",
                table: "WheelQuestionAttempts",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_WheelQuestionAttempts_SessionId",
                table: "WheelQuestionAttempts",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WheelQuestions_GradeId",
                table: "WheelQuestions",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_WheelSpinSegments_GradeId",
                table: "WheelSpinSegments",
                column: "GradeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AttemptAnswers");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BookingRequests");

            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "DragDropAttempts");

            migrationBuilder.DropTable(
                name: "FlipCardAttempts");

            migrationBuilder.DropTable(
                name: "FlipCardPairs");

            migrationBuilder.DropTable(
                name: "GuestSessions");

            migrationBuilder.DropTable(
                name: "LeaderboardEntries");

            migrationBuilder.DropTable(
                name: "MatchingAttempts");

            migrationBuilder.DropTable(
                name: "MatchingGamePairs");

            migrationBuilder.DropTable(
                name: "MatchingPairs");

            migrationBuilder.DropTable(
                name: "ParentChildren");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "StudentProgress");

            migrationBuilder.DropTable(
                name: "SubQuestions");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TestQuestions");

            migrationBuilder.DropTable(
                name: "UserBadges");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "WheelQuestionAttempts");

            migrationBuilder.DropTable(
                name: "WheelSpinSegments");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "TestAttempts");

            migrationBuilder.DropTable(
                name: "DragDropGameSessions");

            migrationBuilder.DropTable(
                name: "DragDropItems");

            migrationBuilder.DropTable(
                name: "FlipCardGameSessions");

            migrationBuilder.DropTable(
                name: "MatchingGameSessions");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropTable(
                name: "WheelGameSessions");

            migrationBuilder.DropTable(
                name: "WheelQuestions");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "DragDropZones");

            migrationBuilder.DropTable(
                name: "FlipCardQuestions");

            migrationBuilder.DropTable(
                name: "MatchingGames");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "DragDropQuestions");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Grades");
        }
    }
}
