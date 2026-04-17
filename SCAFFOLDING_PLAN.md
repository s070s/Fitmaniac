# Fitmaniac — Scaffolding Plan

> **Target path:** `C:\Users\GS\Desktop\ResetYourFutureToFitmaniac\Fitmaniac`
> **Target framework:** `net10.0` across every project (MAUI included).
> **SDK pin:** `10.0.100` via `global.json` (`rollForward: latestFeature`).
> **Primary references:**
> - **Architecture:** `ResetYourFuture` (Clean Architecture: Domain / Application / Infrastructure / Shared / Web; Blazor Server InteractiveServer; ASP.NET Core Identity; JWT + cookie multi-auth).
> - **Domain:** `FullStackTest` (Trainer/Client fitness domain: Workouts, Exercises, Programs, Goals, Measurements, Medical History, Equipment; JWT access + HttpOnly refresh cookie).
>
> **Fitmaniac splits Web (Blazor Server) and Api (REST)** — this diverges from ResetYourFuture (which hosts controllers inside Web) to let MAUI and Web share one API surface, matching FullStackTest's split. This is an explicit, justified divergence.
>
> **Storage decision:** SQL Server (LocalDB for dev, full SQL Server for prod) across all environments — matches ResetYourFuture and user requirement. Single EF provider (`UseSqlServer`); SQLite is not wired.
>
> **Items tagged `[NEEDS CLARIFICATION]`** are things neither reference project resolved and the user must decide before scaffolding proceeds. None block `2l` from running — each has a documented fallback.

---

## 2a. Solution Architecture

### Projects

| # | Project | Type | SDK | TargetFramework | Role |
|---|---|---|---|---|---|
| 1 | `Fitmaniac.Domain` | Class Library | `Microsoft.NET.Sdk` | `net10.0` | Pure entities, enums, domain exceptions, domain extensions. No framework deps except `Microsoft.AspNetCore.App` framework reference (to use `IdentityUser` as entity base — mirrors ResetYourFuture). |
| 2 | `Fitmaniac.Application` | Class Library | `Microsoft.NET.Sdk` | `net10.0` | Service interfaces, DTOs, Result types, `IApplicationDbContext`, seed DTOs, business-logic orchestration interfaces. References Domain. Framework reference on `Microsoft.AspNetCore.App`. |
| 3 | `Fitmaniac.Infrastructure` | Class Library | `Microsoft.NET.Sdk` | `net10.0` | `FitmaniacDbContext`, Fluent configurations, migrations, Identity wiring, `TokenService`, `LocalFileStorage`, `StubEmailService`, seeders. References Application + Domain. |
| 4 | `Fitmaniac.Shared` | Class Library | `Microsoft.NET.Sdk` | `net10.0` | DTOs, contracts, enums, constants, resource (`.resx`) files, JSON seed files (embedded). Consumed by Api, Web, MAUI. No project references. |
| 5 | `Fitmaniac.Api` | Web (REST) | `Microsoft.NET.Sdk.Web` | `net10.0` | ASP.NET Core minimal-API endpoints + MVC controllers (mixed, as in RYF). Hosts all JWT-protected REST. References Application, Infrastructure, Shared, Domain. |
| 6 | `Fitmaniac.Web` | Blazor Server | `Microsoft.NET.Sdk.Web` | `net10.0` | Blazor Server (`InteractiveServerRenderMode(prerender: true)`). Consumes `Fitmaniac.Api` via typed `HttpClient` consumers + SSR cookie→JWT handler (pattern from RYF `SsrApiHandler`). References Shared + Application (for interfaces only) + Domain (for enums). |
| 7 | `Fitmaniac.MAUI` | MAUI App | `Microsoft.NET.Sdk` (`<UseMaui>true</UseMaui>`) | `net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows10.0.19041.0` | Mobile app for Android/iOS/Windows consuming `Fitmaniac.Api`. References Shared + Domain (enums only). |

### Inter-project reference graph

```
Fitmaniac.Shared   ◄─────────────────────────────────────┐
                                                         │
Fitmaniac.Domain   ◄──┐                                  │
                      │                                  │
Fitmaniac.Application ◄── Fitmaniac.Infrastructure       │
     ▲                           ▲                       │
     │                           │                       │
     └──────── Fitmaniac.Api ◄───┘                       │
                    ▲                                    │
                    │ (HTTP only, not a project ref)     │
                    │                                    │
              Fitmaniac.Web ─────────────────────────────┤
                                                         │
              Fitmaniac.MAUI ────────────────────────────┘
```

Explicit references:
- `Fitmaniac.Application` → `Fitmaniac.Domain`, `Fitmaniac.Shared`
- `Fitmaniac.Infrastructure` → `Fitmaniac.Application`, `Fitmaniac.Domain`, `Fitmaniac.Shared`
- `Fitmaniac.Api` → `Fitmaniac.Application`, `Fitmaniac.Infrastructure`, `Fitmaniac.Domain`, `Fitmaniac.Shared`
- `Fitmaniac.Web` → `Fitmaniac.Application`, `Fitmaniac.Domain`, `Fitmaniac.Shared`
- `Fitmaniac.MAUI` → `Fitmaniac.Domain`, `Fitmaniac.Shared`

Solution file `Fitmaniac.sln` groups the five .NET libraries + Api + Web under a `src/` solution folder and `Fitmaniac.MAUI` under a separate `mobile/` solution folder.

### Solution-wide files

- `Fitmaniac.sln`
- `global.json` — pin SDK 10.0.100 with `rollForward: latestFeature`
- `Directory.Build.props` — `TargetFramework=net10.0`, `Nullable=enable`, `ImplicitUsings=enable`, `ProduceReferenceAssembly=true`, `AccelerateBuildsInVisualStudio=true` (matches RYF verbatim)
- `.editorconfig` — standard C# 10/file-scoped-namespaces/4-space indent
- `.gitignore` — standard VS/dotnet ignore
- `README.md` — build/run instructions (API port, Web port, MAUI platforms, seeded admin creds)

---

## 2b. NuGet Package Manifest

All versions are the .NET 10–compatible set that RYF ships (10.0.5 for EF/Identity/OpenApi/Auth; 8.3.1 for `System.IdentityModel.Tokens.Jwt`). FullStackTest's 9.x packages are upgraded. Any FST package not present in RYF is re-evaluated for retention.

### `Fitmaniac.Domain`
- No `PackageReference`
- `FrameworkReference`: `Microsoft.AspNetCore.App` (needed for `IdentityUser` base class, matching RYF)

### `Fitmaniac.Application`
| Package | Version | Purpose |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | `10.0.5` | `DbSet<T>` on `IApplicationDbContext` |
| `HtmlSanitizer` | `9.0.892` | Sanitize user-provided HTML (notes, bios, descriptions) — retained from RYF |
| FrameworkReference `Microsoft.AspNetCore.App` | — | `Microsoft.Extensions.Logging`, `IFormFile` |

### `Fitmaniac.Infrastructure`
| Package | Version |
|---|---|
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | `10.0.5` |
| `Microsoft.EntityFrameworkCore.SqlServer` | `10.0.5` |
| `Microsoft.EntityFrameworkCore.Tools` | `10.0.5` (PrivateAssets=all) |
| `System.IdentityModel.Tokens.Jwt` | `8.3.1` |
| `QuestPDF` | `2026.2.4` — used by `ProgramPdfService` to export `WeeklyProgram` + its workouts/exercises/sets as a printable PDF for clients. **Not** used for certificates (certificates are out of scope in v1). |
| FrameworkReference `Microsoft.AspNetCore.App` | — |

### `Fitmaniac.Shared`
- No `PackageReference`. Contains embedded `.resx` and JSON seed resources only.

### `Fitmaniac.Api`
| Package | Version |
|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `10.0.5` |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | `10.0.5` |
| `Microsoft.EntityFrameworkCore.Design` | `10.0.5` (PrivateAssets=all) |
| `Microsoft.EntityFrameworkCore.SqlServer` | `10.0.5` |
| `Microsoft.AspNetCore.SignalR` | `10.0.5` — hosts the chat hub (trainer ↔ client messaging, carried over from RYF) |
| `Microsoft.AspNetCore.OpenApi` | `10.0.5` |
| `System.IdentityModel.Tokens.Jwt` | `8.3.1` |
| `Swashbuckle.AspNetCore` | `7.2.0` — **[NEEDS CLARIFICATION]** retained for Swagger UI; RYF uses `AddOpenApi` alone. Keep both; remove Swashbuckle later if unused. |

### `Fitmaniac.Web`
| Package | Version |
|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `10.0.5` |
| `Microsoft.AspNetCore.Authentication.Cookies` (implicit in framework) | — |
| `Microsoft.AspNetCore.SignalR.Client` | `10.0.5` — kept for future real-time features (chat/workout-live-timer) per RYF |
| `Microsoft.AspNetCore.OpenApi` | `10.0.5` |
| `System.IdentityModel.Tokens.Jwt` | `8.3.1` |

### `Fitmaniac.MAUI`
| Package | Version |
|---|---|
| `Microsoft.Maui.Controls` | `10.0.0` |
| `Microsoft.Maui.Controls.Compatibility` | `10.0.0` |
| `Microsoft.Extensions.Logging.Debug` | `10.0.0` |
| `Microsoft.Extensions.Http` | `10.0.0` |
| `CommunityToolkit.Maui` | `11.0.0` — alerts, behaviors, converters |
| `CommunityToolkit.Mvvm` | `8.4.0` — `ObservableObject`, `RelayCommand`, `ObservableProperty` |
| `System.IdentityModel.Tokens.Jwt` | `8.3.1` — for client-side token expiry parsing |
| `Microsoft.Maui.Controls.Maps` | `10.0.0` — optional; **[NEEDS CLARIFICATION]** keep only if outdoor activity tracking is in scope |

---

## 2c. Folder and File Structure

Every file listed below must be created. No wildcards. Boilerplate `obj/`, `bin/`, `.vs/` are omitted.

### `src/Fitmaniac.Domain/`
```
Fitmaniac.Domain.csproj
Common/
  AuditableEntity.cs
  IAuditable.cs
Entities/
  ApplicationUser.cs
  Trainer.cs
  Client.cs
  TrainerClient.cs
  RefreshToken.cs
  PersonalInformation.cs
  Goal.cs
  Measurement.cs
  MedicalHistory.cs
  Equipment.cs
  ExerciseDefinition.cs
  ExerciseEquipment.cs
  WeeklyProgram.cs
  Workout.cs
  ClientWorkout.cs
  WorkoutExercise.cs
  WorkoutExerciseSet.cs
  SubscriptionPlan.cs
  UserSubscription.cs
  BillingTransaction.cs
  SiteSetting.cs
  ChatConversation.cs
  ChatMessage.cs
Enums/
  UserRole.cs
  UserStatus.cs
  ClientExperience.cs
  GoalType.cs
  GoalStatus.cs
  GoalUnit.cs
  IntensityLevel.cs
  TrainerSpecialization.cs
  MedicalCondition.cs
  MedicationType.cs
  SurgeryType.cs
  MuscleGroup.cs
  SubscriptionTier.cs
  BillingPeriod.cs
Exceptions/
  DomainException.cs
  EntityNotFoundException.cs
  BusinessRuleViolationException.cs
Extensions/
  UserSearchExtensions.cs
  WorkoutSearchExtensions.cs
```

### `src/Fitmaniac.Application/`
```
Fitmaniac.Application.csproj
GlobalUsings.cs
Data/
  IApplicationDbContext.cs
Common/
  ServiceResult.cs
  PagedResult.cs
  PaginationRequest.cs
  SortRequest.cs
Abstractions/
  ICurrentUserService.cs
  ITokenService.cs
  IFileStorage.cs
  IEmailService.cs
  IDateTimeProvider.cs
Services/
  Auth/
    IAuthService.cs
    IRefreshTokenCookieService.cs
  Users/
    IUserService.cs
    IAdminUserService.cs
  Trainers/
    ITrainerService.cs
    IAdminTrainerService.cs
  Clients/
    IClientService.cs
  Workouts/
    IWorkoutService.cs
    IAdminWorkoutService.cs
  Exercises/
    IExerciseDefinitionService.cs
    IEquipmentService.cs
  Programs/
    IWeeklyProgramService.cs
  Goals/
    IGoalService.cs
  Measurements/
    IMeasurementService.cs
  MedicalHistory/
    IMedicalHistoryService.cs
  Subscriptions/
    ISubscriptionService.cs
    IBillingService.cs
  Progress/
    IProgressService.cs
  Chat/
    IChatService.cs
    IChatQueryService.cs
  Pdf/
    IProgramPdfService.cs
  Validation/
    IValidationService.cs
Mapping/
  IModelToDtoMapper.cs
```

### `src/Fitmaniac.Infrastructure/`
```
Fitmaniac.Infrastructure.csproj
GlobalUsings.cs
Persistence/
  FitmaniacDbContext.cs
  DesignTimeDbContextFactory.cs
  FitmaniacDbContextExtensions.cs
  Configurations/
    ApplicationUserConfiguration.cs
    TrainerConfiguration.cs
    ClientConfiguration.cs
    TrainerClientConfiguration.cs
    RefreshTokenConfiguration.cs
    GoalConfiguration.cs
    MeasurementConfiguration.cs
    MedicalHistoryConfiguration.cs
    EquipmentConfiguration.cs
    ExerciseDefinitionConfiguration.cs
    ExerciseEquipmentConfiguration.cs
    WeeklyProgramConfiguration.cs
    WorkoutConfiguration.cs
    ClientWorkoutConfiguration.cs
    WorkoutExerciseConfiguration.cs
    WorkoutExerciseSetConfiguration.cs
    SubscriptionPlanConfiguration.cs
    UserSubscriptionConfiguration.cs
    BillingTransactionConfiguration.cs
    SiteSettingConfiguration.cs
    ChatConversationConfiguration.cs
    ChatMessageConfiguration.cs
  Interceptors/
    AuditSaveChangesInterceptor.cs
    SoftDeleteInterceptor.cs
  Migrations/
    (generated by `dotnet ef migrations add InitialCreate`)
Identity/
  IdentityRoleSeeder.cs
  AdminUserSeeder.cs
Services/
  TokenService.cs
  CurrentUserService.cs
  LocalFileStorage.cs
  StubEmailService.cs
  DateTimeProvider.cs
  ValidationService.cs
  ModelToDtoMapper.cs
  RefreshTokenCookieService.cs
  SubscriptionService.cs
  BillingService.cs
  UserService.cs
  AdminUserService.cs
  TrainerService.cs
  AdminTrainerService.cs
  ClientService.cs
  WorkoutService.cs
  AdminWorkoutService.cs
  ExerciseDefinitionService.cs
  EquipmentService.cs
  WeeklyProgramService.cs
  GoalService.cs
  MeasurementService.cs
  MedicalHistoryService.cs
  ProgressService.cs
  AuthService.cs
  ChatService.cs
  ChatQueryService.cs
  ProgramPdfService.cs
Seeders/
  SubscriptionPlanSeeder.cs
  EquipmentSeeder.cs
  ExerciseDefinitionSeeder.cs
  SampleTrainerSeeder.cs
  SampleClientSeeder.cs
  DatabaseSeederOrchestrator.cs
  BulkClientSeedingService.cs
DependencyInjection/
  InfrastructureServiceCollectionExtensions.cs
```

### `src/Fitmaniac.Shared/`
```
Fitmaniac.Shared.csproj
GlobalUsings.cs
DTOs/
  Auth/
    RegisterRequestDto.cs
    LoginRequestDto.cs
    RefreshResponseDto.cs
    AuthResponseDto.cs
    ForgotPasswordRequestDto.cs
    ResetPasswordRequestDto.cs
    ConfirmEmailRequestDto.cs
    ChangePasswordRequestDto.cs
    AccessTokenDto.cs
  Users/
    UserDto.cs
    CreateUserDto.cs
    UpdateUserDto.cs
    UserStatisticsDto.cs
  Trainers/
    TrainerDto.cs
    TrainerListItemDto.cs
    CreateTrainerDto.cs
    UpdateTrainerProfileDto.cs
  Clients/
    ClientDto.cs
    CreateClientDto.cs
    UpdateClientProfileDto.cs
  Workouts/
    WorkoutDto.cs
    WorkoutListItemDto.cs
    CreateWorkoutDto.cs
    UpdateWorkoutDto.cs
    WorkoutExerciseDto.cs
    CreateWorkoutExerciseDto.cs
    UpdateWorkoutExerciseDto.cs
    WorkoutExerciseSetDto.cs
    CreateWorkoutExerciseSetDto.cs
    UpdateWorkoutExerciseSetDto.cs
  Exercises/
    ExerciseDefinitionDto.cs
    CreateExerciseDefinitionDto.cs
    UpdateExerciseDefinitionDto.cs
    EquipmentDto.cs
    CreateEquipmentDto.cs
    UpdateEquipmentDto.cs
  Programs/
    WeeklyProgramDto.cs
    CreateWeeklyProgramDto.cs
    UpdateWeeklyProgramDto.cs
  Goals/
    GoalDto.cs
    CreateGoalDto.cs
    UpdateGoalDto.cs
  Measurements/
    MeasurementDto.cs
    CreateMeasurementDto.cs
    UpdateMeasurementDto.cs
  MedicalHistory/
    MedicalHistoryDto.cs
    CreateMedicalHistoryDto.cs
    UpdateMedicalHistoryDto.cs
  Subscriptions/
    SubscriptionPlanDto.cs
    UserSubscriptionDto.cs
    BillingTransactionDto.cs
  Progress/
    ProgressSummaryDto.cs
    WeeklyProgressDto.cs
  Chat/
    ChatConversationDto.cs
    ChatMessageDto.cs
    SendMessageRequestDto.cs
    StartConversationRequestDto.cs
  Common/
    PagedResultDto.cs
    ErrorResponseDto.cs
    ProblemDetailsDto.cs
Constants/
  AuthConstants.cs
  ClaimTypesExtended.cs
  PolicyNames.cs
  RoleNames.cs
  RouteConstants.cs
  SeedDefaults.cs
Resources/
  GlobalRes.resx
  GlobalRes.el.resx
  GlobalRes.Designer.cs
  NavMenuRes.resx
  NavMenuRes.el.resx
  NavMenuRes.Designer.cs
  HomeRes.resx
  HomeRes.el.resx
  HomeRes.Designer.cs
  AuthRes.resx
  AuthRes.el.resx
  AuthRes.Designer.cs
  WorkoutRes.resx
  WorkoutRes.el.resx
  WorkoutRes.Designer.cs
  ExerciseRes.resx
  ExerciseRes.el.resx
  ExerciseRes.Designer.cs
  ProgramRes.resx
  ProgramRes.el.resx
  ProgramRes.Designer.cs
  GoalRes.resx
  GoalRes.el.resx
  GoalRes.Designer.cs
  ProfileRes.resx
  ProfileRes.el.resx
  ProfileRes.Designer.cs
  AdminRes.resx
  AdminRes.el.resx
  AdminRes.Designer.cs
  TrainerRes.resx
  TrainerRes.el.resx
  TrainerRes.Designer.cs
  ClientRes.resx
  ClientRes.el.resx
  ClientRes.Designer.cs
  BillingRes.resx
  BillingRes.el.resx
  BillingRes.Designer.cs
  Messages/
    ErrorMessagesRes.resx
    ErrorMessagesRes.el.resx
    ErrorMessagesRes.Designer.cs
    SuccessMessagesRes.resx
    SuccessMessagesRes.el.resx
    SuccessMessagesRes.Designer.cs
JSON/
  Exercises/
    exercises.core.json
    exercises.strength.json
    exercises.cardio.json
  Equipment/
    equipment.json
  Programs/
    sample-programs.json
  Trainers/
    sample-trainers.json
  Clients/
    sample-clients.json
```

### `src/Fitmaniac.Api/`
```
Fitmaniac.Api.csproj
Program.cs
GlobalUsings.cs
appsettings.json
appsettings.Development.json
appsettings.Production.json
Properties/
  launchSettings.json
DependencyInjection/
  ApiServiceCollectionExtensions.cs
  SwaggerServiceCollectionExtensions.cs
  AuthenticationServiceCollectionExtensions.cs
Authentication/
  JwtBearerEventsHandler.cs
Endpoints/
  IEndpointRouteBuilderExtensions.cs
  AuthEndpoints.cs
  AdminUserEndpoints.cs
  UserProfileEndpoints.cs
  TrainerEndpoints.cs
  AdminTrainerEndpoints.cs
  ClientEndpoints.cs
  WorkoutEndpoints.cs
  AdminWorkoutEndpoints.cs
  ExerciseDefinitionEndpoints.cs
  EquipmentEndpoints.cs
  WeeklyProgramEndpoints.cs
  GoalEndpoints.cs
  MeasurementEndpoints.cs
  MedicalHistoryEndpoints.cs
  SubscriptionEndpoints.cs
  BillingEndpoints.cs
  ProgressEndpoints.cs
  MediaEndpoints.cs
  ChatEndpoints.cs
Hubs/
  ChatHub.cs
Middleware/
  ExceptionHandlingMiddleware.cs
  DisabledUserMiddleware.cs
Filters/
  ValidateModelFilter.cs
Logs/
  .gitkeep
App_Data/
  Uploads/
    avatars/.gitkeep
    exercises/video/.gitkeep
    workouts/.gitkeep
```

### `src/Fitmaniac.Web/`
```
Fitmaniac.Web.csproj
Program.cs
GlobalUsings.cs
appsettings.json
appsettings.Development.json
Properties/
  launchSettings.json
Components/
  App.razor
  Routes.razor
  _Imports.razor
  Layout/
    MainLayout.razor
    MainLayout.razor.css
    NavMenu.razor
    NavMenu.razor.css
    AvatarDropdown.razor
    CultureSelector.razor
  Pages/
    Home.razor
    Pricing.razor
    Login.razor
    Register.razor
    ForgotPassword.razor
    ResetPassword.razor
    Disabled.razor
    SubscriptionSuccess.razor
    VerifyCertificate.razor
    Profile.razor
    Profile.razor.css
    Billing.razor
    Trainers.razor
    TrainerDetail.razor
    ClientDashboard.razor
    TrainerDashboard.razor
    Workouts.razor
    WorkoutDetail.razor
    WorkoutRunner.razor
    Exercises.razor
    ExerciseDetail.razor
    WeeklyProgramView.razor
    Goals.razor
    Measurements.razor
    MedicalHistory.razor
    ProgressDashboard.razor
    Chat.razor
    Admin/
      AdminUsers.razor
      AdminTrainers.razor
      AdminClients.razor
      AdminExercises.razor
      AdminExerciseEdit.razor
      AdminEquipment.razor
      AdminWorkouts.razor
      AdminWorkoutEdit.razor
      AdminPrograms.razor
      AdminProgramEdit.razor
      AdminAnalytics.razor
      AdminSubscriptionPlans.razor
  Shared/
    LoadingSpinner.razor
    DismissibleAlert.razor
    ConfirmModal.razor
    FormModal.razor
    SaveCancelBar.razor
    FormCardSection.razor
    PaginationNav.razor
    SortableColumnHeader.razor
    StatusBadge.razor
    VirtualizedTable.razor
    SeoHead.razor
    UpgradePrompt.razor
    QuillEditor.razor
    WorkoutCard.razor
    WorkoutCard.razor.css
    ExerciseCard.razor
    ExerciseCard.razor.css
    SetInputRow.razor
    GoalCard.razor
    MeasurementChart.razor
    BmiIndicator.razor
    Chat/
      ConversationSidebar.razor
      MessagePane.razor
      UserPickerModal.razor
Consumers/
  ApiClientBase.cs
  SsrApiHandler.cs
  IAuthConsumer.cs
  AuthConsumer.cs
  IUserConsumer.cs
  UserConsumer.cs
  IAdminUserConsumer.cs
  AdminUserConsumer.cs
  ITrainerConsumer.cs
  TrainerConsumer.cs
  IAdminTrainerConsumer.cs
  AdminTrainerConsumer.cs
  IClientConsumer.cs
  ClientConsumer.cs
  IWorkoutConsumer.cs
  WorkoutConsumer.cs
  IAdminWorkoutConsumer.cs
  AdminWorkoutConsumer.cs
  IExerciseConsumer.cs
  ExerciseConsumer.cs
  IEquipmentConsumer.cs
  EquipmentConsumer.cs
  IWeeklyProgramConsumer.cs
  WeeklyProgramConsumer.cs
  IGoalConsumer.cs
  GoalConsumer.cs
  IMeasurementConsumer.cs
  MeasurementConsumer.cs
  IMedicalHistoryConsumer.cs
  MedicalHistoryConsumer.cs
  ISubscriptionConsumer.cs
  SubscriptionConsumer.cs
  IAdminAnalyticsConsumer.cs
  AdminAnalyticsConsumer.cs
  IProgressConsumer.cs
  ProgressConsumer.cs
  IChatConsumer.cs
  ChatConsumer.cs
Services/
  AuthService.cs
  IAuthService.cs
  ChatSignalRService.cs
  IChatSignalRService.cs
Interfaces/
  ITokenProvider.cs
Logging/
  FileLoggerExtensions.cs
  FileLoggerProvider.cs
  FileLogger.cs
Logs/
  .gitkeep
wwwroot/
  favicon.png
  css/
    app.css
    shared-components.css
  js/
    quill-interop.js
    chart-interop.js
    workout-timer-interop.js
    chat-interop.js
  images/
    logo.svg
    hero-placeholder.jpg
  lib/
    bootstrap/
      dist/
        css/bootstrap.min.css
        js/bootstrap.bundle.min.js
```

### `mobile/Fitmaniac.MAUI/`
```
Fitmaniac.MAUI.csproj
MauiProgram.cs
App.xaml
App.xaml.cs
AppShell.xaml
AppShell.xaml.cs
GlobalUsings.cs
MauiApp.xaml
appsettings.json
Platforms/
  Android/
    AndroidManifest.xml
    MainActivity.cs
    MainApplication.cs
    Resources/values/colors.xml
  iOS/
    AppDelegate.cs
    Info.plist
    Program.cs
  MacCatalyst/
    AppDelegate.cs
    Info.plist
    Program.cs
  Windows/
    App.xaml
    App.xaml.cs
    Package.appxmanifest
    app.manifest
Resources/
  AppIcon/appicon.svg
  AppIcon/appiconfg.svg
  Splash/splash.svg
  Fonts/OpenSans-Regular.ttf
  Fonts/OpenSans-Semibold.ttf
  Images/dotnet_bot.png
  Raw/AboutAssets.txt
  Styles/Colors.xaml
  Styles/Styles.xaml
Pages/
  LoginPage.xaml
  LoginPage.xaml.cs
  RegisterPage.xaml
  RegisterPage.xaml.cs
  HomePage.xaml
  HomePage.xaml.cs
  WorkoutsPage.xaml
  WorkoutsPage.xaml.cs
  WorkoutDetailPage.xaml
  WorkoutDetailPage.xaml.cs
  WorkoutRunnerPage.xaml
  WorkoutRunnerPage.xaml.cs
  ExercisesPage.xaml
  ExercisesPage.xaml.cs
  ExerciseDetailPage.xaml
  ExerciseDetailPage.xaml.cs
  ProgramsPage.xaml
  ProgramsPage.xaml.cs
  GoalsPage.xaml
  GoalsPage.xaml.cs
  MeasurementsPage.xaml
  MeasurementsPage.xaml.cs
  ProgressPage.xaml
  ProgressPage.xaml.cs
  ProfilePage.xaml
  ProfilePage.xaml.cs
  SettingsPage.xaml
  SettingsPage.xaml.cs
ViewModels/
  BaseViewModel.cs
  LoginViewModel.cs
  RegisterViewModel.cs
  HomeViewModel.cs
  WorkoutsViewModel.cs
  WorkoutDetailViewModel.cs
  WorkoutRunnerViewModel.cs
  ExercisesViewModel.cs
  ExerciseDetailViewModel.cs
  ProgramsViewModel.cs
  GoalsViewModel.cs
  MeasurementsViewModel.cs
  ProgressViewModel.cs
  ProfileViewModel.cs
  SettingsViewModel.cs
Services/
  ApiClient.cs
  IApiClient.cs
  AuthTokenStore.cs
  IAuthTokenStore.cs
  AuthHttpHandler.cs
  AuthService.cs
  IAuthService.cs
  WorkoutService.cs
  IWorkoutService.cs
  ExerciseService.cs
  IExerciseService.cs
  ProgramService.cs
  IProgramService.cs
  GoalService.cs
  IGoalService.cs
  MeasurementService.cs
  IMeasurementService.cs
  ProgressService.cs
  IProgressService.cs
  ConnectivityService.cs
  OfflineCacheService.cs
Converters/
  InverseBoolConverter.cs
  NullToBoolConverter.cs
  StringNotNullOrEmptyConverter.cs
Controls/
  SetInputView.xaml
  SetInputView.xaml.cs
  WorkoutTimerView.xaml
  WorkoutTimerView.xaml.cs
```

---

## 2d. Domain Model Definitions

All under namespace `Fitmaniac.Domain.*`. Entity bases mirror RYF (`AuditableEntity`) where multilingual content / soft-delete applies, plain POCO where the FST entity was simple.

> **Convention:** `int Id` for FST-origin entities (users, clients, trainers, exercises, workouts…) to match FST's schema; `Guid Id` is reserved for immutable content entities added later. This honors FST's simple integer-key style and matches what MAUI's offline cache handles best.

### `Common/IAuditable.cs`
```csharp
namespace Fitmaniac.Domain.Common;

public interface IAuditable
{
    DateTime CreatedUtc { get; set; }
    DateTime UpdatedUtc { get; set; }
    string? CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
}
```

### `Common/AuditableEntity.cs`
```csharp
namespace Fitmaniac.Domain.Common;

public abstract class AuditableEntity : IAuditable
{
    public int Id { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedUtc { get; set; }
}
```

### `Entities/ApplicationUser.cs`
```csharp
using Microsoft.AspNetCore.Identity;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public UserRole Role { get; set; } = UserRole.Client;
    public UserStatus Status { get; set; } = UserStatus.Unknown;
    public bool IsEnabled { get; set; } = true;
    public bool GdprConsentGiven { get; set; }
    public DateTime? GdprConsentDate { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginUtc { get; set; }

    public Trainer? TrainerProfile { get; set; }
    public Client? ClientProfile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
```

### `Entities/PersonalInformation.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public abstract class PersonalInformation : AuditableEntity
{
    [Url] public string? ProfilePhotoUrl { get; set; }
    [MaxLength(30)] public string? FirstName { get; set; }
    [MaxLength(30)] public string? LastName { get; set; }
    [DataType(DataType.Date)] public DateTime? DateOfBirth { get; set; }
    [Phone, MaxLength(20)] public string? PhoneNumber { get; set; }
    [MaxLength(100)] public string? Address { get; set; }
    [MaxLength(50)] public string? City { get; set; }
    [MaxLength(50)] public string? State { get; set; }
    [MaxLength(10)] public string? ZipCode { get; set; }
    [MaxLength(50)] public string? Country { get; set; }
    [Range(0, 500)] public double? Weight { get; set; }
    [Range(0, 300)] public double? Height { get; set; }
    [Range(0, 10000)] public double? BMR { get; set; }
    [Range(0, 100)] public double? BMI { get; set; }

    public int? Age => DateOfBirth is null ? null : CalculateAge(DateOfBirth.Value);

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.UtcNow;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
}
```

### `Entities/Trainer.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class Trainer : PersonalInformation
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    [StringLength(1000)] public string? Bio { get; set; }
    public ICollection<TrainerSpecialization> Specializations { get; set; } = new List<TrainerSpecialization>();

    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
```

### `Entities/Client.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class Client : PersonalInformation
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    [StringLength(500)] public string? Bio { get; set; }
    public ClientExperience ExperienceLevel { get; set; } = ClientExperience.Beginner;
    public IntensityLevel PreferredIntensityLevel { get; set; } = IntensityLevel.Medium;

    public MedicalHistory? MedicalHistory { get; set; }
    public WeeklyProgram? CurrentWeeklyProgram { get; set; }

    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}
```

### `Entities/TrainerClient.cs`
```csharp
namespace Fitmaniac.Domain.Entities;

public class TrainerClient
{
    public int TrainerId { get; set; }
    public Trainer Trainer { get; set; } = null!;
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public DateTime SubscribedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UnsubscribedUtc { get; set; }
}
```

### `Entities/RefreshToken.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class RefreshToken : AuditableEntity
{
    [Required, MaxLength(128)] public string TokenHash { get; set; } = null!;
    [MaxLength(128)] public string? ReplacedByTokenHash { get; set; }
    [Required] public DateTime ExpiresUtc { get; set; }
    public DateTime? RevokedUtc { get; set; }
    [MaxLength(45)] public string? CreatedByIp { get; set; }
    [MaxLength(45)] public string? RevokedByIp { get; set; }

    [NotMapped] public bool IsExpired => DateTime.UtcNow >= ExpiresUtc;
    [NotMapped] public bool IsActive => RevokedUtc is null && !IsExpired;

    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}
```

### `Entities/Goal.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class Goal : AuditableEntity
{
    [Required] public GoalType GoalType { get; set; }
    [StringLength(255)] public string? Description { get; set; }
    [Required] public DateTime TargetDate { get; set; }
    [Required] public GoalStatus Status { get; set; } = GoalStatus.Active;
    [Range(0, 1000)] public int? GoalQuantity { get; set; }
    public GoalUnit? GoalUnit { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}
```

### `Entities/Measurement.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class Measurement : AuditableEntity
{
    [Required] public GoalUnit Unit { get; set; }
    [Required, Range(0, 1000)] public double Value { get; set; }
    [Required] public DateTime Date { get; set; } = DateTime.UtcNow;
    [Required] public IntensityLevel Intensity { get; set; }
    public bool IsPersonalBest { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}
```

### `Entities/MedicalHistory.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class MedicalHistory : AuditableEntity
{
    [StringLength(1000)] public string? Description { get; set; }
    public ICollection<MedicalCondition> Conditions { get; set; } = new List<MedicalCondition>();
    public ICollection<MedicationType> MedicationTypes { get; set; } = new List<MedicationType>();
    public ICollection<SurgeryType> Surgeries { get; set; } = new List<SurgeryType>();
    public IntensityLevel? RecommendedIntensityLevel { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}
```

### `Entities/Equipment.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class Equipment : AuditableEntity
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }

    public ICollection<ExerciseDefinition> Exercises { get; set; } = new List<ExerciseDefinition>();
}
```

### `Entities/ExerciseDefinition.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class ExerciseDefinition : AuditableEntity
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    [StringLength(1000)] public string? Description { get; set; }
    [Url] public string? VideoUrl { get; set; }
    [Range(0, 2000)] public int CaloriesBurnedPerHour { get; set; }
    public bool IsCompoundExercise { get; set; }

    [Required, MinLength(1)]
    public ICollection<MuscleGroup> PrimaryMuscleGroups { get; set; } = new List<MuscleGroup>();
    public ICollection<MuscleGroup> SecondaryMuscleGroups { get; set; } = new List<MuscleGroup>();

    public ClientExperience? ExperienceLevel { get; set; }
    [StringLength(50)] public string? Category { get; set; }

    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}
```

### `Entities/ExerciseEquipment.cs`
```csharp
namespace Fitmaniac.Domain.Entities;

public class ExerciseEquipment
{
    public int ExerciseDefinitionId { get; set; }
    public ExerciseDefinition ExerciseDefinition { get; set; } = null!;
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;
}
```

### `Entities/WeeklyProgram.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class WeeklyProgram : AuditableEntity
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }
    [Required, Range(1, 52)] public int DurationInWeeks { get; set; }
    [Required, Range(1, 52)] public int CurrentWeek { get; set; } = 1;

    [NotMapped] public bool IsCompleted => CurrentWeek > DurationInWeeks;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
```

### `Entities/Workout.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class Workout : AuditableEntity
{
    [Required] public DateTime ScheduledDateTime { get; set; }
    [Required, StringLength(50)] public string Type { get; set; } = null!;
    [Required, Range(1, 300)] public int DurationInMinutes { get; set; }
    [StringLength(500)] public string? Notes { get; set; }

    public int? TrainerId { get; set; }
    public Trainer? Trainer { get; set; }

    public int? WeeklyProgramId { get; set; }
    public WeeklyProgram? WeeklyProgram { get; set; }

    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}
```

### `Entities/ClientWorkout.cs`
```csharp
namespace Fitmaniac.Domain.Entities;

public class ClientWorkout
{
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;
    public DateTime? CompletedUtc { get; set; }
    public int? PerceivedIntensity { get; set; }
    public string? ClientNotes { get; set; }
}
```

### `Entities/WorkoutExercise.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class WorkoutExercise : AuditableEntity
{
    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;

    public int ExerciseDefinitionId { get; set; }
    public ExerciseDefinition ExerciseDefinition { get; set; } = null!;

    [MaxLength(500)] public string? Notes { get; set; }

    public ICollection<WorkoutExerciseSet> Sets { get; set; } = new List<WorkoutExerciseSet>();
}
```

### `Entities/WorkoutExerciseSet.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class WorkoutExerciseSet : AuditableEntity
{
    public int WorkoutExerciseId { get; set; }
    public WorkoutExercise WorkoutExercise { get; set; } = null!;

    [Required, Range(1, 20)] public int SetNumber { get; set; }
    [Required, Range(1, 200)] public int Repetitions { get; set; }
    [Range(0, 1000)] public double? Weight { get; set; }
    public GoalUnit? GoalUnit { get; set; }
    [Required] public IntensityLevel OverallIntensityLevel { get; set; }
    [Range(0, int.MaxValue)] public int DurationInSeconds { get; set; }
    [Range(0, int.MaxValue)] public int RestPeriodInSeconds { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
}
```

### `Entities/SubscriptionPlan.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class SubscriptionPlan : AuditableEntity
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    [StringLength(1000)] public string? Description { get; set; }
    public decimal Price { get; set; }
    public BillingPeriod BillingPeriod { get; set; }
    public SubscriptionTier Tier { get; set; } = SubscriptionTier.Free;
    public string? FeaturesJson { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
```

### `Entities/UserSubscription.cs`
```csharp
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class UserSubscription : AuditableEntity
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public int SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}
```

### `Entities/BillingTransaction.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class BillingTransaction : AuditableEntity
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public int? SubscriptionPlanId { get; set; }
    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public decimal Amount { get; set; }
    [MaxLength(100)] public string? TransactionReference { get; set; }
    [MaxLength(30)] public string Status { get; set; } = "Pending";
}
```

### `Entities/SiteSetting.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace Fitmaniac.Domain.Entities;

public class SiteSetting
{
    [Key, MaxLength(100)] public string Key { get; set; } = null!;
    public string? Value { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }
}
```

### `Entities/ChatConversation.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class ChatConversation : AuditableEntity
{
    public int CreatorId { get; set; }
    public ApplicationUser Creator { get; set; } = null!;

    public int ParticipantId { get; set; }
    public ApplicationUser Participant { get; set; } = null!;

    [MaxLength(500)] public string? LastMessageContent { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
```

### `Entities/ChatMessage.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class ChatMessage : AuditableEntity
{
    public int ConversationId { get; set; }
    public ChatConversation Conversation { get; set; } = null!;

    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    [Required, MaxLength(2000)] public string Content { get; set; } = null!;
    [MaxLength(50)] public string? SenderRole { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}
```

### Enums (`Enums/*.cs`)
All enum values preserved verbatim from FullStackTest (see catalog §2) and extended where Fitmaniac needs it. Representative examples:

```csharp
namespace Fitmaniac.Domain.Enums;
public enum UserRole { Admin = 1, Trainer = 2, Client = 3 }
public enum UserStatus { Unknown = 0, Active = 1, Pending = 2, Disabled = 3 }
public enum ClientExperience { Beginner, Occasional, Regular, Athlete }
public enum GoalType { WeightLoss, MuscleGain, Strength, Power, Endurance, Flexibility, GeneralFitness, BodyComposition, SkillDevelopment, Maintenance }
public enum GoalStatus { Active, Completed, Abandoned }
public enum GoalUnit { Kilograms, Pounds, Meters, Kilometers, Miles, Repetitions, Minutes, Hours, Sessions, Calories }
public enum IntensityLevel { Low, Medium, High, Extreme }
public enum TrainerSpecialization { GeneralFitness, StrengthTraining, Cardio, Yoga, Pilates, CrossFit, Bodybuilding, WeightLoss, Rehabilitation, SportsPerformance, SeniorFitness, GroupFitness, Nutrition }
public enum MedicalCondition { None, Asthma, Diabetes, Hypertension, HeartDisease, Arthritis, Obesity, HighCholesterol, BackPain, Pregnancy, Epilepsy, RespiratoryIssues, JointProblems, Other }
public enum MedicationType { None, BloodPressure, Cholesterol, Diabetes, Asthma, PainRelief, AntiInflammatory, Antidepressant, BirthControl, Thyroid, Allergy, Heart, Other }
public enum SurgeryType { None, Cardiac, Orthopedic, Neurological, Gastrointestinal, Cosmetic, Gynecological, Urological, Respiratory, Ophthalmological, ENT, Other }
public enum MuscleGroup { None, Chest, Back, Shoulders, Arms, Legs, Core, Glutes, Calves, Forearms, Neck, FullBody, Other }
public enum SubscriptionTier { Free = 0, Plus = 1, Pro = 2 }
public enum BillingPeriod { Monthly = 1, Quarterly = 2, Yearly = 3 }
```

### Exceptions
```csharp
namespace Fitmaniac.Domain.Exceptions;

public abstract class DomainException : Exception { protected DomainException(string msg) : base(msg) {} }
public sealed class EntityNotFoundException : DomainException { public EntityNotFoundException(string entity, object key) : base($"{entity} with key '{key}' not found.") {} }
public sealed class BusinessRuleViolationException : DomainException { public BusinessRuleViolationException(string msg) : base(msg) {} }
```

---

## 2e. DbContext and Migrations Plan

### `FitmaniacDbContext`

- **Base:** `IdentityDbContext<ApplicationUser, IdentityRole<int>, int>` — integer user keys (matches FST schema; enables simpler MAUI offline storage).
- **Interface:** implements `IApplicationDbContext` (defines every `DbSet<T>`).
- **Provider:** `UseSqlServer(connectionString)` with `EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)` — matches RYF. Default dev connection targets LocalDB (`(localdb)\MSSQLLocalDB;Database=FitmaniacDb;Trusted_Connection=True;TrustServerCertificate=True;`).

### DbSets
```
DbSet<Trainer>                 Trainers
DbSet<Client>                  Clients
DbSet<TrainerClient>           TrainerClients
DbSet<RefreshToken>            RefreshTokens
DbSet<Goal>                    Goals
DbSet<Measurement>             Measurements
DbSet<MedicalHistory>          MedicalHistories
DbSet<Equipment>               Equipments
DbSet<ExerciseDefinition>      ExerciseDefinitions
DbSet<ExerciseEquipment>       ExerciseEquipments
DbSet<WeeklyProgram>           WeeklyPrograms
DbSet<Workout>                 Workouts
DbSet<ClientWorkout>           ClientWorkouts
DbSet<WorkoutExercise>         WorkoutExercises
DbSet<WorkoutExerciseSet>      WorkoutExerciseSets
DbSet<SubscriptionPlan>        SubscriptionPlans
DbSet<UserSubscription>        UserSubscriptions
DbSet<BillingTransaction>      BillingTransactions
DbSet<SiteSetting>             SiteSettings
DbSet<ChatConversation>        ChatConversations
DbSet<ChatMessage>             ChatMessages
```

### Fluent API rules (per configuration class)
- **`ApplicationUserConfiguration`** — indexes on `Email`, `UserName`, `Role`, `CreatedUtc`. Soft delete via `IsEnabled` filter (no global filter — Identity enforces via `IsEnabled` check in login).
- **`TrainerConfiguration`** — one-to-one `User` via `UserId` unique index, cascade delete. Many-to-many with `Client` via `TrainerClient`. One-to-many `Workouts` FK `TrainerId` SetNull on delete. Value converter: `List<TrainerSpecialization>` → JSON string.
- **`ClientConfiguration`** — one-to-one `User`, one-to-one `MedicalHistory`, one-to-one `CurrentWeeklyProgram`. Many-to-many with `Trainer` and `Workout`. Cascade delete on owned collections (Goals, Measurements).
- **`TrainerClientConfiguration`** — composite key `(TrainerId, ClientId)`. Index on `SubscribedUtc`.
- **`RefreshTokenConfiguration`** — unique index on `TokenHash`, index on `(UserId, ExpiresUtc)`, cascade on user delete.
- **`GoalConfiguration`** — FK to `Client` cascade. Index on `(ClientId, Status, TargetDate)`.
- **`MeasurementConfiguration`** — FK to `Client` cascade. Index on `(ClientId, Date DESC)`.
- **`MedicalHistoryConfiguration`** — one-to-one with `Client`, cascade. JSON string converters for `Conditions`, `MedicationTypes`, `Surgeries` lists.
- **`EquipmentConfiguration`** — name unique index.
- **`ExerciseDefinitionConfiguration`** — indexes on `Name`, `Category`. JSON converter for muscle-group collections.
- **`ExerciseEquipmentConfiguration`** — composite key `(ExerciseDefinitionId, EquipmentId)`.
- **`WeeklyProgramConfiguration`** — cascade from `Client`. One-to-many `Workouts`.
- **`WorkoutConfiguration`** — many-to-many with `Client` via `ClientWorkout`. Indexes on `ScheduledDateTime`, `TrainerId`, `WeeklyProgramId`.
- **`ClientWorkoutConfiguration`** — composite key `(ClientId, WorkoutId)`.
- **`WorkoutExerciseConfiguration`** — cascade delete from `Workout`. Restrict delete from `ExerciseDefinition` (protect catalog).
- **`WorkoutExerciseSetConfiguration`** — cascade delete from `WorkoutExercise`. Unique index on `(WorkoutExerciseId, SetNumber)`.
- **`SubscriptionPlanConfiguration`** — unique index on `Name`. `Price` decimal(18,2).
- **`UserSubscriptionConfiguration`** — index on `(UserId, IsActive)`.
- **`BillingTransactionConfiguration`** — `Amount` decimal(18,2). Index on `(UserId, CreatedUtc DESC)`.
- **`SiteSettingConfiguration`** — `Key` is string PK, max 100.
- **`ChatConversationConfiguration`** — FKs to `ApplicationUser` for `Creator` and `Participant` with `OnDelete(Restrict)` (prevent cascade cycles). Unique composite index on `(CreatorId, ParticipantId)` to dedupe conversations. Index on `LastMessageAt DESC` for list queries.
- **`ChatMessageConfiguration`** — FK to `ChatConversation` cascade. FK to `ApplicationUser` restrict. Index on `(ConversationId, SentAt)`. `Content` max length 2000.

### Interceptors
- `AuditSaveChangesInterceptor` — on SaveChanges, stamps `CreatedUtc/UpdatedUtc/CreatedBy/UpdatedBy` from `ICurrentUserService` for all `IAuditable` entries.
- `SoftDeleteInterceptor` — converts `Remove()` on `AuditableEntity` into `IsDeleted = true, DeletedUtc = now`. Global query filter excludes `IsDeleted = true`.

### Seeding strategy
`DatabaseSeederOrchestrator.SeedAsync()` runs at startup (Api only). Order:
1. `IdentityRoleSeeder` — ensures `Admin`, `Trainer`, `Client` roles exist.
2. `SubscriptionPlanSeeder` — Free / Plus / Pro (idempotent by Name).
3. `EquipmentSeeder` — loads from embedded `Fitmaniac.Shared/JSON/Equipment/equipment.json`.
4. `ExerciseDefinitionSeeder` — loads from `JSON/Exercises/*.json`, links equipment by name.
5. `AdminUserSeeder` — uses `AdminUser:Email` + `AdminUser:Password` from config; creates with `Admin` role + `UserStatus.Active`.
6. `SampleTrainerSeeder` / `SampleClientSeeder` — only when `SeedSampleData:Enabled = true` (dev); each reads from `Fitmaniac.Shared/JSON/Trainers/sample-trainers.json` and `JSON/Clients/sample-clients.json`.

### Initial migration
```
cd src/Fitmaniac.Api
dotnet ef migrations add InitialCreate -p ../Fitmaniac.Infrastructure -s . -o Persistence/Migrations
dotnet ef database update -p ../Fitmaniac.Infrastructure -s .
```

Migrations output directory is `Fitmaniac.Infrastructure/Persistence/Migrations/`; the startup project is `Fitmaniac.Api`.

---

## 2f. API Endpoint Specification

All endpoints are under base route `/api` (applied in `MapGroup("/api")`). Auth convention: `[Authorize]` by default; `[AllowAnonymous]` only where noted. Role names: `Admin`, `Trainer`, `Client`. Version stays implicit (`v1`) until we introduce versioning.

**Response envelope:** successful 2xx returns the typed body directly; errors return RFC7807 `ProblemDetails`. `401` and `403` are auto-generated by auth middleware.

### `AuthEndpoints` — `/api/auth`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| POST | `/register` | Anonymous | `RegisterRequestDto` | `201 Created` | Validate email/password, create user + profile in transaction, assign role, send email confirmation token. |
| POST | `/login` | Anonymous + RateLimit("login") | `LoginRequestDto` | `AccessTokenDto` + sets `refreshToken` HttpOnly cookie | Validate credentials, issue JWT (30 min) + rotate refresh token. |
| POST | `/refresh` | Anonymous (reads cookie) | — | `AccessTokenDto` + rotates cookie | Silent refresh using HttpOnly cookie. |
| POST | `/logout` | `[Authorize]` | — | `200` | Revoke server-side refresh token, clear cookie. |
| POST | `/forgot-password` | Anonymous | `ForgotPasswordRequestDto` | `200` | Generate reset token, send email. |
| POST | `/reset-password` | Anonymous | `ResetPasswordRequestDto` | `200` | Validate token, set new password. |
| POST | `/confirm-email` | Anonymous | `ConfirmEmailRequestDto` | `200` | Confirm user's email via token. |
| POST | `/change-password` | `[Authorize]` | `ChangePasswordRequestDto` | `200` | Change password for logged-in user. |

### `UserProfileEndpoints` — `/api/users`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/me` | `[Authorize]` | — | `UserDto` | Current user with profile. |
| POST | `/{id:int}/upload-photo` | `[Authorize]` (self or Admin) | multipart `photo` | `{ message, photoUrl }` | Validate image (JPEG/PNG ≤2MB, magic-bytes check), save to `App_Data/Uploads/avatars/{userId}_{guid}.ext`. |
| PUT | `/{id:int}/profile` | `[Authorize]` (self or Admin) | `UpdateClientProfileDto` or `UpdateTrainerProfileDto` | `UserDto` | Role-aware update. |

### `AdminUserEndpoints` — `/api/admin/users`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/` | `Admin` | query: page, pageSize, sortBy, sortOrder, search | `PagedResultDto<UserDto>` | Paginated/sorted users. |
| GET | `/statistics` | `Admin` | — | `UserStatisticsDto` | Counts by role and status. |
| GET | `/{id:int}` | `Admin` | — | `UserDto` | Full user detail. |
| POST | `/` | `Admin` | `CreateUserDto` | `201` | Admin-created user. |
| PUT | `/{id:int}` | `Admin` | `UpdateUserDto` | `200` | Cannot change last Admin's role. |
| DELETE | `/{id:int}` | `Admin` | — | `200` | Cannot delete Admins. |
| PUT | `/{id:int}/enable` | `Admin` | — | `200` | Set `IsEnabled=true`. |
| PUT | `/{id:int}/disable` | `Admin` | — | `200` | Set `IsEnabled=false`; invalidate refresh tokens. |

### `TrainerEndpoints` — `/api/trainers`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/` | `[Authorize]` | query: page, pageSize, sortBy, sortOrder, specialization | `PagedResultDto<TrainerListItemDto>` | Paginated trainer directory. |
| GET | `/{id:int}` | `[Authorize]` | — | `TrainerDto` | Public trainer profile. |

### `AdminTrainerEndpoints` — `/api/admin/trainers`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| POST | `/` | `Admin` | `CreateTrainerDto` | `201` | Create trainer profile for an existing user (promote Client → Trainer). |
| PUT | `/{id:int}` | `Admin` | `UpdateTrainerProfileDto` | `200` | Update trainer profile including specializations. |
| DELETE | `/{id:int}` | `Admin` | — | `200` | Soft-delete trainer; reassign clients to `null`. |

### `ClientEndpoints` — `/api/clients`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| POST | `/{userId:int}/subscribe/{trainerId:int}` | `Client` (self) | — | `200` | Add trainer to client's trainers list. |
| POST | `/{userId:int}/unsubscribe/{trainerId:int}` | `Client` (self) | — | `200` | Remove trainer from client's trainers list. |
| GET | `/{userId:int}/subscriptions` | `Client` (self) | — | `int[]` trainer ids | List subscribed trainers. |
| GET | `/{id:int}` | `[Authorize]` (self, their trainer, or Admin) | — | `ClientDto` | Full client profile incl. metrics summary. |

### `WorkoutEndpoints` — `/api/workouts`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/my` | `Client` or `Trainer` | query: from, to, programId | `WorkoutListItemDto[]` | Current user's workouts (Client: their own; Trainer: ones they authored). |
| GET | `/{id:int}` | `[Authorize]` (participant or author) | — | `WorkoutDto` | Workout + exercises + sets. |
| POST | `/{id:int}/complete` | `Client` | `{ perceivedIntensity, notes }` | `200` | Mark `ClientWorkout.CompletedUtc`. |

### `AdminWorkoutEndpoints` — `/api/admin/workouts` (and trainer-scoped writes via `[Authorize(Roles="Admin,Trainer")]`)
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/` | `Admin,Trainer` | paged | `PagedResultDto<WorkoutListItemDto>` | All workouts (trainer: filtered to their authored). |
| POST | `/` | `Admin,Trainer` | `CreateWorkoutDto` | `201` | Create workout + assign clients + add exercises/sets. |
| PUT | `/{id:int}` | `Admin,Trainer` | `UpdateWorkoutDto` | `200` | Update scheduling/participants/exercises. |
| DELETE | `/{id:int}` | `Admin,Trainer` | — | `200` | Soft-delete. |
| POST | `/{id:int}/exercises` | `Admin,Trainer` | `CreateWorkoutExerciseDto` | `201` | Append exercise. |
| PUT | `/exercises/{weId:int}` | `Admin,Trainer` | `UpdateWorkoutExerciseDto` | `200` | Update notes. |
| DELETE | `/exercises/{weId:int}` | `Admin,Trainer` | — | `200` | Remove. |
| POST | `/exercises/{weId:int}/sets` | `Admin,Trainer` | `CreateWorkoutExerciseSetDto` | `201` | Add set. |
| PUT | `/sets/{setId:int}` | `Admin,Trainer` or `Client` (for actuals only) | `UpdateWorkoutExerciseSetDto` | `200` | Update a set. |
| DELETE | `/sets/{setId:int}` | `Admin,Trainer` | — | `200` | Delete set. |

### `ExerciseDefinitionEndpoints` — `/api/exercises`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/` | `[Authorize]` | query: category, muscleGroup, experience, equipmentId | `PagedResultDto<ExerciseDefinitionDto>` | Exercise catalog. |
| GET | `/{id:int}` | `[Authorize]` | — | `ExerciseDefinitionDto` | Exercise detail. |
| POST | `/` | `Admin,Trainer` | `CreateExerciseDefinitionDto` | `201` | Create. |
| PUT | `/{id:int}` | `Admin,Trainer` | `UpdateExerciseDefinitionDto` | `200` | Update. |
| DELETE | `/{id:int}` | `Admin` | — | `200` | Soft-delete (protected by `Restrict` FK). |

### `EquipmentEndpoints` — `/api/equipment`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/` | `[Authorize]` | — | `EquipmentDto[]` | All equipment. |
| POST | `/` | `Admin` | `CreateEquipmentDto` | `201` | Create. |
| PUT | `/{id:int}` | `Admin` | `UpdateEquipmentDto` | `200` | Update + exercise link sync. |
| DELETE | `/{id:int}` | `Admin` | — | `200` | Remove. |

### `WeeklyProgramEndpoints` — `/api/programs`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/my` | `Client` | — | `WeeklyProgramDto?` | Current active program. |
| GET | `/{id:int}` | `[Authorize]` (owner, their trainer, Admin) | — | `WeeklyProgramDto` | Program + workouts. |
| POST | `/` | `Admin,Trainer` | `CreateWeeklyProgramDto` | `201` | Create program for a client. |
| PUT | `/{id:int}` | `Admin,Trainer` | `UpdateWeeklyProgramDto` | `200` | Update. |
| POST | `/{id:int}/advance-week` | `Admin,Trainer,Client` (owner) | — | `200` | `CurrentWeek++`. |
| DELETE | `/{id:int}` | `Admin,Trainer` | — | `200` | Remove. |

### `GoalEndpoints` — `/api/goals`
| Verb | Route | Auth | Request | Response |
|---|---|---|---|---|
| GET | `/my` | `Client` | — | `GoalDto[]` |
| POST | `/` | `Client` | `CreateGoalDto` | `201` |
| PUT | `/{id:int}` | `Client` (owner) | `UpdateGoalDto` | `200` |
| DELETE | `/{id:int}` | `Client` (owner) | — | `200` |

### `MeasurementEndpoints` — `/api/measurements`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/my` | `Client` | query: unit, from, to | `MeasurementDto[]` | Chronological measurements. |
| POST | `/` | `Client` | `CreateMeasurementDto` | `201` | Auto-compute `IsPersonalBest`. |
| PUT | `/{id:int}` | `Client` (owner) | `UpdateMeasurementDto` | `200` | Update. |
| DELETE | `/{id:int}` | `Client` (owner) | — | `200` | Remove. |

### `MedicalHistoryEndpoints` — `/api/medical-history`
| Verb | Route | Auth | Request | Response |
|---|---|---|---|---|
| GET | `/my` | `Client` | — | `MedicalHistoryDto?` |
| PUT | `/` | `Client` | `UpdateMedicalHistoryDto` | `200` (upsert) |

### `SubscriptionEndpoints` — `/api/subscription`
| Verb | Route | Auth | Request | Response |
|---|---|---|---|---|
| GET | `/plans` | Anonymous | — | `SubscriptionPlanDto[]` |
| GET | `/current` | `[Authorize]` | — | `UserSubscriptionDto?` |
| POST | `/upgrade` | `[Authorize]` | `{ planId }` | `200` (initiates payment — stub until billing provider wired) |

### `BillingEndpoints` — `/api/billing`
| Verb | Route | Auth | Request | Response |
|---|---|---|---|---|
| GET | `/transactions` | `[Authorize]` | query: page, pageSize | `PagedResultDto<BillingTransactionDto>` |

### `ProgressEndpoints` — `/api/progress`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/summary` | `Client` | — | `ProgressSummaryDto` | Totals: sessions completed, kcal burned, personal bests. |
| GET | `/weekly` | `Client` | query: weeks (default 12) | `WeeklyProgressDto[]` | Rolling weekly aggregates. |

### `MediaEndpoints` — `/api/media`
| Verb | Route | Auth | Request | Response |
|---|---|---|---|---|
| GET | `/avatars/{filename}` | Anonymous | — | `FileResult` |
| GET | `/exercises/{filename}` | `[Authorize]` | — | `FileResult` |

### `ChatEndpoints` — `/api/chat`
| Verb | Route | Auth | Request | Response | Logic |
|---|---|---|---|---|---|
| GET | `/conversations` | `[Authorize]` | — | `ChatConversationDto[]` | All conversations where current user is creator or participant, ordered by `LastMessageAt DESC`. |
| POST | `/conversations` | `[Authorize]` | `StartConversationRequestDto { targetUserId }` | `201` + `ChatConversationDto` | Creates or returns existing conversation between current user and `targetUserId`. Trainer↔Client only (Admin can chat with anyone). |
| GET | `/conversations/{conversationId:int}/messages` | `[Authorize]` (participant) | query: page, pageSize | `PagedResultDto<ChatMessageDto>` | Paginated message history (newest first). |
| POST | `/conversations/{conversationId:int}/messages` | `[Authorize]` (participant) | `SendMessageRequestDto { content }` | `201` + `ChatMessageDto` | REST fallback for sending when SignalR unavailable (offline MAUI send). Broadcasts over hub if connected. |
| POST | `/conversations/{conversationId:int}/read` | `[Authorize]` (participant) | — | `200` | Marks all unread messages in the conversation as read for current user. |

### `ChatHub` — `/hubs/chat` (SignalR)
| Method | Direction | Auth | Payload | Description |
|---|---|---|---|---|
| `SendMessage(int conversationId, string content)` | Client → Server | `[Authorize]` (participant) | — | Persists `ChatMessage`, updates `LastMessage*` on conversation, broadcasts `ReceiveMessage` to both participants' groups. |
| `InitiateConversation(int targetUserId)` | Client → Server | `[Authorize]` | — | Creates conversation and returns id; joins both users' groups. |
| `ReceiveMessage(ChatMessageDto message)` | Server → Client | — | — | Pushes new message to the user group. |
| `MessageRead(int messageId, int readerUserId)` | Server → Client | — | — | Notifies sender that their message was read. |
Hub lives in `Fitmaniac.Api/Hubs/ChatHub.cs`. Authentication uses JWT bearer; for WebSocket connections the hub reads token from `access_token` query string (same pattern as RYF). MAUI and Web both connect using `HubConnectionBuilder`.

---

## 2g. Blazor Component Reuse Map

| ResetYourFuture Component | Reuse Decision | Notes / Changes for Fitmaniac |
|---|---|---|
| `App.razor` | **Adapt** | Keep DOCTYPE + critical/deferred CSS pattern; replace inline Bootstrap Icons + Quill link set with Fitmaniac's fitness-themed font loads; keep `InteractiveServerRenderMode(prerender: true)`. |
| `Routes.razor` | **Copy as-is** | Router + AuthorizeRouteView + FocusOnNavigate pattern is domain-agnostic. |
| `_Imports.razor` | **Adapt** | Swap `Shared.Resources`, `Web.Services`, `Web.Consumers` namespaces from RYF → Fitmaniac equivalents. |
| `Layout/MainLayout.razor` (+`.razor.css`) | **Copy as-is** | Two-column layout is reusable. Replace logo & palette only. |
| `Layout/NavMenu.razor` (+`.razor.css`) | **Adapt** | Menu entries change: Workouts / Programs / Goals / Measurements / Trainers / Admin. Keep collapsible structure, role-gated sections, culture selector. |
| `Layout/AvatarDropdown.razor` | **Copy as-is** | Profile + Logout + admin tools links. |
| `Layout/CultureSelector.razor` | **Copy as-is** | `en-GB` / `el-GR` selector. |
| `Layout/ImpersonationBanner.razor` | **Adapt** | Keep. Useful for Admin impersonating Trainer or Client. |
| `Shared/Components/Data/VirtualizedTable.razor` | **Copy as-is** | Drives admin user / workout / exercise lists. |
| `Shared/Components/Data/AdminPaginationToolbar.razor` | **Copy as-is** | Pagination/sort toolbar. |
| `Shared/Components/Data/SortableColumnHeader.razor` | **Copy as-is** | Sort-toggle header. |
| `Shared/Components/Data/StatusBadge.razor` | **Copy as-is** | Use for `GoalStatus`, `UserStatus`, `SubscriptionTier`. |
| `Shared/Components/Data/PaginationNav.razor` | **Copy as-is** | — |
| `Shared/Components/Data/ConfirmModal.razor` | **Copy as-is** | — |
| `Shared/Components/Data/FormModal.razor` | **Copy as-is** | — |
| `Shared/Components/Forms/FormCardSection.razor` | **Copy as-is** | Form section wrapper. |
| `Shared/Components/Forms/SaveCancelBar.razor` | **Copy as-is** | Footer buttons. |
| `Shared/Components/Forms/ModuleEditorModal.razor` | **Excluded** | Career-specific (course modules). Replace with `WorkoutExerciseEditorModal.razor`. |
| `Shared/Components/Forms/LessonEditorModal.razor` | **Excluded** | Career-specific. Replace with `SetEditorModal.razor`. |
| `Shared/Components/Layout/DismissibleAlert.razor` | **Copy as-is** | Toast/banner. |
| `Shared/Components/Layout/PageStateContainer.razor` | **Copy as-is** | — |
| `Shared/Components/Layout/SeoHead.razor` | **Copy as-is** | Inject title/meta via `HeadOutlet`. |
| `Shared/Components/Layout/UpgradePrompt.razor` | **Copy as-is** | Subscription upgrade hint. |
| `Shared/Components/LoadingSpinner.razor` | **Copy as-is** | — |
| `Shared/Components/QuillEditor.razor` | **Copy as-is** | Reused for workout notes, trainer bio, blog-style content. |
| `Shared/Components/Chat/ConversationSidebar.razor` | **Copy as-is** | Reused for trainer↔client messaging. Conversation list pulled from `/api/chat/conversations`. |
| `Shared/Components/Chat/MessagePane.razor` | **Copy as-is** | Reused. Message send path: SignalR `SendMessage` hub call with REST fallback to `POST /api/chat/conversations/{id}/messages`. |
| `Shared/Components/Chat/UserPickerModal.razor` | **Adapt** | Picker filters to valid chat partners — a Client can pick any of their subscribed Trainers; a Trainer can pick any of their Clients; Admin can pick anyone. |
| Pages: `Home.razor` | **Adapt** | Replace hero copy/imagery — same page shell. |
| Pages: `Pricing.razor` | **Adapt** | Same plan card layout; populate fitness plan features. |
| Pages: `Login.razor`, `Register.razor`, `ForgotPassword.razor`, `Disabled.razor`, `SubscriptionSuccess.razor` | **Copy as-is** (text strings swap) | Auth/account flows are domain-agnostic. |
| Pages: `BlogArticle.razor`, `VerifyCertificate.razor` | **Excluded** | Career-domain. Drop. |
| Pages: `Courses.razor`, `CourseDetail.razor`, `LessonViewer.razor` | **Replaced** | Becomes `Workouts.razor`, `WorkoutDetail.razor`, `WorkoutRunner.razor`. |
| Pages: `Assessments.razor`, `AssessmentForm.razor`, `AssessmentHistory.razor` | **Replaced** | Becomes `Goals.razor`, `Measurements.razor`, `ProgressDashboard.razor`. |
| Pages: `Chat.razor` | **Copy as-is** | Full-screen chat page with conversation sidebar + message pane; consumes `IChatConsumer` + `IChatSignalRService`. |
| Pages: `Profile.razor` | **Adapt** | Same shell; profile fields expand to Personal Information + medical (client-only). |
| Pages: `Billing.razor`, `MyCertificates.razor` | **Copy as-is** / **Excluded** | Billing copied; MyCertificates dropped. |
| Admin pages: `AdminCourses.razor`, `AdminCourseEdit.razor`, `AdminLessonEdit.razor` | **Replaced** | Becomes `AdminWorkouts.razor`, `AdminWorkoutEdit.razor`, `AdminExercises.razor`, `AdminExerciseEdit.razor`. |
| Admin pages: `AdminAssessments.razor`, `AdminAssessmentEdit.razor`, `AdminAssessmentSubmissions.razor` | **Replaced** | Becomes `AdminPrograms.razor`, `AdminProgramEdit.razor`. |
| `AdminUsers.razor` | **Copy as-is** (column set changes) | User management. |
| `AdminAnalytics.razor` | **Adapt** | Metrics shift from enrollments/completions to active users / sessions completed / weight lifted. |
| `AdminBlog.razor`, `AdminBlogEditor.razor` | **Excluded** | No blog in Fitmaniac v1. |
| `AdminTestimonials.razor`, `AdminTestimonialEditor.razor` | **Excluded (v1)** | **[NEEDS CLARIFICATION]** whether to retain for marketing site. Excluding for now. |
| `Hubs/ChatHub.cs` + SignalR setup | **Adapt & move** | ChatHub moves from RYF's Web project into `Fitmaniac.Api/Hubs/ChatHub.cs` so MAUI and Web share a single hub endpoint. Both clients connect via `HubConnectionBuilder().WithUrl("{ApiBaseUrl}/hubs/chat", o => o.AccessTokenProvider = () => Task.FromResult(token))`. |
| `Services/SsrApiHandler.cs` | **Copy as-is** | Cookie→JWT SSR handler, essential for Web → Api loopback. |
| `Services/AuthService.cs` | **Adapt** | Keep DataProtection ticket pattern for `/auth/complete` SSR sign-in bridge; point to Fitmaniac.Api. |
| Controllers (all `*Controller.cs`) | **Excluded from Web** | Controllers move to `Fitmaniac.Api` as minimal-API endpoint groups. |

---

## 2h. MAUI Project Plan

### Target platforms
- `net10.0-android` (min SDK 24)
- `net10.0-ios` (min 17.0)
- `net10.0-maccatalyst` (min 15.0)
- `net10.0-windows10.0.19041.0`

### Navigation: Shell flyout + tabs

```
AppShell
├── LoginPage                 (initial when unauthenticated)
├── RegisterPage
└── Flyout (post-login)
    ├── Tabs
    │   ├── HomePage          (dashboard: today's workout, weekly progress)
    │   ├── WorkoutsPage      (list of upcoming + completed)
    │   ├── ProgramsPage      (current weekly program)
    │   └── ProgressPage      (charts: measurements, PRs, sessions)
    ├── ExercisesPage         (catalog browse)
    ├── GoalsPage
    ├── MeasurementsPage
    ├── ProfilePage
    └── SettingsPage          (theme, units, logout)
```

- Runner flow: `WorkoutsPage` → `WorkoutDetailPage` → `WorkoutRunnerPage` (full-screen, keep-awake, set-by-set progression with timer).

### Pages ↔ purpose

| Page | Purpose |
|---|---|
| `LoginPage` | Email + password login. Calls `/api/auth/login`. On success, stores refresh-token cookie (from API) + access token in secure store. |
| `RegisterPage` | Register as Client (default). Calls `/api/auth/register`. |
| `HomePage` | "Today" view: next workout, weekly progress ring, recent measurements. |
| `WorkoutsPage` | List from `/api/workouts/my` grouped by date. Pull-to-refresh. |
| `WorkoutDetailPage` | Workout metadata + exercises + planned sets. Start-Workout button. |
| `WorkoutRunnerPage` | Live set-by-set UI; logs actuals; auto-rest timer; offline-capable. Posts `PUT /sets/{id}` on each set save. |
| `ExercisesPage` | Catalog filtered by muscle group / experience / equipment. |
| `ExerciseDetailPage` | Video, muscles, technique notes. |
| `ProgramsPage` | Current `WeeklyProgram` with week markers. |
| `GoalsPage` | CRUD goals via `/api/goals/*`. |
| `MeasurementsPage` | Enter a measurement; list history; PR indicator. |
| `ProgressPage` | Charts from `/api/progress/summary` + `/api/progress/weekly`. |
| `ProfilePage` | Edit personal info; upload avatar. |
| `SettingsPage` | Theme (light/dark/auto), units (kg/lb, km/mi), API base URL override (dev), Logout. |

### HttpClient / auth token handling

- **`ApiClient`** (singleton) — wraps a single `HttpClient` bound to `appsettings.json → ApiBaseUrl`.
- **`AuthHttpHandler`** (DelegatingHandler) — on each request:
  1. Reads access token from `IAuthTokenStore`.
  2. If expired (exp claim < now+30s), calls `/api/auth/refresh` before retrying once. Uses a `SemaphoreSlim(1,1)` to coalesce concurrent refreshes.
  3. Attaches `Authorization: Bearer {access}`.
- **`IAuthTokenStore`** — backed by `SecureStorage.Default` (`Microsoft.Maui.Storage`):
  - `access_token` (plaintext JWT) — invalidated on sign-out.
  - `refresh_token` (plaintext; refresh flow is not cookie-based on MAUI, the API returns `refresh_token` in JSON body when the request is from a MAUI `X-Client: mobile` header — see §2i for token flow).

### Shared code with `Fitmaniac.Shared`

- MAUI consumes DTOs from `Fitmaniac.Shared.DTOs.*` directly (no copies).
- MAUI consumes enums from `Fitmaniac.Domain.Enums` (reference Domain).
- MAUI uses `Fitmaniac.Shared.Constants.RouteConstants` to build API paths (single source of truth).
- MAUI does **not** reference `Fitmaniac.Application` or `Fitmaniac.Infrastructure`.

### Offline strategy (v1, minimal)

- SQLite local cache via `sqlite-net-pcl` (add later, not v1 default). For v1: in-memory cache + "last workout" persisted to preferences for offline workout-runner resumption.

### Platform-specific wiring

- Android: add `INTERNET`, `ACCESS_NETWORK_STATE`, `WAKE_LOCK` permissions in `AndroidManifest.xml`.
- iOS: `NSAppTransportSecurity` `NSAllowsLocalNetworking` for dev API over HTTP.
- Windows: no extra config.

---

## 2i. Authentication and Authorization Plan

### Identity core
- **ASP.NET Core Identity** with `ApplicationUser : IdentityUser<int>` and `IdentityRole<int>`. Key type `int` (FST-compatible).
- Roles seeded: `Admin`, `Trainer`, `Client`.
- Password policy: minimum length 8, digit required, uppercase required. Email must be unique and confirmed (dev auto-confirms; prod requires email flow).

### Tokens
- **Access token (JWT, HS256):** lifetime 30 minutes. Claims:
  - `sub` = username
  - `nameid` = user id (int, as string)
  - `email`
  - `role` (repeat for each role)
  - `subscriptionTier` (Free/Plus/Pro — resolved via `ISubscriptionService` on issue)
- **Refresh token:** 7-day opaque 64-byte `RandomNumberGenerator` token. Stored server-side as **SHA256 hash** in `RefreshTokens`. Rotation on every refresh (old one revoked, new one replaces `ReplacedByTokenHash`).

### Transport of refresh token
- **Web (browser):** refresh token lives in an `HttpOnly, Secure, SameSite=Lax` cookie named `refreshToken` set by Api at `/api/auth/login`. Path=`/`, domain matches API. Cookie scopes to API origin; Web calls Api via `SsrApiHandler` which forwards cookies.
- **MAUI (mobile):** request header `X-Client: mobile` on `/api/auth/login` and `/api/auth/refresh` — Api responds with refresh token in JSON body instead of Set-Cookie. MAUI stores it in `SecureStorage`. This is a deliberate split — mobile clients don't have cookie jars wired like browsers.

### Flow — Web (Blazor Server, ResetYourFuture SSR pattern)
1. `Login.razor` posts credentials to `Fitmaniac.Web` `IAuthService`.
2. `AuthService` calls `POST /api/auth/login` via `SelfClient` → gets JWT + Set-Cookie refresh.
3. `AuthService` generates a DataProtection-protected ticket `(userId|empty|0)` and redirects to `GET /auth/complete?t={ticket}`.
4. `/auth/complete` endpoint on Web (`Program.cs`) unprotects ticket, rebuilds `ClaimsPrincipal` with roles and claim `fitmaniac.access_token={JWT}`, calls `HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ...)` — sets `.FMC.Auth` cookie.
5. Subsequent Blazor circuit has `AuthenticationStateProvider` populated. Consumers use `SsrApiHandler` which reads the access token claim and adds `Authorization: Bearer` when calling `Fitmaniac.Api`.
6. When JWT is about to expire, `SsrApiHandler` calls Api `/refresh` (forwarding cookie), receives a fresh JWT, updates the cookie claim via the data-protection round-trip.

### Flow — MAUI
1. `LoginPage` → `AuthService.LoginAsync(email, pwd)` → `POST /api/auth/login` with header `X-Client: mobile`.
2. Api responds with `{ accessToken, accessTokenExpiresUtc, refreshToken, refreshTokenExpiresUtc }`.
3. `IAuthTokenStore` persists both to `SecureStorage`.
4. `AuthHttpHandler` auto-refreshes near expiry (single-flight) using `POST /api/auth/refresh` body `{ refreshToken }`.
5. Logout: `POST /api/auth/logout` → server revokes token, MAUI clears `SecureStorage`.

### Multi-auth scheme on Api
- `DefaultAuthenticateScheme = JwtBearer`.
- Cookie is only used inside **Web**, not Api. Api accepts Bearer only.
- Events on JwtBearer:
  - `OnTokenValidated`: verify `IsEnabled` on user (cached 60s). If disabled → set `Context.Items["UserDisabled"] = true`.
  - `OnChallenge`: on disabled user, add response header `X-User-Disabled: 1`.

### Authorization policies (Api)
```
policies.AddPolicy("Admin",   p => p.RequireRole("Admin"));
policies.AddPolicy("Trainer", p => p.RequireRole("Trainer"));
policies.AddPolicy("Client",  p => p.RequireRole("Client"));
policies.AddPolicy("StaffOrOwner", p => /* custom handler: Admin or self-match on route param */);
```

### Rate limiting
- Fixed-window `"login"` limiter on `/api/auth/login`: 5 permits / minute.
- Fixed-window `"refresh"` limiter on `/api/auth/refresh`: 20 permits / minute per IP.

### MAUI SecureStorage usage
```csharp
await SecureStorage.Default.SetAsync(AuthTokenStore.AccessKey, accessToken);
await SecureStorage.Default.SetAsync(AuthTokenStore.RefreshKey, refreshToken);
```

### Blazor AuthenticationStateProvider
- Use `ServerAuthenticationStateProvider` (default). Add `AddCascadingAuthenticationState()` — same as RYF.
- `AuthorizeRouteView` handles redirect-to-login (pointing to `/login`).

---

## 2j. CSS and UI Asset Reuse

### Carry-over from ResetYourFuture (copy into `Fitmaniac.Web/wwwroot/`)
| RYF asset | Copy path | Rebranding |
|---|---|---|
| `lib/bootstrap/dist/css/bootstrap.min.css` | same | none |
| `lib/bootstrap/dist/js/bootstrap.bundle.min.js` | same | none |
| `css/app.css` | `css/app.css` | **Replace palette variables** — swap purple/magenta (`--text-strong-accent: #C47CB4`) for energetic fitness scheme (orange/teal). **[NEEDS CLARIFICATION]** exact colors pending brand decision. Default palette: `--bg-primary: #0b1220`, `--bg-surface: #111a2b`, `--accent-primary: #ff6b2d` (orange), `--accent-secondary: #14b8a6` (teal), success `#22c55e`, warning `#f59e0b`, error `#ef4444`. |
| `css/shared-components.css` | same | none structural; only color-var references follow new palette |
| `js/quill-interop.js` | same | none |
| `js/chart-interop.js` | same | none |
| `images/favicon.png` | replace with Fitmaniac icon | new asset |
| `App.razor` head links (Bootstrap Icons, Font Awesome, Quill CSS, critical CSS inlining) | same pattern | keep |
| Layout CSS (`MainLayout.razor.css`, `NavMenu.razor.css`) | same | replace logo URL |

### New assets to author
- `images/logo.svg` — Fitmaniac logo (SVG).
- `js/workout-timer-interop.js` — rest-timer ticker, audio cue, vibration API (Web fallback of MAUI timer).
- `css/fitness.css` — workout-card, exercise-card, set-input-row, progress-ring styles (scoped new selectors).

### Excluded from RYF
- Blog article styles (`blog.css` if present) — blog is out of v1.
- Certificate PDF styles — certificates are out of v1.

---

## 2k. Dependency Injection Registration

### `Fitmaniac.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs`
```csharp
public static IServiceCollection AddFitmaniacInfrastructure(this IServiceCollection services, IConfiguration config)
{
    services.AddDbContext<FitmaniacDbContext>(opts =>
    {
        var cs = config.GetConnectionString("DefaultConnection")!;
        opts.UseSqlServer(cs, sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
    });
    services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<FitmaniacDbContext>());

    services.AddScoped<AuditSaveChangesInterceptor>();
    services.AddScoped<SoftDeleteInterceptor>();

    // Identity
    services.AddIdentity<ApplicationUser, IdentityRole<int>>(o =>
    {
        o.Password.RequireDigit = true;
        o.Password.RequireUppercase = true;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequiredLength = 8;
        o.User.RequireUniqueEmail = true;
        o.SignIn.RequireConfirmedEmail = false; // [NEEDS CLARIFICATION] enable in prod
    })
    .AddEntityFrameworkStores<FitmaniacDbContext>()
    .AddDefaultTokenProviders();

    // Cross-cutting
    services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    services.AddScoped<ICurrentUserService, CurrentUserService>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IFileStorage, LocalFileStorage>();
    services.AddScoped<IEmailService, StubEmailService>();
    services.AddScoped<IValidationService, ValidationService>();
    services.AddScoped<IRefreshTokenCookieService, RefreshTokenCookieService>();
    services.AddSingleton<IModelToDtoMapper, ModelToDtoMapper>();

    // HtmlSanitizer singleton
    services.AddSingleton<Ganss.Xss.IHtmlSanitizer>(_ => new Ganss.Xss.HtmlSanitizer());

    // Application services
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IAdminUserService, AdminUserService>();
    services.AddScoped<ITrainerService, TrainerService>();
    services.AddScoped<IAdminTrainerService, AdminTrainerService>();
    services.AddScoped<IClientService, ClientService>();
    services.AddScoped<IWorkoutService, WorkoutService>();
    services.AddScoped<IAdminWorkoutService, AdminWorkoutService>();
    services.AddScoped<IExerciseDefinitionService, ExerciseDefinitionService>();
    services.AddScoped<IEquipmentService, EquipmentService>();
    services.AddScoped<IWeeklyProgramService, WeeklyProgramService>();
    services.AddScoped<IGoalService, GoalService>();
    services.AddScoped<IMeasurementService, MeasurementService>();
    services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
    services.AddScoped<ISubscriptionService, SubscriptionService>();
    services.AddScoped<IBillingService, BillingService>();
    services.AddScoped<IProgressService, ProgressService>();
    services.AddScoped<IChatService, ChatService>();
    services.AddScoped<IChatQueryService, ChatQueryService>();
    services.AddScoped<IProgramPdfService, ProgramPdfService>();

    // Seeders
    services.AddScoped<IdentityRoleSeeder>();
    services.AddScoped<AdminUserSeeder>();
    services.AddScoped<SubscriptionPlanSeeder>();
    services.AddScoped<EquipmentSeeder>();
    services.AddScoped<ExerciseDefinitionSeeder>();
    services.AddScoped<SampleTrainerSeeder>();
    services.AddScoped<SampleClientSeeder>();
    services.AddScoped<DatabaseSeederOrchestrator>();
    services.AddHostedService<BulkClientSeedingService>();

    return services;
}
```

### `Fitmaniac.Api/Program.cs` additional registrations
- `AddControllers()`, `AddEndpointsApiExplorer()`, `AddOpenApi()`.
- `AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` with `OnMessageReceived` pulling `access_token` from query string for `/hubs/chat` upgrades.
- `AddAuthorization(opts => { /* Admin, Trainer, Client policies */ })`.
- `AddCors("AllowFrontend", b => b.WithOrigins(cors:AllowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials())`.
- `AddRateLimiter(...)` with "login" and "refresh" fixed windows.
- `AddSignalR(o => o.MaximumReceiveMessageSize = 32_000)` — hosts `ChatHub` at `/hubs/chat`.
- `AddHttpContextAccessor()`.
- `services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()))`.
- `QuestPDF.Settings.License = LicenseType.Community;` (static init at top of `Program.cs`) — required for `ProgramPdfService`.

### `Fitmaniac.Web/Program.cs` additional registrations
- `AddRazorComponents().AddInteractiveServerComponents()`.
- `AddCascadingAuthenticationState()`.
- `AddAuthentication("MultiAuth")` with cookie (`.FMC.Auth`) + JWT bearer (for SSR loopback) + `AddPolicyScheme("MultiAuth", ...)` forwarding selector.
- `AddAuthorization(...)` — `AdminOnly`, `TrainerOnly`, `ClientOnly`.
- `AddDataProtection().PersistKeysToFileSystem(...)`.
- `AddHttpContextAccessor()`.
- No `AddSignalR()` in Web — Web is a SignalR **client** of the Api hub. `ChatSignalRService` (scoped) wraps a per-user `HubConnection` built with `Microsoft.AspNetCore.SignalR.Client`; connection URL `{ApiBaseUrl}/hubs/chat` with access-token provider reading from the current user's SSR-cached JWT.
- `AddRateLimiter(...)` with "auth" fixed window.
- `AddMemoryCache()`.
- `AddLocalization()` + `Configure<RequestLocalizationOptions>(...)` (`en-GB`, `el-GR`).
- Typed clients — `AddScoped<SsrApiHandler>` then one `AddHttpClient<IXConsumer, XConsumer>(c => c.BaseAddress = new(apiBaseUrl)).AddHttpMessageHandler<SsrApiHandler>()` per consumer listed in §2c under `Consumers/`.
- Web-only service: `IAuthService`, `ITokenProvider`.

### `Fitmaniac.MAUI/MauiProgram.cs`
```csharp
builder.Services.AddSingleton<IAuthTokenStore, AuthTokenStore>();
builder.Services.AddTransient<AuthHttpHandler>();
builder.Services.AddHttpClient<IApiClient, ApiClient>(c =>
    c.BaseAddress = new(config["ApiBaseUrl"]!))
    .AddHttpMessageHandler<AuthHttpHandler>();

builder.Services.AddSingleton<IAuthService, Services.AuthService>();
builder.Services.AddSingleton<IWorkoutService, Services.WorkoutService>();
builder.Services.AddSingleton<IExerciseService, Services.ExerciseService>();
builder.Services.AddSingleton<IProgramService, Services.ProgramService>();
builder.Services.AddSingleton<IGoalService, Services.GoalService>();
builder.Services.AddSingleton<IMeasurementService, Services.MeasurementService>();
builder.Services.AddSingleton<IProgressService, Services.ProgressService>();
builder.Services.AddSingleton<ConnectivityService>();
builder.Services.AddSingleton<OfflineCacheService>();

// ViewModels - Transient
builder.Services.AddTransient<LoginViewModel>();
builder.Services.AddTransient<RegisterViewModel>();
builder.Services.AddTransient<HomeViewModel>();
builder.Services.AddTransient<WorkoutsViewModel>();
builder.Services.AddTransient<WorkoutDetailViewModel>();
builder.Services.AddTransient<WorkoutRunnerViewModel>();
builder.Services.AddTransient<ExercisesViewModel>();
builder.Services.AddTransient<ExerciseDetailViewModel>();
builder.Services.AddTransient<ProgramsViewModel>();
builder.Services.AddTransient<GoalsViewModel>();
builder.Services.AddTransient<MeasurementsViewModel>();
builder.Services.AddTransient<ProgressViewModel>();
builder.Services.AddTransient<ProfileViewModel>();
builder.Services.AddTransient<SettingsViewModel>();

// Pages - Transient
builder.Services.AddTransient<LoginPage>();
builder.Services.AddTransient<RegisterPage>();
builder.Services.AddTransient<HomePage>();
builder.Services.AddTransient<WorkoutsPage>();
builder.Services.AddTransient<WorkoutDetailPage>();
builder.Services.AddTransient<WorkoutRunnerPage>();
builder.Services.AddTransient<ExercisesPage>();
builder.Services.AddTransient<ExerciseDetailPage>();
builder.Services.AddTransient<ProgramsPage>();
builder.Services.AddTransient<GoalsPage>();
builder.Services.AddTransient<MeasurementsPage>();
builder.Services.AddTransient<ProgressPage>();
builder.Services.AddTransient<ProfilePage>();
builder.Services.AddTransient<SettingsPage>();
```

---

## 2l. Step-by-Step Scaffold Execution Order

All paths are relative to `C:\Users\GS\Desktop\ResetYourFutureToFitmaniac\Fitmaniac\` unless stated. Paths in commands use forward slashes where the shell accepts them. After each `dotnet new` step, the .csproj content is overwritten per §2b. After each entity/file step, content is per §2d / §2e / §2f / §2k.

> **Block A — Solution & project bootstrap**

1. `cd C:\Users\GS\Desktop\ResetYourFutureToFitmaniac\Fitmaniac`
2. Run: `dotnet new sln -n Fitmaniac`
3. Create file `global.json`:
   ```json
   { "sdk": { "version": "10.0.100", "rollForward": "latestFeature" } }
   ```
4. Create file `Directory.Build.props`:
   ```xml
   <Project>
     <PropertyGroup>
       <TargetFramework>net10.0</TargetFramework>
       <Nullable>enable</Nullable>
       <ImplicitUsings>enable</ImplicitUsings>
       <ProduceReferenceAssembly>true</ProduceReferenceAssembly>
       <AccelerateBuildsInVisualStudio>true</AccelerateBuildsInVisualStudio>
     </PropertyGroup>
   </Project>
   ```
5. Create file `.editorconfig` — standard C# conventions (4-space indent, file-scoped namespaces, LF endings).
6. Create file `.gitignore` — output of `dotnet new gitignore`.
7. Run: `mkdir src && mkdir mobile`
8. Run: `dotnet new classlib -n Fitmaniac.Domain -o src/Fitmaniac.Domain -f net10.0` — delete `Class1.cs`.
9. Run: `dotnet new classlib -n Fitmaniac.Application -o src/Fitmaniac.Application -f net10.0` — delete `Class1.cs`.
10. Run: `dotnet new classlib -n Fitmaniac.Infrastructure -o src/Fitmaniac.Infrastructure -f net10.0` — delete `Class1.cs`.
11. Run: `dotnet new classlib -n Fitmaniac.Shared -o src/Fitmaniac.Shared -f net10.0` — delete `Class1.cs`.
12. Run: `dotnet new webapi -n Fitmaniac.Api -o src/Fitmaniac.Api -f net10.0 --use-controllers false` — delete `WeatherForecast.cs` and template endpoints.
13. Run: `dotnet new blazor -n Fitmaniac.Web -o src/Fitmaniac.Web -f net10.0 --interactivity Server --auth None` — delete template pages under `Components/Pages/`.
14. Run: `dotnet workload install maui` *(skip if already installed)*.
15. Run: `dotnet new maui -n Fitmaniac.MAUI -o mobile/Fitmaniac.MAUI -f net10.0` — delete template `MainPage.xaml`.

> **Block B — Solution wiring**

16. Run: `dotnet sln Fitmaniac.sln add src/Fitmaniac.Domain/Fitmaniac.Domain.csproj --solution-folder src`
17. Repeat for: Fitmaniac.Application, Fitmaniac.Infrastructure, Fitmaniac.Shared, Fitmaniac.Api, Fitmaniac.Web — all `--solution-folder src`.
18. Run: `dotnet sln Fitmaniac.sln add mobile/Fitmaniac.MAUI/Fitmaniac.MAUI.csproj --solution-folder mobile`
19. Run inter-project references:
    ```
    dotnet add src/Fitmaniac.Application/Fitmaniac.Application.csproj reference src/Fitmaniac.Domain/Fitmaniac.Domain.csproj src/Fitmaniac.Shared/Fitmaniac.Shared.csproj
    dotnet add src/Fitmaniac.Infrastructure/Fitmaniac.Infrastructure.csproj reference src/Fitmaniac.Application/Fitmaniac.Application.csproj src/Fitmaniac.Domain/Fitmaniac.Domain.csproj src/Fitmaniac.Shared/Fitmaniac.Shared.csproj
    dotnet add src/Fitmaniac.Api/Fitmaniac.Api.csproj reference src/Fitmaniac.Application/Fitmaniac.Application.csproj src/Fitmaniac.Infrastructure/Fitmaniac.Infrastructure.csproj src/Fitmaniac.Domain/Fitmaniac.Domain.csproj src/Fitmaniac.Shared/Fitmaniac.Shared.csproj
    dotnet add src/Fitmaniac.Web/Fitmaniac.Web.csproj reference src/Fitmaniac.Application/Fitmaniac.Application.csproj src/Fitmaniac.Domain/Fitmaniac.Domain.csproj src/Fitmaniac.Shared/Fitmaniac.Shared.csproj
    dotnet add mobile/Fitmaniac.MAUI/Fitmaniac.MAUI.csproj reference src/Fitmaniac.Domain/Fitmaniac.Domain.csproj src/Fitmaniac.Shared/Fitmaniac.Shared.csproj
    ```

> **Block C — NuGet packages** (one `dotnet add package` per line; see §2b for full list)

20. In `src/Fitmaniac.Domain`: edit csproj to add `<FrameworkReference Include="Microsoft.AspNetCore.App" />` under an `<ItemGroup>`.
21. In `src/Fitmaniac.Application`:
    ```
    dotnet add package Microsoft.EntityFrameworkCore --version 10.0.5
    dotnet add package HtmlSanitizer --version 9.0.892
    ```
    Add `<FrameworkReference Include="Microsoft.AspNetCore.App" />` in csproj.
22. In `src/Fitmaniac.Infrastructure`:
    ```
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 10.0.5
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.5
    dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.5
    dotnet add package System.IdentityModel.Tokens.Jwt --version 8.3.1
    dotnet add package QuestPDF --version 2026.2.4
    ```
    Add `<FrameworkReference Include="Microsoft.AspNetCore.App" />` in csproj.
23. In `src/Fitmaniac.Api`:
    ```
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.5
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 10.0.5
    dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.5
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.5
    dotnet add package Microsoft.AspNetCore.OpenApi --version 10.0.5
    dotnet add package System.IdentityModel.Tokens.Jwt --version 8.3.1
    dotnet add package Swashbuckle.AspNetCore --version 7.2.0
    ```
    (SignalR is included in the `Microsoft.AspNetCore.App` framework reference via `Microsoft.NET.Sdk.Web` — no explicit package needed for the server-side hub.)
24. In `src/Fitmaniac.Web`:
    ```
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.5
    dotnet add package Microsoft.AspNetCore.SignalR.Client --version 10.0.5
    dotnet add package Microsoft.AspNetCore.OpenApi --version 10.0.5
    dotnet add package System.IdentityModel.Tokens.Jwt --version 8.3.1
    ```
25. In `mobile/Fitmaniac.MAUI`:
    ```
    dotnet add package CommunityToolkit.Maui --version 11.0.0
    dotnet add package CommunityToolkit.Mvvm --version 8.4.0
    dotnet add package Microsoft.Extensions.Http --version 10.0.0
    dotnet add package Microsoft.AspNetCore.SignalR.Client --version 10.0.5
    dotnet add package System.IdentityModel.Tokens.Jwt --version 8.3.1
    ```
    (`SignalR.Client` is required for the MAUI chat consumer that connects to the Api's `/hubs/chat` hub.)

> **Block D — Domain layer**

26. Create folders `src/Fitmaniac.Domain/Common`, `Entities`, `Enums`, `Exceptions`, `Extensions`.
27. Create each file listed in §2c under `Fitmaniac.Domain/` with the content given in §2d. Full class bodies in §2d. Enum bodies are listed verbatim there.
28. `Extensions/UserSearchExtensions.cs`:
    ```csharp
    namespace Fitmaniac.Domain.Extensions;
    public static class UserSearchExtensions
    {
        public static IQueryable<ApplicationUser> SearchByTerm(this IQueryable<ApplicationUser> q, string? term) =>
            string.IsNullOrWhiteSpace(term) ? q :
            q.Where(u => u.Email!.Contains(term) || u.UserName!.Contains(term));
    }
    ```
29. `Extensions/WorkoutSearchExtensions.cs`:
    ```csharp
    namespace Fitmaniac.Domain.Extensions;
    public static class WorkoutSearchExtensions
    {
        public static IQueryable<Workout> InRange(this IQueryable<Workout> q, DateTime? from, DateTime? to) =>
            q.Where(w => (!from.HasValue || w.ScheduledDateTime >= from) && (!to.HasValue || w.ScheduledDateTime <= to));
    }
    ```

> **Block E — Shared layer**

30. Create folders `src/Fitmaniac.Shared/DTOs/{Auth,Users,Trainers,Clients,Workouts,Exercises,Programs,Goals,Measurements,MedicalHistory,Subscriptions,Progress,Common}`, `Constants`, `Resources`, `Resources/Messages`, `JSON/{Exercises,Equipment,Programs,Trainers,Clients}`.
31. Create every DTO file listed in §2c with properties mirroring the §2f endpoint contracts + §2d entities. One example skeleton (applies verbatim to all DTOs — only the property list changes):
    ```csharp
    namespace Fitmaniac.Shared.DTOs.Auth;
    public sealed record LoginRequestDto(string UsernameOrEmail, string Password);
    public sealed record RegisterRequestDto(string Username, string Email, string Password, string Role);
    public sealed record AuthResponseDto(bool Success, string? AccessToken, DateTime? AccessTokenExpiresUtc, string[]? Errors);
    public sealed record AccessTokenDto(string AccessToken, DateTime AccessTokenExpiresUtc);
    public sealed record RefreshResponseDto(string AccessToken, DateTime AccessTokenExpiresUtc, string? RefreshToken, DateTime? RefreshTokenExpiresUtc);
    public sealed record ForgotPasswordRequestDto(string Email);
    public sealed record ResetPasswordRequestDto(string Email, string Token, string NewPassword);
    public sealed record ConfirmEmailRequestDto(string Email, string Token);
    public sealed record ChangePasswordRequestDto(string CurrentPassword, string NewPassword);
    ```
    Workouts example:
    ```csharp
    namespace Fitmaniac.Shared.DTOs.Workouts;
    public sealed record WorkoutListItemDto(int Id, DateTime ScheduledDateTime, string Type, int DurationInMinutes, int? TrainerId, int? WeeklyProgramId, bool CompletedByMe);
    public sealed record WorkoutDto(int Id, DateTime ScheduledDateTime, string Type, int DurationInMinutes, string? Notes, int? TrainerId, int? WeeklyProgramId, IReadOnlyList<WorkoutExerciseDto> Exercises, IReadOnlyList<int> ClientIds);
    public sealed record CreateWorkoutDto(IReadOnlyList<int> ClientIds, int? TrainerId, int? WeeklyProgramId, DateTime ScheduledDateTime, string Type, int DurationInMinutes, string? Notes);
    public sealed record UpdateWorkoutDto(IReadOnlyList<int>? ClientIds, int? TrainerId, int? WeeklyProgramId, DateTime? ScheduledDateTime, string? Type, int? DurationInMinutes, string? Notes);
    public sealed record WorkoutExerciseDto(int Id, int WorkoutId, int ExerciseDefinitionId, string ExerciseName, IReadOnlyList<WorkoutExerciseSetDto> Sets, string? Notes);
    public sealed record CreateWorkoutExerciseDto(int WorkoutId, int ExerciseDefinitionId, string? Notes);
    public sealed record UpdateWorkoutExerciseDto(int Id, string? Notes);
    public sealed record WorkoutExerciseSetDto(int Id, int WorkoutExerciseId, int SetNumber, int Repetitions, double? Weight, Fitmaniac.Domain.Enums.GoalUnit? GoalUnit, Fitmaniac.Domain.Enums.IntensityLevel OverallIntensityLevel, int DurationInSeconds, int RestPeriodInSeconds, string? Notes);
    public sealed record CreateWorkoutExerciseSetDto(int WorkoutExerciseId, int SetNumber, int Repetitions, double? Weight, Fitmaniac.Domain.Enums.GoalUnit? GoalUnit, Fitmaniac.Domain.Enums.IntensityLevel OverallIntensityLevel, int DurationInSeconds, int RestPeriodInSeconds, string? Notes);
    public sealed record UpdateWorkoutExerciseSetDto(int Id, int? SetNumber, int? Repetitions, double? Weight, Fitmaniac.Domain.Enums.GoalUnit? GoalUnit, Fitmaniac.Domain.Enums.IntensityLevel? OverallIntensityLevel, int? DurationInSeconds, int? RestPeriodInSeconds, string? Notes);
    ```
    Users:
    ```csharp
    namespace Fitmaniac.Shared.DTOs.Users;
    public sealed record UserDto(int Id, string Username, string Email, string Role, bool IsActive, DateTime CreatedUtc, Trainers.TrainerDto? TrainerProfile, Clients.ClientDto? ClientProfile);
    public sealed record CreateUserDto(string Username, string Email, string Password, string Role, bool IsActive = true);
    public sealed record UpdateUserDto(int Id, string? Username, string? Email, bool? IsActive, string? Role, string? Password);
    public sealed record UserStatisticsDto(int TotalUsers, int ActiveUsers, int InactiveUsers, int Admins, int Trainers, int Clients);
    ```
    Repeat the same discipline for all DTO categories listed in §2c — one record per file. The full property set per DTO is enumerated in FullStackTest catalog §3, migrated 1:1 with role strings replaced by `Fitmaniac.Domain.Enums.*` types where safe.
32. `Constants/RoleNames.cs`:
    ```csharp
    namespace Fitmaniac.Shared.Constants;
    public static class RoleNames { public const string Admin = "Admin"; public const string Trainer = "Trainer"; public const string Client = "Client"; }
    ```
33. `Constants/PolicyNames.cs`:
    ```csharp
    namespace Fitmaniac.Shared.Constants;
    public static class PolicyNames { public const string Admin = "Admin"; public const string Trainer = "Trainer"; public const string Client = "Client"; public const string StaffOrOwner = "StaffOrOwner"; }
    ```
34. `Constants/AuthConstants.cs`:
    ```csharp
    namespace Fitmaniac.Shared.Constants;
    public static class AuthConstants
    {
        public const string WebCookieName = ".FMC.Auth";
        public const string RefreshCookieName = "refreshToken";
        public const string MobileClientHeader = "X-Client";
        public const string MobileClientValue = "mobile";
        public const string AccessTokenClaim = "fitmaniac.access_token";
        public const string SubscriptionTierClaim = "subscriptionTier";
        public const string DataProtectionPurpose = "Fitmaniac.Web.AuthService.Ticket";
    }
    ```
35. `Constants/RouteConstants.cs` — every path from §2f as `public const string` (e.g. `public const string AuthLogin = "api/auth/login";`).
36. `Constants/SeedDefaults.cs`:
    ```csharp
    namespace Fitmaniac.Shared.Constants;
    public static class SeedDefaults
    {
        public const string AdminEmailFallback = "admin@fitmaniac.local";
        public const string AdminPasswordFallback = "Admin123!";
    }
    ```
37. `Constants/ClaimTypesExtended.cs` — additional claim type constants beyond standard.
38. Create empty `.resx` + `.el.resx` pairs for each resource file listed in §2c (Resources/). Each `.resx` contains a valid empty `<root>...</root>` XML header. Designer `.cs` files are generated by the tooling. Add csproj `ItemGroup` entries:
    ```xml
    <ItemGroup>
      <EmbeddedResource Update="Resources\GlobalRes.resx"><Generator>PublicResXFileCodeGenerator</Generator><LastGenOutput>GlobalRes.Designer.cs</LastGenOutput></EmbeddedResource>
      <EmbeddedResource Update="Resources\GlobalRes.el.resx"><DependentUpon>GlobalRes.resx</DependentUpon></EmbeddedResource>
      <!-- repeat for every .resx pair -->
    </ItemGroup>
    ```
39. Create `JSON/Exercises/exercises.core.json` with at least 20 bodyweight/core exercises (squat, push-up, plank, crunch, pull-up, burpee, lunge, mountain climber, hollow hold, Russian twist, glute bridge, jumping jack, bird dog, dead bug, deadlift, bent-over row, overhead press, bench press, kettlebell swing, farmer carry). Schema:
    ```json
    [{ "name": "Squat", "description": "...", "videoUrl": null, "caloriesBurnedPerHour": 350, "isCompoundExercise": true, "primaryMuscleGroups": ["Legs","Glutes"], "secondaryMuscleGroups": ["Core"], "experienceLevel": "Beginner", "category": "Strength", "equipment": [] }]
    ```
40. `JSON/Equipment/equipment.json`: Barbell, Dumbbell, Kettlebell, Pull-up Bar, Bench, Squat Rack, Resistance Band, Medicine Ball, Yoga Mat, Treadmill, Rowing Machine, Jump Rope, Cable Machine.
41. `JSON/Programs/sample-programs.json`: two starter programs (4-week Full-Body Beginner; 6-week Upper/Lower Split).
42. `JSON/Trainers/sample-trainers.json` and `JSON/Clients/sample-clients.json`: seed five of each with plausible personal-information fields.
43. Mark all JSON files as `EmbeddedResource` in csproj:
    ```xml
    <ItemGroup><EmbeddedResource Include="JSON\**\*.json" /></ItemGroup>
    ```

> **Block F — Application layer**

44. Create folders `src/Fitmaniac.Application/{Data,Common,Abstractions,Services/...,Mapping}`.
45. `Data/IApplicationDbContext.cs` — interface with `DbSet<T>` for every entity listed in §2e plus `Task<int> SaveChangesAsync(CancellationToken ct = default);`.
46. `Common/ServiceResult.cs`:
    ```csharp
    namespace Fitmaniac.Application.Common;
    public sealed record ServiceResult<T>(T? Value, int StatusCode, string? ErrorMessage)
    {
        public bool IsSuccess => StatusCode is >= 200 and < 300;
        public static ServiceResult<T> Ok(T v) => new(v, 200, null);
        public static ServiceResult<T> Created(T v) => new(v, 201, null);
        public static ServiceResult<T> NoContent() => new(default, 204, null);
        public static ServiceResult<T> BadRequest(string m) => new(default, 400, m);
        public static ServiceResult<T> NotFound(string m = "Not found") => new(default, 404, m);
        public static ServiceResult<T> Forbidden(string m = "Forbidden") => new(default, 403, m);
        public static ServiceResult<T> Conflict(string m) => new(default, 409, m);
    }
    ```
47. `Common/PagedResult.cs`, `Common/PaginationRequest.cs`, `Common/SortRequest.cs` — straightforward record types.
48. `Abstractions/ICurrentUserService.cs`:
    ```csharp
    public interface ICurrentUserService { int? UserId { get; } string? UserName { get; } IReadOnlyList<string> Roles { get; } bool IsInRole(string role); }
    ```
49. `Abstractions/ITokenService.cs`, `IFileStorage.cs`, `IEmailService.cs`, `IDateTimeProvider.cs` — signatures per §2k.
50. Service interface files (every `I*Service.cs` listed in §2c under `Application/Services/`) — define one method per endpoint in §2f plus any supporting helper.
51. `Mapping/IModelToDtoMapper.cs` — method signatures `UserDto? ToDto(ApplicationUser?)`, `TrainerDto? ToDto(Trainer?)`, `ClientDto? ToDto(Client?)`, `WorkoutDto? ToDto(Workout?)`, and so on for every entity/DTO pair.

> **Block G — Infrastructure layer**

52. Create folders `src/Fitmaniac.Infrastructure/{Persistence,Persistence/Configurations,Persistence/Migrations,Interceptors,Identity,Services,Seeders,DependencyInjection}`.
53. `Persistence/FitmaniacDbContext.cs` — implements `IApplicationDbContext`, extends `IdentityDbContext<ApplicationUser, IdentityRole<int>, int>`, `DbSet<T>` properties for all entities, `OnModelCreating` → `ApplyConfigurationsFromAssembly(typeof(FitmaniacDbContext).Assembly)`, applies interceptors via `OnConfiguring`.
54. `Persistence/DesignTimeDbContextFactory.cs` — `IDesignTimeDbContextFactory<FitmaniacDbContext>` using LocalDB `Server=(localdb)\MSSQLLocalDB;Database=FitmaniacDb.Design;Trusted_Connection=True;TrustServerCertificate=True;`.
55. Each `Persistence/Configurations/*Configuration.cs` — one class per entity, content per §2e.
56. `Interceptors/AuditSaveChangesInterceptor.cs` — overrides `SavingChangesAsync`, loops `ChangeTracker.Entries<IAuditable>()`, stamps audit fields using `ICurrentUserService`.
57. `Interceptors/SoftDeleteInterceptor.cs` — converts deletes on `AuditableEntity` into `IsDeleted=true, DeletedUtc=now`.
58. `Services/*.cs` — one implementation per interface. Skeletons follow the RYF + FST patterns; every method calls `IApplicationDbContext` for data access and returns `ServiceResult<T>`. Example for `AuthService`:
    ```csharp
    public sealed class AuthService(UserManager<ApplicationUser> users, RoleManager<IdentityRole<int>> roles, ITokenService tokens, IHttpContextAccessor http, IRefreshTokenCookieService cookies) : IAuthService { /* Register, Login, Refresh, Logout, Forgot/Reset */ }
    ```
    Endpoints in §2f define the public surface; the implementation logic mirrors FullStackTest's `AuthService`/`JwtService` with rotation + hash + mobile-vs-web refresh-transport split.
59. `Seeders/*.cs` — logic per §2e.
60. `DependencyInjection/InfrastructureServiceCollectionExtensions.cs` — verbatim from §2k.

> **Block H — Api project**

61. Replace `src/Fitmaniac.Api/Program.cs` with the full top-level `Program.cs`:
    ```csharp
    using Fitmaniac.Api.Endpoints;
    using Fitmaniac.Api.Middleware;
    using Fitmaniac.Application.Abstractions;
    using Fitmaniac.Infrastructure.DependencyInjection;
    using Fitmaniac.Infrastructure.Persistence;
    using Fitmaniac.Infrastructure.Seeders;
    using Fitmaniac.Shared.Constants;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using System.Text.Json.Serialization;

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.Configure<ForwardedHeadersOptions>(o =>
    {
        o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        o.KnownNetworks.Clear();
        o.KnownProxies.Clear();
    });

    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    builder.Services.AddCors(o => o.AddPolicy("AllowFrontend", p => p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

    builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddFitmaniacInfrastructure(builder.Configuration);

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy(PolicyNames.Admin,   p => p.RequireRole(RoleNames.Admin))
        .AddPolicy(PolicyNames.Trainer, p => p.RequireRole(RoleNames.Trainer))
        .AddPolicy(PolicyNames.Client,  p => p.RequireRole(RoleNames.Client));

    builder.Services.AddRateLimiter(o =>
    {
        o.AddFixedWindowLimiter("login",   c => { c.PermitLimit = 5;  c.Window = TimeSpan.FromMinutes(1); c.QueueLimit = 2; });
        o.AddFixedWindowLimiter("refresh", c => { c.PermitLimit = 20; c.Window = TimeSpan.FromMinutes(1); });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    // Migrate + seed
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<FitmaniacDbContext>();
        await db.Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<DatabaseSeederOrchestrator>().SeedAsync();
    }

    app.UseForwardedHeaders();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseHttpsRedirection();
    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseMiddleware<DisabledUserMiddleware>();
    app.UseAuthorization();
    app.UseRateLimiter();
    app.UseStaticFiles();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapGet("/", () => "Fitmaniac API — running");
    var api = app.MapGroup("/api");
    api.MapAuthEndpoints();
    api.MapUserProfileEndpoints();
    api.MapAdminUserEndpoints();
    api.MapTrainerEndpoints();
    api.MapAdminTrainerEndpoints();
    api.MapClientEndpoints();
    api.MapWorkoutEndpoints();
    api.MapAdminWorkoutEndpoints();
    api.MapExerciseDefinitionEndpoints();
    api.MapEquipmentEndpoints();
    api.MapWeeklyProgramEndpoints();
    api.MapGoalEndpoints();
    api.MapMeasurementEndpoints();
    api.MapMedicalHistoryEndpoints();
    api.MapSubscriptionEndpoints();
    api.MapBillingEndpoints();
    api.MapProgressEndpoints();
    api.MapMediaEndpoints();
    api.MapChatEndpoints();

    app.MapHub<ChatHub>("/hubs/chat").RequireAuthorization();

    app.Run();
    ```
62. Each `Endpoints/*.cs` — one static class per group, `public static IEndpointRouteBuilder Map*Endpoints(this IEndpointRouteBuilder e)`. Body maps each verb per §2f to the corresponding application service method, wiring DTO bindings from body/query, `[Authorize]` and `.RequireAuthorization(policy)` where stated.
    Example for `AuthEndpoints.cs`:
    ```csharp
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder e)
        {
            var g = e.MapGroup("/auth");
            g.MapPost("/register", async (RegisterRequestDto dto, IAuthService svc) => (await svc.RegisterAsync(dto)).ToResult()).AllowAnonymous();
            g.MapPost("/login",    async (LoginRequestDto dto, IAuthService svc) => (await svc.LoginAsync(dto)).ToResult()).AllowAnonymous().RequireRateLimiting("login");
            g.MapPost("/refresh",  async (HttpContext ctx, IAuthService svc) => (await svc.RefreshAsync(ctx)).ToResult()).AllowAnonymous().RequireRateLimiting("refresh");
            g.MapPost("/logout",   async (HttpContext ctx, IAuthService svc) => (await svc.LogoutAsync(ctx)).ToResult()).RequireAuthorization();
            g.MapPost("/forgot-password", async (ForgotPasswordRequestDto dto, IAuthService svc) => (await svc.ForgotPasswordAsync(dto)).ToResult()).AllowAnonymous();
            g.MapPost("/reset-password", async (ResetPasswordRequestDto dto, IAuthService svc) => (await svc.ResetPasswordAsync(dto)).ToResult()).AllowAnonymous();
            g.MapPost("/confirm-email", async (ConfirmEmailRequestDto dto, IAuthService svc) => (await svc.ConfirmEmailAsync(dto)).ToResult()).AllowAnonymous();
            g.MapPost("/change-password", async (ChangePasswordRequestDto dto, IAuthService svc) => (await svc.ChangePasswordAsync(dto)).ToResult()).RequireAuthorization();
            return e;
        }
    }
    ```
    `ServiceResult<T>.ToResult()` is an extension in `Endpoints/IEndpointRouteBuilderExtensions.cs` that maps status code + body to `Results.*`.
63. `Middleware/ExceptionHandlingMiddleware.cs` — catches unhandled exceptions, returns `ProblemDetails`.
64. `Middleware/DisabledUserMiddleware.cs` — if `HttpContext.Items["UserDisabled"] == true`, returns 403 with `X-User-Disabled: 1`.
65. `appsettings.json`:
    ```json
    {
      "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
      "AllowedHosts": "*",
      "ConnectionStrings": { "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=FitmaniacDb;Trusted_Connection=True;TrustServerCertificate=True;" },
      "Cors": { "AllowedOrigins": [ "http://localhost:5173", "https://localhost:7090" ] },
      "Jwt": {
        "Key": "",
        "Issuer": "Fitmaniac.Api",
        "Audience": "Fitmaniac.Client",
        "AccessTokenMinutes": 30,
        "RefreshTokenDays": 7
      },
      "AdminUser": { "Email": "admin@fitmaniac.local", "Password": "" },
      "SeedSampleData": false
    }
    ```
66. `appsettings.Development.json`: override `Jwt.Key` (dev-only secret), `AdminUser.Password = "Admin123!"`, `SeedSampleData: true`.
67. `Properties/launchSettings.json`: HTTPS `https://localhost:7301`, HTTP `http://localhost:5301`, `ASPNETCORE_ENVIRONMENT=Development`.
68. Run initial migration:
    ```
    cd src/Fitmaniac.Api
    dotnet ef migrations add InitialCreate -p ../Fitmaniac.Infrastructure -s . -o Persistence/Migrations
    dotnet ef database update -p ../Fitmaniac.Infrastructure -s .
    ```

> **Block I — Web project**

69. Replace `src/Fitmaniac.Web/Program.cs` with the Blazor host wiring: Razor components + InteractiveServer; cookie + JWT multi-auth via `PolicyScheme("MultiAuth")`; DataProtection persistence; request localization; `/auth/complete` and `/auth/signout` minimal endpoints (mirror RYF lines 397+ per the catalog). Consumers registered via `AddHttpClient<...>().AddHttpMessageHandler<SsrApiHandler>()`.
70. `Components/App.razor`, `Routes.razor`, `_Imports.razor` — copy RYF templates per §2g (adapt namespaces).
71. `Components/Layout/*.razor(.css)` — copy MainLayout/NavMenu/AvatarDropdown/CultureSelector and adapt nav entries to Fitmaniac's routes.
72. `Components/Pages/*.razor` — create each page listed in §2c. Each page begins with `@page "/route"`, `@attribute [Authorize(Roles="…")]` where applicable, an injected `I*Consumer` for API calls, and body HTML matching RYF's shared-component patterns.
73. `Components/Shared/*.razor` — copy RYF shared components (Data, Forms, Layout subfolders) per the **Copy as-is** / **Adapt** decisions in §2g. Create the new fitness-specific components: `WorkoutCard.razor`, `ExerciseCard.razor`, `SetInputRow.razor`, `GoalCard.razor`, `MeasurementChart.razor`, `BmiIndicator.razor`.
74. `Consumers/*.cs` — each `I*Consumer` + `*Consumer` pair. Base: `ApiClientBase` with `protected HttpClient Http`. Each consumer is a thin wrapper over `Fitmaniac.Api` endpoints. `SsrApiHandler.cs` copied from RYF with constant renames (`ResetYourFuture.Web.AuthService.Ticket` → `Fitmaniac.Web.AuthService.Ticket`; cookie name `.RYF.Auth` → `.FMC.Auth`).
75. `Services/AuthService.cs` + `Interfaces/ITokenProvider.cs` — adapt from RYF, point to `Fitmaniac.Api` base URL via config `ApiBaseUrl`.
76. `Logging/FileLogger*.cs` — copy RYF file logger.
77. `wwwroot/` — copy `lib/bootstrap/...`, then author `css/app.css` (new palette variables), `css/shared-components.css`, `js/quill-interop.js`, `js/chart-interop.js`, `js/workout-timer-interop.js`, `images/logo.svg`, `favicon.png`.
78. `appsettings.json`:
    ```json
    {
      "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
      "AllowedHosts": "*",
      "ApiBaseUrl": "https://localhost:7301",
      "SelfBaseUrl": "https://localhost:7090"
    }
    ```
79. `appsettings.Development.json` — same, plus `"DataProtection": { "KeyPath": "DataProtection-Keys" }`.
80. `Properties/launchSettings.json` — HTTPS `https://localhost:7090`, HTTP `http://localhost:5090`.

> **Block J — MAUI project**

81. Replace `mobile/Fitmaniac.MAUI/MauiProgram.cs` with content per §2k (MAUI section).
82. `AppShell.xaml` — Shell flyout with tabs per §2h.
83. Each `Pages/*.xaml(.cs)` — pages per §2h with `BindingContext = {binding path from VM}`.
84. Each `ViewModels/*.cs` — `ObservableObject` + `RelayCommand` methods using `CommunityToolkit.Mvvm`.
85. `Services/ApiClient.cs` + `AuthHttpHandler.cs` + `AuthTokenStore.cs` per §2h + §2i.
86. `Services/AuthService.cs`, `WorkoutService.cs`, etc. — one per backend endpoint group; each uses `IApiClient` and returns DTOs from `Fitmaniac.Shared.DTOs.*`.
87. `Converters/*.cs` — value converters listed in §2c.
88. `appsettings.json` inside MAUI — embedded via `MauiAsset` and loaded in `MauiProgram.cs`:
    ```json
    { "ApiBaseUrl": "https://10.0.2.2:7301" }
    ```
    (Android emulator localhost alias; iOS simulator uses `https://localhost:7301`.)

> **Block K — Post-scaffold verification**

89. Run: `dotnet restore`
90. Run: `dotnet build Fitmaniac.sln`
91. Run: `dotnet run --project src/Fitmaniac.Api/Fitmaniac.Api.csproj` — confirm migrations apply, seeders run, Swagger loads at `/swagger`.
92. In a second terminal: `dotnet run --project src/Fitmaniac.Web/Fitmaniac.Web.csproj` — confirm home page renders and login hits Api.
93. Run: `dotnet build mobile/Fitmaniac.MAUI/Fitmaniac.MAUI.csproj -f net10.0-windows10.0.19041.0` — confirm MAUI compiles on at least one platform.
94. Commit initial scaffold.

---

## Summary of `[NEEDS CLARIFICATION]` items

1. Fitmaniac brand palette (§2j) — default orange/teal applied; override on request.
2. `Swashbuckle.AspNetCore` vs `AddOpenApi` alone (§2b Api) — both wired; simplify later.
3. `Microsoft.Maui.Controls.Maps` (§2b MAUI) — retained pending decision on outdoor activity tracking.
4. Testimonials admin pages (§2g) — excluded from v1.
5. Identity `RequireConfirmedEmail` in prod (§2k) — off in dev, needs explicit prod policy.

### Resolved by user directive (2026-04-17)

- **Database provider:** SQL Server only (LocalDB for dev). SQLite removed from Infrastructure + Api packages, §2e DbContext, and `appsettings.json`.
- **Chat / SignalR:** Reused from ResetYourFuture. `ChatHub` hosted in `Fitmaniac.Api` at `/hubs/chat`; Web + MAUI are clients. Entities, DTOs, endpoints, hub, page, components, and consumer wiring documented in §2c, §2d, §2e, §2f, §2g, §2k, §2l.
- **QuestPDF:** Retained and retargeted to Fitness Programs (weekly program export PDFs). `IProgramPdfService` in Infrastructure, consumed via `WeeklyProgramEndpoints`. Not used for certificates.
- **BCrypt:** Removed. Primary hashing is ASP.NET Core Identity's built-in `PasswordHasher<TUser>`. No legacy-hash compatibility path is required for a greenfield fitness app.

All items have documented defaults so `2l` runs unblocked.
