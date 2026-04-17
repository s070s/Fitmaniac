using Fitmaniac.Application.Data;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Infrastructure.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Persistence;

public sealed class FitmaniacDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IApplicationDbContext
{
    private readonly AuditSaveChangesInterceptor _auditInterceptor;
    private readonly SoftDeleteInterceptor _softDeleteInterceptor;

    public FitmaniacDbContext(
        DbContextOptions<FitmaniacDbContext> options,
        AuditSaveChangesInterceptor auditInterceptor,
        SoftDeleteInterceptor softDeleteInterceptor)
        : base(options)
    {
        _auditInterceptor = auditInterceptor;
        _softDeleteInterceptor = softDeleteInterceptor;
    }

    public DbSet<Trainer> Trainers => Set<Trainer>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<TrainerClient> TrainerClients => Set<TrainerClient>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<Measurement> Measurements => Set<Measurement>();
    public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<ExerciseDefinition> ExerciseDefinitions => Set<ExerciseDefinition>();
    public DbSet<ExerciseEquipment> ExerciseEquipments => Set<ExerciseEquipment>();
    public DbSet<WeeklyProgram> WeeklyPrograms => Set<WeeklyProgram>();
    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<ClientWorkout> ClientWorkouts => Set<ClientWorkout>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
    public DbSet<WorkoutExerciseSet> WorkoutExerciseSets => Set<WorkoutExerciseSet>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<BillingTransaction> BillingTransactions => Set<BillingTransaction>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<ChatConversation> ChatConversations => Set<ChatConversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(FitmaniacDbContext).Assembly);

        // Global query filter: exclude soft-deleted entities
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType);
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
                var notDeleted = System.Linq.Expressions.Expression.Not(property);
                var lambda = System.Linq.Expressions.Expression.Lambda(notDeleted, parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor, _softDeleteInterceptor);
    }
}
