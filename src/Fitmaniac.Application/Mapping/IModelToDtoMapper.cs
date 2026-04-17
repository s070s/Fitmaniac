using Fitmaniac.Domain.Entities;
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
using Fitmaniac.Shared.DTOs.Chat;

namespace Fitmaniac.Application.Mapping;

public interface IModelToDtoMapper
{
    UserDto? ToDto(ApplicationUser? user);
    TrainerDto? ToDto(Trainer? trainer);
    TrainerListItemDto? ToListItemDto(Trainer? trainer);
    ClientDto? ToDto(Client? client);
    WorkoutDto? ToDto(Workout? workout, int? requestingUserId = null);
    WorkoutListItemDto? ToListItemDto(Workout? workout, int? requestingUserId = null);
    WorkoutExerciseDto? ToDto(WorkoutExercise? we);
    WorkoutExerciseSetDto? ToDto(WorkoutExerciseSet? set);
    ExerciseDefinitionDto? ToDto(ExerciseDefinition? ex);
    EquipmentDto? ToDto(Equipment? equipment);
    WeeklyProgramDto? ToDto(WeeklyProgram? program);
    GoalDto? ToDto(Goal? goal);
    MeasurementDto? ToDto(Measurement? measurement);
    MedicalHistoryDto? ToDto(MedicalHistory? history);
    SubscriptionPlanDto? ToDto(SubscriptionPlan? plan);
    UserSubscriptionDto? ToDto(UserSubscription? sub);
    BillingTransactionDto? ToDto(BillingTransaction? tx);
    ChatConversationDto? ToDto(ChatConversation? convo);
    ChatMessageDto? ToDto(ChatMessage? msg);
}
