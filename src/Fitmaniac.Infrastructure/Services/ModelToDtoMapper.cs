using Fitmaniac.Application.Mapping;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.Chat;
using Fitmaniac.Shared.DTOs.Clients;
using Fitmaniac.Shared.DTOs.Exercises;
using Fitmaniac.Shared.DTOs.Goals;
using Fitmaniac.Shared.DTOs.Measurements;
using Fitmaniac.Shared.DTOs.MedicalHistory;
using Fitmaniac.Shared.DTOs.Programs;
using Fitmaniac.Shared.DTOs.Subscriptions;
using Fitmaniac.Shared.DTOs.Trainers;
using Fitmaniac.Shared.DTOs.Users;
using Fitmaniac.Shared.DTOs.Workouts;

namespace Fitmaniac.Infrastructure.Services;

public sealed class ModelToDtoMapper : IModelToDtoMapper
{
    public UserDto? ToDto(ApplicationUser? user)
    {
        if (user is null) return null;
        return new UserDto(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            user.Role.ToString(),
            user.IsEnabled,
            user.Status,
            user.CreatedUtc,
            user.LastLoginUtc,
            ToDto(user.TrainerProfile),
            ToDto(user.ClientProfile));
    }

    public TrainerDto? ToDto(Trainer? trainer)
    {
        if (trainer is null) return null;
        return new TrainerDto(
            trainer.Id,
            trainer.UserId,
            trainer.FirstName,
            trainer.LastName,
            trainer.ProfilePhotoUrl,
            trainer.Bio,
            trainer.Specializations.ToList(),
            trainer.Age,
            trainer.City,
            trainer.Country);
    }

    public TrainerListItemDto? ToListItemDto(Trainer? trainer)
    {
        if (trainer is null) return null;
        return new TrainerListItemDto(
            trainer.Id,
            trainer.UserId,
            trainer.FirstName,
            trainer.LastName,
            trainer.ProfilePhotoUrl,
            trainer.Specializations.ToList(),
            trainer.City);
    }

    public ClientDto? ToDto(Client? client)
    {
        if (client is null) return null;
        return new ClientDto(
            client.Id,
            client.UserId,
            client.FirstName,
            client.LastName,
            client.ProfilePhotoUrl,
            client.Bio,
            client.ExperienceLevel,
            client.PreferredIntensityLevel,
            client.Age,
            client.Weight,
            client.Height,
            client.BMI,
            client.BMR,
            client.City,
            client.Country);
    }

    public WorkoutDto? ToDto(Workout? workout, int? requestingUserId = null)
    {
        if (workout is null) return null;
        return new WorkoutDto(
            workout.Id,
            workout.ScheduledDateTime,
            workout.Type,
            workout.DurationInMinutes,
            workout.Notes,
            workout.TrainerId,
            workout.WeeklyProgramId,
            workout.WorkoutExercises.Select(we => ToDto(we)!).ToList(),
            workout.Clients.Select(c => c.Id).ToList());
    }

    public WorkoutListItemDto? ToListItemDto(Workout? workout, int? requestingUserId = null)
    {
        if (workout is null) return null;
        var completedByMe = requestingUserId.HasValue &&
            workout.Clients.Any(c => c.UserId == requestingUserId.Value);
        return new WorkoutListItemDto(
            workout.Id,
            workout.ScheduledDateTime,
            workout.Type,
            workout.DurationInMinutes,
            workout.TrainerId,
            workout.WeeklyProgramId,
            completedByMe);
    }

    public WorkoutExerciseDto? ToDto(WorkoutExercise? we)
    {
        if (we is null) return null;
        return new WorkoutExerciseDto(
            we.Id,
            we.WorkoutId,
            we.ExerciseDefinitionId,
            we.ExerciseDefinition?.Name ?? string.Empty,
            we.Sets.Select(s => ToDto(s)!).ToList(),
            we.Notes);
    }

    public WorkoutExerciseSetDto? ToDto(WorkoutExerciseSet? set)
    {
        if (set is null) return null;
        return new WorkoutExerciseSetDto(
            set.Id,
            set.WorkoutExerciseId,
            set.SetNumber,
            set.Repetitions,
            set.Weight,
            set.GoalUnit,
            set.OverallIntensityLevel,
            set.DurationInSeconds,
            set.RestPeriodInSeconds,
            set.Notes);
    }

    public ExerciseDefinitionDto? ToDto(ExerciseDefinition? ex)
    {
        if (ex is null) return null;
        return new ExerciseDefinitionDto(
            ex.Id,
            ex.Name,
            ex.Description,
            ex.VideoUrl,
            ex.CaloriesBurnedPerHour,
            ex.IsCompoundExercise,
            ex.PrimaryMuscleGroups.ToList(),
            ex.SecondaryMuscleGroups.ToList(),
            ex.ExperienceLevel,
            ex.Category,
            ex.Equipments.Select(e => ToDto(e)!).ToList());
    }

    public EquipmentDto? ToDto(Equipment? equipment)
    {
        if (equipment is null) return null;
        return new EquipmentDto(equipment.Id, equipment.Name, equipment.Description);
    }

    public WeeklyProgramDto? ToDto(WeeklyProgram? program)
    {
        if (program is null) return null;
        return new WeeklyProgramDto(
            program.Id,
            program.Name,
            program.Description,
            program.DurationInWeeks,
            program.CurrentWeek,
            program.IsCompleted,
            program.ClientId);
    }

    public GoalDto? ToDto(Goal? goal)
    {
        if (goal is null) return null;
        return new GoalDto(
            goal.Id,
            goal.ClientId,
            goal.GoalType,
            goal.Description,
            goal.TargetDate,
            goal.Status,
            goal.GoalQuantity,
            goal.GoalUnit);
    }

    public MeasurementDto? ToDto(Measurement? measurement)
    {
        if (measurement is null) return null;
        return new MeasurementDto(
            measurement.Id,
            measurement.ClientId,
            measurement.Unit,
            measurement.Value,
            measurement.Date,
            measurement.Intensity,
            measurement.IsPersonalBest,
            measurement.Notes);
    }

    public MedicalHistoryDto? ToDto(MedicalHistory? history)
    {
        if (history is null) return null;
        return new MedicalHistoryDto(
            history.Id,
            history.ClientId,
            history.Description,
            history.Conditions.ToList(),
            history.MedicationTypes.ToList(),
            history.Surgeries.ToList(),
            history.RecommendedIntensityLevel);
    }

    public SubscriptionPlanDto? ToDto(SubscriptionPlan? plan)
    {
        if (plan is null) return null;
        return new SubscriptionPlanDto(
            plan.Id,
            plan.Name,
            plan.Description,
            plan.Price,
            plan.BillingPeriod,
            plan.SubscriptionTier,
            plan.FeaturesJson,
            plan.IsActive);
    }

    public UserSubscriptionDto? ToDto(UserSubscription? sub)
    {
        if (sub is null) return null;
        return new UserSubscriptionDto(
            sub.Id,
            sub.UserId,
            sub.SubscriptionPlanId,
            sub.SubscriptionPlan?.Name ?? string.Empty,
            sub.SubscriptionPlan?.SubscriptionTier ?? Domain.Enums.SubscriptionTier.Free,
            sub.StartDate,
            sub.EndDate,
            sub.IsActive);
    }

    public BillingTransactionDto? ToDto(BillingTransaction? tx)
    {
        if (tx is null) return null;
        return new BillingTransactionDto(
            tx.Id,
            tx.UserId,
            tx.SubscriptionPlanId,
            tx.SubscriptionPlan?.Name,
            tx.Amount,
            tx.TransactionReference,
            tx.Status,
            tx.CreatedUtc);
    }

    public ChatConversationDto? ToDto(ChatConversation? convo)
    {
        if (convo is null) return null;
        return new ChatConversationDto(
            convo.Id,
            convo.CreatorId,
            convo.Creator?.UserName ?? string.Empty,
            convo.ParticipantId,
            convo.Participant?.UserName ?? string.Empty,
            convo.LastMessageContent,
            convo.LastMessageAt);
    }

    public ChatMessageDto? ToDto(ChatMessage? msg)
    {
        if (msg is null) return null;
        return new ChatMessageDto(
            msg.Id,
            msg.ConversationId,
            msg.UserId,
            msg.User?.UserName ?? string.Empty,
            msg.Content,
            msg.SenderRole.ToString(),
            msg.SentAt,
            msg.ReadAt);
    }
}
