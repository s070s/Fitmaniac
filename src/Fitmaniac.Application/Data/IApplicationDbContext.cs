using Fitmaniac.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Application.Data;

public interface IApplicationDbContext
{
    DbSet<Trainer> Trainers { get; }
    DbSet<Client> Clients { get; }
    DbSet<TrainerClient> TrainerClients { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Goal> Goals { get; }
    DbSet<Measurement> Measurements { get; }
    DbSet<MedicalHistory> MedicalHistories { get; }
    DbSet<Equipment> Equipments { get; }
    DbSet<ExerciseDefinition> ExerciseDefinitions { get; }
    DbSet<ExerciseEquipment> ExerciseEquipments { get; }
    DbSet<WeeklyProgram> WeeklyPrograms { get; }
    DbSet<Workout> Workouts { get; }
    DbSet<ClientWorkout> ClientWorkouts { get; }
    DbSet<WorkoutExercise> WorkoutExercises { get; }
    DbSet<WorkoutExerciseSet> WorkoutExerciseSets { get; }
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    DbSet<UserSubscription> UserSubscriptions { get; }
    DbSet<BillingTransaction> BillingTransactions { get; }
    DbSet<SiteSetting> SiteSettings { get; }
    DbSet<ChatConversation> ChatConversations { get; }
    DbSet<ChatMessage> ChatMessages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
