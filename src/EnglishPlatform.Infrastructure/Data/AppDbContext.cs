using EnglishPlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Infrastructure.Data;

/// <summary>
/// Main database context for the English Platform.
/// Extends IdentityDbContext for ASP.NET Identity integration.
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Content Hierarchy
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    // Questions
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<MatchingPair> MatchingPairs => Set<MatchingPair>();
    public DbSet<SubQuestion> SubQuestions => Set<SubQuestion>();

    // Tests
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<TestQuestion> TestQuestions => Set<TestQuestion>();
    public DbSet<TestAttempt> TestAttempts => Set<TestAttempt>();
    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();

    // Matching Game
    public DbSet<MatchingGame> MatchingGames => Set<MatchingGame>();
    public DbSet<MatchingGamePair> MatchingGamePairs => Set<MatchingGamePair>();
    public DbSet<MatchingGameSession> MatchingGameSessions => Set<MatchingGameSession>();
    public DbSet<MatchingAttempt> MatchingAttempts => Set<MatchingAttempt>();

    // Wheel Game
    public DbSet<WheelQuestion> WheelQuestions => Set<WheelQuestion>();
    public DbSet<WheelSpinSegment> WheelSpinSegments => Set<WheelSpinSegment>();
    public DbSet<WheelGameSession> WheelGameSessions => Set<WheelGameSession>();
    public DbSet<WheelQuestionAttempt> WheelQuestionAttempts => Set<WheelQuestionAttempt>();

    // Drag & Drop Game
    public DbSet<DragDropQuestion> DragDropQuestions => Set<DragDropQuestion>();
    public DbSet<DragDropZone> DragDropZones => Set<DragDropZone>();
    public DbSet<DragDropItem> DragDropItems => Set<DragDropItem>();
    public DbSet<DragDropGameSession> DragDropGameSessions => Set<DragDropGameSession>();
    public DbSet<DragDropAttempt> DragDropAttempts => Set<DragDropAttempt>();

    // Flip Card Game
    public DbSet<FlipCardQuestion> FlipCardQuestions => Set<FlipCardQuestion>();
    public DbSet<FlipCardPair> FlipCardPairs => Set<FlipCardPair>();
    public DbSet<FlipCardGameSession> FlipCardGameSessions => Set<FlipCardGameSession>();
    public DbSet<FlipCardAttempt> FlipCardAttempts => Set<FlipCardAttempt>();

    // Gamification
    public DbSet<StudentProgress> StudentProgress => Set<StudentProgress>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<LeaderboardEntry> LeaderboardEntries => Set<LeaderboardEntry>();

    // Auth & Users
    public DbSet<ParentChild> ParentChildren => Set<ParentChild>();
    public DbSet<GuestSession> GuestSessions => Set<GuestSession>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();

    // Booking & Contact
    public DbSet<BookingRequest> BookingRequests => Set<BookingRequest>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    // System
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ===== SQL Server: Disable cascading deletes globally =====
        // SQL Server does not allow multiple cascade paths. Set all FKs to NoAction.
        foreach (var relationship in builder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        // ===== Users & Auth =====
        builder.Entity<ApplicationUser>(e =>
        {
            e.HasIndex(u => u.StudentCode).IsUnique().HasFilter("[StudentCode] IS NOT NULL");
            e.Property(u => u.FirstName).HasMaxLength(100);
            e.Property(u => u.LastName).HasMaxLength(100);
            e.Property(u => u.AvatarUrl).HasMaxLength(500);
            e.Property(u => u.FacebookId).HasMaxLength(200);
            e.Property(u => u.StudentCode).HasMaxLength(20);
            e.Property(u => u.PreferredLanguage).HasMaxLength(5);
            e.Property(u => u.Role).HasMaxLength(20);

            e.HasOne(u => u.Grade)
                .WithMany(g => g.Students)
                .HasForeignKey(u => u.GradeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<ParentChild>(e =>
        {
            e.HasKey(pc => new { pc.ParentId, pc.ChildId });

            e.HasOne(pc => pc.Parent)
                .WithMany(u => u.ParentLinks)
                .HasForeignKey(pc => pc.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(pc => pc.Child)
                .WithMany(u => u.ChildLinks)
                .HasForeignKey(pc => pc.ChildId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<GuestSession>(e =>
        {
            e.Property(g => g.DisplayName).HasMaxLength(100);
            e.Property(g => g.SessionToken).HasMaxLength(500);
            e.HasOne(g => g.Grade).WithMany().HasForeignKey(g => g.GradeId);
        });

        builder.Entity<RefreshToken>(e =>
        {
            e.HasIndex(r => r.Token).IsUnique();
            e.Property(r => r.Token).HasMaxLength(500);
            e.HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== Content Hierarchy =====
        builder.Entity<Grade>(e =>
        {
            e.Property(g => g.NameAr).HasMaxLength(50);
            e.Property(g => g.NameEn).HasMaxLength(50);
            e.Property(g => g.SchoolType).HasMaxLength(20);
            e.HasIndex(g => g.Level).IsUnique();
        });

        builder.Entity<Unit>(e =>
        {
            e.Property(u => u.NameAr).HasMaxLength(200);
            e.Property(u => u.NameEn).HasMaxLength(200);
            e.HasOne(u => u.Grade).WithMany(g => g.Units).HasForeignKey(u => u.GradeId);
        });

        builder.Entity<Lesson>(e =>
        {
            e.Property(l => l.NameAr).HasMaxLength(200);
            e.Property(l => l.NameEn).HasMaxLength(200);
            e.HasOne(l => l.Unit).WithMany(u => u.Lessons).HasForeignKey(l => l.UnitId);
        });

        // ===== Questions =====
        builder.Entity<Question>(e =>
        {
            e.Property(q => q.QuestionText).HasMaxLength(2000);
            e.Property(q => q.InstructionAr).HasMaxLength(500);
            e.Property(q => q.ImageUrl).HasMaxLength(500);
            e.Property(q => q.AudioUrl).HasMaxLength(500);
            e.Property(q => q.VideoUrl).HasMaxLength(500);
            e.Property(q => q.PassageText).HasMaxLength(50000);
            e.Property(q => q.CorrectAnswer).HasMaxLength(500);
            e.Property(q => q.Explanation).HasMaxLength(2000);
            e.Property(q => q.HintText).HasMaxLength(500);
            e.HasOne(q => q.Grade).WithMany().HasForeignKey(q => q.GradeId);
            e.HasQueryFilter(q => !q.IsDeleted);
        });

        builder.Entity<QuestionOption>(e =>
        {
            e.Property(o => o.OptionText).HasMaxLength(500);
            e.Property(o => o.ImageUrl).HasMaxLength(500);
            e.HasOne(o => o.Question).WithMany(q => q.QuestionOptions).HasForeignKey(o => o.QuestionId);
        });

        builder.Entity<MatchingPair>(e =>
        {
            e.Property(m => m.LeftText).HasMaxLength(300);
            e.Property(m => m.RightText).HasMaxLength(300);
            e.Property(m => m.LeftImageUrl).HasMaxLength(500);
            e.Property(m => m.RightImageUrl).HasMaxLength(500);
            e.HasOne(m => m.Question).WithMany(q => q.MatchingPairs).HasForeignKey(m => m.QuestionId);
        });

        builder.Entity<SubQuestion>(e =>
        {
            e.Property(s => s.Text).HasMaxLength(1000);
            e.Property(s => s.CorrectAnswer).HasMaxLength(500);
            e.Property(s => s.Explanation).HasMaxLength(2000);
            e.HasOne(s => s.Question).WithMany(q => q.SubQuestions).HasForeignKey(s => s.QuestionId);
        });

        // ===== Tests =====
        builder.Entity<Test>(e =>
        {
            e.Property(t => t.TitleAr).HasMaxLength(300);
            e.Property(t => t.TitleEn).HasMaxLength(300);
            e.Property(t => t.DescriptionAr).HasMaxLength(1000);
            e.Property(t => t.DescriptionEn).HasMaxLength(1000);
            e.Property(t => t.Instructions).HasMaxLength(2000);
            e.HasOne(t => t.Grade).WithMany(g => g.Tests).HasForeignKey(t => t.GradeId);
            e.HasOne(t => t.Unit).WithMany().HasForeignKey(t => t.UnitId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.Lesson).WithMany().HasForeignKey(t => t.LessonId).OnDelete(DeleteBehavior.SetNull);
            e.HasQueryFilter(t => !t.IsDeleted);
        });

        builder.Entity<TestQuestion>(e =>
        {
            e.HasIndex(tq => new { tq.TestId, tq.QuestionId }).IsUnique();
            e.HasOne(tq => tq.Test).WithMany(t => t.TestQuestions).HasForeignKey(tq => tq.TestId);
            e.HasOne(tq => tq.Question).WithMany(q => q.TestQuestions).HasForeignKey(tq => tq.QuestionId);
        });

        builder.Entity<TestAttempt>(e =>
        {
            e.Property(a => a.TotalScore).HasPrecision(5, 2);
            e.Property(a => a.MaxPossibleScore).HasPrecision(5, 2);
            e.Property(a => a.Percentage).HasPrecision(5, 2);
            e.Property(a => a.IpAddress).HasMaxLength(50);
            e.Property(a => a.UserAgent).HasMaxLength(500);
            e.HasOne(a => a.Test).WithMany(t => t.TestAttempts).HasForeignKey(a => a.TestId);
            e.HasOne(a => a.User).WithMany(u => u.TestAttempts).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<AttemptAnswer>(e =>
        {
            e.Property(a => a.AnswerText).HasMaxLength(2000);
            e.Property(a => a.SelectedOptionIds).HasMaxLength(500);
            e.Property(a => a.PointsEarned).HasPrecision(5, 2);
            e.HasOne(a => a.Attempt).WithMany(ta => ta.AttemptAnswers).HasForeignKey(a => a.AttemptId);
            e.HasOne(a => a.Question).WithMany().HasForeignKey(a => a.QuestionId);
        });

        // ===== Matching Game =====
        builder.Entity<MatchingGame>(e =>
        {
            e.Property(m => m.GameTitle).HasMaxLength(500);
            e.Property(m => m.Instructions).HasMaxLength(1000);
            e.Property(m => m.UITheme).HasMaxLength(50);
            e.Property(m => m.Category).HasMaxLength(100);
            e.Property(m => m.ThumbnailUrl).HasMaxLength(500);
            e.HasOne(m => m.Grade).WithMany().HasForeignKey(m => m.GradeId);
            e.HasQueryFilter(m => !m.IsDeleted);
        });

        builder.Entity<MatchingGamePair>(e =>
        {
            e.Property(p => p.QuestionText).HasMaxLength(500);
            e.Property(p => p.AnswerText).HasMaxLength(500);
            e.Property(p => p.QuestionImageUrl).HasMaxLength(500);
            e.Property(p => p.AnswerImageUrl).HasMaxLength(500);
            e.Property(p => p.QuestionAudioUrl).HasMaxLength(500);
            e.Property(p => p.AnswerAudioUrl).HasMaxLength(500);
            e.Property(p => p.Explanation).HasMaxLength(1000);
            e.HasOne(p => p.MatchingGame).WithMany(m => m.Pairs).HasForeignKey(p => p.MatchingGameId);
        });

        builder.Entity<MatchingGameSession>(e =>
        {
            e.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(s => s.MatchingGame).WithMany(m => m.Sessions).HasForeignKey(s => s.MatchingGameId);
        });

        builder.Entity<MatchingAttempt>(e =>
        {
            e.HasOne(a => a.Session).WithMany(s => s.Attempts).HasForeignKey(a => a.SessionId);
        });

        // ===== Wheel Game =====
        builder.Entity<WheelQuestion>(e =>
        {
            e.Property(w => w.QuestionText).HasMaxLength(1000);
            e.Property(w => w.CorrectAnswer).HasMaxLength(500);
            e.Property(w => w.AudioUrl).HasMaxLength(500);
            e.Property(w => w.ImageUrl).HasMaxLength(500);
            e.Property(w => w.HintText).HasMaxLength(500);
            e.Property(w => w.CategoryTag).HasMaxLength(100);
            e.HasOne(w => w.Grade).WithMany().HasForeignKey(w => w.GradeId);
            e.HasQueryFilter(w => !w.IsDeleted);
        });

        builder.Entity<WheelSpinSegment>(e =>
        {
            e.Property(s => s.Label).HasMaxLength(100);
            e.Property(s => s.Color).HasMaxLength(20);
            e.HasOne(s => s.Grade).WithMany().HasForeignKey(s => s.GradeId);
        });

        builder.Entity<WheelGameSession>(e =>
        {
            e.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(s => s.Grade).WithMany().HasForeignKey(s => s.GradeId);
        });

        builder.Entity<WheelQuestionAttempt>(e =>
        {
            e.Property(a => a.SegmentLanded).HasMaxLength(100);
            e.Property(a => a.SelectedAnswer).HasMaxLength(500);
            e.HasOne(a => a.Session).WithMany(s => s.Attempts).HasForeignKey(a => a.SessionId);
            e.HasOne(a => a.Question).WithMany().HasForeignKey(a => a.QuestionId).OnDelete(DeleteBehavior.NoAction);
        });

        // ===== Drag & Drop Game =====
        builder.Entity<DragDropQuestion>(e =>
        {
            e.Property(d => d.GameTitle).HasMaxLength(500);
            e.Property(d => d.Instructions).HasMaxLength(1000);
            e.Property(d => d.UITheme).HasMaxLength(50);
            e.Property(d => d.ThumbnailUrl).HasMaxLength(500);
            e.HasOne(d => d.Grade).WithMany().HasForeignKey(d => d.GradeId);
            e.HasQueryFilter(d => !d.IsDeleted);
        });

        builder.Entity<DragDropZone>(e =>
        {
            e.Property(z => z.ZoneLabel).HasMaxLength(200);
            e.Property(z => z.ZoneColor).HasMaxLength(20);
            e.Property(z => z.ImageUrl).HasMaxLength(500);
            e.Property(z => z.Description).HasMaxLength(500);
            e.HasOne(z => z.DragDropQuestion).WithMany(d => d.Zones).HasForeignKey(z => z.DragDropQuestionId);
        });

        builder.Entity<DragDropItem>(e =>
        {
            e.Property(i => i.ItemText).HasMaxLength(200);
            e.Property(i => i.ItemImageUrl).HasMaxLength(500);
            e.Property(i => i.ItemAudioUrl).HasMaxLength(500);
            e.Property(i => i.Explanation).HasMaxLength(500);
            e.HasOne(i => i.DragDropQuestion).WithMany(d => d.Items).HasForeignKey(i => i.DragDropQuestionId);
            e.HasOne(i => i.CorrectZone).WithMany(z => z.CorrectItems).HasForeignKey(i => i.CorrectZoneId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<DragDropGameSession>(e =>
        {
            e.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(s => s.DragDropQuestion).WithMany(d => d.Sessions).HasForeignKey(s => s.DragDropQuestionId);
        });

        builder.Entity<DragDropAttempt>(e =>
        {
            e.HasOne(a => a.Session).WithMany(s => s.Attempts).HasForeignKey(a => a.SessionId);
            e.HasOne(a => a.Item).WithMany().HasForeignKey(a => a.ItemId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.PlacedInZone).WithMany().HasForeignKey(a => a.PlacedInZoneId).OnDelete(DeleteBehavior.Restrict);
        });

        // ===== Flip Card Game =====
        builder.Entity<FlipCardQuestion>(e =>
        {
            e.Property(f => f.GameTitle).HasMaxLength(500);
            e.Property(f => f.Instructions).HasMaxLength(1000);
            e.Property(f => f.UITheme).HasMaxLength(50);
            e.Property(f => f.CardBackDesign).HasMaxLength(50);
            e.Property(f => f.CustomCardBackUrl).HasMaxLength(500);
            e.Property(f => f.Category).HasMaxLength(100);
            e.Property(f => f.ThumbnailUrl).HasMaxLength(500);
            e.HasOne(f => f.Grade).WithMany().HasForeignKey(f => f.GradeId);
            e.HasQueryFilter(f => !f.IsDeleted);
        });

        builder.Entity<FlipCardPair>(e =>
        {
            e.Property(p => p.Card1Text).HasMaxLength(500);
            e.Property(p => p.Card2Text).HasMaxLength(500);
            e.Property(p => p.Card1ImageUrl).HasMaxLength(500);
            e.Property(p => p.Card2ImageUrl).HasMaxLength(500);
            e.Property(p => p.Card1AudioUrl).HasMaxLength(500);
            e.Property(p => p.Card2AudioUrl).HasMaxLength(500);
            e.Property(p => p.Explanation).HasMaxLength(1000);
            e.HasOne(p => p.FlipCardQuestion).WithMany(f => f.Pairs).HasForeignKey(p => p.FlipCardQuestionId);
        });

        builder.Entity<FlipCardGameSession>(e =>
        {
            e.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(s => s.FlipCardQuestion).WithMany(f => f.Sessions).HasForeignKey(s => s.FlipCardQuestionId);
        });

        builder.Entity<FlipCardAttempt>(e =>
        {
            e.HasOne(a => a.Session).WithMany(s => s.Attempts).HasForeignKey(a => a.SessionId);
        });

        // ===== Gamification =====
        builder.Entity<StudentProgress>(e =>
        {
            e.HasKey(sp => new { sp.UserId, sp.GradeId });
            e.Property(sp => sp.AverageScore).HasPrecision(5, 2);
            e.HasOne(sp => sp.User).WithOne(u => u.StudentProgress).HasForeignKey<StudentProgress>(sp => sp.UserId);
            e.HasOne(sp => sp.Grade).WithMany().HasForeignKey(sp => sp.GradeId);
        });

        builder.Entity<Badge>(e =>
        {
            e.Property(b => b.NameAr).HasMaxLength(200);
            e.Property(b => b.NameEn).HasMaxLength(200);
            e.Property(b => b.DescriptionAr).HasMaxLength(500);
            e.Property(b => b.DescriptionEn).HasMaxLength(500);
            e.Property(b => b.IconUrl).HasMaxLength(500);
            e.Property(b => b.Icon).HasMaxLength(10);
        });

        builder.Entity<UserBadge>(e =>
        {
            e.HasKey(ub => new { ub.UserId, ub.BadgeId });
            e.HasOne(ub => ub.User).WithMany(u => u.UserBadges).HasForeignKey(ub => ub.UserId);
            e.HasOne(ub => ub.Badge).WithMany(b => b.UserBadges).HasForeignKey(ub => ub.BadgeId);
        });

        builder.Entity<LeaderboardEntry>(e =>
        {
            e.HasKey(le => new { le.UserId, le.GradeId });
            e.Property(le => le.DisplayName).HasMaxLength(200);
            e.Property(le => le.AvatarUrl).HasMaxLength(500);
            e.HasOne(le => le.User).WithMany().HasForeignKey(le => le.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(le => le.Grade).WithMany().HasForeignKey(le => le.GradeId);
        });

        // ===== Notifications =====
        builder.Entity<Notification>(e =>
        {
            e.Property(n => n.Title).HasMaxLength(300);
            e.Property(n => n.Body).HasMaxLength(1000);
        });

        builder.Entity<UserNotification>(e =>
        {
            e.HasOne(un => un.User).WithMany(u => u.UserNotifications).HasForeignKey(un => un.UserId);
            e.HasOne(un => un.Notification).WithMany(n => n.UserNotifications).HasForeignKey(un => un.NotificationId);
        });

        // ===== Booking & Contact =====
        builder.Entity<BookingRequest>(e =>
        {
            e.Property(b => b.ParentName).HasMaxLength(200);
            e.Property(b => b.StudentName).HasMaxLength(200);
            e.Property(b => b.Email).HasMaxLength(300);
            e.Property(b => b.Phone).HasMaxLength(30);
            e.Property(b => b.PreferredDates).HasMaxLength(500);
            e.Property(b => b.Message).HasMaxLength(1000);
            e.Property(b => b.AdminNotes).HasMaxLength(500);
            e.HasOne(b => b.Grade).WithMany().HasForeignKey(b => b.GradeId);
        });

        builder.Entity<ContactMessage>(e =>
        {
            e.Property(c => c.SenderName).HasMaxLength(200);
            e.Property(c => c.SenderEmail).HasMaxLength(300);
            e.Property(c => c.SenderPhone).HasMaxLength(30);
            e.Property(c => c.Subject).HasMaxLength(300);
            e.Property(c => c.Message).HasMaxLength(2000);
        });

        // ===== System =====
        builder.Entity<AuditLog>(e =>
        {
            e.Property(a => a.UserName).HasMaxLength(200);
            e.Property(a => a.Action).HasMaxLength(50);
            e.Property(a => a.EntityType).HasMaxLength(100);
            e.Property(a => a.EntityId).HasMaxLength(50);
            e.Property(a => a.IpAddress).HasMaxLength(50);
            e.HasIndex(a => a.Timestamp);
        });

        builder.Entity<SystemSetting>(e =>
        {
            e.HasIndex(s => s.Key).IsUnique();
            e.Property(s => s.Key).HasMaxLength(100);
            e.Property(s => s.Value).HasMaxLength(2000);
            e.Property(s => s.Description).HasMaxLength(500);
        });

        builder.Entity<SubscriptionPlan>(e =>
        {
            e.Property(sp => sp.NameAr).HasMaxLength(200);
            e.Property(sp => sp.NameEn).HasMaxLength(200);
            e.Property(sp => sp.PriceEGP).HasPrecision(10, 2);
        });

        builder.Entity<UserSubscription>(e =>
        {
            e.HasOne(us => us.User).WithMany(u => u.UserSubscriptions).HasForeignKey(us => us.UserId);
            e.HasOne(us => us.Plan).WithMany(p => p.UserSubscriptions).HasForeignKey(us => us.PlanId);
        });

        // ===== Seed Data =====
        SeedGrades(builder);
        SeedBadges(builder);
    }

    private static void SeedGrades(ModelBuilder builder)
    {
        builder.Entity<Grade>().HasData(
            new Grade { Id = 1, NameAr = "الصف الأول الابتدائي", NameEn = "Grade 1 Primary", Level = 1, SchoolType = "Primary", DisplayOrder = 1, IsActive = true },
            new Grade { Id = 2, NameAr = "الصف الثاني الابتدائي", NameEn = "Grade 2 Primary", Level = 2, SchoolType = "Primary", DisplayOrder = 2, IsActive = true },
            new Grade { Id = 3, NameAr = "الصف الثالث الابتدائي", NameEn = "Grade 3 Primary", Level = 3, SchoolType = "Primary", DisplayOrder = 3, IsActive = true },
            new Grade { Id = 4, NameAr = "الصف الرابع الابتدائي", NameEn = "Grade 4 Primary", Level = 4, SchoolType = "Primary", DisplayOrder = 4, IsActive = true },
            new Grade { Id = 5, NameAr = "الصف الخامس الابتدائي", NameEn = "Grade 5 Primary", Level = 5, SchoolType = "Primary", DisplayOrder = 5, IsActive = true },
            new Grade { Id = 6, NameAr = "الصف السادس الابتدائي", NameEn = "Grade 6 Primary", Level = 6, SchoolType = "Primary", DisplayOrder = 6, IsActive = true },
            new Grade { Id = 7, NameAr = "الصف الأول الإعدادي", NameEn = "Grade 1 Preparatory", Level = 7, SchoolType = "Preparatory", DisplayOrder = 7, IsActive = true },
            new Grade { Id = 8, NameAr = "الصف الثاني الإعدادي", NameEn = "Grade 2 Preparatory", Level = 8, SchoolType = "Preparatory", DisplayOrder = 8, IsActive = true },
            new Grade { Id = 9, NameAr = "الصف الثالث الإعدادي", NameEn = "Grade 3 Preparatory", Level = 9, SchoolType = "Preparatory", DisplayOrder = 9, IsActive = true }
        );
    }

    private static void SeedBadges(ModelBuilder builder)
    {
        builder.Entity<Badge>().HasData(
            new Badge { Id = 1, NameAr = "الخطوة الأولى", NameEn = "First Steps", DescriptionAr = "أكمل أول اختبار", DescriptionEn = "Complete your first test", Icon = "🎯", BadgeType = Domain.Enums.BadgeType.FirstTest, BonusPoints = 10, IsActive = true },
            new Badge { Id = 2, NameAr = "العلامة الكاملة", NameEn = "Perfect Score", DescriptionAr = "احصل على 100% في أي اختبار", DescriptionEn = "Get 100% on any test", Icon = "⭐", BadgeType = Domain.Enums.BadgeType.PerfectScore, BonusPoints = 50, IsActive = true },
            new Badge { Id = 3, NameAr = "سلسلة 7 أيام", NameEn = "7 Day Streak", DescriptionAr = "نشاط 7 أيام متتالية", DescriptionEn = "7 consecutive days of activity", Icon = "🔥", BadgeType = Domain.Enums.BadgeType.Streak7, BonusPoints = 25, CriteriaType = "Streak", CriteriaValue = 7, IsActive = true },
            new Badge { Id = 4, NameAr = "سلسلة 30 يوم", NameEn = "30 Day Streak", DescriptionAr = "نشاط 30 يوم متتالي", DescriptionEn = "30 consecutive days of activity", Icon = "💪", BadgeType = Domain.Enums.BadgeType.Streak30, BonusPoints = 100, CriteriaType = "Streak", CriteriaValue = 30, IsActive = true },
            new Badge { Id = 5, NameAr = "ضمن أفضل 3", NameEn = "Top 3", DescriptionAr = "احتل المركز الثالث أو أعلى", DescriptionEn = "Reach top 3 on grade leaderboard", Icon = "🥉", BadgeType = Domain.Enums.BadgeType.Top3Grade, BonusPoints = 50, IsActive = true },
            new Badge { Id = 6, NameAr = "المركز الأول", NameEn = "Champion", DescriptionAr = "احتل المركز الأول", DescriptionEn = "Reach #1 on grade leaderboard", Icon = "🏆", BadgeType = Domain.Enums.BadgeType.Top1Grade, BonusPoints = 100, IsActive = true },
            new Badge { Id = 7, NameAr = "محترف الألعاب", NameEn = "Game Master", DescriptionAr = "أكمل جميع أنواع الألعاب", DescriptionEn = "Complete all 4 game types", Icon = "🎮", BadgeType = Domain.Enums.BadgeType.GameMaster, BonusPoints = 40, IsActive = true },
            new Badge { Id = 8, NameAr = "سريع البرق", NameEn = "Speed Runner", DescriptionAr = "أنهِ اختباراً في أقل من 30% من الوقت مع 80%+", DescriptionEn = "Complete a timed test in <30% time with 80%+", Icon = "⚡", BadgeType = Domain.Enums.BadgeType.SpeedRunner, BonusPoints = 30, IsActive = true },
            new Badge { Id = 9, NameAr = "المستكشف", NameEn = "Explorer", DescriptionAr = "أكمل اختبارات من 5 وحدات مختلفة", DescriptionEn = "Complete tests from 5 different units", Icon = "🧭", BadgeType = Domain.Enums.BadgeType.Curious, BonusPoints = 35, CriteriaType = "UnitCount", CriteriaValue = 5, IsActive = true },
            new Badge { Id = 10, NameAr = "متفوق الوحدات", NameEn = "All Units", DescriptionAr = "أكمل اختباراً في كل وحدة", DescriptionEn = "Complete at least one test in every unit", Icon = "📚", BadgeType = Domain.Enums.BadgeType.AllUnits, BonusPoints = 75, IsActive = true }
        );
    }

    /// <summary>
    /// Automatic soft-delete and audit fields on SaveChanges.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
