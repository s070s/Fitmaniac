namespace Fitmaniac.Shared.Constants;

public static class RouteConstants
{
    // Auth
    public const string AuthRegister = "api/auth/register";
    public const string AuthLogin = "api/auth/login";
    public const string AuthRefresh = "api/auth/refresh";
    public const string AuthLogout = "api/auth/logout";
    public const string AuthForgotPassword = "api/auth/forgot-password";
    public const string AuthResetPassword = "api/auth/reset-password";
    public const string AuthConfirmEmail = "api/auth/confirm-email";
    public const string AuthChangePassword = "api/auth/change-password";

    // Users
    public const string UsersMe = "api/users/me";
    public const string UsersUploadPhoto = "api/users/{0}/upload-photo";
    public const string UsersUpdateProfile = "api/users/{0}/profile";

    // Admin Users
    public const string AdminUsers = "api/admin/users";
    public const string AdminUsersStatistics = "api/admin/users/statistics";
    public const string AdminUserById = "api/admin/users/{0}";
    public const string AdminUserEnable = "api/admin/users/{0}/enable";
    public const string AdminUserDisable = "api/admin/users/{0}/disable";

    // Trainers
    public const string Trainers = "api/trainers";
    public const string TrainerById = "api/trainers/{0}";
    public const string AdminTrainers = "api/admin/trainers";
    public const string AdminTrainerById = "api/admin/trainers/{0}";

    // Clients
    public const string ClientSubscribe = "api/clients/{0}/subscribe/{1}";
    public const string ClientUnsubscribe = "api/clients/{0}/unsubscribe/{1}";
    public const string ClientSubscriptions = "api/clients/{0}/subscriptions";
    public const string ClientById = "api/clients/{0}";

    // Workouts
    public const string WorkoutsMy = "api/workouts/my";
    public const string WorkoutById = "api/workouts/{0}";
    public const string WorkoutComplete = "api/workouts/{0}/complete";
    public const string AdminWorkouts = "api/admin/workouts";
    public const string AdminWorkoutById = "api/admin/workouts/{0}";
    public const string AdminWorkoutExercises = "api/admin/workouts/{0}/exercises";
    public const string AdminWorkoutExerciseById = "api/admin/workouts/exercises/{0}";
    public const string AdminWorkoutExerciseSets = "api/admin/workouts/exercises/{0}/sets";
    public const string AdminWorkoutSetById = "api/admin/workouts/sets/{0}";

    // Exercises
    public const string Exercises = "api/exercises";
    public const string ExerciseById = "api/exercises/{0}";

    // Equipment
    public const string Equipment = "api/equipment";
    public const string EquipmentById = "api/equipment/{0}";

    // Programs
    public const string ProgramsMy = "api/programs/my";
    public const string ProgramById = "api/programs/{0}";
    public const string Programs = "api/programs";
    public const string ProgramAdvanceWeek = "api/programs/{0}/advance-week";

    // Goals
    public const string GoalsMy = "api/goals/my";
    public const string GoalById = "api/goals/{0}";
    public const string Goals = "api/goals";

    // Measurements
    public const string MeasurementsMy = "api/measurements/my";
    public const string MeasurementById = "api/measurements/{0}";
    public const string Measurements = "api/measurements";

    // Medical History
    public const string MedicalHistoryMy = "api/medical-history/my";
    public const string MedicalHistory = "api/medical-history";

    // Subscriptions
    public const string SubscriptionPlans = "api/subscription/plans";
    public const string SubscriptionCurrent = "api/subscription/current";
    public const string SubscriptionUpgrade = "api/subscription/upgrade";

    // Billing
    public const string BillingTransactions = "api/billing/transactions";

    // Progress
    public const string ProgressSummary = "api/progress/summary";
    public const string ProgressWeekly = "api/progress/weekly";

    // Media
    public const string MediaAvatars = "api/media/avatars/{0}";
    public const string MediaExercises = "api/media/exercises/{0}";

    // Chat
    public const string ChatConversations = "api/chat/conversations";
    public const string ChatConversationMessages = "api/chat/conversations/{0}/messages";
    public const string ChatSendMessage = "api/chat/conversations/{0}/messages";
    public const string ChatReadMessages = "api/chat/conversations/{0}/read";

    // Hubs
    public const string ChatHub = "/hubs/chat";
}
